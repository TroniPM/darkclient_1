using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoFlashSurface : MonoBehaviour {

    UITexture m_texLight;
    float m_fOffset = -1;
    public int m_fSpeed = 2;
    bool m_bStartFlashing = true;

    void Awake()
    {
        m_texLight = GetComponentsInChildren<UITexture>(true)[0];
    }

    void SetTheTextureOffset(float offset)
    {
        m_texLight.material.SetTextureOffset("_MainTex", new Vector2(offset,0));
    }

    void Update()
    {
        if (m_bStartFlashing)
        {
            SetTheTextureOffset(m_fOffset += m_fSpeed * 0.01f);

            if (m_fOffset >= 1f)
            {
                m_fOffset = -1f;

                m_bStartFlashing = false;

                TimerHeap.AddTimer(3000, 0, () => { m_bStartFlashing = true; });
            }
        }
    }
}
