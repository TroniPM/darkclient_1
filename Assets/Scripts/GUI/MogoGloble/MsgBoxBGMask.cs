using UnityEngine;
using System.Collections;

public class MsgBoxBGMask : MonoBehaviour {

	
    void OnClick()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
