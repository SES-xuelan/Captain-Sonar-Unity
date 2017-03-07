using UnityEngine;
using System.Collections;
using XLAF.Public;
using SimpleJSON;
using UnityEngine.UI;
using System;

public class Confirm : Storyboard
{
    // /Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates/82-C# Storyboard Dialog-NewDialog.cs.txt

    void Update ()
    {
	
    }

    /// <summary>
    /// The callback.
    /// 0: press android back
    /// 1~3: press button 1~3
    /// </summary>
    private Action<int> callback;

    #region  Storyboard Listeners

    public override void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            MgrDialog.HideDialog ("Confirm", 0f);
            if (e.target.name == "Btn1") {
                callback (1);
            } else if (e.target.name == "Btn2") {
                callback (2);
            } else if (e.target.name == "Btn3") {
                callback (3);
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
        
        try {
            ConfirmConfig config = obj as ConfirmConfig;
            ModUIUtils.GetChild<Text> (transform, "Bg/Title").text = config.title;
            ModUIUtils.GetChild<Text> (transform, "Bg/Content").text = config.content;

            ModUIUtils.GetChild<Text> (transform, "Bg/Btn1/Text").text = config.btn1Text;
            ModUIUtils.GetChild<Text> (transform, "Bg/Btn2/Text").text = config.btn2Text;
            ModUIUtils.GetChild<Text> (transform, "Bg/Btn3/Text").text = config.btn3Text;

            ModUIUtils.GetChild<Button> (transform, "Bg/Btn1").gameObject.SetActive (config.btn1Enable);
            ModUIUtils.GetChild<Button> (transform, "Bg/Btn2").gameObject.SetActive (config.btn2Enable);
            ModUIUtils.GetChild<Button> (transform, "Bg/Btn3").gameObject.SetActive (config.btn3Enable);

            callback = config.callback;
        } catch (Exception e) {
            Log.Error (e);
        }
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
    }

    public override void DestoryScene ()
    {
    }

    public override void UpdateLanguage ()
    {

    }
    #if UNITY_ANDROID
    public override void AndroidGoBack ()
    {
        MgrDialog.HideDialog ("Confirm", 0f);
        callback (0);
    }
    #endif
    #endregion
}

public class ConfirmConfig
{
    public string title = "";
    public string content = "content";

    public bool btn1Enable { get { return !string.IsNullOrEmpty (btn1Text); } }

    public bool btn2Enable { get { return !string.IsNullOrEmpty (btn2Text); } }

    public bool btn3Enable { get { return !string.IsNullOrEmpty (btn3Text); } }


    public string btn1Text = "";
    public string btn2Text = "OK";
    public string btn3Text = "";
    public Action<int> callback;

    public ConfirmConfig ()
    {
        
    }

    public ConfirmConfig (string title, string content)
    {
        this.title = title;
        this.content = content;
    }

    public void SetCallback (Action<int> callback)
    {
        this.callback = callback;
    }

    public void SetTitle (string title)
    {
        this.title = title;
    }

    public void SetContent (string content)
    {
        this.content = content;
    }

    /// <summary>
    /// Sets the button1 text.(set "" means do not use button1)
    /// </summary>
    /// <param name="text">Text.</param>
    public void SetButton1Text (string text)
    {
        this.btn1Text = text;
    }

    /// <summary>
    /// Sets the button2 text.(set "" means do not use button2)
    /// </summary>
    /// <param name="text">Text.</param>
    public void SetButton2Text (string text)
    {
        this.btn2Text = text;
    }

    /// <summary>
    /// Sets the button3 text.(set "" means do not use button3)
    /// </summary>
    /// <param name="text">Text.</param>
    public void SetButton3Text (string text)
    {
        this.btn3Text = text;
    }
}