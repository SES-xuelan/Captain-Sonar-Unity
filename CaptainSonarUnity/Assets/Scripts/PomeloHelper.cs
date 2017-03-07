using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using SimpleJson;
using Pomelo.DotNetClient;

public class PomeloHelper:MonoBehaviour
{
    /*
	流程：
	①连接gate服务器，获取chat服务器地址和端口；
	②连接chat服务器；
	③正常通信；
	*/

    public class Route
    {
        public const string GATE_ROUTE = "gate.gateHandler.queryEntry";
        public const string CONNECTOR_ROUTE = "connector.entryHandler.enter";
        public const string CHAT_ROUTE = "chat.chatHandler.send";
    }

    private static PomeloHelper instance;
    private static Connection pc;


    public static string chatServerHost;
    public static int chatServerPort;

    private static Dictionary<string ,Action<Message>> disconnectListeners;
    private static Dictionary<string ,Action<Message>> errorListeners;
    private static Dictionary<string ,Action<Message>> chatListeners;
    private static Dictionary<string ,Action<Message>> userAddedListeners;
    private static Dictionary<string ,Action<Message>> userLeaveListeners;

    private static Dictionary<string,Action<Message>> systemListeners;

    public static bool isChatServerReady = false;

    static PomeloHelper ()
    {
        Init ();
    }

    private static void Init ()
    {
        if (PomeloHelper.instance == null) {
            var g = new GameObject ("PomeloHelper", typeof(PomeloHelper));
            PomeloHelper.instance = g.AddComponent<PomeloHelper> ();
            chatServerHost = null;
            chatServerPort = 0;
            disconnectListeners = new Dictionary<string, Action<Message>> ();
            errorListeners = new Dictionary<string, Action<Message>> ();
            chatListeners = new Dictionary<string, Action<Message>> ();
            userAddedListeners = new Dictionary<string, Action<Message>> ();
            userLeaveListeners = new Dictionary<string, Action<Message>> ();
            systemListeners = new Dictionary<string, Action<Message>> ();
        }
    }

    private static bool isPomeloConnected { get { return (pc != null && pc.netWorkState == NetWorkState.CONNECTED); } }
    // get chat server host & port
    private static void ConnectGateServer (string serverHost, int port, string userName, Action callback)
    {
        if (isChatServerReady) {
            if (callback != null)
                callback ();
            return;
        }
        pc = new Connection ();
        pc.on (Connection.DisconnectEvent, (msg) => {
            Debug.Log ("Network error, reason: " + msg.ToString ());
            if (disconnectListeners.Count > 0) {
                foreach (Action<Message> cb in disconnectListeners.Values) {
                    cb (msg);
                }
            }
        });
        pc.on (Connection.ErrorEvent, (msg) => {
            Debug.Log ("Error, reason: " + msg.ToString ());
            if (errorListeners.Count > 0) {
                foreach (Action<Message> cb in errorListeners.Values) {
                    cb (msg);
                }
            }
        });
        pc.InitClient (serverHost, port, (_dat) => {
            pc.connect (null, (_data) => {
				
                JsonObject msg = new JsonObject ();
                msg ["uid"] = userName;
                pc.request (Route.GATE_ROUTE, msg, (data) => {
                    Debug.Log ("OnQuery " + data.ToString ());
                    JsonObject result = data.jsonObj; 
                    if (Convert.ToInt32 (result ["code"]) == 200) {
                        pc.Disconnect ();
                        pc = null;

                        chatServerHost = (string)result ["host"];
                        chatServerPort = Convert.ToInt32 (result ["port"]);
                        isChatServerReady = true;
                        if (callback != null)
                            callback ();
                    }
                });
            });
        });

    }





    // enter chat server
    public static void EnterChatServer (string userName, string room, Action<Message> callback)
    {
        if (!isChatServerReady) {
            Debug.LogError ("chat server is not ready, please give host and port");
            return;
        }
        if (isPomeloConnected) {
            pc.Disconnect ();
        }
        pc = null;
        pc = new Connection ();


        //listen on network state changed event
        pc.on (Connection.DisconnectEvent, (msg) => {
            if (disconnectListeners.Count > 0) {
                foreach (Action<Message> cb in disconnectListeners.Values) {
                    cb (msg);
                }
            }
        });
        pc.on (Connection.ErrorEvent, (msg) => {
            if (errorListeners.Count > 0) {
                foreach (Action<Message> cb in errorListeners.Values) {
                    cb (msg);
                }
            }
        });
        pc.on (Connection.SystemEvent, (msg) => {
            if (systemListeners.Count > 0) {
                foreach (Action<Message> cb in systemListeners.Values) {
                    cb (msg);
                }
            }

        });
        pc.on (Connection.ChatEvent, (msg) => {
            if (chatListeners.Count > 0) {
                foreach (Action<Message> cb in chatListeners.Values) {
                    cb (msg);
                }
            }
        });
        pc.on (Connection.UserAddEvent, (msg) => {
            if (userAddedListeners.Count > 0) {
                foreach (Action<Message> cb in userAddedListeners.Values) {
                    cb (msg);
                }
            }

        });
        pc.on (Connection.UserLeaveEvent, (msg) => {
            if (userLeaveListeners.Count > 0) {
                foreach (Action<Message> cb in userLeaveListeners.Values) {
                    cb (msg);
                }
            }

        });


        pc.InitClient (chatServerHost, chatServerPort, (_dat) => {
            pc.connect (null, (_data) => {
                //Login
                JsonObject msg = new JsonObject ();
                msg ["username"] = userName;
                msg ["rid"] = room;

                pc.request (Route.CONNECTOR_ROUTE, msg, callback);

            });
        });
    }
		
    // enter chat server
    public static void EnterChatServer (string serverHost, int port, string userName, string room, Action<Message> callback)
    {
        if (!isChatServerReady) {
            ConnectGateServer (serverHost, port, userName, () => {
                EnterChatServer (userName, room, callback);
            });
            return;
        }
        EnterChatServer (userName, room, callback);
    }


    public static void SendMessageToServer (string route, JsonObject jsonObj, Action<Message> callback)
    {
        if (!isPomeloConnected) {
            Debug.LogError ("SendMessageToServer error: chat server is not connected!");
            return;
        }
        pc.request (route, jsonObj, callback);
    }

    public static void SendMessageToServer (string route, string room, string content, string fromUser, string toUser, Action<Message> callback)
    {
        JsonObject jsn = new JsonObject ();
        jsn.Add ("rid", room);
        jsn.Add ("target", toUser);
        jsn.Add ("content", content);
        jsn.Add ("from", fromUser);
        SendMessageToServer (route, jsn, callback);
    }

    public static void SendMessageToChatServer (JsonObject jsonObj, Action<Message> callback)
    {
        if (!isPomeloConnected) {
            Debug.LogError ("SendMessageToChatServer error: chat server is not connected!");
            return;
        }
        SendMessageToServer (Route.CHAT_ROUTE, jsonObj, callback);
    }

    public static void SendMessageToChatServer (string room, string content, string fromUser, string toUser, Action<Message> callback)
    {
        if (!isPomeloConnected) {
            Debug.LogError ("SendMessageToChatServer error: chat server is not connected!");
            return;
        }
        SendMessageToServer (Route.CHAT_ROUTE, room, content, fromUser, toUser, callback);
    }


    //返回Add lestener 是否成功
    public static bool AddDisconnectListener (string listenerName, Action<Message> listener, bool overrideIfListenerExist = true)
    {
        if (disconnectListeners.ContainsKey (listenerName)) {
            if (overrideIfListenerExist) {
                disconnectListeners.Remove (listenerName);
            } else {
                //listener already exist
                return false;
            }
        }
        disconnectListeners.Add (listenerName, listener);
        return true;
    }

    //返回Add lestener 是否成功
    public static bool AddErrorListener (string listenerName, Action<Message> listener, bool overrideIfListenerExist = true)
    {
        if (errorListeners.ContainsKey (listenerName)) {
            if (overrideIfListenerExist) {
                errorListeners.Remove (listenerName);
            } else {
                //listener already exist
                return false;
            }
        }
        errorListeners.Add (listenerName, listener);
        return true;
    }

    //返回Add lestener 是否成功
    public static bool AddSystemListener (string listenerName, Action<Message> listener, bool overrideIfListenerExist = true)
    {
        if (systemListeners.ContainsKey (listenerName)) {
            if (overrideIfListenerExist) {
                systemListeners.Remove (listenerName);
            } else {
                //listener already exist
                return false;
            }
        }
        systemListeners.Add (listenerName, listener);
        return true;
    }
    //返回Add lestener 是否成功
    public static bool AddChatListener (string listenerName, Action<Message> listener, bool overrideIfListenerExist = true)
    {
        if (chatListeners.ContainsKey (listenerName)) {
            if (overrideIfListenerExist) {
                chatListeners.Remove (listenerName);
            } else {
                //listener already exist
                return false;
            }
        }
        chatListeners.Add (listenerName, listener);
        return true;
    }
		
    //返回Add lestener 是否成功
    public static bool AddUserAddListener (string listenerName, Action<Message> listener, bool overrideIfListenerExist = true)
    {
        if (userAddedListeners.ContainsKey (listenerName)) {
            if (overrideIfListenerExist) {
                userAddedListeners.Remove (listenerName);
            } else {
                //listener already exist
                return false;
            }
        }
        userAddedListeners.Add (listenerName, listener);
        return true;
    }
		
    //返回Add lestener 是否成功
    public static bool AddUserLeaveListener (string listenerName, Action<Message> listener, bool overrideIfListenerExist = true)
    {
        if (userLeaveListeners.ContainsKey (listenerName)) {
            if (overrideIfListenerExist) {
                userLeaveListeners.Remove (listenerName);
            } else {
                //listener already exist
                return false;
            }
        }
        userLeaveListeners.Add (listenerName, listener);
        return true;
    }

    public static void  RemoveDisconnectListener (string listenerName)
    {
        if (disconnectListeners.ContainsKey (listenerName)) {
            disconnectListeners.Remove (listenerName);
        }
    }

    public static void  RemoveErrorListener (string listenerName)
    {
        if (errorListeners.ContainsKey (listenerName)) {
            errorListeners.Remove (listenerName);
        }
    }

    public static void  RemoveSystemListener (string listenerName)
    {
        if (systemListeners.ContainsKey (listenerName)) {
            systemListeners.Remove (listenerName);
        }
    }

    public static void  RemoveChatListener (string listenerName)
    {
        if (chatListeners.ContainsKey (listenerName)) {
            chatListeners.Remove (listenerName);
        }
    }

    public static void  RemoveUserAddListener (string listenerName)
    {
        if (userAddedListeners.ContainsKey (listenerName)) {
            userAddedListeners.Remove (listenerName);
        }
    }

    public static void  RemoveUserLeaveListener (string listenerName)
    {
        if (userLeaveListeners.ContainsKey (listenerName)) {
            userLeaveListeners.Remove (listenerName);
        }
    }

    /// <summary>
    /// 移除所有监听事件
    /// </summary>
    public static void RemoveAllListeners ()
    {
        disconnectListeners.Clear ();
        errorListeners.Clear ();
        systemListeners.Clear ();
        chatListeners.Clear ();
        userAddedListeners.Clear ();
        userLeaveListeners.Clear ();
    }

    /// <summary>
    /// 移除各个事件中 name 为 listenerName 的事件
    /// </summary>
    /// <param name="listenerName">Listener name.</param>
    public static void RemoveAllListenersWithName (string listenerName)
    {
        RemoveDisconnectListener (listenerName);
        RemoveErrorListener (listenerName);
        RemoveSystemListener (listenerName);
        RemoveChatListener (listenerName);
        RemoveUserAddListener (listenerName);
        RemoveUserLeaveListener (listenerName);
    }


    public static void Disconnect ()
    {
        if (isPomeloConnected) {
            pc.Disconnect ();
        }
    }

    void Awake ()
    {
        if (instance != null) {
            Destroy (this);
            return;
        }

        instance = this;
    }


    // Update is called once per frame
    void Update ()
    {
        if (isPomeloConnected) {
            pc.Update ();
        }
    }
}