using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MeteoriteShooter : GearParent
{
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

    protected uint timer { get; set; }
    protected Transform bombTarget;

    void Start()
    {
        gearType = "Meteorite";

        ID = (uint)defaultID;
        timer = uint.MaxValue;

        AddListeners();

        ShootMeteoriteBomb();
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

    #endregion


    #region 断线重连

    protected override void SetGearEnable(uint enableID)
    {
        base.SetGearEnable(enableID);
        if (enableID == ID)
            ShootMeteoriteBomb();
    }

    #endregion


    public void ShootMeteoriteBomb()
    {
        if (triggleEnable && MogoWorld.thePlayer.sceneId != MogoWorld.globalSetting.homeScene)
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
                script.target = MogoWorld.thePlayer.Transform;

                script.canAttackDummy = bombCanAttackDummy;

                script.radius = bombRadius;
                script.percentage = bombPercentage;
                script.damageMin = bombDamageMin;
                script.damageMax = bombDamageMax;

                timer = TimerHeap.AddTimer((uint)defaultShootTime, 0, ShootMeteoriteBomb);
            });
        }
    }
}
