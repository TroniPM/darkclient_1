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

public struct OccupyTowerPassUIPlayerData
{
    public string playerName;
    public int playerScore;
    public int playerAddition;
    public int camp;
}

public class OccupyTowerPassUILogicManager : UILogicManager
{
    private static OccupyTowerPassUILogicManager m_instance;
    public static OccupyTowerPassUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new OccupyTowerPassUILogicManager();
            }

            return OccupyTowerPassUILogicManager.m_instance;
        }
    }

    #region 事件

    public void Initialize()
    {
        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
    }

    public override void Release()
    {
        base.Release();
    }

    #endregion
}
