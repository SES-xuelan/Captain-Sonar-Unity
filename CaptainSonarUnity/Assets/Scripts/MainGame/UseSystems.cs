using UnityEngine;
using System.Collections;
using XLAF.Public;
using UnityEngine.UI;

public class UseSystems : Storyboard
{
    // /Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates/82-C# Storyboard Dialog-NewDialog.cs.txt

    void Update ()
    {
	
    }




    private bool busying = false;
    private Sprite canUse;
    private Sprite canNotUse;

    private bool mineCanUse;
    private bool torpedoCanUse;
    private bool droneCanUse;
    private bool sonarCanUse;
    private bool silenceCanUse;
    private bool scenarioCanUse;


    #region  Storyboard Listeners

    public override void OnUIEvent (UIEvent e)
    {
        if (busying)
            return;
        if (e.phase == TouchPhase.Ended) {
            Log.Debug (e.target.name);
            if (e.target.name == "BtnClose") {
                busying = true;
                MgrDialog.HideDialog (this.sceneName, SceneAnimation.fromLeft);
            } else if (e.target.name == "Mine") {
                if (mineCanUse) {
                    if (GameEngine.firstMate.mine.UseSystem ()) {
                        Log.Debug ("use this system succeed");
                        mineCanUse = false;
                        ModUIUtils.GetChild<Image> (transform, "Mine").sprite = canNotUse;

                    } else {
                        Log.Debug ("use this system failed");
                    }
                } else {
                    Log.Warning ("Can't use this system");
                }
            } else if (e.target.name == "Torpedo") {
                if (torpedoCanUse) {
                    if (GameEngine.firstMate.torpedo.UseSystem ()) {
                        Log.Debug ("use this system succeed");
                        torpedoCanUse = false;
                        ModUIUtils.GetChild<Image> (transform, "Torpedo").sprite = canNotUse;
                    } else {
                        Log.Debug ("use this system failed");
                    }
                } else {
                    Log.Warning ("Can't use this system");
                }
            } else if (e.target.name == "Drone") {
                if (droneCanUse) {
                    if (GameEngine.firstMate.drone.UseSystem ()) {
                        Log.Debug ("use this system succeed");
                        droneCanUse = false;
                        ModUIUtils.GetChild<Image> (transform, "Drone").sprite = canNotUse;
                    } else {
                        Log.Debug ("use this system failed");
                    }
                } else {
                    Log.Warning ("Can't use this system");
                }
            } else if (e.target.name == "Sonar") {
                if (sonarCanUse) {
                    if (GameEngine.firstMate.sonar.UseSystem ()) {
                        Log.Debug ("use this system succeed");
                        sonarCanUse = false;
                        ModUIUtils.GetChild<Image> (transform, "Sonar").sprite = canNotUse;
                    } else {
                        Log.Debug ("use this system failed");
                    }
                } else {
                    Log.Warning ("Can't use this system");
                }
            } else if (e.target.name == "Silence") {
                if (silenceCanUse) {
                    if (GameEngine.firstMate.silence.UseSystem ()) {
                        Log.Debug ("use this system succeed");
                        mineCanUse = false;
                        ModUIUtils.GetChild<Image> (transform, "Silence").sprite = canNotUse;
                    } else {
                        Log.Debug ("use this system failed");
                    }
                } else {
                    Log.Warning ("Can't use this system");
                }
            } else if (e.target.name == "Scenario") {
                if (scenarioCanUse) {
                    if (GameEngine.firstMate.scenario.UseSystem ()) {
                        Log.Debug ("use this system succeed");
                        scenarioCanUse = false;
                        ModUIUtils.GetChild<Image> (transform, "Scenario").sprite = canNotUse;
                    } else {
                        Log.Debug ("use this system failed");
                    }
                } else {
                    Log.Warning ("Can't use this system");
                }
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
        canUse = Resources.Load<Sprite> ("Images/MainGame/zeroAlpha");
        canNotUse = Resources.Load<Sprite> ("Images/MainGame/CanNotUse");
        //需要结合各类信息来判断哪些系统能用，哪些不能用
        //需要传递的信息：
        //谁触发的【captain/firstmate】
        //captain可使用所有信息
        //firstmate只能使用无人机和声呐

        //需要根据充能情况来判断哪些系统能用
        mineCanUse = GameEngine.firstMate.mine.isFullCharged;
        torpedoCanUse = GameEngine.firstMate.torpedo.isFullCharged;
        droneCanUse = GameEngine.firstMate.drone.isFullCharged;
        sonarCanUse = GameEngine.firstMate.sonar.isFullCharged;
        silenceCanUse = GameEngine.firstMate.silence.isFullCharged;
        scenarioCanUse = GameEngine.firstMate.scenario.isFullCharged;

        ModUIUtils.GetChild<Image> (transform, "Mine").sprite = mineCanUse ? canUse : canNotUse;
        ModUIUtils.GetChild<Image> (transform, "Torpedo").sprite = torpedoCanUse ? canUse : canNotUse;
        ModUIUtils.GetChild<Image> (transform, "Drone").sprite = droneCanUse ? canUse : canNotUse;
        ModUIUtils.GetChild<Image> (transform, "Sonar").sprite = sonarCanUse ? canUse : canNotUse;
        ModUIUtils.GetChild<Image> (transform, "Silence").sprite = silenceCanUse ? canUse : canNotUse;
        ModUIUtils.GetChild<Image> (transform, "Scenario").sprite = scenarioCanUse ? canUse : canNotUse;
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

    #if UNITY_ANDROID
    public override void AndroidGoBack ()
    {
        if (busying)
            return;
        busying = true;
        MgrDialog.HideDialog (this.sceneName, SceneAnimation.fromLeft);
    }
    #endif
    #endregion
}
