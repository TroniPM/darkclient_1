/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityPlayer
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013.1.31
// 模块描述：玩家对象
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.FSM;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    public class EntityPlayer : EntityParent
    {
        private uint idleTimer = 0;

        public override int hitStateID
        {
            get
            {
                return (int)vocation;
            }
        }

        public override uint curHp
        {
            get
            {
                return base.curHp;
            }
            set
            {
                base.curHp = value;
                EventDispatcher.TriggerEvent(Events.CampaignEvent.SetPlayerMessage, this as EntityParent);

                if (BillboardViewManager.Instance)
                    BillboardViewManager.Instance.SetBillboardBlood(base.PercentageHp, ID);
            }
        }

        public override ulong stateFlag
        {
            get
            {
                return base.stateFlag;
            }
            set
            {
                base.stateFlag = value;
                if (animator == null)
                {
                    return;
                }
                byte f = (byte)(m_stateFlag & 1);
                if (f != deathFlag)
                {
                    if (f == 0)
                    {
                        Revive();
                    }
                    deathFlag = f;
                }
            }
        }

        private byte m_deathFlag;
        public byte deathFlag
        {
            get
            {
                return m_deathFlag;
            }
            set
            {
                m_deathFlag = value;
                if (m_deathFlag > 0)
                {
                    this.OnDeath(-1);
                    GuideSystem.Instance.TriggerEvent(GlobalEvents.Death);
                }
            }
        }

        public byte factionFlag
        {
            get 
            {
                return m_factionFlag;
            }

            set
            {
                m_factionFlag = value;

                if (MogoWorld.thePlayer != null)
                    return;

                if (!Transform)
                    return;

                if (!GameObject)
                    return;

                SetBillBoard();
            }
        }

        private uint m_loadedWingId;
        public uint loadedWingId
        {
            get
            {
                return m_loadedWingId;
            }
            set
            {
                m_loadedWingId = value;
                UpdateDressWing();
            }
        }

        /// <summary>
        /// 宝石特效ID
        /// </summary>
        private uint m_loadedJewelId;

        public uint loadedJewelId
        {
            get { return m_loadedJewelId; }
            set
            {
                m_loadedJewelId = value;
                if (GameObject != null)
                {
                    AddJewelFX(m_loadedJewelId);
                }
            }
        }

        /// <summary>
        /// 装备特效ID
        /// </summary>
        private uint m_loadedEquipId;

        public uint loadedEquipId
        {
            get { return m_loadedEquipId; }
            set
            {
                m_loadedEquipId = value;
                if (GameObject != null)
                {
                    AddEquipFX(m_loadedEquipId);
                }
            }
        }

        /// <summary>
        /// 强化特效ID
        /// </summary>
        private uint m_loadedStrengthenId;

        public uint loadedStrengthenId
        {
            get { return m_loadedStrengthenId; }
            set
            {
                m_loadedStrengthenId = value;
                if (GameObject != null)
                {
                    AddStrenthFX(m_loadedStrengthenId);
                }
            }
        }

        public void Revive()
        {
            ClearSkill();
            currentMotionState = Mogo.FSM.MotionState.IDLE;
            SetAction(ActionConstants.REVIVE);
            SetSpeed(0);
            motor.SetSpeed(0);
            TimerHeap.AddTimer(500, 0, Stand);
        }

        private void Stand()
        {
            if (!GameObject)
                return;
            if (MogoWorld.inCity)
            {
                TimerHeap.AddTimer(500, 0, SetAction, -1);
            }
            motor.enableStick = true;
        }

        protected Dictionary<string, List<int>> spells = new Dictionary<string, List<int>>();

        public EntityPlayer()
        {
            //spellManager = new SpellManager(this); 不能在这里初始化，要在EnterWorld
            //battleManger = new BattleManager(this);
            //sfxManager = new SfxManager(this);
        }

        override public void CreateModel()
        {
            //CreateDeafaultModel();
            CreateActualModel();
        }

        public override void CreateActualModel()
        {
           
            isCreatingModel = true;
            AssetCacheMgr.ReleaseLocalInstance(GameObject);
            AvatarModelData data = AvatarModelData.dataMap.Get((int)vocation);
            SubAssetCacheMgr.GetPlayerInstance(data.prefabName,
                (prefab, guid, go) =>
                {
                    var gameObject = go as GameObject;
                    var actor = gameObject.AddComponent<ActorPlayer>();

                    actor.theEntity = this;
                    motor = gameObject.AddComponent<MogoMotorServer>();
                    animator = gameObject.GetComponent<Animator>();

                    sfxHandler = gameObject.AddComponent<SfxHandler>();

                    //actor.mogoMotor = motor;

                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.rolloffMode = AudioRolloffMode.Custom;

                    GameObject = gameObject;
                    Transform = gameObject.transform;
                    GameObject.layer = 20;
                    //Debug.LogError("entityPlayer.CreateActualModel:" + gameObject.layer);
                    // Debug.LogError(GameObject.name + " " + ID + " Player CreateActualModel: " + Transform.position);

                    this.Actor = actor;
                    //Transform.localScale = scale;
                    Transform.tag = "OtherPlayer";
                   
                    UpdatePosition();
                    foreach (var item in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    {
                        item.enabled = false;
                    }

                    if (data.scale > 0)
                    {
                        Transform.localScale = new Vector3(data.scale, data.scale, data.scale);
                    }

                    base.CreateModel();

                    LoadEquip();

                    foreach (var item in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    {
                        item.enabled = true;
                    }
                    //Mogo.Util.LoggerHelper.Debug("AddComponent<CombineSkinnedMeshes>");

                    //gameObject.AddComponent<CombineSkinnedMeshes>();
                    //Mogo.Util.LoggerHelper.Debug("AddComponent<CombineSkinnedMeshes>");
                    gameObject.SetActive(false);
                    idleTimer = TimerHeap.AddTimer(1000, 0, DelayCheckIdle);
                    TimerHeap.AddTimer(3000, 0, () =>
                    {
                        if (gameObject == null) return;
                        gameObject.SetActive(true);
                        isCreatingModel = false;

                        SetBillBoard();

                        MogoFXManager.Instance.AddShadow(gameObject, ID);

                        //gameObject.AddComponent<MogoObjOpt>().ObjType = MogoObjType.Player;

                        //if (gameObject.GetComponent<Animator>() != null)
                        //{
                        //    gameObject.GetComponent<Animator>().enabled = false;
                        //}

                        //if (gameObject.GetComponent<Animation>() != null)
                        //{
                        //    gameObject.GetComponent<Animation>().enabled = false;
                        //}
                        
                    });
                    Actor.ActChangeHandle = ActionChange;

                    UpdateDressWing();
                    AddEquipFX(loadedEquipId);
                    AddJewelFX(loadedJewelId);
                    AddStrenthFX(loadedStrengthenId);
                }

            );
        }

        protected void SetBillBoard()
        {
            if (MogoWorld.thePlayer != null)
            {
                switch (BillboardLogicManager.Instance.GlobalBillBoardCurrentType)
                {
                    case GlobalBillBoardType.OccupyTower:
                        if (m_factionFlag == MogoWorld.thePlayer.factionFlag)
                        {
                            // 队友
                            BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, true, View.HeadBloodColor.Blue);
                            BillboardViewManager.Instance.SetHead(this, BillboardViewManager.HeadStatus.PVP, BillboardViewManager.PVPCamp.CampOwn);
                        }
                        else
                        {
                            // 对手
                            BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, true, View.HeadBloodColor.Red);
                            BillboardViewManager.Instance.SetHead(this, BillboardViewManager.HeadStatus.PVP, BillboardViewManager.PVPCamp.CampEnemy);
                        }
                        break;

                    case GlobalBillBoardType.Normal:
                        BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, false);
                        BillboardViewManager.Instance.SetHead(this, BillboardViewManager.HeadStatus.Normal);
                        break;
                }
            }
            else
            {
                BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, false);
                BillboardViewManager.Instance.SetHead(this, BillboardViewManager.HeadStatus.Normal);
            }
        }

        private int checkCnt = 0;
        private int correctCnt = 0;
        private void DelayCheckIdle()
        {
            if (!MogoWorld.inCity)
            {
                return;
            }
            if (checkCnt > 11)
            {
                TimerHeap.DelTimer(idleTimer);
                return;
            }
            if (animator == null)
            {
                idleTimer = TimerHeap.AddTimer(1000, 0, DelayCheckIdle);
                checkCnt++;
                return;
            }
            int act = animator.GetInteger("Action");
            if (act == 0)
            {
                SetAction(-1);
                idleTimer = TimerHeap.AddTimer(1000, 0, DelayCheckIdle);
                checkCnt++;
                return;
            }
            if (act == -1)
            {
                correctCnt++;
                if (correctCnt > 2)
                {
                    TimerHeap.DelTimer(idleTimer);
                    return;
                }
            }
            idleTimer = TimerHeap.AddTimer(1000, 0, DelayCheckIdle);
            checkCnt++;
        }

        private void LoadEquip()
        {
            if (m_loadedCuirass != 0)
            {
                Equip((int)m_loadedCuirass);
            }

            if (m_loadedArmguard != 0)
            {
                Equip((int)m_loadedArmguard);
            }

            if (m_loadedLeg != 0)
            {
                Equip((int)m_loadedLeg);
            }

            if (loadedWeapon != 0)
            {
                Mogo.Util.LoggerHelper.Debug("id:" + dbid);
                Mogo.Util.LoggerHelper.Debug("load weapon:" + loadedWeapon + ",vocation:" + (int)vocation);
                Equip((int)loadedWeapon);
            }
        }

        public override void CreateDeafaultModel()
        {
            AvatarModelData data = AvatarModelData.dataMap[999];
            GameObject gameObject = AssetCacheMgr.GetLocalInstance(data.prefabName) as GameObject;

            ActorPlayer actor = gameObject.AddComponent<ActorPlayer>();
            motor = gameObject.AddComponent<MogoMotorServer>();
            animator = gameObject.GetComponent<Animator>();

            sfxHandler = gameObject.AddComponent<SfxHandler>();

            // GearEffectListener作用已经交由ActorMyself处理，不需要增加这个脚本了
            // gameObject.AddComponent<GearEffectListener>();

            //actor.mogoMotor = motor;
            actor.theEntity = this;
            GameObject = gameObject;
            Transform = gameObject.transform;

            Debug.LogError(GameObject.name + " " + ID + " Player CreateDeafaultModel: " + Transform.position);

            this.Actor = actor;
            Transform.localScale = scale;
            Transform.tag = "OtherPlayer";
            UpdatePosition();
            base.CreateModel();

        }

        //private void SceneLoaded(int sceneId, bool isInstance)
        //{
        //    CreateModel();
        //}

        // 对象进入场景，在这里初始化各种数据， 资源， 模型等
        // 传入数据。
        override public void OnEnterWorld()
        {
            // todo: 这里会加入数据解析

            base.OnEnterWorld();
            if (ID != MogoWorld.thePlayer.ID)
            {
                skillManager = new SkillManager(this);
                battleManger = new OtherPlayerBattleManager(this, skillManager);
            }
            sfxManager = new SfxManager(this);
        }


        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            // todo: 这里会释放资源
            correctCnt = 0;
            checkCnt = 0;
            TimerHeap.DelTimer(idleTimer);
            if (Actor)
                Actor.ActChangeHandle = null;
            base.OnLeaveWorld();
            if (ID != MogoWorld.thePlayer.ID)
            {
                skillManager.Clean();
                battleManger.Clean();
            }
            BillboardLogicManager.Instance.RemoveBillboard(ID);
        }

        override public void MainCameraCompleted()
        {
            base.MainCameraCompleted();
            SetBillBoard();
            BillboardLogicManager.Instance.SetHead(this);
            //EventDispatcher.TriggerEvent<float, uint>
            //      (BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, PercentageHp, ID);
            //EventDispatcher.TriggerEvent<string, uint>
            //     (BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDNAME, name, ID);
        }


        public void OnPowerCharge()
        {
            ChangeMotionState(MotionState.CHARGING);
        }

        public void OnPowerChargeInterrupt()
        {
            ChangeMotionState(MotionState.IDLE);
        }

        public virtual void FixedErr()
        {//修正错误状态，用于回城容
            if (!MogoWorld.inCity)
            {
                return;
            }
            if (animator == null)
            {
                return;
            }
            int act = animator.GetInteger("Action");
            if (act == 0)
            {
                SetAction(-1);
            }
        }

        public void UpdateDressWing()
        {
            int dressed = (int)loadedWingId;
            if (Actor == null)
            {
                return;
            }
            if (dressed <= 0)
            {
                if (this is EntityMyself)
                {
                    (Actor as ActorMyself).RemoveWing();
                }
                else
                {
                    (Actor as ActorPlayer).RemoveWing();
                }
                return;
            }
            WingData d = WingData.dataMap.Get(dressed);
            if (d == null)
            {
                return;
            }
            string[] ms = d.modes.Split(',');
            string path = ms[(int)vocation - 1];
            if (this is EntityMyself)
            {
                (Actor as ActorMyself).AddWing(path, () => { });
            }
            else
            {
                (Actor as ActorPlayer).AddWing(path, () => { });
            }
        }

        uint m_oldJewelFXID = 0;
        uint m_oldEquipFXID = 0;
        uint m_oldStrenthFXID = 0;

        Material oldEquipMat;
        Material oldWeaponMat;

        private void AddEquipFX(uint fxId)
        {
           

            Debug.LogError("@@@@@@@@AddEquipFX " + fxId);

            if (vocation != Vocation.Warrior)
                return;

            if (fxId == m_oldEquipFXID)
                return;
           
            int fxResId = EquipSpecialEffectData.dataMap[(int)fxId].fxid;
            
            m_oldEquipFXID = loadedEquipId;

            if (fxId != 0)
            {
                AssetCacheMgr.GetResource("equip_warrior_01_fx_5_mat.mat", (obj) =>
                {
                    Debug.LogError(fxId + " @@@@@@@@@@@@@@@");
                    Material fxMat = (Material)obj;
                    GameObject.transform.Find("101_00").GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].material = null;
                    GameObject.transform.Find("101_00").GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].material = fxMat;
                    UVAnim anim = GameObject.transform.Find("101_00").gameObject.AddComponent<UVAnim>();
                    anim.Direction.x = 0.5f;
                    anim.Direction.y = 0.13f;
                    anim.speed = 1.5f;

                });
            }
            else
            {
                //AssetCacheMgr.GetResource(Actor.EquipOnDic, (obj) =>
                //{
                //    Debug.LogError(fxId + " @@@@@@@@@@@@@@@");
                //    Material fxMat = (Material)obj;
                //    GameObject.transform.FindChild("101_00").GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].material = null;
                //    GameObject.transform.FindChild("101_00").GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].material = fxMat;
                //    UVAnim anim = GameObject.transform.FindChild("101_00").gameObject.AddComponent<UVAnim>();
                //    anim.Direction.x = 0.5f;
                //    anim.Direction.y = 0.13f;
                //    anim.speed = 1.5f;

                //});
            }
        }

        private void AddJewelFX(uint fxId)
        {
            Debug.LogError("@@@@@@@@AddJewelFX " + fxId);

            if (vocation != Vocation.Warrior)
                return;

            if (fxId == m_oldJewelFXID)
                return;

            int fxResId = EquipSpecialEffectData.dataMap[(int)fxId].fxid;

            m_oldJewelFXID = loadedJewelId;

            if (fxId != 0)
            {
                AssetCacheMgr.GetResource("blade_01_fx_6_mat.mat", (obj) =>
                {
                    Material fxMat = (Material)obj;

                    System.Collections.Generic.List<GameObject> listGo = GameObject.GetComponent<ActorParent>().WeaponObj;
                    for (int i = 0; i < listGo.Count; ++i)
                    {
                        int index = i;
                        listGo[index].GetComponentsInChildren<MeshRenderer>(true)[0].material = null;
                        listGo[index].GetComponentsInChildren<MeshRenderer>(true)[0].material = fxMat;
                        UVAnim anim = listGo[index].AddComponent<UVAnim>();
                        anim.Direction.x = 0.5f;
                        anim.Direction.y = 0.13f;
                        anim.speed = 1.5f;
                    }


                });
            }
            else
            {
                
            }

        }

        private void AddStrenthFX(uint fxId)
        {
            Debug.LogError("@@@@@@@@AddStrenthFX " + fxId);
            if (fxId == m_oldStrenthFXID)
                return;

            int fxResId = EquipSpecialEffectData.dataMap[(int)fxId].fxid;

            m_oldStrenthFXID = loadedStrengthenId;

            System.Collections.Generic.List<GameObject> listGo = GameObject.GetComponent<ActorParent>().WeaponObj;

            if (fxId != 0)
            {
                AssetCacheMgr.GetInstance("H_blade_01_fx_6.prefab", (name, id, obj) =>
                {
                    for (int i = 0; i < listGo.Count; ++i)
                    {
                        int index = i;
                        MFUIUtils.AttachWidget(((GameObject)obj).transform, listGo[index].transform);
                    }
                });
            }
            else
            {
 
            }
        }
       
        public void UpdateEquipFX(uint fxId)
        {
            Debug.LogError("@@@@@@@@UpdateEquipFX " + fxId); 
//            foreach (var item in Actor.EquipOnDic)
//            {
//                Debug.LogError(item.Value + " " + EquipData.dataMap[item.Value].ma
//);
//            }

            if (vocation != Vocation.Warrior)
                return;

            switch (EquipSpecialEffectData.dataMap[(int)fxId].group)
            {
                case 1:
                    AddJewelFX(fxId);
                    break;

                case 2:
                    AddEquipFX(fxId);
                    break;

                case 3:
                    AddStrenthFX(fxId);
                    break;
            }
        }

        public void RefreshEquipFX()
        {
            AddJewelFX(loadedJewelId);
            AddEquipFX(loadedEquipId);
            AddStrenthFX(loadedStrengthenId);
        }
    }
}