using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewChargeRewardUIViewManager : MFUIUnit
{
    private static NewChargeRewardUIViewManager m_instance;

    public static NewChargeRewardUIViewManager Instance
    {
        get
        {
            return NewChargeRewardUIViewManager.m_instance;
        }
    }


    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.NewChargeRewardUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "NewChargeRewardUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(NewChargeRewardUILogicManager.Instance);

        RegisterButtonHandler("NewChargeRewardUIVipBtn");
        SetButtonClickHandler("NewChargeRewardUIVipBtn", VIPBtnUp);

        RegisterButtonHandler("NewChargeRewardUIChargeBtn");
        SetButtonClickHandler("NewChargeRewardUIChargeBtn", ChargeBtnUp);

        RegisterButtonHandler("NewChargeRewardUIGetBtn");
        SetButtonClickHandler("NewChargeRewardUIGetBtn", GetBtnUp);

        GetTransform("NewChargeRewardUIProgressGridListCam").GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = MogoUIManager.Instance.GetMainUICamera();

        //List<string> m_listItem = new List<string>();
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //m_listItem.Add("100");
        //AddChargePriceItem(m_listItem);
    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    private void VIPBtnUp(int id)
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        MogoUIManager.Instance.ShowVIPInfoUI();
    }

    private void ChargeBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.OtherEvent.Charge);
    }

    private void GetBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.RewardEvent.GetChargeReward);
    }

    private void OnProgressGridUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.RewardEvent.SelectReward, id);
    }

    public void SetItemList(List<int> itemIDList)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < itemIDList.Count)
            {
                GetTransform(string.Concat("NewChargeRewardItem", i)).gameObject.SetActive(true);
                InventoryManager.SetIcon(itemIDList[i], GetSprite(string.Concat("NewChargeRewardItem", i, "FG")), 0, null,
                    GetSprite(string.Concat("NewChargeRewardItem", i, "BG")));
            }
            else
            {
                GetTransform(string.Concat("NewChargeRewardItem", i)).gameObject.SetActive(false);
            }
        }
    }

    public void ShowAsCharged(bool isCharged)
    {
        GetTexture("NewChargeRewardUIBG").gameObject.SetActive(!isCharged);
        GetTexture("NewChargeRewardUIBGCharged").gameObject.SetActive(isCharged);
        GetTransform("NewChargeRewardUIVipBtn").gameObject.SetActive(!isCharged);
        GetTransform("NewChargeRewardUIProgress").gameObject.SetActive(isCharged);

        if (!isCharged)
        {
            GetTransform("NewChargeRewardUIChargeBtn").localPosition = new Vector3(212.6f, -65.8f, 0);
        }
        else
        {
            GetTransform("NewChargeRewardUIChargeBtn").localPosition = new Vector3(212.6f, 60f, 0);
        }

    }

    List<GameObject> m_listPriceItem = new List<GameObject>();

    public void ClearChargePriceItem()
    {
        for (int i = 0; i < m_listPriceItem.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listPriceItem[i]);
            m_listPriceItem[i] = null;
        }

        m_listPriceItem.Clear();

        List<Transform> transList = GetTransform("NewChargeRewardUIProgressGridListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].transformList;
        Vector3 pos0 = transList[0].localPosition;
        Vector3 pos1 = transList[1].localPosition;

        transList.Clear();
        GameObject go0 = new GameObject();
        go0.transform.localPosition = pos0;
        GameObject go1 = new GameObject();
        go1.transform.localPosition = pos1;
        transList.Add(go0.transform);
        transList.Add(go1.transform);

        GetTransform("NewChargeRewardUIProgressGridListCam").localPosition = Vector3.zero;
        GetTransform("NewChargeRewardUIProgressGridList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Clear();
    }

    public void AddChargePriceItem(List<string> itemText)
    {
        ClearChargePriceItem();

        int page = itemText.Count / 6 + 1;
        if (page > 2)
        {
            int offset = page - 2;

            for (int i = 0; i < offset; ++i)
            {
                GameObject go = new GameObject();
                go.transform.localPosition = new Vector3(655 * (i + 2), 0, 0);
                GetTransform("NewChargeRewardUIProgressGridListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Add(go.transform);
            }
        }
        for (int i = 0; i < itemText.Count; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("NewChargeRewardUIProgressGrid.prefab", (name, id, obj) =>
            {
                GameObject itemGo = (GameObject)obj;
                MFUIUtils.AttachWidget(itemGo.transform, GetTransform("NewChargeRewardUIProgressGridList"));
                itemGo.transform.localPosition = new Vector3(-273 + index * 109f, 0, 0);
                itemGo.transform.localScale = Vector3.one;

                itemGo.GetComponentsInChildren<MogoSingleButton>(true)[0].ButtonListTransform = GetTransform("NewChargeRewardUIProgressGridList");
                GetTransform("NewChargeRewardUIProgressGridList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(itemGo.GetComponentsInChildren<MogoSingleButton>(true)[0]);

                itemGo.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = GetTransform("NewChargeRewardUIProgressGridListCam").GetComponentsInChildren<Camera>(true)[0];

                itemGo.transform.Find("NewChargeRewardUIProgressGridBtn/NewChargeRewardUIProgressGridBtnText").GetComponentsInChildren<UILabel>(true)[0].text = itemText[index];

                itemGo.transform.Find("NewChargeRewardUIProgressGridBtn").GetComponentsInChildren<MFUIButtonHandler>(true)[0].ClickHandler = OnProgressGridUp;

                itemGo.transform.Find("NewChargeRewardUIProgressGridBtn").GetComponentsInChildren<MFUIButtonHandler>(true)[0].ID = index;

                //if (itemText.Count <= 6)
                //{
                //    GetTransform("NewChargeRewardUIProgressGridListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX = 0;
                //}
                //else
                //{
                //    GetTransform("NewChargeRewardUIProgressGridListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                //        109f * (itemText.Count-6);
                //}

                m_listPriceItem.Add(itemGo);
            });

        }
    }

    public void SetProgressSize(float size)
    {
        GetSprite("NewChargeRewardUIProgressFG").transform.localScale = new Vector3(size, 28, 1);
    }

    public void ShowGetBtn(bool isShow)
    {
        GetTransform("NewChargeRewardUIGetBtn").gameObject.SetActive(isShow);
    }

    public void SetProgressGridText(string text, int id)
    {
        m_listPriceItem[id].transform.Find("NewChargeRewardUIProgressGridBtn/NewChargeRewardUIProgressGridBtnText").GetComponentsInChildren<UILabel>(true)[0].text = text;
    }

    public void ShowProgressGridIconHighLight(bool isHighLight, int id)
    {
        m_listPriceItem[id].transform.Find("NewChargeRewardUIProgressGridBtn/NewChargeRewardUIProgressGridBtnDown").gameObject.SetActive(isHighLight);
    }
}
