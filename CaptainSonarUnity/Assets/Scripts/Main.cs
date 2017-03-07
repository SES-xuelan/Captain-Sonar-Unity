using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XLAF.Public;

/*
创建某个类型的第一个实例时,所进行的操作顺序为:
1.静态变量设置为0
2.执行静态变量初始化器
3.执行基类的静态构造函数
4.执行静态构造函数
5.实例变量设置为0
6.执行实例变量初始化器
7.执行基类中合适的实例构造函数
8.执行实例构造函数
*/
using System.Configuration;

public class Main : MonoBehaviour
{
    void Init ()
    {
        MgrScene.destoryOnSceneChange = false;
        Application.targetFrameRate = 60;
        XLAFMain.Init ();
        InitializeAppConfig ();
//        WeChat.WeChatSDK.Init ();
        MgrMutiLanguage.Init ();
    }

    // Use this for initialization
    void Start ()
    {
        Init ();

        MgrScene.SetViewRoot (ModUIUtils.GetChild<Transform> (transform, "SceneViewRoot"));
        MgrDialog.SetDialogRoot (ModUIUtils.GetChild<Transform> (transform, "DialogViewRoot"));
//        MgrFPS.ShowFPS ();
        MgrScene.GotoScene ("MainMenu");
//        MgrScene.GotoScene ("TestAds");
    }


    void InitializeAppConfig ()
    {
        var configFile = Application.streamingAssetsPath + "/app.config";

        Log.Debug ("App Config Path: " + configFile);

        var configFileMap = new ExeConfigurationFileMap () {
            ExeConfigFilename = configFile
        };

        var config = ConfigurationManager.OpenMappedExeConfiguration (
            configFileMap, ConfigurationUserLevel.None);

        var configSystemField = typeof(ConfigurationManager).GetField ("configSystem", 
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        var configSystem = configSystemField.GetValue (null);

        Log.Debug ("ConfigSystem type = " + configSystem.GetType ().ToString ());

        var cfgField = configSystem.GetType ().GetField ("cfg", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        cfgField.SetValue (configSystem, config);        
    }

}
