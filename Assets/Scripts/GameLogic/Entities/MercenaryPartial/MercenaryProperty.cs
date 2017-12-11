using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Task;
using Mogo.Mission;
using Mogo.Util;
using Mogo.GameData;
using Mogo.AI;
using Mogo.AI.BT;

namespace Mogo.Game
{
    public partial class EntityMercenary
    {
        #region 私有属性


 

        private uint m_airDamage;
        private uint m_airDefense;
        private uint m_allElementsDamage;
        private uint m_allElementsDefense;
        private uint m_antiCrit;
        private uint m_atk;
        private uint m_antiDefense;
        private uint m_antiTrueStrike;
        private uint m_crit;
        private uint m_critExtraAttack;
        private uint m_def;
        private uint m_earthDamage;
        private uint m_fireDamage;
        private uint m_earthDefense;
        private uint m_fireDefense;
        private uint m_hit;
        private uint m_pvpAddition;
        private uint m_pvpAnti;
        private uint m_speedAddRate;
        private uint m_trueStrike;
        private uint m_waterDamage;
        private uint m_waterDefense;

        private ushort m_sceneId;
        private ushort m_imap_id;
        private byte m_deathFlag;

        private int m_clientTrapId;


        private bool m_borned = false;

        private bool m_bModelBuilded = false;
        

        private UInt32 m_monsterId;

        private uint m_updateCoordTimerID = uint.MaxValue;

        protected BehaviorTreeRoot m_aiRoot;

        private bool isLittleGuy;

        private uint fadeTimer = uint.MaxValue;
        private float fadeInTime = 0.3f;
        private float fadeOutTime = 0.3f;
        private uint beginFadeOutTimeForward = 500;
        // 为去除警告暂时屏蔽以下代码
        //#region DEBUG
        //private int m_debugCount = 0;
        //private int m_debugCount2 = 0;
        //#endregion

        #endregion

        #region 公有属性

 
        public uint airDamage
        {
            get { return m_airDamage; }
            set
            {
                m_airDamage = value;
            }
        }

        public uint airDefense
        {
            get { return m_airDefense; }
            set
            {
                m_airDefense = value;
            }
        }

        public uint allElementsDamage
        {
            get { return m_allElementsDamage; }
            set
            {
                m_allElementsDamage = value;
            }
        }

        public uint allElementsDefense
        {
            get { return m_allElementsDefense; }
            set
            {
                m_allElementsDefense = value;
            }
        }

        public uint antiCrit
        {
            get { return m_antiCrit; }
            set
            {
                m_antiCrit = value;
            }
        }

        public uint atk
        {
            get { return m_atk; }
            set
            {
                m_atk = value;
            }
        }

        public uint antiDefense
        {
            get { return m_antiDefense; }
            set
            {
                m_antiDefense = value;
            }
        }

        public uint antiTrueStrike
        {
            get { return m_antiTrueStrike; }
            set
            {
                m_antiTrueStrike = value;
            }
        }

        public uint crit
        {
            get { return m_crit; }
            set
            {
                m_crit = value;
            }
        }

        public uint critExtraAttack
        {
            get { return m_critExtraAttack; }
            set
            {
                m_critExtraAttack = value;
            }
        }

        public uint def
        {
            get { return m_def; }
            set
            {
                m_def = value;
            }
        }

        public uint earthDamage
        {
            get { return m_earthDamage; }
            set
            {
                m_earthDamage = value;
            }
        }

        public uint fireDamage
        {
            get { return m_fireDamage; }
            set
            {
                m_fireDamage = value;
            }
        }

        public uint earthDefense
        {
            get { return m_earthDefense; }
            set
            {
                m_earthDefense = value;
            }
        }


        public uint fireDefense
        {
            get { return m_fireDefense; }
            set
            {
                m_fireDefense = value;
            }
        }


        public uint hit
        {
            get { return m_hit; }
            set
            {
                m_hit = value;
            }
        }

        public uint pvpAddition
        {
            get { return m_pvpAddition; }
            set
            {
                m_pvpAddition = value;
            }
        }

        public uint pvpAnti
        {
            get { return m_pvpAnti; }
            set
            {
                m_pvpAnti = value;
            }
        }

        public uint speedAddRate
        {
            get { return m_speedAddRate; }
            set
            {
                m_speedAddRate = value;
            }
        }

        public uint trueStrike
        {
            get { return m_trueStrike; }
            set
            {
                m_trueStrike = value;
            }
        }

        public uint waterDamage
        {
            get { return m_waterDamage; }
            set
            {
                m_waterDamage = value;
            }
        }

        public uint waterDefense
        {
            get { return m_waterDefense; }
            set
            {
                m_waterDefense = value;
            }
        }


        public byte deathFlag
        {
            get { return m_deathFlag; }
            set
            {
                if (m_deathFlag != value && value == 1)
                {//死亡处理
                    m_borned = false;//怪物死了没关ai
                    if (blackBoard.timeoutId > 0)
                        TimerHeap.DelTimer(blackBoard.timeoutId);
                    if (m_clientTrapId != 0)
                    {
                        EventDispatcher.TriggerEvent(Events.GearEvent.SetGearEnable, (uint)m_clientTrapId);
                    }

                    OnDeath(-1);

                    EventDispatcher.TriggerEvent(Events.AIEvent.SomeOneDie, factionFlag, ID);
                    if (m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 2)
                        MainUIViewManager.Instance.ShowBossTarget(false);
                }
                else if (m_deathFlag != value && value == 0)
                {
                    m_borned = true;//怪物复活
                    Revive();
                }
                m_deathFlag = value;
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

                if (MogoWorld.thePlayer.factionFlag == m_factionFlag)
                {//是雇佣兵 
                    MogoWorld.theLittleGuyID = ID;
                }
            }
        }


        public int clientTrapId
        {
            get { return m_clientTrapId; }
            set { m_clientTrapId = value; }
        }

        override public ulong stateFlag
        {
            get
            {
                return m_stateFlag;
            }
            set
            {
                base.stateFlag = value;
                if (animator == null)
                {
                    return;
                }
                byte f = (byte)Utils.BitTest(m_stateFlag, StateCfg.DEATH_STATE);
                if (f != deathFlag)
                {
                    deathFlag = f;
                }
                
            }
        }

        public UInt32 monsterId
        {
            get { return m_monsterId; }
            set
            {
                m_monsterId = value;
            }
        }

        public UInt32 ownerEid
        {
            get { return m_ownerEid; }
            set
            {
                m_ownerEid = value;
                //Mogo.Util.LoggerHelper.Debug("kevin ownerEid:" + m_ownerEid);
            }
        }

        public List<int> HitShader
        {
            get
            {
                return m_monsterData.hitShader;
            }
        }

        public int ShowHitAct
        {
            get
            {
                return m_monsterData.showHitAct;
            }
        }

        public override string HeadIcon
        {
            get
            {
                if (m_monsterId > 0)
                {
                    if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                        return IconData.dataMap.Get(m_monsterData.hpShow[0]).path;
                    else
                        return base.HeadIcon;
                }
                else
                    return base.HeadIcon;
            }
        }

        public override string name
        {
            get
            {
                if (m_monsterId > 0)
                {
                    if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                        return LanguageData.GetContent(m_monsterData.hpShow[1]);
                    else
                        return base.name;
                }
                else
                    return base.name;
            }
            set
            {
                base.name = value;

                if (MogoWorld.theLittleGuyID == ID)
                    InstanceUILogicManager.Instance.mercenaryName = value;
            }
        }


        public override byte level
        {
            get
            {
                if (m_monsterId > 0)
                {
                    if (m_monsterData != null)
                        return (byte)(MogoWorld.thePlayer.ApplyMissionID == Mogo.Game.RandomFB.RAIDID ? MogoWorld.thePlayer.level : m_monsterData.level);
                    else
                        return base.level;
                }
                else
                    return base.level;
            }
            set
            {
                base.level = value;
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

                if (isLittleGuy)
                {
                    EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnFlushMercenaryBlood, (int)(PercentageHp * 100));
                }
                else
                {
                    if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnFlushBossBlood, this as EntityParent, (int)base.curHp);
                }

                //EventDispatcher.TriggerEvent<float, uint>(BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, base.PercentageHp, ID);
                BillboardViewManager.Instance.SetBillboardBlood(base.PercentageHp, ID);
            }
        }



        #endregion
    }
}
