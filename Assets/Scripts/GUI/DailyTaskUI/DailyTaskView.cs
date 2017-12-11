using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class DailyTaskView : MonoBehaviour,IUIView
{
	public 	bool 			m_bIsOnFocus=false;
	public void UpdateView<T>(List<T> datas)
	{
        int nChildCount = transform.childCount;
		for (int i = 0; i < nChildCount; i++)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}
		int nCount=datas.Count;
		for(int i=0;i<nCount;i++)
		{
			{
				GameObject o = AssetCacheMgr.SynGetInstance("Task.prefab") as GameObject;
				o.transform.parent =transform;
				o.transform.localPosition = new Vector3(0, 0, 0);
				o.transform.localScale = new Vector3(1, 1, 1);
				DailyTask dailyTask = o.gameObject.AddComponent<DailyTask>();
				dailyTask.Init();
				dailyTask.m_strOrder = i.ToString();
				
				dailyTask.UpdateData((datas[i] as DailyTaskInfo).strName,
				                     (datas[i] as DailyTaskInfo).nTaskID,
				                     (datas[i] as DailyTaskInfo).nExpAmount,
				                     (datas[i] as DailyTaskInfo).nGoldAmount,
				                     (datas[i] as DailyTaskInfo).nCurrentStep,
				                     (datas[i] as DailyTaskInfo).nTotalStep,
				                     (datas[i] as DailyTaskInfo).strTaskSprite,
				                     (datas[i] as DailyTaskInfo).nTaskStatus);
				GameObject.FindGameObjectWithTag("DailyTaskLists").GetComponent<UIGrid>().repositionNow = true;
			}
		}
	}

	public void CleanUp()
	{
        int nChildCount = transform.childCount;
		for (int i = 0; i < nChildCount; i++)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}
	}

	public bool isOnFocus()
	{
		return m_bIsOnFocus;
	}
}
