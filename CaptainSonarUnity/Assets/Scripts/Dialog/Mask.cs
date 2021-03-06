﻿using UnityEngine;
using System.Collections;
using XLAF.Public;
using UnityEngine.UI;

public class Mask : Storyboard
{
    // /Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates/82-C# Storyboard Dialog-NewDialog.cs.txt

    void Update ()
    {
	
    }

    private Text title;


    #region  Storyboard Listeners

    public override void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            if (e.target.name == "button1") {

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
        title = ModUIUtils.GetChild<Text> (transform, "Title");
    }

    public override void WillEnterScene (object obj)
    {
        title.text = obj as string;
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
    }
    #endif
    #endregion
}
