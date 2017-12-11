using System;
using System.Collections.Generic;
using System.Linq;
using Mogo.Util;
using Mogo.GameLogic.LocalServer;
using UnityEngine;

public class GMManager
{
    public GMManager()
    {
        AddListeners();
    }

    private void AddListeners()
    {
        EventDispatcher.AddEventListener<string>(Events.OtherEvent.ClientGM, ProcGM);
    }

    private void DelListeners()
    {
        EventDispatcher.RemoveEventListener<string>(Events.OtherEvent.ClientGM, ProcGM);
    }

    private void ProcGM(string cmd)
    {
        if (cmd.EndsWith("|"))
        {//去除NGUI PC平台输入时多个竖线问题
            cmd = cmd.Substring(0, cmd.Length - 1);
        }

        string[] cmds = cmd.Split(' ');
        if (cmds[0] == "@OnRange")
        {
            SkillManager.showSkillRange = true;
        }
        else if (cmds[0] == "@OffRange")
        {
            SkillManager.showSkillRange = false;
        }
        else if (cmds[0] == "@ShowSelf")
        {
            MogoWorld.gmcontent = GetSelfInfo();
        }
        else if (cmds[0] == "@ShowGuide")
        {
            MogoWorld.gmcontent = "isLockOrNot" + MogoUIQueue.Instance.IsLocking;
            LoggerHelper.Debug(MogoUIQueue.Instance.m_listUI.PackList('\n'));
        }
        else if (cmds[0] == "@AddState")
        {
            int idx = Int32.Parse(cmds[1]);
            MogoWorld.thePlayer.stateFlag = Utils.BitSet(MogoWorld.thePlayer.stateFlag, idx);
        }
        else if (cmds[0] == "@DelState")
        {
            int idx = Int32.Parse(cmds[1]);
            MogoWorld.thePlayer.stateFlag = Utils.BitReset(MogoWorld.thePlayer.stateFlag, idx);
        }
        else if (cmds[0] == "@ShowAll")
        {
            MogoWorld.gmcontent = GetAll();
        }
        else if (cmds[0] == "@ShowWithID")
        {
            uint id = UInt32.Parse(cmds[1]);
            MogoWorld.gmcontent = GetInfo(id);
        }
        else if (cmds[0] == "@DelayAddState")
        {
            uint t = UInt32.Parse(cmds[1]);
            int idx = Int32.Parse(cmds[2]);
            TimerHeap.AddTimer(t, 0, (i) => { MogoWorld.thePlayer.stateFlag = Utils.BitSet(MogoWorld.thePlayer.stateFlag, i); }, idx);
        }
        else if (cmds[0] == "@DelEntity")
        {
            uint id = UInt32.Parse(cmds[1]);
            EventDispatcher.TriggerEvent<uint>(Events.FrameWorkEvent.AOIDelEvtity, id);
        }
        else if (cmds[0] == "@PlayStory")
        {
            int id = Int32.Parse(cmds[1]);
            StoryManager.Instance.PlayStory(id);
        }
        else if (cmds[0] == "@ShowProp")
        {
            uint id = UInt32.Parse(cmds[1]);
            MogoWorld.gmcontent = GetProp(id, cmds[2]);
        }
        else if (cmds[0] == "@ShowSelfProp")
        {
            MogoWorld.gmcontent = GetSelfProp(cmds[1]);
        }
        else if (cmds[0] == "@StopCGAnim")
        {
            MogoMainCamera.Instance.OnCGAnimStop();
        }
        else if (cmds[0] == "@StopUIFX")
        {
            MogoFXManager.Instance.ReleaseAllParticleAnim();
        }
        else if (cmds[0] == "@StopSkillFX")
        {
            MogoWorld.showSkillFx = false;
        }
        else if (cmds[0] == "@StartSkillFX")
        {
            MogoWorld.showSkillFx = true;
        }
        else if (cmds[0] == "@StopLogStack")
        {

        }
        else if (cmds[0] == "@StartLogStack")
        {
            //LoggerHelper.SHOW_STACK = false;
        }
        else if (cmds[0] == "@StopHitAction")
        {
            MogoWorld.showHitAction = false;
        }
        else if (cmds[0] == "@StartHitAction")
        {
            MogoWorld.showHitAction = true;
        }
        else if (cmds[0] == "@StopHitEM")
        {
            MogoWorld.showHitEM = false;
        }
        else if (cmds[0] == "@StartHitEM")
        {
            MogoWorld.showHitEM = true;
        }
        else if (cmds[0] == "@StopHitShader")
        {
            MogoWorld.showHitShader = false;
        }
        else if (cmds[0] == "@StartHitShader")
        {
            MogoWorld.showHitShader = true;
        }
        else if (cmds[0] == "@StopFloatBlood")
        {
            MogoWorld.showFloatBlood = false;
        }
        else if (cmds[0] == "@StartFloatBlood")
        {
            MogoWorld.showFloatBlood = true;
        }
        else if (cmds[0] == "@HurtMeZero")
        {
            MogoWorld.unhurtMe = true;
        }
        else if (cmds[0] == "@HurtMeNormal")
        {
            MogoWorld.unhurtMe = false;
        }
        else if (cmds[0] == "@HurtDummyZero")
        {
            MogoWorld.unhurtDummy = true;
        }
        else if (cmds[0] == "@HurtDummyNormal")
        {
            MogoWorld.unhurtDummy = false;
        }
        else if (cmds[0] == "@StopAI")
        {
            MogoWorld.pauseAI = true;
        }
        else if (cmds[0] == "@StartAI")
        {
            MogoWorld.pauseAI = false;
        }
        else if (cmds[0] == "@OpenGM")
        {
            MogoWorld.showClientGM = true;
        }
        else if (cmds[0] == "@CloseGM")
        {
            MogoWorld.showClientGM = false;
        }
        else if (cmds[0] == "@RFC")
        {
            var rfc = ResourceManager.resources.ToDictionary(c => c.Value.RelativePath, c => c.Value.referenceCount).PackMap(':', '\n');
            LoggerHelper.Warning("current reference count" + '\n' + rfc);
        }
        else if (cmds[0] == "@Shake")
        {
            StoryManager.Instance.IsShake = true;
        }
        else if (cmds[0] == "@CG")
        {
            if (StoryManager.Instance.IsOpen)
            {
                StoryManager.Instance.IsOpen = false;
            }
            else
            {
                StoryManager.Instance.IsOpen = true;
            }
        }
        else if (cmds[0] == "@GUIDE")
        {
            if (GuideSystem.Instance.IsOpen)
            {
                GuideSystem.Instance.IsOpen = false;
            }
            else
            {
                GuideSystem.Instance.IsOpen = true;
            }
        }
        else if (cmds[0] == "@OpenFloatText")
        {
            MogoWorld.showFloatText = true;
        }
        else if (cmds[0] == "@CloseFloatText")
        {
            MogoWorld.showFloatText = false;
        }
        else if (cmds[0] == "@ClearAll")
        {
            ResourceManager.ClearAll();
            MogoMsgBox.Instance.ShowFloatingText("ClearAll");
        }
        else if (cmds[0] == "@TG")
        {
            GuideSystem.Instance.enqueueGuide(Int32.Parse(cmds[1]), true);
        }
        else if (cmds[0] == "@Debug")
        {
            if (MogoWorld.showDebug)
            {
                MogoWorld.showDebug = false;
            }
            else
            {
                MogoWorld.showDebug = true;
            }

            MogoUIManager.Instance.ShowDebugUI(MogoWorld.showDebug);
        }
        else if (cmds[0] == "@ShowObjID")
        {
            BillboardViewManager.Instance.ShowTestInfo(true);
        }
        else if (cmds[0] == "@HideObjID")
        {
            BillboardViewManager.Instance.ShowTestInfo(false);
        }
        else if (cmds[0] == "@ShowEquip")
        {
            string msg = "\n";
            foreach (Mogo.Game.EntityParent e in MogoWorld.Entities.Values)
            {
                msg += e.Transform.tag + " " + e.Transform.gameObject.name + "," + e.ID + "\'s EquipList:\n";
                msg += e.loadedWeapon + ",";
                msg += e.loadedCuirass + "\n";
            }
            MogoWorld.gmcontent += msg;
            //MogoMsgBox.Instance.ShowMsgBox(msg);
        }
        else if (cmds[0] == "@StartAnger")
        {
            (MogoWorld.thePlayer.skillManager as PlayerSkillManager).isAnger = true;
        }
        else if (cmds[0] == "@StopAnger")
        {
            (MogoWorld.thePlayer.skillManager as PlayerSkillManager).isAnger = false;
        }
        else if (cmds[0] == "@StartToLocalSvr")
        {
            Mogo.Game.ServerProxy.SomeToLocal = true;
        }
        else if (cmds[0] == "@StopToLocalSvr")
        {
            Mogo.Game.ServerProxy.SomeToLocal = false;
        }
        else if (cmds[0] == "@LoadMissionData")
        {
            LocalServerSceneManager.Instance.EnterMission(Int32.Parse(cmds[1]), Int32.Parse(cmds[2]));
        }
        else if (cmds[0] == "@PR")
        {
            LoggerHelper.Info(ResourceManager.resources.Values.ToList().PackList('\n'));
        }
        else if (cmds[0] == "@PRD")
        {
            LoggerHelper.Info(AssetCacheMgr.ResourceDic.PackMap(mapSpriter: '\n'));
        }
        else if (cmds[0] == "@PGONM")
        {
            LoggerHelper.Info(AssetCacheMgr.GameObjectNameMapping.PackMap(mapSpriter: '\n'));
        }
        else if (cmds[0] == "@UISwitch")
        {
            MogoWorld.scenesManager.MainUI.SetActive(!MogoWorld.scenesManager.MainUI.activeSelf);
        }
        else if (cmds[0] == "@FPS")
        {
            Application.targetFrameRate = int.Parse(cmds[1]);
        }
        else if (cmds[0] == "@wing")
        {
            winggm(cmds);
        }
        else if (cmds[0] == "@call")
        {
            switch (cmds[1])
            {
                case "withdraw":
                    {
                        EventDispatcher.TriggerEvent(Events.OtherEvent.Withdraw, ulong.Parse(cmds[2]));
                        break;
                    }
                case "diamond":
                    {
                        EventDispatcher.TriggerEvent(Events.OtherEvent.DiamondMine);
                        break;
                    }
                case "charge":
                    {
                        EventDispatcher.TriggerEvent(Events.OtherEvent.CheckCharge);
                        break;
                    }
            }
        }
        MogoWorld.gmcontent += "\nGM Proc Complete";
    }

    private void winggm(string[] cmds)
    {
        switch (cmds[1])
            {
                case "buy":
                    {
                        EventDispatcher.TriggerEvent(Events.WingEvent.Buy, int.Parse(cmds[2]));
                        break;
                    }
                case "active":
                    {
                        EventDispatcher.TriggerEvent(Events.WingEvent.Active, int.Parse(cmds[2]));
                        break;
                    }
                case "puton":
                    {
                        EventDispatcher.TriggerEvent(Events.WingEvent.PutOn, int.Parse(cmds[2]));
                        break;
                    }
                case "undo":
                    {
                        EventDispatcher.TriggerEvent(Events.WingEvent.Undo, int.Parse(cmds[2]));
                        break;
                    }
                case "unlock":
                    {
                        EventDispatcher.TriggerEvent(Events.WingEvent.UnLock, int.Parse(cmds[2]));
                        break;
                    }
                case "upgrade":
                    {
                        EventDispatcher.TriggerEvent(Events.WingEvent.Upgrade, int.Parse(cmds[2]));
                        break;
                    }
            }
    }

    private string GetSelfInfo()
    {
        string content = "";
        content += "id: " + MogoWorld.thePlayer.ID + "  ";
        content += "dbid: " + MogoWorld.thePlayer.dbid + "   ";
        content += "hp: " + MogoWorld.thePlayer.curHp + "  ";
        content += "max hp: " + MogoWorld.thePlayer.hp + "  ";
        content += "state: " + MogoWorld.thePlayer.CurrentMotionState + "  ";
        content += "stiff: " + MogoWorld.thePlayer.stiff + "  ";
        content += "hit air: " + MogoWorld.thePlayer.hitAir + "  ";
        content += "knock down: " + MogoWorld.thePlayer.knockDown + "  ";
        content += "curr skill: " + MogoWorld.thePlayer.currSpellID + "  ";
        content += "curr hitAction: " + MogoWorld.thePlayer.currHitAction + "  ";
        content += "curr act: " + MogoWorld.thePlayer.animator.GetInteger("Action") + "  ";
        content += "state flag: " + MogoWorld.thePlayer.stateFlag + "  ";
        content += "death flag: " + MogoWorld.thePlayer.deathFlag + "  ";
        content += "scene id: " + MogoWorld.thePlayer.sceneId + "  ";
        content += "skill act name: " + MogoWorld.thePlayer.skillActName + "  ";
        content += "anger: " + MogoWorld.thePlayer.Anger + "   ";
        return content;
    }

    private string GetAll()
    {
        string content = "";
        foreach (var i in MogoWorld.Entities)
        {
            if (i.Value is Mogo.Game.EntityDummy)
            {
                content += "dummy id: " + i.Key + " monsterId: " + i.Value.MonsterData.id + ", \n";
            }
            else if (i.Value is Mogo.Game.EntityPlayer)
            {
                content += "player id: " + i.Key + ", \n";
            }
            else if (i.Value is Mogo.Game.EntityMercenary)
            {
                content += "mercenary id: " + i.Key + " monsterId: " + i.Value.MonsterData.id + ", \n";
            }
            else if (i.Value is Mogo.Game.EntityMonster)
            {
                content += "monster id: " + i.Key + " monsterId: " + i.Value.MonsterData.id + ", \n";
            }
            else
            {
                content += "id: " + i.Key + ", \n";
            }
        }
        return content;
    }

    private string GetInfo(uint id)
    {
        string content = "";
        Mogo.Game.EntityParent e = null;
        if (!MogoWorld.Entities.TryGetValue(id, out e))
        {
            return content;
        }
        content += "id: " + e.ID + "   ";
        content += "hp: " + e.curHp + "   ";
        content += "max hp: " + e.hp + "   ";
        content += "state: " + e.CurrentMotionState + "   ";
        content += "stiff: " + e.stiff + "   ";
        content += "hit air: " + e.hitAir + "   ";
        content += "knockdown: " + e.knockDown + "   ";
        content += "curr skill: " + e.currSpellID + "   ";
        content += "curr hitAction: " + e.currHitAction + "   ";
        if (e.animator)
        {
            content += "curr act: " + e.animator.GetInteger("Action") + "  ";
        }
        else
        {
            content += "curr act: not animator";
        }
        content += "state flag: " + e.stateFlag + "  ";
        content += "skill act name: " + e.skillActName + "  ";
        return content;
    }

    private string GetProp(uint id, string prop)
    {
        string content = "";
        Mogo.Game.EntityParent e = null;
        if (!MogoWorld.Entities.TryGetValue(id, out e))
        {
            return content;
        }
        if (e.IntAttrs.ContainsKey(prop))
        {
            return prop + ": " + e.IntAttrs[prop];
        }
        if (e.DoubleAttrs.ContainsKey(prop))
        {
            return prop + ": " + e.DoubleAttrs[prop];
        }
        if (e.StringAttrs.ContainsKey(prop))
        {
            return prop + ": " + e.StringAttrs[prop];
        }
        if (e.ObjectAttrs.ContainsKey(prop))
        {
            return prop + ": object";
        }
        return content;
    }

    private string GetSelfProp(string prop)
    {
        if (MogoWorld.thePlayer.IntAttrs.ContainsKey(prop))
        {
            return prop + ": " + MogoWorld.thePlayer.IntAttrs[prop];
        }
        if (MogoWorld.thePlayer.DoubleAttrs.ContainsKey(prop))
        {
            return prop + ": " + MogoWorld.thePlayer.DoubleAttrs[prop];
        }
        if (MogoWorld.thePlayer.StringAttrs.ContainsKey(prop))
        {
            return prop + ": " + MogoWorld.thePlayer.StringAttrs[prop];
        }
        if (MogoWorld.thePlayer.ObjectAttrs.ContainsKey(prop))
        {
            return prop + ": object";
        }
        LoggerHelper.Error("fdfdfdf " + prop);
        return "";

    }

    public void Clean()
    {

    }
}