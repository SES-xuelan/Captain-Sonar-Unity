using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLAF.Public;
using Pomelo.DotNetClient;

public class FirstMate
{

    private  Transform layer = null;

    public System mine = null;
    public System torpedo = null;
    public System drone = null;
    public System sonar = null;
    public System silence = null;
    public System scenario = null;

    private Sprite blood0 = null;
    private Sprite blood1 = null;

    private Button mask;
    private Text maskText;

    private void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            Log.Debug ("firstmate onUIEvent", e);
            if (e.target.name == "BtnUseSystem") {
                MgrDialog.ShowDialog ("UsedSystems", "firstmate", 0.1F);
            } else if (e.target.name == "Mine") {
                if (mine.Charge ()) {
                    //
                    ShowMask ("Waiting captain");
                    GameEngine.PomeloSendMessageToChat ("FD", "", delegate(Message obj) {

                    });
                    Log.Debug ("mine charge succeed");
                } else {
                    Log.Debug ("mine charge failed");
                }
            } else if (e.target.name == "Torpedo") {
                if (torpedo.Charge ()) {
                    //
                    ShowMask ("Waiting captain");
                    GameEngine.PomeloSendMessageToChat ("FD", "", delegate(Message obj) {

                    });
                    Log.Debug ("torpedo charge succeed");
                } else {
                    Log.Debug ("torpedo charge failed");
                }
            } else if (e.target.name == "Drone") {
                if (drone.Charge ()) {
                    //
                    ShowMask ("Waiting captain");
                    GameEngine.PomeloSendMessageToChat ("FD", "", delegate(Message obj) {

                    });
                    Log.Debug ("drone charge succeed");
                } else {
                    Log.Debug ("drone charge failed");
                }
            } else if (e.target.name == "Sonar") {
                if (sonar.Charge ()) {
                    //
                    ShowMask ("Waiting captain");
                    GameEngine.PomeloSendMessageToChat ("FD", "", delegate(Message obj) {

                    });
                    Log.Debug ("sonar charge succeed");
                } else {
                    Log.Debug ("sonar charge failed");
                }
            } else if (e.target.name == "Silence") {
                if (silence.Charge ()) {
                    //
                    ShowMask ("Waiting captain");
                    GameEngine.PomeloSendMessageToChat ("FD", "", delegate(Message obj) {

                    });
                    Log.Debug ("silence charge succeed");
                } else {
                    Log.Debug ("silence charge failed");
                }
            } else if (e.target.name == "Scenario") {
                if (scenario.Charge ()) {
                    //
                    ShowMask ("Waiting captain");
                    GameEngine.PomeloSendMessageToChat ("FD", "", delegate(Message obj) {

                    });
                    Log.Debug ("scenario charge succeed");
                } else {
                    Log.Debug ("scenario charge failed");
                }
            } else if (e.target.name.IndexOf ("Blood") >= 0) {
                Image i = e.target.GetComponent<Image> ();
                i.sprite = (i.sprite == blood0) ? blood1 : blood0;
            }
        }
    }


    public void Init (Transform layFirstMate, string mapName)
    {
        if (layer == null) {
            layer = layFirstMate;
            Rect layMapRect = ModUIUtils.GetChild<RectTransform> (layer, "LayMap").rect;
            float layMapRectHeight = MgrScene.GetRoot ().rect.height - 700f;//100+200+200+100+100
            float shortSize = layMapRect.width < layMapRectHeight ? layMapRect.width : layMapRectHeight;
            ModUIUtils.ChangeSize (ModUIUtils.GetChild<RectTransform> (layer, "LayMap"), shortSize, shortSize);

            mask = ModUIUtils.GetChild<Button> (layer, "Mask");
            maskText = ModUIUtils.GetChild<Text> (layer, "Mask/Text");
        }
        mask.gameObject.SetActive (false);
        maskText.text = "";
        if (blood0 == null || blood1 == null) {
            blood0 = blood0 == null ? Resources.Load<Sprite> ("Images/MainGame/zeroAlpha") : blood0;
            blood1 = blood1 == null ? Resources.Load<Sprite> ("Images/MainGame/halfAlpha") : blood1;

            ModUIUtils.GetChild<Image> (layer, "LayMap/Blood1").sprite = blood0;
            ModUIUtils.GetChild<Image> (layer, "LayMap/Blood2").sprite = blood0;
            ModUIUtils.GetChild<Image> (layer, "LayMap/Blood3").sprite = blood0;
            ModUIUtils.GetChild<Image> (layer, "LayMap/Blood4").sprite = blood0;
        }
        //image size: w2792   h2210
        //screensize: w1080   h1080
        //blood size: w180    h100
        // 2792/180=1080/realw  realw=1080*180/2792
        // 2210/100=1080/realh  realh=1080*100/2210

        // 改变血量图片的大小和位置
        //        float bloodButtonWidth = shortSize * 180f / 2792f;
        //        float bloodButtonHeight = shortSize * 100f / 2210f;
        //
        //        ModUIUtils.ChangeSize (bloods [1].gameObject, bloodButtonWidth, bloodButtonHeight);
        //        ModUIUtils.ChangeSize (bloods [2].gameObject, bloodButtonWidth, bloodButtonHeight);
        //        ModUIUtils.ChangeSize (bloods [3].gameObject, bloodButtonWidth, bloodButtonHeight);
        //        ModUIUtils.ChangeSize (bloods [4].gameObject, bloodButtonWidth, bloodButtonHeight);


        // init system and images
        if (mine == null) {
            Image[] mineImages = new Image[3];
            mineImages [0] = ModUIUtils.GetChild<Image> (layer, "LayMap/Mine/1");
            mineImages [1] = ModUIUtils.GetChild<Image> (layer, "LayMap/Mine/2");
            mineImages [2] = ModUIUtils.GetChild<Image> (layer, "LayMap/Mine/3");
            mine = new System (mineImages, 3);
        } 
        if (torpedo == null) {
            Image[] torpedoImages = new Image[3];
            torpedoImages [0] = ModUIUtils.GetChild<Image> (layer, "LayMap/Torpedo/1");
            torpedoImages [1] = ModUIUtils.GetChild<Image> (layer, "LayMap/Torpedo/2");
            torpedoImages [2] = ModUIUtils.GetChild<Image> (layer, "LayMap/Torpedo/3");
            torpedo = new System (torpedoImages, 3);
        }
        if (drone == null) {
            Image[] droneImages = new Image[4];
            droneImages [0] = ModUIUtils.GetChild<Image> (layer, "LayMap/Drone/1");
            droneImages [1] = ModUIUtils.GetChild<Image> (layer, "LayMap/Drone/2");
            droneImages [2] = ModUIUtils.GetChild<Image> (layer, "LayMap/Drone/3");
            droneImages [3] = ModUIUtils.GetChild<Image> (layer, "LayMap/Drone/4");
            drone = new System (droneImages, 4);
        }
        if (sonar == null) {
            Image[] sonarImages = new Image[3];
            sonarImages [0] = ModUIUtils.GetChild<Image> (layer, "LayMap/Sonar/1");
            sonarImages [1] = ModUIUtils.GetChild<Image> (layer, "LayMap/Sonar/2");
            sonarImages [2] = ModUIUtils.GetChild<Image> (layer, "LayMap/Sonar/3");
            sonar = new System (sonarImages, 3);
        }
        if (silence == null) {
            Image[] silenceImages = new Image[6];
            silenceImages [0] = ModUIUtils.GetChild<Image> (layer, "LayMap/Silence/1");
            silenceImages [1] = ModUIUtils.GetChild<Image> (layer, "LayMap/Silence/2");
            silenceImages [2] = ModUIUtils.GetChild<Image> (layer, "LayMap/Silence/3");
            silenceImages [3] = ModUIUtils.GetChild<Image> (layer, "LayMap/Silence/4");
            silenceImages [4] = ModUIUtils.GetChild<Image> (layer, "LayMap/Silence/5");
            silenceImages [5] = ModUIUtils.GetChild<Image> (layer, "LayMap/Silence/6");
            silence = new System (silenceImages, 6);
        }
        if (scenario == null) {
            Image[] scenarioImages = new Image[6];
            scenarioImages [0] = ModUIUtils.GetChild<Image> (layer, "LayMap/Scenario/1");
            scenarioImages [1] = ModUIUtils.GetChild<Image> (layer, "LayMap/Scenario/2");
            scenarioImages [2] = ModUIUtils.GetChild<Image> (layer, "LayMap/Scenario/3");
            scenarioImages [3] = ModUIUtils.GetChild<Image> (layer, "LayMap/Scenario/4");
            scenarioImages [4] = ModUIUtils.GetChild<Image> (layer, "LayMap/Scenario/5");
            scenarioImages [5] = ModUIUtils.GetChild<Image> (layer, "LayMap/Scenario/6");

            if (mapName == "Alpha Map 9") {
                scenario = new System (scenarioImages, 0);
            } else if (mapName == "Bravo Map 9") {
                scenario = new System (scenarioImages, 0);
            } else if (mapName == "Charlie Map 9") {
                scenario = new System (scenarioImages, 0);
            } else if (mapName == "Delta Map 9") {
                scenario = new System (scenarioImages, 0);
            } else if (mapName == "Echo Map 9") {
                scenario = new System (scenarioImages, 4);
            } else if (mapName == "XXXX Map") {
                //!TODO! 特殊地图会有6步充能，目前内置的echo地图为4步充能
                scenario = new System (scenarioImages, 6);
            } else {
                Log.Error ("unsupported map : ", mapName);
            }
        }


        ModUtils.ReplaceButtonEvent (layer.gameObject, OnUIEvent);
    }

    public void ShowMask (string text = "")
    {
        mask.gameObject.SetActive (true);
        maskText.text = text;
    }

    public void HideMask ()
    {
        mask.gameObject.SetActive (false);
    }

    public void ChangeMaskText (string text)
    {
        maskText.text = text;
    }

    public string GetMaskText ()
    {
        return maskText.text;
    }
    public void Destory ()
    {
        mine.ReInit ();
        torpedo.ReInit ();
        drone.ReInit ();
        sonar.ReInit ();
        silence.ReInit ();
        scenario.ReInit ();


        scenario = null;
        Log.Debug ("firstmate Destory");
    }

    /// <summary>
    /// 6大系统
    /// </summary>
    public class System
    {
        public Image[] images;
        public int maxCharge;
        public int currentCharge = 0;
        private bool isDisableThisSystem = false;


        public System (Image[] images, int maxCharge)
        {
            this.maxCharge = maxCharge;
            this.images = images;
            foreach (Image v in this.images) {
                v.color = Color.white;
            }
            this.isDisableThisSystem = (this.maxCharge == 0);
        }

        /// <summary>
        /// 当前系统是否启用（主要用在Scenario系统，因为并不是所有系统都能启用Scenario）
        /// </summary>
        /// <value><c>true</c> if is enable; otherwise, <c>false</c>.</value>
        public bool isEnable{ get { return !this.isDisableThisSystem; } }


        /// <summary>
        /// 当前系统是否充满了
        /// </summary>
        public bool isFullCharged {
            get { 
                if (this.isDisableThisSystem) {
                    return false;
                } else {
                    return this.currentCharge == this.maxCharge;
                }
            }
        }

        /// <summary>
        /// 使用系统（充能清空）
        /// </summary>
        public bool UseSystem ()
        {
            if (this.isDisableThisSystem || !this.isFullCharged) {
                return false;
            }
            this.currentCharge = 0;
            foreach (Image v in this.images) {
                v.color = Color.white;
            }
            return true;
        }

        /// <summary>
        /// 为系统充能
        /// </summary>
        /// <returns>返回是否充能成功</returns>
        public bool Charge ()
        {
            if (this.isFullCharged || this.isDisableThisSystem) {
                return false;
            } else {
                this.currentCharge++;
                for (int i = 0; i < this.currentCharge; i++) {
                    this.images [i].color = Color.red;
                }
                return true;
            }
        }

        public void ReInit ()
        {
            this.isDisableThisSystem = false;
            this.currentCharge = 0;
            foreach (Image v in this.images) {
                v.color = Color.white;
            }
        }
    }
}
