using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class UIGear : GearParent
{
    public int UIID;
    protected uint timer;


    void Start()
    {
        gearType = "UIGear";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(timer);
        RemoveListeners();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
            ShowUI();
    }

    public void ShowUI()
    {
        if (ControlStick.instance != null)
            ControlStick.instance.Reset();

        timer = TimerHeap.AddTimer(20, 0, () =>
        {
            switch (UIID)
            {
                case 1:
                    NormalMainUILogicManager.Instance.GearInstancePlayIconUp();
                    break;

                default:
                    break;
            }
        });
    }
}
