/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ActorParent
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-2-20
// 模块描述：Actor 对象的基类
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;
using Object = UnityEngine.Object;

public class ActorParent<T> : ActorParent where T : EntityParent
{
    private T m_theEntity;
    public T theEntity
    {
        get { return m_theEntity; }
        set { m_theEntity = value; }
    }
    public override EntityParent GetEntity()
    {
        return m_theEntity;
    }
}

public class ActorParent : MonoBehaviour
{
    public const string ON_EQUIP_DONE = "ActorParent.ON_EQUIP_DONE";

    private MecanimEvent m_mecanimEvent;
    protected Animator m_animator;
    protected string preActName = "";
    public Action<string, string> ActChangeHandle;

    private Action<String, Boolean> m_animatorStateChanged;
    public Action<String, Boolean> AnimatorStateChanged
    {
        get
        {
            return m_animatorStateChanged;
        }
        set
        {
            m_animatorStateChanged = value;
        }
    }

    private Action<String, Boolean> m_hitStateChanged;
    public Action<String, Boolean> HitStateChanged
    {
        get
        {
            return m_hitStateChanged;
        }
        set
        {
            m_hitStateChanged = value;
        }
    }

    public bool isNeedInitEquip = true;
    private uint m_checkAnimationChangeTimeHeapId = uint.MaxValue;

    virtual public EntityParent GetEntity()
    {
        return null;
    }

    // 初始化
    virtual protected void Awake()
    {
        m_animator = GetComponent<Animator>();
        if (m_animator)
        {
            m_mecanimEvent = new MecanimEvent(m_animator);
            m_checkAnimationChangeTimeHeapId = TimerHeap.AddTimer(500, 0, StartCheckAnimationChange);
        }

        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);

        //Debug.LogError("ActorParent Awake" + tag);
    }

    void OnDestroy()
    {
        //Debug.LogError("ActorParent OnDestroy" + tag);

        //RemoveAll();
        RemoveOld();

        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
        TimerHeap.DelTimer(m_checkAnimationChangeTimeHeapId);
        StopCoroutine("CheckAnimationChange");
    }

    private int idleCnt = 0;
    virtual public void ActChange()
    {
        if (m_animator == null)
        {
            return;
        }
        if (m_animator.IsInTransition(0))
        {//融合期间
            return;
		}
		var state = m_animator.GetCurrentAnimatorClipInfo(0);
        if (state.Length == 0)
        {
            return;
        }
        string currName = state[0].clip.name;
        if (currName != preActName)
        {//动作变换
            if (ActChangeHandle != null)
            {
                ActChangeHandle(preActName, currName);
            }
            if (!currName.EndsWith("ready") && !currName.EndsWith("run") &&
                !currName.EndsWith("idle") && !currName.EndsWith("powercharge") &&
                !currName.EndsWith("powercharge_loop") && !currName.EndsWith("powercharge_left") &&
                !currName.EndsWith("roll"))
            {
                SkillAction a = null;
                if (GetEntity().currSpellID != -1 &&
                    SkillAction.dataMap.TryGetValue(SkillData.dataMap[GetEntity().currSpellID].skillAction[0], out a))
                {//只为当前版本所用，新版本中动作不一样了要去掉
                    if (a.duration <= 0)
                    {
                        m_animator.SetInteger("Action", 0);
                    }
                    else
                    {//旋风斩之类技能使用
                        m_animator.SetInteger("Action", -3);
                    }
                }
                else
                {
                    m_animator.SetInteger("Action", 0);
                }
            }
            preActName = currName;
        }
        if ((currName.EndsWith("hit") && preActName.EndsWith("hit")) ||
            (currName.EndsWith("push") && preActName.EndsWith("push")) ||
            (currName.EndsWith("hitair") && preActName.EndsWith("hitair")) ||
            (currName.EndsWith("knockdown") && preActName.EndsWith("knockdown")))
        {
            int act = m_animator.GetInteger("Action");
            if (act != 0 && act != -1)
            {
                m_animator.SetInteger("Action", 0);
            }
        }
        if (GetEntity() != null &&
            currName != null &&
            GetEntity().stiff &&
            (currName.EndsWith("ready") ||
            currName.EndsWith("run") ||
            currName.EndsWith("run_left") ||
            currName.EndsWith("run_right") ||
            currName.EndsWith("run_back")))
        {
            idleCnt++;
            if (idleCnt > 5)
            {
                GetEntity().ClearHitAct();
                idleCnt = 0;
            }
        }
        else
        {
            idleCnt = 0;
        }
    }

    virtual public void Release()
    {
        RemoveAll();
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
        TimerHeap.DelTimer(m_checkAnimationChangeTimeHeapId);
        StopCoroutine("CheckAnimationChange");
    }

    virtual public void Attack(int _SpellID)
    {
    }

    virtual public void Idle()
    {
    }

    virtual public void OnHit(int _SpellID)
    {
    }

    virtual public void Walk()
    {
    }

    public void ReleaseController()
    {
        if (m_animator != null)
        {
            if (m_animator.runtimeAnimatorController != null)
                AssetCacheMgr.ReleaseResource(m_animator.runtimeAnimatorController);

            m_animator = null;
        }
    }

    public void ChangeMotionStateInFrames(string nextState, params object[] args)
    {
        int inFrames = 3;
        StartCoroutine(SwitchToNextStateInFrames(nextState, inFrames, args));
    }

    public IEnumerator SwitchToNextStateInFrames(string nextState, int inFrames, params object[] args)
    {
        int n = 0;
        while (n < inFrames)
        {
            yield return new WaitForFixedUpdate();
            n += 1;
        }
        GetEntity().ChangeMotionState(nextState, args);
    }

    private void OnStateChanged(string name, bool isStart)
    {
        if (AnimatorStateChanged != null)
        {
            AnimatorStateChanged(name, isStart);
        }
        if (HitStateChanged != null)
        {
            HitStateChanged(name, isStart);
        }
    }

    private void StartCheckAnimationChange()
    {
        if (this && this.gameObject.activeSelf)
            StartCoroutine("CheckAnimationChange");
    }

    private IEnumerator CheckAnimationChange()
    {
        return m_mecanimEvent.CheckAnimationChange(OnStateChanged);
    }

    public void FloatBlood(int hp, SplitBattleBillboardType type = SplitBattleBillboardType.CriticalPlayer)
    {
        if (MogoUtils.GetChild(transform, "slot_billboard"))
            BillboardLogicManager.Instance.AddSplitBattleBillboard(MogoUtils.GetChild(transform, "slot_billboard").position, hp, type);
    }

    #region 包装基于帧的回调函数

    // 基于帧的回调函数。 用于处理必须在异帧 完成的事情。
    public void AddCallbackInFrames(Action callback, int inFrames = 3)
    {
        StartCoroutine(CallBackInFrames(callback, inFrames));
    }

    public void AddCallbackInFrames<U>(Action<U> callback, U arg1, int inFrames = 3)
    {
        StartCoroutine(CallBackInFrames(callback, arg1, inFrames));
    }

    public void AddCallbackInFrames<U, V>(Action<U, V> callback, U arg1, V arg2, int inFrames = 3)
    {
        StartCoroutine(CallBackInFrames(callback, arg1, arg2, inFrames));
    }

    public void AddCallbackInFrames<U, V, T>(Action<U, V, T> callback, U arg1, V arg2, T arg3, int inFrames = 3)
    {
        StartCoroutine(CallBackInFrames(callback, arg1, arg2, arg3, inFrames));
    }

    public void AddCallbackInFrames<U, V, T, W>(Action<U, V, T, W> callback, U arg1, V arg2, T arg3, W arg4, int inFrames = 3)
    {
        StartCoroutine(CallBackInFrames(callback, arg1, arg2, arg3, arg4, inFrames));
    }

    IEnumerator CallBackInFrames(Action callback, int inFrames)
    {
        int n = 0;
        while (n < inFrames)
        {
            yield return new WaitForFixedUpdate();
            n += 1;
        }
        callback();
    }

    IEnumerator CallBackInFrames<U>(Action<U> callback, U arg1, int inFrames)
    {
        int n = 0;
        while (n < inFrames)
        {
            yield return new WaitForFixedUpdate();
            n += 1;
        }
        callback(arg1);
    }

    IEnumerator CallBackInFrames<U, V>(Action<U, V> callback, U arg1, V arg2, int inFrames)
    {
        int n = 0;
        while (n < inFrames)
        {
            yield return new WaitForFixedUpdate();
            n += 1;
        }
        callback(arg1, arg2);
    }

    IEnumerator CallBackInFrames<U, V, T>(Action<U, V, T> callback, U arg1, V arg2, T arg3, int inFrames)
    {
        int n = 0;
        while (n < inFrames)
        {
            yield return new WaitForFixedUpdate();
            n += 1;
        }
        callback(arg1, arg2, arg3);
    }

    IEnumerator CallBackInFrames<U, V, T, W>(Action<U, V, T, W> callback, U arg1, V arg2, T arg3, W arg4, int inFrames)
    {
        int n = 0;
        while (n < inFrames)
        {
            yield return new WaitForFixedUpdate();
            n += 1;
        }
        callback(arg1, arg2, arg3, arg4);
    }

    #endregion

    #region 换装
    private static object m_equipWeaponLock = new object();

    public bool m_isChangingWeapon = false;
    //身上已穿装备<部位类型，装备id>
    private Dictionary<int, int> m_equipOnDic = new Dictionary<int, int>();
    ////裸体时的“装备”（外形显示）
    private List<int> m_nakedEquipList = new List<int>();
    //已装备中的用挂载方法装备的装备
    private List<GameObject> m_equipList = new List<GameObject>();
    //已装备中的Mesh或Material
    private List<Object> m_equipMeshOrMaterialList = new List<Object>();
    private List<GameObject> weaponObj = new List<GameObject>();
    public List<GameObject> WeaponObj { get { return weaponObj; } }
    private EquipData m_weaponData = null;
    private List<SkinnedMeshRenderer> m_smrList = new List<SkinnedMeshRenderer>();
    public List<SkinnedMeshRenderer> SmrList { get { return m_smrList; } }
    private List<SkinnedMeshRenderer> m_smrAllList = new List<SkinnedMeshRenderer>();

    private Dictionary<int, EquipObjectData> m_equipGoDic = new Dictionary<int, EquipObjectData>();

    public EquipData WeaponData
    {
        get
        {
            return m_weaponData;
        }
    }

    public Dictionary<int, int> EquipOnDic
    {
        get
        {
            return m_equipOnDic;
        }
    }

    //private SkinnedMeshRenderer m_smr;

    //用于合成装备
    //List<CombineInstance> m_combineInstances = new List<CombineInstance>();
    //List<Material> m_materials = new List<Material>();
    //List<Transform> m_bones = new List<Transform>();
    //Dictionary<string, Transform> m_bonesDic = new Dictionary<string, Transform>();

    /// <summary>
    /// 初始化装备外形
    /// </summary>
    public void InitEquipment()
    {
        Mogo.Util.LoggerHelper.Debug("name:" + transform.name + ",vocation:" + (int)GetEntity().vocation);
        if (!AvatarModelData.dataMap.ContainsKey((int)GetEntity().vocation)) return;
        AvatarModelData data = AvatarModelData.dataMap[(int)GetEntity().vocation];
        if (data.nakedEquipList == null || data.nakedEquipList.Count <= 0) return;
        SetNakedEquid(data.nakedEquipList);
        InitNakedEquid();
    }

    public void InitEquipment(int vocation)
    {
        if (!AvatarModelData.dataMap.ContainsKey(vocation)) return;
        AvatarModelData data = AvatarModelData.dataMap[vocation];
        if (data.nakedEquipList == null || data.nakedEquipList.Count <= 0) return;
        SetNakedEquid(data.nakedEquipList);
        InitNakedEquid();
    }

    /// <summary>
    /// 初始显示裸装
    /// </summary>
    public void InitNakedEquid()
    {
        //Mogo.Util.LoggerHelper.Debug("InitNakedEquid");
        ClearOriginalModel();
        PutOnNakedEquip();
        ReEquidAll(m_equipOnDic);
    }

    private void ClearOriginalModel()
    {
        if (m_smrAllList.Count <= 0)
        {
            foreach (Transform t in transform)
            {
                SkinnedMeshRenderer smr = t.GetComponent<SkinnedMeshRenderer>();
                if (smr == null) continue;
                smr.gameObject.SetActive(true);
                smr.sharedMesh = null;
                m_smrAllList.Add(smr);
            }
        }
        else
        {
            foreach (SkinnedMeshRenderer smr in m_smrAllList)
            {
                smr.gameObject.SetActive(true);
                smr.sharedMesh = null;
            }
        }

    }

    /// <summary>
    /// 设置裸装，用于卸装时恢复裸装
    /// </summary>
    /// <param name="ids"></param>
    public void SetNakedEquid(List<int> _nakedEquipList)
    {
        m_nakedEquipList = _nakedEquipList;
    }

    public void Equip(List<int> idList, Action onDone = null)
    {
        //for (int i = 0; i < idList.Count; i++)
        //{
        //    if (i == idList.Count - 1)
        //        Equip(idList[i], onDone);
        //    else
        //        Equip(idList[i]);
        //}


        //Mogo.Util.LoggerHelper.Debug("Equid:" + _equidID);
        List<EquipData> equipDataList = new List<EquipData>();
        for (int i = 0; i < idList.Count; i++)
        {
            int equidID = idList[i];
            EquipData equipData = EquipData.dataMap.GetValueOrDefault(equidID, null);
            if (equipData == null)
                return;

            if (m_equipOnDic.ContainsValue(equipData.id)) continue;


            equipDataList.Add(equipData);
        }

        if (equipDataList.Count == 0)
        {
            if (onDone != null) onDone();
            return;
        }
        GetEquipObjectList(equipDataList, (equipGoDic, equipOnDic) =>
        {
            m_equipGoDic = equipGoDic;

            RemoveOld();

            //根据优先级等重装所有装备
            ReEquidAll(equipOnDic);

            if (onDone != null)
            {
                onDone();
            }
            EventDispatcher.TriggerEvent(ON_EQUIP_DONE);
        });
    }

    /// <summary>
    /// 刷新身上装备，有时候因为模型没创建好却调用装备接口引起装备失败
    /// </summary>
    public void RefreshEquip()
    {
        RemoveOld();
        //穿上裸装
        PutOnNakedEquip();


        //根据优先级等重装所有装备
        ReEquidAll(m_equipOnDic);
    }


    //旧版
    //public void Equip(int _equidID)
    //{
    //    //Mogo.Util.LoggerHelper.Debug("Equid:" + _equidID);

    //    EquipData equipData = EquipData.dataMap.GetValueOrDefault(_equidID, null);
    //    if (equipData == null)
    //        return;

    //    foreach (int id in m_equipOnDic.Values)
    //    {
    //        if (id == equipData.id) return;
    //    }

    //    RemoveOld();

    //    //判断是否需要解除套装
    //    foreach (int t in equipData.type)
    //    {
    //        if (m_equipOnDic.ContainsKey(t))
    //        {
    //            UncombineEquip(equipData, t);
    //        }
    //    }

    //    //去陈推新
    //    foreach (int t in equipData.type)
    //    {

    //        if (m_equipOnDic.ContainsKey(t))
    //        {
    //            EquipData old = EquipData.dataMap[m_equipOnDic[t]];
    //            if (old != null && old.type.Count > 0)
    //            {
    //                foreach (int temp in old.type)
    //                {
    //                    m_equipOnDic.Remove(temp);
    //                }
    //            }
    //        }

    //        m_equipOnDic[t] = equipData.id;
    //    }

    //    //判断是否需要装备“合体”
    //    if (equipData.type.Count == 1)
    //    {
    //        CombineEquip(equipData);
    //    }

    //    //穿上裸装
    //    PutOnNakedEquip();


    //    //根据优先级等重装所有装备
    //    ReEquidAll();
    //    //Mogo.Util.LoggerHelper.Debug("Equid done:" + _equidID);
    //}

    /// <summary>
    /// 穿上装备
    /// </summary>
    /// <param name="_equidID"></param>
    public void Equip(int _equidID, Action onDone = null)
    {
        //Mogo.Util.LoggerHelper.Debug("Equid:" + _equidID);

        EquipData equipData = EquipData.dataMap.GetValueOrDefault(_equidID, null);
        if (equipData == null)
            return;

        foreach (int id in m_equipOnDic.Values)
        {
            if (id == equipData.id)
            {
                if (onDone != null) onDone();
                return;
            }
        }

        List<EquipData> equipDataList = new List<EquipData>();
        equipDataList.Add(equipData);
        if (equipDataList.Count == 0)
        {
            if (onDone != null) onDone();
            return;
        }
        GetEquipObjectList(equipDataList, (equipGoDic, equipOnDic) =>
        {
            if (!this) return;
            if (transform == null) return;
            m_equipGoDic = equipGoDic;

            RemoveOld();

            //根据优先级等重装所有装备
            ReEquidAll(equipOnDic);

            if (onDone != null)
            {
                onDone();

            }
            EventDispatcher.TriggerEvent(ON_EQUIP_DONE);
        });
    }

    protected void PutOnNakedEquip()
    {
        if (m_nakedEquipList == null) return;
        foreach (int id in m_nakedEquipList)
        {
            if (!EquipData.dataMap.ContainsKey(id))
            {
                Mogo.Util.LoggerHelper.Debug("cannot find equipData:" + id);
                LoggerHelper.Error("cannot find equipData:" + id);
            }
            EquipData equip = EquipData.dataMap[id];
            if (m_equipOnDic.ContainsKey(equip.type[0])) continue;
            m_equipOnDic[equip.type[0]] = id;
        }
    }

    private void UncombineEquip(EquipData equipData, int type)
    {
        EquipData old = EquipData.dataMap[m_equipOnDic[type]];
        if (old.subEquip == null) return;
        if (old.subEquip.Count > 0)
        {
            foreach (int id in old.subEquip)
            {
                EquipData equip = EquipData.dataMap[id];

                //子装备只允许为一个type
                m_equipOnDic[equip.type[0]] = equip.id;
            }
        }
    }

    private void CombineEquip(EquipData newEquip)
    {
        if (newEquip.suit <= 0) return;
        int count = 0;
        foreach (int id in m_equipOnDic.Values)
        {
            EquipData equip = EquipData.dataMap[id];
            if (equip.type[0] == newEquip.type[0])
            {
                count++;
                continue;
            }
            if (equip.suit == newEquip.suit) count++;
        }
        if (count != newEquip.suitCount) return;

        EquipData suit = EquipData.dataMap[newEquip.suit];
        foreach (int id in suit.subEquip)
        {
            EquipData equip = EquipData.dataMap[id];
            m_equipOnDic[equip.type[0]] = suit.id;
        }
    }

    /// <summary>
    /// 卸下装备
    /// </summary>
    /// <param name="_equidID"></param>
    public void RemoveEquid(int _equidID)
    {
        //EquipData equipData = EquipData.dataMap[_equidID];

        //if (!m_equipOnDic.ContainsValue(_equidID) && !m_equipOnDic.ContainsValue(equipData.suit)) return;

        //RemoveOld();

        //if (equipData.type.Count == 1)
        //{
        //    if (m_equipOnDic.ContainsKey(equipData.type[0]))
        //    {
        //        UncombineEquip(equipData, equipData.type[0]);
        //    }
        //}

        //foreach (int t in equipData.type)
        //{
        //    m_equipOnDic.Remove(t);
        //}
        //PutOnNakedEquip();
        //ReEquidAll();
    }

    /// <summary>
    /// 按优先级顺序重新穿上所有装备
    /// </summary>
    protected void ReEquidAll(Dictionary<int, int> equipOnDic)
    {
        List<int> equipIds = new List<int>();
        foreach (int id in equipOnDic.Values)
        {
            equipIds.Add(id);
        }

        equipIds.Sort(delegate(int a, int b)
        {
            if (EquipData.dataMap[a].priority >= EquipData.dataMap[b].priority) return 1;
            else return -1;
        });

        HashSet<int> suitHasPuton = new HashSet<int>();

        //m_combineInstances.Clear();
        //m_materials.Clear();
        //m_bones.Clear();

        for (int i = 0; i < equipIds.Count; i++)
        {
            if (suitHasPuton.Contains(equipIds[i])) continue;
            EquipData equip = EquipData.dataMap[equipIds[i]];
            AddEquid(equip);
            if ((equip.subEquip != null && equip.subEquip.Count > 0) || equip.type.Count > 1)
                suitHasPuton.Add(equip.id);
        }

        //m_smr.sharedMesh = new Mesh();
        //m_smr.sharedMesh.CombineMeshes(m_combineInstances.ToArray(), false, false);
        //m_smr.bones = m_bones.ToArray();
        //m_smr.materials = m_materials.ToArray();


    }

    /// <summary>
    /// 卸下所有装备只剩裸装
    /// </summary>
    public void RemoveAll()
    {
        m_equipOnDic.Clear();
        RemoveOld();
        PutOnNakedEquip();
        ReEquidAll(m_equipOnDic);
    }

    public void RemoveOld()
    {
        //weaponData = null;
        //foreach (GameObject go in weaponObj)
        //{
        //    AssetCacheMgr.ReleaseInstance(go);
        //}

        //weaponObj.Clear();

        for (int i = 0; i < m_equipList.Count; i++)
        {
            AssetCacheMgr.ReleaseInstance(m_equipList[i], false);
            m_equipList[i] = null;
        }
        for (int i = 0; i < m_equipMeshOrMaterialList.Count; i++)
        {
            AssetCacheMgr.ReleaseResource(m_equipMeshOrMaterialList[i], false);
            m_equipMeshOrMaterialList[i] = null;
        }
        for (int i = 0; i < m_smrList.Count; i++)
        {
            m_smrList[i].sharedMaterial = null;
            m_smrList[i].sharedMesh = null;
            m_smrList[i] = null;
        }

        //m_smr.sharedMaterial = null;
        //m_smr.sharedMesh = null;

        m_smrList.Clear();
        m_equipList.Clear();
        m_equipMeshOrMaterialList.Clear();
    }

    private void AddEquid(EquipData equip)
    {
        if (equip.putOnMethod == 1)
        {
            AddEquidMethod1(equip);
        }
        else
        {
            AddEquidMethod2(equip);
        }
    }

    /// <summary>
    /// 替换mesh和material
    /// </summary>
    /// <param name="equipData"></param>
    private void AddEquidMethod2(EquipData equipData)
    {
        if (!m_equipGoDic.ContainsKey(equipData.id))
        {
            LoggerHelper.Warning("!m_equipGoDic.ContainsKey(equipData.id):" + equipData.id);
            return;
        }
        Material material = m_equipGoDic[equipData.id].mat;
        GameObject instance = m_equipGoDic[equipData.id].goList[0];
        if (transform == null) return;
        Transform equipPart = MogoUtils.GetChild(transform, equipData.slot[0]);

        if (equipPart == null)
        {
            Mogo.Util.LoggerHelper.Debug("can not find slot:" + equipData.slot[0] + ",equipId:" + equipData.id);
            Mogo.Util.LoggerHelper.Debug("gameObject:" + name);
            LoggerHelper.Error("can not find slot:" + equipData.slot[0] + ",equipId:" + equipData.id + ",vocation:" + GetEntity().vocation);
            return;
        }

        SkinnedMeshRenderer smr = equipPart.GetComponent<SkinnedMeshRenderer>();
        if (!smr)//安全检查
            return;
        m_smrList.Add(smr);
        m_equipMeshOrMaterialList.Add(material);
        smr.sharedMaterial = material;
        smr.castShadows = false;
        smr.receiveShadows = false;
        smr.useLightProbes = true;
        SkinnedMeshRenderer smrTemp = m_equipGoDic[equipData.id].smr;

        if (equipData.type.Count > 1) ClearOriginalModel();

        smr.sharedMesh = smrTemp.sharedMesh;
        //CombineInstance ci = new CombineInstance();
        //ci.mesh = smrTemp.sharedMesh;
        //m_combineInstances.Add(ci);

        List<Transform> bones = new List<Transform>();
        for (int i = 0; i < smrTemp.bones.Length; i++)
        {
            bones.Add(MogoUtils.GetChild(this.transform, smrTemp.bones[i].name));
        }
        //m_bones.AddRange(bones);
        smr.bones = bones.ToArray();
        m_equipMeshOrMaterialList.Add(instance);


    }

    //private bool isEquipingWeapon = false;//以挂载方式加的武器
    EquipData currentEquip;
    const string EQUIP_TAP = "equip";

    /// <summary>
    /// 装备挂载在某个gameObject下
    /// </summary>
    /// <param name="equipData"></param>
    private void AddEquidMethod1(EquipData equipData)
    {
        if (transform == null) return;
        if (!m_equipGoDic.ContainsKey(equipData.id))
        {
            LoggerHelper.Warning("!m_equipGoDic.ContainsKey(equipData.id):" + equipData.id);
            return;
        }
        int ccount = 0;

        currentEquip = equipData;

        //if (equipData.isWeapon > 0)
        //{
        //foreach (GameObject go in weaponObj)
        //{
        //    //LoggerHelper.Debug("release go:" + go.name);
        //    AssetCacheMgr.ReleaseInstance(go, false);
        //    //GameObject.Destroy(go);
        //}
        weaponObj.Clear();
        m_weaponData = equipData;
        //LoggerHelper.Debug("clear");
        //}
        //LoggerHelper.Debug("equipData.prefabPath.ccount:" + equipData.prefabPath.Count);

        EquipObjectData eod = m_equipGoDic[equipData.id];
        for (int i = 0; i < equipData.prefabPath.Count; i++)
        {
            int index = i;
            GameObject equipGo = null;


            equipGo = eod.goList[index];
            if (equipGo == null)
            {
                //Mogo.Util.LoggerHelper.Debug("equip load fail!:" + equipData.id);
                LoggerHelper.Error("equip load fail!");
                ccount++;
                return;
            }

            Transform equipPart = null;
            if (equipGo == null) return;
            if (transform == null) return;

            if (MogoWorld.inCity)
            {
                equipPart = MogoUtils.GetChild(transform, equipData.slotInCity[ccount]);
            }
            else
            {
                equipPart = MogoUtils.GetChild(transform, equipData.slot[ccount]);
            }


            if (equipPart == null)
            {
                //Mogo.Util.LoggerHelper.Debug("cannot find equip slot!:" + equipData.slot[ccount]);
                LoggerHelper.Error("cannot find equip slot!");
                ccount++;
                return;
            }

            //LoggerHelper.Debug("equipPart:" + equipPart.name);
            //Transform tempTransform;
            //if ((tempTransform = equipPart.FindChild(equipGo.name)) != null)
            //{
            //    Mogo.Util.LoggerHelper.Debug("已经装备：" + equipGo.name);
            //    AssetCacheMgr.ReleaseInstance(tempTransform.gameObject);
            //    weaponObj.Remove(tempTransform.gameObject);
            //}

            Vector3 scale = equipGo.transform.localScale;
            equipGo.transform.parent = equipPart;
            equipGo.transform.localPosition = Vector3.zero;
            equipGo.transform.localEulerAngles = Vector3.zero;
            equipGo.transform.localScale = scale;

            if (equipData.isWeapon > 0)
            {
                weaponObj.Add(equipGo);
            }
            //保存已穿装备，方便替换时卸载
            m_equipList.Add(equipGo);

            //LoggerHelper.Debug("load done:" + equipGo.name);
            ccount++;
            //LoggerHelper.Debug("ccount:" + ccount);

            if (ccount == equipData.prefabPath.Count)
            {
                //Mogo.Util.LoggerHelper.Debug("ccount == equipData.prefabPath.Count");
                //isEquipingWeapon = false;
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("ccount != equipData.prefabPath.Count");
            }
            if (m_isChangingWeapon)
            {
                m_isChangingWeapon = false;
                EventDispatcher.TriggerEvent<GameObject>(Events.OtherEvent.OnChangeWeapon, equipGo);
                if (GetEntity() != null)
                    GetEntity().weaponAnimator = equipGo.GetComponent<Animator>();
            }

        }
        LoggerHelper.Debug("equip done:" + currentEquip.id);

        //StartCoroutine(haha(equipData, c));
        //while (ccount < equipData.prefabPath.Count)
        //{
        //    if (currentEquip.id == 1040403)
        //    {
        //        LoggerHelper.Debug(equipData.prefabPath.Count);
        //    }
        //    LoggerHelper.Debug("count < equipData.prefabPath.Count,count:" + ccount + ",equipData.prefabPath.Count:" + equipData.prefabPath.Count + ",equip:" + currentEquip.id);
        //    yield return null;
        //}
        //isEquipingWeapon = false;
        //LoggerHelper.Debug("end equip done:" + currentEquip.id);
        //yield break;
        //}
    }

    /// <summary>
    /// 装备换位，一些动作变换需要用到
    /// </summary>
    /// <param name="equipName"></param>
    /// <param name="partName"></param>
    public void ChangeEquipPosition(string equipName, string partName)
    {
        Transform t = MogoUtils.GetChild(transform, equipName);
        t.parent = MogoUtils.GetChild(transform, partName);
        t.localPosition = Vector3.zero;
        t.localEulerAngles = Vector3.zero;
    }

    public void ChangeEquipPosition(GameObject weapon, string partName)
    {
        if (weapon == null) return;
        Transform t = weapon.transform;
        t.parent = MogoUtils.GetChild(transform, partName);
        t.localPosition = Vector3.zero;
        t.localEulerAngles = Vector3.zero;
    }

    private void InstanceLoaded(int copyId, bool isInCopy)
    {
        Mogo.Util.LoggerHelper.Debug("InstanceLoaded");
        LoggerHelper.Debug(EQUIP_TAP, "InstanceLoaded");
        StartCoroutine(SwitchWeaponPos(isInCopy));

        if (tag == "Player")
        {
            int id = InventoryManager.Instance.GetCurrentWeaponId();
            ItemEquipmentData equip = ItemEquipmentData.dataMap.Get(id);
            int subtype = equip.subtype;
            int type = equip.type;
            ControllerOfWeaponData controllerData = ControllerOfWeaponData.dataMap[subtype];
            RuntimeAnimatorController controller;
            if (this == null) return;
            if (transform == null) return;
            Animator animator = GetEntity().animator;
            if (animator == null) return;

            string controllerName = (isInCopy ? controllerData.controller : controllerData.controllerInCity);
            if (animator.runtimeAnimatorController != null)
            {
                if (animator.runtimeAnimatorController.name == controllerName) return;
                AssetCacheMgr.ReleaseResource(animator.runtimeAnimatorController);
            }


            AssetCacheMgr.GetResource(controllerName,
            (obj) =>
            {
                controller = obj as RuntimeAnimatorController;
                if (this == null) return;
                if (transform == null) return;
                if (animator == null) return;
                animator.runtimeAnimatorController = controller;

                //(GetEntity() as EntityMyself).UpdateSkillToManager();
                //EventDispatcher.TriggerEvent<int, int>(InventoryEvent.OnChangeEquip, type, subtype);

            });
        }
    }

    public IEnumerator SwitchWeaponPos(bool isInCopy)
    {
        Mogo.Util.LoggerHelper.Debug("SwitchWeaponPos:" + isInCopy);
        LoggerHelper.Debug(EQUIP_TAP, "SwitchWeaponPos");
        //while (isEquipingWeapon)
        //{
        //    yield return null;
        //}
        //isEquipingWeapon = true;
        //Mogo.Util.LoggerHelper.Debug("SwitchWeaponPos!!!!!");
        if (this == null)
        {
            Mogo.Util.LoggerHelper.Debug("SwitchWeaponPos this == null!");
            //LoggerHelper.Error("SwitchWeaponPos this == null!");
            //isEquipingWeapon = false;
            yield break;
        }
        if (m_weaponData == null)
        {
            Mogo.Util.LoggerHelper.Debug("weaponData == null!");
            //LoggerHelper.Error("weaponData == null!");
            //isEquipingWeapon = false;
            yield break;
        }
        List<string> slots;
        if (isInCopy)
        {
            if (m_weaponData.slot == null)
            {
                LoggerHelper.Error("weaponData.slot == null!");
                yield break;
            }
            slots = m_weaponData.slot;
        }
        else
        {
            if (m_weaponData.slotInCity == null)
            {
                LoggerHelper.Error("weaponData.slotInCity == null!");
                yield break;
            }
            slots = m_weaponData.slotInCity;
        }

        if (weaponObj.Count != slots.Count)
        {
            Mogo.Util.LoggerHelper.Debug("weaponData.prefabPath.Count != slots.Count");
            Mogo.Util.LoggerHelper.Debug("weaponData.prefabPath.Count:" + weaponObj.Count);
            Mogo.Util.LoggerHelper.Debug("slots.Count:" + slots.Count);
            Mogo.Util.LoggerHelper.Debug("weaponData.id:" + m_weaponData.id);
            //LoggerHelper.Error("weaponData.prefabPath.Count != slots.Count");
            //isEquipingWeapon = false;
            yield break;
        }

        for (int i = 0; i < weaponObj.Count && i < slots.Count; i++)
        {
            //Mogo.Util.LoggerHelper.Debug("weaponObj[i].name:" + weaponObj[i].name + ",slot[i]" + slots[i]);
            ChangeEquipPosition(weaponObj.Get(i), slots.Get(i));
        }
        //Mogo.Util.LoggerHelper.Debug("SwitchWeaponPos done!!!!!");

        //isEquipingWeapon = false;
    }

    private void GetEquipObjectList(List<EquipData> equipDataList, Action<Dictionary<int, EquipObjectData>, Dictionary<int, int>> onLoad)
    {
        HashSet<int> equipIdSet = new HashSet<int>();
        for (int i = 0; i < equipDataList.Count; i++)
        {
            equipIdSet = GetEquipIdSet(equipDataList[i]);
        }

        Dictionary<int, int> tempEquipOnDic = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> pair in m_equipOnDic)
        {
            tempEquipOnDic[pair.Key] = pair.Value;
        }

        Dictionary<int, EquipObjectData> eo = new Dictionary<int, EquipObjectData>();
        int count = 0;
        foreach (int id in equipIdSet)
        {
            eo[id] = new EquipObjectData();
            var index = id;
            EquipData equip = EquipData.dataMap.Get(id);
            if (equip.putOnMethod == 1)
            {
                GetInstanceList(equip.prefabPath, (goList) =>
                {
                    eo[index].type = 1;
                    eo[index].goList = goList;
                    count++;
                    if (count == equipIdSet.Count) onLoad(eo, tempEquipOnDic);
                });
            }
            else if (equip.putOnMethod == 2)
            {
                eo[index].type = 2;
                AssetCacheMgr.GetResource(equip.material + ".mat",
                (obj) =>
                {
                    eo[index].mat = obj as Material;
                });
                AssetCacheMgr.GetResource(equip.mesh + ".FBX",
                (obj) =>
                {
                    GameObject go = obj as GameObject;
                    eo[index].goList = new List<GameObject>();
                    eo[index].goList.Add(go);
                    eo[index].smr = go.transform.Find(equip.mesh).GetComponent<SkinnedMeshRenderer>();

                    count++;
                    if (count == equipIdSet.Count) onLoad(eo, tempEquipOnDic);
                });
            }
        }

    }

    private HashSet<int> GetEquipIdSet(EquipData equipData)
    {

        HashSet<int> equipIdSet = new HashSet<int>();

        //foreach (int id in m_equipOnDic.Values)
        //{
        //    if (equipIdSet.Contains(id)) continue;
        //    equipIdSet.Add(id);
        //}
        //foreach (int i in equipIdSet)
        //{
        //    Debug.LogError(i + ",count1:" + equipIdSet.Count);
        //}

        //foreach (int t in equipData.type)
        //{
        //    if (m_equipOnDic.ContainsKey(t))
        //    {
        //        if (equipIdSet.Contains(m_equipOnDic[t]))
        //            equipIdSet.Remove(m_equipOnDic[t]);
        //    }
        //    if (!equipIdSet.Contains(equipData.id))
        //        equipIdSet.Add(equipData.id);
        //}

        //判断是否需要解除套装
        foreach (int t in equipData.type)
        {
            if (m_equipOnDic.ContainsKey(t))
            {
                UncombineEquip(equipData, t);
            }
        }

        //去陈推新
        foreach (int t in equipData.type)
        {

            if (m_equipOnDic.ContainsKey(t))
            {
                EquipData old = EquipData.dataMap[m_equipOnDic[t]];
                if (old != null && old.type.Count > 0)
                {
                    foreach (int temp in old.type)
                    {
                        m_equipOnDic.Remove(temp);
                    }
                }
            }
            m_equipOnDic[t] = equipData.id;
        }

        //判断是否需要装备“合体”
        if (equipData.type.Count == 1)
        {
            CombineEquip(equipData);
        }

        //穿上裸装
        PutOnNakedEquip();


        foreach (int id in m_equipOnDic.Values)
        {
            if (!equipIdSet.Contains(id)) equipIdSet.Add(id);
        }

        return equipIdSet;
    }

    private void GetInstanceList(List<string> prefabNameList, Action<List<GameObject>> onLoad)
    {
        AssetCacheMgr.GetInstances(prefabNameList.ToArray(), (gos) =>
        {
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < gos.Length; i++)
            {
                goList.Add(gos[i] as GameObject);
            }
            onLoad(goList);
        });
    }


    class EquipObjectData
    {
        public int type;//1挂载，2mesh
        public List<GameObject> goList;
        public Material mat;
        public SkinnedMeshRenderer smr;
    }
    #endregion

    public List<int> GetEquipList()
    {
        List<int> list = new List<int>();
        if (m_equipOnDic != null)
        {
            foreach (int i in m_equipOnDic.Values)
            {
                if (list.Contains(i)) continue;
                list.Add(i);
            }
        }
        return list;
    }
}