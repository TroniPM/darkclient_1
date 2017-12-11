/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MonsterBattleManager
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：服务器端控制的怪物， 客户端战斗系统
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.FSM;
using Mogo.GameData;

namespace Mogo.Game
{
    public class MonsterBattleManager : BattleManager
    {

        private Dictionary<uint, int> hatedList = new Dictionary<uint, int>();
        private EntityPlayer theTarget = MogoWorld.thePlayer as EntityPlayer;

        public MonsterBattleManager(EntityParent _theOwner, SkillManager _skillManager)
            : base(_theOwner, _skillManager)
        {
        }

        public override void Clean()
        {
            base.Clean();
        }

        override public void OnHit(int _actionID, uint _attackerID, uint woundId, List<int> harm)
        {
            if (MogoWorld.inCity ||
                theOwner.ID != woundId ||
                !theOwner.canBeHit
                )
            {
                return;
            }

            if (theOwner.GetType() == typeof(EntityMonster) && (theOwner as EntityMonster).IsGolem)
            {
                if (theOwner.MonsterData.id == SpecialMonsterId.TowerDefCrystalId)
                {//女神水晶ID 
                    EventDispatcher.TriggerEvent(Events.CampaignEvent.CrystalAttacked);
                }
                return;
            }

            int hitType = harm[0];
            int hitNum = harm[1];
            if (hitType != 1 && !theOwner.immuneShift)
            {
                base.OnHit(_actionID, _attackerID, woundId, harm);
            }
            if (MogoWorld.showHitShader && (theOwner as EntityMonster).HitShader != null)
            {
                int _h = Utils.Choice<int>((theOwner as EntityMonster).HitShader);
                if (_h == 1)
                {//等于1闪白
                    MogoFXManager.Instance.GetHit(theOwner.GameObject, 0.2f, MogoFXManager.HitColorType.HCT_WHITE);
                }
                else if (_h == 2)
                {//等于2闪红
                    MogoFXManager.Instance.GetHit(theOwner.GameObject, 0.2f, MogoFXManager.HitColorType.HCT_RED);
                }
            }
            if (MogoWorld.showFloatBlood && GameData.SkillAction.dataMap[_actionID].damageFlag == 1)
            {
                FloatBlood(hitType, hitNum);
            }
            theOwner.ProcOnHit();
        }

        override public void OnDead(int actionID)
        {
            if (theOwner.GetType() == typeof(EntityMonster) && (theOwner as EntityMonster).IsGolem)
            {
                return;
            }

            LoggerHelper.Debug("MonsterBattleManager OnDead");
            base.OnDead(actionID);
        }

        override public void OnAttacking(int nSkillID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {
            skillManager.OnAttacking(nSkillID, ltwm, rotation, forward, position);
        }
    }
}