using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class DropRockArea : GearParent
{
    public float triggleRange;

    public bool dropRockCanAttackDummy = false;

    public float dropRockRadius;
    public float dropRockPercentage;
    public int dropRockDamageMin;
    public int dropRockDamageMax;

    public int[] defaultDropTime;
    protected List<uint> timeIDs;

    void Start()
    {
        gearType = "DropRockArea";
        ID = (uint)defaultID;

        timeIDs = new List<uint>();

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }


    #region Åö×²´¥·¢

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (triggleRange != 0)
                {
                    if (Vector3.Distance(transform.position, other.transform.position) < triggleRange)
                    {
                        BeginDrop(other.gameObject);
                    }
                }
                else
                {
                    BeginDrop(other.gameObject);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                for (int i = 0; i < timeIDs.Count; i++)
                {
                    TimerHeap.DelTimer(timeIDs[i]);
                }
            }
        }
    }

    #endregion


    void BeginDrop(GameObject go)
    {
        foreach (int time in defaultDropTime)
        {
            uint dropTime = (uint)RandomHelper.GetRandomInt((int)(0.5 * time), (int)(1.5 * time));
            timeIDs.Add(TimerHeap.AddTimer(dropTime, 0, DropRock, go));
        }
    }

    private void DropRock(GameObject go)
    {
        SubAssetCacheMgr.GetGearInstance("DropRock", (prefabName, dbid, prefabGo) => 
        {
            if (MogoWorld.thePlayer.sceneId != MogoWorld.globalSetting.homeScene)
            {
                var rock = (prefabGo as GameObject).AddComponent<DropRock>();

                rock.canAttackDummy = dropRockCanAttackDummy;

                rock.radius = dropRockRadius;
                rock.percentage = dropRockPercentage;
                rock.damageMin = dropRockDamageMin;
                rock.damageMax = dropRockDamageMax;

                rock.transform.position = RandomHelper.GetRandomVector3InRangeCircle(dropRockRadius, 10) + go.transform.position;
            }
        });
    }
}
