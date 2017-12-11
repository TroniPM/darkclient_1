/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DecoratorNodes
// 创建者：Hooke Hu
// 修改者列表：
// 创建日期：
// 模块描述：继承自DecoratorNode各种装饰节点
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogo.Game;

namespace Mogo.AI
{
    public class Not : DecoratorNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            return !base.Proc(theOwner);
        }
    }

    public class Success : DecoratorNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            return base.Proc(theOwner);
        }
    }
}
