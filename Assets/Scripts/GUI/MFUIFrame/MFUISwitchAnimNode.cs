using UnityEngine;
using System.Collections;

public class MFUISwitchAnimNode: MonoBehaviour
{
    public System.Action OnAnimDone;

    void Awake()
    {
        
    }

    private void ResetAnimStatus()
    {
 
    }

    public void Play(MFUISwitchAnim.MFUISwitchAnimType type)
    {
        Debug.LogError("Playing " + " type  == "+ type);
    }

    private void CallWhenPlayDone()
    {
        if (OnAnimDone != null)
        {
            OnAnimDone();
        }
    }
}
