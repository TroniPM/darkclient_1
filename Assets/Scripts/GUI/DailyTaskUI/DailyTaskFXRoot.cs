using UnityEngine;
using System.Collections;

public class DailyTaskFXRoot : MonoBehaviour {
	int 	m_nGetAwardFXCount=0;
    public int ChildCount
    {
        get
        {
            return m_nGetAwardFXCount;
        } 
    }

    public void OnNewFX()
    {
        m_nGetAwardFXCount++;
    }

    public void OnFXDie()
    {
        m_nGetAwardFXCount--;
    }
}
