using UnityEngine;
using System.Collections;
using XLAF.Public;
using UnityEngine.UI;
using Pomelo.DotNetClient;

public class Engineer
{
    private Transform layer;

    private Sprite intactMark;
    private Sprite brokenMark;


    //武器 侦查 特殊 辐射
    private int[] weapon;
    private int[] detection;
    private int[] special;
    private int[] radiation;

    private int[] map;
    private Image[] breakdowns;

    private int[] lineYellow;
    private int[] lineRed;
    private int[] lineGray;

    private int[] dirW;
    private int[] dirN;
    private int[] dirE;
    private int[] dirS;

    private Button mask;
    private Text maskText;


    public bool isWeaponCanUse{ get { return true; } }

    public bool isDetectionCanUse{ get { return true; } }

    public bool isSpecialCanUse{ get { return true; } }


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

    public void Init (Transform layEngineer, string mapName)
    {
        layer = layEngineer;
        intactMark = Resources.Load<Sprite> ("Images/MainGame/zeroAlpha");
        brokenMark = Resources.Load<Sprite> ("Images/MainGame/CanNotUse");

        mask = ModUIUtils.GetChild<Button> (layer, "Mask");
        maskText = ModUIUtils.GetChild<Text> (layer, "Mask/Text");
        mask.gameObject.SetActive (false);
        maskText.text = "";

        if (mapName == "Alpha Map 9"
            || mapName == "Bravo Map 9"
            || mapName == "Charlie Map 9"
            || mapName == "Delta Map 9"
            || mapName == "Echo Map 9") {
            // 0未损坏，1损坏
            //起点从1开始，1到24
            map = new int[25];
            //配置各个index
            lineYellow = new int[]{ 1, 2, 3, 21 };
            lineRed = new int[]{ 7, 8, 9, 19 };
            lineGray = new int[]{ 13, 14, 15, 20 };
            dirW = new int[]{ 1, 2, 3, 4, 5, 6 };
            dirN = new int[]{ 7, 8, 9, 10, 11, 12 };
            dirS = new int[]{ 13, 14, 15, 16, 17, 18 };
            dirE = new int[]{ 19, 20, 21, 22, 23, 24 };
            weapon = new int[]{ 1, 8, 11, 15, 16, 21 };
            detection = new int[]{ 3, 4, 10, 13, 19, 23 };
            special = new int[]{ 2, 7, 9, 14, 18, 20 };
            radiation = new int[]{ 5, 6, 12, 17, 22, 24 };


            breakdowns = new Image[25];
            for (int i = 1; i < 25; i++) {
                breakdowns [i] = ModUIUtils.GetChild<Image> (layer, "LayMap/" + i.ToString ());
                ChangeMark (i, false);
            }

        } else {
            Log.Error ("unsupported map : ", mapName);
        }

        ModUtils.ReplaceButtonEvent (layer.gameObject, OnUIEvent);

    }


    public void ChangeMark (int index, bool isBreak)
    {
        // 0未损坏，1损坏
        breakdowns [index].sprite = isBreak ? brokenMark : intactMark;
        map [index] = isBreak ? 1 : 0;
    }

    /// <summary>
    /// 检测标记们
    /// 如果一个区域的都损坏，则潜艇掉血，然后此区域全部修好
    /// 如果所有的辐射标记都损坏，则潜艇掉血，然后此区域全部修好
    /// 如果一条线路上的标记都损坏则修复
    /// </summary>
    /// <returns>如果潜艇掉血，返回<c>true</c> ，没掉血返回<c>false</c></returns>
    public bool CheckMarks ()
    {
        #region 如果一条线路上的标记都损坏则修复
        int res = 1;
        foreach (int index in lineYellow) {
            res = res * map [index];
        }
        if (res == 1) {
            //全部损坏，修复它
            foreach (int index in lineYellow) {
                ChangeMark (index, false);
            }
        }

        res = 1;
        foreach (int index in lineRed) {
            res = res * map [index];
        }
        if (res == 1) {
            //全部损坏，修复它
            foreach (int index in lineRed) {
                ChangeMark (index, false);
            }
        }

        res = 1;
        foreach (int index in lineGray) {
            res = res * map [index];
        }
        if (res == 1) {
            //全部损坏，修复它
            foreach (int index in lineGray) {
                ChangeMark (index, false);
            }
        }
        #endregion

        #region 如果一个区域的都损坏，则潜艇掉血，然后此区域全部修好
        res = 1;
        foreach (int index in dirE) {
            res = res * map [index];
        }
        if (res == 1) {
            foreach (int index in dirE) {
                ChangeMark (index, false);
            }
            return true;
        }

        res = 1;
        foreach (int index in dirN) {
            res = res * map [index];
        }
        if (res == 1) {
            foreach (int index in dirN) {
                ChangeMark (index, false);
            }
            return true;
        }

        res = 1;
        foreach (int index in dirS) {
            res = res * map [index];
        }
        if (res == 1) {
            foreach (int index in dirS) {
                ChangeMark (index, false);
            }
            return true;
        }

        res = 1;
        foreach (int index in dirW) {
            res = res * map [index];
        }
        if (res == 1) {
            foreach (int index in dirW) {
                ChangeMark (index, false);
            }
            return true;
        }
        #endregion

        #region 如果所有的辐射标记都损坏，则潜艇掉血，然后此区域全部修好
        res = 1;
        foreach (int index in radiation) {
            res = res * map [index];
        }
        if (res == 1) {
            foreach (int index in radiation) {
                ChangeMark (index, false);
            }
            return true;
        }
        #endregion

        return false;
    }

    /// <summary>
    /// 修复所有（潜艇上浮的时候用）
    /// </summary>
    public void RepairAll ()
    {
        for (int i = 1; i <= 24; i++) {
            ChangeMark (i, false);
        }
    }

    /// <summary>
    /// 检测系统是否可以使用
    /// System names : weapon/detection/special
    /// </summary>
    /// <returns>系统可以使用返回true，否则返回false</returns>
    /// <param name="systemName">System name.</param>
    public bool CheckSystemCanUse (string systemName)
    {
        int[] system;
        if (systemName == "weapon") {
            system = weapon;
        } else if (systemName == "detection") {
            system = detection;
        } else if (systemName == "special") {
            system = special;
        } else {
            Log.Warning ("un support system:", systemName);
            return false;
        }

        foreach (int index in system) {
            if (map [index] == 1) {
                return false;
            }
        }
        return true;
    }

    public void Destory ()
    {
        //do nothing
        Log.Debug ("engineer Destory");
    }

    private void OnUIEvent (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            int index = 0;
            if (int.TryParse (e.target.name, out index)) {
                ChangeMark (index, true);
                ShowMask ("Waiting captain");
                GameEngine.PomeloSendMessageToChat ("ED", "", delegate(Message obj) {
                    
                });
                if (CheckMarks ()) {
                    Log.Warning ("Your Submarine was lost 1 blood!!");
                }
            }
        }
    }
}
