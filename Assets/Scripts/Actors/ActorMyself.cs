using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using Mogo.Task;

using System;
using System.IO;
using System.Text;


public class ActorMyself : ActorPlayer<EntityMyself>
{
    private const string EXPORT_FILE_PATH = "Assets/Resources/data/xml";
    private const string EXPORT_FILE_NAME = "GearData.xml";

    public bool isMoving = false;
    public MogoMotor mogoMotor;

    // GearEffectListener参数，作用是记录原来的速度，为变速做记录
    private float preSpeed;

    private float powerTime = 0;
    private bool powering = false;
    private bool chargstart = false;



    // 初始化
    void Start()
    {
        gameObject.layer = 8;
        DontDestroyOnLoad(this);
        // mogoMotor = this.gameObject.AddComponent<MogoMotorMyself>();
        // (mogoMotor as MogoMotorMyself).SwitchSetting(isInCity);

        if (isNeedInitEquip) InitEquipment();

        m_billboardTrans = transform.Find("slot_billboard");

    }




    // 每帧调用
    void Update()
    {
        ActChange();
        ProcessMotionInput();

        if (theEntity == null)
        {
            return;
        }
        if (m_billboardTrans != null && theEntity != null)
        {
            //EventDispatcher.TriggerEvent<Vector3, uint>(BillboardViewManager.BillboardViewEvent.UPDATEBILLBOARDPOS,

            //   m_billboardTrans.position, theEntity.ID);

            BillboardViewManager.Instance.UpdateBillboardPos(m_billboardTrans.position, theEntity.ID);
        }

    }

    private string cmds = "";
    void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            theEntity.taskManager.OnCloseToNPC(theEntity.taskManager.PlayerCurrentTask.npc);
        }
        if (Input.GetButton("1"))
        {
            theEntity.SpellOneAttack();
        }
        else if (Input.GetButton("2"))
        {
            theEntity.SpellTwoAttack();
        }
        else if (Input.GetButton("3"))
        {
            theEntity.SpellThreeAttack();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnNormalAttack);
            if (!powering)
            {
                powerTime = Time.realtimeSinceStartup;
                powering = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            powering = false;
            float t = Time.realtimeSinceStartup;
            if (chargstart)
            {
                if (t - powerTime > 1.5f)
                {
                    theEntity.PowerChargeComplete();
                }
                else
                {
                    theEntity.PowerChargeInterrupt();
                }
            }
            chargstart = false;
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            float tt = Time.realtimeSinceStartup;
            if (tt - powerTime > 0.5f && !chargstart)
            {
                chargstart = true;
                theEntity.PowerChargeStart();
            }
        }
        else
        {
            if (chargstart)
            {
                chargstart = false;
                theEntity.PowerChargeInterrupt();
            }
            powering = false;
        }
        if (MogoWorld.showClientGM)
        {
            GUI.skin.label = GUI.skin.textArea;
            GUI.Label(new Rect(0, 0, 500, 500), MogoWorld.gmcontent);
            cmds = GUI.TextField(new Rect(510, 30, 100, 30), cmds);
            if (GUI.Button(new Rect(510, 80, 40, 20), "gm"))
            {
                EventDispatcher.TriggerEvent(Events.OtherEvent.ClientGM, cmds);
            }
        }
    }

    // 接收输入
    void ProcessMotionInput()
    {
        if (theEntity == null)
        {
            return;
        }
        if (theEntity.deathFlag == 1 || theEntity.currSpellID > 0)
        {
            if (ControlStick.instance.IsDraging)
            {
                (theEntity as EntityMyself).ClearCmdCache();
            }
            return;
        }
        if (mogoMotor.enableStick)
        {
            if (ControlStick.instance.IsDraging)
            {
                isMoving = true;
                //        LoggerHelper.Debug("set to move");
                theEntity.Move();

            }
            else if (!mogoMotor.isMovingToTarget)
            {
                if (isMoving)
                {
                    isMoving = false;
                    theEntity.Idle();
                }
            }
            //else
            //{
            //    isMoving = true;
            //    theEntity.Move();
            //}
        }
        //else
        //{
        //    theEntity.Idle();
        //}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap.Get(604).content, (rst) =>
            {
                if (rst)
                {
                    MogoGlobleUIManager.Instance.ConfirmHide();
                    Application.Quit();
                }
                else
                {
                    MogoGlobleUIManager.Instance.ConfirmHide();
                }
            });

        }
        else if (Input.GetKeyDown(KeyCode.Menu))
        {
			ScreenCapture.CaptureScreenshot("ScreenShot.png");
            //MogoMsgBox.Instance.ShowFloatingText("ScreenShot Saved in " + Application.persistentDataPath);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            theEntity.CreateDuplication();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            theEntity.SetFreeze();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            theEntity.SetThaw();
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {

            StoryManager.Instance.IsOpen = false;
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            MogoUIManager.Instance.ShowOccupyTowerUI(null);
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            StoryManager.Instance.PlayStory(99);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            CommunityUILogicManager.Instance.SetLastWords();
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            StoryManager.Instance.PlayStory(100);
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            for (int i = 0; i < 10; i++)
            {
                if (InstanceMissionChooseUIViewManager.SHOW_MISSION_BY_DRAG)
                {
                    InstanceMissionChooseUIViewManager.Instance.SetMapOpenPage(6);
                    for (int j = 0; j < 6; j++)
                        for (int k = 0; k < 10; k++)
                            InstanceMissionChooseUIViewManager.Instance.SetGridEnable(j, k, true);
                }
                else
                {
                    NewInstanceUIChooseLevelViewManager.Instance.SetGridEnable(i, true);
                }

                InstanceLevelChooseUIViewManager.Instance.SetBtnLevelChooseEnable(0, true);
                InstanceLevelChooseUIViewManager.Instance.SetBtnLevelChooseEnable(1, true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            theEntity.taskManager.CheckTaskRewardShow();
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            theEntity.RpcCall("CliEntitySkillReq", (uint)1, (uint)1);
        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {
            MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.NewChallengeUI);


            EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);


            if (MogoUIManager.Instance.WaitingWidgetName == "ChallengeGrid0")
            {
                TimerHeap.AddTimer(100, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
            }
            EventDispatcher.TriggerEvent(Events.TowerEvent.GetInfo);
            EventDispatcher.TriggerEvent(Events.SanctuaryEvent.QuerySanctuaryInfo);


            NewChallengeUILogicManager.Instance.SetUIDirty();
        }
    }

    // 以下原GearEffectListener的内容
    // 移动进来是为了减少不必要的类
    public void SetControl(bool curIsMovable)
    {
        mogoMotor.isMovable = curIsMovable;
    }

    public void SetMoveTo(Vector3 target, bool bLookAtTarget = false)
    {
        // mogoMotor.MoveTo(target, bLookAtTarget);
    }

    public void SetMoveToDirectly(Vector3 distance)
    {
        transform.position += distance;
    }

    public void SpeedDown(float coefficient)
    {
        if (mogoMotor.speed == preSpeed)
        {
            preSpeed = mogoMotor.speed;
            mogoMotor.SetSpeed(preSpeed * coefficient);
        }
    }

    public void SpeedUp()
    {
        if (mogoMotor.speed != preSpeed)
        {
            mogoMotor.SetSpeed(preSpeed);
        }
    }
}
