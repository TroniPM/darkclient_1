using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.GameData;

public class FadeAgent : GearParent
{
    public int shader = 0;

    protected bool canFade;

    protected uint fadeInTimer;
    protected uint fadeOutTimer;

    void Start()
    {
        gearType = "FadeAgent";
        ID = (uint)defaultID;
        
        fadeInTimer = uint.MaxValue;
        fadeOutTimer = uint.MaxValue;

        if (ShaderData.dataMap.ContainsKey(shader)
            && gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) != null)
        {
            MogoFXManager.Instance.SetObjShader(gameObject, ShaderData.dataMap[shader].name, ShaderData.dataMap[shader].color);
            canFade = true;
        }
        else
        {
            canFade = false;
        }

        AddListeners();
    }

    public void SetMainFadeIn(float fadeTime)
    {
        if (fadeOutTimer != uint.MaxValue)
            TimerHeap.DelTimer(fadeOutTimer);
        fadeOutTimer = uint.MaxValue;

        if (fadeInTimer != uint.MaxValue)
            TimerHeap.DelTimer(fadeInTimer);
        fadeInTimer = uint.MaxValue;

        FadeIn(fadeTime);
    }

    public void SetSecondStageFadeIn(uint delay, float fadeTime)
    {
        if (fadeOutTimer != uint.MaxValue)
            TimerHeap.DelTimer(fadeOutTimer);
        fadeOutTimer = uint.MaxValue;

        if (fadeInTimer != uint.MaxValue)
            return;

        fadeInTimer = TimerHeap.AddTimer(delay, 0, FadeIn, fadeTime);
    }

    public void FadeIn(float fadeTime)
    {
        MogoFXManager.Instance.AlphaFadeIn(gameObject, fadeTime);
    }

    public void SetSecondStageFadeOut(float fadeTime)
    {
        if (fadeInTimer != uint.MaxValue)
            TimerHeap.DelTimer(fadeInTimer);
        fadeInTimer = uint.MaxValue;

        if (fadeOutTimer != uint.MaxValue)
            TimerHeap.DelTimer(fadeOutTimer);
        fadeOutTimer = uint.MaxValue;

        FadeOut(fadeTime);
    }

    public void SetMainFadeOut(uint delay, float fadeTime)
    {
        if (fadeInTimer != uint.MaxValue)
            TimerHeap.DelTimer(fadeInTimer);
        fadeInTimer = uint.MaxValue;

        if (fadeOutTimer != uint.MaxValue)
            return;

        fadeOutTimer = TimerHeap.AddTimer(delay, 0, FadeOut, fadeTime);
    }

    public void FadeOut(float fadeTime)
    {
        MogoFXManager.Instance.AlphaFadeOut(gameObject, fadeTime);
    }
}
