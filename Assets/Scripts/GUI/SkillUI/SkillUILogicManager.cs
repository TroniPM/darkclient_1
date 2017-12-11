using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.GameData;

public class SkillUILogicManager
{
    
    private static SkillUILogicManager m_instance;

    private SkillMapping skills;
    private int currSkillTree = 0;
    private Dictionary<int, List<int>> vocationCfg;
    private bool clickedWeapon = false;
    private int weaponPos = 0;

    public static SkillUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SkillUILogicManager();
            }

            return SkillUILogicManager.m_instance;

        }
    }

    private SkillUILogicManager()
    {
        vocationCfg = new Dictionary<int, List<int>>();
        List<int> l = new List<int>();
        l.Add(1);
        l.Add(2);
        l.Add(360);//中文名
        l.Add(361);
        vocationCfg.Add(1, l); //战士
        l = new List<int>();
        l.Add(3);
        l.Add(4);
        l.Add(362);//中文
        l.Add(363);
        vocationCfg.Add(2, l); //刺客
        l = new List<int>();
        l.Add(7);
        l.Add(8);
        l.Add(366);//中文
        l.Add(367);
        vocationCfg.Add(3, l); //射手
        l = new List<int>();
        l.Add(5);
        l.Add(6);
        l.Add(364);//中文
        l.Add(365);
        vocationCfg.Add(4, l); //法师
    }

    void OnSwitchIconUp(int i)
    {
        Mogo.Util.LoggerHelper.Error("SwitchIcon");
    }

    void OnWeaponUp(int i)
    {
        clickedWeapon = true;
        weaponPos = i;
        List<int> l = vocationCfg[(int)MogoWorld.thePlayer.vocation];
        skills = (MogoWorld.thePlayer.skillManager as PlayerSkillManager).GetSkillMapping(l[i]);
        UpdateView();
    }

    void OnLearnBtnUp(int i)
    {
        List<int> skillList = new List<int>();
        skillList.Add(skills.normalAttack);
        skillList.Add(skills.spellOne);
        skillList.Add(skills.spellTwo);
        skillList.Add(skills.spellThree);
        skillList.Add(skills.maxPowerupAttack);

        int id = skillList[currSkillTree];
        int vocation = (int)MogoWorld.thePlayer.vocation;
        int weaponType = MogoWorld.thePlayer.GetEquipSubType();
        SkillData s = null;
        //s = SkillData.GetSkillByVWLP(vocation, weaponType, currSkillTree + 1, currSkill + 1);
        SkillData.dataMap.TryGetValue(id, out s);
        //if (s != null && skills.hadStudy(s.id))
        //{
        //    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(200004));
        //    return;
        //}
        if (id <= 0)
        {
            s = SkillData.GetSkillByVWLP(vocation, weaponType, currSkillTree, 1);
        }
        else
        {
            s = SkillData.GetSkillByVWLP(vocation, weaponType, currSkillTree, s.level + 1);
        }
        if (s == null)
        {
            LoggerHelper.Error("next skill null " + vocation + "  " + weaponType + "   " + currSkillTree);
            return;
        }
        int nextId = s.id;
        EventDispatcher.TriggerEvent<int, int>(Events.SpellEvent.Study, id, nextId);
    }

    void OnSkillIconGridUp(int id)
    {
        currSkillTree = id;
        UpdateView();
        //EventDispatcher.TriggerEvent(Events.SpellEvent.SelectGroup, id);
    }

    void OnSkillInfoIconUp(int id)
    {
        UpdateView();
        //EventDispatcher.TriggerEvent(Events.SpellEvent.SelectLevel, id);
    }

    private void UpdateView()
    {
        int vocation = (int)MogoWorld.thePlayer.vocation;
        int weaponType = MogoWorld.thePlayer.GetEquipSubType();
        List<int> l = vocationCfg[vocation];
        if (clickedWeapon)
        {
            weaponType = l[weaponPos];
            skills = (MogoWorld.thePlayer.skillManager as PlayerSkillManager).GetSkillMapping(l[weaponPos]);
        }
        SkillUIViewManager.Instance.SetWeapon0PageText(LanguageData.GetContent(l[2]));
        SkillUIViewManager.Instance.SetWeapon1PageText(LanguageData.GetContent(l[3]));
        List<int> skillList = new List<int>();
        skillList.Add(skills.normalAttack);
        skillList.Add(skills.spellOne);
        skillList.Add(skills.spellTwo);
        skillList.Add(skills.spellThree);
        skillList.Add(skills.maxPowerupAttack);
        UpdateWeaponPage(weaponType);
        for (int i = 0; i < skillList.Count; i++)
        {
            UpdateSkillTree(skillList, i, vocation, weaponType);
        }
        UpdateSkillInfo();
    }

    private void UpdateWeaponPage(int wt)
    {
        if (clickedWeapon)
        {
            SkillUIViewManager.Instance.SetWeaponPageDown(weaponPos);
            return;
        }
        if (wt % 2 == 0)
        {
            SkillUIViewManager.Instance.SetWeaponPageDown(1);
        }
        else
        {
            SkillUIViewManager.Instance.SetWeaponPageDown(0);
        }
    }

    private void UpdateSkillTree(List<int> skillList, int idx, int vocation, int weaponType)
    {
        SkillData s = null;
        if (skillList[idx] > 0)
        {
            s = SkillData.dataMap[skillList[idx]];
            SkillUIViewManager.Instance.SetSkillGridIcon(IconData.dataMap[s.icon].path, idx);
            SkillUIViewManager.Instance.SetSkillGridActive(true, idx);
        }
        else
        {
            s = SkillData.GetSkillByVWLP(vocation, weaponType, idx, 1);
            if (s == null)
            {
                LoggerHelper.Debug("skill " + vocation + " " + weaponType + " " + idx + " " + 1);
                return;
            }
            if (s.icon != null && IconData.dataMap.ContainsKey(s.icon))
            {
                SkillUIViewManager.Instance.SetSkillGridIcon(IconData.dataMap[s.icon].path, idx);
            }
            if (s.limitLevel <= MogoWorld.thePlayer.level)
            {
                SkillUIViewManager.Instance.SetSkillGridActive(true, idx);
            }
            else
            {
                SkillUIViewManager.Instance.SetSkillGridActive(false, idx);
            }
        }
        SkillData next = SkillData.GetSkillByVWLP(vocation, weaponType, s.posi, s.level + 1);
        if (next == null)
        {
            SkillUIViewManager.Instance.ShowSkillCanLearnAnim(false, idx);
            return;
        }
        SkillUIViewManager.Instance.ShowSkillCanLearnAnim(false, idx);
        if (next.limitLevel <= MogoWorld.thePlayer.level && next.moneyCost <= MogoWorld.thePlayer.gold)
        {
            SkillUIViewManager.Instance.ShowSkillCanLearnAnim(true, idx);
        }
    }

    private void UpdateSkillInfo()
    {
        int idx = currSkillTree;
        List<int> skillList = new List<int>();
        skillList.Add(skills.normalAttack);
        skillList.Add(skills.spellOne);
        skillList.Add(skills.spellTwo);
        skillList.Add(skills.spellThree);
        skillList.Add(skills.maxPowerupAttack);
        int vocation = (int)MogoWorld.thePlayer.vocation;
        int weaponType = MogoWorld.thePlayer.GetEquipSubType();
        List<int> l = vocationCfg[vocation];
        if (clickedWeapon)
        {
            weaponType = l[weaponPos];
        }
        SkillData s = SkillData.dataMap.GetValueOrDefault(skillList[idx], null);
        if (s == null)
        {
            if (skillList[idx] != 0)
            {
                return;
            }
            s = SkillData.GetSkillByVWLP(vocation, weaponType, idx, 1);
            if (s == null)
            {
                return;
            }
        }
        SkillData next = SkillData.GetSkillByVWLP(vocation, weaponType, s.posi, s.level + 1);
        SkillUIViewManager.Instance.SetLearnBtnText(LanguageData.GetContent(14602));
        if (skillList[idx] == 0)
        {
            SkillUIViewManager.Instance.SetLearnBtnText(LanguageData.GetContent(14601));
            next = s;
        }
        string name = LanguageData.dataMap[s.name].content;
        string desc;
        int gameMoney;
        //int horner;
        int limitLevel;
        if (next == null)
        {
            desc = LanguageData.GetContent(s.desc);
            gameMoney = s.moneyCost;
            //horner = s.pvpCreditCost;
            limitLevel = s.limitLevel;
            SkillUIViewManager.Instance.SetLearnBtnEnable(false);
        }
        else
        {
            desc = LanguageData.GetContent(s.desc);
            gameMoney = next.moneyCost;
            //horner = next.pvpCreditCost;
            limitLevel = next.limitLevel;
            SkillUIViewManager.Instance.SetLearnBtnEnable(true);
        }
        if (skills.hadStudy(s.id))
        {
            //SkillUIViewManager.Instance.SetSkillName(name + " (" + LanguageData.GetContent(200003) + ")");
            SkillUIViewManager.Instance.SetSkillName(name + " [ffffff]" + LanguageData.GetContent(168, s.level) + "[-]");
        }
        else
        {
            SkillUIViewManager.Instance.SetSkillName(name);
        }
        SkillUIViewManager.Instance.SetSkillDamageRace(LanguageData.GetContent(14603), s.extraRate * 100 + "%");
        SkillUIViewManager.Instance.SetSkillExtraDamage(LanguageData.GetContent(14604), s.extraHarm.ToString());
        SkillUIViewManager.Instance.SetSkillDescripe(desc);
        SkillUIViewManager.Instance.SetSkillLearnCostGold(gameMoney);
        //SkillUIViewManager.Instance.SetSkillLearnCostHorner(horner);       
        SkillUIViewManager.Instance.SetSkillNeedLevel(limitLevel);        
    }

    public void SetSkills(SkillMapping skills, int directId)
    {
        this.skills = skills;
        if (directId == -1)
        {
            UpdateView();
        }
        else
        {
            SkillData s = SkillData.dataMap[directId];
            OnSkillIconGridUp(s.posi);
           //OnSkillInfoIconUp(s.level - 1);
            TimerHeap.AddTimer<int>(500, 0, SkillUIViewManager.Instance.SetCurrentSkillGridIcon, s.posi);
        }
    }

    public void Initialize()
    {
        SkillUIViewManager.Instance.LEARNBTNUP += OnLearnBtnUp;
        SkillUIViewManager.Instance.SWITCHICONUP += OnSwitchIconUp;
        SkillUIViewManager.Instance.SKILLICONGRIDUP += OnSkillIconGridUp;
        SkillUIViewManager.Instance.SKILLINFOICONUP += OnSkillInfoIconUp;
        SkillUIViewManager.Instance.WEAPONUP += OnWeaponUp;
        clickedWeapon = false;
        weaponPos = 0;
    }

    public void Release()
    {
        SkillUIViewManager.Instance.LEARNBTNUP -= OnLearnBtnUp;
        SkillUIViewManager.Instance.SWITCHICONUP -= OnSwitchIconUp;
        SkillUIViewManager.Instance.SKILLICONGRIDUP -= OnSkillIconGridUp;
        SkillUIViewManager.Instance.SKILLINFOICONUP -= OnSkillInfoIconUp;
        SkillUIViewManager.Instance.WEAPONUP -= OnWeaponUp;
        clickedWeapon = false;
        weaponPos = 0;
    }
}
