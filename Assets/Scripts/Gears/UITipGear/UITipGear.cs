using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.GameData;

public class UITipGear : GearParent
{
    public int widgetID;

    public string widgetName;
    public int widgetArgument;

    public int textID;

    public bool isRepeat = false;
    public bool isClickHide = false;

    protected bool hasTrigger = false;

    void Start()
    {
        gearType = "UITipGear";

        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        HideUITip();
        RemoveListeners();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable && stateOne && !hasTrigger)
            {
                stateOne = false;
                ShowUITip();

                if (!isRepeat)
                    triggleEnable = false;

                hasTrigger = true;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable && stateOne && !hasTrigger)
            {
                stateOne = false;
                ShowUITip();

                if (!isRepeat)
                    triggleEnable = false;

                hasTrigger = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (isRepeat)
                hasTrigger = false;
        }
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            ShowUITip();
    }


    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
            HideUITip();
    }

    protected void ShowUITip()
    {
        if (widgetName != string.Empty && widgetName.Length > 0)
        {
            var type = typeof(MainUIViewManager);
            var method = type.GetMethod("Show" + widgetName);
            if (widgetArgument == -1)
                method.Invoke(MainUIViewManager.Instance, new object[] { true });
            else
                method.Invoke(MainUIViewManager.Instance, new object[] { true, widgetArgument });
        }
        else
            MogoUIManager.Instance.ShowUI(true);

        if (isClickHide)
            TeachUILogicManager.Instance.TruelySetTeachUIFocus(widgetID, LanguageData.GetContent(textID), false, 0, isClickHide, true);
        else
            TeachUILogicManager.Instance.TruelySetTeachUIFocus(widgetID, LanguageData.GetContent(textID), false, 0, isClickHide, false);
    }

    protected void HideUITip()
    {
        TeachUIViewManager.Instance.ShowFingerAnim(false);
        TeachUIViewManager.Instance.ShowTip(Vector3.zero, string.Empty, false);
        TeachUIViewManager.Instance.DestroyCloneObject();
    }
}
