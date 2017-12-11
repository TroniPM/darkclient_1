using UnityEngine;
using System.Collections;

public class MogoButtonDownScale : MonoBehaviour {

    Transform m_myTransform;

    public float XScale = 1f;
    public float YScale = 1f;
    public float ZScale = 1f;

    void Awake()
    {
        m_myTransform = transform;

    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            m_myTransform.localScale = new Vector3(XScale, YScale, ZScale);
        }
        else
        {
            m_myTransform.localScale = Vector3.one;
        }
    }
}
