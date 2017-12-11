/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ActionNodes
// 创建者：Hooke Hu
// 修改者列表：
// 创建日期：
// 模块描述：继承自ActionNode各种行为节点
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using UnityEngine;

using Mogo.Game;
using Mogo.Util;
using Mogo.FSM;

namespace Mogo.AI
{
    public class AOI : ActionNode
    {
        protected int m_iSearchChance = 0;

        public AOI(int _searchChance)
        {
            m_iSearchChance = _searchChance;
        }
        public override bool Proc(EntityParent theOwner)
        {
            //查找目标，记录到blackBoard
            //Mogo.Util.LoggerHelper.Debug("AI:" + "AOI");
            bool rnt = theOwner.ProcAOI(m_iSearchChance);
            return rnt;
        }
    }

    //------------------------------------------------------------
    public class ChooseCastPoint : ActionNode
    {
        protected int m_iSkillId = 0;

        public ChooseCastPoint(int _skillId)
        {
            m_iSkillId = _skillId;
        }

        public override bool Proc(EntityParent theOwner)
        {
            
            bool rnt = theOwner.ProcChooseCastPoint(m_iSkillId);
            if (rnt == true)
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "ChooseCastPoint:true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "ChooseCastPoint:false");
            }
            return rnt;
        }
    }
    //------------------------------------------------------------
    public class MoveTo : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "MoveTo");
            theOwner.ProcMoveTo();
            return true;
        }
    }
    //------------------------------------------------------------
    public class Rest : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "Rest");
            //theOwner.ProcRest();
            return false;
        }
    }
    //------------------------------------------------------------
    public class EnterThink : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "EnterThink");
            theOwner.blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
            return true;
        }
    }
    //------------------------------------------------------------
    public class Think : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "Think");
            theOwner.ProcThink();
            return true;
        }
    }
    //------------------------------------------------------------
    public class EnterRest : ActionNode
    {
        protected uint m_iSec = 0;

        public EnterRest(uint _sec)
        {
            m_iSec = _sec;
        }
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "EnterRest:" + m_iSec);
            theOwner.ProcEnterRest(m_iSec);
            return true;
        }
    }
    //------------------------------------------------------------
    public class EnterCD : ActionNode
    {
        protected int m_iSec = 0;

        public EnterCD(int _sec)
        {
            m_iSec = _sec;
        }
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "EnterCD:" + m_iSec);
            theOwner.ProcEnterCD(m_iSec);//kevintestcd
            return true;
        }
    }
    //------------------------------------------------------------
    public class CastSpell : ActionNode
    {
        protected int m_iSkillId = 0;
        protected int m_iReversal = 0;

        public CastSpell(int _skillId, int _reversal)
        {
            m_iSkillId = _skillId;
            m_iReversal = _reversal;
        }
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "CastSpell:" + m_iSkillId);
            return theOwner.ProcCastSpell(m_iSkillId, m_iReversal);          
        }
    }
    //------------------------------------------------------------
    public class ReinitLastCast : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "ReinitLastCast");
            theOwner.ProcReinitLastCast();
            return true;
        }
    }
    //------------------------------------------------------------
    public class Escape : ActionNode
    {
        protected uint m_iSec = 0;

        public Escape(uint _sec)
        {
            m_iSec = _sec;
        }
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "Escape:" + m_iSec);
            theOwner.ProcEscape(m_iSec);
            return true;
        }
    }
    //------------------------------------------------------------
    public class FollowOwner : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "FollowOwner");
            theOwner.ProcFollowOwner();
            return true;
        }
    }
    //------------------------------------------------------------500,100,1000,61,1500,62,2000,63,2500,64,3000,65,38,1
    public class LookOn : ActionNode
    {
        protected int m_Mode5Skill = 0;
        protected float m_DistanceMax = 0.0f;
        protected float m_DistanceMin = 0.0f;
        protected int[] m_ModePercent = new int[6];
        protected int[] m_ModeInterval = new int[5];

        public LookOn(int _DistanceMax, int _DistanceMin, 
            int _Mode0Interval, int _Mode0Percent, 
            int _Mode1Interval, int _Mode1Percent,
            int _Mode2Interval, int _Mode2Percent,
            int _Mode3Interval, int _Mode3Percent,
            int _Mode4Interval, int _Mode4Percent,
            int _Mode5Percent, int _Mode5Skill
            )
        {
            m_Mode5Skill = _Mode5Skill;
            m_DistanceMax = _DistanceMax;
            m_DistanceMin = _DistanceMin;

            m_ModePercent[0] = _Mode0Percent;
            m_ModePercent[1] = _Mode1Percent;
            m_ModePercent[2] = _Mode2Percent;
            m_ModePercent[3] = _Mode3Percent;
            m_ModePercent[4] = _Mode4Percent;
            m_ModePercent[5] = _Mode5Percent;

            m_ModeInterval[0] = _Mode0Interval;
            m_ModeInterval[1] = _Mode1Interval;
            m_ModeInterval[2] = _Mode2Interval;
            m_ModeInterval[3] = _Mode3Interval;
            m_ModeInterval[4] = _Mode4Interval;

            m_Mode5Skill = _Mode5Skill;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "LookOn");

            theOwner.blackBoard.LookOn_DistanceMax = m_DistanceMax*0.01f;
            theOwner.blackBoard.LookOn_DistanceMin = m_DistanceMin*0.01f;

            for (int i = 0; i < m_ModePercent.Length; i++)
            {
                theOwner.blackBoard.LookOn_ModePercent[i] = m_ModePercent[i];
            }

            for (int i = 0; i < m_ModeInterval.Length; i++)
            {
                theOwner.blackBoard.LookOn_ModeInterval[i] = m_ModeInterval[i]*0.001f;
            }

            theOwner.blackBoard.LookOn_Mode5Skill = m_Mode5Skill;

            theOwner.ProcLookOn();

            return true;
        }
    }
    //------------------------------------------------------------
    public class MercenaryAOI : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "MercenaryAOI");
            return theOwner.ProcMercenaryAOI();
        }
    }
    //------------------------------------------------------------
    public class PVPAOI : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "PVPAOI");
            return theOwner.ProcPVPAOI();
        }
    }
    //------------------------------------------------------------
    public class SelectAutoFightMovePoint : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "SelectAutoFightMovePoint");
            return theOwner.ProcSelectAutoFightMovePoint();
        }
    }
    //------------------------------------------------------------
    public class Patrol : ActionNode
    {
        protected int m_iCDTimeMin = 0;
        protected int m_iCDTimeMax = 0;

        public Patrol(int _CDTimeMin, int _CDTimeMax)
        {
            m_iCDTimeMin = _CDTimeMin;
            m_iCDTimeMax = _CDTimeMax;
        }
        public override bool Proc(EntityParent theOwner)
        {
            bool rnt = theOwner.ProcPatrol(m_iCDTimeMin, m_iCDTimeMax);
            if (rnt)
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "PatrolAction" + "true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "PatrolAction" + "false");
            }
            return rnt;
        }
    }
    //------------------------------------------------------------
    public class PowerFX : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //Mogo.Util.LoggerHelper.Debug("AI:" + "PowerFX");
            return theOwner.PowerFX();
        }
    }
}
  