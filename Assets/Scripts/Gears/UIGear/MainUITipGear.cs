using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.GameData;

public class MainUITipGear : GearParent
{
    public int UIID;
    public int iconID;
    public int textID;

    protected uint timer;

    void Start()
    {
        gearType = "MainUITipGear";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(timer);
        HideUI();
        RemoveListeners();
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
        {
            ShowUI();
        }
    }

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
        {
            HideUI();
        }
    }

    public void ShowUI()
    {
        //if (ControlStick.instance != null)
        //    ControlStick.instance.Reset();

        timer = TimerHeap.AddTimer(20, 0, () =>
        {
            switch (UIID)
            {
                case 1:
                    MainUIViewManager.Instance.ShowProtectDiamondTip(true);
                    MainUIViewManager.Instance.SetProtectDiamondTipIcon(IconData.dataMap.Get(iconID).path);
                    MainUIViewManager.Instance.SetProtectDiamondTipText(LanguageData.GetContent(textID));
                    break;

                case 2:
                    MainUIViewManager.Instance.ShowAttackOgreTip(true);
                    MainUIViewManager.Instance.SetAttackOgreTipIcon(IconData.dataMap.Get(iconID).path);
                    MainUIViewManager.Instance.SetAttackOgreTipText(LanguageData.GetContent(textID));
                    break;

                default:
                    break;
            }
        });
    }

    public void HideUI()
    {
        switch (UIID)
        {
            case 1:
                MainUIViewManager.Instance.ShowProtectDiamondTip(false);
                break;

            case 2:
                MainUIViewManager.Instance.ShowAttackOgreTip(false);
                break;

            default:
                break;
        }
    }
}
