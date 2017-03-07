using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLAF.Public;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using Pomelo.DotNetClient;

public class Captain
{
    private  Text captainCurrInfo;

    private  Transform layer;
    private  string mapName;
    private GameEngine.MapPoint lastPoint;
    private bool isShowingPosition = false;
    private Button mask;
    private Text maskText;

    private  List<GameObject> listLayerChildren = new List<GameObject> ();
    private Dictionary<string,GameEngine.MapPoint> currMap;

    // //////////////////////////////////////////////////////////////////////////////////////////

    public  void Init (Transform layCaptain, string currMapName)
    {
        layer = layCaptain;
        mapName = currMapName;
        currMap = GameEngine.GetEmptyMap (currMapName);
        _Init ();
        _InitMap ();

    }

    public void Destory ()
    {
        currMap.Clear ();
        lastPoint = null;
        mapName = null;

        foreach (GameObject obj in listLayerChildren) {
            GameObject.Destroy (obj);
        }
        Log.Debug ("captain Destory");
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

    // //////////////////////////////////////////////////////////////////////////////////////////

    private  void _Init ()
    {
        ModUtils.ReplaceUIEvents (layer.gameObject, OnUIEvnt);

        captainCurrInfo = ModUIUtils.GetChild<Text> (layer, "CurrInfo");
        mask = ModUIUtils.GetChild<Button> (layer, "Mask");
        maskText = ModUIUtils.GetChild<Text> (layer, "Mask/Text");
        mask.gameObject.SetActive (false);
        maskText.text = "";
    }

    private  void _InitMap ()
    {
        float width = MgrScene.GetRoot ().rect.width / 5f;
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnWest"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnWest"), width * 0.5f);
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnNorth"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnNorth"), width * 1.5f);
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnEast"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnEast"), width * 2.5f);
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnSouth"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnSouth"), width * 3.5f);
        ModUIUtils.ChangeSize (ModUIUtils.GetChild<Button> (layer, "BtnSurfacing"), width);
        ModUIUtils.ChangePos (ModUIUtils.GetChild<Button> (layer, "BtnSurfacing"), width * 4.5f);


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
                currMap [pos].ChangeButtonAppearanceWithState ();
            }
        }

        ModUtils.ReplaceUIEvents (layMap.gameObject, onLayMapEvent);
    }

    private  void OnUIEvnt (UIEvent e)
    {
        if (e.phase == TouchPhase.Ended) {
            if (isShowingPosition)
                return;

            string date = DateTime.Now.ToString ("mm:ss:fff");
            if (e.target.name == "BtnWest") {
                if (lastPoint == null) {
                    Log.Warning ("You must select a start point first");
                    return;
                }
                string pointPos = GameEngine.GetPointThroughDirection (lastPoint.pos, GameEngine.MapPoint.TextType.WEST);
                if (pointPos == "") {
                    Log.Warning ("You can't go to there because you have reached the edge.");
                    return;
                }
                if (currMap [pointPos].isAlreadyPassed) {
                    Log.Warning ("Already passed this point!");
                    return;
                }
                if (currMap [pointPos].state == 0) {
                    Log.Warning ("You can't go there, it is reef!");
                    return;
                }
                currMap [pointPos].OnPassedBy ();
                currMap [pointPos].SetText (GameEngine.MapPoint.TextType.WEST);
                lastPoint = currMap [pointPos];
                captainCurrInfo.text = MgrMutiLanguage.GetString ("Captain_captainCurrInfo", pointPos, currMap [pointPos].area);
                GameEngine.PomeloSendMessageToChat ("SGW", "\n" + date + " west ←", delegate(Message obj) {
//                    Log.Debug ("Message:", obj);
                });

            } else if (e.target.name == "BtnNorth") {
                if (lastPoint == null) {
                    Log.Warning ("You must select a start point first");
                    return;
                }
                string pointPos = GameEngine.GetPointThroughDirection (lastPoint.pos, GameEngine.MapPoint.TextType.NORTH);
                if (pointPos == "") {
                    Log.Warning ("You can't go to there because you have reached the edge.");
                    return;
                }
                if (currMap [pointPos].isAlreadyPassed) {
                    Log.Warning ("Already passed this point!");
                    return;
                }
                if (currMap [pointPos].state == 0) {
                    Log.Warning ("You can't go there, it is reef!");
                    return;
                }
                currMap [pointPos].OnPassedBy ();
                currMap [pointPos].SetText (GameEngine.MapPoint.TextType.NORTH);
                lastPoint = currMap [pointPos];
                captainCurrInfo.text = MgrMutiLanguage.GetString ("Captain_captainCurrInfo", pointPos, currMap [pointPos].area);
                GameEngine.PomeloSendMessageToChat ("SGN", "\n" + date + " north ↑", delegate(Message obj) {
//                    Log.Debug ("Message:", obj);
                });
            } else if (e.target.name == "BtnEast") {
                if (lastPoint == null) {
                    Log.Warning ("You must select a start point first");
                    return;
                }
                string pointPos = GameEngine.GetPointThroughDirection (lastPoint.pos, GameEngine.MapPoint.TextType.EAST);
                if (pointPos == "") {
                    Log.Warning ("You can't go to there because you have reached the edge.");
                    return;
                }
                if (currMap [pointPos].isAlreadyPassed) {
                    Log.Warning ("Already passed this point!");
                    return;
                }
                if (currMap [pointPos].state == 0) {
                    Log.Warning ("You can't go there, it is reef!");
                    return;
                }
                currMap [pointPos].OnPassedBy ();
                currMap [pointPos].SetText (GameEngine.MapPoint.TextType.EAST);
                lastPoint = currMap [pointPos];
                captainCurrInfo.text = MgrMutiLanguage.GetString ("Captain_captainCurrInfo", pointPos, currMap [pointPos].area);
                GameEngine.PomeloSendMessageToChat ("SGE", "\n" + date + " east →", delegate(Message obj) {
//                    Log.Debug ("Message:", obj);
                });
            } else if (e.target.name == "BtnSouth") {
                if (lastPoint == null) {
                    Log.Warning ("You must select a start point first");
                    return;
                }
                string pointPos = GameEngine.GetPointThroughDirection (lastPoint.pos, GameEngine.MapPoint.TextType.SOUTH);
                if (pointPos == "") {
                    Log.Warning ("You can't go to there because you have reached the edge.");
                    return;
                }
                if (currMap [pointPos].isAlreadyPassed) {
                    Log.Warning ("Already passed this point!");
                    return;
                }
                if (currMap [pointPos].state == 0) {
                    Log.Warning ("You can't go there, it is reef!");
                    return;
                }
                currMap [pointPos].OnPassedBy ();
                currMap [pointPos].SetText (GameEngine.MapPoint.TextType.SOUTH);
                lastPoint = currMap [pointPos];
                captainCurrInfo.text = MgrMutiLanguage.GetString ("Captain_captainCurrInfo", pointPos, currMap [pointPos].area);
                GameEngine.PomeloSendMessageToChat ("SGS", "\n" + date + " south ↓", delegate(Message obj) {
//                    Log.Debug ("Message:", obj);
                });
            } else if (e.target.name == "BtnSurfacing") {
                Surfacing ();
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

                string date = DateTime.Now.ToString ("mm:ss:fff");
                GameEngine.MapPoint point = currMap [e.target.name];
                if (point.isAlreadyPassed) {
                    Log.Warning ("Already passed this point!");
                } else {
                    if (lastPoint == null) {
                        point.SetText (GameEngine.MapPoint.TextType.START);
                    } else {
                        GameEngine.MapPoint.TextType dir = GameEngine.GetDirectionThroughPoint (lastPoint.pos, point.pos);
                        if (dir != GameEngine.MapPoint.TextType.NONE) {
                            if (dir == GameEngine.MapPoint.TextType.WEST) {
                                point.SetText (dir);
                                GameEngine.PomeloSendMessageToChat ("SGW", "\n" + date + " west ←", delegate(Message obj) {
//                                    Log.Debug ("Message:", obj);
                                });
                            } else if (dir == GameEngine.MapPoint.TextType.NORTH) {
                                point.SetText (dir);
                                GameEngine.PomeloSendMessageToChat ("SGN", "\n" + date + " north ↑", delegate(Message obj) {
//                                    Log.Debug ("Message:", obj);
                                });
                            } else if (dir == GameEngine.MapPoint.TextType.EAST) {
                                point.SetText (dir);
                                GameEngine.PomeloSendMessageToChat ("SGE", "\n" + date + " east →", delegate(Message obj) {
//                                    Log.Debug ("Message:", obj);
                                });
                            } else if (dir == GameEngine.MapPoint.TextType.SOUTH) {
                                point.SetText (dir);
                                GameEngine.PomeloSendMessageToChat ("SGS", "\n" + date + " south ↓", delegate(Message obj) {
//                                    Log.Debug ("Message:", obj);
                                });
                            }
                        } else {
                            //两点不相邻，不继续执行
                            return;
                        }
                    }
                    point.OnPassedBy ();
                    lastPoint = point;
                    captainCurrInfo.text = MgrMutiLanguage.GetString ("Captain_captainCurrInfo", e.target.name, currMap [e.target.name].area);


                }
            }
        }
    }

    /// <summary>
    /// 潜艇上浮
    /// </summary>
    private void Surfacing ()
    {
        string newStartPoint = lastPoint.pos;

        for (int y = 1; y <= 15; y++) {
            for (int x = 1; x <= 15; x++) {
                string pos = string.Format ("{0}{1}", ModUtils.Ascii2Character (64 + x), y);
                currMap [pos].ReInit ();
            }
        }

        currMap [newStartPoint].SetText (GameEngine.MapPoint.TextType.START);
        currMap [newStartPoint].OnPassedBy ();
        lastPoint = currMap [newStartPoint];
        captainCurrInfo.text = MgrMutiLanguage.GetString ("Captain_captainCurrInfo", newStartPoint, currMap [newStartPoint].area);
    }

}
    
