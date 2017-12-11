using UnityEngine;
using System.Collections;

public class NewBattleRewardUIViewManager : MFUIUnit
{
    private static NewBattleRewardUIViewManager m_instance;

    public static NewBattleRewardUIViewManager Instance
    {
        get
        {
            return NewBattleRewardUIViewManager.m_instance;
        }
    }

    Camera m_camCardList;
    int cardMaxNum;

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        //ID = MFUIManager.MFUIID.NewBattleRewardUI;
        //MFUIManager.GetSingleton().RegisterUI(MFUIManager.MFUIID.NewBattleRewardUI, m_myGameObject);
        m_myGameObject.name = "NewBattleRewardUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);

        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        RegisterButtonHandler("NewBattleRewardUIOKBtn");

        SetButtonClickHandler("NewBattleRewardUIOKBtn", OnOKBtnUp);

        for (int i = 0; i < 5; ++i)
        {
            RegisterButtonHandler(string.Concat("NewBattleRewardUICard", i));
            SetButtonClickHandler(string.Concat("NewBattleRewardUICard", i), OnCardUp);
        }

        m_camCardList = GetTransform("NewBattleRewardUICardListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_camCardList.GetComponent<UIViewport>().sourceCamera = MogoUIManager.Instance.GetMainUICamera();
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

    void OnOKBtnUp(int id)
    {
        if (cardMaxNum > 0)
            Debug.Log("OK");
    }

    void OnCardUp(int id)
    {
        Debug.Log(id);
    }


    public void ShowOKBtn(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetButtonHandler("NewBattleRewardUIOKBtn").gameObject);
    }

    public void ShowInfoText(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetLabel("NewBattleRewardUIInfoText").gameObject);
    }

    public void SetComboNum(string num)
    {
        SetLabelText("NewBattleRewardUITopComboNum", num);
    }

    public void SetRewardName(int id, string text)
    {
        SetLabelText(string.Concat("NewBattleRewardUIReward", id), text);
    }

    public void SetPassTime(int minute, int second)
    {
        SetPassTime(string.Concat(minute.ToString("d2")," : ",second.ToString("d2")));
    }

    public void SetPassTime(string time)
    {
        SetLabelText("NewBattleRewardUIPassTimeNum",time);
    }

    public void SetFlipCardMaxNum(int num)
    {
        cardMaxNum = num;
    }

    public void SetCardItem(int index, string iconName, string itemName, int number)
    {
        cardMaxNum--;
        // to do 
    }

    public void SetGrade(int grade)
    {
        string texName;

        switch (grade)
        {
            case 0:
                texName = "fb-ds";
                break;

            case 1:
                texName = "fb-da";
                break;

            case 2:
                texName = "fb-db";
                break;

            default:
                texName = "fb-dc";
                break;
        }
        if (GetTexture("NewBattleRewardUIGrade").mainTexture.name != texName)
        {
            MFUIResourceManager.GetSingleton().LoadResource(ID, string.Concat(texName, ".png"), (obj) => 
            {
                SetTexture("NewBattleRewardUIGrade", (Texture)obj);
            });
        }
    }
}
