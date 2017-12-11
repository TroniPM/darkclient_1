/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：States
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

namespace Mogo.FSM
{
    // 状态
    static public class MotionState
    {
        static readonly public string IDLE = "idle";
        static readonly public string WALKING = "walking";
        static readonly public string DEAD = "dead";
        static readonly public string CHARGING = "charging";
        static readonly public string ATTACKING = "attacking";
        static readonly public string HIT = "hit";
        static readonly public string PREPARING = "preparing";
        static readonly public string ROLL = "roll";

        static readonly public string LOCKING = "locking";
        static readonly public string PICKING = "picking";
    }
}