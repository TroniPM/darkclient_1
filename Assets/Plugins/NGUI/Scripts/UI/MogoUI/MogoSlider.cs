using UnityEngine;
using System.Collections;
using System;

public class MogoSlider : MonoBehaviour
{

    public float ProgressLength;
    public Transform SliderBeginPos;
    public Transform SliderEndPos;
    public GameObject Slider;
    public Camera RelatedCamera;
    public GameObject ProgressBar;
    public Action<float> OnSliding;

    bool m_bTryingToDrag = false;
    float m_fCurrentStatus = 0;


    const float m_fSlicedSpriteOffcet = 10f; //MaiFeo ÐÞÕýSlicedSprite Á½±ßÆ«ÒÆ

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            Vector3 touchPosInWorld = RelatedCamera.ScreenToWorldPoint(Input.mousePosition);

            if (touchPosInWorld.x > SliderBeginPos.position.x && touchPosInWorld.x < SliderEndPos.position.x)
            {
                m_bTryingToDrag = true;
            }

        }
        else
        {
            m_bTryingToDrag = false;
        }
    }

    void Update()
    {
        if (m_bTryingToDrag)
        {
            Vector3 touchPosInWorld = RelatedCamera.ScreenToWorldPoint(Input.mousePosition);

            if (touchPosInWorld.x < SliderBeginPos.position.x)
            {
                Slider.transform.position = SliderBeginPos.position;
                ProgressBar.transform.localScale = new Vector3(m_fSlicedSpriteOffcet, ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
            }
            else if (touchPosInWorld.x > SliderEndPos.position.x)
            {
                Slider.transform.position = SliderEndPos.position;
                ProgressBar.transform.localScale = new Vector3(ProgressLength, ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
            }
            else
            {
                float length = touchPosInWorld.x - SliderBeginPos.position.x;
                float totalLenth = SliderEndPos.position.x - SliderBeginPos.position.x;

                float lenghScale = length / totalLenth ;
                m_fCurrentStatus = lenghScale;
                //float sliderPos = SliderBeginPos.position.x + lenghScale * (SliderEndPos.position.x - SliderBeginPos.position.x);

                //Slider.transform.position = new Vector3(sliderPos, Slider.transform.position.y, Slider.transform.position.z);
                //ProgressBar.transform.localScale = new Vector3(lenghScale * ProgressLength, ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);

                SetCurrentStatus(m_fCurrentStatus);

                if (OnSliding != null)
                {
                    OnSliding(m_fCurrentStatus);
                }
            }
        }
    }

    public void SetCurrentStatus(float status)
    {
        if (status < 0)
        {
            status = 0;
        }
        else if (status > 1)
        {
            status = 1;
        }

        float sliderPos = SliderBeginPos.position.x + status * (SliderEndPos.position.x - SliderBeginPos.position.x);

        Slider.transform.position = new Vector3(sliderPos, Slider.transform.position.y, Slider.transform.position.z);
        ProgressBar.transform.localScale = new Vector3(status * ProgressLength, ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);

        if (ProgressBar.transform.localScale.x < m_fSlicedSpriteOffcet)
        {
            ProgressBar.transform.localScale += new Vector3(m_fSlicedSpriteOffcet, 0, 0);
        }
    }

    public float GetCurrentStatus()
    {
        return m_fCurrentStatus;
    }
}
