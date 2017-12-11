using UnityEngine;
using System.Collections;

public class LoadingAnim : MonoBehaviour
{

    Transform m_myTransform;

    public Vector3 TweenStart;
    public Vector3 TweenEnd;
    public Vector3 TweenSpeed;

    Vector3 m_vec3Current;

    void Awake()
    {
        m_myTransform = transform;
    }

    void Start()
    {
        m_myTransform.localEulerAngles = TweenStart;
        m_vec3Current = TweenStart;
    }

    void Update()
    {
        if (m_vec3Current.z < TweenEnd.z)
        {
            m_vec3Current.z = 0;
        }

        m_vec3Current = m_vec3Current - new Vector3(TweenSpeed.x * Time.deltaTime, TweenSpeed.y * Time.deltaTime, TweenSpeed.z * Time.deltaTime);
        m_myTransform.localEulerAngles = m_vec3Current;
    }
}
