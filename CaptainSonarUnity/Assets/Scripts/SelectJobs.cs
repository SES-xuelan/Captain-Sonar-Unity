using UnityEngine;
using System.Collections;
using XLAF.Public;
using UnityEngine.UI;
using SimpleJson;
using System.Collections.Generic;
using SimpleJSON;

public class SelectJobs : Storyboard
{
    // /Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates/81-C# Storyboard-NewStoryboard.cs.txt

    void Update ()
    {
	
    }


    private Toggle redTeam;
    //        private Toggle blueTeam;

    private Toggle Captain;
    private Toggle FirstMate;
    private Toggle Engineer;
    private Toggle RadioOperator;

    private Dropdown selectMap;
    /// <summary>
    /// The map info.
    /// </summary>
    private Dictionary<string,string> mapInfo = new Dictionary<string, string> ();

    private bool isBusy;



    #region  Storyboard Listeners

    public override void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            MgrAudio.PlaySound ("s_click.mp3");
            if (e.target.name == "BtnOK") {
                BtnOK ();
            } else if (e.target.name == "BtnBack") {
                BtnBack ();
            } else if (e.target.name == "BtnMapInfo") {
                GameEngine.ShowTip (selectMap.captionText.text, mapInfo [selectMap.captionText.text]);
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
        redTeam = ModUIUtils.GetChild<Toggle> (transform, "SelectTeamRed");
//        blueTeam = ModUIUtils.GetChild<Toggle> (transform, "SelectTeamBlue");
        Captain = ModUIUtils.GetChild<Toggle> (transform, "Captain");
        FirstMate = ModUIUtils.GetChild<Toggle> (transform, "FirstMate");
        Engineer = ModUIUtils.GetChild<Toggle> (transform, "Engineer");
        RadioOperator = ModUIUtils.GetChild<Toggle> (transform, "RadioOperator");
        selectMap = ModUIUtils.GetChild<Dropdown> (transform, "SelectMap");

        selectMap.onValueChanged.AddListener (MapSelected);


    }

    public override void WillEnterScene (object obj)
    {
    }

    public override void EnterScene (object obj)
    {
    }

    public override void WillExitScene ()
    {
    }

    public override void ExitScene ()
    {
        isBusy = false;
    }

    public override void DestoryScene ()
    {
    }

    public override void OverlayBegan (string overlaySceneceneName)
    {
        Log.Debug ("OverlayBegan", overlaySceneceneName);
    }

    public override void OverlayEnded (string overlaySceneceneName)
    {
        Log.Debug ("OverlayEnded", overlaySceneceneName);
    }

    public override void UpdateLanguage ()
    {
        mapInfo.Clear ();
        mapInfo.Add ("Alpha Map 9", MgrMutiLanguage.GetString ("SelectJobs_AlphaMap9"));
        mapInfo.Add ("Bravo Map 9", MgrMutiLanguage.GetString ("SelectJobs_BravoMap9"));
        mapInfo.Add ("Charlie Map 9", MgrMutiLanguage.GetString ("SelectJobs_CharlieMap9"));
        mapInfo.Add ("Delta Map 9", MgrMutiLanguage.GetString ("SelectJobs_DeltaMap9"));
        mapInfo.Add ("Echo Map 9", MgrMutiLanguage.GetString ("SelectJobs_EchoMap9"));

        ModUIUtils.GetChild<Text> (transform, "Title").text = MgrMutiLanguage.GetString ("SelectJobs_Title");
        ModUIUtils.GetChild<Text> (transform, "SelectTeamRed/Label").text = MgrMutiLanguage.GetString ("SelectJobs_RedTeam");
        ModUIUtils.GetChild<Text> (transform, "SelectTeamBlue/Label").text = MgrMutiLanguage.GetString ("SelectJobs_BlueTeam");
        ModUIUtils.GetChild<Text> (transform, "Captain/Label").text = MgrMutiLanguage.GetString ("Nomal_Captain");
        ModUIUtils.GetChild<Text> (transform, "FirstMate/Label").text = MgrMutiLanguage.GetString ("Nomal_FirstMate");
        ModUIUtils.GetChild<Text> (transform, "Engineer/Label").text = MgrMutiLanguage.GetString ("Nomal_Engineer");
        ModUIUtils.GetChild<Text> (transform, "RadioOperator/Label").text = MgrMutiLanguage.GetString ("Nomal_RadioOperator");
        ModUIUtils.GetChild<Text> (transform, "BtnOK/Text").text = MgrMutiLanguage.GetString ("SelectJobs_BtnOK");
    }
    #if UNITY_ANDROID
    public override void AndroidGoBack ()
    {
        BtnBack ();
    }
    #endif
    #endregion



    private void BtnOK ()
    {
        if (isBusy)
            return;
        isBusy = true;

        if (Captain.isOn || FirstMate.isOn || Engineer.isOn || RadioOperator.isOn) {
            if (GameEngine.hasServer) {
                TellJobAndTeam ();
            } else {
                GameEngine.isCaptain = Captain.isOn;
                GameEngine.isFirstMate = FirstMate.isOn;
                GameEngine.isEngineer = Engineer.isOn;
                GameEngine.isRadioOperator = RadioOperator.isOn;
                GameEngine.myTeam = redTeam.isOn ? "R" : "B";
                MgrScene.GotoScene ("MainGame", selectMap.captionText.text, SceneAnimation.slideLeft);
            }
        } else {
            GameEngine.ShowTip (MgrMutiLanguage.GetString ("Tip_langWarningTitle"), MgrMutiLanguage.GetString ("SelectJobs_tipNoJob"));
            isBusy = false;
        }
    }

    private void BtnBack ()
    {
        if (isBusy)
            return;
        isBusy = true;
        MgrScene.GotoScene ("MainMenu", SceneAnimation.slideRight);
    }


    private void MapSelected (int value)
    {
        Log.Debug ("selectMap.captionText", selectMap.captionText.text, value);
    }

    private void TellJobAndTeam ()
    {
        
        JsonObject json = new JsonObject ();
        json ["rid"] = GameEngine.room;
        json ["from"] = GameEngine.userName;
        json ["content"] = "xxx";//!TODO!
        json ["target"] = "*";
        PomeloHelper.SendMessageToChatServer (json, (msg) => {
            //告诉别人的职业和队伍成功
            GameEngine.isCaptain = Captain.isOn;
            GameEngine.isFirstMate = FirstMate.isOn;
            GameEngine.isEngineer = Engineer.isOn;
            GameEngine.isRadioOperator = RadioOperator.isOn;
            GameEngine.myTeam = redTeam.isOn ? "R" : "B";
            MgrScene.GotoScene ("MainGame", selectMap.captionText.text, SceneAnimation.slideLeft);
        });

    }
}
