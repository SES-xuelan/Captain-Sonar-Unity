﻿using UnityEngine;
using System.Collections;
using XLAF.Public;

public class TestAds : Storyboard
{
    
    #region  Storyboard Listeners

    public override void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            Log.Debug ("e.target.name:", e.target.name);
            if (e.target.name == "BtnBanner") {

            } else if (e.target.name == "BtnScreen") {

            }
        }
    }

    /*
        creat_scene 加载完界面，还未播放动画（只有界面加载的时候，才会触发；读取缓存界面不会触发）
        will_enter_scene 加载完毕scene，即将播放过渡动画
        enter_scene 播放完毕过渡动画

        will_exit_scene 即将播放退出界面的动画
        exit_scene 播放完退出界面的动画
        destory_scene 销毁界面前触发
    */

    public override void CreatScene (object obj)
    {
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

    public override void OverlayBegan (string overlaySceneceneName)
    {
    }

    public override void OverlayEnded (string overlaySceneceneName)
    {
    }

    #if UNITY_ANDROID
    public override void AndroidGoBack ()
    {
    }
    #endif
    #endregion
}
