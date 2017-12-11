using UnityEngine;
using System.Collections;

public class AutoDie : MonoBehaviour
{
	public GameObject		 	m_Target = null;
	public Camera	 		 	m_RelativeCamera=null;
	public System.Action	    m_WhenIDie=null;
	public	float 				m_fDisapearIn=0.0f;
	public 	float 				m_fLifeTime=0.0f;
	void Start () 
	{
		if(m_fDisapearIn>0.0f)
		{
			StartCoroutine(Disapear());
		}
	}
	
	IEnumerator Disapear()
	{
		while(m_fDisapearIn>0.0f)
		{	
			m_fDisapearIn-=Time.deltaTime;
			yield return null;
		}
		gameObject.SetActive(false);
		while(m_fLifeTime>0.0f)
		{	
			m_fLifeTime-=Time.deltaTime;
			yield return null;
		}
		AssetCacheMgr.ReleaseInstance(gameObject);
	}
	
	void Update()
	{
		if(null!=m_Target)
		{
			Vector3 pos =transform.parent.parent.GetChild(0).GetComponent<Camera>().WorldToScreenPoint(m_Target.transform.position);
   			transform.position = m_RelativeCamera.ScreenToWorldPoint(new Vector3(pos.x,pos.y,0));
		}
	}
	
	void OnDisable()
	{
		AssetCacheMgr.ReleaseInstance(gameObject);
	}
	
	void OnDestroy()
	{
        if (null != m_WhenIDie)
		{
            m_WhenIDie();
		}
	}
}
