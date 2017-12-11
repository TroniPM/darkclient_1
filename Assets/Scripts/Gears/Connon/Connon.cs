using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.FSM;

public class Connon : GearParent
{
    // public Transform defaultTargetPosition;
    public Barrel barrel;
    public Animation anima;

    public float gravity;

    public int defaultFireTime;
    public int defaultFireRestTime;

    public Transform target;
    public int fireRotateDelay;
    public int fireActionDelay;
    public int firePersonDelay;

    public bool bombCanAttackDummy = false;
    public float bombRadius;
    public float bombPercentage;
    public int bombDamageMin;
    public int bombDamageMax;

    protected Vector3 targetPosition { get; set; }

    protected SkinnedMeshRenderer[] renderers { get; set; }

    protected enum ConnonState
    {
        connon,
        catapult
    }
    protected ConnonState state;

    protected bool connonEnable = true;

    protected uint timerID;
    GameObject bipMasterGo;
    private bool hasFirePerson;

    void Start()
    {
        gearType = "Connon";
        state = ConnonState.connon;
        connonEnable = true;
        targetPosition = target.transform.position;
        timerID = uint.MaxValue;

        hasFirePerson = false;

        EventDispatcher.AddEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, ReturnGround);
    }

    void OnDestroy()
    {
        if (MainUILogicManager.Instance != null)
            MainUILogicManager.Instance.IsAttackable = true;

        TimerHeap.DelTimer(timerID);
        EventDispatcher.RemoveEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, ReturnGround);
    }

    public void TriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            connonEnable = true;
            SetNextFire(other.gameObject);
        }
    }

    public void TriggerStay(Collider other)
    {

    }

    public void TriggerExit(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            connonEnable = false;
            barrel.Rest();
            TimerHeap.DelTimer(timerID);
        }
    }

    protected void SetNextFire(GameObject go)
    {
        if (state == ConnonState.connon)
        {
            uint nextNextFireTime = (uint)RandomHelper.GetRandomInt((int)(0.5 * defaultFireTime), (int)(1.5 * defaultFireTime));

            barrel.AimTarget(go.transform);

            timerID = TimerHeap.AddTimer(nextNextFireTime, 0, Fire, go);
        }
    }

    protected void Fire(GameObject go)
    {
        if (state == ConnonState.connon)
        {
            float h = transform.position.y - go.transform.position.y;
            float s = Vector3.Distance(new Vector3(go.transform.position.x, transform.position.y, go.transform.position.z), transform.position);

            float vy = Mathf.Sqrt(gravity * Mathf.Pow(s, 2) / (4 * s + 8 * h));
            float vx = 2 * vy;

            Vector3 directionX = (new Vector3(go.transform.position.x, transform.position.y, go.transform.position.z) - transform.position).normalized;
            Vector3 directionY = Vector3.up;

            Fire();

            SubAssetCacheMgr.GetGearInstance("Bomb.prefab", (prefabName, dbid, prefabGo) =>
            {
                //GameObject bomb = (GameObject)Instantiate(Resources.Load("Gear/10401_BigGunBall"));

                Bomb temp = (prefabGo as GameObject).AddComponent<Bomb>();
                temp.gravity = gravity;

                temp.vx = vx;
                temp.vy = vy;

                temp.directionX = directionX;
                temp.directionY = directionY;

                temp.canAttackDummy = bombCanAttackDummy;

                temp.radius = bombRadius;
                temp.percentage = bombPercentage;
                temp.damageMin = bombDamageMin;
                temp.damageMax = bombDamageMax;

                (prefabGo as GameObject).transform.position = transform.position;
            });

            ResetConnon(go);
        }
    }

    public void ResetConnon(GameObject go)
    {
        barrel.Rest();
        timerID = TimerHeap.AddTimer((uint)defaultFireRestTime, 0, SetNextFire, go);
    }

    public void SetState()
    {
        state = ConnonState.catapult;

        if (MainUILogicManager.Instance != null)
            MainUILogicManager.Instance.IsAttackable = false;

        MogoWorld.thePlayer.CleanCharging();
        MogoWorld.thePlayer.ClearSkill();
        MogoWorld.thePlayer.ChangeMotionState(MotionState.ROLL);
        MogoWorld.thePlayer.motor.enableStick = false;
    }

    public void SetFirePerson(GameObject go)
    {
        if (state == ConnonState.catapult && !hasFirePerson)
        {
            // LoggerHelper.Debug("Switchhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
            barrel.AimTarget(target);
            TimerHeap.AddTimer((uint)firePersonDelay, 0, FirePerson, go);
            TimerHeap.AddTimer((uint)fireActionDelay, 0, Fire);
            hasFirePerson = true;
        }
    }

    private void Fire()
    {
        anima.Play();
        barrel.PlaySfx();
    }

    private void HideGameObject(GameObject go)
    {
        MogoFXManager.Instance.AlphaFadeOut(go, 0.01f);
        MogoWorld.connoning = true;
        //检测指引消失/出现
        EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
        //让主角飞行过程中不会托管
        EventDispatcher.TriggerEvent(Events.StoryEvent.CGBegin);
        //bipMasterGo = go.transform.FindChild("Bip_master").gameObject;
        //bipMasterGo.SetActive(false);
        //renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        //foreach (var ren in renderers)
        //{
        //    ren.gameObject.SetActive(false);
        //}

        ControlStick.instance.Reset();
        MogoMotor motor = go.GetComponent<MogoMotor>();
        motor.enableStick = false;

        MogoFXManager.Instance.RemoveShadow(MogoWorld.thePlayer.ID);
    }

    private void ShowGameObject(GameObject go)
    {
        MogoFXManager.Instance.AlphaFadeIn(go, 0.01f);
        //foreach (var ren in renderers)
        //{
        //    ren.gameObject.SetActive(true);
        //}
        //bipMasterGo.SetActive(true);
    }

    private void FirePerson(GameObject go)
    {
        if (MainUILogicManager.Instance != null)
            MainUILogicManager.Instance.IsAttackable = false;

        MogoWorld.thePlayer.CleanCharging();
        MogoWorld.thePlayer.ClearSkill();
        MogoWorld.thePlayer.ChangeMotionState(MotionState.ROLL);

        MogoMainCamera.Instance.Shake(5, 0.1f);

        ShowGameObject(go);

        go.transform.LookAt(new Vector3(target.transform.position.x, go.transform.position.y, target.transform.position.z));

        // TimerHeap.AddTimer(200, 0, ShowGameObject, go);
        go.transform.position = transform.position;
        float h = go.transform.position.y - targetPosition.y;
        float s = Vector3.Distance(new Vector3(targetPosition.x, go.transform.position.y, targetPosition.z), go.transform.position);

        float vy = Mathf.Sqrt(gravity * Mathf.Pow(s, 2) / (4 * s + 8 * h));
        float vx = 2 * vy;

        Vector3 directionX = (new Vector3(targetPosition.x, go.transform.position.y, targetPosition.z) - go.transform.position).normalized;
        Vector3 directionY = Vector3.up;

        // 玩家MoveTo
        MogoMotor motor = go.GetComponent<MogoMotor>();
        //motor.enableStick = false;
        motor.SetIfFlying(true);
        motor.SetExrtaSpeed(vx);
        motor.verticalSpeed = vy;
        motor.SetMoveDirection(directionX);

        EventDispatcher.TriggerEvent(Events.GearEvent.TrapBegin, gearType);
    }

    public void SetBarrelRotate(GameObject go)
    {
        HideGameObject(go);
        TimerHeap.AddTimer((uint)fireRotateDelay, 0, barrel.SetRotate, target);
        // barrel.SetRotate(target);
    }

    public void ReturnGround(MonoBehaviour script)
    {
        //Debug.LogError("ReturnGround");
        if (hasFirePerson)
            EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, gearType);
        hasFirePerson = false;
        MogoWorld.connoning = false;
        //检测指引消失/出现
        EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
        //让主角飞行落地后继续托管
        EventDispatcher.TriggerEvent(Events.StoryEvent.CGEnd);
        MogoMainCamera.Instance.Shake(5, 0.1f);

        if (MainUILogicManager.Instance != null)
            MainUILogicManager.Instance.IsAttackable = true;

        MogoWorld.thePlayer.TriggerUniqEvent(Events.FSMMotionEvent.OnRollEnd);

        MogoFXManager.Instance.AddShadow(MogoWorld.thePlayer.GameObject, MogoWorld.thePlayer.ID);
    }
}
