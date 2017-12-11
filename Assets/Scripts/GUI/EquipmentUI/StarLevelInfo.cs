#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：2013-9-12
// 模块描述：星星/钻石/皇冠显示控制
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class StarLevelInfo : MogoUIBehaviour 
{
	public enum StarType
	{
		StarType1,	// 钻石
        StarType2,	// 灰冠
        StarType3,	// 皇冠
	}

	private const int LIST_GAP = 30;
    private Transform m_tranPosBegin;
    private List<Transform> transformList;	
	
    void Awake()
    {
		m_myTransform = transform;
        FillFullNameData(m_myTransform);
		
		m_tranPosBegin = FindTransform("StarLevelPosBegin");
        SetupStarList(5);
	}

    private void SetupStarList(int maxLevel)
    {  			
		transformList = new List<Transform>();
        for (int i = 0; i < maxLevel; i++)
        {
            GameObject go = new GameObject();
            go.layer = 10;
			go.transform.localScale = new Vector3(35, 35, 1);
			go.name = "StarLevel" + i;
			UISprite spStar = go.AddComponent<UISprite>();

            Utils.MountToSomeObjWithoutPosChange(go.transform, m_myTransform);
            go.transform.localPosition = new Vector3(m_tranPosBegin.localPosition.x + i * LIST_GAP, 
				m_tranPosBegin.localPosition.y, 
				m_tranPosBegin.localPosition.z);
           	transformList.Add(go.transform);
        }
    }
	
	private void SetStarImage(StarType starType, UISprite spStar)
	{
		string starSpriteName = "fb_xing_yidengdao";
		switch(starType)
		{
            case StarType.StarType1:
		    {
                starSpriteName = "zs";
			
			    spStar.atlas = MogoUIManager.Instance.GetAtlasByIconName(starSpriteName);
			    spStar.spriteName = starSpriteName;
                spStar.MakePixelPerfect();
		    }break;

            case StarType.StarType2:
		    {
                starSpriteName = "hg_h";
			
			    spStar.atlas = MogoUIManager.Instance.GetAtlasByIconName(starSpriteName);
			    spStar.spriteName = starSpriteName;	
			    spStar.MakePixelPerfect();
		    }break;

            case StarType.StarType3:
		    {
			    starSpriteName = "hg";
			
			    spStar.atlas = MogoUIManager.Instance.GetAtlasByIconName(starSpriteName);
			    spStar.spriteName = starSpriteName;	
			    spStar.MakePixelPerfect();
		    }break;
		}		
	}

    public void SetLevel(StarType type, int level)
    {
        for (int i = 0; i < transformList.Count; i++)
        {
            UISprite spStar = transformList[i].GetComponent<UISprite>();
            if (spStar != null)
                SetStarImage(type, spStar);

            if (i < level)
                transformList[i].gameObject.SetActive(true);
            else
                transformList[i].gameObject.SetActive(false);
        }
    }
}
