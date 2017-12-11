#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MecanimEvent
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.11
// 模块描述：动作事件触发器。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections;
using UnityEngine;

public class MecanimEvent
{
    private Animator m_animator;
    private AnimationClip m_currentMark;
    private bool m_isNotFadeing = true;

    private float passTime = 0; //当前帧累积时间

    /// <summary>
    /// 构造函数。
    /// </summary>
    /// <param name="animator">Mecanim动作系统对象。</param>
    public MecanimEvent(Animator animator)
    {
        m_animator = animator;
    }

    /// <summary>
    /// 监测动作状态变迁。
    /// </summary>
    /// <param name="StateChanged">动画状态变化事件。
    /// String: 动画名称。
    /// Boolean: 动画变化状态。true: 动画开始；false: 动画结束。
    /// </param>
    /// <returns>迭代器。</returns>
    public IEnumerator CheckAnimationChange(Action<String, Boolean> StateChanged)
    {
        while (true)
        {
            var state = m_animator.GetCurrentAnimatorClipInfo(0);//获取当前播放动作状态
            if (state.Length != 0)
                if (state[0].weight != 1)//当前动作正在融合
                {
                    if (m_isNotFadeing)//判断已标记为融合，如果不是融合，则表示为刚开始融合，此刻为下一个动作的开始点
                    {
                        var nextState = m_animator.GetNextAnimatorClipInfo(0);
                        if (StateChanged != null && nextState.Length != 0)
                            StateChanged(nextState[0].clip.name, true);//LoggerHelper.Debug(nextState[0].clip.name + " start...");
                        m_currentMark = state[0].clip;//更新当前动作
                        m_isNotFadeing = false;//标记为融合状态
                        yield return new WaitForFixedUpdate();
                    }
                }
                else
                {
                    if (m_currentMark != state[0].clip)//在动作变迁时，state会瞬间变为新动作，weight变为1，所以需判断state是否已改变，此刻为旧动作结束点
                    {
                        if (m_currentMark != null && StateChanged != null)
                            StateChanged(m_currentMark.name, false);//LoggerHelper.Debug(currentMark.name + " end...");
                        m_currentMark = state[0].clip;//更新当前动作
                        m_isNotFadeing = true;//标记为非融合状态
                        yield return new WaitForFixedUpdate();
                    }
                }
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 监测动作状态变迁。
    /// </summary>
    /// <param name="StateChanged">动画状态变化事件。
    /// String: 动画名称。
    /// Boolean: 动画变化状态。true: 动画开始；false: 动画结束。
    /// </param>
    /// <returns>迭代器。</returns>
    public IEnumerator CheckAnimationChange(Action<int, bool> StateChanged)
    {
        while (true)
        {
            var state = m_animator.GetCurrentAnimatorStateInfo(0);//获取当前播放动作状态
            passTime = passTime + Time.deltaTime;
            if (passTime >= state.length)
            {
                StateChanged(state.nameHash, state.loop);
                passTime = 0;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

