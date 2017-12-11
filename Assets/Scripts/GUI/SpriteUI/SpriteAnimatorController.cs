#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class SpriteAnimatorController : MonoBehaviour 
{
    private Animator m_animator;

    private readonly int ActionDefault = 0;
    public enum ActionStatus
    {
        ActionHand = 1,
        ActionNormal = 3
    }

     private bool m_bLoadResourceInsteadOfAwake = false;
     public void LoadResourceInsteadOfAwake()
     {
         if (m_bLoadResourceInsteadOfAwake)
             return;

         m_bLoadResourceInsteadOfAwake = true;

         m_animator = GetComponent<Animator>();
     }

     private uint timerID = 0;
     public void ActionChange(ActionStatus status)
     {
         if (m_animator)
         {
             m_animator.SetInteger("Action", (int)status);

             switch (status)
             {
                 case ActionStatus.ActionNormal:
                     {                         
                         timerID = FrameTimerHeap.AddTimer(100, 0, () => { ActionChange((ActionStatus)ActionDefault); });
                     }
                     break;
                 case ActionStatus.ActionHand:
                     {
                         timerID = FrameTimerHeap.AddTimer(100, 0, () => { ActionChange((ActionStatus)ActionDefault); });
                     }
                     break;
             }             
         }
     }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ActionChange(ActionStatus.ActionHand);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ActionChange(ActionStatus.ActionNormal);
        }
    }
}
