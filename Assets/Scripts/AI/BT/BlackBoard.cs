using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Game;

namespace Mogo.AI
{
    public class AIState
    {
        public static string THINK_STATE = "THINK_STATE";
        public static string REST_STATE = "REST_STATE";
        public static string PATROL_STATE = "PATROL_STATE";
        public static string ESCAPE_STATE = "ESCAPE_STATE";
        public static string FIGHT_STATE = "FIGHT_STATE";
        public static string CD_STATE = "CD_STATE";
        public static string PATROL_CD_STATE = "PATROL_CD_STATE";
    }

    public enum AIEvent
    {
        MoveEnd =1 ,     
        Born,        
        AvatarDie,
        CDEnd,       
        RestEnd,        
        BeHit,       
        AvatarPosSync,
        Self,
        StiffEnd,
    }

    /// <summary>
    /// 行为树状态黑板，用于记录行为树的一些状态
    /// </summary>
	public class BTBlackBoard
	{
        public string aiState = AIState.THINK_STATE;
        public AIEvent aiEvent = AIEvent.MoveEnd;
      
        public uint enemyId = 0;
        public Container enemyTrap = null;
        public uint timeoutId = 0;
        
        public Vector3 movePoint = new Vector3();
        public int lastCastIndex = -1;
        public uint skillActTime = 0;
        public System.UInt64 skillActTick = 0;
        public uint navTargetDistance = 0;

        public int[] skillUseCount = new int[15];
        public int skillReversal = 0;//技能反向释放
        public float speedFactor = 1.0f;
        public Vector3 lastCastCoord = new Vector3();//上次使用技能时候的坐标
        //lookon
        public int turnAngleDir = -1;
        public int LookOn_LastMode = -1;
        public int LookOn_Mode5Skill = 0;
        public float LookOn_DistanceMax = 0.0f;
        public float LookOn_DistanceMin = 0.0f;
        public int[] LookOn_ModePercent = new int[6];
        public float[] LookOn_ModeInterval = new float[5];
        public System.UInt64 LookOn_Tick = 0;
        public uint LookOn_ActTime = 0;
        //see
        public Dictionary<uint, int> mHatred = new Dictionary<uint, int>();
        public uint patrolActTime = 0;
        //public System.UInt64 patrolActTick = 0;

        public int LookOn_Mode
        {
            get { return turnAngleDir; }
            set { turnAngleDir = value;
                if (value >= 0)
                    LookOn_LastMode = value;
            }
        }

        public BTBlackBoard()
        {
            for (int i = 0; i < 15; i++)
            {
                skillUseCount[i] = 0;
            }
        }

        public void ChangeState(string newState)
        {
            aiState = newState;
        }

        public void ChangeEvent(AIEvent newEvent)
        {
            aiEvent = newEvent;
        }

        public void EditHatred(uint key, int value)
        {
            if (mHatred.ContainsKey(key))
            {
                mHatred[key] = value;
            }
            else
            {
                mHatred.Add(key, value);
            }
        }

        public bool HasHatred(uint key)
        {
            return mHatred.ContainsKey(key);
        }

        public int HatredCount()
        {
            return mHatred.Count;
        }
	}
}
