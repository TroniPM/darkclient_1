using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.FSM;
using Mogo.GameData;

namespace Mogo.Game
{
    public class OtherPlayerBattleManager : BattleManager
    {
        public OtherPlayerBattleManager(EntityParent _owner, SkillManager _skillManager)
            : base(_owner, _skillManager)
        {
            skillManager = _skillManager;
        }

        public override void OnHit(int _actionID, uint _attackerID, uint woundId, List<int> harm)
        {
            if (MogoWorld.inCity 
                || theOwner.ID != woundId 
                || !theOwner.canBeHit 
                || theOwner.ID == _attackerID 
                || Utils.BitTest(theOwner.stateFlag, StateCfg.DEATH_STATE) == 1)
            {
                return;
            }

            int hitType = harm[0];
            int hitNum = harm[1];
            if (MogoWorld.showFloatBlood && SkillAction.dataMap[_actionID].damageFlag == 1)
            {
                if (_attackerID == MogoWorld.thePlayer.ID)
                {
                    Debug.LogError("FloatBlood(_attackerID == MogoWorld.thePlayer.ID) : " + hitType + " " + hitNum);
                    FloatBlood(hitType, hitNum);
                }
            }
            if (!theOwner.breakAble)
            {
                return;
            }
            if (theOwner.curHp > 0/* && theOwner.curHp > harm[1]*/)
            {
                base.OnHit(_actionID, _attackerID, woundId, harm);
            }
        }
    }
}
