using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ContainerSwicther : GearParent
{
    public GameObject[] MustHitContainers;
    public GameObject[] NotMustHitContainers;

    void Start()
    {
        gearType = "ContainerSwicther";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }


    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag && triggleEnable)
        {
            if (MustHitContainers != null && MustHitContainers.Length > 0)
                foreach (var item in MustHitContainers)
                {
                    Container containerScript = item.GetComponent<Container>();
                    containerScript.SetMustBeHit();
                }

            if (NotMustHitContainers != null && NotMustHitContainers.Length > 0)
                foreach (var item in NotMustHitContainers)
                {
                    Container containerScript = item.GetComponent<Container>();
                    containerScript.SetNotMustBeHit();
                }
        }
    }

    #endregion
}
