using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Mogo.Util;

public class ChargeRewardUIButton : MonoBehaviour
{
    public int id;

    Transform m_myTranform;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_ChargeRewardUI;
    }

    void OnPress(bool isOver)
    {
        if (!isOver)
        {
            EventDispatcher.TriggerEvent("ShowChargeRewardByBtn", id);
            // ChargeRewardUILogicManager.Instance.ShowChargeRewardByBtn(id);
        }
    }

    public void FakePress(bool isPressed)
    {
        EventDispatcher.TriggerEvent("ShowChargeRewardByBtn", id);
        // ChargeRewardUILogicManager.Instance.ShowChargeRewardByBtn(id);
    }
}
