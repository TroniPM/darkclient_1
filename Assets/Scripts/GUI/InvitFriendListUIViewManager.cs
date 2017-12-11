using UnityEngine;
using System.Collections;

public class InvitFriendListUIViewManager : MFUIUnit
{
    private static InvitFriendListUIViewManager m_instance;

    public static InvitFriendListUIViewManager Instance
    {
        get
        {
            return InvitFriendListUIViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;

        ID = MFUIManager.MFUIID.InvitFriendListUI;
        m_myGameObject.name = "InvitFriendListUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        AttachLogicUnit(InvitFriendListUILogicManager.Instance);

        System.Collections.Generic.List<MFUIResourceReqInfo> listInfo = new System.Collections.Generic.List<MFUIResourceReqInfo>();

        for (int i = 0; i < 10; ++i)
        {
            int index = i;
            MFUIResourceReqInfo info = new MFUIResourceReqInfo();
            info.id = ID;
            info.path = "InvitFriendListUIGrid.prefab";
            info.goName = string.Concat("InvitFriendListGrid", index);
            listInfo.Add(info);

        }

        MFUIGameObjectPool.GetSingleton().RegisterGameObjectList(listInfo, null, true);
    }


    Camera m_camGridList;

    public override void CallWhenCreate()
    {
        RegisterButtonHandler("InvitFriendListUIClose");
        SetButtonClickHandler("InvitFriendListUIClose", OnCloseUp);

        RegisterButtonHandler("InvitFriendListUIRefresh");
        SetButtonClickHandler("InvitFriendListUIRefresh", OnRefreshUp);

        RegisterButtonHandler("InvitFriendListUIFriendIcon");
        SetButtonClickHandler("InvitFriendListUIFriendIcon", OnFriendIconUp);

        RegisterButtonHandler("InvitFriendListUITongIcon");
        SetButtonClickHandler("InvitFriendListUITongIcon", OnTongIconUp);

        m_camGridList = GetTransform("InvitFriendListUIGridListCamera").GetComponentsInChildren<Camera>(true)[0];

        m_camGridList.GetComponent<UIViewport>().sourceCamera = MogoUIManager.Instance.GetMainUICamera();
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
    }

    public override void CallWhenDestroy()
    {
        m_instance = null;
    }

    void OnCloseUp(int id)
    {
 
    }

    void OnRefreshUp(int id)
    { }

    void OnFriendIconUp(int id)
    { }

    void OnTongIconUp(int id)
    { }
}
