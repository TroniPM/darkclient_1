using System;
using System.Collections.Generic;
using Mogo.Game;

namespace Mogo.GameData
{
    public class HitState : GameData<HitState>
	{
        public List<int> idle { get; protected set; }
        public List<int> hit { get; protected set; }
        public List<int> push { get; protected set; }
        public List<int> knockdown { get; protected set; }
        public List<int> hitair { get; protected set; }
        public List<int> knockout { get; protected set; }
        public List<int> getup { get; protected set; }
        public List<int> revive { get; protected set; }
        public List<int> skill { get; protected set; }

        static public readonly string fileName = "xml/HitState";
        //static public Dictionary<int, HitState> dataMap { get; set; }

        private int GetIdx(int target)
        {
            if (target == ActionConstants.DIE)
            {
                return 0;
            }
            else if (target == ActionConstants.HIT)
            {
                return 1;
            }
            else if (target == ActionConstants.PUSH)
            {
                return 2;
            }
            else if (target == ActionConstants.KNOCK_DOWN)
            {
                return 3;
            }
            else if (target == ActionConstants.HIT_AIR)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        public int GetIdleAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return idle[i];
        }

        public int GetHitAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return hit[i];
        }

        public int GetPushAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return push[i];
        }

        public int GetKnockDownAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return knockdown[i];
        }

        public int GetKnockOutAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return knockout[i];
        }

        public int GetHitAirAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return hitair[i];
        }

        public int GetGetUpAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return getup[i];
        }

        public int GetReviveAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return revive[i];
        }

        public int GetSkillAct(int targetAct)
        {
            int i = GetIdx(targetAct);
            return skill[i];
        }
	}
}
