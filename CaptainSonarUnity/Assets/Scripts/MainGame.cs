using UnityEngine;
using System.Collections;
using XLAF.Public;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using System.Text.RegularExpressions;

public class MainGame : Storyboard
{
    // /Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates/81-C# Storyboard-NewStoryboard.cs.txt

    void Update ()
    {
	
    }



    private Transform layCaptain;
    private Transform layFirstMate;
    private Transform layEngineer;
    private Transform layRadioOperator;

    private string currMapName;
    private Layers currentLayer = Layers.NONE;


    //    private bool isPlaying = false;

    private Dictionary<string , Button> btnTop = new Dictionary<string, Button> ();

    private bool isBusy = false;

    #region  Storyboard Listeners

    public override void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            Log.Debug ("e.target.name", e.target.name);
            if (e.target.name == "BtnCaptain") {
                BtnCaptain ();
            } else if (e.target.name == "BtnFirstMate") {
                BtnFirstMate ();
            } else if (e.target.name == "BtnEngineer") {
                BtnEngineer ();
            } else if (e.target.name == "BtnRadioOperator") {
                BtnRadioOperator ();
            } else if (e.target.name == "BtnBack") {
                DoBack ();
            } else if (e.target.name == "BtnTitle") {
                Log.Debug ("currentLayer:", currentLayer);
                if (currentLayer == Layers.CAPTAIN) {
                    GameEngine.captain.TapTitle ();
                } else if (currentLayer == Layers.FIRSTMATE) {
                } else if (currentLayer == Layers.ENGINEER) {
                } else if (currentLayer == Layers.RADIOOPERATOR) {
                    GameEngine.radioOperator.TapTitle ();
                }
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
//        Button btnCaptain = ModUIUtils.GetChild<Button> (transform, "BtnCaptain");
//        Button btnFirstMate = ModUIUtils.GetChild<Button> (transform, "BtnFirstMate");
//        Button btnEngineer = ModUIUtils.GetChild<Button> (transform, "BtnEngineer");
//        Button btnRadioOperator = ModUIUtils.GetChild<Button> (transform, "BtnRadioOperator");

        layCaptain = ModUIUtils.GetChild<Transform> (transform, "LayCaptain");
        layFirstMate = ModUIUtils.GetChild<Transform> (transform, "LayFirstMate");
        layEngineer = ModUIUtils.GetChild<Transform> (transform, "LayEngineer");
        layRadioOperator = ModUIUtils.GetChild<Transform> (transform, "LayRadioOperator");
        btnTop.Add ("C", ModUIUtils.GetChild<Button> (transform, "BtnCaptain"));
        btnTop.Add ("F", ModUIUtils.GetChild<Button> (transform, "BtnFirstMate"));
        btnTop.Add ("E", ModUIUtils.GetChild<Button> (transform, "BtnEngineer"));
        btnTop.Add ("R", ModUIUtils.GetChild<Button> (transform, "BtnRadioOperator"));

        GameEngine.captain = new Captain ();
        GameEngine.firstMate = new FirstMate ();
        GameEngine.engineer = new Engineer ();
        GameEngine.radioOperator = new RadioOperator ();

        GameEngine.redInfo.text = ModUIUtils.GetChild<Text> (transform, "RedInfo/Viewport/Content");
        GameEngine.redInfo.scrollRect = ModUIUtils.GetChild<ScrollRect> (transform, "RedInfo");
        GameEngine.redInfo.rect = ModUIUtils.GetChild<RectTransform> (transform, "RedInfo/Viewport/Content");
        GameEngine.redInfo.text.text = "";
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<RectTransform> (transform, "RedInfo"), width: MgrScene.GetRoot ().rect.width / 2f);

        GameEngine.blueInfo.text = ModUIUtils.GetChild<Text> (transform, "BlueInfo/Viewport/Content");
        GameEngine.blueInfo.scrollRect = ModUIUtils.GetChild<ScrollRect> (transform, "BlueInfo");
        GameEngine.blueInfo.rect = ModUIUtils.GetChild<RectTransform> (transform, "BlueInfo/Viewport/Content");
        GameEngine.blueInfo.text.text = "";
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<RectTransform> (transform, "BlueInfo"), width: MgrScene.GetRoot ().rect.width / 2f);



    }

    public override void WillEnterScene (object obj)
    {
        currMapName = obj as string;
        Text mapText = ModUIUtils.GetChild<Text> (transform, "MapText");
        mapText.text = currMapName;
        mapText.color = GameEngine.myTeam == "R" ? Color.red : Color.blue;
        InitUI ();
        ShowBoard (Layers.NONE);
        if (GameEngine.isCaptain) {
            GameEngine.captain.Init (layCaptain, currMapName);
        }
        if (GameEngine.isFirstMate) {
            GameEngine.firstMate.Init (layFirstMate, currMapName);
        }
        if (GameEngine.isEngineer) {
            GameEngine.engineer.Init (layEngineer, currMapName);
        }
        if (GameEngine.isRadioOperator) {
            GameEngine.radioOperator.Init (layRadioOperator, currMapName);
        }
    }

    public override void EnterScene (object obj)
    {
        isBusy = false;
    }

    public override void WillExitScene ()
    {
        if (GameEngine.isCaptain) {
            GameEngine.captain.Destory ();
        }
        if (GameEngine.isFirstMate) {
            GameEngine.firstMate.Destory ();
        }
        if (GameEngine.isEngineer) {
            GameEngine.engineer.Destory ();
        }
        if (GameEngine.isRadioOperator) {
            GameEngine.radioOperator.Destory ();
        }

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
//        if (overlaySceneceneName == "UseSystems") {
//
//            if (currentLayer == Layers.CAPTAIN) {
//            } else if (currentLayer == Layers.FIRSTMATE) {
//            } else if (currentLayer == Layers.ENGINEER) {
//            } else if (currentLayer == Layers.RADIOOPERATOR) {
//            }
//        }
    }

    public override void UpdateLanguage ()
    {
        ModUIUtils.GetChild<Text> (transform, "BtnCaptain/Text").text = MgrMutiLanguage.GetString ("Nomal_Captain");
        ModUIUtils.GetChild<Text> (transform, "BtnFirstMate/Text").text = MgrMutiLanguage.GetString ("Nomal_FirstMate");
        ModUIUtils.GetChild<Text> (transform, "BtnEngineer/Text").text = MgrMutiLanguage.GetString ("Nomal_Engineer");
        ModUIUtils.GetChild<Text> (transform, "BtnRadioOperator/Text").text = MgrMutiLanguage.GetString ("Nomal_RadioOperator");

    }
    #if UNITY_ANDROID
    public override void AndroidGoBack ()
    {
        DoBack ();
    }
    #endif
    #endregion


    private enum Layers
    {
        NONE,
        CAPTAIN,
        FIRSTMATE,
        ENGINEER,
        RADIOOPERATOR
    }


    private void ShowBoard (Layers job)
    {
        currentLayer = job;
        layCaptain.gameObject.SetActive (false);
        layFirstMate.gameObject.SetActive (false);
        layEngineer.gameObject.SetActive (false);
        layRadioOperator.gameObject.SetActive (false);

        foreach (KeyValuePair<string, Button>  kv in btnTop) {
            kv.Value.image.color = Color.white;
        }
        if (job == Layers.CAPTAIN && GameEngine.isCaptain) {
            layCaptain.gameObject.SetActive (true);
            btnTop ["C"].image.color = Color.red;
        } else if (job == Layers.FIRSTMATE && GameEngine.isFirstMate) {
            layFirstMate.gameObject.SetActive (true);
            btnTop ["F"].image.color = Color.red;
        } else if (job == Layers.ENGINEER && GameEngine.isEngineer) {
            layEngineer.gameObject.SetActive (true);
            btnTop ["E"].image.color = Color.red;
        } else if (job == Layers.RADIOOPERATOR && GameEngine.isRadioOperator) {
            layRadioOperator.gameObject.SetActive (true);
            btnTop ["R"].image.color = Color.red;
        }
    }

    private void BtnCaptain ()
    {
        ShowBoard (Layers.CAPTAIN);
    }

    private void BtnFirstMate ()
    {
        ShowBoard (Layers.FIRSTMATE);
    }

    private void BtnEngineer ()
    {
        ShowBoard (Layers.ENGINEER);
    }

    private void BtnRadioOperator ()
    {
        ShowBoard (Layers.RADIOOPERATOR);

    }

    void InitUI ()
    {
        btnTop ["C"].gameObject.SetActive (GameEngine.isCaptain);
        btnTop ["F"].gameObject.SetActive (GameEngine.isFirstMate);
        btnTop ["E"].gameObject.SetActive (GameEngine.isEngineer);
        btnTop ["R"].gameObject.SetActive (GameEngine.isRadioOperator);

        float rootWidth = MgrScene.GetRoot ().rect.width;
//        Log.Debug ("rootWidth", rootWidth);

        float width = rootWidth / GameEngine.GetJobsCount ();
        float num = 0.5f;
        if (GameEngine.isCaptain) {
            ModUIUtils.ChangeSize (btnTop ["C"], width);
            ModUIUtils.ChangePos (btnTop ["C"], width * num);
            num++;
        }
        if (GameEngine.isFirstMate) {
            ModUIUtils.ChangeSize (btnTop ["F"], width);
            ModUIUtils.ChangePos (btnTop ["F"], width * num);
            num++;
        }
        if (GameEngine.isEngineer) {
            ModUIUtils.ChangeSize (btnTop ["E"], width);
            ModUIUtils.ChangePos (btnTop ["E"], width * num);
            num++;
        }
        if (GameEngine.isRadioOperator) {
            ModUIUtils.ChangeSize (btnTop ["R"], width);
            ModUIUtils.ChangePos (btnTop ["R"], width * num);
        }



    }

    private void DoBack ()
    {
        if (isBusy)
            return;
        isBusy = true;

        ConfirmConfig cc = new ConfirmConfig (MgrMutiLanguage.GetString("Nomal_Warning"), MgrMutiLanguage.GetString("Confirm_LostProgress"));
        cc.SetButton1Text (MgrMutiLanguage.GetString("Nomal_OK"));
        cc.SetButton2Text ("");
        cc.SetButton3Text (MgrMutiLanguage.GetString("Nomal_Cancel"));
        cc.SetCallback (delegate(int index) {
            if (index == 1) {
                MgrScene.GotoScene ("SelectJobs", SceneAnimation.slideRight);
            } else {
                isBusy = false;
            }
        });
        MgrDialog.ShowDialog ("Confirm", cc, SceneAnimation.none);
    }




}
    