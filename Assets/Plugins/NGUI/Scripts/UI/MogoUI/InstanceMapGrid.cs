using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class InstanceMapGrid : MonoBehaviour
{
    public int id;

    //void OnPress(bool isOver)
    //{
    //    if (!isOver)
    //    {
    //        EventDispatcher.TriggerEvent("SetMapID", id);
    //        EventDispatcher.TriggerEvent("BackToChooseUI");

    //        //InstanceUILogicManager.Instance.SetMapID(id);
    //        //InstanceUILogicManager.Instance.BackToChooseUI();
    //    }
    //}

    void OnClick()
    {
        EventDispatcher.TriggerEvent("SetMapID", id);
        EventDispatcher.TriggerEvent("BackToChooseUI");

        //InstanceUILogicManager.Instance.SetMapID(id);
        //InstanceUILogicManager.Instance.BackToChooseUI();
    }
}

