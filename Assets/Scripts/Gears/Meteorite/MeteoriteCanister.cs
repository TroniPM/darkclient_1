using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class MeteoriteCanister : GearParent
{
    public int randomNum;
    public int randomTime;

    public int defaultShootTime;

    public float bombSpeed;
    public float bombRotationSpeed;

    public int bombShakeCamAnimID;
    public float bombShakeCamAnimTime;

    public int bombDeleteTime;

    public bool bombCanAttackDummy = false;
    public float bombRadius;
    public float bombPercentage;
    public int bombDamageMin;
    public int bombDamageMax;

    public Transform targets;
    protected List<Transform> fixTargets;

    protected uint timer { get; set; }
    protected List<uint> listTimer { get; set; }

    protected int counter;


    void Start()
    {
        gearType = "Meteorite";

        ID = (uint)defaultID;

        timer = uint.MaxValue;
        listTimer = new List<uint>();

        GetFixTargets();

        AddListeners();

        // ShootMeteoriteBomb();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(timer);
        RemoveListeners();
    }


    #region 机关触发

    protected override void SetGearEventEnable(uint enableID)
    {
        base.SetGearEventEnable(enableID);
        if (enableID == ID)
            ShootMeteoriteBomb();
    }

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
            ShootMeteoriteBombImmediately();
    }

    #endregion


    #region 断线重连

    protected override void SetGearEnable(uint enableID)
    {
        base.SetGearEnable(enableID);
        if (enableID == ID)
            ShootMeteoriteBomb();
    }

    #endregion


    protected void GetFixTargets()
    {
        fixTargets = new List<Transform>();
        foreach (Transform child in targets)
            fixTargets.Add(child);
    }


    public void ShootMeteoriteBomb()
    {
        if (triggleEnable && MogoWorld.thePlayer.sceneId != MogoWorld.globalSetting.homeScene)
            ShootMeteoriteBombImmediately();
    }

    protected void ShootMeteoriteBombImmediately()
    {
        int curRandomNum = RandomHelper.GetRandomInt((int)(randomNum * 0.8), (int)(randomNum * 1.2));
        if (curRandomNum > fixTargets.Count)
            curRandomNum = fixTargets.Count;

        uint curRandomTime = uint.MaxValue;

        ResortTargets();

        for (int i = 0; i < curRandomNum; i++)
        {
            curRandomTime = (uint)RandomHelper.GetRandomInt(0, randomTime);
            listTimer.Add(TimerHeap.AddTimer(curRandomTime, 0, CreateMeteoriteBomb, fixTargets[i]));
        }
    }

    protected void ResortTargets()
    {
        for (int i = 0; i < fixTargets.Count; i++)
        {
            Swap(fixTargets[i], fixTargets[RandomHelper.GetRandomInt(i, fixTargets.Count)]);
        }
    }

    protected void Swap(Transform a, Transform b)
    {
        Vector3 temp = a.position;
        a.position = b.position;
        b.position = temp;
    }

    protected void CreateMeteoriteBomb(Transform theTarget)
    {
        SubAssetCacheMgr.GetGearInstance("Meteolite.prefab", (prefabName, id, obj) =>
        {
            GameObject go = obj as GameObject;

            if (go == null)
                return;

            go.transform.position = transform.position;

            var script = go.AddComponent<MeteoriteBomb>();

            script.speed = bombSpeed;
            script.rotationSpeed = bombRotationSpeed;

            script.shakeCamAnimID = bombShakeCamAnimID;
            script.shakeCamAnimTime = bombShakeCamAnimTime;

            script.deleteTime = bombDeleteTime;
            script.target = theTarget;

            script.canAttackDummy = bombCanAttackDummy;

            script.radius = bombRadius;
            script.percentage = bombPercentage;
            script.damageMin = bombDamageMin;
            script.damageMax = bombDamageMax;

            CheckAllShootEnd();
        });
    }

    protected void CheckAllShootEnd()
    {
        counter++;
        if (counter == listTimer.Count)
        {
            counter = 0;

            foreach (uint timerID in listTimer)
                TimerHeap.DelTimer(timerID);
            listTimer.Clear();

            if (stateOne)
                stateOne = false;

            timer = TimerHeap.AddTimer((uint)defaultShootTime, 0, ShootMeteoriteBomb);
        }
    }
}
