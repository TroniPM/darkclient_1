using UnityEngine;
using System.Collections;

public class WingUIUpgradeDialogViewManager : MFUIUnit
{

    private static WingUIUpgradeDialogViewManager m_instance;

    public static WingUIUpgradeDialogViewManager Instance
    {
        get
        {
            return WingUIUpgradeDialogViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.WingUIUpgradeDialog;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "WingUIUpgradeDialog";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);


    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(WingUIUpgradeDialogLogicManager.Instance);

        RegisterButtonHandler("WingUIUpgradeDialogCloseBtn");
        SetButtonClickHandler("WingUIUpgradeDialogCloseBtn", OnUpgradeDialogCloseBtnUp);

        RegisterButtonHandler("WingUIUpgradeDialogUpgradeBtn");
        SetButtonClickHandler("WingUIUpgradeDialogUpgradeBtn", OnUpgradeDialogUpgradeBtnUp);

    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
        //MogoMainCamera.instance.SetActive(false);
    }

    public override void CallWhenHide()
    {
        //DisablePlayerModel();
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        //MogoMainCamera.instance.SetActive(true);
        //MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    void OnUpgradeDialogCloseBtnUp(int id)
    {
        Debug.LogError("UpgradeDialogCloseBtnUp");
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.WingUI);
        WingUILogicManager.Instance.SetUIDirty();
    }

    void OnUpgradeDialogUpgradeBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.Upgrade, id);
    }

    public void SetUpgradeDialogTitle(string title)
    {
        SetLabelText("WingUIUpgradeDialogTitleText", title);
    }

    public void SetUpgradeDialogCurrentLevel(string level)
    {
        SetLabelText("WingUIUpgradeDialogCurrentLevel", level);
        SetLabelText("WingUIUpgradeDialogCurrentLevelPro", level);
    }

    public void SetUpgradeDialogNextLevel(string level)
    {
        SetLabelText("WingUIUpgradeDialogNextLevel", level);
        SetLabelText("WingUIUpgradeDialogNextLevelPro", level);
    }

    public void SetUpgradeDialogCost(string cost)
    {
        SetLabelText("WingUIUpgradeDialogInfoCostNum", cost);
    }

    public void SetUpgradeDialogIcon(string imgName)
    {
        SetSpriteImage("WingUIUpgradeDialogIconFG", imgName);
    }

    public void SetUpgradeDialogCurrentAttr(string attr, int id)
    {
        SetLabelText(string.Concat("WingUIUpgradeDialogInfoGrid", id, "Text"), attr);
    }

    public void SetUpgradeDialogNextAttr(string attr, int id)
    {
        SetLabelText(string.Concat("WingUIUpgradeDialogInfoGrid", id, "NextText"), attr);
    }

    public void SetUpgradeDialogProgressText(string text)
    {
        SetLabelText("WingUIUpgradeDialogProgressText", text);
    }

    public void SetUpgradeDialogProgressSize(float size)
    {
        GetSprite("WingUIUpgradeDialogProgressFG").transform.localScale = new Vector3(size * 1835f, 74.4f, 1);
    }

    public void SetUpgradeDialogStarNum(int num)
    {
        if (num == 0)
        {
            GetTransform("WingUIUpgradeDialogStarList").gameObject.SetActive(false);
        }
        else
        {
            GetTransform("WingUIUpgradeDialogStarList").gameObject.SetActive(true);
        }

        for (int i = 0; i < 10; ++i)
        {
            if (i < num)
            {
                GetSprite(string.Concat("WingUIUpgradeDialogStar", i)).gameObject.SetActive(true);
            }
            else
            {

                GetSprite(string.Concat("WingUIUpgradeDialogStar", i)).gameObject.SetActive(false);
            }
        }

        GetTransform("WingUIUpgradeDialogStarList").transform.localPosition = new Vector3(-18 * (num - 1),
            GetTransform("WingUIUpgradeDialogStarList").transform.localPosition.y, 0);
    }
}
