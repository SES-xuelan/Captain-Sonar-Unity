using UnityEngine;
using System.Collections;
using XLAF.Public;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class RadioOperator
{
    private Transform layer = null;
    private  string mapName;
    private GameEngine.MapPoint lastPoint;
    private bool isShowingPosition = false;
    private  List<GameObject> listLayerChildren = new List<GameObject> ();
    private Dictionary<string,GameEngine.MapPoint> currMap;
    private string currentTxt = "";

    private Dictionary<string , Button> btnBottomTexts = new Dictionary<string, Button> ();
    private InputField selfDefine;

    private Button mask;
    private Text maskText;

    public void Init (Transform layEngineer, string mapName)
    {
        isShowingPosition = false;
        if (layer == null) {
            layer = layEngineer;
            selfDefine = ModUIUtils.GetChild<InputField> (layer, "SelfDefine");

            mask = ModUIUtils.GetChild<Button> (layer, "Mask");
            maskText = ModUIUtils.GetChild<Text> (layer, "Mask/Text");
            selfDefine.onValueChanged.AddListener (SelfDefineOnValueChanged);
            btnBottomTexts.Add ("BtnS", ModUIUtils.GetChild<Button> (layer, "BtnS"));
            btnBottomTexts.Add ("BtnL", ModUIUtils.GetChild<Button> (layer, "BtnL"));
            btnBottomTexts.Add ("BtnU", ModUIUtils.GetChild<Button> (layer, "BtnU"));
            btnBottomTexts.Add ("BtnR", ModUIUtils.GetChild<Button> (layer, "BtnR"));
            btnBottomTexts.Add ("BtnD", ModUIUtils.GetChild<Button> (layer, "BtnD"));
            btnBottomTexts.Add ("BtnM", ModUIUtils.GetChild<Button> (layer, "BtnM"));
            btnBottomTexts.Add ("BtnSelfDefine", ModUIUtils.GetChild<Button> (layer, "BtnSelfDefine"));
            ModUtils.ReplaceUIEvents (layer.gameObject, OnUIEvnt);
        }
        mask.gameObject.SetActive (false);
        maskText.text = "";
        foreach (KeyValuePair<string , Button> kv in btnBottomTexts) {
            kv.Value.image.color = Color.white;
        }
        currentTxt = "S";
        btnBottomTexts ["BtnS"].image.color = Color.red;
        if (this.mapName != mapName) {
            this.mapName = mapName;
            currMap = GameEngine.GetEmptyMap (mapName);
            _InitMap ();
        }       
    }

    private void SelfDefineOnValueChanged (string str)
    {
        if (btnBottomTexts ["BtnSelfDefine"].image.color == Color.red) {
            currentTxt = str;
        }

    }

    public void TapTitle ()
    {
        isShowingPosition = !isShowingPosition;
        foreach (KeyValuePair<string,GameEngine.MapPoint> kv in currMap) {
            if (isShowingPosition)
                kv.Value.ShowPosition ();
            else
                kv.Value.HidePosition ();
        }
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

        foreach (KeyValuePair<string,GameEngine.MapPoint> kv in currMap) {
            kv.Value.ReInit ();
        }
        Log.Debug ("RadioOperator Destory");
    }

    private void _SwitchBottomButtonBackground (string key)
    {
        Button tmp;
        if (btnBottomTexts.TryGetValue (key, out tmp)) {
            foreach (KeyValuePair<string , Button> kv in btnBottomTexts) {
                kv.Value.image.color = Color.white;
            }
            tmp.image.color = Color.red;
        }
    }

    private  void _InitMap ()
    {
        float width = MgrScene.GetRoot ().rect.width / 4f;
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnLeft"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnLeft"), width * 0.5f);
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnUp"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnUp"), width * 1.5f);
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnRight"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnRight"), width * 2.5f);
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnDown"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnDown"), width * 3.5f);


        Transform layMap = ModUIUtils.GetChild<Transform> (layer, "LayMap");
        Rect layMapRect = ModUIUtils.GetChild<RectTransform> (layer, "LayMap").rect;
        float layMapRectHeight = MgrScene.GetRoot ().rect.height - 700f;//100+200+200+100+100
        float shortSize = layMapRect.width < layMapRectHeight ? layMapRect.width : layMapRectHeight;

        ModUIUtils.ChangeSize (ModUIUtils.GetChild<RectTransform> (layer, "LayMap"), shortSize, shortSize);
        Image backgroundMap = layMap.GetComponent<Image> ();
        backgroundMap.sprite = Resources.Load<Sprite> ("Images/MainGame/" + mapName);
        float buttonWidth = shortSize / 15f;

        for (int y = 1; y <= 15; y++) {
            for (int x = 1; x <= 15; x++) {
                string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y);

                UnityEngine.Object _prefab = Resources.Load ("Elements/Button");
                GameObject obj = (GameObject)UnityEngine.Object.Instantiate (_prefab);
                listLayerChildren.Add (obj);
                obj.transform.SetParent (layMap, false);
                obj.name = pos;
                ModUIUtils.GetChild<Text> (obj.transform, "Text").text = "";//pos
                ModUIUtils.ChangeSize (obj.GetComponent<Button> (), buttonWidth, buttonWidth);
                ModUIUtils.ChangePos (obj.GetComponent<Button> (), (x - 1) * (buttonWidth), (y - 1) * (-buttonWidth));
                currMap [pos].SetButtonPrefabObj (obj);
            }
        }

        ModUtils.ReplaceUIEvents (layMap.gameObject, onLayMapEvent);
    }

    private  void OnUIEvnt (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            if (e.target.name == "BtnS") {
                currentTxt = "S";
                _SwitchBottomButtonBackground (e.target.name);
            } else if (e.target.name == "BtnL") {
                currentTxt = "←";
                _SwitchBottomButtonBackground (e.target.name);
            } else if (e.target.name == "BtnU") {
                currentTxt = "↑";
                _SwitchBottomButtonBackground (e.target.name);
            } else if (e.target.name == "BtnR") {
                currentTxt = "→";
                _SwitchBottomButtonBackground (e.target.name);
            } else if (e.target.name == "BtnD") {
                currentTxt = "↓";
                _SwitchBottomButtonBackground (e.target.name);
            } else if (e.target.name == "BtnM") {
                currentTxt = "M";
                _SwitchBottomButtonBackground (e.target.name);
            } else if (e.target.name == "BtnSelfDefine") {
                currentTxt = selfDefine.text;
                _SwitchBottomButtonBackground (e.target.name);

                // 移动地图
            } else if (e.target.name == "BtnLeft") {
                MoveMapLeft ();
            } else if (e.target.name == "BtnUp") {
                MoveMapUp ();
            } else if (e.target.name == "BtnRight") {
                MoveMapRight ();
            } else if (e.target.name == "BtnDown") {
                MoveMapDown ();
            }
        }
    }

    private void onLayMapEvent (UIEvent e)
    {
        //        Log.Debug ("onCaptainLayerEvent", e.ToString ());
        if (e.phase == TouchPhase.Ended) {
            if (isShowingPosition)
                return;

            if (Regex.IsMatch (e.target.name, @"[A-Z]\d+")) {
                Log.Debug ("click map:", e.target.name);

                GameEngine.MapPoint point = currMap [e.target.name];
                point.OnPressed (currentTxt);

            }
        }
    }


    #region Move map

    private void MoveMapUp ()
    {
        for (int x = 1; x <= 15; x++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), 1);
            if (currMap [pos].isAlreadyPassed) {
                //到顶部了，不能向上移动了
                return;
            }
        }

        for (int y = 2; y <= 15; y++) {
            for (int x = 1; x <= 15; x++) {
                string currPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y);
                string gotoPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y - 1);
                currMap [gotoPos].SetFullInfo (currMap [currPos].GetFullInfo ());
            }
        }
        for (int x = 1; x <= 15; x++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), 15);
            currMap [pos].ReInit ();

        }
    }

    private void MoveMapDown ()
    {
        for (int x = 1; x <= 15; x++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), 15);
            if (currMap [pos].isAlreadyPassed) {
                //到达边界，无法移动
                return;
            }
        }

        for (int y = 14; y >= 1; y--) {
            for (int x = 1; x <= 15; x++) {
                string currPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y);
                string gotoPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y + 1);
                currMap [gotoPos].SetFullInfo (currMap [currPos].GetFullInfo ());
            }
        }
        for (int x = 1; x <= 15; x++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), 1);
            currMap [pos].ReInit ();

        }
    }

    private void MoveMapLeft ()
    {
        for (int y = 1; y <= 15; y++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + 1), y);
            if (currMap [pos].isAlreadyPassed) {
                //到达边界，无法移动
                return;
            }
        }

        for (int x = 2; x <= 15; x++) {
            for (int y = 1; y <= 15; y++) {
                string currPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y);
                string gotoPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x - 1), y);
                currMap [gotoPos].SetFullInfo (currMap [currPos].GetFullInfo ());
            }
        }
        for (int y = 1; y <= 15; y++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + 15), y);
            currMap [pos].ReInit ();

        }
    }

    private void MoveMapRight ()
    {
        for (int y = 1; y <= 15; y++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + 15), y);
            if (currMap [pos].isAlreadyPassed) {
                //到达边界，无法移动
                return;
            }
        }

        for (int x = 14; x >= 1; x--) {
            for (int y = 1; y <= 15; y++) {
                string currPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y);
                string gotoPos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x + 1), y);
                currMap [gotoPos].SetFullInfo (currMap [currPos].GetFullInfo ());
            }
        }
        for (int y = 1; y <= 15; y++) {
            string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + 1), y);
            currMap [pos].ReInit ();

        }
    }

    #endregion
}
