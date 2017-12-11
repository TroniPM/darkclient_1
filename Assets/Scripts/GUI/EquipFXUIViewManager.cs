using UnityEngine;
using System.Collections;
using Mogo.GameData;

public class EquipFXUIGridData
{
    public string gridIcon;
    public string gridText;
    public string gridProgressText;
    public float gridProgressSize;
    public bool isActive;
}

public class EquipFXUIViewManager : MFUIUnit
{
    private static EquipFXUIViewManager m_instance;

    public static EquipFXUIViewManager Instance
    {
        get
        {
            return EquipFXUIViewManager.m_instance;
        }
    }

    System.Collections.Generic.List<GameObject> m_listFXGrid = new System.Collections.Generic.List<GameObject>();

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.EquipFXUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "EquipFXUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);


    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(EquipFXUILogicManager.Instance);

        RegisterButtonHandler("EquipFXUICloseBtn");
        SetButtonClickHandler("EquipFXUICloseBtn", OnCloseBtnUp);

        RegisterButtonHandler("EquipFXUIIcon0");
        SetButtonClickHandler("EquipFXUIIcon0", OnIcon0Up);

        RegisterButtonHandler("EquipFXUIIcon1");
        SetButtonClickHandler("EquipFXUIIcon1", OnIcon1Up);

        RegisterButtonHandler("EquipFXUIIcon2");
        SetButtonClickHandler("EquipFXUIIcon2", OnIcon2Up);

        GetTransform("EquipFXUIDialogFXGridListCam").GetComponent<UIViewport>().sourceCamera =
            MogoUIManager.Instance.GetMainUICamera();

        SetLabelText("EquipFXUITitleText", Mogo.GameData.LanguageData.GetContent(7528));
        SetLabelText("EquipFXUIIcon0Text", Mogo.GameData.LanguageData.GetContent(7529));
        SetLabelText("EquipFXUIIcon0TextDown", Mogo.GameData.LanguageData.GetContent(7529));
        SetLabelText("EquipFXUIIcon1Text", Mogo.GameData.LanguageData.GetContent(7530));
        SetLabelText("EquipFXUIIcon1TextDown", Mogo.GameData.LanguageData.GetContent(7530));
        SetLabelText("EquipFXUIIcon2Text", Mogo.GameData.LanguageData.GetContent(7531));
        SetLabelText("EquipFXUIIcon2TextDown", Mogo.GameData.LanguageData.GetContent(7531));
    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
        MenuUIViewManager.Instance.ShowAllEquipItem(false);
        MenuUIViewManager.Instance.ShowRefreshEquipFXBtn(true);
        MenuUIViewManager.Instance.ShowIconListMask(true);
        MogoMainCamera.instance.SetActive(false);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MenuUIViewManager.Instance.ShowAllEquipItem(true);
        MenuUIViewManager.Instance.ShowRefreshEquipFXBtn(false);
        MenuUIViewManager.Instance.ShowIconListMask(false);
        MogoMainCamera.instance.SetActive(true);
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    void OnCloseBtnUp(int id)
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.None);
    }

    void OnActiveBtnUp(int id)
    {
        GameObject current = GetTransform("EquipFXUIIconList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].CurrentBtn.gameObject;

        int currentPage = -1;

        switch (current.name)
        {
            case "EquipFXUIIcon0":
                currentPage = 0;
                break;

            case "EquipFXUIIcon1":
                currentPage = 1;
                break;

            case "EquipFXUIIcon2":
                currentPage = 2;
                break;
        }
        //Debug.LogError("Active " + id);

        Mogo.Util.EventDispatcher.TriggerEvent<int,int>("EquipFXUIActiveBtnUp", id,currentPage);
    }

    void OnFXGridUp(int id)
    {
        Debug.LogError(id);
    }

    void OnIcon0Up(int id)
    {
        EquipFXUILogicManager.Instance.FillJewelFXGridList(MenuUILogicManager.Instance.EquipFXLuaTable);
    }

    void OnIcon1Up(int id)
    {
        EquipFXUILogicManager.Instance.FillEquipFXGridList(MenuUILogicManager.Instance.EquipFXLuaTable);
    }

    void OnIcon2Up(int id)
    {
        EquipFXUILogicManager.Instance.FillStrenthFXGridList(MenuUILogicManager.Instance.EquipFXLuaTable);
    }

    public System.Collections.Generic.List<int> m_listCanActiveFXID = new System.Collections.Generic.List<int>();

    public void RefreshFXGrid(System.Collections.Generic.List<EquipFXUIGridData> list)
    {
        for (int i = 0; i < m_listFXGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listFXGrid[i]);
        }

        m_listFXGrid.Clear();
        m_listCanActiveFXID.Clear();
        GetTransform("EquipFXUIDialogFXGridList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Clear();
        GetTransform("EquipFXUIDialogFXGridListCam").localPosition = new Vector3(0, -200f, 0);

        bool isNextShowAsQuestionMark = false;
        for (int i = 0; i < list.Count; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("EquipFXUIDialogFXGrid.prefab", (name, id, go) =>
                {
                    GameObject gameObj = (GameObject)go;

                    gameObj.name = string.Concat("EquipFXUIDialogFXGrid", index);
                    EquipFXUIGrid grid =  gameObj.AddComponent<EquipFXUIGrid>();
                    grid.SetGotText(list[index].gridText);
                    grid.SetIconFG(list[index].gridIcon);

                    if (!isNextShowAsQuestionMark)
                    {
                        grid.SetNotGetText(list[index].gridText);
                    }
                    else
                    {
                        grid.SetNotGetText("???");
                    }

                    if (list[index].gridProgressSize > 1f)
                    {
                        grid.SetPrgressSize(1);
                    }
                    else
                    {
                        grid.SetPrgressSize(list[index].gridProgressSize);
                    }
                    grid.SetProgressText(list[index].gridProgressText);
                    grid.ID = index;
                    grid.SetActiveBtnUpHandler(OnActiveBtnUp);
                    grid.GetComponentsInChildren<MFUIButtonHandler>(true)[0].ID = index;
                    grid.GetComponentsInChildren<MFUIButtonHandler>(true)[0].ClickHandler = OnFXGridUp;
                    
                    if (list[index].isActive)
                    {
                        grid.ShowActiveBtn(false);
                        grid.ShowGotText(true);
                        grid.ShowNotGetText(false);
                        //grid.SetActiveBtnText(LanguageData.GetContent(7532));
                        grid.ShowProgress(false);
                        grid.ShowGetSign(true);
                    }
                    else
                    {
                        if (list[index].gridProgressSize >= 1f)
                        {
                            grid.ShowActiveBtn(true);
                            grid.ShowGotText(false);
                            grid.ShowNotGetText(true);
                            grid.SetActiveBtnText(LanguageData.GetContent(7532));
                            grid.ShowProgress(true);
                            grid.ShowGetSign(false);
                            m_listCanActiveFXID.Add(index);

                        }
                        else
                        {
                            grid.ShowActiveBtn(true);
                            grid.ShowProgress(true);
                            grid.ShowGotText(false);
                            grid.ShowNotGetText(true);
                            grid.ShowGetSign(false);

                            grid.SetActiveBtnText(LanguageData.GetContent(7533));
                        }
                       
                        isNextShowAsQuestionMark = true;
                    }

                    gameObj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera =
                        GetTransform("EquipFXUIDialogFXGridListCam").GetComponentsInChildren<Camera>(true)[0];

                    GetTransform("EquipFXUIDialogFXGridList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(
                        gameObj.GetComponentsInChildren<MogoSingleButton>(true)[0]);

                    MFUIUtils.AttachWidget(gameObj.transform, GetTransform("EquipFXUIDialogFXGridList"));
                    gameObj.transform.localPosition = new Vector3(0, -135f * index, 0);
                    gameObj.transform.localScale = Vector3.one;

                    if (list.Count > 4)
                    {
                        GetTransform("EquipFXUIDialogFXGridListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MINY =
                            -200f - 135f * (list.Count - 4);
                    }
                    else
                    {
                        GetTransform("EquipFXUIDialogFXGridListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -200f;
                    }

                    m_listFXGrid.Add(gameObj);
                });
        }
    }
}
