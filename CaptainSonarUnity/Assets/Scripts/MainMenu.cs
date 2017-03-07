using UnityEngine;
using System.Collections;
using XLAF.Public;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;
using Pomelo.DotNetClient;
using MySql.Data.MySqlClient;
using SimpleJson;
using System.Text;

//using XLua;

public class MainMenu : Storyboard
{
    void Start ()
    {
//        LuaEnv luaenv = new LuaEnv ();
//        luaenv.DoString ("CS.XLAF.Public.Log.Debug(\" aaaa\",123)");
//        luaenv.Dispose ();
    }

    private bool isBusy = false;

    #region  Storyboard Listeners



    public override void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            if (e.target.name == "BtnLogin") {
                //                BtnLogin ();
                MgrScene.GotoScene ("SelectJobs");
            }
        }
    }

    /*
        CreatScene 加载完界面，还未播放动画（只有界面加载的时候，才会触发；读取缓存界面不会触发）
        WillEnterScene 加载完毕scene，即将播放过渡动画
        EnterScene 播放完毕过渡动画

        WillExitScene 即将播放退出界面的动画
        ExitScene 播放完退出界面的动画
        DestoryScene 销毁界面前触发

        OverlayBegan 界面被遮挡时触发，仅计算XLAF框架内的遮挡
        OverlayEnded 界面取消遮挡时触发，仅计算XLAF框架内的遮挡
        AndroidGoBack Android系统下按实体back按钮触发
        UpdateLanguage 更新语言时、CreatScene之后触发，用于更新界面文字
    */
    public override void CreatScene (object obj)
    {
        Dropdown lang = ModUIUtils.GetChild<Dropdown> (transform, "Lang");
        lang.value = (int)MgrMutiLanguage.GetCurrentLanguage ();
        lang.onValueChanged.AddListener (OnLanguageChanged);
        ModUIUtils.GetChild<Toggle> (transform, "NoServer").onValueChanged.AddListener (delegate(bool isOn) {
            GameEngine.hasServer = !isOn;
            ModUIUtils.GetChild<RectTransform> (transform, "layServer").gameObject.SetActive (GameEngine.hasServer);
        });
        ModUIUtils.GetChild<Toggle> (transform, "HaveServer").onValueChanged.AddListener (delegate(bool isOn) {
            GameEngine.hasServer = isOn;
            ModUIUtils.GetChild<RectTransform> (transform, "layServer").gameObject.SetActive (GameEngine.hasServer);
        });


        ModUIUtils.GetChild<InputField> (transform, "layServer/TxtHost").text = "192.168.1.35:3014";


    }

    public override void WillEnterScene (object obj)
    {
        PomeloHelper.Disconnect ();
    }

    public override void EnterScene (object obj)
    {
        isBusy = false;

        //test
        ModUIUtils.GetChild<InputField> (transform, "layServer/TxtUserName").text = "123";
        ModUIUtils.GetChild<InputField> (transform, "layServer/TxtRoom").text = "123";

    }

    public override void WillExitScene ()
    {
    }

    public override void ExitScene ()
    {
    }

    public override void DestoryScene ()
    {
    }

    public override void OverlayBegan (string overlaySceneceneName)
    {
    }

    public override void OverlayEnded (string overlaySceneceneName)
    {
    }

    public override void UpdateLanguage ()
    {
        ModUIUtils.GetChild<Text> (transform, "layServer/TxtHost/Placeholder").text = MgrMutiLanguage.GetString ("MainMenu_HostTip");
        ModUIUtils.GetChild<Text> (transform, "layServer/TxtUserName/Placeholder").text = MgrMutiLanguage.GetString ("MainMenu_UserNameTip");
        ModUIUtils.GetChild<Text> (transform, "layServer/TxtRoom/Placeholder").text = MgrMutiLanguage.GetString ("MainMenu_RoomTip");
        ModUIUtils.GetChild<Text> (transform, "layServer/ErrorTip").text = MgrMutiLanguage.GetString ("MainMenu_ErrorTip");

        ModUIUtils.GetChild<Text> (transform, "NoServer/Label").text = MgrMutiLanguage.GetString ("MainMenu_Noserver");
        ModUIUtils.GetChild<Text> (transform, "HaveServer/Label").text = MgrMutiLanguage.GetString ("MainMenu_Server");

        ModUIUtils.GetChild<Text> (transform, "Logo").text = MgrMutiLanguage.GetString ("MainMenu_Logo");
        ModUIUtils.GetChild<Text> (transform, "BtnLogin/Text").text = MgrMutiLanguage.GetString ("MainMenu_Login");
        ModUIUtils.GetChild<Text> (transform, "Tip").text = MgrMutiLanguage.GetString ("MainMenu_Tip");
    }
    #if UNITY_ANDROID
    public override void AndroidGoBack ()
    {
        Log.Debug ("AndroidGoBack", this.sceneName);
//        Application.Quit ();
    }
    #endif
    #endregion


    private void OnLanguageChanged (int v)
    {
        if (v == 0) {
            MgrMutiLanguage.SwitchLanguage (MgrMutiLanguage.Language.zh_cn);
        } else if (v == 1) {
            MgrMutiLanguage.SwitchLanguage (MgrMutiLanguage.Language.en_us);
        }

    }

    private void BtnLogin ()
    {
        if (isBusy)
            return;
        isBusy = true;

        if (GameEngine.hasServer) {

            //查找text获取内容
            //      string str=transform.Find("Container/userName").GetComponent<InputField>().text;
            GameEngine.userName = ModUIUtils.GetChild<InputField> (transform, "layServer/TxtUserName").text;
            GameEngine.room = ModUIUtils.GetChild<InputField> (transform, "layServer/TxtRoom").text;

            string hostAndPort = ModUIUtils.GetChild<InputField> (transform, "layServer/TxtHost").text;
            string host;
            int port;
            if (hostAndPort.IndexOf (":") > 0) {
                string[] arr = hostAndPort.Split (new char[]{ ':' }, 2);
                host = arr [0].ToString ();
                if (!int.TryParse (arr [1].ToString (), out port)) {
                    GameEngine.ShowTip (MgrMutiLanguage.GetString ("Nomal_Warning"), MgrMutiLanguage.GetString ("MainMenu_HostError"));
                    isBusy = false;
                    return;
                } else {
                    if (port < 0 || port > 65535) {
                        GameEngine.ShowTip (MgrMutiLanguage.GetString ("Nomal_Warning"), MgrMutiLanguage.GetString ("MainMenu_HostError"));
                        isBusy = false;
                        return;
                    }
                }

            } else {
                host = hostAndPort;
                port = 3014;
            }

            Log.Debug (host, port);

            //        Debug.Log ("host:" + host + "|port:" + port.ToString ());

            PomeloHelper.EnterChatServer (host, port, GameEngine.userName, GameEngine.room, OnEnter);
            GameEngine.ShowMask (MgrMutiLanguage.GetString ("Mask_Connecting"));
        } else {
            MgrScene.GotoScene ("SelectJobs", SceneAnimation.slideLeft);
        }

    }


    private void OnEnter (Message data)
    {
        Debug.Log ("on login " + data.ToString ());
        JsonObject result = data.jsonObj;

        if (result.ContainsKey ("error")) {
            GameEngine.ShowTip (MgrMutiLanguage.GetString ("Nomal_Error"), MgrMutiLanguage.GetString ("MainMenu_EnterError", result ["code"].ToString ()));
            isBusy = false;
            return;
        }
        MgrDialog.HideDialog ("Mask", delegate {
        });
        MgrScene.GotoScene ("SelectJobs", SceneAnimation.slideLeft);
    }



}
