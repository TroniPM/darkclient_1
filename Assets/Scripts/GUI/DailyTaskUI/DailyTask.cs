using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using Mogo.Util;

public class DailyTaskInfo
{
    public string strName;
    public int nTaskID;
    public int nExpAmount;
    public int nGoldAmount;
    public int nCurrentStep;
    public int nTotalStep;
    public string strTaskSprite;
    public int nTaskStatus;
    public DailyTaskInfo(string strName, int nTaskID, int nExpAmount, int nGoldAmount, int nCurrentStep, int nTotalStep,
                         string strTaskSprite, int nTaskStatus)
    {
        this.strName = strName;
        this.nTaskID = nTaskID;
        this.nExpAmount = nExpAmount;
        this.nGoldAmount = nGoldAmount;
        this.nCurrentStep = nCurrentStep;
        this.nTotalStep = nTotalStep;
        this.strTaskSprite = strTaskSprite;
        this.nTaskStatus = nTaskStatus;
    }
}

public class DailyTask : MonoBehaviour 
{
	public 	GameObject	m_TaskName;
	public 	GameObject	m_ExpAmount;
	public 	GameObject	m_GoldAmount;
	public 	GameObject	m_CurrentStep;
	public	GameObject	m_TaskIcon;
	public	GameObject	m_GetAwardButton;
	public	GameObject 	m_AlreadyGetAwardButton;
	public 	GameObject	m_UnfinishedButton;
	public 	GameObject 	m_ProgressBar;
	public 	GameObject 	m_FinishedText;
	public	int 		m_nTaskID=-1;
	public 	string 		m_strOrder="0";
	public  GameObject  m_FinishedFX=null;
	public	GameObject	m_GetAwardFX=null;
	public	DailyTaskFXRoot 	m_scDailyTaskFXRoot=null;
	void 	Awake()
	{
		Init();
	}
	
	public 	void 	Init()
	{
		string TaskDir="MogoMainUI/Camera/Anchor/MogoMainUIPanel/DailyTaskUI(Clone)/DailyTaskContainer/DailyTaskLists/";
		if(m_TaskName==null)				m_TaskName=GameObject.Find(TaskDir+"Task(Clone)/taskname");
		if(m_ExpAmount==null)				m_ExpAmount=GameObject.Find(TaskDir+"Task(Clone)/expamount");
		if(m_GoldAmount==null)				m_GoldAmount=GameObject.Find(TaskDir+"Task(Clone)/goldamount");
		if(m_CurrentStep==null)				m_CurrentStep=GameObject.Find(TaskDir+"Task(Clone)/progress");
		if(m_TaskIcon==null)				m_TaskIcon=GameObject.Find(TaskDir+"Task(Clone)/taskico");
		if(m_GetAwardButton==null)			m_GetAwardButton=GameObject.Find(TaskDir+"Task(Clone)/awardbtn/Button");
		if(m_AlreadyGetAwardButton==null)	m_AlreadyGetAwardButton=GameObject.Find(TaskDir+"Task(Clone)/awardbtn/alreadygetaward");
		if(m_UnfinishedButton==null)		m_UnfinishedButton=GameObject.Find(TaskDir+"Task(Clone)/awardbtn/unfinished");
		if(m_FinishedText==null)			m_FinishedText=GameObject.Find(TaskDir+"Task(Clone)/awardbtn/alreadyfinished");
		if(m_ProgressBar==null)				m_ProgressBar=GameObject.Find(TaskDir+"Task(Clone)/progressbar/Foreground");
		if(m_scDailyTaskFXRoot==null)		m_scDailyTaskFXRoot=GameObject.Find("MogoMainUI/Camera/Anchor/MogoMainUIPanel/DailyTaskUI(Clone)/DailyTaskContainer/DailyTaskFXRoot").GetComponent<DailyTaskFXRoot>();
	}
	
	IEnumerator PrintInfo()
	{
		
		yield return new WaitForSeconds(5.0f);
	}
	
	void PlayTaskFinishedAnimation()
	{
        Vector3 pos = transform.parent.parent.GetChild(0).GetComponent<Camera>().WorldToScreenPoint(m_GetAwardButton.transform.position);
        pos =  transform.parent.parent.GetChild(1).GetComponent<Camera>().ScreenToWorldPoint(pos);

        AssetCacheMgr.GetUIInstance("fx_ui_baoxiangxingxing.prefab", (prefab, id, go) =>
        {
            m_FinishedFX = (GameObject)go;
            m_FinishedFX.name = m_strOrder;
            m_FinishedFX.transform.parent = transform.parent.parent.Find("DailyTaskFXRoot").transform;
            m_FinishedFX.transform.position = pos + new Vector3(0,0,0);
			m_FinishedFX.transform.localScale=new Vector3(1.5f,1.5f,0);
			m_FinishedFX.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().startSize=200.0f;
			AutoDie sc=m_FinishedFX.AddComponent<AutoDie>();
			sc.m_fLifeTime=0.0f;
			sc.m_fDisapearIn=0.0f;
			sc.m_Target=m_GetAwardButton;
			sc.m_RelativeCamera=transform.parent.parent.GetChild(1).GetComponent<Camera>();
        });
	}
	
	void PlayGetAwardFX()
	{
        Vector3 pos = transform.parent.parent.GetChild(0).GetComponent<Camera>().WorldToScreenPoint(m_GetAwardButton.transform.position);
        pos =  transform.parent.parent.GetChild(1).GetComponent<Camera>().ScreenToWorldPoint(pos);

        AssetCacheMgr.GetUIInstance("fx_ui_icon_open.prefab", (prefab, id, go) =>
        {
            m_GetAwardFX = (GameObject)go;
            m_GetAwardFX.name = m_strOrder;
            m_GetAwardFX.transform.parent = m_scDailyTaskFXRoot.transform;
            m_GetAwardFX.transform.position = pos;
			m_GetAwardFX.transform.localScale=new Vector3(1.5f,1.5f,0);
			m_GetAwardFX.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().startSize=600.0f;
			m_GetAwardFX.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().startSize=600.0f;
			AutoDie sc=m_GetAwardFX.AddComponent<AutoDie>();
			sc.m_fDisapearIn=1.0f;
			sc.m_fLifeTime=1.0f;
			sc.m_WhenIDie=m_scDailyTaskFXRoot.OnFXDie;
			sc.m_Target=m_GetAwardButton;
			sc.m_RelativeCamera=transform.parent.parent.GetChild(1).GetComponent<Camera>();
        });
	}
	
	public 	void 	UpdateData(string strName,int nTaskID,int nExpAmount,int nGoldAmount,int nCurrentStep,int nTotalStep,
		string strTaskSprite,int nTaskStatus)
	{
		m_TaskName.GetComponent<UILabel>().text=strName;
		this.name=m_strOrder;
		m_nTaskID=nTaskID;
		m_ExpAmount.GetComponent<UILabel>().text=nExpAmount.ToString();
		m_GoldAmount.GetComponent<UILabel>().text=nGoldAmount.ToString();
		m_CurrentStep.GetComponent<UILabel>().text=Mathf.Clamp(nCurrentStep,0,nTotalStep).ToString()+"/"+nTotalStep.ToString();
		m_TaskIcon.GetComponent<UISprite>().spriteName=strTaskSprite;
		m_ProgressBar.gameObject.transform.localScale=
			new Vector3(295.0f*Mathf.Clamp((nCurrentStep*1.0f)/(nTotalStep*1.0f),0.0001f,1.0f),m_ProgressBar.gameObject.transform.localScale.y,0);
		switch(nTaskStatus)
		{
		case 0:
			m_GetAwardButton.SetActive(true);
			PlayTaskFinishedAnimation();
			m_AlreadyGetAwardButton.SetActive(false);
			m_UnfinishedButton.SetActive(false);
			m_FinishedText.SetActive(false);
			break;
		case 1:
			m_GetAwardButton.SetActive(false);
			m_AlreadyGetAwardButton.SetActive(false);
			m_UnfinishedButton.SetActive(true);
			m_FinishedText.SetActive(false);
			break;
		case 2:
			m_GetAwardButton.SetActive(false);
			m_AlreadyGetAwardButton.SetActive(true);
			m_FinishedText.SetActive(true);
			m_UnfinishedButton.SetActive(false);
			break;
		}
	}

	public void Link()
	{
        EventDispatcher.TriggerEvent<int>(Events.DailyTaskEvent.DailyTaskJumpToOtherUI, m_nTaskID > 10 ? m_nTaskID : m_nTaskID + 10);
	}
	
	public 	void 	UpdateProgress(int nCurrentStep,int nTotalStep)
	{
		m_CurrentStep.GetComponent<UILabel>().text=nCurrentStep.ToString()+"/"+nTotalStep.ToString();
		m_ProgressBar.gameObject.transform.localScale=new Vector3(295.0f*((nCurrentStep*1.0f)/(nTotalStep*1.0f)),m_ProgressBar.gameObject.transform.localScale.y,0);
	}
	
	IEnumerator		PlayGetAwardAnimation()
	{
        m_scDailyTaskFXRoot.OnNewFX();
		EventDispatcher.TriggerEvent(Events.DailyTaskEvent.GetDailyEventReward, m_nTaskID);
		if(m_FinishedFX!=null)
		{
			DestroyImmediate(m_FinishedFX);
			m_FinishedFX=null;
		}
		PlayGetAwardFX();
		yield return new WaitForSeconds(0.8f);
		m_GetAwardButton.SetActive(false);
		m_AlreadyGetAwardButton.SetActive(true);
		m_FinishedText.SetActive(true);
		m_UnfinishedButton.SetActive(false);
		yield return new WaitForSeconds(1.0f);
	}
	
	public	void 	OnGetAwardButtonPressed()
	{
		m_GetAwardButton.GetComponent<UIButtonMessage>().enabled=false;
		StartCoroutine(PlayGetAwardAnimation());
	}
	
	void OnDestroy()
	{
		m_TaskName=null;
		m_ExpAmount=null;
		m_GoldAmount=null;
		m_CurrentStep=null;
		m_TaskIcon=null;
		m_GetAwardButton=null;
		m_AlreadyGetAwardButton=null;
		m_UnfinishedButton=null;
		m_ProgressBar=null;
		m_FinishedText=null;
		if(m_FinishedFX!=null)
		{
			DestroyImmediate(m_FinishedFX);
			m_FinishedFX=null;
		}
		if(m_GetAwardFX!=null)
		{
			DestroyImmediate(m_GetAwardFX);
			m_GetAwardFX=null;
		}
		AssetCacheMgr.SynReleaseInstance(gameObject);
	}
}
