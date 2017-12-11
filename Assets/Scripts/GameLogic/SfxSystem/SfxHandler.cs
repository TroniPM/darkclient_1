#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：HandleCur
// 创建者：Ash Tang
// 修改者列表：Key Pan
// 创建日期：2013-2-6
// 模块描述：动作事件处理器。
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.GameData;
using System.Collections.Generic;
using Mogo.Util;
using System.IO;
using System;

/// <summary>
/// 动作事件处理器。
/// </summary>
public class SfxHandler : MonoBehaviour
{
    //private static Dictionary<int, GameObject> m_resourceDic = new Dictionary<int, GameObject>();
    private Dictionary<string, Transform> m_locationDic = new Dictionary<string, Transform>();
    private Dictionary<int, Dictionary<int, GameObject>> m_fxDic = new Dictionary<int, Dictionary<int, GameObject>>();
    private Dictionary<int, List<int>> m_groupFXList = new Dictionary<int, List<int>>();
    //private MeleeWeaponTrail[] m_weaponTrails;
    private Dictionary<string, MeleeWeaponTrail> m_weaponTrailsDic = new Dictionary<string, MeleeWeaponTrail>();
    private static Dictionary<string, Material> m_weaponTrailMaterial = new Dictionary<string, Material>();
    private static Dictionary<string, AnimationClip> m_animationClip = new Dictionary<string, AnimationClip>();
    private Renderer[] m_randerer;
    private Material[] m_mat;

    private static HashSet<string> m_loadedFX = new HashSet<string>();

    // 记录SlotCueHandler
    SlotCueHandler slotCueHandler;

    //void Start()
    //{
    //    //m_weaponTrails = GetComponentsInChildren<MeleeWeaponTrail>();
    //    //foreach (var item in m_weaponTrails)
    //    //{
    //    //    //Mogo.Util.LoggerHelper.Debug(item.transform.parent.name + "......................" + item.name);
    //    //    m_weaponTrailsDic[item.transform.parent.name] = item;
    //    //}
    //}

    void Awake()
    {
        slotCueHandler = gameObject.GetComponent<SlotCueHandler>();
        GetMaterials();
        EventDispatcher.AddEventListener<GameObject>(Events.OtherEvent.OnChangeWeapon, OnChangeWeapon);
        EventDispatcher.AddEventListener(ActorParent.ON_EQUIP_DONE, OnEquitDone);
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveEventListener<GameObject>(Events.OtherEvent.OnChangeWeapon, OnChangeWeapon);
        EventDispatcher.RemoveEventListener(ActorParent.ON_EQUIP_DONE, OnEquitDone);
        Clear();
    }

    public static void AddloadedFX(String fxResourceName)
    {
        m_loadedFX.Add(fxResourceName);
    }

    public static void UnloadAllFXs()
    {
        //Debug.LogError("m_loadedFX: " + m_loadedFX.Count);
        foreach (var item in m_loadedFX)
        {
            AssetCacheMgr.ReleaseResourceImmediate(item);
            //Debug.LogError("...............................UnloadAllFXs: " + item);
        }
        m_loadedFX.Clear();
    }

    /// <summary>
    /// 发射弓箭或火球等
    /// </summary>
    /// <param name="shootSfxId">飞行物</param>
    /// <param name="boomSfxId">碰撞后特效,-1代表无</param>
    /// <param name="target">目标</param>
    /// <param name="speed">速度</param>
    /// <param name="distance">如果目标为null时，到前方一定距离后就消失</param>
    public void Shoot(FXData fx, Transform target, float speed = 10, float distance = 30)
    {
        PlayFX(fx.id, fx, (go, guid) =>
        {
            //加上bullet脚本,设置参数
            var bullet = go.AddComponent<ActorBullet>();
            bullet.OnDestroy = () =>
            {
                RemoveFX(fx.id, guid);
            };
            Vector3 targetPosition = Vector3.zero;

            if (target == null)
                targetPosition = go.transform.position + transform.forward * distance;

            bullet.Setup(target, speed, targetPosition);
        });
    }

    /// <summary>
    /// 插入特效
    /// </summary>
    /// <param name="id">FXData id</param>
    /// <param name="action">加载对象回调</param>
    public void HandleFx(int id, Transform target = null, Action<GameObject, int> action = null, string bone_path = "")
    {
        if (FXData.dataMap.ContainsKey(id))
        {
            var fxData = FXData.dataMap[id];
            if (fxData.effectType == EffectType.Flying)
                Shoot(fxData, target);
            else
                PlayFX(id, fxData, action, bone_path);
        }
        else
            LoggerHelper.Warning(string.Format("Can not find fxData {0}", id));
    }

    // 提供给SlotCueHandler调用，用以删除特定的
    public void RemoveSlotCue(int id, int index)
    {
        RemoveFX(id, index);
    }

    public void Clear()
    {
        RemoveAllFX();
        m_mat = null;
        m_randerer = null;
        m_weaponTrailsDic.Clear();
        m_locationDic.Clear();
    }

    public void RemoveAllFX()
    {
        List<int> list = new List<int>();
        foreach (var fxs in m_fxDic)
        {
            list.Add(fxs.Key);
        }
        foreach (var item in list)
        {
            RemoveFXs(item);
        }
        m_fxDic.Clear();
        StopShaderFX();
    }

    public void RemoveFXs(int id)
    {
        if (id == 0)
            return;
        var fx = FXData.dataMap.Get(id);
        RemoveFXs(id, fx.group);
        if (id == currentShaderFx)
            StopShaderFX();
    }

    public void RemoveFXs(int id, int group)
    {
        if (m_fxDic.ContainsKey(id))
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            foreach (var item in m_fxDic[id])
            {
                list.Add(new KeyValuePair<int, int>(id, item.Key));
            }
            foreach (var item in list)
            {
                RemoveFX(item.Key, item.Value, group);
            }
            m_fxDic.Remove(id);
        }
    }

    public void RemoveFXByGroup(int group)
    {
        if (m_groupFXList.ContainsKey(group))
        {
            var list = m_groupFXList[group].ToArray();

            foreach (var item in list)
            {
                RemoveFXs(item, group);
            }
        }
    }

    public void RemoveFX(int id, int guid)
    {
        if (m_fxDic.ContainsKey(id) && m_fxDic[id].ContainsKey(guid))
        {
            GameObject.Destroy(m_fxDic[id][guid]);
            //AssetCacheMgr.ReleaseInstance(guid);
            m_fxDic[id].Remove(guid);
        }
    }

    private void OnChangeWeapon(GameObject go)
    {
        if (this && go)
        {
            var wt = go.GetComponent<MeleeWeaponTrail>();
            if (wt)
            {
                //LoggerHelper.Debug("MeleeWeaponTrail: " + go.name);
                m_weaponTrailsDic[wt.transform.parent.name] = wt;
            }
            else
            {
                //LoggerHelper.Debug("No MeleeWeaponTrail: " + go.name);
                //if (m_weaponTrailsDic.ContainsKey(wt.transform.parent.name))
                //    m_weaponTrailsDic.Remove(wt.transform.parent.name);
            }
        }
    }

    private void OnEquitDone()
    {
        //LoggerHelper.Error("OnEquitDone");
        if (this)
            GetMaterials();
    }

    private void GetMaterials()
    {
        var randers = new List<Renderer>();
        var smr = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        var mr = GetComponentsInChildren<MeshRenderer>(true);
        randers.AddRange(smr);
        randers.AddRange(mr);
        if (randers.Count != 0)
        {
            //LoggerHelper.Warning("randers.Count: " + randers.Count);
            m_randerer = randers.ToArray();
            m_mat = new Material[m_randerer.Length];
            for (int i = 0; i < m_randerer.Length; i++)
            {
                m_mat[i] = m_randerer[i].material;
            }
        }
        else
        {
            m_randerer = null;
            m_mat = null;
        }
    }

    ///// <param name="bone_path">绑特效的骨骼，默认为空（即使用xml里面指定的骨骼）</param>
    //private void HandleNormalFX(int id, string bone_path = "")
    //{
    //    if (!FXData.dataMap.ContainsKey(id))
    //    {
    //        LoggerHelper.Warning(string.Format("Can not find fx {0}", id));
    //        return;
    //    }

    //    HandleFX(id, FXData.dataMap[id], bone_path);
    //}

    /// <param name="bone_path">绑特效的骨骼，默认为空（即使用xml里面指定的骨骼）</param>
    private void PlayFX(int id, FXData fx, Action<GameObject, int> action = null, string bone_path = "")
    {
        //HandleWeaponTrial(fx);
        HandleAnim(fx);
        HandleFade(fx);

        if (!string.IsNullOrEmpty(fx.shader))
            HandleShaderFX(fx);
        if (!string.IsNullOrEmpty(fx.dissonShader))
            HandleDissonFx(fx);
        if (string.IsNullOrEmpty(fx.resourcePath))
            return;

        //销毁自身冲突
        if (fx.isConflict == FXConflict.Conflict && m_fxDic.ContainsKey(id))
            RemoveFXs(id);

        //去除组内冲突
        if (fx.group != 0)
        {
            RemoveFXByGroup(fx.group);
        }

        //AssetCacheMgr.GetResourceAutoRelease(fx.resourcePath, (obj) =>
        AssetCacheMgr.GetNoCacheInstance(fx.resourcePath, (prefab, guid, obj) =>
        {
            m_loadedFX.Add(fx.resourcePath);
            //Debug.LogError("m_loadedFX.Add: " + fx.resourcePath + " " + m_loadedFX.Count);
            //var go = GameObject.Instantiate(obj) as GameObject;
            //var guid = go.GetInstanceID();
            var go = obj as GameObject;
            if (!go || !this)
            {
                AssetCacheMgr.SynReleaseInstance(go);
                return;
            }
            if (!m_fxDic.ContainsKey(id))
            {
                m_fxDic[id] = new Dictionary<int, GameObject>();
            }
            m_fxDic[id].Add(guid, go);
            //处理音效
            var audio = go.GetComponent<AudioSource>();
            if (audio != null)
            {
                if (SettingsUIViewManager.Instance != null)
                    audio.volume = SoundManager.SoundVolume;

                FrameTimerHeap.AddTimer((uint)fx.soundDelay, 0, () =>
                {
                    if (audio != null)
                        audio.enabled = true;
                });
            }
            switch (fx.locationType)
            {
                case FXLocationType.SelfSlot:

                    // 判断输入路径是否指定，若未指定则绑在预设的骨上
                    if (bone_path == "")
                    {
                        if (!m_locationDic.ContainsKey(fx.slot))
                        {
                            m_locationDic.Add(fx.slot, GetBone(transform, fx.slot));
                        }
                        go.transform.parent = m_locationDic[fx.slot];
                    }

                    // 若已经指定则绑在指定的骨上
                    else
                    {
                        if (!m_locationDic.ContainsKey(bone_path))
                        {
                            m_locationDic.Add(bone_path, GetBone(transform, bone_path));
                        }
                        go.transform.parent = m_locationDic[bone_path];

                        // 记下index，以便删除
                        slotCueHandler.SetFxList(id, bone_path, guid);
                    }

                    go.transform.localPosition = go.transform.position;
                    go.transform.localRotation = go.transform.rotation;
                    break;
                case FXLocationType.World:
                    go.transform.position = fx.location;
                    break;
                case FXLocationType.SelfLocal:
                    go.transform.parent = transform;
                    //LoggerHelper.Debug(go.transform.position.x + " " + go.transform.position.y + " " + go.transform.position.z);
                    go.transform.localPosition = go.transform.position;
                    go.transform.localRotation = go.transform.rotation;
                    go.transform.forward = transform.forward;
                    break;
                case FXLocationType.SelfWorld:
                    go.transform.localPosition = transform.localPosition;
                    go.transform.localRotation = go.transform.rotation;
                    go.transform.position = transform.position;
                    go.transform.forward = transform.forward;
                    break;
                case FXLocationType.SlotWorld:

                    if (!m_locationDic.ContainsKey(fx.slot))
                    {
                        m_locationDic.Add(fx.slot, GetBone(transform, fx.slot));
                    }

                    var solt = m_locationDic[fx.slot];
                    go.transform.localPosition = solt.transform.localPosition;
                    go.transform.position = solt.transform.position;
                    go.transform.localRotation = solt.transform.rotation;
                    break;
                default:
                    break;
            }
            if (fx.isStatic == FXStatic.NotStatic)
            {
                Action actRemove = () =>
                {
                    if (this)
                        RemoveFX(id, guid, fx.group);
                };
                if (fx.duration > 0)
                    FrameTimerHeap.AddTimer((uint)(fx.duration * 1000), 0, actRemove);
                else
                    FrameTimerHeap.AddTimer(3000, 0, actRemove);
            }
            else
            {
                //如果技能为静态技能，分组标记非0，而且特效绑定到自身
                if (fx.group != 0 && (fx.locationType == FXLocationType.SelfSlot || fx.locationType == FXLocationType.SelfLocal))
                {
                    if (!m_groupFXList.ContainsKey(fx.group))
                        m_groupFXList.Add(fx.group, new List<int>());
                    m_groupFXList[fx.group].Add(id);
                }
            }
            if (action != null)
                action(go, guid);
        });
        //    m_fxDic.Add(id, new Dictionary<int, GameObject>());

        //return go;
    }

    ///// <summary>
    ///// 处理刀光
    ///// </summary>
    ///// <param name="fx"></param>
    //private void HandleWeaponTrial(FXData fx)
    //{
    //    if (!string.IsNullOrEmpty(fx.weaponTailMaterial))
    //    {
    //        if (m_weaponTrailsDic.ContainsKey(fx.weaponTailSlot))
    //        {
    //            var item = m_weaponTrailsDic[fx.weaponTailSlot];
    //            if (m_weaponTrailMaterial.ContainsKey(fx.weaponTailMaterial))
    //            {
    //                item.Material = m_weaponTrailMaterial[fx.weaponTailMaterial];
    //                item.subdivisions = fx.weaponTailSubdivisions;
    //                item._lifeTime = fx.weaponTailLeftTime;
    //                FrameFrameTimerHeap.AddTimer((uint)fx.weaponTailEmitTime, 0, () => { item.Emit = true; });
    //                FrameFrameTimerHeap.AddTimer((uint)(fx.weaponTailEmitTime + fx.weaponTailDurationTime), 0, () => { item.Emit = false; });
    //            }
    //            else
    //            {
    //                //to do: 资源释放
    //                AssetCacheMgr.GetResource(fx.weaponTailMaterial, (go) =>
    //                {
    //                    m_weaponTrailMaterial[fx.weaponTailMaterial] = go as Material;
    //                    item.Material = m_weaponTrailMaterial[fx.weaponTailMaterial];
    //                    item.subdivisions = fx.weaponTailSubdivisions;
    //                    item._lifeTime = fx.weaponTailLeftTime;
    //                    FrameFrameTimerHeap.AddTimer((uint)fx.weaponTailEmitTime, 0, () => { item.Emit = true; });
    //                    FrameFrameTimerHeap.AddTimer((uint)(fx.weaponTailEmitTime + fx.weaponTailDurationTime), 0, () => { item.Emit = false; });
    //                });
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 处理动画播放
    /// </summary>
    /// <param name="fx"></param>
    private void HandleAnim(FXData fx)
    {
        if (!String.IsNullOrEmpty(fx.anim))
        {
            if (!gameObject.GetComponent<Animation>())
                gameObject.AddComponent<Animation>();
            if (m_animationClip.ContainsKey(fx.anim))
            {
                gameObject.GetComponent<Animation>().AddClip(m_animationClip[fx.anim], fx.anim);
                gameObject.GetComponent<Animation>().Play(fx.anim);
            }
            else
            {
                //to do: 资源释放
                AssetCacheMgr.GetResourceAutoRelease(fx.anim, (go) =>
                {
                    m_animationClip[fx.anim] = go as AnimationClip;
                    gameObject.GetComponent<Animation>().AddClip(m_animationClip[fx.anim], fx.anim);
                    gameObject.GetComponent<Animation>().Play(fx.anim);
                });
            }
        }
    }

    /// <summary>
    /// 处理淡入淡出
    /// </summary>
    /// <param name="fx"></param>
    private void HandleFade(FXData fx)
    {
        if (fx.fadeStart != fx.fadeEnd)
        {
            try
            {
                if (m_mat != null)
                {
                    var color = GetMatColor("_Color");
                    if (color.a == fx.fadeEnd)
                    {
                        if (m_randerer != null && fx.fadeEnd == 0)
                            SetRanderEnable(false);
                        return;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LoggerHelper.Except(ex);
            }
            var startTime = Time.realtimeSinceStartup;
            Debug.Log("startTime: " + startTime);
            var repeatTimer = FrameTimerHeap.AddTimer((uint)fx.fadeDelay, 100, () =>
            {
                if (m_mat != null)
                {
                    var deltaTime = Time.realtimeSinceStartup - startTime;
                    //Debug.Log("deltaTime: " + deltaTime);
                    var target = fx.fadeStart + ((fx.fadeEnd - fx.fadeStart) * deltaTime * 1000 / fx.fadeDulation);
                    Debug.Log("target: " + target);
                    SetMatColor("_Color", new Color(1, 1, 1, target));
                }
            });
            FrameTimerHeap.AddTimer((uint)(fx.fadeDelay + fx.fadeDulation), 0, () =>
            {
                FrameTimerHeap.DelTimer(repeatTimer);
                if (m_randerer != null && fx.fadeEnd == 0)
                    SetRanderEnable(false);
            });
        }
    }

    private void HandleDissonFx(FXData fx)
    {
        orgShader = GetMatShader();
        AssetCacheMgr.GetResource(fx.dissonShader, (shader) =>
        {
            SetMatShader(shader as Shader);
            currentShaderFx = fx.id;
            AssetCacheMgr.GetResource(fx.nosieTexture, (nosieTexture) =>
            {
                SetMatTexture("_NosieTex", nosieTexture as Texture);
                float nosieOffset = fx.nosieOffetFrom;
                SetMatFloat("_NosieOffset", nosieOffset);

                TimerHeap.AddTimer((uint)fx.dissonDelay, 0, () =>
                {
                    //Debug.Log("begin HandleDissonFx");
                    var duration = fx.dissonDuration;
                    var loopTimer = TimerHeap.AddTimer(0, 100, () =>
                    {
                        SetMatFloat("_NosieOffset", nosieOffset);
                        nosieOffset = nosieOffset + (fx.nosieOffetTo - fx.nosieOffetFrom) * 100 / duration;
                        //Debug.Log("nosieOffset: " + nosieOffset);
                    });
                    TimerHeap.AddTimer((uint)duration, 0, () =>
                    {
                        //Debug.Log("DelTimer HandleDissonFx");
                        TimerHeap.DelTimer(loopTimer);
                    });
                });
            });
        });
        //Debug.Log("HandleDissonFx: " + fx.actionID);
    }

    private int currentShaderIndex = 0;
    private int nextShaderIndex = 1;
    private int curFrame = 0;
    private bool updatingShader = false;
    private int currentShaderFx = 0;
    private Shader orgShader;

    private void HandleShaderFX(FXData fx)
    {
        //LoggerHelper.Error("HandleShaderFX");
        orgShader = GetMatShader();
        AssetCacheMgr.GetResource(fx.shader, (shader) =>
        {
            //LoggerHelper.Error("SetMatShader: " + shader.name);
            SetMatShader(shader as Shader);
            currentShaderFx = fx.id;
            updatingShader = true;
            StartCoroutine(UpdateShader(fx));
        });
        //SetMatShader(Shader.Find("Mogo/FakeLight"));
        //Debug.Log("HandleShaderFX: " + fx.actionID);
    }

    public void StopShaderFX()
    {
        //StopCoroutine("UpdateShader");
        updatingShader = false;
        SetMatShader(orgShader);
        currentShaderFx = 0;
    }

    private IEnumerator UpdateShader(FXData fx)
    {
        while (updatingShader)
        {
            var ci = currentShaderIndex;
            var ni = nextShaderIndex;
            if (curFrame < fx.shaderDuration[ci])
            {
                var rimWidth = fx.rimWidth[ci] + ((fx.rimWidth[ni] - fx.rimWidth[ci]) * curFrame / fx.shaderDuration[ci]);
                var rimPower = fx.rimPower[ci] + ((fx.rimPower[ni] - fx.rimPower[ci]) * curFrame / fx.shaderDuration[ci]);
                var finalPower = fx.finalPower[ci] + ((fx.finalPower[ni] - fx.finalPower[ci]) * curFrame / fx.shaderDuration[ci]);
                var r = fx.r[ci] + ((fx.r[ni] - fx.r[ci]) * curFrame / fx.shaderDuration[ci]);
                var g = fx.g[ci] + ((fx.g[ni] - fx.g[ci]) * curFrame / fx.shaderDuration[ci]);
                var b = fx.b[ci] + ((fx.b[ni] - fx.b[ci]) * curFrame / fx.shaderDuration[ci]);
                var a = fx.a[ci] + ((fx.a[ni] - fx.a[ci]) * curFrame / fx.shaderDuration[ci]);
                SetMatColor("_RimColor", new Color(r, g, b, a));
                SetMatFloat("_RimWidth", rimWidth);
                SetMatFloat("_RimPower", rimPower);
                SetMatFloat("_FinalPower", finalPower);
                curFrame++;
            }
            else
            {
                curFrame = 0;
                currentShaderIndex++;
                if (currentShaderIndex >= fx.rimWidth.Count)
                    currentShaderIndex = 0;
                nextShaderIndex = currentShaderIndex + 1;
                if (nextShaderIndex >= fx.rimWidth.Count)
                    nextShaderIndex = 0;
            }
            yield return null;
        }
    }


    #region Mat & Render

    private void SetRanderEnable(bool value)
    {
        foreach (var item in m_randerer)
        {
            item.enabled = value;
        }
    }

    private Shader GetMatShader()
    {
        if (m_mat != null && m_mat.Length != 0)
            return m_mat[0].shader;
        else
            return null;
    }

    private void SetMatShader(Shader shader)
    {
        if (m_mat != null && shader)
            foreach (var item in m_mat)
            {
                //LoggerHelper.Error("SetMatShader item: " + item.name);
                item.shader = shader;
            }
    }

    private Color GetMatColor(string prop)
    {
        if (m_mat != null && m_mat.Length != 0)
            return m_mat[0].GetColor(prop);
        else
            return Color.clear;
    }

    private void SetMatColor(string prop, Color color)
    {
        if (m_mat != null)
            foreach (var item in m_mat)
            {
                item.SetColor(prop, color);
            }
    }

    private void SetMatFloat(string prop, float value)
    {
        if (m_mat != null)
            foreach (var item in m_mat)
            {
                item.SetFloat(prop, value);
            }
    }

    private void SetMatTexture(string prop, Texture texture)
    {
        foreach (var item in m_mat)
        {
            item.SetTexture(prop, texture);
        }
    }

    #endregion

    private Transform GetBone(Transform transform, string boneName)
    {
        Transform bone = transform.Find(boneName);
        if (bone == null)
        {
            foreach (Transform child in transform)
            {
                bone = GetBone(child, boneName);
                if (bone != null) return bone;
            }
        }
        return bone;
    }

    private void RemoveFX(int id, int guid, int group)
    {
        RemoveFX(id, guid);
        if (m_groupFXList.ContainsKey(group))
            m_groupFXList[group].Remove(id);
    }
}