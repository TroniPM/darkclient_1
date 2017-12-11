using UnityEngine;
using System.Collections;

public class WingUIViewManager : MFUIUnit 
{
    private static WingUIViewManager m_instance;

    public static WingUIViewManager Instance
    {
        get
        {
            return WingUIViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.WingUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "WingUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);


    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(WingUILogicManager.Instance);

        RegisterButtonHandler("WingUITitleCloseBtn");
        SetButtonClickHandler("WingUITitleCloseBtn", OnCloseBtnUp);

        RegisterButtonHandler("WingUIListNormalIcon");
        SetButtonClickHandler("WingUIListNormalIcon", OnNormalIconUp);

        RegisterButtonHandler("WingUIListMagicIcon");
        SetButtonClickHandler("WingUIListMagicIcon", OnMagicIconUp);

        RegisterButtonHandler("WingUIListIconListChargeBtn");
        SetButtonClickHandler("WingUIListIconListChargeBtn", OnChargeBtnUp);

        RegisterButtonHandler("WingUITipCloseBtn");
        SetButtonClickHandler("WingUITipCloseBtn", OnTipCloseBtnUp);

        RegisterButtonHandler("WingUITipBuyBtn");
        SetButtonClickHandler("WingUITipBuyBtn", OnTipBuyBtnUp);

        RegisterButtonHandler("WingUITipUpgradeBtn");
        SetButtonClickHandler("WingUITipUpgradeBtn", OnTipUpgradeBtnUp);

        RegisterLabel("WingUITipBuyBtnText");
        

        GetTransform("WingUIListDialogGridListCamera").GetComponentsInChildren<UIViewport>(true)[0].sourceCamera =
            MogoUIManager.Instance.GetMainUICamera();

        GetTransform("WingUIReviewModelCam").GetComponentsInChildren<UIViewport>(true)[0].sourceCamera =
            MogoUIManager.Instance.GetMainUICamera();

        GetSprite("WingUIReviewModelBG").gameObject.AddComponent<WingModelImge>();
    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
        MogoMainCamera.instance.SetActive(false);
    }

    public override void CallWhenHide()
    {
        DisablePlayerModel();
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MogoMainCamera.instance.SetActive(true);
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    void OnCloseBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.Close);
    }

    void OnNormalIconUp(int id)
    {
        ShowWingTip(false);
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.CommonWing);
    }

    void OnMagicIconUp(int id)
    {
        ShowWingTip(false);
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.MagicWing);
    }

    void OnChargeBtnUp(int id)
    {
        Debug.LogError("ChargeBtnUp");
    }

    void OnTipCloseBtnUp(int id)
    {
        ShowWingTip(false);
    }

    void OnTipBuyBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.TipBuyClick);
    }

    void OnTipUpgradeBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.OpenUpgrade, id);
    }

    public void ShowTipUpgradeBtn(bool isShow)
    {
        GetTransform("WingUITipUpgradeBtn").gameObject.SetActive(isShow);
    }

    public void ShowTipBuyBtn(bool isShow)
    {
        GetTransform("WingUITipBuyBtn").gameObject.SetActive(isShow);
    }

    public void SetBuyText(string name)
    {
        GetLabel("WingUITipBuyBtnText").text = name;
    }

    public void SetGold(string gold)
    {
        SetLabelText("WingUITitleGoldText", gold);
    }

    public void SetDiamond(string diamond)
    {
        SetLabelText("WingUITitleDiamondText", diamond);
    }

    public void SetTitle(string title)
    {
        SetLabelText("WingUITitleText", title);
    }

    public void SetWingAttribute(System.Collections.Generic.List<string> attrList)
    {
        for (int i = 0; i < 6; ++i)
        {
            if (i < attrList.Count)
            {
                GetTransform(string.Concat("WingUIReviewInfoGrid", i)).gameObject.SetActive(true);
                SetLabelText(string.Concat("WingUIReviewInfoGrid", i, "Text"), attrList[i]);
            }
            else
            {
                GetTransform(string.Concat("WingUIReviewInfoGrid", i)).gameObject.SetActive(false);
            }
        }
    }

    public void ShowWingTip(bool isShow)
    {
        GetTransform("WingUITip").gameObject.SetActive(isShow);
    }

    public void SetWingTipTitle(string name)
    {
        SetLabelText("WingUITipTitle", name);
    }

    public void SetWingTipCurAttr(string attr,int id)
    {
        SetLabelText(string.Concat("WingUITipAttr",id,"Current"),attr);
    }

    public void SetWingTipNextAttr(string attr, int id)
    {
        SetLabelText(string.Concat("WingUITipAttr", id, "Next"), attr);
    }

    public void SetWingTipAttrDescripe(string descripe, int id)
    {
        SetLabelText(string.Concat("WingUITipAttr", id, "Descripe"), descripe);
    }

    System.Collections.Generic.List<GameObject> gridList = new System.Collections.Generic.List<GameObject>();

    public void RefreshWingGridList(System.Collections.Generic.List<WingGridData> dataList)
    {
        for (int i = 0; i < gridList.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(gridList[i].gameObject);
        }

        gridList.Clear();
        GetTransform("WingUIListDialogGridList").GetComponent<MogoSingleButtonList>().SingleButtonList.Clear();
        GetTransform("WingUIListDialogGridListCamera").localPosition = new Vector3(0, -166f, 0);

        for (int i = 0; i < dataList.Count; ++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("WingUIListDialogGrid.prefab", (name, id, go) =>
                {
                    GameObject gameObj = (GameObject)go;
                    MFUIUtils.AttachWidget(gameObj.transform, GetTransform("WingUIListDialogGridList"));
                    gameObj.transform.localPosition = new Vector3(0, - 112 * index, 0);
                    gameObj.GetComponent<MyDragCamera>().RelatedCamera = GetTransform("WingUIListDialogGridListCamera").GetComponent<Camera>();
                    GetTransform("WingUIListDialogGridList").GetComponent<MogoSingleButtonList>().SingleButtonList.Add(
                        gameObj.GetComponent<MogoSingleButton>());
                    WingUIGrid uigrid = gameObj.AddComponent<WingUIGrid>();
                    uigrid.ID = index;

                    WingGridData data = dataList[index];

                    if (data.IsHave == false)
                    {
                        uigrid.ShowBuyBtn(true);
                        uigrid.ShowUsedSign(false);
                        uigrid.SetWingCost(data.WingPrice);
                        uigrid.SetWingName(data.WingName);
                        uigrid.SetWingStatus(data.WingStatus);
                        uigrid.SetWingDescripte(data.WingDescripe);
                        uigrid.ShowWingLock(false);
                        uigrid.ShowWingProgress(false);
                        uigrid.ShowWingIconBlackWhite(true);
                        uigrid.ShowCost(true);
                    }
                    else
                    {
                        if (data.IsActive == true)
                        {
                            if (data.IsUsing == false)
                            {
                                uigrid.ShowBuyBtn(false);
                                uigrid.ShowUsedSign(false);
                                uigrid.SetWingName(data.WingName);
                                uigrid.SetWingStatus(data.WingStatus);
                                uigrid.SetWingDescripte(data.WingDescripe);
                                uigrid.ShowWingLock(false);
                                //uigrid.ShowWingProgress(false);
                                uigrid.ShowWingProgress(true);
                                uigrid.SetWingProgressText(string.Concat(data.WingCurExp, "/", data.WingTotalExp));
                                uigrid.SetWingProgressSize((float)data.WingCurExp / (float)data.WingTotalExp);
                                uigrid.ShowWingIconBlackWhite(false);
                                uigrid.ShowCost(false);
                            }
                            else
                            {
                                uigrid.ShowBuyBtn(false);
                                uigrid.ShowUsedSign(true);
                                uigrid.SetWingName(data.WingName);
                                uigrid.SetWingStatus(data.WingStatus);
                                uigrid.SetWingDescripte(data.WingDescripe);
                                uigrid.ShowWingLock(false);
                                uigrid.ShowWingProgress(true);
                                uigrid.SetWingProgressText(string.Concat(data.WingCurExp, "/", data.WingTotalExp));
                                uigrid.SetWingProgressSize((float)data.WingCurExp / (float)data.WingTotalExp);
                                uigrid.ShowWingIconBlackWhite(false);
                                uigrid.ShowCost(false);
                            }
                        }
                        else
                        {
                            uigrid.ShowBuyBtn(false);
                            uigrid.ShowUsedSign(false);
                            uigrid.SetWingName(data.WingName);
                            uigrid.SetWingStatus(data.WingStatus);
                            uigrid.SetWingDescripte(data.WingDescripe);
                            uigrid.ShowWingLock(true);
                            uigrid.ShowWingProgress(false);
                            uigrid.ShowWingIconBlackWhite(false);
                            uigrid.ShowCost(false);
                        }
                    }
                    

                    if (dataList.Count <= 4)
                    {
                        GetTransform("WingUIListDialogGridListCamera").GetComponent<MyDragableCamera>().MINY =
                            -166f;
                    }
                    else
                    {
                        GetTransform("WingUIListDialogGridListCamera").GetComponent<MyDragableCamera>().MINY =
                            -166f - (dataList.Count - 4) * 112f;
                    }

                    gridList.Add(gameObj);
                });
        }
    }

    public void SetObjectLayer(int layer, GameObject obj)
    {
        if (!obj)
            return;

        obj.layer = layer;

        foreach (Transform item in obj.transform)
        {
            SetObjectLayer(layer, item.gameObject);
        }
    }

    public void LoadPlayerModel(float fOffect = 0, float xOffset = 0, float yOffset = 0, float zOffset = 0, float fLookatOffsetY = 0f,
       float fLookatOffsetX = 0f)
    {
        GameObject obj = MogoWorld.thePlayer.GameObject;

        Transform cam = GetTransform("WingUIReviewModelCam");

        Vector3 pos = obj.transform.Find("slot_camera").position;
        cam.position = pos + obj.transform.forward * fOffect;
        cam.position += obj.transform.right * xOffset;
        cam.position += obj.transform.up * yOffset;
        cam.LookAt(pos + new Vector3(fLookatOffsetX, fLookatOffsetY, 0));

        SetObjectLayer(10, obj);
    }

    public void DisablePlayerModel()
    {
        SetObjectLayer(8, MogoWorld.thePlayer.GameObject);
    }
}

public class WingGridData
{
    public int wingId;
    public string WingName;
    public string WingStatus;
    public string WingIconName;
    public bool IsHave;
    public bool IsActive;
    public bool IsUsing;
    public string WingDescripe;
    public string WingPrice;
    public int WingStarNum;
    public int WingCurExp;
    public int WingTotalExp;
}
