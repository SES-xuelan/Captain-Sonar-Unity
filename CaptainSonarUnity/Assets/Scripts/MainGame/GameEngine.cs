using UnityEngine;
using System.Collections;
using System;
using XLAF.Public;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine.UI;
using Pomelo.DotNetClient;

public class GameEngine
{
    //!IMPORTANT!
    //当前没有用到mysql，polemo也只发送潜艇的移动方向，其余的需要以后慢慢完善

    ////////////////////////////////////////////////////////////////////////////////////////////////

    //!TODO!

    #region  数据交互协议

    /*
    定义通讯协议： 
    发送方：需要发送信息，格式为【{发送信息的队伍}|{信息头部}|{发送的信息}】
    接收方：根据发送的信息来读取数据库中相应的信息，并且更新自身的界面显示

    通讯协议详情：【*代表已经实现的功能】
        发送信息的队伍：B/R 【B:蓝队  R:红队】
        发送的信息头部：
            TJ【tell jobs，告诉人们，自己担任的职业；收到信息的人需要读取数据库，更新职业信息】
            GO【game over，游戏结束；】
            *SG{E/S/W/N}【submarine go to east/south/west/north；潜艇向四个方向移动；】
            SS【submarine surfacing，潜艇上浮，10秒无法操作（可以自己规定时间，一般建议10-30秒）】
            U{MI/TO/DR/SO/SI/SC}【使用系统，系统的前两个字母大写表示这个系统】
            *FD【firstmate done，firstmate充能完毕】
            *ED【engineer done，engineer 损坏完毕】
            ...!TODO!
    */

    #endregion

    #region 需要用pomelo发送的信息

    /* 
    通用：
        通知告诉所有人自己担任的职业和队伍
        游戏结束
    captain：
        潜艇移动方向
        是否上浮
    firstmate：
        使用了哪个系统
        是否充能完毕
    engineer：
        是否损坏完毕
    radiooperator：
        无
    */

    #endregion

    #region  需要数据库储存的信息

    /*
    数据库名：
        CaptainSonar
    ---------- 表↓ 全部小写 --------------
    submarine【潜艇】
        id【编号，自动增长】
        room【string 房间号】
        team【string 队伍名，blue/red】
        blood【int 潜艇血量】

        startpos【string 起始位置，字符串"A1"  "H6"】
        currentpos【string 当前位置，字符串"A1"  "H6"】
        way【string 己方潜艇走的路线，潜艇上浮后清空，json字符串】

        mine【int 武器：水雷 记录当前充能】
        torpedo【int 武器：鱼雷 记录当前充能】
        drone【int 侦测：无人机 记录当前充能】
        sonar【int 侦测：声呐 记录当前充能】
        silence【int 特殊功能：潜行 记录当前充能】
        scenario【int 特殊功能：剧本 记录当前充能】
        mineok【bool 是否充能完毕】
        torpedook【bool 是否充能完毕】
        droneok【bool 是否充能完毕】
        sonarok【bool 是否充能完毕】
        silenceok【bool 是否充能完毕】
        scenariook【bool 是否充能完毕】

        engineermap【string 工程师负责的，潜艇哪部分损坏的map，json字符串】
        weaponok【bool 是否可以使用"攻击"类别的功能】
        detectionok【bool 是否可以使用"侦查"类别的功能】
        specialok【bool 是否可以使用"特殊"类别的功能】

        radioway【string 监听的敌方的路线，json字符串】

        captain【string 负责这个职位的用户名】
        firstmate【string 负责这个职位的用户名】
        engineer【string 负责这个职位的用户名】
        radiooperator【string 负责这个职位的用户名】

        captainstatus【int 当前的状态，  0:还未移动  1:移动完毕  】
        firstmatestatus【int 当前的状态，  0:还未操作  1:操作完毕  】
        engineerstatus【int 当前的状态，  0:还未操作  1:操作完毕  】
        radiooperatorstatus【int 当前的状态，  0:还未操作  1:操作完毕  】
    */

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static bool hasServer { get; set; }

    public static string userName{ get; set; }

    public static string room{ get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////

    //  B/R
    public static string myTeam{ get; set; }


    public static bool isCaptain { get; set; }

    public static bool isFirstMate { get; set; }

    public static bool isEngineer { get; set; }

    public static bool isRadioOperator { get; set; }

    public static Captain captain;
    public static FirstMate firstMate;
    public static Engineer engineer;
    public static RadioOperator radioOperator;

    public static  ChatInfo redInfo = new ChatInfo ();

    public static  ChatInfo blueInfo = new ChatInfo ();


    public static Dictionary<string,MapPoint> GetEmptyMap (string mapName)
    {
        Dictionary<string,MapPoint> map = new Dictionary<string, MapPoint> ();



        #region  Map Status

        //0 不能经过，有礁石
        //1 普通的点，能经过
        //2 有预设的水雷
        //3 有自己放的水雷
        //4 极寒之地，能经过，在此处上浮会受到伤害
        int[] alphaStatus = new int[] {
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 0, 1, 1,/**/ 1, 0, 1, 1, 1,/**/ 1, 1, 0, 0, 1,
            1, 1, 0, 1, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 1, 0, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 0, 1, 0, 1,/**/ 1, 0, 1, 0, 1,/**/ 1, 1, 1, 1, 1,
            1, 0, 1, 0, 1,/**/ 1, 0, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1,/**/ 1, 1, 0, 1, 1,/**/ 1, 0, 0, 0, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 0, 1, 1,/**/ 1, 1, 0, 1, 1,/**/ 1, 0, 1, 1, 1,
            0, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 0, 1, 1,
            1, 1, 0, 1, 1,/**/ 1, 0, 1, 0, 1,/**/ 1, 1, 1, 0, 1,
            1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1
        };
        int[] bravoStatus = new int[] {
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 0, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 0, 0, 1,
            1, 1, 0, 1, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 0, 0, 0, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 0, 1, 1,/**/ 1, 1, 0, 1, 1,/**/ 1, 0, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1
        };
        int[] charlieStatus = new int[] {
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 0, 1, 1, 1, 1,/**/ 1, 1, 0, 1, 1,
            1, 1, 1, 1, 1,/**/ 0, 1, 1, 1, 1,/**/ 1, 0, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 0, 0, 1,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 0,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1
        };
        int[] deltaStatus = new int[] {
            4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,
            4, 4, 0, 4, 4,/**/ 4, 0, 4, 4, 4,/**/ 4, 4, 0, 0, 4,
            4, 4, 0, 4, 4,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4,/**/ 4, 0, 4, 0, 4,/**/ 4, 4, 1, 1, 4,
            4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 1, 1, 4,
            ////////////////////////////////////////////////////
            4, 4, 4, 1, 1,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,
            4, 0, 4, 1, 1,/**/ 4, 0, 4, 0, 4,/**/ 4, 4, 4, 4, 4,
            4, 0, 4, 4, 4,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,
            4, 4, 4, 0, 4,/**/ 4, 4, 0, 4, 4,/**/ 4, 0, 4, 0, 4,
            4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,
            ////////////////////////////////////////////////////
            4, 4, 4, 0, 4,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,
            4, 4, 0, 4, 4,/**/ 4, 4, 0, 1, 1,/**/ 4, 0, 4, 4, 4,
            0, 4, 4, 4, 4,/**/ 4, 4, 4, 1, 1,/**/ 4, 4, 4, 0, 4,
            4, 4, 0, 4, 4,/**/ 4, 0, 4, 4, 4,/**/ 4, 4, 4, 0, 4,
            4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4,/**/ 4, 4, 4, 4, 4
        };
        int[] echoStatus = new int[] {
            1, 1, 0, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 0, 0, 1,/**/ 1, 2, 1, 1, 1,
            1, 0, 1, 0, 1,/**/ 1, 2, 1, 1, 1,/**/ 1, 1, 0, 0, 1,
            1, 1, 2, 1, 1,/**/ 1, 0, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 1, 1,/**/ 2, 1, 1, 1, 2,/**/ 1, 1, 1, 1, 1,
            1, 0, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 0, 1,
            1, 1, 1, 0, 1,/**/ 1, 0, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 2, 1, 1, 1,/**/ 1, 1, 1, 0, 1,/**/ 1, 0, 1, 2, 1,
            1, 1, 1, 1, 1,/**/ 2, 1, 1, 1, 2,/**/ 1, 1, 1, 1, 1,
            ////////////////////////////////////////////////////
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1,/**/ 1, 0, 1, 1, 1,/**/ 1, 2, 1, 1, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 0, 1,
            1, 0, 1, 2, 1,/**/ 1, 0, 2, 1, 1,/**/ 1, 1, 1, 0, 1,
            1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1,/**/ 1, 1, 1, 1, 1
        };
        #endregion

        int[] status = new int[15 * 15];
        if (mapName == "Alpha Map 9") {
            status = alphaStatus;
        } else if (mapName == "Bravo Map 9") {
            status = bravoStatus;
        } else if (mapName == "Charlie Map 9") {
            status = charlieStatus;
        } else if (mapName == "Delta Map 9") {
            status = deltaStatus;
        } else if (mapName == "Echo Map 9") {
            status = echoStatus;
        } else {
            Log.Error ("unsupported map : ", mapName);
            return null;
        }

        for (int y = 1; y <= 15; y++) {
            for (int x = 1; x <= 15; x++) {
                string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y);
                int index = (y - 1) * 15 + (x - 1);

                if (x <= 5 && y <= 5) {
                    map.Add (pos, new MapPoint (pos, Color.blue, 1, status [index]));
                } else if (x >= 6 && x <= 10 && y <= 5) {
                    map.Add (pos, new MapPoint (pos, Color.gray, 2, status [index]));
                } else if (x >= 11 && x <= 15 && y <= 5) {
                    map.Add (pos, new MapPoint (pos, Color.blue, 3, status [index]));
                } else if (x <= 5 && y >= 6 && y <= 10) {
                    map.Add (pos, new MapPoint (pos, Color.gray, 4, status [index]));
                } else if (x >= 6 && x <= 10 && y >= 6 && y <= 10) {
                    map.Add (pos, new MapPoint (pos, Color.blue, 5, status [index]));
                } else if (x >= 11 && x <= 15 && y >= 6 && y <= 10) {
                    map.Add (pos, new MapPoint (pos, Color.gray, 6, status [index]));
                } else if (x <= 5 && y >= 11 && y <= 15) {
                    map.Add (pos, new MapPoint (pos, Color.blue, 7, status [index]));
                } else if (x >= 6 && x <= 10 && y >= 11 && y <= 15) {
                    map.Add (pos, new MapPoint (pos, Color.gray, 8, status [index]));
                } else if (x >= 11 && x <= 15 && y >= 11 && y <= 15) {
                    //                    ColorUtility.TryParseHtmlString ("#00EE00", out c);
                    map.Add (pos, new MapPoint (pos, Color.blue, 9, status [index]));
                }

            }
        }



        return map;
    }

    public static int GetJobsCount ()
    {
        int ret = 0;
        if (isCaptain)
            ret++;
        if (isFirstMate)
            ret++;
        if (isEngineer)
            ret++;
        if (isRadioOperator)
            ret++;
        return ret;
    }


    /// <summary>
    /// Gets the direction through point.
    /// </summary>
    /// <returns>The direction through point. (e.g. MapPoint.TextType.WEST)</returns>
    /// <param name="currPointPos">Curr point position.</param>
    /// <param name="pointPosYouWantToGo">Point position you want to go.</param>
    public static MapPoint.TextType GetDirectionThroughPoint (string currPointPos, string pointPosYouWantToGo)
    {
        int currX = ModUtils.Character2Ascii (currPointPos.Substring (0, 1));
        int currY = int.Parse (currPointPos.Substring (1));
        int wantX = ModUtils.Character2Ascii (pointPosYouWantToGo.Substring (0, 1));
        int wantY = int.Parse (pointPosYouWantToGo.Substring (1));

        if (Math.Abs (wantX - currX) == 0) {
            if (wantY - currY == -1) {
                return MapPoint.TextType.NORTH;
            } else if (wantY - currY == 1) {
                return MapPoint.TextType.SOUTH;
            }
        } else if (Math.Abs (wantY - currY) == 0) {
            if (wantX - currX == -1) {
                return MapPoint.TextType.WEST;
            } else if (wantX - currX == 1) {
                return MapPoint.TextType.EAST;
            }
        }
        return MapPoint.TextType.NONE;
    }

    /// <summary>
    /// Gets the point through direction.
    /// </summary>
    /// <returns>The point through direction.(e.g "A1")</returns>
    /// <param name="currPos">Curr position.</param>
    /// <param name="textType">Text type.</param>
    public static string GetPointThroughDirection (string currPos, MapPoint.TextType textType)
    {

        int currX = ModUtils.Character2Ascii (currPos.Substring (0, 1));
        int currY = int.Parse (currPos.Substring (1));


        if (textType == MapPoint.TextType.WEST && currX > 65) {
            return string.Format ("{0}{1}", ModUtils.Ascii2Character (currX - 1), currY);
        } else if (textType == MapPoint.TextType.NORTH && currY > 1) {
            return string.Format ("{0}{1}", ModUtils.Ascii2Character (currX), currY - 1);
        } else if (textType == MapPoint.TextType.EAST && currX < 79) {
            return string.Format ("{0}{1}", ModUtils.Ascii2Character (currX + 1), currY);
        } else if (textType == MapPoint.TextType.SOUTH && currY < 15) {
            return string.Format ("{0}{1}", ModUtils.Ascii2Character (currX), currY + 1);
        }
        return "";
    }

    public static void ShowTip (string title, string content)
    {
        JSONNode js = JSONNode.Parse ("{}");
        js ["title"] = title;
        js ["content"] = content;
        MgrDialog.ShowDialog ("Tip", js, 0.1f);
    }

    public static void ShowMask (string str)
    {
        MgrDialog.ShowDialog ("Mask", str, SceneAnimation.none);
    }

    public static void ChangeMaskText (string str)
    {
        SceneObject so = MgrDialog.GetDialog ("Mask");
        if (so != null) {
            so.script.WillEnterScene (str);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    static GameEngine ()
    {
        hasServer = false;
        AddPolemoListeners ();
    }



    ////////////////////////////////////// Polemo //////////////////////////////////////////////////

    public static void AddPolemoListeners ()
    {

        PomeloHelper.AddChatListener ("GameEngine", HandleChat);
        PomeloHelper.AddDisconnectListener ("GameEngine", HandleDisconnect);
        PomeloHelper.AddUserAddListener ("GameEngine", HandleUserAdd);
        PomeloHelper.AddUserLeaveListener ("GameEngine", HandleUserLeave);
    }

    public static void RemovePolemoListeners ()
    {
        PomeloHelper.RemoveAllListenersWithName ("GameEngine");
    }

    public static void PomeloSendMessageToChat (string head, string msg, Action<Message> cb)
    {
        if (GameEngine.hasServer) {
            string fullMsg = string.Format ("{0}|{1}|{2}", myTeam, head, msg);
            PomeloHelper.SendMessageToChatServer (room, fullMsg, userName, "*", cb);
        }
    }

    /// <summary>
    /// 收到消息之后的操作，注意，自己做的操作也需要等到消息收到后再执行操作，防止本地和服务器记录不一致
    /// </summary>
    /// <param name="msg">Message.</param>
    public static void HandleChat (Message msg)
    {
        string data = msg.jsonObj ["msg"].ToString ();
        if (data.IndexOf ('|') <= 0) {
            return;
        }

        string[] _arr = data.Split ('|');
        string team = _arr [0];
        string head = _arr [1];
        string main = _arr [2];

        Log.Debug ("team:", team);
        Log.Debug ("head:", head);
        Log.Debug ("main:", main);

        if (head == "SGE" || head == "SGS" || head == "SGW" || head == "SGN") {
            if (team == "R") {
                GameEngine.redInfo.text.text += main;
                GameEngine.redInfo.scrollRect.StopMovement ();
                ModUIUtils.ChangePos (GameEngine.redInfo.rect, y: -200f);
            } else {
                GameEngine.blueInfo.text.text += main;
                GameEngine.blueInfo.scrollRect.StopMovement ();
                ModUIUtils.ChangePos (GameEngine.blueInfo.rect, y: -200f);
            }
            if (team == myTeam) {
                if (isCaptain) {
                    captain.ShowMask ("waiting...\nFirstMate:not OK\nEngineer:not OK");
                }
                if (isFirstMate) {
                    firstMate.HideMask ();
                }
                if (isEngineer) {
                    engineer.HideMask ();
                }
            }
        } else if (head == "FD") {
            if (team == myTeam) {
                if (isCaptain) {
                    string txt = captain.GetMaskText ();
                    captain.ChangeMaskText (txt.Replace ("FirstMate:not OK", "FirstMate:OK"));
                    if (captain.GetMaskText ().IndexOf ("FirstMate:OK\nEngineer:OK") > 0) {
                        captain.HideMask ();
                    }
                }
            }
        } else if (head == "ED") {
            if (team == myTeam) {
                if (isCaptain) {
                    string txt = captain.GetMaskText ();
                    captain.ChangeMaskText (txt.Replace ("Engineer:not OK", "Engineer:OK"));
                    if (captain.GetMaskText ().IndexOf ("FirstMate:OK\nEngineer:OK") > 0) {
                        captain.HideMask ();
                    }
                }
            }
        } else if (head == "") {
            //!TODO!
        }
    }

    public static void HandleDisconnect (Message data)
    {

    }

    public static void HandleUserAdd (Message data)
    {

    }

    public static void HandleUserLeave (Message data)
    {

    }


    ////////////////////////////////////////////////////////////////////////////////////////////////



    /// <summary>
    /// Map point. Captain只有经过和上浮，radioOperator可以来回点击
    /// </summary>
    public class MapPoint
    {
        public enum TextType
        {
            START,
            WEST,
            NORTH,
            EAST,
            SOUTH,
            MINE,
            // none 代表没有方向，用于处理一些特殊判断
            NONE
        }

        public class FullInfo
        {
            public string text;
            public Sprite sprite;
            public bool isPressed;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Sprite zeroAlphaSprite;
        public static Sprite knobSprite;

        static MapPoint ()
        {
            MapPoint.zeroAlphaSprite = Resources.Load<Sprite> ("Images/MainGame/zeroAlpha");
            MapPoint.knobSprite = Resources.Load<Sprite> ("Images/MainGame/Knob");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public string pos;
        public Color txtColor;
        public int area;
        public int state;

        private Text textObj;
        private Image imageObj;
        private Button btnObj;
        private GameObject btnPrefabObj;

        private bool _isPressed = false;

        //缓存text的文字，用于显示位置和显示位置路径之间切换
        private string cacheTextString = "";


        public bool isPressed{ get { return this._isPressed; } }

        public bool isAlreadyPassed{ get { return this._isPressed; } }

        public MapPoint (string pos, Color txtColor, int area, int state = 1)
        {
            this.area = area;
            this.pos = pos;
            this.txtColor = txtColor;
            this.state = state;
        }


        public void SetButtonPrefabObj (GameObject btnPrefabObj)
        {
            this.btnPrefabObj = btnPrefabObj;

            this.textObj = ModUIUtils.GetChild<Text> (btnPrefabObj.transform, "Text");
            this.imageObj = btnPrefabObj.GetComponent<Image> ();
            this.btnObj = btnPrefabObj.GetComponent<Button> ();
        }

        public void ChangeButtonAppearanceWithState ()
        {
            if (this.btnPrefabObj == null) {
                Log.Error ("You must call SetButtonPrefabObj first");
                return;
            }
            //0 不能经过，有礁石
            //1 普通的点，能经过
            //2 有预设的水雷
            //3 有自己放的水雷
            //4 极寒之地，能经过，在此处上浮会受到伤害

            switch (this.state) {
            case 0:
                textObj.text = "";
                btnObj.interactable = false;
                break;
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                
                break;
            case 4:
                
                break;
            default:
                
                break;
            }

//            this.textObj.color = this.txtColor;

        }

        /// <summary>
        /// Press the point. 
        /// Captain should NOT use, radioOperator can use.
        /// </summary>
        public void OnPressed (string btnText = "")
        {
            if (this._isPressed) {
                this.imageObj.sprite = MapPoint.zeroAlphaSprite;
                this.textObj.text = "";
            } else {
                this.imageObj.sprite = MapPoint.knobSprite;
                this.textObj.text = btnText;
            }
            this._isPressed = !this._isPressed;
        }

        /// <summary>
        /// Passed by this point
        /// radioOperator should NOT use, Captain can use.
        /// </summary>
        public void OnPassedBy ()
        {
            this.imageObj.sprite = MapPoint.knobSprite;
            this._isPressed = true;
        }


        public void ReInit ()
        {
            this.textObj.text = "";
            this._isPressed = false;
            this.imageObj.sprite = MapPoint.zeroAlphaSprite;
        }

        public void SetText (TextType textType)
        {
            switch (textType) {
            case TextType.START:
                this.textObj.text = "S";
                break;
            case TextType.MINE:
                this.textObj.text += "M";
                break;
            case TextType.EAST:
                this.textObj.text = "→";
                break;
            case TextType.NORTH:
                this.textObj.text = "↑";
                break;
            case TextType.SOUTH:
                this.textObj.text = "↓";
                break;
            case TextType.WEST:
                this.textObj.text = "←";
                break;
            case TextType.NONE:
                this.textObj.text = "";
                break;
            }
        }

        /// <summary>
        /// Shows the position.
        /// </summary>
        public void ShowPosition ()
        {
            cacheTextString = this.textObj.text;
            this.textObj.text = this.pos;
            this.textObj.color = Color.blue;
        }

        /// <summary>
        /// Hides the position.
        /// </summary>
        public void HidePosition ()
        {
            this.textObj.text = cacheTextString;
            this.textObj.color = Color.black;
        }



        public FullInfo GetFullInfo ()
        {
            FullInfo info = new FullInfo ();
            info.text = this.textObj.text;
            info.sprite = this.imageObj.sprite;
            info.isPressed = this._isPressed;
            return info;
        }

        public void SetFullInfo (FullInfo fullinfo)
        {
            this.textObj.text = fullinfo.text;
            this.imageObj.sprite = fullinfo.sprite;
            this._isPressed = fullinfo.isPressed;
        }
    }


    public class ChatInfo
    {
        public Text text;
        public ScrollRect scrollRect;
        public RectTransform rect;

    }
}
