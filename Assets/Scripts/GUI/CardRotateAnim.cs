using UnityEngine;
using System.Collections;

public class CardRotateAnim : MonoBehaviour {

    bool m_bStartPlay = false;
    float scaleOffset = 0f;
    public float Speed = 1f;
    Transform m_myTransform;
    bool m_bPlayed = false;

   public GameObject CardFX;

    void Awake()
    {
        m_myTransform = transform;
    }
    public void Play()
    {
        if (m_bPlayed)
            return;
        m_bPlayed = true;
        m_bStartPlay = true;
    }

    void Update()
    {
        if (m_bStartPlay)
        {
            scaleOffset += Speed;

            if (scaleOffset >= 180f)
            {
                m_bStartPlay = false;
                scaleOffset = 180f;
            }

            m_myTransform.localEulerAngles = new Vector3(0, scaleOffset, 0);
        }
    }

    public void ShowCardFX(bool isShow)
    {

        if (CardFX != null)
            CardFX.SetActive(isShow);
    }


}
