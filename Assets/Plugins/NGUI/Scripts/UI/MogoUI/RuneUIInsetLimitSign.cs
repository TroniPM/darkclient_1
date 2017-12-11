using UnityEngine;
using System.Collections;

public class RuneUIInsetLimitSign : MonoBehaviour 
{

    public bool  IsBeginCountDown = false;
    float m_fLastTime = 0f;

    void Update()
    {
        if (IsBeginCountDown)
        {
            m_fLastTime += Time.deltaTime;

            if (m_fLastTime > 3.0f)
            {
                m_fLastTime = 0f;

                IsBeginCountDown = false;

                gameObject.SetActive(false);
            }
        }
    }
}
