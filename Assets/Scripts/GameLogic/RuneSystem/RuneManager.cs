/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：RuneManager
// 创建者：Hooke HU
// 修改者列表：
// 创建日期：2013-4-2
// 模块描述：符文管理器
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;

//符文数据类
class Rune
{
    public string uuid;
    public int resID;
    public int currExp;
    public bool inBag = true; //为true时在符文包中，为false时有身上
    public int index; //符文包中位置或身上位置

    private RuneData resData;

    public RuneData ResData
    {
        get { return resData; }
    }

    public Rune(string _uuid, int _resID, int _exp)
    {
        uuid = _uuid;
        resID = _resID;
        currExp = _exp;
        resData = RuneData.dataMap[resID];
    }
}

class BodyRune
{
    public int posi;
    public bool locked = true;
    public Rune rune = null;
}

class RuneManager
{
    static public RuneManager Instance;

    private int BAG_WISH_LEN = 16;
    private int BODY_LEN = 11;
    // 为去除警告暂时屏蔽以下代码
    //private int BAG_LEN = 16;
    private int RUNE_PROPERTY = 1;
    private int RUNE_MONEY = 2;
    private int RUNE_EXP = 3;
    
    private EntityMyself m_myself;
    private int gameMoney = 0; //刷新需要的游戏币
    private int RMB = 0; //刷新需要的人民币

    private Dictionary<int, Rune> bag = new Dictionary<int, Rune>(); //符文背包 32格子，0-15是许愿背包，16-31是符文背包
    private List<BodyRune> body = new List<BodyRune>(); //身上符文位置

    private int wishCnt = 0;
    private int pickCnt = 0;

    public RuneManager(EntityMyself _myself)
    {
        Instance = this;
        m_myself = _myself;
        //初始化6个符文位
        for (int i = 0; i < BODY_LEN; i++)
        {
            BodyRune brune = new BodyRune();
            brune.posi = i;
            body.Add(brune);
        }
        AddListeners();
    }

    private void AddListeners()
    {
        EventDispatcher.AddEventListener(Events.RuneEvent.GetRuneBag, GetRuneBag);
        EventDispatcher.AddEventListener(Events.RuneEvent.GetBodyRunes, GetBodyRunes);
        EventDispatcher.AddEventListener(Events.RuneEvent.GameMoneyRefresh, GameMoneyRefresh);
        EventDispatcher.AddEventListener(Events.RuneEvent.FullRefresh, FullRefresh);
        EventDispatcher.AddEventListener(Events.RuneEvent.RMBRefresh, RMBRefresh);
        EventDispatcher.AddEventListener<bool>(Events.RuneEvent.AutoCombine, AutoCombine);
        EventDispatcher.AddEventListener(Events.RuneEvent.AutoPickUp, AutoPickUp);
        EventDispatcher.AddEventListener<int, bool>(Events.RuneEvent.UseRune, UseRune);
        EventDispatcher.AddEventListener<int, int>(Events.RuneEvent.PutOn, PutOn);
        EventDispatcher.AddEventListener<int, int>(Events.RuneEvent.PutDown, PutDown);
        EventDispatcher.AddEventListener<int, int, bool>(Events.RuneEvent.ChangeIndex, ChangeIndex);
        EventDispatcher.AddEventListener<int, int>(Events.RuneEvent.ChangePosi, ChangePosi);
        EventDispatcher.AddEventListener<int, int, bool, bool>(Events.RuneEvent.ShowTips, ShowTips);
        EventDispatcher.AddEventListener(Events.RuneEvent.CloseDragon, CloseDragon);
    }

    private void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(Events.RuneEvent.GetRuneBag, GetRuneBag);
        EventDispatcher.RemoveEventListener(Events.RuneEvent.GetBodyRunes, GetBodyRunes);
        EventDispatcher.RemoveEventListener(Events.RuneEvent.GameMoneyRefresh, GameMoneyRefresh);
        EventDispatcher.RemoveEventListener(Events.RuneEvent.FullRefresh, FullRefresh);
        EventDispatcher.RemoveEventListener(Events.RuneEvent.RMBRefresh, RMBRefresh);
        EventDispatcher.RemoveEventListener<bool>(Events.RuneEvent.AutoCombine, AutoCombine);
        EventDispatcher.RemoveEventListener(Events.RuneEvent.AutoPickUp, AutoPickUp);
        EventDispatcher.RemoveEventListener<int, bool>(Events.RuneEvent.UseRune, UseRune);
        EventDispatcher.RemoveEventListener<int, int>(Events.RuneEvent.PutOn, PutOn);
        EventDispatcher.RemoveEventListener<int, int>(Events.RuneEvent.PutDown, PutDown);
        EventDispatcher.RemoveEventListener<int, int, bool>(Events.RuneEvent.ChangeIndex, ChangeIndex);
        EventDispatcher.RemoveEventListener<int, int>(Events.RuneEvent.ChangePosi, ChangePosi);
        EventDispatcher.RemoveEventListener<int, int, bool, bool>(Events.RuneEvent.ShowTips, ShowTips);
        EventDispatcher.RemoveEventListener(Events.RuneEvent.CloseDragon, CloseDragon);
    }

    public void Clean()
    {
        RemoveListeners();
    }

    private void CloseDragon()
    {
        wishCnt = 0;
        pickCnt = 0;
    }

    private bool hasWishSpace()
    {
        //0-15是许愿背包
        for (int i = 0; i < 16; i++)
        {
            if (!bag.ContainsKey(i))
            {
                return true;
            }
        }
        return false;
    }

    private bool hasRuneSpace()
    {
        //16-31是符文背包
        for (int i = 16; i < 32; i++)
        {
            if (!bag.ContainsKey(i))
            {
                return true;
            }
        }
        return false;
    }

    private bool hasPosiSpace()
    {
        for (int i = 0; i < BODY_LEN; i++)
        {
            if (body[i].rune == null && !body[i].locked)
            {
                return true;
            }
        }
        return false;
    }

    private bool isEmpty(bool wish)
    {
        int s = 0;
        if (!wish)
        {
            s = 16;
        }
        for (int i = s; i < 16 + s; i++)
        {
            if (bag.ContainsKey(i))
            {
                return false;
            }
        }
        return true;
    }

    private bool EatAble(Rune n, Rune o)
    {
        if (n.ResData.type == RUNE_EXP && o.ResData.type != RUNE_EXP)
        {
            return false;
        }
        if (o.ResData.type == RUNE_EXP && n.ResData.type != RUNE_EXP)
        {
            return true;
        }
        if (n.ResData.quality < o.ResData.quality)
        {
            return false;
        }
        if (n.ResData.quality > o.ResData.quality)
        {
            return true;
        }
        if (n.currExp >= o.currExp)
        {
            return true;
        }
        return false;
    }

    private void UpdatePosiLock()
    {
        int lv = m_myself.level;
        int num = 0;
        foreach (var item in RunePosiLock.dataMap.Values)
        {
            if (item.level > lv)
            {
                //RuneUIViewManager.Instance.SetRuneInsetGridLevel(item.id - 1, String.Format("{0}级", item.level));
                if (RuneUIViewManager.Instance != null)
                {
                    RuneUIViewManager.Instance.SetRuneInsetGridLevel(item.id - 1, LanguageData.GetContent(200005, item.level));
                }
                continue;
            }
            if (RuneUIViewManager.Instance != null)
            {
                RuneUIViewManager.Instance.SetRuneInsetGridLevel(item.id - 1, String.Empty);
            }
            if (item.id > num)
            {
                num = item.id;
            }
        }
        for (int i = 0; i < num; i++)
        {
            body[i].locked = false;
            if (RuneUIViewManager.Instance != null)
            {
                RuneUIViewManager.Instance.UnLockInsetGrid(i);
                if (num >= 6)
                {
                    RuneUIViewManager.Instance.ShowAsExtraGrid();
                }
                else
                {
                    RuneUIViewManager.Instance.ShowAsPlayerModel();
                }
            }
        }
    }

    #region 前端UI操作触发
    private void ShowTips(int idx, int gridId, bool fromDragon, bool inBag)
    {
        string name = "no name";
        RuneData rd;
        if (fromDragon)
        {
            var r = this.bag[idx];
            if (LanguageData.dataMap.ContainsKey(r.ResData.name))
            {
                name = LanguageData.dataMap[r.ResData.name].content;
                QualityColorData colorData = QualityColorData.dataMap[r.ResData.quality];
                name = String.Format("[{0}]{1}[-]", colorData.color, name);
            }
            DragonUIViewManager.Instance.SetDragonUIToolTipName(gridId, name);
            DragonUIViewManager.Instance.SetDragonUITooltipExp(gridId, r.currExp + "/" + r.ResData.expNeed);
            DragonUIViewManager.Instance.SetDragonUITooltipLevel(gridId, r.ResData.level + "");
            if (r.ResData.type == RUNE_EXP)
            {
                DragonUIViewManager.Instance.SetDragonUITooltipCurrentDesc(gridId, LanguageData.dataMap[r.ResData.effectDesc].Format(r.ResData.expValue));
            }
            else
            {
                DragonUIViewManager.Instance.SetDragonUITooltipCurrentDesc(gridId, LanguageData.dataMap[r.ResData.effectDesc].Format(GetEffectNum(r.ResData.effectID)));
            }
            rd = RuneData.GetNextLvRune(r.ResData.type, r.ResData.subtype, r.ResData.quality, r.ResData.level + 1);
            if (rd == null)
            {
                DragonUIViewManager.Instance.SetDragonUITooltipNextDesc(gridId, " ");
            }
            else
            {
                DragonUIViewManager.Instance.SetDragonUITooltipNextDesc(gridId, LanguageData.dataMap[rd.effectDesc].Format(GetEffectNum(rd.effectID)));
            }
            return;
        }
        if (inBag)
        {
            var r = this.bag[idx + 16];
            if (LanguageData.dataMap.ContainsKey(r.ResData.name))
            {
                name = LanguageData.dataMap[r.ResData.name].content;
                QualityColorData colorData = QualityColorData.dataMap[r.ResData.quality];
                name = String.Format("[{0}]{1}[-]", colorData.color, name);
            }
            RuneUIViewManager.Instance.SetDragonUIToolTipName(gridId, name);
            RuneUIViewManager.Instance.SetRuneUITooltipExp(gridId, r.currExp + "/" + r.ResData.expNeed);
            RuneUIViewManager.Instance.SetRuneUITooltipLevel(gridId, r.ResData.level + "");
            if (r.ResData.type == RUNE_EXP)
            {
                RuneUIViewManager.Instance.SetRuneUITooltipCurrentDesc(gridId, LanguageData.dataMap[r.ResData.effectDesc].Format(r.ResData.expValue), true);
            }
            else
            {
                RuneUIViewManager.Instance.SetRuneUITooltipCurrentDesc(gridId, LanguageData.dataMap[r.ResData.effectDesc].Format(GetEffectNum(r.ResData.effectID)), true);
            }
            rd = RuneData.GetNextLvRune(r.ResData.type, r.ResData.subtype, r.ResData.quality, r.ResData.level + 1);
            if (rd == null)
            {
                RuneUIViewManager.Instance.SetRuneUITooltipNextDesc(gridId, " ",true);
            }
            else
            {
                RuneUIViewManager.Instance.SetRuneUITooltipNextDesc(gridId, LanguageData.dataMap[rd.effectDesc].Format(GetEffectNum(rd.effectID)), true);
            }
            return;
        }
        var r1 = this.body[idx].rune;
        if (LanguageData.dataMap.ContainsKey(r1.ResData.name))
        {
            name = LanguageData.dataMap[r1.ResData.name].content;
            QualityColorData colorData = QualityColorData.dataMap[r1.ResData.quality];
            name = String.Format("[{0}]{1}[-]", colorData.color, name);
        }
        RuneUIViewManager.Instance.SetBodyUIToolTipName(gridId, name);
        RuneUIViewManager.Instance.SetBodyUITooltipExp(gridId, r1.currExp + "/" + r1.ResData.expNeed);
        RuneUIViewManager.Instance.SetBodyUITooltipLevel(gridId, r1.ResData.level + "");
        RuneUIViewManager.Instance.SetRuneUITooltipCurrentDesc(gridId, LanguageData.dataMap[r1.ResData.effectDesc].Format(GetEffectNum(r1.ResData.effectID)), false);
        rd = RuneData.GetNextLvRune(r1.ResData.type, r1.ResData.subtype, r1.ResData.quality, r1.ResData.level + 1);
        if (rd == null)
        {
            RuneUIViewManager.Instance.SetRuneUITooltipNextDesc(gridId, " ",false);
        }
        else
        {
            RuneUIViewManager.Instance.SetRuneUITooltipNextDesc(gridId, LanguageData.dataMap[rd.effectDesc].Format(GetEffectNum(rd.effectID)), false);
        }
    }

    private string GetEffectNum(int id)
    {
        string rst = String.Empty;
        PropertyEffectData p = PropertyEffectData.dataMap.Get(id);
        if (p == null)
        {
            return rst;
        }
        if (p.hpBase > 0)
        {
            return p.hpBase + "";
        }
        if (p.attackBase > 0)
        {
            return p.attackBase + "";
        }
        if (p.defenseBase > 0)
        {
            return p.defenseBase + "";
        }
        if (p.hit > 0)
        {
            return p.hit + "";
        }
        if (p.crit > 0)
        {
            return p.crit + "";
        }
        if (p.trueStrike > 0)
        {
            return p.trueStrike + "";
        }
        if (p.critExtraAttack > 0)
        {
            return p.critExtraAttack + "";
        }
        if (p.antiDefense > 0)
        {
            return p.antiDefense + "";
        }
        if (p.pvpAddition > 0)
        {
            return p.pvpAddition + ""; // PVP强度11
        }
        if (p.pvpAnti > 0)
        {
            return p.pvpAnti + ""; // PVP抗性12
        }
        if (p.antiCrit > 0)
        {
            return p.antiCrit + ""; // 抗暴击8
        }
        if (p.antiTrueStrike > 0)
        {
            return p.antiTrueStrike + ""; // 抗破击9
        }       
       
        return "0";
    }

    private void GetRuneBag()
    {
        //todo RPC更新符文背包
        m_myself.RpcCall("GetRuneBagReq");
    }

    private void GetBodyRunes()
    {
        //todo RPC更新符文位置
        m_myself.RpcCall("GetBodyRunesReq");
        UpdatePosiLock();
        CalcuScore();
    }

    private float pre = 0;
    private void GameMoneyRefresh()
    {
        //todo游戏币刷新，前端先判断，再RPC后端
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {//刷新CD
            return;
        }
        pre = t;
        if (gameMoney > MogoWorld.thePlayer.gold)
        {
            string msg = LanguageData.dataMap[306].content;
            MogoMsgBox.Instance.ShowFloatingText(msg);
            return;
        }
        if (!hasWishSpace())
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[300].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        //if (!VipControl())
        //{
        //    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(839, m_myself.VipLevel));
        //    return;
        //}
        DragonUIViewManager.Instance.ShowGoldWishAnim();
        wishCnt++;
        m_myself.RpcCall("GameMoneyRefreshReq");
        //m_myself.m_VipRealStateMap[(int)VipRealStateEnum.DAILY_RUNE_WISH_TIMES] += 1;
    }

    private bool VipControl()
    {
        if (!PrivilegeData.dataMap.ContainsKey(m_myself.VipLevel))
        {
            return false;
        }
        int limit = PrivilegeData.dataMap[m_myself.VipLevel].dailyRuneWishLimit;
        int cur = m_myself.m_VipRealStateMap[(int)VipRealStateEnum.DAILY_RUNE_WISH_TIMES];
        if (cur >= limit)
        {
            return false;
        }
        return true;
    }

    private void FullRefresh()
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo一键刷新，RPC后端
        if (!hasWishSpace())
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[300].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        m_myself.RpcCall("FullRefreshReq");
    }

    private void RMBRefresh()
    {
        //todo人民币刷新，前端先判断，再RPC后端
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {//刷新CD
            return;
        }
        pre = t;
        if (RMB > MogoWorld.thePlayer.diamond)
        {
            string msg = LanguageData.dataMap.Get(307).content;
            MogoMsgBox.Instance.ShowFloatingText(msg);
            return;
        }
        if (!hasWishSpace())
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap.Get(300).content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        if (!VipControl())
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(839, m_myself.VipLevel));
            return;
        }
        DragonUIViewManager.Instance.ShowDiamondWishAnim();
        wishCnt++;
        m_myself.RpcCall("RMBRefreshReq");
        m_myself.m_VipRealStateMap[(int)VipRealStateEnum.DAILY_RUNE_WISH_TIMES] += 1;
    }

    private void AutoCombine(bool fromDragon)
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo一键合成
        Int16 b = 1;
        int expCnt = 0;
        int cnt = 0;
        int hintCnt = 0;
        if (!fromDragon)
        {//从符文界面触发
            for (int i = 16; i < 32; i++)
            {
                if (bag.ContainsKey(i))
                {
                    cnt++;
                    if (bag[i].ResData.type == RUNE_EXP)
                    {
                        expCnt++;
                    }
                    else if(bag[i].ResData.type == RUNE_PROPERTY && bag[i].ResData.quality >= 4)
                    {
                        hintCnt++;
                    }
                }
            }
            b = -1;
        }
        else
        {
            for (int i = 0; i < 16; i++)
            {
                if (bag.ContainsKey(i))
                {
                    cnt++;
                    if (bag[i].ResData.type == RUNE_EXP)
                    {
                        expCnt++;
                    }
                    else if (bag[i].ResData.type == RUNE_PROPERTY && bag[i].ResData.quality >= 4)
                    {
                        hintCnt++;
                    }
                }
            }
        }
        if (isEmpty(fromDragon))
        {
            //LoggerHelper.Debug("背包是空的");
            return;
        }
        if (expCnt == cnt)
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[306].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        if (hintCnt >= 2)
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(310), (rst) =>
            {
                MogoGlobleUIManager.Instance.ConfirmHide();
                if (!rst)
                {
                    return;
                }
                pickCnt = 0;
                m_myself.RpcCall("AutoCombineReq", b);
            });
            return;
        }
        pickCnt = 0;
        m_myself.RpcCall("AutoCombineReq", b);
    }

    private void AutoPickUp()
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo一键拾取
        if (isEmpty(true))
        {
            //LoggerHelper.Debug("背包是空的");
            return;
        }
        if (!hasRuneSpace())
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[301].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        for (int i = 0; i < BAG_WISH_LEN; i++)
        {
            if (bag.ContainsKey(i))
            {
                pickCnt++;
            }
        }
        m_myself.RpcCall("AutoPickUpReq");
    }

    private void UseRune(int idx, bool fromDragon)
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo使用符文，如果是经验，金钱符文，显示使用效果，如果是属性符文，装备身上（要判断是否有空位）
        pickCnt = 0;
        int i = idx;
        if (!fromDragon)
        {//从符文界面触发
            i = i + BAG_WISH_LEN;
        }
        else
        {
            if (!bag.ContainsKey(i))
            {
                return;
            }
            if (bag[i].ResData.type != RUNE_MONEY)
            {
                if (!hasRuneSpace())
                {
                    MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[301].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
                    return;
                }
                pickCnt++;
            }
        }
        if (!bag.ContainsKey(i))
        {
            return;
        }
        if (bag[i].ResData.type == RUNE_EXP && i > 15)
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[302].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        if (bag[i].ResData.type == RUNE_PROPERTY && !hasPosiSpace() && i > 15)
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[303].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        m_myself.RpcCall("UseRuneReq", (byte)i);
    }

    /// <summary>
    /// 穿上符文
    /// </summary>
    /// <param name="idx">背包位置</param>
    /// <param name="posi">目标位置，为-1时系统找一个空位</param>
    private void PutOn(int idx, int posi = -1)
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo穿上，先判断是否有同类型在身上，然后RPC后端
        idx = idx + BAG_WISH_LEN;
        if (this.bag[idx].ResData.type == RUNE_EXP && posi == -1)
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[304].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        if (posi == -1 && !hasPosiSpace())
        {//没有空位
            return;
        }
        if (posi != -1 && body[posi].locked)
        {//目标位置未开启
            return;
        }
         //&& (posi != -1 && body[posi].rune == null)
        if (bag[idx].ResData.type == RUNE_PROPERTY && HasSubTypeOnBody(bag[idx].ResData.subtype))
        {
            if((posi != -1 && body[posi].rune == null) || (posi == -1 && hasPosiSpace()))
            {
                
                MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[305].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
                return;
            }
        }
        if (posi != -1 && body[posi].rune != null && bag[idx].ResData.type != RUNE_MONEY)
        {
            string s = "";
            if (EatAble(body[posi].rune, bag[idx]))
            {
                //s = LanguageData.dataMap[body[posi].rune.ResData.name].content + "将吞噬" + LanguageData.dataMap[bag[idx].ResData.name].content;
                s = LanguageData.GetContent(200006, LanguageData.dataMap[body[posi].rune.ResData.name].content, LanguageData.dataMap[bag[idx].ResData.name].content);
            }
            else
            {
                //s = LanguageData.dataMap[bag[idx].ResData.name].content + "将吞噬" + LanguageData.dataMap[body[posi].rune.ResData.name].content;
                if (HasSubTypeOnBody(bag[idx].ResData.subtype))
                {
                    //MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[305].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(305));
                    return;
                }
                s = LanguageData.GetContent(200006, LanguageData.dataMap[bag[idx].ResData.name].content, LanguageData.dataMap[body[posi].rune.ResData.name].content);
            }
            MogoGlobleUIManager.Instance.Confirm(s,
                    (rst) =>
                    {
                        if (rst)
                        {
                            m_myself.RpcCall("PutOnRuneReq", (byte)idx, (Int16)posi);
                        }
                        MogoGlobleUIManager.Instance.ConfirmHide();
                    });
            return;
        }
        m_myself.RpcCall("PutOnRuneReq", (byte)idx, (Int16)posi);
    }

    private bool HasSubTypeOnBody(int subType)
    {
        foreach (var item in body)
        {
            if (item.rune == null)
            {
                continue;
            }
            if (item.rune.ResData.subtype == subType)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 脱下符文
    /// </summary>
    /// <param name="posi">身上位置</param>
    /// <param name="desIdx">目标位置，为-1时系统找一个空位</param>
    private void PutDown(int posi, int desIdx = -1)
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo脱下，先判断是否有空位，然后RPC后端
        if (desIdx == -1 && !hasRuneSpace())
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap[301].content, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
            return;
        }
        if (desIdx != -1)
        {
            desIdx = desIdx + BAG_WISH_LEN;
        }
        //提示部分，代码要整理
        if (desIdx != -1 && bag.ContainsKey(desIdx) && bag[desIdx].ResData.type != RUNE_MONEY)
        {
            string s = "";
            if (EatAble(body[posi].rune, bag[desIdx]))
            {
                //s = LanguageData.dataMap[body[posi].rune.ResData.name].content + "将吞噬" + LanguageData.dataMap[bag[desIdx].ResData.name].content;
                s = LanguageData.GetContent(200006, LanguageData.dataMap[body[posi].rune.ResData.name].content, LanguageData.dataMap[bag[desIdx].ResData.name].content);
            }
            else
            {
                //s = LanguageData.dataMap[bag[desIdx].ResData.name].content + "将吞噬" + LanguageData.dataMap[body[posi].rune.ResData.name].content;
                s = LanguageData.GetContent(200006, LanguageData.dataMap[bag[desIdx].ResData.name].content, LanguageData.dataMap[body[posi].rune.ResData.name].content);
            }
            MogoGlobleUIManager.Instance.Confirm(s,
                    (rst) =>
                    {
                        if (rst)
                        {
                            m_myself.RpcCall("PutDownRuneReq", (byte)posi, (byte)desIdx);
                        }
                        MogoGlobleUIManager.Instance.ConfirmHide();
                    });
            return;
        }
        m_myself.RpcCall("PutDownRuneReq", (byte)posi, (Int16)desIdx);
    }

    /// <summary>
    /// 更换格子位置
    /// </summary>
    /// <param name="oldIdx">原索引</param>
    /// <param name="newIdx">新索引</param>
    private void ChangeIndex(int oldIdx, int newIdx, bool fromDragon)
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo更换格子,要判断是否为吞噬操作
        int o = oldIdx;
        int n = newIdx;
        if (o == n)
        {
            return;
        }
        pickCnt = 0;
        if (!fromDragon)
        {//从符文界面触发
            o = o + BAG_WISH_LEN;
            n = n + BAG_WISH_LEN;
        }
        //提示部分,要整理的代码，恶心
        if (bag.ContainsKey(o) && bag.ContainsKey(n))
        {
            if (bag[o].ResData.type != RUNE_MONEY && bag[n].ResData.type != RUNE_MONEY &&
                !(bag[o].ResData.type == RUNE_EXP && bag[n].ResData.type == RUNE_EXP))
            {
                string s = "";
                if (EatAble(bag[n], bag[o]))
                {
                    //s = LanguageData.dataMap[bag[n].ResData.name].content + "将吞噬" + LanguageData.dataMap[bag[o].ResData.name].content;
                    s = LanguageData.GetContent(200006, LanguageData.dataMap[bag[n].ResData.name].content, LanguageData.dataMap[bag[o].ResData.name].content);
                }
                else
                {
                    //s = LanguageData.dataMap[bag[o].ResData.name].content + "将吞噬" + LanguageData.dataMap[bag[n].ResData.name].content;
                    s = LanguageData.GetContent(200006, LanguageData.dataMap[bag[o].ResData.name].content, LanguageData.dataMap[bag[n].ResData.name].content);
                }
                MogoGlobleUIManager.Instance.Confirm(s,
                    (rst) =>
                    {
                        if (rst)
                        {
                            m_myself.RpcCall("ChangeRuneIndexReq", (byte)o, (byte)n);
                        }
                        MogoGlobleUIManager.Instance.ConfirmHide();
                    });
                return;
            }
        }
        m_myself.RpcCall("ChangeRuneIndexReq", (byte)o, (byte)n);
    }

    /// <summary>
    /// 更换身上位置
    /// </summary>
    /// <param name="oldPosi">原位置</param>
    /// <param name="newPosi">新位置</param>
    private void ChangePosi(int oldPosi, int newPosi)
    {
        float t = UnityEngine.Time.realtimeSinceStartup;
        if (t - pre < 0.3f)
        {
            return;
        }
        pre = t;
        //todo更换身上位置,要判断是否为吞噬操作
        if (oldPosi == newPosi)
        {
            return;
        }
        if (body[newPosi].locked)
        {
            return;
        }
        //提示部分
        if (body[oldPosi].rune != null && body[newPosi].rune != null)
        {
            string s = "";
            if (EatAble(body[oldPosi].rune, body[newPosi].rune))
            {
                //s = LanguageData.dataMap[body[oldPosi].rune.ResData.name].content + "将吞噬" + LanguageData.dataMap[body[newPosi].rune.ResData.name].content;
                s = LanguageData.GetContent(200006, LanguageData.dataMap[body[oldPosi].rune.ResData.name].content, LanguageData.dataMap[body[newPosi].rune.ResData.name].content);
            }
            else
            {
                //s = LanguageData.dataMap[body[newPosi].rune.ResData.name].content + "将吞噬" + LanguageData.dataMap[body[oldPosi].rune.ResData.name].content;
                s = LanguageData.GetContent(200006, LanguageData.dataMap[body[newPosi].rune.ResData.name].content, LanguageData.dataMap[body[oldPosi].rune.ResData.name].content);
            }
            MogoGlobleUIManager.Instance.Confirm(s,
                (rst) => 
                {
                    if (rst)
                    {
                        m_myself.RpcCall("ChangeRunePosiReq", (byte)oldPosi, (byte)newPosi);
                    }
                    MogoGlobleUIManager.Instance.ConfirmHide();
                });
            return;
        }
        m_myself.RpcCall("ChangeRunePosiReq", (byte)oldPosi, (byte)newPosi);
    }
    #endregion

    #region 以下为后端更新函数
    private string GetRuneNameString(int quality, string name)
    {
        QualityColorData colorData = QualityColorData.dataMap[quality];
        string strName = string.Concat("[", colorData.color, "]", name, "[-]");
        return strName;
    }

    //增加一个符文到背包
    public void AddRuneToBag(string uuid, int idx, int resID, int exp)
    {
        LoggerHelper.Debug("add idx: " + idx + " " + resID + " " + exp);
        Rune rune = new Rune(uuid, resID, exp);
        rune.index = idx;
        rune.inBag = true;
        bag[rune.index] = rune;
        //todo更新UI
        string name = "no name";
        if (LanguageData.dataMap.ContainsKey(rune.ResData.name))
        {
            name = LanguageData.dataMap[rune.ResData.name].content;
            name = GetRuneNameString(rune.ResData.quality, name);
        }

        IconData _t = IconData.dataMap[rune.ResData.icon];       

        if (idx < 16)
        {
            if (wishCnt <= 0)
            {
                if (DragonUIViewManager.Instance != null)
                {
                    DragonUIViewManager.Instance.AddDragonItem(rune.ResData.level, name, idx, _t.path, _t.color);
                }
                wishCnt = 0;
            }
            else
            {
                wishCnt--;

                if (DragonUIViewManager.Instance != null)
                {
                    DragonUIViewManager.Instance.WishAddDragonItem(rune.ResData.level, name, idx, _t.path, _t.color);
                }
            }
        }
        else
        {
            if (RuneUIViewManager.Instance == null)
            {
                return;
            }
            RuneUIViewManager.Instance.AddPackageGridItem(rune.ResData.level, name, idx % 16, _t.path, _t.color);
        }
    }

    //删除背包中的一个符文
    public void DelRuneFromBag(int idx)
    {
        LoggerHelper.Debug("del idx: " + idx);
        Rune r = bag[idx];
        bag.Remove(idx);
        //todo更新UI
        if (idx < 16)
        {
            if (r.ResData.type == RUNE_MONEY)
            {
                DragonUIViewManager.Instance.ShowFloatGold(idx);
                DragonUIViewManager.Instance.SetFloatGoldText(LanguageData.dataMap[r.ResData.effectDesc].Format(r.ResData.price), idx);
            }
            if (pickCnt <= 0)
            {
                DragonUIViewManager.Instance.RemoveDragonItem(idx);
                pickCnt = 0;
            }
            else
            {
                pickCnt--;
                DragonUIViewManager.Instance.PickUpDragonItem(idx);
            }
            
        }
        else
        {
            RuneUIViewManager.Instance.RemoveRuneItem(idx % 16);
        }
    }

    //穿上符文
    public void PutOnRuneResp(int idx, int posi)
    {
        Rune rune = bag[idx];
        bag.Remove(rune.index);
        BodyRune brune = body[posi];
        brune.rune = rune;
        rune.index = posi;
        rune.inBag = false;
        //todo更新UI
        string name = "no name";
        if (LanguageData.dataMap.ContainsKey(rune.ResData.name))
        {
            name = LanguageData.dataMap[rune.ResData.name].content;
            name = GetRuneNameString(rune.ResData.quality, name);
        }
        if (idx < 16)
        {
            DragonUIViewManager.Instance.RemoveDragonItem(idx);
        }
        else
        {
            RuneUIViewManager.Instance.RemoveRuneItem(idx % 16);
        }
        IconData _t = IconData.dataMap[rune.ResData.icon];
        RuneUIViewManager.Instance.AddInsetGridItem(rune.ResData.level, name, rune.index, _t.path, _t.color);
        CalcuScore();
    }

    //卸下符文
    public void PutDownRuneResp(int posi, int idx)
    {
        BodyRune brune = body[posi];
        Rune rune = brune.rune;
        rune.index = idx;
        rune.inBag = true;
        brune.rune = null;
        bag.Add(rune.index, rune);
        //todo更新UI
        string name = "no name";
        IconData _t = IconData.dataMap[rune.ResData.icon];
        if (LanguageData.dataMap.ContainsKey(rune.ResData.name))
        {
            name = LanguageData.dataMap[rune.ResData.name].content;
            name = GetRuneNameString(rune.ResData.quality, name);
        }
        if (idx < 16)
        {
            DragonUIViewManager.Instance.AddDragonItem(rune.ResData.level, name, idx, _t.path, _t.color);
        }
        else
        {
            RuneUIViewManager.Instance.AddPackageGridItem(rune.ResData.level, name, idx % 16, _t.path, _t.color);
        }
        RuneUIViewManager.Instance.RemoveRuneInsetItem(posi);
        CalcuScore();
    }

    //增加一个符文到身上
    public void AddRuneToBody(string uuid, int posi, int resID, int exp)
    {
        LoggerHelper.Debug("add body idx: " + posi + " " + resID + " " + exp);
        Rune rune = new Rune(uuid, resID, exp);
        rune.index = posi;
        rune.inBag = false;
        BodyRune brune = body[posi];
        brune.rune = rune;
        //todo更新UI
        string name = "no name";
        if (LanguageData.dataMap.ContainsKey(rune.ResData.name))
        {
            name = LanguageData.dataMap[rune.ResData.name].content;
			name = GetRuneNameString(rune.ResData.quality, name);
        }
        IconData _t = IconData.dataMap[rune.ResData.icon];
        if (RuneUIViewManager.Instance != null)
        {
            RuneUIViewManager.Instance.AddInsetGridItem(rune.ResData.level, name, rune.index, _t.path, _t.color);
        }
        CalcuScore();
    }

    //删除一个符文从身上
    public void DelRuneFromBody(int posi)
    {
        BodyRune brune = body[posi];
        brune.rune = null;
        if (RuneUIViewManager.Instance != null)
        {
            RuneUIViewManager.Instance.RemoveRuneInsetItem(posi);
        }
        CalcuScore();
    }

     //更新整个背包
    public void UpdateAllRune()
    {

    }

    //更新身上位置是否为锁状态
    public void UpdatePosition(int posi, bool locked)
    {
        body[posi].locked = locked;
        RuneUIViewManager.Instance.UnLockInsetGrid(posi);
    }

    //更新刷新价钱,从后端来，为后面加入随机需求准备
    public void UpdateRefreshPrice(int _gameMoney, int _rmb)
    {
        gameMoney = _gameMoney;
        RMB = _rmb;
        if (DragonUIViewManager.Instance == null)
        {
            return;
        }
        if (m_myself.gold >= gameMoney)
        {
            //DragonUIViewManager.Instance.SetGlodWishCost(String.Format("x{0}许愿", gameMoney));
            DragonUIViewManager.Instance.SetGlodWishCost(LanguageData.GetContent(48800, gameMoney));
        }
        else
        {
            //DragonUIViewManager.Instance.SetGlodWishCost(String.Format("x[FF0000]{0}许愿[-]", gameMoney));
            DragonUIViewManager.Instance.SetGlodWishCost(LanguageData.GetContent(48801, gameMoney));
        }
        if (m_myself.diamond >= RMB)
        {
            //DragonUIViewManager.Instance.SetDiamondWishCost(String.Format("x{0}许愿", RMB));
            DragonUIViewManager.Instance.SetDiamondWishCost(LanguageData.GetContent(200007, RMB));
        }
        else
        {
            //DragonUIViewManager.Instance.SetDiamondWishCost(String.Format("x[FF0000]{0}许愿[-]", RMB));
            DragonUIViewManager.Instance.SetDiamondWishCost(LanguageData.GetContent(200008, RMB));
        }
        //DragonUIViewManager.Instance.SetGlodWishAnimText(String.Format("消费{0}金币", gameMoney));
        //DragonUIViewManager.Instance.SetDiamondWishAnimText(String.Format("消费{0}宝石", RMB));
        DragonUIViewManager.Instance.SetGlodWishAnimText(LanguageData.GetContent(200009, gameMoney));
        DragonUIViewManager.Instance.SetDiamondWishAnimText(LanguageData.GetContent(200010, RMB));
        DragonUIViewManager.Instance.SetPackageGoldNum((int)m_myself.gold);
        DragonUIViewManager.Instance.SetPackageDiamondNum((int)m_myself.diamond);
    }

    public void UpdateGold()
    {
        if (DragonUIViewManager.Instance == null)
        {
            return;
        }
        if (m_myself.gold >= gameMoney)
        {
            //DragonUIViewManager.Instance.SetGlodWishCost(String.Format("x{0}许愿", gameMoney));
            DragonUIViewManager.Instance.SetGlodWishCost(LanguageData.GetContent(48800, gameMoney));
        }
        else
        {
            //DragonUIViewManager.Instance.SetGlodWishCost(String.Format("x[FF0000]{0}许愿[-]", gameMoney));
            DragonUIViewManager.Instance.SetGlodWishCost(LanguageData.GetContent(48801, gameMoney));
        }
        DragonUIViewManager.Instance.SetPackageGoldNum((int)m_myself.gold);
    }

    public void UpdateDiamond()
    {
        if (DragonUIViewManager.Instance == null)
        {
            return;
        }
        if (m_myself.diamond >= RMB)
        {
            //DragonUIViewManager.Instance.SetDiamondWishCost(String.Format("x{0}许愿", RMB));
            DragonUIViewManager.Instance.SetDiamondWishCost(LanguageData.GetContent(200007, RMB));
        }
        else
        {
            DragonUIViewManager.Instance.SetDiamondWishCost(LanguageData.GetContent(200008, RMB));
        }
        DragonUIViewManager.Instance.SetPackageDiamondNum((int)m_myself.diamond);
    }
    #endregion

    public int CalcuScore()
    {
        int rst = 0;
        foreach (var r in body)
        {
            if (r.rune != null)
            {
                rst += r.rune.ResData.score;
            }
        }
        if (RuneUIViewManager.Instance != null)
        {
            RuneUIViewManager.Instance.SetRuneUILifePower(rst);
        }
        return rst;
    }
}
