using UnityEngine;
using System.Collections;

public class MogoFootHalo : MonoBehaviour 
{
    public Vector3 Target;
    public bool IsEnable = true;

    void Update()
    {
        if (IsEnable)
        {
            Vector3 vec = Target - transform.position;
            transform.forward = new Vector3(vec.x, transform.forward.y, vec.z);
            transform.localEulerAngles += new Vector3(0, 45, 0);   //别想了 这是魔法数字
        }
    }

}
