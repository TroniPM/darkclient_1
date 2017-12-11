/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：BehaviorTree
// 创建者：Hooke Hu
// 修改者列表：
// 创建日期：
// 模块描述：行为树框架,具体实现要继承自DecoratorNode, ConditionNode, ActionNode, ImpulseNode
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogo.Game;

namespace Mogo.AI
{

    public class BehaviorTreeRoot : IBehaviorTreeNode
    {
        public IBehaviorTreeNode root = null;

        public BehaviorTreeRoot()
        {
        }

        public bool Proc(EntityParent theOwner)
        {
            return root.Proc(theOwner);
        }

        public void AddChild(IBehaviorTreeNode root)
        {
            this.root = root;
        }
    }

    /// <summary>
    /// 行为树节点基类
    /// </summary>
	public interface IBehaviorTreeNode
	{
        bool Proc(EntityParent theOwner);
	}

    /// <summary>
    /// 复合节点，不能为叶子节点
    /// </summary>
    public class CompositeNode : IBehaviorTreeNode
    {
        protected List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();

        /// <summary>
        /// 由子数实现
        /// </summary>
        /// <returns></returns>
        public virtual bool Proc(EntityParent theOwner)
        {
            return true;
        }

        public void AddChild(IBehaviorTreeNode _node)
        {
            children.Add(_node);
        }

        public void DelChild(IBehaviorTreeNode _node)
        {
            children.Remove(_node);
        }

        public void ClearChildren()
        {
            children.Clear();
        }
    }

    /// <summary>
    /// 装饰类
    /// </summary>
    public class DecoratorNode : IBehaviorTreeNode
    {
        protected IBehaviorTreeNode child = null;

        public virtual bool Proc(EntityParent theOwner)
        {
            return child.Proc(theOwner) ;
        }

        public void Proxy(IBehaviorTreeNode _child)
        {
            child = _child;
        }
    }

    /// <summary>
    /// 脉冲类
    /// </summary>
    public class ImpulseNode : IBehaviorTreeNode
    {
        protected IBehaviorTreeNode child = null;

        /// <summary>
        /// 由子类实现
        /// </summary>
        /// <returns></returns>
        public virtual bool Proc(EntityParent theOwner)
        {
            return true;
        }

        public void Proxy(IBehaviorTreeNode _child)
        {
            child = _child;
        }
    }

    /// <summary>
    /// 条件判断类,叶子节点
    /// </summary>
    public class ConditionNode : IBehaviorTreeNode
    {
        /// <summary>
        /// 由子数实现
        /// </summary>
        /// <returns></returns>
        public virtual bool Proc(EntityParent theOwner)
        {
            return false;
        }
    }

    /// <summary>
    /// 具体的行为实现类,叶子节点
    /// </summary>
    public class ActionNode : IBehaviorTreeNode
    {
        /// <summary>
        /// 由子类实现
        /// </summary>
        /// <returns></returns>
        public virtual bool Proc(EntityParent theOwner)
        {
            return false;
        }
    }

    /// <summary>
    /// 遇到一个child执行后返回true,停止迭代
    /// 本node向自己的的父节点也返回true
    /// 如果所有child返回false,本node向自己父节点返回false
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            foreach (IBehaviorTreeNode _node in children)
            {
                if (_node.Proc(theOwner))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 遇到一个child执行后返回false,停止迭代
    /// 本node向自己父节点返回flase
    /// 如果所有child返回true,本node向自己父节点返回true
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            foreach (IBehaviorTreeNode _node in children)
            {
                if (!_node.Proc(theOwner))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
