/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SpellManager
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-3-5
// 模块描述：技能管理系统
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;
using Mogo.FSM;

public class SkillMapping
{
    public int normalAttack = 0;
    public int spellOne = 0;
    public int spellTwo = 0;
    public int spellThree = 0;
    public int powerupAttack = 0;
    public int maxPowerupAttack = 0;
    public List<int> powers = new List<int>();

    public void clean()
    {
        normalAttack = 0;
        spellOne = 0;
        spellTwo = 0;
        spellThree = 0;
        powerupAttack = 0;
        maxPowerupAttack = 0;
        powers.Clear();
    }

    public bool hadStudy(int id)
    {
        SkillData s = SkillData.dataMap[id];
        SkillData s1 = null;
        if (SkillData.dataMap.TryGetValue(normalAttack, out s1))
        {
            if (s.posi == s1.posi && s.level <= s1.level)
            {
                return true;
            }
        }
        if (SkillData.dataMap.TryGetValue(spellOne, out s1))
        {
            if (s.posi == s1.posi && s.level <= s1.level)
            {
                return true;
            }
        }
        if (SkillData.dataMap.TryGetValue(spellTwo, out s1))
        {
            if (s.posi == s1.posi && s.level <= s1.level)
            {
                return true;
            }
        }
        if (SkillData.dataMap.TryGetValue(spellThree, out s1))
        {
            if (s.posi == s1.posi && s.level <= s1.level)
            {
                return true;
            }
        }
        return (id == normalAttack || id == spellOne ||
            id == spellTwo || id == spellThree ||
             powers.Contains(id));
    }
}

public class PlayerSkillManager : SkillManager
{
    // 公共cd 为 0.2 秒
    public int CommonCD = 200;
    private float lastAttackTime = 0.0f;
    private int lastSkillID = 0;
    private int currentSpellID = 0;    // 当前正在使用的技能配置数据(未使用技能时为0)
    private int currentHitSpellID = 0; // 受击技能
    private int lastPowerSkillID = 0;
    public bool isAnger = false;

    // 技能cd 相关
    private Dictionary<int, float> skilllastCastTime = new Dictionary<int, float>();
    private Dictionary<int, int> skillCoolTime = new Dictionary<int, int>();

    private Dictionary<int, List<int>> dependenceSkill = new Dictionary<int, List<int>>();
    private Dictionary<int, int> comboSkillPeriodStart = new Dictionary<int, int>();
    private Dictionary<int, int> comboSkillPeriod = new Dictionary<int, int>();
    private Dictionary<int, int> commonCD = new Dictionary<int, int>();

    private SkillMapping skillMapping = new SkillMapping();

    public PlayerSkillManager(EntityParent owner)
        : base(owner)
    {
        theOwner = owner;
        AddListeners();
        try
        {
            InitData();
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    public int CurrentSpellID
    {
        get { return currentSpellID; }
        set { currentSpellID = value; }
    }

    public int CurrentHitSpellID
    {
        get { return currentHitSpellID; }
        set { currentHitSpellID = value; }
    }

    public override void Clean()
    {
        DelListeners();
        base.Clean();
    }
    // 为去除警告暂时屏蔽以下代码
    //private readonly int PRESKILL = 0;
    //private readonly int PRECOUNT = 1;
    private readonly int NEXTSKILL = 2;

    private void AddListeners()
    {
        EventDispatcher.AddEventListener<int, int>(InventoryEvent.OnChangeEquip, OnChangeEquip);
        EventDispatcher.AddEventListener(Events.SpellEvent.OpenView, OpenViewHandler);
        EventDispatcher.AddEventListener<int, int>(Events.SpellEvent.Study, StudyHandler);

        EventDispatcher.AddEventListener<int>(Events.ComboEvent.AddCombo, AddCombo);
        EventDispatcher.AddEventListener(Events.ComboEvent.ResetCombo, ForceResetCombo);
    }

    private void DelListeners()
    {
        EventDispatcher.RemoveEventListener<int, int>(InventoryEvent.OnChangeEquip, OnChangeEquip);
        EventDispatcher.RemoveEventListener(Events.SpellEvent.OpenView, OpenViewHandler);
        EventDispatcher.RemoveEventListener<int, int>(Events.SpellEvent.Study, StudyHandler);

        EventDispatcher.RemoveEventListener<int>(Events.ComboEvent.AddCombo, AddCombo);
        EventDispatcher.RemoveEventListener(Events.ComboEvent.ResetCombo, ForceResetCombo);
    }

    private void OpenViewHandler()
    {
        UpdateView();
    }

    private int directId = -1;
    private void UpdateView()
    {
        if (SkillUIViewManager.Instance != null && MogoWorld.inCity)
        {
            SkillUILogicManager.Instance.SetSkills(skillMapping, directId);
            directId = -1;
            SkillUIViewManager.Instance.SetSkillUIGold((theOwner as EntityMyself).gold + "");
        }
        if (MainUIViewManager.Instance == null)
        {
            return;
        }
        if (skillMapping.spellOne > 0)
        {
            MainUIViewManager.Instance.SetOutputImage(IconData.dataMap[SkillData.dataMap[skillMapping.spellOne].icon].path);
        }
        else
        {
            MainUIViewManager.Instance.SetOutputImage("lyfw_weikaiqi");
        }
        if (skillMapping.spellTwo > 0)
        {
            MainUIViewManager.Instance.SetAffectImage(IconData.dataMap[SkillData.dataMap[skillMapping.spellTwo].icon].path);
        }
        else
        {
            MainUIViewManager.Instance.SetAffectImage("lyfw_weikaiqi");
        }
        if (skillMapping.spellThree > 0)
        {
            MainUIViewManager.Instance.SetMoveImage(IconData.dataMap[SkillData.dataMap[skillMapping.spellThree].icon].path);
        }
        else
        {
            MainUIViewManager.Instance.SetMoveImage("lyfw_weikaiqi");
        }
        if (skillMapping.powerupAttack > 0)
        {
            MainUIViewManager.Instance.SetSpeicalImage(IconData.dataMap[SkillData.dataMap[skillMapping.powerupAttack].icon].path);
        }
        else
        {
            MainUIViewManager.Instance.SetSpeicalImage("lyfw_weikaiqi");
        }
        MainUIViewManager.Instance.SetNormalAttackIconByID(MogoWorld.thePlayer.GetEquipSubType());
        MainUIViewManager.Instance.EnableNormalAttackCD(ChargeAble());
    }

    public void OpenSkillUI(int id, Action callback = null)
    {
        directId = id;
        MogoUIManager.Instance.SwitchSkillUI(callback);
    }

    public int HadStudySkill()
    {//返回-1表示无
        int vocation = (int)theOwner.vocation;
        int weaponType = (theOwner as EntityMyself).GetEquipSubType();
        SkillData s = null;
        int rst = -1;
        List<int> ids = new List<int>();
        ids.Add(skillMapping.normalAttack);
        ids.Add(skillMapping.spellOne);
        ids.Add(skillMapping.spellTwo);
        ids.Add(skillMapping.spellThree);
        ids.Add(skillMapping.powerupAttack);
        for (int idx = 0; idx < 5; idx++)
        {
            if (ids[idx] == 0)
            {
                s = SkillData.GetSkillByVWLP(vocation, weaponType, idx, 1);
                if (s == null)
                {
                    continue;
                }
                if (skillMapping.hadStudy(s.id))
                {
                    continue;
                }
                if ((theOwner as EntityMyself).level >= s.limitLevel && (theOwner as EntityMyself).gold >= s.moneyCost)
                {
                    rst = s.id;
                    break;
                }
            }
            else
            {
                SkillData cur = SkillData.dataMap[ids[idx]];
                s = SkillData.GetSkillByVWLP(vocation, weaponType, idx, cur.level + 1);
                if (s == null)
                {
                    continue;
                }
                if (skillMapping.hadStudy(s.id))
                {
                    continue;
                }
                if ((theOwner as EntityMyself).level >= s.limitLevel && (theOwner as EntityMyself).gold >= s.moneyCost)
                {
                    rst = s.id;
                    break;
                }
            }
            if (rst != -1)
            {//找到
                break;
            }
        }
        return rst;
    }

    private float preStudy = 0;
    private void StudyHandler(int id, int nextId)
    {
        float cur = Time.realtimeSinceStartup;
        if ((cur - preStudy) < 0.3f)
        {
            return;
        }
        preStudy = cur;
        if (!SkillData.dataMap.ContainsKey(nextId) ||
            skillMapping.hadStudy(nextId))
        {
            LoggerHelper.Error("has study");
            return;
        }
        SkillData s = SkillData.dataMap[nextId];
        if (s.limitLevel > theOwner.level)
        {
            string msg = LanguageData.dataMap.Get(456).content;
            MogoMsgBox.Instance.ShowFloatingText(msg);
        }
        if (s.moneyCost > (theOwner as EntityMyself).gold)
        {
            string msg = LanguageData.dataMap.Get(454).content;
            MogoMsgBox.Instance.ShowFloatingText(msg);
        }
        if (s.limitLevel > theOwner.level || s.moneyCost > (theOwner as EntityMyself).gold)
        {
            return;
        }
        theOwner.RpcCall("SkillUpgradeReq", id, nextId);
        SkillUIViewManager.Instance.ShowSkillOpenAnim(true, s.posi);
    }

    // 初始化 技能数据
    public void InitData()
    {
        // 初始化 技能冷却时间： 技能cd， 连续技cd，连续技最大有效时间，组技能cd, 
        foreach (KeyValuePair<int, SkillData> pair in SkillData.dataMap)
        {
            if (pair.Value.cd.Count == 4)
            {
                // 技能 cd
                skillCoolTime[pair.Key] = pair.Value.cd[0];
                if (pair.Value.cd[2] > 0)
                {
                    // 记录连续技最大有效时间
                    comboSkillPeriod[pair.Key] = pair.Value.cd[2];
                }
                comboSkillPeriodStart[pair.Key] = pair.Value.cd[1];
                commonCD[pair.Key] = pair.Value.cd[3];
            }
            else
            {
                LoggerHelper.Error("format error: spell cool time:" + pair.Key);
            }
        }

        // 依赖技能： 前置技能， 前置次数， 后置技能
        foreach (KeyValuePair<int, SkillData> pair in SkillData.dataMap)
        {
            if (pair.Value.dependSkill != null && pair.Value.dependSkill.Count == 3)
            {
                dependenceSkill[pair.Key] = pair.Value.dependSkill;
            }
            else
            {
                LoggerHelper.Error("format error: spell dependence:" + pair.Key);
            }
        }
        lastSkillID = skillMapping.normalAttack;
    }

    //从后端更新技能列表
    public void UpdateSkillData(object ids)
    {
        LuaTable idTable = (LuaTable)ids;
        List<int> skills = new List<int>();
        foreach (var item in idTable)
        {
            skills.Add(Int32.Parse(item.Key));
        }
        skillMapping.clean();
        for (int i = 0; i < skills.Count; i++)
        {
            SkillData s = null;
            SkillData.dataMap.TryGetValue(skills[i], out s);
            if (s == null)
            {
                LoggerHelper.Error("skill not exist " + skills[i]);
                continue;
            }
            int weaponType = MogoWorld.thePlayer.GetEquipSubType();
            switch (s.posi)
            {
                case 0:
                    {//普攻
                        if (s.weapon == weaponType && s.dependSkill[0] == 0 && s.dependSkill[1] == 0)
                        {
                            skillMapping.normalAttack = s.id;
                        }
                        break;
                    }
                case 1:
                    {//spellOne键对应技能
                        if (s.weapon == weaponType)
                        {
                            skillMapping.spellOne = s.id;
                        }
                        break;
                    }
                case 2:
                    {//spellTwo键对应技能
                        if (s.weapon == weaponType)
                        {
                            skillMapping.spellTwo = s.id;
                        }
                        break;
                    }
                case 3:
                    {//spellThree对应技能
                        if (s.weapon == weaponType)
                        {
                            skillMapping.spellThree = s.id;
                        }
                        break;
                    }
                case 4:
                    {//蓄力技能
                        if (s.weapon == weaponType && s.dependSkill[0] == 0 && s.dependSkill[1] == 0)
                        {
                            skillMapping.powerupAttack = s.id;
                            skillMapping.powers.Add(s.id);
                        }
                        if (s.weapon == weaponType)
                        {
                            skillMapping.powers.Add(s.id);
                            if (s.id > skillMapping.maxPowerupAttack)
                            {
                                skillMapping.maxPowerupAttack = s.id;
                            }
                        }
                        break;
                    }
            }
        }
        skillMapping.powers.Sort();
        UpdateView();
    }

    public SkillMapping GetSkillMapping(int weaponType)
    {
        SkillMapping skillMap = new SkillMapping();
        LuaTable idTable = (LuaTable)(theOwner.GetObjectAttr("skillBag"));
        List<int> skills = new List<int>();
        foreach (var item in idTable)
        {
            skills.Add(Int32.Parse(item.Key));
        }
        for (int i = 0; i < skills.Count; i++)
        {
            SkillData s = null;
            SkillData.dataMap.TryGetValue(skills[i], out s);
            if (s == null)
            {
                LoggerHelper.Error("skill not exist " + skills[i]);
                continue;
            }
            switch (s.posi)
            {
                case 0:
                    {//普攻
                        if (s.weapon == weaponType && s.dependSkill[0] == 0 && s.dependSkill[1] == 0)
                        {
                            skillMap.normalAttack = s.id;
                        }
                        break;
                    }
                case 1:
                    {//spellOne键对应技能
                        if (s.weapon == weaponType)
                        {
                            skillMap.spellOne = s.id;
                        }
                        break;
                    }
                case 2:
                    {//spellTwo键对应技能
                        if (s.weapon == weaponType)
                        {
                            skillMap.spellTwo = s.id;
                        }
                        break;
                    }
                case 3:
                    {//spellThree对应技能
                        if (s.weapon == weaponType)
                        {
                            skillMap.spellThree = s.id;
                        }
                        break;
                    }
                case 4:
                    {//蓄力技能
                        if (s.weapon == weaponType && s.dependSkill[0] == 0 && s.dependSkill[1] == 0)
                        {
                            skillMap.powerupAttack = s.id;
                            skillMap.powers.Add(s.id);
                        }
                        if (s.weapon == weaponType)
                        {
                            skillMap.powers.Add(s.id);
                            if (s.id > skillMap.maxPowerupAttack)
                            {
                                skillMap.maxPowerupAttack = s.id;
                            }
                        }
                        break;
                    }
            }
        }
        skillMap.powers.Sort();
        return skillMap;
    }

    public bool IsCommonCooldown()
    {
        int attackInterval = (int)((Time.realtimeSinceStartup - lastAttackTime) * 1000);

        if (attackInterval < CommonCD)
        {
            LoggerHelper.Debug("common cool down time");
            return true;
        }
        return false;
    }

    public bool IsSkillCooldown(int skillID)
    {
        if (!SkillData.dataMap.ContainsKey(skillID))
        {
            return true;
        }
        if (!this.skilllastCastTime.ContainsKey(skillID))
        {
            skilllastCastTime[skillID] = 0;
        }
        int skillInterval = (int)((Time.realtimeSinceStartup - this.skilllastCastTime[skillID]) * 1000);
        if (!this.skillCoolTime.ContainsKey(skillID))
        {
            skillCoolTime[skillID] = 0;
        }
        if (skillInterval < this.skillCoolTime[skillID])
        {
            LoggerHelper.Debug("skill cool down time");
            return true;
        }
        return false;

    }

    public void ResetCoolTime(int skillID)
    {
        CommonCD = commonCD[skillID];
        lastAttackTime = Time.realtimeSinceStartup;
        skilllastCastTime[skillID] = lastAttackTime;
    }

    public void Compensation(float t)
    {
        lastAttackTime += t;
        List<int> key = new List<int>();
        foreach (var item in skilllastCastTime)
        {
            key.Add(item.Key);
        }
        for (int i = 0; i < key.Count; i++)
        {
            skilllastCastTime[key[i]] += t;
        }
    }

    public void OnChangeEquip(int typeId, int subtypeId)
    {
        if (typeId != (int)(EquipType.Weapon))
        {
            return;
        }
        (theOwner as EntityMyself).UpdateSkillToManager();
    }

    public void ClearComboSkill()
    {
        lastSkillID = 0;
        lastPowerSkillID = 0;
    }

    public int GetCommonCD(int id)
    {
        if (!commonCD.ContainsKey(id))
        {
            return 0;
        }
        return commonCD[id];
    }

    public int GetNormalOne()
    {
        return skillMapping.normalAttack;
    }

    // 获取普通攻击，技能id
    public int GetNormalAttackID()
    {
        //if (lastPowerSkillID > 0)
        //{//蓄力技能连续技能状态

        //    int id = GetPowerAttackID();
        //    if (id != skillMapping.powerupAttack && skillMapping.powers.IndexOf(id) != -1)
        //    {
        //        lastSkillID = 0;
        //        return id;
        //    }
        //    lastPowerSkillID = 0;
        //}
        if (isAnger)
        {//怒气
            return GetSuperAttackID();
        }
        int interval = (int)((Time.realtimeSinceStartup - lastAttackTime) * 1000);

        if (dependenceSkill.ContainsKey(lastSkillID) && this.comboSkillPeriod.ContainsKey(lastSkillID))
        {
            int nextSkill = dependenceSkill[lastSkillID][NEXTSKILL];
            int cd = comboSkillPeriodStart[lastSkillID];
            if (commonCD[lastSkillID] > cd)
            {
                cd = commonCD[lastSkillID];
            }
            if (nextSkill > 0 && interval > cd && interval < this.comboSkillPeriod[lastSkillID])
            {
                lastSkillID = nextSkill;
                return nextSkill;
            }
        }
        lastSkillID = skillMapping.normalAttack;
        return skillMapping.normalAttack;
    }

    public bool HasDependence(int skillId)
    {
        if (dependenceSkill.ContainsKey(skillId) && dependenceSkill[skillId][NEXTSKILL] > 0)
        {
            return true;
        }
        return false;
    }

    public int GetSuperAttackID()
    {
        int interval = (int)((Time.realtimeSinceStartup - lastAttackTime) * 1000);

        if (dependenceSkill.ContainsKey(lastSkillID) && this.comboSkillPeriod.ContainsKey(lastSkillID))
        {
            int nextSkill = dependenceSkill[lastSkillID][NEXTSKILL];
            int cd = comboSkillPeriodStart[lastSkillID];
            if (commonCD[lastSkillID] > cd)
            {
                cd = commonCD[lastSkillID];
            }
            if (nextSkill > 0 && interval > cd && interval < this.comboSkillPeriod[lastSkillID])
            {
                lastSkillID = nextSkill;
                return nextSkill;
            }
        }
        lastSkillID = skillMapping.powerupAttack;
        return skillMapping.powerupAttack;
    }

    public int GetPowerAttackID()
    {
        int interval = (int)((Time.realtimeSinceStartup - lastAttackTime) * 1000);

        if (dependenceSkill.ContainsKey(lastPowerSkillID) && this.comboSkillPeriod.ContainsKey(lastPowerSkillID))
        {
            int nextSkill = dependenceSkill[lastPowerSkillID][NEXTSKILL];
            int cd = comboSkillPeriodStart[lastPowerSkillID];
            if (nextSkill > 0 && interval > cd && interval < this.comboSkillPeriod[lastPowerSkillID])
            {
                lastPowerSkillID = nextSkill;
                return nextSkill;
            }
        }
        lastSkillID = 0;
        lastPowerSkillID = skillMapping.powerupAttack;
        return skillMapping.powerupAttack;
    }

    public int GetSpellOneID()
    {
        return skillMapping.spellOne;
    }

    public int GetSpellTwoID()
    {
        return skillMapping.spellTwo;
    }

    public int GetSpellThreeID()
    {
        return skillMapping.spellThree;
    }

    public bool ChargeAble()
    {
        return skillMapping.powerupAttack > 0;
    }

    #region Combo Part

    protected static readonly uint resetComboTime = 5000;
    protected bool hasResetCombo = false;
    protected uint resetComboTimerID = uint.MaxValue;

    protected int comboNumber = 0;
    protected int maxCombo = 0;

    protected void AddCombo(int num)
    {
        if (num > 0)
        {
            if (!hasResetCombo)
            {
                TimerHeap.DelTimer(resetComboTimerID);
            }
            else
            {
                hasResetCombo = false;
            }

            comboNumber += num;
            if (comboNumber > maxCombo)
            {
                maxCombo = comboNumber;
                // EventDispatcher.TriggerEvent(Events.InstanceEvent.UploadMaxCombo, maxCombo);
            }

            resetComboTimerID = TimerHeap.AddTimer(resetComboTime, 0, ResetCombo);
            MainUIViewManager.Instance.SetComboAttackNum(comboNumber);
        }
    }

    protected void ResetCombo()
    {
        hasResetCombo = true;
        comboNumber = 0;
        MainUIViewManager.Instance.SetComboAttackNum(comboNumber);
    }

    protected void ForceResetCombo()
    {
        TimerHeap.DelTimer(resetComboTimerID);
        ResetCombo();
    }

    public int GetMaxCombo()
    {
        return maxCombo;
    }

    public void ResetMaxCombo()
    {
        maxCombo = 0;
    }

    #endregion
}
