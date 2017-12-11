using UnityEngine;
using System.Collections;

public class DailyTaskIcon : MonoBehaviour 
{
	GameObject	m_NoticeIcon=null;
	bool 		m_bTaskFinished=false;
	void Awake()
	{
		m_NoticeIcon=transform.GetChild(3).gameObject;
	}

	void ShowDailyTaskNotification()
	{
		m_NoticeIcon.SetActive(true);
	}

	void HideDailyTaskNotification()
	{
		m_NoticeIcon.SetActive(false);
	}

	void OnEnable()
	{
		if(m_bTaskFinished)
		{
			m_NoticeIcon.SetActive(true);
		}
		else
		{
			m_NoticeIcon.SetActive(false);
		}
	}
	
	public void 	ShowDailyTaskFinishedNotice()
	{
		m_bTaskFinished=true;
	}
	
	public void 	HideDailyTaskFinishedNotice()
	{
		m_bTaskFinished=false;
	}
}
