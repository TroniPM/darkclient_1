using UnityEngine;
using System.Collections;

public class MogoCameraLighting : MonoBehaviour
{
    //public Material Mat;

    TweenAlpha m_ta;
    UISprite m_spLighting;

    void Awake()
    {
        //AssetCacheMgr.GetUIResource("MogoCameraLighting.mat", (go) =>
        //{
        //    Mat = go as Material;

        //    Mat.SetFloat("_LastTime", 0);
        //});

        m_ta = GameObject.Find("BillboardPanel").transform.Find("CGCameraLighting").GetComponentsInChildren<TweenAlpha>(true)[0];
        m_spLighting = m_ta.transform.parent.GetComponentsInChildren<UISprite>(true)[0];

        
    }

    void OnTAEnd()
    {
        m_ta.gameObject.SetActive(false);
    }

    //void OnRenderImage(RenderTexture source, RenderTexture dest)
    //{
        
    //    Graphics.Blit(source, dest, Mat);
    //}

    public void SetCenter(Vector2 vec2)
    {
        //Mat.SetFloat("_CenterX", vec2.x);
        //Mat.SetFloat("_CenterY", vec2.y);
    }

    public void SetControll(float controll)
    {
        //Mat.SetFloat("_Controll", controll);
    }

    public void SetLastTime(float time)
    {
        //Mat.SetFloat("_LastTime", time);
        m_ta.duration = time;
    }

    public void ResetToNoneWhite()
    {
        //Mat.SetFloat("_Controll", 0);
        m_spLighting.color = new Color(1, 1, 1, 0);
    }

    public void ResetToAllWhite()
    {
        //Mat.SetFloat("_Controll", 100);
        m_spLighting.color = new Color(1, 1, 1, 1);
    }

    public void SetFadeColor(Color color)
    {
        //Mat.SetColor("_Color",color);
        m_spLighting.color = color;
    }

    public void StartFade(bool forward)
    {
        m_ta.Reset();

        m_ta.gameObject.SetActive(true);

        if (forward)
        {
            m_spLighting.color = new Color(1, 1, 1, 0);
            m_ta.from = 0;
            m_ta.to = 1;

            m_ta.eventReceiver = null;
        }
        else
        {
            m_spLighting.color = new Color(1, 1, 1, 1);
            m_ta.from = 1;
            m_ta.to = 0;

            m_ta.eventReceiver = gameObject;
            m_ta.callWhenFinished = "OnTAEnd";
        }

        m_ta.Play(true);
    }

}
