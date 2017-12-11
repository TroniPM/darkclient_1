using UnityEngine;
using System.Collections;
using Mogo.Util;

public class Gadget : MonoBehaviour 
{
    private Transform m_transform;
    private BoxCollider m_collider;
    private SfxHandler handler;
    private int m_fxID;
    void Start()
    {
        m_transform = transform;
    }
    public void EvolveToEndlessFX(int fxID,uint start)
    {
        m_fxID = fxID;
        handler = gameObject.AddComponent<SfxHandler>();
        TimerHeap.AddTimer(start, 0, () => { handler.HandleFx(m_fxID, m_transform); });
        
    }
    public void StopFX()
    {
        handler.RemoveFXs(m_fxID);
    }
}
