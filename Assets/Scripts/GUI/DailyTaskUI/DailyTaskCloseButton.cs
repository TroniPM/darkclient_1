using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DailyTaskCloseButton : MonoBehaviour {
	
	void Start()
	{
		GetComponent<UIButtonMessage>().target=gameObject;
	}
	
	public void OnCloseDailyTask()
	{
		EventDispatcher.TriggerEvent("OnDailyTaskCloseButtonClicked");
	}
}
