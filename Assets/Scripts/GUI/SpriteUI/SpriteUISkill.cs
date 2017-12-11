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

public class SpriteUISkill : MogoUIBehaviour
{
    public Camera SpriteSkillCamera;
    public BoxCollider cc;

    #region 事件

    void OnPress(bool isOver)
    {
        if (SpriteSkillCamera == null || cc == null)
            return;

        if (isOver)
        {

        }
        else
        {
            RaycastHit hit = new RaycastHit();
            if (cc.Raycast(SpriteSkillCamera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
            {
                if (transform.name == SpriteUIViewManager.SpriteSkill1Name
                    || transform.name == SpriteUIViewManager.SpriteSkill2Name
                    || transform.name == SpriteUIViewManager.SpriteSkill3Name
                    || transform.name == SpriteUIViewManager.SpriteSkill4Name
                    || transform.name == SpriteUIViewManager.SpriteSkill5Name)
                {
                    EventDispatcher.TriggerEvent<int>(SpriteUIDict.SpriteUIEvent.OnSpriteSkillUp, Index);
                }
            }
        }
    }
   
    #endregion

    private int m_index;
    public int Index
    {
        get { return m_index; }
        set
        {
            m_index = value;
        }
    }
}
