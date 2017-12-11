using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class BattlePassCardListUIViewManager : MFUIUnit
{
    private static BattlePassCardListUIViewManager m_instance;

    public static BattlePassCardListUIViewManager Instance
    {
        get
        {
            return BattlePassCardListUIViewManager.m_instance;
        }
    }

    int flipCount;
    List<int> hasClick = new List<int>();
    bool m_bStartCountDown = false;
    int m_iCountDownTime = 15;
    float m_fDeltaTime = 0;
    const int CountDownTime = 15;

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.BattlePassCardListUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "BattlePassCardListUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(BattlePassCardListUILogicManager.Instance);
        RegisterButtonHandler("BattlePassCardListUIOKBtn");
        SetButtonClickHandler("BattlePassCardListUIOKBtn", OnOKBtnUp);

        for (int i = 0; i < 5; ++i)
        {
            RegisterButtonHandler(string.Concat("BattlePassCardListUICard", i));
            SetButtonClickHandler(string.Concat("BattlePassCardListUICard", i), OnCardBtnUp);

            CardRotateAnim anim = GetTransform(string.Concat("BattlePassCardListUICard", i)).gameObject.AddComponent<CardRotateAnim>();
            anim.Speed = 10;
            anim.CardFX = GetSprite(string.Concat("BattlePassCardListUICard", i, "BGFX")).gameObject;
        }

        GetTransform("BattlePassCardListUICardListCamera").GetComponentsInChildren<UIViewport>(true)[0].sourceCamera =
            MogoUIManager.Instance.GetMainUICamera();

        TimerHeap.AddTimer(100, 0, () => { ShowCard(0); });
        TimerHeap.AddTimer(200, 0, () => { ShowCard(1); });
        TimerHeap.AddTimer(300, 0, () => { ShowCard(2); });
        TimerHeap.AddTimer(400, 0, () => { ShowCard(3); });
        TimerHeap.AddTimer(500, 0, () =>
        {
            ShowCard(4);
            StartCountDown();
        });
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
        BattlePassUIViewManager.Instance.DestroySelf();
    }

    void OnOKBtnUp(int id)
    {
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);

        ShowOKBtn(false);
        EventDispatcher.TriggerEvent(Events.InstanceUIEvent.FlipRestCard);
    }

    void OnCardBtnUp(int id)
    {
        if (flipCount <= 0)
            return;

        if (hasClick.Contains(id))
            return;

        flipCount--;
        hasClick.Add(id);
        EventDispatcher.TriggerEvent(Events.InstanceUIEvent.FlipCard, id);
    }

    public void ShowOKBtn(bool isShow = true)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("BattlePassCardListUIOKBtn").gameObject);
    }

    public void ShowCard(int id, bool isShow = true)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform(string.Concat("BattlePassCardListUICard", id)).gameObject);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, string.Concat("BattlePassCardListUICard", id));
    }

    public void PlayCardAnim(int id, bool showFX = false)
    {
        GetTransform(string.Concat("BattlePassCardListUICard", id)).GetComponent<CardRotateAnim>().Play();
        GetTransform(string.Concat("BattlePassCardListUICard", id)).GetComponent<CardRotateAnim>().ShowCardFX(showFX);
        StopCountDown();
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, string.Concat("BattlePassCardListUICard", id));
    }

    public void SetRewardItem(int id, string imgName)
    {
        GetSprite(string.Concat("BattlePassCardListUICard", id, "ItemFG")).atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        SetSpriteImage(string.Concat("BattlePassCardListUICard", id, "ItemFG"), imgName);
    }

    public void SetRewardItem(int id, int itemId, int num = 1)
    {
        InventoryManager.SetIcon(itemId, GetSprite(string.Concat("BattlePassCardListUICard", id, "ItemFG")), 0, null, null,2);
        SetLabelText(string.Concat("BattlePassCardListUICard", id, "ItemName"), Mogo.GameData.ItemParentData.GetNameWithNum(itemId, num));
    }


    public void SetTipImg(int mark)
    {
        string texName;

        switch (mark)
        {
            case 1:
                texName = "fb-dc";
                break;

            case 2:
                texName = "fb-db";
                break;

            case 3:
                texName = "fb-da";
                break;

            case 4:
                texName = "fb-ds";
                break;

            default:
                texName = "fb-dc";
                break;
        }
        if (GetTexture("BattlePassCardListUICardListTipImg").mainTexture.name != texName)
        {
            MFUIResourceManager.GetSingleton().LoadResource(ID, string.Concat(texName, ".png"), (obj) =>
            {
                SetTexture("BattlePassCardListUICardListTipImg", (Texture)obj);
            });
        }
    }

    public void SetTipText(string text)
    {
        SetLabelText("BattlePassCardListUICardListTipText", text);
    }

    //int id = 0;

    //public override void CallWhenUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.F5))
    //    {
    //        ShowCard(id++);
    //    }
    //}

    public void SetCardCanNotGetItem(List<KeyValuePair<int, int>> result)
    {
        if (result.Count < 5 - hasClick.Count)
            return;

        int resultIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            if (hasClick.Contains(i))
                continue;

            //if (hasClick.Count == 4) 
            //{
            //    EventDispatcher.TriggerEvent(Events.InstanceUIEvent.FlipLastCard, i);
            //    DelayExit();
            //    return;
            //}

            hasClick.Add(i);
            resultIndex = RandomHelper.GetRandomInt(0, result.Count);
            InventoryManager.SetIcon(result[resultIndex].Key, GetSprite(string.Concat("BattlePassCardListUICard", i, "ItemFG")), 0, null, null, 2);
            SetLabelText(string.Concat("BattlePassCardListUICard", i, "ItemName"), Mogo.GameData.ItemParentData.GetNameWithNum(result[resultIndex].Key, result[resultIndex].Value));

            PlayCardAnim(i);
            result.RemoveAt(resultIndex);
        }

        DelayExit();
    }

    protected void DelayExit()
    {
        TimerHeap.AddTimer(2000, 0, () => { EventDispatcher.TriggerEvent(Events.InstanceEvent.WinReturnHome); });
    }

    public void SetFlipCount(int count)
    {
        flipCount = count;
    }

    public void StartCountDown()
    {
        m_bStartCountDown = true;
    }

    public void StopCountDown()
    {
        m_bStartCountDown = false;
        m_fDeltaTime = 0;
        m_iCountDownTime = CountDownTime;
        SetLabelText("BattlePassCardListUICountDown", "");
    }

    public override void CallWhenUpdate()
    {
        if (m_bStartCountDown)
        {
            m_fDeltaTime += Time.deltaTime;

            if (m_fDeltaTime >= 1f)
            {
                --m_iCountDownTime;
                m_fDeltaTime = 0;

                string tmp = string.Concat(m_iCountDownTime, 's');
                SetLabelText("BattlePassCardListUICountDown", Mogo.GameData.LanguageData.GetContent(46985, tmp));

                if (m_iCountDownTime == 0)
                {
                    // EventDispatcher.TriggerEvent("CardListCountDownFinished");
                    StopCountDown();
                    AutoFlipCard();
                    SetLabelText("BattlePassCardListUICountDown", "");
                }
            }
        }
    }

    public void AutoFlipCard()
    {
        uint n = 100;
        for (int i = 0; i < 5; i++)
        {
            if (flipCount <= 0)
            {
                TimerHeap.AddTimer(n + 1500, 0, () =>
                {
                    EventDispatcher.TriggerEvent(Events.InstanceUIEvent.AutoFlipRestCard);
                });
                break;
            }

            if (hasClick.Contains(i))
                continue;

            flipCount--;
            hasClick.Add(i);
            TimerHeap.AddTimer(n, 0, (index) =>
            {
                EventDispatcher.TriggerEvent(Events.InstanceUIEvent.AutoFlipCard, index);
            }, i);
            n += 100;
        }
    }
}
