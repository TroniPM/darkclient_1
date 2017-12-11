using UnityEngine;
using System.Collections;
using System;

public class MogoObjOptWorker : MonoBehaviour
{
    
    Transform m_myTransform;

    public Action BecameVisibleCB;
    public Action BecameInVisibleCB;

    void OnBecameVisible()
    {
        if (BecameVisibleCB != null)
        {
            BecameVisibleCB();
        }
    }

    void OnBecameInvisible()
    {
        if (BecameInVisibleCB != null)
        {
            BecameInVisibleCB();
        }
    }
}
