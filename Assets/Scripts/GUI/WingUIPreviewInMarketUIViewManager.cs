using UnityEngine;
using System.Collections;

public class WingUIPreviewInMarketUIViewManager : MFUIUnit
{
    private static WingUIPreviewInMarketUIViewManager m_instance;

    public static WingUIPreviewInMarketUIViewManager Instance
    {
        get
        {
            return WingUIPreviewInMarketUIViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.WingPreviewInMarketUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "WingPreviewInMarketUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);


    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(WingUIPreviewInMarketUILogicManager.Instance);

        RegisterButtonHandler("WingUIReviewInMarketCloseBtn");
        SetButtonClickHandler("WingUIReviewInMarketCloseBtn", OnCloseBtnUp);

        RegisterButtonHandler("WingUIReviewInMarketTipCloseBtn");
        SetButtonClickHandler("WingUIReviewInMarketTipCloseBtn", OnTipCloseBtnUp);

        RegisterButtonHandler("WingUIReviewInMarketBuyBtn");
        SetButtonClickHandler("WingUIReviewInMarketBuyBtn", OnBuyBtnUp);

        RegisterButtonHandler("WingUIReviewInMarketResetBtn");
        SetButtonClickHandler("WingUIReviewInMarketResetBtn", OnResetBtnUp);

        GetTransform("WingUIReviewInMarketModelCam").GetComponentsInChildren<UIViewport>(true)[0].sourceCamera =
            MogoUIManager.Instance.GetMainUICamera();

        GetSprite("WingUIReviewInMarketModelBG").gameObject.AddComponent<WingModelImageInMarket>();
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        DisablePlayerModel();
        MFUIUtils.ShowGameObject(false, m_myGameObject);
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

        Transform cam = GetTransform("WingUIReviewInMarketModelCam");

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

    public void SetTitle(string name)
    {
        SetLabelText("WingUIReviewInMarketTitleText", name);

    }

    public void SetTipTitle(string name)
    {
        SetLabelText("WingUIReviewInMarketTipTitle", name);
    }

    public void SetTipAttr(System.Collections.Generic.List<string> listAttr)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < listAttr.Count)
            {
                SetLabelText(string.Concat("WingUIReviewInMarketTipAttr", i, "Current"), listAttr[i]);
                GetTransform(string.Concat("WingUIReviewInMarketTipAttr", i)).gameObject.SetActive(true);
            }
            else
            {
                GetTransform(string.Concat("WingUIReviewInMarketTipAttr", i)).gameObject.SetActive(false);
            }
        }
    }

    public void ShowTip(bool isShow)
    {
        GetTransform("WingUIReviewInMarketTip").gameObject.SetActive(isShow);
        GetSprite("WingUIReviewInMarketTipCloseBtnBGDown").gameObject.SetActive(isShow);
    }

    public void ShowBuyBtn(bool isShow)
    {
        GetTransform("WingUIReviewInMarketBuyBtn").gameObject.SetActive(isShow);
    }

    public void ShowResetBtn(bool isShow)
    {
        GetTransform("WingUIReviewInMarketResetBtn").gameObject.SetActive(isShow);
    }

    public void SetWingAttr(System.Collections.Generic.List<string> listAttr)
    {
        for (int i = 0; i < 6; ++i)
        {
            if (i < listAttr.Count)
            {
                SetLabelText(string.Concat("WingUIReviewInMarketInfoGrid", i, "Text"), listAttr[i]);
                GetTransform(string.Concat("WingUIReviewInMarketGrid", i)).gameObject.SetActive(true);
            }
            else
            {
                GetTransform(string.Concat("WingUIReviewInMarketGrid", i)).gameObject.SetActive(false);
            }
        }
    }
    void OnCloseBtnUp(int id)
    {
        Debug.LogError("Close");
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.ClosePreview);
    }

    void OnTipCloseBtnUp(int id)
    {
        Debug.LogError("TipClose");

        ShowTip(!GetTransform("WingUIReviewInMarketTip").gameObject.activeSelf);

    }

    void OnBuyBtnUp(int id)
    {
        Debug.LogError("Buy");
        Mogo.Util.EventDispatcher.TriggerEvent(MarketEvent.PreviewBuy);
    }

    void OnResetBtnUp(int id)
    {
        Debug.LogError("Reset");
    }
}
