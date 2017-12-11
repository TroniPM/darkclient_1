using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;

public class SettingsUILogicManager
{

    private static SettingsUILogicManager m_instance;

    public static SettingsUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SettingsUILogicManager();
            }

            return SettingsUILogicManager.m_instance;

        }
    }
    public void Initialize()
    {
        var index = Mogo.Util.SystemConfig.Instance.PlayerCountInScreen / 5 - 1;
        Mogo.Util.LoggerHelper.Debug("PlayerCountInScreen index: " + index);
        SettingsUIViewManager.Instance.SetCurrentUIPeopleInScreeBtnDown(index);
        SettingsUIViewManager.Instance.ShowGraphicQualityDesripe(Mogo.Util.SystemConfig.Instance.GraphicQuality);
        SettingsUIViewManager.Instance.SetCurrentUIGraphicQualityBtnDown(Mogo.Util.SystemConfig.Instance.GraphicQuality);
        
        SettingsUIViewManager.Instance.OnPeopleInScreenChanged += PeopleInScreenChanged;
        SettingsUIViewManager.Instance.OnGraphicQualityChanged += GraphicQualityChanged;
        CallWhenMusicSliderSlidering((musicVolumn) =>
        {
            Mogo.Util.LoggerHelper.Debug("musicVolumn" + musicVolumn);
            EventDispatcher.TriggerEvent(SettingEvent.MotityMusicVolume, musicVolumn);
        });

        CallWhenSoundSliderSlidering((soundVolumn) =>
        {
            Mogo.Util.LoggerHelper.Debug("soundVolumn" + soundVolumn);
            EventDispatcher.TriggerEvent(SettingEvent.MotitySoundVolume, soundVolumn);
        });
    }

    public void Release()
    {
        SettingsUIViewManager.Instance.OnPeopleInScreenChanged -= PeopleInScreenChanged;
        SettingsUIViewManager.Instance.OnGraphicQualityChanged -= GraphicQualityChanged;
    }


    public void CallWhenMusicSliderSlidering(Action<float> act)
    {
        SettingsUIViewManager.Instance.CallWhenMusicSliderSlidering(act);
    }

    public void CallWhenSoundSliderSlidering(Action<float> act)
    {
        SettingsUIViewManager.Instance.CallWhenSoundSliderSlidering(act);
    }

    private void GraphicQualityChanged(int id)
    {
        SettingsUIViewManager.Instance.ShowGraphicQualityDesripe(id);
        Mogo.Util.SystemConfig.Instance.GraphicQuality = id;
        Mogo.Util.SystemConfig.SaveConfig();
    }

    private void PeopleInScreenChanged(int count)
    {
        if (count >= 0 && count <= 50)
        {
            Mogo.Util.SystemConfig.Instance.PlayerCountInScreen = count;
            Mogo.Util.SystemConfig.SaveConfig();
        }
    }
}
