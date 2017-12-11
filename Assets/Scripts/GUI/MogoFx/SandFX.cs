using UnityEngine;
using System.Collections;

public class SandFX : MonoBehaviour
{

    public float scrollSpeed = 10f;
    float offset = 0f;

    public bool isHorizonal = false;

    UITexture tex;
    TweenAlpha ta;

    void Awake()
    {
        tex = transform.GetComponentsInChildren<UITexture>(true)[0];
        ta = transform.GetComponentsInChildren<TweenAlpha>(true)[0];

        GetComponentsInChildren<UIFXUnit>(true)[0].UIFXPlayFuncWithFloat = Play;
        GetComponentsInChildren<UIFXUnit>(true)[0].UIFXStopFuncWithFloat = Stop;
    }

    void Update()
    {
        offset += (scrollSpeed * 0.01f);

        if (offset > 1000)
        {
            offset = 0;
        }

        tex.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));

    }

    public void Play(float fadeTime = 1f)
    {

        gameObject.SetActive(true);
        if (ta != null)
        {

            ta.Reset();
            tex.color = new Color(tex.color.r, tex.color.g, tex.color.b, 0);
            ta.from = 0f;
            ta.to = 1f;
            ta.duration = fadeTime;
            ta.callWhenFinished = "";


            ta.Play(true);
        }
    }

    public void Stop(float fadeTime = 1f)
    {
        if (ta != null)
        {
            ta.Reset();
            tex.color = new Color(tex.color.r, tex.color.g, tex.color.b, 1);
            ta.from = 1f;
            ta.to = 0f;
            ta.duration = fadeTime;
            ta.callWhenFinished = "OnStopDone";
            ta.eventReceiver = gameObject;
            ta.Play(true);
        }
    }

    void OnStopDone()
    {
        Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        GameObject.Destroy(gameObject);
    }
}
