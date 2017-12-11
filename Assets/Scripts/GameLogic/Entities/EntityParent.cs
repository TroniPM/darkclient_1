/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������EntityParent
// �����ߣ�Steven Yang
// �޸����б��
// �������ڣ�2013-1-29
// ģ���������ͻ��� Entity����
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections.Generic;

using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
using Mogo.Task;

namespace Mogo.Game
{
    /// <summary>
    /// �߼������࣬���ﴦ��Entity���ݱ仯��״̬�仯���߼�
    /// </summary>
    public partial class EntityParent : NotifyPropChanged, INotifyPropChanged
    {
        #region �鷽��

        virtual public void MainCameraCompleted()
        {

        }

        virtual public void CreateModel()
        {
            if (GameObject)
            {
                MogoWorld.GameObjects.Add(GameObject.GetInstanceID(), this);
            }
        }

        virtual public void CreateActualModel()
        {

        }

        virtual public void CreateDeafaultModel()
        {

        }

        virtual public void ApplyRootMotion(bool b)
        {
            if (animator == null)
            {
                return;
            }
            animator.applyRootMotion = b;
        }

        virtual public void SetAction(int act)
        {
            if (animator == null)
            {
                return;
            }
            animator.SetInteger("Action", act);
            if (weaponAnimator)
            {
                weaponAnimator.SetInteger("Action", act);
            }
            if (act == ActionConstants.HIT_AIR)
            {
                stiff = true;
                hitAir = true;
                //Actor.HitStateChanged = HitStateChange;
                //TimerHeap.DelTimer(revertHitTimerID);
                //revertHitTimerID = TimerHeap.AddTimer(ActionTime.HIT_AIR, 0, DelayCheck);
            }
            else if (act == ActionConstants.KNOCK_DOWN)
            {
                stiff = true;
                knockDown = true;
                //Actor.HitStateChanged = HitStateChange;
                //TimerHeap.DelTimer(revertHitTimerID);
                //revertHitTimerID = TimerHeap.AddTimer(ActionTime.KNOCK_DOWN, 0, DelayCheck);
            }
            else if (act == ActionConstants.HIT)
            {
                stiff = true;
                //Actor.HitStateChanged = HitStateChange;
                //TimerHeap.DelTimer(revertHitTimerID);
                //revertHitTimerID = TimerHeap.AddTimer(ActionTime.HIT, 0, DelayCheck);
            }
            else if (act == ActionConstants.PUSH)
            {
                stiff = true;
                //Actor.HitStateChanged = HitStateChange;
                //TimerHeap.DelTimer(revertHitTimerID);
                //revertHitTimerID = TimerHeap.AddTimer(ActionTime.PUSH, 0, DelayCheck);
            }
            else if (act == ActionConstants.HIT_GROUND)
            {
                stiff = true;
                hitGround = true;
                //Actor.HitStateChanged = HitStateChange;
                //TimerHeap.DelTimer(revertHitTimerID);
                //revertHitTimerID = TimerHeap.AddTimer(ActionTime.HIT_GROUND, 0, DelayCheck);
            }
        }

        private void HitStateChange(string name, bool start)
        {
            if ((name.EndsWith("ready") || name.EndsWith("run")) && start)
            {
                Actor.HitStateChanged = null;
                ClearHitAct();
            }
        }

        public void ClearHitAct()
        {
            if (this is EntityMyself)
            {
                //Transform.localRotation = preQuaternion;
                currSpellID = -1; //���ڹ������ܻ���Ϻ���ٴ��ݴ�
            }
            ChangeMotionState(MotionState.IDLE);
            hitAir = false;
            knockDown = false;
            hitGround = false;
            stiff = false;
            EventDispatcher.TriggerEvent(Events.AIEvent.DummyStiffEnd, Transform.gameObject);
            //TimerHeap.AddTimer(500, 0, DelayCheck);//��ʱ�ٴ��ݴ��ж�
        }

        private void DelayCheck()
        {
            if (animator == null)
            {
                return;
            }
            if (CurrentMotionState == MotionState.HIT && animator.GetInteger("Action") == 0)
            {
                ClearHitAct();
            }
            if (stiff && animator.GetInteger("Action") == 0)
            {
                ClearHitAct();
            }
        }

        virtual public void SetSpeed(float speed)
        {
            if (animator == null)
            {
                return;
            }
            animator.SetFloat("Speed", speed);
        }

        virtual public bool IsInTransition()
        {
            return animator.IsInTransition(0);
        }

        virtual public void ChangeMotionState(string newState, params System.Object[] args)
        {
            fsmMotion.ChangeStatus(this, newState, args);
        }

        virtual public void ChangeMotionStateInFrames(string newState, params System.Object[] args)
        {
            fsmMotion.ChangeStatus(this, newState, args);
        }

        // ������볡�����������ʼ���������ݣ� ��Դ�� ģ�͵�
        // �������ݡ�
        virtual public void OnEnterWorld()
        {
            // todo: �����������ݽ���
            buffManager = new BuffManager(this);
            EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.AddEventListener<GameObject, Vector3, float>(MogoMotor.ON_MOVE_TO_FALSE, OnMoveToFalse);
        }

        // ����ӳ�����ɾ���� �������ͷ���Դ
        virtual public void OnLeaveWorld()
        {
            // todo: ������ͷ���Դ
            EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.RemoveEventListener<GameObject, Vector3, float>(MogoMotor.ON_MOVE_TO_FALSE, OnMoveToFalse);
            if (buffManager != null)
            {
                buffManager.Clean();
            }
            RemoveListener();
            ClearBinding();
            if (GameObject)
            {
                MogoWorld.GameObjects.Remove(GameObject.GetInstanceID());
            }
            //if (MogoWorld.Entities.ContainsKey(ID))
            //{
            //    MogoWorld.Entities.Remove(ID);
            //}
            if (Actor)
                Actor.ReleaseController();
            GameObject.Destroy(GameObject);
            //AssetCacheMgr.ReleaseInstance(GameObject, false);
            GameObject = null;
            Transform = null;
            weaponAnimator = null;
            animator = null;
            motor = null;
            sfxHandler = null;
            audioSource = null;
        }

        virtual public void Idle()
        {
            if ((this is EntityMyself) && (this as EntityMyself).deathFlag == 1)
            {
                return;
            }
            if (battleManger == null)
            {
                ChangeMotionState(MotionState.IDLE);
            }
            else
            {
                this.battleManger.Idle();
            }
        }

        virtual public void Roll()
        {
            this.battleManger.Roll();
        }

        #endregion

        #region ��������

        #region �������

        /// <summary>
        /// ȥ�������������������Դ�λ�ƶ�ȥ�������
        /// ��animator�Դ�chactorController�������Դ�һ����������û�ҵ����õ�ȥ��������
        /// </summary>
        public void RemoveGravity()
        {
            motor.gravity = 0;
            //animator.applyRootMotion = false;
        }

        public void SetGravity(float gravity = 20)
        {
            motor.gravity = gravity;
            //animator.applyRootMotion = true;
        }

        /// <summary>
        /// ����
        /// </summary>
        public void SetFreeze()
        {
            if (this is EntityMyself)
            {
                motor.enableStick = false;
            }
            SetSpeedReduce();

        }

        /// <summary>
        /// �ⶳ
        /// </summary>
        public void SetThaw()
        {
            SetSpeedRecover();
            motor.enableStick = true;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="speedRate">������</param>
        public void SetSpeedReduce(float speedRate = 0)
        {
            if (isSrcSpeed)
            {
                isSrcSpeed = false;
                srcSpeed = animator.speed;
                gearMoveSpeedRate = speedRate;
            }
            animator.speed = srcSpeed * speedRate;
            // motor.SetSpeed(motor.speed);
        }

        /// <summary>
        /// �ָ��ٶ�
        /// </summary>
        public void SetSpeedRecover()
        {
            if (!isSrcSpeed)
            {
                gearMoveSpeedRate = 1;
                isSrcSpeed = true;
                animator.speed = srcSpeed;
                // motor.SetSpeed(0);
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        public void CreateDuplication()
        {
            GameObject duplication = (GameObject)UnityEngine.Object.Instantiate(Actor.gameObject, Vector3.zero, Quaternion.identity);

            MonoBehaviour[] scripts = duplication.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script.GetType() != typeof(Transform))
                {
                    UnityEngine.Object.Destroy(script);
                }
            }

            Animator anima = duplication.GetComponent<Animator>();
            if (anima != null)
                UnityEngine.Object.Destroy(anima);

            CharacterController chaController = duplication.GetComponent<CharacterController>();
            if (chaController != null)
                UnityEngine.Object.Destroy(chaController);

            UnityEngine.AI.NavMeshAgent navAgent = duplication.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null)
                UnityEngine.Object.Destroy(navAgent);

            duplication.transform.position = Transform.position;
            duplication.transform.rotation = Transform.rotation;
            TimerHeap.AddTimer<GameObject>(1000, 0, (copy) => { UnityEngine.Object.Destroy(copy); }, duplication);
        }

        #endregion

        public int GetIntAttr(string attrName)
        {
            return intAttrs.GetValueOrDefault(attrName, 0);
        }

        public void SetIntAttr(string attrName, int value)
        {
            intAttrs[attrName] = value;
        }

        public void SetDoubleAttr(string attrName, double value)
        {
            doubleAttrs[attrName] = value;
        }

        public double GetDoubleAttr(string attrName)
        {
            return doubleAttrs.GetValueOrDefault(attrName, 0);
        }

        public string GetStringAttr(string attrName)
        {
            return stringAttrs.GetValueOrDefault(attrName, "");
        }

        public object GetObjectAttr(string attrName)
        {
            return objectAttrs.GetValueOrDefault(attrName, null);
        }

        public void SetObjectAttr(string attrName, object value)
        {
            objectAttrs[attrName] = value;
        }

        public void PlaySfx(int nSpellID)
        {
            if (sfxManager == null)
            {
                return;
            }
            sfxManager.PlaySfx(nSpellID);
        }

        public void RemoveSfx(int nSpellID)
        {
            if (sfxManager == null)
            {
                return;
            }
            sfxManager.RemoveSfx(nSpellID);
        }

        public void PlayFx(int fxID, Transform target = null, Action<GameObject, int> action = null)
        {
            if (sfxHandler)
                sfxHandler.HandleFx(fxID, target, action);
        }

        public void RemoveFx(int fxID)
        {
            if (sfxHandler)
                sfxHandler.RemoveFXs(fxID);
        }

        // ������Զ�̹��̵���
        public void RpcCall(string func, params object[] args)
        {
            ServerProxy.Instance.RpcCall(func, args);
        }

        public void OnPositionChange(float pX, float pY, float pZ)
        {
            RpcCall("OnPositionChange", pX, pY, pZ);
        }

        public void OnRotationChange(float rX, float rY, float rZ)
        {
            RpcCall("OnRotationChange", rX, rY, rZ);
        }

        public virtual void UpdatePosition()
        {
            if (MogoWorld.isLoadingScene)
                return;
            Vector3 point;
            if (Transform)
            {
                if (Mogo.Util.MogoUtils.GetPointInTerrain(position.x, position.z, out point))
                {
                    Transform.position = new Vector3(point.x, point.y + 0.3f, point.z);
                    if (rotation != Vector3.zero)
                        Transform.eulerAngles = new Vector3(0, rotation.y, 0);
                }
                else
                {
                    var myself = this as EntityMyself;
                    if (myself != null)//������ײʧ�ܾ���������������
                    {
                        var map = MapData.dataMap.Get(myself.sceneId);
                        LoggerHelper.Warning("Pull character to born point: " + map.enterX * 0.01 + ", " + map.enterY * 0.01);
                        Vector3 bornPoint;
                        if (Mogo.Util.MogoUtils.GetPointInTerrain((float)(map.enterX * 0.01), (float)(map.enterY * 0.01), out bornPoint))
                        {
                            Transform.position = new Vector3(bornPoint.x, bornPoint.y + 0.5f, bornPoint.z);
                        }
                        else
                        {
                            Transform.position = new Vector3(bornPoint.x, bornPoint.y, bornPoint.z);
                            //if (motor)
                            //    motor.gravity = 10000f;
                        }
                    }
                    else
                    {
                        Transform.position = new Vector3(point.x, point.y + 1, point.z);
                        //if (motor)
                        //    motor.gravity = 10000f;
                    }
                }
            }

        }

        public void GotoPreparePosition()
        {
            if (Transform)
            {
                GameObject.layer = 14;

                Transform.position = Transform.position - new Vector3(0, 10000, 0);
            }
        }

        public void SetPosition()
        {
            if (Transform)
            {
                position = Transform.position;
                rotation = Transform.eulerAngles;
            }
        }

        public void SetPositon(float pX, float pY, float pZ)
        {
            if (Transform != null)
            {
                Transform.position = new Vector3(pX, pY, pZ);
            }

        }

        public void SetRotation(float rX, float rY, float rZ)
        {
            if (Transform != null)
            {
                Transform.eulerAngles = new Vector3(rX, rY, rZ);
            }

        }
        public bool isCreatingModel = false;

        public void Equip(int _equipId)
        {
            if (Transform == null)
            {
                return;
            }
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            if (!ItemEquipmentData.dataMap.ContainsKey(_equipId))
            {
                LoggerHelper.Error("can not find equip:" + _equipId);
                return;
            }
            ItemEquipmentData equip = ItemEquipmentData.dataMap[_equipId];
            if (equip.mode > 0)
            {
                if (Actor == null)
                {
                    return;
                }
                Actor.m_isChangingWeapon = true;
                Actor.Equip(equip.mode);
                if (equip.type == (int)EquipType.Weapon)
                {
                    ControllerOfWeaponData controllerData = ControllerOfWeaponData.dataMap[equip.subtype];
                    RuntimeAnimatorController controller;
                    if (animator == null) return;
                    string controllerName = (MogoWorld.inCity ? controllerData.controllerInCity : controllerData.controller);
                    if (animator.runtimeAnimatorController != null)
                    {
                        if (animator.runtimeAnimatorController.name == controllerName) return;
                        AssetCacheMgr.ReleaseResource(animator.runtimeAnimatorController);
                    }

                    AssetCacheMgr.GetResource(controllerName,
                    (obj) =>
                    {
                        controller = obj as RuntimeAnimatorController;
                        if (animator == null) return;
                        animator.runtimeAnimatorController = controller;
                        if (this is EntityMyself)
                        {
                            (this as EntityMyself).UpdateSkillToManager();
                            EventDispatcher.TriggerEvent<int, int>(InventoryEvent.OnChangeEquip, equip.type, equip.subtype);
                        }
                        if (this is EntityPlayer)
                        {
                            if (MogoWorld.inCity)
                            {
                                animator.SetInteger("Action", -1);
                            }
                            else
                            {
                                animator.SetInteger("Action", 0);
                            }
                            if (MogoWorld.isReConnect)
                            {
                                ulong s = stateFlag;
                                stateFlag = 0;
                                stateFlag = s;
                            }
                        }
                    });
                }

                stopWatch.Stop();

                //if (!isCreatingModel)
                //{
                //    SetPosition();
                //    stopWatch.Start();
                //    //AssetCacheMgr.ReleaseInstance(GameObject);
                //    CreateActualModel();
                //    stopWatch.Stop();
                //    Mogo.Util.LoggerHelper.Debug("CreateModel:" + stopWatch.Elapsed.Milliseconds);

                //}
            }

        }

        public void RemoveEquip(int _equipId)
        {
            //if (!ItemEquipmentData.dataMap.ContainsKey(_equipId))
            //{
            //    return;
            //}
            //ItemEquipmentData equip = ItemEquipmentData.dataMap[_equipId];
            //if (equip.mode > 0)
            //{
            //    Transform.GetComponent<ActorParent>().RemoveEquid(equip.mode);

            //    if (equip.type == (int)EquipType.Weapon)
            //    {
            //        ControllerOfWeaponData controllerData = ControllerOfWeaponData.dataMap[0];
            //        RuntimeAnimatorController controller;
            //        AssetCacheMgr.GetResource(controllerData.controller,
            //        (obj) =>
            //        {
            //            controller = obj as RuntimeAnimatorController;
            //            animator.runtimeAnimatorController = controller;
            //        });

            //    }

            //    //if (!isCreatingModel)
            //    //{
            //    //    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            //    //    stopWatch.Start();
            //    //    SetPosition();
            //    //    AssetCacheMgr.ReleaseInstance(GameObject);
            //    //    CreateActualModel();
            //    //    stopWatch.Stop();
            //    //    Mogo.Util.LoggerHelper.Debug("CreateModel:" + stopWatch.Elapsed.Milliseconds);
            //    //    MogoMainCamera.Instance.target = Mogo.Util.MogoUtils.GetChild(GameObject.transform, "slot_camera");
            //    //}
            //}
        }

        /// <summary>
        /// ����������������ʵ������ֵ��
        /// </summary>
        /// <param name="args"></param>
        public void SetEntityInfo(BaseAttachedInfo info)
        {
            ID = info.id;
            dbid = info.dbid;
            entity = info.entity;
            SynEntityAttrs(info);
        }

        /// <summary>
        /// ����������������ʵ������ֵ��
        /// </summary>
        /// <param name="args"></param>
        public void SetEntityCellInfo(CellAttachedInfo info)
        {
            position = info.position;
            //rotation = info.rotation;
            SynEntityAttrs(info);
            hadSyncProp = true;
            //��Ӧ��������ͬ���������������꣬�������ҵ�����
            //if (Transform)
            //    UpdatePosition();
        }

        public void SynEntityAttrs(AttachedInfo info)
        {
            if (info.props == null)
                return;
            var type = this.GetType();
            foreach (var prop in info.props)
            {
                //Mogo.Util.LoggerHelper.Debug("SynEntityAttrs:------ " + prop.Key + " " + prop.Value);
                SetAttr(prop.Key, prop.Value, type);
            }
            //if (!MogoWorld.isLoadingScene)
            //{
            //    UpdateView();
            //}
            //else
            //{
            //    hasCache = true;
            //}
        }

        /// <summary>
        /// ����UI��������ʵ��
        /// </summary>
        public virtual void UpdateView()
        {
        }


        #endregion

        #region �ܱ�������

        /// <summary>
        /// ��������ֵ
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        protected void SetAttr(EntityDefProperties propInfo, object value, Type type)
        {
            var prop = type.GetProperty(propInfo.Name);
            try
            {
                if (prop != null)
                {
                    // Mogo.Util.LoggerHelper.Debug("prop: " + prop.Name + " value: " + value);
                    prop.SetValue(this, value, null);
                }
                else
                {
                    var typeCode = Type.GetTypeCode(propInfo.VType.VValueType);
                    if (m_intSet.Contains(typeCode))
                        intAttrs[propInfo.Name] = Convert.ToInt32(value);
                    else if (m_doubleSet.Contains(typeCode))
                        doubleAttrs[propInfo.Name] = Convert.ToDouble(value);
                    else if (propInfo.VType.VValueType == typeof(string))
                        stringAttrs[propInfo.Name] = value as string;
                    else
                        objectAttrs[propInfo.Name] = value;
                    //LoggerHelper.Info("Static property not found: " + propInfo.Name);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("SetAttr error: " + propInfo.VType.VValueType + ":" + propInfo.Name + " " + value.GetType() + ":" + value + "\n" + ex);
                LoggerHelper.Error("prop: " + prop + " this: " + this.GetType());
            }
        }

        /// <summary>
        /// ����Define�ļ�������ص�������
        /// </summary>
        protected void AddListener()
        {
            var ety = Mogo.RPC.DefParser.Instance.GetEntityByName(entityType);
            if (ety == null)
            {
                LoggerHelper.Warning("Entity not found: " + entityType);
                return;
            }
            foreach (var item in ety.ClientMethodsByName)
            {
                var methodName = item.Key;
                var method = this.GetType().GetMethod(methodName, ~System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    var e = new KeyValuePair<string, Action<object[]>>(String.Concat(Mogo.Util.Utils.RPC_HEAD, methodName), (args) =>
                    {//RPC�ص��¼�����
                        try
                        {
                            //LoggerHelper.Debug("RPC_resp: " + methodName);
                            method.Invoke(this, args);
                        }
                        catch (Exception ex)
                        {
                            var sb = new System.Text.StringBuilder();
                            sb.Append("method paras are: ");
                            foreach (var methodPara in method.GetParameters())
                            {
                                sb.Append(methodPara.ParameterType + " ");
                            }
                            sb.Append(", rpc resp paras are: ");
                            foreach (var realPara in args)
                            {
                                sb.Append(realPara.GetType() + " ");
                            }

                            Exception inner = ex;
                            while (inner.InnerException != null)
                            {
                                inner = inner.InnerException;
                            }
                            LoggerHelper.Error(String.Format("RPC resp error: method name: {0}, message: {1} {2} {3}", methodName, sb.ToString(), inner.Message, inner.StackTrace));
                        }
                    });
                    EventDispatcher.AddEventListener<object[]>(e.Key, e.Value);
                    m_respMethods.Add(e);
                }
                else
                    LoggerHelper.Warning("Method not found: " + item.Key);
            }
        }

        /// <summary>
        /// �Ƴ�����Define�ļ�������ص�������
        /// </summary>
        protected void RemoveListener()
        {
            //LoggerHelper.Warning("EventDispatcher.TheRouter.Count: " + EventDispatcher.TheRouter.Count);

            //LoggerHelper.Warning("m_respMethods: " + m_respMethods.Count);
            foreach (var e in m_respMethods)
            {
                EventDispatcher.RemoveEventListener<object[]>(e.Key, e.Value);
            }
            m_respMethods.Clear();
            //LoggerHelper.Warning("EventDispatcher.TheRouter.Count: " + EventDispatcher.TheRouter.Count);
        }

        #endregion

        #region ��װ�¼���������

        // �� entity id ������ eventType �У�����Ψһ�� eventType, 
        // �������в�ͬʵ������Ϣ
        public string GenUniqMessage(string eventType)
        {
            return String.Concat(eventType, this.ID);
        }

        virtual public void AddUniqEventListener(string eventType, Action handler)
        {
            EventDispatcher.AddEventListener(GenUniqMessage(eventType), handler);
        }

        virtual public void AddUniqEventListener<T>(string eventType, Action<T> handler)
        {
            EventDispatcher.AddEventListener<T>(GenUniqMessage(eventType), handler);
        }

        virtual public void AddUniqEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            EventDispatcher.AddEventListener<T, U>(GenUniqMessage(eventType), handler);
        }

        virtual public void AddUniqEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            EventDispatcher.AddEventListener<T, U, V>(GenUniqMessage(eventType), handler);
        }

        virtual public void RemoveUniqEventListener(string eventType, Action handler)
        {
            EventDispatcher.RemoveEventListener(GenUniqMessage(eventType), handler);
        }

        virtual public void RemoveUniqEventListener<T>(string eventType, Action<T> handler)
        {
            EventDispatcher.RemoveEventListener<T>(GenUniqMessage(eventType), handler);
        }

        virtual public void RemoveUniqEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            EventDispatcher.RemoveEventListener<T, U>(GenUniqMessage(eventType), handler);
        }

        virtual public void RemoveUniqEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            EventDispatcher.RemoveEventListener<T, U, V>(GenUniqMessage(eventType), handler);
        }

        virtual public void TriggerUniqEvent(string eventType)
        {
            EventDispatcher.TriggerEvent(GenUniqMessage(eventType));
        }

        virtual public void TriggerUniqEvent<T>(string eventType, T arg1)
        {
            EventDispatcher.TriggerEvent<T>(GenUniqMessage(eventType), arg1);
        }

        virtual public void TriggerUniqEvent<T, U>(string eventType, T arg1, U arg2)
        {
            EventDispatcher.TriggerEvent<T, U>(GenUniqMessage(eventType), arg1, arg2);
        }

        virtual public void TriggerUniqEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            EventDispatcher.TriggerEvent<T, U, V>(GenUniqMessage(eventType), arg1, arg2, arg3);
        }

        #endregion ��װ�¼���������

        #region ��װ����֡�Ļص�����

        // ����֡�Ļص������� ���ڴ����������֡ ��ɵ����顣
        public void AddCallbackInFrames(Action callback, int inFrames = 3)
        {
            if (Actor)
                Actor.AddCallbackInFrames(callback, inFrames);
        }

        public void AddCallbackInFrames<U>(Action<U> callback, U arg1, int inFrames = 3)
        {
            if (Actor)
                Actor.AddCallbackInFrames<U>(callback, arg1, inFrames);
        }

        public void AddCallbackInFrames<U, V>(Action<U, V> callback, U arg1, V arg2, int inFrames = 3)
        {
            if (Actor)
                Actor.AddCallbackInFrames<U, V>(callback, arg1, arg2, inFrames);
        }

        public void AddCallbackInFrames<U, V, T>(Action<U, V, T> callback, U arg1, V arg2, T arg3, int inFrames = 3)
        {
            if (Actor)
            {
                if (inFrames == 0)
                    inFrames = 3;
                Actor.AddCallbackInFrames<U, V, T>(callback, arg1, arg2, arg3, inFrames);
            }
        }

        public void AddCallbackInFrames<U, V, T, W>(Action<U, V, T, W> callback, U arg1, V arg2, T arg3, W arg4, int inFrames = 3)
        {
            if (Actor)
            {
                if (inFrames == 0)
                    inFrames = 3;
                Actor.AddCallbackInFrames<U, V, T, W>(callback, arg1, arg2, arg3, arg4, inFrames);
            }
        }

        #endregion

        /// <summary>
        /// buff�Ŀͻ��˱��ֺ�buff����
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="isAdd"></param>
        /// <param name="time"></param>
        public void HandleBuff(ushort buffId, byte isAdd, uint time)
        {
            if (buffManager == null)
            {
                LoggerHelper.Debug("buffManager == null,obejct:" + Transform.gameObject.name + ",id:" + ID);
                return;
            }
            buffManager.HandleBuff(buffId, isAdd, time);
        }

        public void ClientAddBuff(int id)
        {
            if (buffManager == null)
            {
                return;
            }
            buffManager.ClientAddBuff(id);
        }

        public void ClientDelBuff(int id)
        {
            if (buffManager == null)
            {
                return;
            }
            buffManager.ClientDelBuff(id);
        }

        /// <summary>
        /// Buff(��½ʱ������ͬ��)
        /// </summary>
        public Dictionary<int, UInt32> m_skillBuffClient;
        public LuaTable skillBuffClient
        {
            set
            {
                Mogo.Util.Utils.ParseLuaTable(value, out m_skillBuffClient);
                if (buffManager != null && buffManager.HasGotLoginBuff == false)
                {
                    buffManager.HasGotLoginBuff = true;
                    foreach (KeyValuePair<int, UInt32> pair in m_skillBuffClient)
                    {
                        buffManager.HandleBuff((ushort)pair.Key, 1, pair.Value);
                    }
                }
            }
        }
    }
}