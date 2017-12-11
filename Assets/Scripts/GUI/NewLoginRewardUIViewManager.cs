using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewLoginRewardGridData
{
    public List<int> listItemID=new List<int>();
    public List<int> listItemNum=new List<int>();
    public bool IsGot;
    public bool IsSendToMailBox;
    public string strDays;
}

public class NewLoginRewardUIViewManager : MFUIUnit
{
    private static NewLoginRewardUIViewManager m_instance;

    public static NewLoginRewardUIViewManager Instance
    {
        get
        {
            return NewLoginRewardUIViewManager.m_instance;
        }
    }


    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.NewLoginRewardUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "NewLoginRewardUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(NewLoginRewardUILogicManager.Instance);

        GetTransform("NewLoginRewardUIRewardListCam").GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = MogoUIManager.Instance.GetMainUICamera();

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
    }

    void OnGetBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.RewardEvent.GetLoginReward, id);
    }

    List<GameObject> m_listRewardGrid = new List<GameObject>();

    public void RefreshGridList(List<NewLoginRewardGridData> list)
    {
        if (m_listRewardGrid.Count > 0)
        {
            return;
        }

        for (int i = 0; i < list.Count; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("NewLoginRewardUIRewardGrid.prefab", (name, id, go) =>
            {
                GameObject gameObj = (GameObject)go;
                gameObj.name = string.Concat("NewLoginRewardUIRewardGrid", index);
                MFUIUtils.AttachWidget(gameObj.transform, GetTransform("NewLoginRewardUIRewardList"));
                gameObj.transform.localPosition = new Vector3(0, -index * 120f, 0);
                gameObj.transform.localScale = Vector3.one;
                gameObj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = GetTransform("NewLoginRewardUIRewardListCam").GetComponentsInChildren<Camera>(true)[0];
                NewLoginRewardUIRewardGrid grid = gameObj.AddComponent<NewLoginRewardUIRewardGrid>();
                gameObj.transform.Find("NewLoginRewardUIGetBtn").GetComponentsInChildren<MFUIButtonHandler>(true)[0].ID = index;
                gameObj.transform.Find("NewLoginRewardUIGetBtn").GetComponentsInChildren<MFUIButtonHandler>(true)[0].ClickHandler = OnGetBtnUp;

                grid.SetDays(list[index].strDays);
                if (list[index].IsGot)
                {
                    grid.ShowItemGotSign(true);
                    grid.ShowSendToMailText(false);
                    grid.ShowGetBtn(false);
                }
                else
                {
                    if (list[index].IsSendToMailBox)
                    {
                        grid.ShowItemGotSign(false);
                        grid.ShowSendToMailText(true);
                        grid.ShowGetBtn(false);
                    }
                    else
                    {
                        grid.ShowGetBtn(true);
                        grid.ShowItemGotSign(false);
                        grid.ShowSendToMailText(false);
                    }
                }

                for (int j = 0; j < 5; ++j)
                {
                    if (j < list[index].listItemID.Count)
                    {
                        grid.ShowItemGrid(true, j);

                        //grid.SetItemImg(list[index].listItemImg[j], j);
                        grid.SetItemID(list[index].listItemID[j], j);
                        grid.SetItemNum(list[index].listItemNum[j].ToString(),j);
                    }
                    else
                    {
                        grid.ShowItemGrid(false, j);
                    }
                }

                if (list.Count > 4)
                {
                    GetTransform("NewLoginRewardUIRewardListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -180f - 120f * (list.Count - 4);
                }
                else
                {
                    GetTransform("NewLoginRewardUIRewardListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -180f;
                }

                m_listRewardGrid.Add(gameObj);
            });
        }
    }
}
