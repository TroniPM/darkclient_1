using UnityEngine;
using System.Collections;
using System;

public class UIFXUnit : MonoBehaviour {

    public Action<float> UIFXPlayFuncWithFloat;
    public Action<float> UIFXStopFuncWithFloat;

    public void Play(float fadeTime = 0f)
    {
        if (UIFXPlayFuncWithFloat != null)
        {
            UIFXPlayFuncWithFloat(fadeTime);
        }
    }

    public void Stop(float fadeTime = 0f)
    {
        if (UIFXStopFuncWithFloat != null)
        {
            UIFXStopFuncWithFloat(fadeTime);
        }
    }
}
