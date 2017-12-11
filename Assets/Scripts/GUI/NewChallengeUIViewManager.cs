using UnityEngine;
using System.Collections;

public class NewChallengeUIViewManager : MFUIUnit
{
    private static NewChallengeUIViewManager m_instance;

    public static NewChallengeUIViewManager Instance
    {
        get
        {
            return NewChallengeUIViewManager.m_instance;
        }
    }

    System.Collections.Generic.List<NewChallengeGrid> m_listNewChallengeGrid = new System.Collections.Generic.List<NewChallengeGrid>();

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.NewChallengeUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        m_myGameObject.name = "NewChallengeUI";
        //MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);

         System.Collections.Generic.List<MFUIResourceReqInfo> listInfo = new System.Collections.Generic.List<MFUIResourceReqInfo>();

        for(int i = 0;i < 8;++i)
        {
             int index = i;
            MFUIResourceReqInfo info = new MFUIResourceReqInfo();
            info.id = ID;
            info.path = "NewChallengeUIGrid.prefab";
            info.goName = string.Concat("NewChallengeUIGrid", index);
            listInfo.Add(info);


        }
        MFUIGameObjectPool.GetSingleton().RegisterGameObjectList(listInfo, null, true);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(NewChallengeUILogicManager.Instance);

        RegisterButtonHandler("NewChallengeUIClose");
        SetButtonClickHandler("NewChallengeUIClose", OnCloseUp);

        for (int i = 0; i < 4; ++i)
        {
            MFUIUtils.AttachWidget(MFUIGameObjectPool.GetSingleton().GetGameObject(string.Concat("NewChallengeUIGrid", i)).transform, 
                GetTransform("NewChallengeUIGridListTop").transform);

            MFUIGameObjectPool.GetSingleton().GetGameObject(string.Concat("NewChallengeUIGrid", i)).transform.localPosition =
                new Vector3(m_fGridListStartPos+m_fGridListWidth*i, 0, 0);

            m_listNewChallengeGrid.Add(MFUIGameObjectPool.GetSingleton().GetGameObject(
                string.Concat("NewChallengeUIGrid", i)).AddComponent<NewChallengeGrid>());
        }

        for (int i = 0; i < 4; ++i)
        {
            MFUIUtils.AttachWidget(MFUIGameObjectPool.GetSingleton().GetGameObject(string.Concat("NewChallengeUIGrid", 4+i)).transform,
                GetTransform("NewChallengeUIGridListBottom").transform);

            MFUIGameObjectPool.GetSingleton().GetGameObject(string.Concat("NewChallengeUIGrid", 4+i)).transform.localPosition =
                new Vector3(m_fGridListStartPos + m_fGridListWidth * i, 0, 0);

            m_listNewChallengeGrid.Add(MFUIGameObjectPool.GetSingleton().GetGameObject(
                string.Concat("NewChallengeUIGrid", 4+i)).AddComponent<NewChallengeGrid>());
        }

    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
    }

    float m_fGridListStartPos = -425f;
    float m_fGridListWidth = 283f;

    void OnCloseUp(int i)
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    public void SetGridIcon(string imgName, int id)
    {
        m_listNewChallengeGrid[id].SetIcon(imgName);
    }

    public void ShowGridFX(bool isShow, int id)
    {
        m_listNewChallengeGrid[id].ShowFX(isShow);
    }

    public void SetGridName(string name, int id)
    {
        m_listNewChallengeGrid[id].SetName(name);
    }

    public void SetGridStatus( int id,string text)
    {
        m_listNewChallengeGrid[id].SetStatusText(text);
    }

    public void ShowGridFG(bool isShow, int id)
    {
        m_listNewChallengeGrid[id].ShowFG(isShow);
    }

    public void SetGridEnable(int id,bool isEnable)
    {
        m_listNewChallengeGrid[id].SetEnable(isEnable);
    }

    public void ShowGrid(bool isShow, int id)
    {
        m_listNewChallengeGrid[id].gameObject.SetActive(isShow);
    }

    public void SetGridClickHandle(System.Action act,int id)
{
    m_listNewChallengeGrid[id].ClickHandler = act;
}

    public void HideAllGrid()
    {
        foreach (var item in m_listNewChallengeGrid)
        {
            item.gameObject.SetActive(false);
        }
    }

}
