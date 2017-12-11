using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;
using System.Collections.Generic;
using Mogo.GameData;

public enum SpriteErrorCode
{
    Successful = 0,
    MaxThanSelfLevel = 1,
    NotEnoughPoint = 2,
    NotFoundConsume = 3,
    NotPlayers = 4,
    NotOpen = 5,
    ActiveSkillExists = 6,
    BeyondIndex = 7
}

public class AssistantUILogicManager
{

    private static AssistantUILogicManager m_instance;

    public static AssistantUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new AssistantUILogicManager();
            }

            return AssistantUILogicManager.m_instance;

        }
    }

    private Dictionary<int, int> m_dictGridIdtoSkillId = new Dictionary<int, int>();
    private Dictionary<int, int> m_dictGridIdtoMarkId = new Dictionary<int, int>();

    int m_iSkillContractGridNum = 0;
    int m_iMintmarkGridNum = 0;



    void OnAssistantSkillGridUp(int id)
    {
        LoggerHelper.Debug(id);


        int skill = m_dictGridIdtoSkillId[id];
        string name = SpiritSkillData.dataMap[skill].name;
        string skillName = LanguageData.dataMap[int.Parse(name)].content;
        //int skillId = SpiritSkillData.dataMap[skill].skillid;

        AssistantUIViewManager.Instance.SkillGridTip.SkillName = skillName;
        AssistantUIViewManager.Instance.SkillGridTip.BaseAttr = LanguageData.dataMap[0].content;
        AssistantUIViewManager.Instance.SkillGridTip.AdditiveAttr = LanguageData.dataMap[0].content;

        AssistantUIViewManager.Instance.ShowSkillsContractTip(true);
    }

    void OnAssistantMintmarkGridUp(int id)
    {
        LoggerHelper.Debug(id);

        int mark = m_dictGridIdtoMarkId[id];
        string name = SpiritMarkData.dataMap[mark].name;
        string markName = LanguageData.dataMap[int.Parse(name)].content;

        AssistantUIViewManager.Instance.MintmarkGridTip.SkillName = markName;

        AssistantUIViewManager.Instance.ShowElemtMintmarkTip(true);
    }

    void OnSkillsContractGrowTipButtonUp()
    {
        LoggerHelper.Debug("TipGrowUp");
        MogoWorld.thePlayer.RpcCall("LevelUpSpiritSkill");
    }

    void OnContractGrowButtonUp()
    {
        AssistantUIViewManager.Instance.SetSkillsContractLevel(MogoWorld.thePlayer.GetDoubleAttr("SpiritSkillLevel").ToString());
        AssistantUIViewManager.Instance.SetSkillsContractMoney(MogoWorld.thePlayer.GetIntAttr("SpiritSkillPoint").ToString());

        AssistantUIViewManager.Instance.ShowSkillsContractGrowTip(true);
    }

    void OnElementMintmarkGrowTipButtonUp()
    {
        LoggerHelper.Debug("TipGrowUp");
        MogoWorld.thePlayer.RpcCall("LevelUpMark");
    }

    void OnMintmarkGrowButtonUp()
    {

        AssistantUIViewManager.Instance.SetElementMintmarkLevel(MogoWorld.thePlayer.GetDoubleAttr("SpiritMarkLevel").ToString());
        AssistantUIViewManager.Instance.SetElementMintmarkMoney(MogoWorld.thePlayer.GetIntAttr("SpiritMarkPoint").ToString());
        AssistantUIViewManager.Instance.ShowElementMintmarkGrowTip(true);
    }

    void OnAssistantUISkillsDragToBodyGridUp(int packageId, int gridId)
    {
        //查表packageId --> SkillId
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
        byte skillId = (byte)m_dictGridIdtoSkillId[packageId];

        MogoWorld.thePlayer.RpcCall("ClientDragSkill", skillId, gridId + 1);
    }

    void OnAssistantUIMintmarkGridDragToBodyGridUp(int packageId, int gridId)
    {
        //查表packageId --> MintmarkId

        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
        byte mintmarkId = (byte)m_dictGridIdtoMarkId[packageId];
        MogoWorld.thePlayer.RpcCall("ClientDragMark", mintmarkId, gridId + 1);
    }

    void OnLevelUpSkillResp(byte errorCode)
    {
        LoggerHelper.Debug("LevelUpSkillResp " + errorCode);

        //查表得tip
        AssistantUIViewManager.Instance.SetSkillsContractLevel(MogoWorld.thePlayer.GetDoubleAttr("SpiritSkillLevel").ToString());
        AssistantUIViewManager.Instance.SetSkillsContractMoney(MogoWorld.thePlayer.GetIntAttr("SpiritSkillPoint").ToString());
    }

    void OnLevelUpMarkResp(byte errorCode)
    {
        LoggerHelper.Debug("LevelUpMarkResp " + errorCode);

        //查表得Tip
        AssistantUIViewManager.Instance.SetElementMintmarkLevel(MogoWorld.thePlayer.GetDoubleAttr("SpiritMarkLevel").ToString());
        AssistantUIViewManager.Instance.SetElementMintmarkMoney(MogoWorld.thePlayer.GetIntAttr("SpiritMarkPoint").ToString());
    }

    void OnClientDragSkillResp(UInt32 skillId, UInt32 gridId, byte errorCode)
    {
        LoggerHelper.Debug("ClientDragSkillResp " + errorCode);

        if ((SpriteErrorCode)errorCode == SpriteErrorCode.Successful)
        {
            RefreshBodySkillsList();
        }
        else
        {
            LoggerHelper.Debug("DragSkillResp Error!!! ErrorCode = " + errorCode);
        }
    }

    void OnClientDragMarkResp(UInt32 markId, UInt32 gridId, byte errorCode)
    {
        LoggerHelper.Debug("ClientDragmarkResp " + errorCode);

        if ((SpriteErrorCode)errorCode == SpriteErrorCode.Successful)
        {
            RefreshBodyMintmarkList();
        }
        else
        {
            LoggerHelper.Debug("DragMarkResp Error!!! ErrorCode = " + errorCode);
        }
    }

    void OnAssistantUISkillGridDragBegin(int id)
    {
        //查表得Icon
        MogoGlobleUIManager.Instance.ShowUICursorSprite("jl_keyinsuipian");
    }

    void OnAssistantUIMintmarkGridDragBegin(int id)
    {

        //查表得Icon
        MogoGlobleUIManager.Instance.ShowUICursorSprite("jl_keyinsuipian");
    }

    void OnAssistantUISkillGridDragOutside()
    {
        LoggerHelper.Debug("Outside");
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    void OnAssistantUIMintmarkGridDragOutside()
    {
        LoggerHelper.Debug("Outside");
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    public void RefreshSkillsContractList(LuaTable lt = null)
    {
        LuaTable ltSS;

        m_dictGridIdtoSkillId.Clear();

        if (lt == null)
        {
            ltSS = MogoWorld.thePlayer.GetObjectAttr("SpiritSkill") as LuaTable;
        }
        else
        {
            ltSS = lt;
            MogoWorld.thePlayer.SetObjectAttr("SpiritSkill", lt);
        }

        int index = 1;

        if (ltSS != null)
        {
            foreach (var item in ltSS)
            {
                switch ((string)item.Value)
                {
                    case "0":
                        AssistantUIViewManager.Instance.SetSkillsContractGridEnable(index - 1, false);
                        break;

                    case "1":
                        AssistantUIViewManager.Instance.SetSkillsContractGridEnable(index - 1, true);
                        break;

                }

                m_dictGridIdtoSkillId[index - 1] = int.Parse((string)item.Key);
                ++index;
            }
            //for (int i = 1; i < 7; ++i)
            //{
            //    switch ((string)ltSS[i.ToString()])
            //    {
            //        case "0":
            //            AssistantUIViewManager.Instance.SetSkillsContractGridEnable(i - 1, false);
            //            break;

            //        case "1":
            //            AssistantUIViewManager.Instance.SetSkillsContractGridEnable(i - 1, true);
            //            break;
            //    }
            //    m_dictGridIdtoSkillId[i - 1] = i;
            //}
        }
    }

    public void RefreshBodySkillsList(LuaTable lt = null)
    {
        LuaTable ltSSS;

        if (lt == null)
        {
            ltSSS = MogoWorld.thePlayer.GetObjectAttr("SelectedSpiritSkill") as LuaTable;
        }
        else
        {
            ltSSS = lt;
            LoggerHelper.Debug("refresh.................");
            MogoWorld.thePlayer.SetObjectAttr("SelectedSpiritSkill", lt);
        }

        if (ltSSS != null)
        {
            for (int i = 1; i < 4; ++i)
            {
                switch (i)
                {
                    case 1:
                        if (!ltSSS.ContainsKey(i.ToString()))
                        {
                            LoggerHelper.Debug("Initative Not Open");
                            AssistantUIViewManager.Instance.ShowSkillsContractInitativeLock(true);
                            AssistantUIViewManager.Instance.ShowSkillsContractInitativeIcon(false);
                        }
                        else if ((string)ltSSS[i.ToString()] == "0")
                        {
                            LoggerHelper.Debug("Initative No Skill");
                            AssistantUIViewManager.Instance.ShowSkillsContractInitativeLock(false);
                            AssistantUIViewManager.Instance.ShowSkillsContractInitativeIcon(false);
                        }
                        else
                        {
                            LoggerHelper.Debug("Initative Have Skill");
                            AssistantUIViewManager.Instance.ShowSkillsContractInitativeLock(false);
                            AssistantUIViewManager.Instance.ShowSkillsContractInitativeIcon(true);
                        }
                        break;

                    case 2:
                        if (!ltSSS.ContainsKey(i.ToString()))
                        {
                            LoggerHelper.Debug("Passive0 Not Open");
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive0Lock(true);
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive0Icon(false);
                        }
                        else if ((string)ltSSS[i.ToString()] == "0")
                        {
                            LoggerHelper.Debug("Passive0 No Skill");
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive0Lock(false);
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive0Icon(false);

                        }
                        else
                        {
                            LoggerHelper.Debug("Passive0 Have Skill");
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive0Lock(false);
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive0Icon(true);

                        }
                        break;

                    case 3:
                        if (!ltSSS.ContainsKey(i.ToString()))
                        {
                            LoggerHelper.Debug("Passive1 Not Open");
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive1Lock(true);
                            AssistantUIViewManager.Instance.ShowSkillsCOntractPassive1Icon(false);
                        }
                        else if ((string)ltSSS[i.ToString()] == "0")
                        {
                            LoggerHelper.Debug("Passive1 No Skill");
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive1Lock(false);
                            AssistantUIViewManager.Instance.ShowSkillsCOntractPassive1Icon(false);
                        }
                        else
                        {
                            LoggerHelper.Debug("Passive1 Have Skill");
                            AssistantUIViewManager.Instance.ShowSkillsContractPassive1Lock(false);
                            AssistantUIViewManager.Instance.ShowSkillsCOntractPassive1Icon(true);
                        }
                        break;

                }
            }
        }

    }

    public void RefreshElementMintmarkList(LuaTable lt = null)
    {
        LuaTable ltSM;

        m_dictGridIdtoMarkId.Clear();

        if (lt == null)
        {
            ltSM = MogoWorld.thePlayer.GetObjectAttr("SpiritMark") as LuaTable;
        }
        else
        {
            ltSM = lt;
            MogoWorld.thePlayer.SetObjectAttr("SpiritMark", lt);
        }

        int index = 1;
        foreach (var item in ltSM)
        {
            switch ((string)item.Value)
            {
                case "0":
                    AssistantUIViewManager.Instance.ShowElementMintmarkGridLock(index - 1, false);
                    break;

                case "1":
                    AssistantUIViewManager.Instance.ShowElementMintmarkGridLock(index - 1, true);
                    break;
            }

            m_dictGridIdtoMarkId[index - 1] = int.Parse((string)item.Key);
            ++index;

        }
    }

    public void RefreshBodyMintmarkList(LuaTable lt = null)
    {
        LuaTable ltSSM;

        if (lt == null)
        {
            ltSSM = MogoWorld.thePlayer.GetObjectAttr("SelectedSpiritMark") as LuaTable;
        }
        else
        {
            ltSSM = lt;
            MogoWorld.thePlayer.SetObjectAttr("SelectedSpiritMark", lt);
        }

        if (ltSSM != null)
        {
            for (int i = 1; i < 6; ++i)
            {
                if (!ltSSM.ContainsKey(i.ToString()))
                {
                    LoggerHelper.Debug("Not Open" + " " + i);
                    AssistantUIViewManager.Instance.ShowMintmarkBodyGridLock(i - 1, true);
                    AssistantUIViewManager.Instance.ShowMintmarkBodyGridIcon(i - 1, false);
                }
                else if ((string)ltSSM[i.ToString()] == "0")
                {
                    LoggerHelper.Debug("No Mintmark " + i);
                    AssistantUIViewManager.Instance.ShowMintmarkBodyGridLock(i - 1, false);
                    AssistantUIViewManager.Instance.ShowMintmarkBodyGridIcon(i - 1, false);
                }
                else
                {
                    LoggerHelper.Debug("Have Mintmark " + i);
                    AssistantUIViewManager.Instance.ShowMintmarkBodyGridLock(i - 1, false);
                    AssistantUIViewManager.Instance.ShowMintmarkBodyGridIcon(i - 1, true);
                }

            }
        }

    }

    void OnSpritePropRefreshResp(byte type, LuaTable lt)
    {
        LoggerHelper.Debug("Resp " + type);
        switch (type)
        {
            case 1:
                RefreshSkillsContractList(lt);
                break;

            case 2:
                RefreshElementMintmarkList(lt);
                break;

            case 3:
                RefreshBodySkillsList(lt);
                break;

            case 4:
                RefreshBodyMintmarkList(lt);
                break;
        }
    }

    void OnLoadSkillContractGridDone(int id)
    {

        if (id == m_iSkillContractGridNum - 1)
        {
            AssistantUILogicManager.Instance.RefreshBodySkillsList();
            AssistantUILogicManager.Instance.RefreshSkillsContractList();
        }
    }

    void OnLoadMintmarkGridDone(int id)
    {
        if (id == m_iMintmarkGridNum - 1)
        {
            AssistantUILogicManager.Instance.RefreshElementMintmarkList();
            AssistantUILogicManager.Instance.RefreshBodyMintmarkList();
        }
    }

    public void Initialize()
    {
        AssistantUIViewManager.Instance.ASSISTANTSKILLGRIDUP += OnAssistantSkillGridUp;
        AssistantUIViewManager.Instance.SKILLSCONTRACTGROWTIPBTNUP += OnSkillsContractGrowTipButtonUp;
        AssistantUIViewManager.Instance.CONTRACTGROWBTNUP += OnContractGrowButtonUp;
        AssistantUIViewManager.Instance.ELEMENTMINTMARKGROWTIPBTNUP += OnElementMintmarkGrowTipButtonUp;
        AssistantUIViewManager.Instance.MINTMARKGROWBTNUP += OnMintmarkGrowButtonUp;
        AssistantUIViewManager.Instance.ASSISTANTMINTMARKGRIDUP += OnAssistantMintmarkGridUp;

        EventDispatcher.AddEventListener<int, int>(Events.AssistantEvent.SkillGridDragToBodyGrid, OnAssistantUISkillsDragToBodyGridUp);
        EventDispatcher.AddEventListener<int, int>(Events.AssistantEvent.MintmarkGridDragToBodyGrid, OnAssistantUIMintmarkGridDragToBodyGridUp);

        EventDispatcher.AddEventListener<byte>(Events.AssistantEvent.LevelUpSkillResp, OnLevelUpSkillResp);
        EventDispatcher.AddEventListener<byte>(Events.AssistantEvent.LevelUpMarkResp, OnLevelUpMarkResp);
        EventDispatcher.AddEventListener<UInt32, UInt32, byte>(Events.AssistantEvent.ClientDragSkillResp, OnClientDragSkillResp);
        EventDispatcher.AddEventListener<UInt32, UInt32, byte>(Events.AssistantEvent.ClientDragMarkResp, OnClientDragMarkResp);

        EventDispatcher.AddEventListener(Events.AssistantEvent.SkillGridDragOutside, OnAssistantUISkillGridDragOutside);
        EventDispatcher.AddEventListener(Events.AssistantEvent.MintmarkGridDragOutside, OnAssistantUIMintmarkGridDragOutside);
        EventDispatcher.AddEventListener<int>(Events.AssistantEvent.SkillGridDragBegin, OnAssistantUISkillGridDragBegin);
        EventDispatcher.AddEventListener<int>(Events.AssistantEvent.MintmarkGridDragBegin, OnAssistantUIMintmarkGridDragBegin);

        EventDispatcher.AddEventListener<byte, LuaTable>(Events.AssistantEvent.PropRefreshResp, OnSpritePropRefreshResp);

        EventDispatcher.AddEventListener<int>("LoadSkillContractGridDone", OnLoadSkillContractGridDone);
        EventDispatcher.AddEventListener<int>("LoadMintmarkGridDone", OnLoadMintmarkGridDone);

        AssistantUIViewManager.Instance.AssistantUIModelName = "NPC_1057.prefab";

        m_iMintmarkGridNum = 11;
        m_iSkillContractGridNum = 10;

    }

    public void Release()
    {

        AssistantUIViewManager.Instance.ASSISTANTSKILLGRIDUP -= OnAssistantSkillGridUp;
        AssistantUIViewManager.Instance.SKILLSCONTRACTGROWTIPBTNUP -= OnSkillsContractGrowTipButtonUp;
        AssistantUIViewManager.Instance.CONTRACTGROWBTNUP -= OnContractGrowButtonUp;
        AssistantUIViewManager.Instance.ELEMENTMINTMARKGROWTIPBTNUP -= OnElementMintmarkGrowTipButtonUp;
        AssistantUIViewManager.Instance.MINTMARKGROWBTNUP -= OnMintmarkGrowButtonUp;
        AssistantUIViewManager.Instance.ASSISTANTMINTMARKGRIDUP -= OnAssistantMintmarkGridUp;

        EventDispatcher.RemoveEventListener<int, int>(Events.AssistantEvent.SkillGridDragToBodyGrid, OnAssistantUISkillsDragToBodyGridUp);
        EventDispatcher.RemoveEventListener<int, int>(Events.AssistantEvent.MintmarkGridDragToBodyGrid, OnAssistantUIMintmarkGridDragToBodyGridUp);

        EventDispatcher.RemoveEventListener<byte>(Events.AssistantEvent.LevelUpSkillResp, OnLevelUpSkillResp);
        EventDispatcher.RemoveEventListener<byte>(Events.AssistantEvent.LevelUpMarkResp, OnLevelUpMarkResp);
        EventDispatcher.RemoveEventListener<UInt32, UInt32, byte>(Events.AssistantEvent.ClientDragSkillResp, OnClientDragSkillResp);
        EventDispatcher.RemoveEventListener<UInt32, UInt32, byte>(Events.AssistantEvent.ClientDragMarkResp, OnClientDragMarkResp);

        EventDispatcher.RemoveEventListener(Events.AssistantEvent.SkillGridDragOutside, OnAssistantUISkillGridDragOutside);
        EventDispatcher.RemoveEventListener(Events.AssistantEvent.MintmarkGridDragOutside, OnAssistantUIMintmarkGridDragOutside);
        EventDispatcher.RemoveEventListener<int>(Events.AssistantEvent.SkillGridDragBegin, OnAssistantUISkillGridDragBegin);
        EventDispatcher.RemoveEventListener<int>(Events.AssistantEvent.MintmarkGridDragBegin, OnAssistantUIMintmarkGridDragBegin);
        EventDispatcher.RemoveEventListener<byte, LuaTable>(Events.AssistantEvent.PropRefreshResp, OnSpritePropRefreshResp);

        EventDispatcher.RemoveEventListener<int>("LoadSkillContractGridDone", OnLoadSkillContractGridDone);
        EventDispatcher.RemoveEventListener<int>("LoadMintmarkGridDone", OnLoadMintmarkGridDone);
    }
}
