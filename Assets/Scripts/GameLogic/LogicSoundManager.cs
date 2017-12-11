using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class LogicSoundManager
{
    #region 主角音效

    public static Dictionary<int, AudioClip> avatarAudioClipBuffer = new Dictionary<int, AudioClip>();

    #endregion


    #region 初始化

    static LogicSoundManager()
    {
        //LoggerHelper.Debug("ConstructLogicAudioManager");
    }

    //~LogicAudioManager()
    //{
    //    RemoveListeners();
    //}

    public static void Init()
    {
        AddListeners();
    }

    public static void AddListeners()
    {
        EventDispatcher.AddEventListener<EntityParent, int>(Events.LogicSoundEvent.OnHitYelling, OnHitYelling);
    }

    public static void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<EntityParent, int>(Events.LogicSoundEvent.OnHitYelling, OnHitYelling);
    }

    #endregion


    #region 逻辑音效播放

    #region 主角音效播放

    public static void MyselfLogicPlaySound(AudioSource ownerSource, int soundID)
    {
        if (avatarAudioClipBuffer.ContainsKey(soundID))
        {
            EventDispatcher.TriggerEvent<AudioSource, AudioClip>(SettingEvent.LogicPlaySoundByClip, ownerSource, avatarAudioClipBuffer[soundID]);
            return;
        }

        if (!SoundData.dataMap.ContainsKey(soundID))
        {
            LoggerHelper.Debug("Sound ID " + soundID + " not exist");
            return;
        }

        AssetCacheMgr.GetResourceAutoRelease(SoundData.dataMap[soundID].path, (obj) =>
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);

            if (obj is AudioClip)
            {
                EventDispatcher.TriggerEvent<AudioSource, AudioClip>(SettingEvent.LogicPlaySoundByClip, ownerSource, obj as AudioClip);

                if (!avatarAudioClipBuffer.ContainsKey(soundID))
                    avatarAudioClipBuffer.Add(soundID, obj as AudioClip);
            }
        });
    }

    #endregion

    protected static void LogicPlaySound(AudioSource ownerSource, int soundID)
    {
        EventDispatcher.TriggerEvent<int, AudioSource>(SettingEvent.LogicPlaySoundByID, soundID, ownerSource);
    }

    #endregion


    #region 逻辑音效处理

    #region 受击音效

    protected static void OnHitYelling(EntityParent entity, int action)
    {
        AudioSource ownerSource = entity.audioSource;
        if (ownerSource == null)
            return;

        int ownerVocation = (int)entity.vocation;
        if (entity is EntityMonster)
        {
            ownerVocation = (int)((entity as EntityMonster).MonsterData.id);
        }
        else if (entity is EntityMercenary)
        {
            ownerVocation = (int)((entity as EntityMercenary).MonsterData.id);
        }
        else if (entity is EntityDummy)
        {
            ownerVocation = (int)((entity as EntityDummy).MonsterData.id);
        }

        ActionSoundData data = ActionSoundData.dataMap.FirstOrDefault(t => t.Value.vocation == ownerVocation && t.Value.action == action).Value;

        if (data == null)
            return;

        int sum = 0;
        foreach (var soundMessage in data.sound)
            sum += soundMessage.Value;

        int soundID = -1;
        int temp = RandomHelper.GetRandomInt(0, sum);
        foreach (var soundMessage in data.sound)
        {
            if (temp < soundMessage.Value)
            {
                soundID = soundMessage.Key;
                break;
            }
            temp -= soundMessage.Value;
        }

        if (soundID == -1)
            return;

        if (entity is EntityMyself)
        {
            MyselfLogicPlaySound(ownerSource, soundID);
        }
        else
        {
            LogicPlaySound(ownerSource, soundID);
        }
    }

    #endregion

    #endregion
}
