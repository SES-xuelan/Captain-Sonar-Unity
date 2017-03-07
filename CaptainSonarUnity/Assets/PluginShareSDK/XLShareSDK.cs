using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using XLAF.Public;

// 注意，使用shareSDK时，需要有回调的activity，否则一些平台无法回调
//【Assets/Plugins/Android/ShareSDK/libs/ShareSDKCallback.jar】中的package name要和你的程序名一致
//【Assets/Plugins/Android/ShareSDK/AndroidManifest.xml】中的package 要和你的程序名一致
// 具体请看这两个文件
using System;
using System.Reflection;


public class XLShareSDK : MonoBehaviour
{
    // ShareSDK appId: 19fca0bee5266

    // WeChat:
    //app_id : wx50697dfa9a2aa176
    //app secret : f81ee51b24b1bbbd69ede6b5d18d7919
    private static ShareSDK ssdk;
    private static XLShareSDK instance;

    static XLShareSDK ()
    {
    }

    void Start ()
    {
        ssdk = GameObject.FindObjectOfType<ShareSDK> ();
        instance = GameObject.FindObjectOfType<XLShareSDK> ();
        ssdk.authHandler = instance.OnAuthResultHandler;
        ssdk.shareHandler = instance.OnShareResultHandler;
        ssdk.showUserHandler = instance.OnGetUserInfoResultHandler;
        ssdk.getFriendsHandler = instance.OnGetFriendsResultHandler;
        ssdk.followFriendHandler = instance.OnFollowFriendResultHandler;
    }



    /// <summary>
    /// 调用Init会触发构造函数，可以用于统一初始化的时候
    /// </summary>
    public static void Init ()
    {

    }

    void OnAuthResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success) {
            Log.Debug ("authorize success !" + "Platform :" + type);
        } else if (state == ResponseState.Fail) {
            #if UNITY_ANDROID
            Log.Debug ("fail! throwable stack = " + result ["stack"] + "; error msg = " + result ["msg"]);
            #elif UNITY_IPHONE
            Log.Debug ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
            #endif
        } else if (state == ResponseState.Cancel) {
            Log.Debug ("cancel !");
        }
    }

    void OnGetUserInfoResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success) {
            Log.Debug ("get user info result :");
            Log.Debug (MiniJSON.jsonEncode (result));
            Log.Debug ("Get userInfo success !Platform :" + type);
        } else if (state == ResponseState.Fail) {
            #if UNITY_ANDROID
            Log.Debug ("fail! throwable stack = " + result ["stack"] + "; error msg = " + result ["msg"]);
            #elif UNITY_IPHONE
            Log.Debug ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
            #endif
        } else if (state == ResponseState.Cancel) {
            Log.Debug ("cancel !");
        }
    }

    void OnShareResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success) {
            Log.Debug ("share successfully - share result :");
            Log.Debug (MiniJSON.jsonEncode (result));
        } else if (state == ResponseState.Fail) {
            #if UNITY_ANDROID
            Log.Debug ("fail! throwable stack = " + result ["stack"] + "; error msg = " + result ["msg"]);
            #elif UNITY_IPHONE
            Log.Debug ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
            #endif
        } else if (state == ResponseState.Cancel) {
            Log.Debug ("cancel !");
        }
    }

    void OnGetFriendsResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success) {           
            Log.Debug ("get friend list result :");
            Log.Debug (MiniJSON.jsonEncode (result));
        } else if (state == ResponseState.Fail) {
            #if UNITY_ANDROID
            Log.Debug ("fail! throwable stack = " + result ["stack"] + "; error msg = " + result ["msg"]);
            #elif UNITY_IPHONE
            Log.Debug ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
            #endif
        } else if (state == ResponseState.Cancel) {
            Log.Debug ("cancel !");
        }
    }

    void OnFollowFriendResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success) {
            Log.Debug ("Follow friend successfully !");
        } else if (state == ResponseState.Fail) {
            #if UNITY_ANDROID
            Log.Debug ("fail! throwable stack = " + result ["stack"] + "; error msg = " + result ["msg"]);
            #elif UNITY_IPHONE
            Log.Debug ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
            #endif
        } else if (state == ResponseState.Cancel) {
            Log.Debug ("cancel !");
        }
    }


    public static void ShareText (PlatformType pt)
    {
        ShareContent content = new ShareContent ();
        content.SetText ("this is a test string.");
//        content.SetImageUrl ("https://f1.webshare.mob.com/code/demo/img/1.jpg");
        content.SetTitle ("test title");
//        content.SetTitleUrl ("http://www.mob.com");
//        content.SetSite ("Mob-ShareSDK");
//        content.SetSiteUrl ("http://www.mob.com");
//        content.SetUrl ("http://www.mob.com");
        content.SetComment ("test description");
//        content.SetMusicUrl ("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
        content.SetShareType (ContentType.Text);
        ssdk.ShowShareContentEditor (pt, content);
    }

    public static void ShareTextList ()
    {
        ShareContent content = new ShareContent ();
        content.SetText ("this is a test string.");
        //        content.SetImageUrl ("https://f1.webshare.mob.com/code/demo/img/1.jpg");
        content.SetTitle ("test title");
        //        content.SetTitleUrl ("http://www.mob.com");
        //        content.SetSite ("Mob-ShareSDK");
        //        content.SetSiteUrl ("http://www.mob.com");
        //        content.SetUrl ("http://www.mob.com");
        content.SetComment ("test description");
        //        content.SetMusicUrl ("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
        content.SetShareType (ContentType.Text);


        //如果想要指定九宫格列表，请删除或者添加对应的jar文件到Assets/Plugins/Android/ShareSDK/libs 目录中
        ssdk.ShowPlatformList (null, content, 100, 100);

    }
}
