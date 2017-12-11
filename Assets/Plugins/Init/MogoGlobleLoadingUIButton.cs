using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoGlobleLoadingUIButton : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        if (GetComponentsInChildren<AudioSource>(true) != null && GetComponentsInChildren<AudioSource>(true).Length > 0)
            audioSource = GetComponentsInChildren<AudioSource>(true)[0];
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
        }
        else
        {
            EventDispatcher.TriggerEvent("MogoGlobleLoadingUIOnPress");
            PlaySound();
        };
    }

    protected void PlaySound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip, 1);
        }
    }
}
