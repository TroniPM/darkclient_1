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

public class StrenthenStarLevelInfo : MonoBehaviour 
{
    private StarLevelInfo m_starLevelInfo;
    private int m_iStarLevel = 0;

    public void CreateStarLevelInfo(Transform parent, Vector3 posBegin)
    {
        AssetCacheMgr.GetUIInstance("StarLevelInfo.prefab", (prefab, guid, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.transform.parent = parent;
            obj.transform.localPosition = posBegin;
            obj.transform.localScale = new Vector3(1, 1, 1);
            m_starLevelInfo = obj.AddComponent<StarLevelInfo>();

            SetStarLevel(m_iStarLevel);
        });
    }

    public void SetStarLevel(int level)
    {
        m_iStarLevel = level;

        if (m_starLevelInfo != null)
        {
            if (level <= 5)
            {
                m_starLevelInfo.SetLevel(StarLevelInfo.StarType.StarType1, level);
            }
            else if (level <= 10)
            {
                m_starLevelInfo.SetLevel(StarLevelInfo.StarType.StarType2, level - 5);
            }
            else if (level > 10)
            {
                m_starLevelInfo.SetLevel(StarLevelInfo.StarType.StarType3, level - 10);
            }
	    }
    }
}
