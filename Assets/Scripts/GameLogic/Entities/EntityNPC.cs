/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityNPC
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.RPC;
using Mogo.Util;
using Mogo.GameData;
using System;
using System.Collections.Generic;

namespace Mogo.Game
{
    public class EntityNPC : EntityParent
    {
        #region 常量

        public static readonly string NPC_QUESTION_MARK_NAME = "fx_ui_jt_l.prefab";
        public static readonly string NPC_EXCLAMATION_MARK_NAME = "fx_ui_jt_h.prefab";

        public static readonly string SIGN_SLOT = "Bip01_Master/Bip01";

        public static readonly int IDLE_ACTION = -1;
        public static readonly int TALKING_ACTION = -2;

        #endregion

        #region 公共变量

        public enum NPCSignState
        {
            Doing,
            Done,
            None
        }
        public NPCSignState signState = NPCSignState.None;

        public GameObject NPCDoingSign = null;
        public GameObject NPCDoneSign = null;

        public int standbyAction;
        public Dictionary<int, int> actionList;
        public List<int> thinkInterval;
        public List<int> idleTimeRange;
        public uint idleTime;

        #endregion

        #region 私有变量

        private bool isFrush = false;
        private MogoMotorNPC npcMotor;

        protected int fixSum;
        protected Dictionary<int, int> fixActionList;
        protected uint thinkTimer;
        protected uint resetTimer;

        protected ActorNPC ap;

        #endregion

        public EntityNPC()
        {
            OnEnterWorld();
        }

        public override void OnEnterWorld()
        {
            LoggerHelper.Debug("OnEnterWorld: EntityNPC");
            EventDispatcher.AddEventListener(Events.NPCEvent.FrushIcon, FrushIcon);
            EventDispatcher.AddEventListener<int, Transform>(Events.NPCEvent.TurnToPlayer, TurnToPlayer);
            EventDispatcher.AddEventListener<int>(Events.NPCEvent.TalkEnd, TalkEnd);

            thinkTimer = uint.MaxValue;
            resetTimer = uint.MaxValue;
        }

        public override void OnLeaveWorld()
        {
            LoggerHelper.Debug("OnLeaveWorld: EntityNPC");
            EventDispatcher.RemoveEventListener(Events.NPCEvent.FrushIcon, FrushIcon);
            EventDispatcher.RemoveEventListener<int, Transform>(Events.NPCEvent.TurnToPlayer, TurnToPlayer);
            EventDispatcher.RemoveEventListener<int>(Events.NPCEvent.TalkEnd, TalkEnd);

            if (NPCDoingSign != null)
            {
                var tempDoingGo = NPCDoingSign;
                NPCDoingSign = null;
                AssetCacheMgr.ReleaseInstance(tempDoingGo);
            }

            if (NPCDoneSign != null)
            {
                var tempDoneGo = NPCDoingSign;
                NPCDoneSign = null;
                AssetCacheMgr.ReleaseInstance(tempDoneGo);
            }

            if (ap)
                ap.billboardTrans = null;

            BillboardLogicManager.Instance.RemoveBillboard(ID);
            MogoFXManager.Instance.RemoveShadow(ID);

            TimerHeap.DelTimer(thinkTimer);
            TimerHeap.DelTimer(resetTimer);

            if (GameObject)
                MogoWorld.GameObjects.Remove(GameObject.GetInstanceID());

            AssetCacheMgr.ReleaseInstance(GameObject);
        }

        public override void CreateModel()
        {
            //AvatarModelData data = AvatarModelData.dataMap[(int)vocation];
            LoggerHelper.Debug("EntityNPC create:" + ID);

            int modelID = NPCData.dataMap[(int)ID].mode;

            if (!AvatarModelData.dataMap.ContainsKey(modelID) && modelID != 150000)
                return;

            if (modelID != 150000)
            {
                AvatarModelData data = AvatarModelData.dataMap[modelID];

                AssetCacheMgr.GetInstanceAutoRelease(data.prefabName,
                    (prefab, guid, gameObject) =>
                    {
                        GameObject = gameObject as GameObject;
                        Transform = (gameObject as GameObject).transform;
                        Transform.position = position;
                        Transform.eulerAngles = rotation;
                        // Transform.localScale = scale;

                        animator = GameObject.GetComponent<Animator>();
                        SetIdleAction();

                        motor = null;
                        npcMotor = GameObject.AddComponent<MogoMotorNPC>();

                        Transform.tag = "NPC";
                        GameObject.layer = 17;
                        UpdatePosition();

                        ap = Transform.GetComponent<ActorNPC>();
                        if (ap == null)
                        {
                            Transform.gameObject.AddComponent<ActorNPC>();
                            ap = Transform.GetComponent<ActorNPC>();
                            (ap as ActorNPC).theEntity = this;
                        }

                        SphereCollider collider = Transform.gameObject.AddComponent<SphereCollider>();
                        collider.isTrigger = true;
                        collider.center = Vector3.zero;
                        collider.radius = NPCData.dataMap[(int)ID].colliderRange / ((Transform.localScale.x < Transform.localScale.y ? Transform.localScale.x : Transform.localScale.y) < Transform.localScale.z ? (Transform.localScale.x < Transform.localScale.y ? Transform.localScale.x : Transform.localScale.y) : Transform.localScale.z);

                        BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, false);
                        BillboardLogicManager.Instance.SetHead(this);
                        EventDispatcher.TriggerEvent<Vector3,uint>(BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDPOS, Transform.position,ID);

                        MogoFXManager.Instance.AddShadow(GameObject, ID, 1 / Transform.localScale.x, 1 / Transform.localScale.y, 1 / Transform.localScale.z);

                        if (MogoWorld.thePlayer.CurrentTask != null)
                        {
                            if (MogoWorld.thePlayer.CurrentTask.conditionType == 1 && MogoWorld.thePlayer.CurrentTask.condition != null
                                && MogoWorld.thePlayer.CurrentTask.condition.Count >= 3 && TaskData.dataMap.Get(MogoWorld.thePlayer.CurrentTask.condition[2]).npc == (int)ID)
                            {
                                SetNPCSign(NPCSignState.Doing);
                            }

                            if (MogoWorld.thePlayer.CurrentTask.isShowNPCTip == 1)
                            {
                                SetNPCSign(NPCSignState.Done);
                            }
                        }

                        ((GameObject)gameObject).AddComponent<MogoObjOpt>().ObjType = MogoObjType.NPC;

                        if (((GameObject)gameObject).GetComponent<Animator>() != null)
                        {
                            ((GameObject)gameObject).GetComponent<Animator>().enabled = false;
                        }

                        if (((GameObject)gameObject).GetComponent<Animation>() != null)
                        {
                            ((GameObject)gameObject).GetComponent<Animation>().enabled = false;
                        }

                        if (!NPCManager.npcEntitiePosition.ContainsKey((uint)ID))
                            NPCManager.npcEntitiePosition.Add((uint)ID, Transform.position);

                        EventDispatcher.TriggerEvent(Events.TaskEvent.CheckNpcInRange);

                        NPCCheckThink();
                    }
                );
            }
            else
            {
                GameObject = new GameObject();
                GameObject.name = "Sky_NPC";
                Transform = GameObject.transform;
                Transform.position = position;
                Transform.eulerAngles = rotation;
                // Transform.localScale = scale;
                Transform.tag = "NPC";

                UpdatePosition();

                ActorNPC ap = Transform.GetComponent<ActorNPC>();
                if (ap == null)
                {
                    Transform.gameObject.AddComponent<ActorNPC>();
                    ap = Transform.GetComponent<ActorNPC>();
                    (ap as ActorNPC).theEntity = this;
                }

                SphereCollider collider = Transform.gameObject.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.center = Vector3.zero;
                collider.radius = 5;
            }
        }

        public override void UpdatePosition()
        {
            Vector3 point;
            LoggerHelper.Debug("model info " + position.x + " z: " + position.z);
            if (Mogo.Util.MogoUtils.GetPointInTerrain(position.x, position.z, out point) && Transform)
                Transform.position = new Vector3(point.x, point.y, point.z);
            Transform.eulerAngles = new Vector3(0, rotation.y, 0);

        }

        //public void Talk(string message)
        //{
        //    this.RpcCall("talk", "message");
        //}

        public void SetFlush(bool frushFlag)
        {
            isFrush = frushFlag;
        }

        public void FrushIcon()
        {
            if (isFrush)
                EventDispatcher.TriggerEvent<int>(Events.TaskEvent.NPCInSight, (int)ID);
        }

        public void SetNPCSign(NPCSignState state)
        {
            switch (state)
            {
                case NPCSignState.Doing:
                    signState = NPCSignState.Doing;
                    if (NPCDoingSign != null)
                    {
                        NPCDoingSign.SetActive(true);
                    }
                    else
                    {
                        AssetCacheMgr.GetInstanceAutoRelease(NPC_QUESTION_MARK_NAME, (prefabName, dbid, obj) =>
                        {
                            if (obj != null && Transform != null)
                            {
                                NPCDoingSign = obj as GameObject;
                                NPCDoingSign.transform.parent = Transform.Find("slot_billboard");
                                NPCDoingSign.transform.localPosition = Vector3.zero;
                                NPCDoingSign.transform.localRotation = Quaternion.identity;
                                NPCDoingSign.transform.localScale = new Vector3(1, 1, 1);

                                if (signState != NPCSignState.Doing)
                                {
                                    NPCDoingSign.SetActive(false);
                                    LoggerHelper.Error("Are you kidding me?" + ID);
                                }
                                else
                                {
                                    LoggerHelper.Info("NPCDoingSign.SetActive(true)" + ID);
                                }
                            }
                            else
                            {
                                LoggerHelper.Error("find dengyongjian" + ID);
                            }
                        });
                    }
                    break;

                case NPCSignState.Done:
                    signState = NPCSignState.Done;
                    if (NPCDoneSign != null)
                    {
                        NPCDoneSign.SetActive(true);
                    }
                    else
                    {
                        AssetCacheMgr.GetInstanceAutoRelease(NPC_EXCLAMATION_MARK_NAME, (prefabName, dbid, obj) =>
                        {
                            if (obj != null && Transform != null)
                            {
                                NPCDoneSign = obj as GameObject;
                                NPCDoneSign.transform.parent = Transform.Find("slot_billboard");
                                NPCDoneSign.transform.localPosition = Vector3.zero;
                                NPCDoneSign.transform.localRotation = Quaternion.identity;
                                NPCDoneSign.transform.localScale = new Vector3(1, 1, 1);

                                if (signState != NPCSignState.Done)
                                {
                                    NPCDoneSign.SetActive(false);
                                    LoggerHelper.Error("Are you kidding me?" + ID);
                                }
                                else
                                {
                                    LoggerHelper.Info("NPCDoingSign.SetActive(true)" + ID);
                                }
                            }
                            else
                            {
                                LoggerHelper.Error("find dengyongjian" + ID);
                            }
                        });
                    }
                    break;

                case NPCSignState.None:
                    if (NPCDoingSign != null)
                    {
                        NPCDoingSign.SetActive(false);
                    }
                    if (NPCDoneSign != null)
                    {
                        NPCDoneSign.SetActive(false);
                    }
                    break;
            }
        }

        public void TurnToPlayer(int npcID, Transform target)
        {
            TurnToPlayerWithAction(npcID, target, SetTalkAction);
        }

        public void TurnToPlayerWithAction(int npcID, Transform target, Action action)
        {
            if (npcID == ID)
            {
                if (npcMotor != null)
                    npcMotor.TurnTo(target, action);
            }
        }

        public void TalkEnd(int npcID)
        {
            if (npcID == ID)
            {
                SetIdleAction();
            }
        }

        public void SetTalkAction()
        {
            if (animator != null)
            {
                animator.SetInteger("Action", TALKING_ACTION);
            }
        }

        public void SetIdleAction()
        {
            if (animator != null)
            {
                animator.SetInteger("Action", standbyAction);
            }
        }

        protected void NPCCheckThink()
        {
            if (actionList == null || idleTimeRange == null || thinkInterval == null)
                return;

            if (actionList.Count == null || idleTimeRange.Count < 2 || thinkInterval.Count < 2)
                return;

            if (idleTimeRange[0] > idleTimeRange[1] || thinkInterval[0] > thinkInterval[1])
                return;

            SetFixActionList();

            thinkTimer = TimerHeap.AddTimer((uint)RandomHelper.GetRandomInt(idleTimeRange[0], idleTimeRange[1]), 0, NPCThink);
        }

        protected void SetFixActionList()
        {
            fixActionList = new Dictionary<int, int>();
            fixSum = 0;
            foreach (var data in actionList)
            {
                fixSum += data.Value;
                fixActionList.Add(data.Key, fixSum);
            }
        }

        protected void NPCThink()
        {
            if (GameObject == null)
                return;

            if (animator.GetInteger("Action") == TALKING_ACTION)
            {
                thinkTimer = TimerHeap.AddTimer((uint)RandomHelper.GetRandomInt(idleTimeRange[0], idleTimeRange[1]), 0, NPCThink);
                return;
            }

            int ranNum = RandomHelper.GetRandomInt(0, fixSum);
            foreach (var data in fixActionList)
            {
                if (ranNum < data.Value)
                {
                    if (data.Key == standbyAction)
                        thinkTimer = TimerHeap.AddTimer((uint)RandomHelper.GetRandomInt(idleTimeRange[0], idleTimeRange[1]), 0, NPCThink);
                    else
                        thinkTimer = TimerHeap.AddTimer((uint)RandomHelper.GetRandomInt(thinkInterval[0], thinkInterval[1]), 0, NPCThink);

                    if (animator != null && animator.enabled)
                    {
                        animator.SetInteger("Action", data.Key);

                        if (data.Key != standbyAction)
                            resetTimer = TimerHeap.AddTimer(20, 0, () =>
                            {
                                if (animator.GetInteger("Action") != TALKING_ACTION)
                                    animator.SetInteger("Action", standbyAction);
                            });
                    }

                    break;
                }
            }
        }
    }
}