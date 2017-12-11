// 模块名   :  Events
// 创建者   :  Steven Yang
// 创建日期 :  2012-12-12
// 描    述 :  定义各种事件参数类

using System;

namespace Mogo.Util
{
    static public class Events
    {
        public readonly static string Unkown = "Unkown";

        // 定义事件参数类
        // 网络消息
        static public class NetworkEvent
        {
            public readonly static string Connect = "NetworkEvent.Connect"; //连接请求事件
            public readonly static string OnClose = "NetworkEvent.OnClose";
            public readonly static string OnDataRecv = "NetworkEvent.OnDataRecv";
            public readonly static string OnConnected = "NetworkEvent.OnConnected";
        }

        static public class FrameWorkEvent
        {
            public readonly static string EntityAttached = Mogo.RPC.MSGIDType.CLIENT_ENTITY_ATTACHED.ToString();
            public readonly static string EntityCellAttached = Mogo.RPC.MSGIDType.CLIENT_ENTITY_CELL_ATTACHED.ToString();
            public readonly static string AOINewEntity = Mogo.RPC.MSGIDType.CLIENT_AOI_NEW_ENTITY.ToString(); //entity进入事件
            public readonly static string AOIDelEvtity = Mogo.RPC.MSGIDType.CLIENT_AOI_DEL_ENTITY.ToString(); //entity退出事件
            public readonly static string BaseLogin = Mogo.RPC.MSGIDType.BASEAPP_CLIENT_LOGIN.ToString();
            public readonly static string Login = "FrameWorkEvent.Login";
            public readonly static string ReConnectKey = "FrameWorkEvent.RECONNECT_KEY";
            public readonly static string ReConnectRefuse = "FrameWorkEvent.RECONNECT_REFUSE";
            public readonly static string DefuseLogin = "FrameWorkEvent.DEFUSE_LOGIN";

            public readonly static string CheckDef = Mogo.RPC.MSGIDType.LOGINAPP_CHECK.ToString();
        }

        // FSMMotionEvent
        static public class FSMMotionEvent
        {
            public readonly static string OnPrepareEnd = "FSMMotionEvent.OnPrepareEnd";
            public readonly static string OnAttackingEnd = "FSMMotionEvent.OnAttackingEnd";
            public readonly static string OnHitAnimEnd = "FSMMotionEvent.OnHitAnimEnd";
            public readonly static string OnRollEnd = "FSMMotionEvent.OnRollEnd";
            public readonly static string OnHit = "FSMMotionEvent.OnHit";
        }

        // 战斗UI操作事件 
        static public class UIBattleEvent
        {
            public readonly static string OnNormalAttack = "UIBattleEvent.OnNormalAttack";
            public readonly static string OnSpellOneAttack = "UIBattleEvent.OnSpellOneAttack";
            public readonly static string OnSpellTwoAttack = "UIBattleEvent.OnSpellTwoAttack";
            public readonly static string OnSpellThreeAttack = "UIBattleEvent.OnSpellThreeAttack";
            public readonly static string OnSpellXPAttack = "UIBattleEvent.OnSpellXPAttack";
            public readonly static string OnPowerChargeStart = "UIBattleEvent.OnPowerChargeStart";
            public readonly static string OnPowerChargeComplete = "UIBattleEvent.OnPowerChargeComplete";
            public readonly static string OnPowerChargeInterrupt = "UIBattleEvent.OnPowerChargeInterrupt";
            public readonly static string OnResetPowerCharge = "UIBattleEvent.OnResetPowerCharge";
            public readonly static string OnUseItem = "UIBattleEvent.OnUseItem";

            public readonly static string OnFlushBossBlood = "UIBattleEvent.OnFlushBossBlood";
            public readonly static string OnFlushMercenaryBlood = "UIBattleEvent.OnFlushMercenaryBlood";

            public readonly static string OnSpriteSkill = "UIBattleEvent.OnSpriteSkill";
        }

        // 帐号UI操作事件 
        static public class UIAccountEvent
        {
            public readonly static string OnLogin = "UIAccountEvent.OnLogin";
            public readonly static string OnCreateCharacter = "UIAccountEvent.OnCreateCharacter";
            public readonly static string OnDelCharacter = "UIAccountEvent.DelCharacter";
            public readonly static string OnStartGame = "UIAccountEvent.OnStartGame";
            public readonly static string OnChooseServer = "UIChooseServerEvent.OnChooseServer";
            public readonly static string OnChangeServer = "UIChooseServerEvent.OnChangeServer";
            public readonly static string OnGetRandomName = "UIChooseServerEvent.OnGetRandomName";
        }

        // OtherEvent
        static public class OtherEvent
        {
            public readonly static string OnEvent1 = "OtherEvent.OnEvent1";
            public readonly static string OnEvent2 = "OtherEvent.OnEvent2";
            public readonly static string OnEvent3 = "OtherEvent.OnEvent3";

            public readonly static string OnThink = "OnThink";
            public readonly static string CallTeammate = "CallTeammate";
            public readonly static string OnChangeWeapon = "OtherEvent.OnChangeWeapon";

            public readonly static string MainCameraComplete = "OtherEvent.MainCameraComplete";
            public readonly static string MapIdChanged = "OtherEvent.MapIdChanged";
            public readonly static string ChangeDummyRate = "OtherEvent.ChangeDummyRate";
            public readonly static string ResetDummyRate = "OtherEvent.ResetDummyRate";
            public readonly static string ClientGM = "OtherEvent.ClientGM";
            public readonly static string SecondPast = "OtherEvent.OneSecondPast";
            public readonly static string Withdraw = "OtherEvent.Withdraw";
            public readonly static string DiamondMine = "OtherEvent.DiamondMine";
            public readonly static string CheckCharge = "OtherEvent.CheckCharge";
            public readonly static string BossDie = "OtherEvent.BossDie";
            public readonly static string Charge = "OtherEvent.Charge";
        }

        static public class TaskEvent
        {
            public readonly static string NPCInSight = "TaskEvent.NPCInSight";
            public readonly static string CloseToNPC = "TaskEvent.CloseToNPC";
            public readonly static string LevelWin = "TaskEvent.LevelWin";
            public readonly static string GuideDone = "TaskEvent.GuidDone";
            public readonly static string LeaveFromNPC = "TaskEvent.LeaveFromNPC";

            public readonly static string AcceptTask = "TaskEvent.AcceptTask";
            public readonly static string TalkEnd = "TaskEvent.TalkEnd";
            public readonly static string ShowRewardEnd = "TaskEvent.ShowRewardEnd";

            public readonly static string GoToNextTask = "TaskEvent.GoToNextTask";
            public readonly static string AcceptNewTask = "TaskEvent.AcceptNewTask";

            public readonly static string NPCSetSign = "TaskEvent.OnNPCSetSign";

            public readonly static string CheckNpcInRange = "TaskEvent.CheckNpcInRange";
        }

        /// <summary>
        /// 副本事件
        /// </summary>
        static public class InstanceEvent
        {
            public readonly static string UpdateMissionMessage = "InstanceEvent.UpdateMissionMessage";

            public readonly static string UpdateEnterableMissions = "InstanceEvent.UpdateFinishedMissions";
            public readonly static string UpdateMissionTimes = "InstanceEvent.UpdateMissionTimes";
            public readonly static string UpdateMissionStars = "InstanceEvent.UpdateMissionStars";
            public readonly static string UpdateMap = "InstanceEvent.UpdateMap";
            public readonly static string InstanceSelected = "InstanceEvent.InstanceSelected";
            public readonly static string BeforeInstanceLoaded = "InstanceEvent.BeforeInstanceLoaded";
            public readonly static string InstanceLoaded = "InstanceEvent.InstanceLoaded";
            public readonly static string InstanceUnLoaded = "InstanceEvent.InstanceUnLoaded";
            public readonly static string MissionStart = "InstanceEvent.MissionStart";
            public readonly static string ReturnHome = "InstanceEvent.ReturnHome";
            public readonly static string WinReturnHome = "InstanceEvent.WinReturnHome";
            public readonly static string ResetMission = "InstanceEvent.ResetMission";

            public readonly static string SpawnPointStart = "InstanceEvent.SpawnPointStart";

            public readonly static string NotReborn = "InstanceEvent.NotReborn";
            public readonly static string Reborn = "InstanceEvent.Reborn";

            public readonly static string GetCurrentReward = "InstanceEvent.GetCurrentReward";
            public readonly static string GetMercenaryInfo = "InstanceEvent.GetMercenaryInfo";
            public readonly static string AddFriendDegree = "InstanceEvent.AddFriendDegree";

            public readonly static string UploadMaxCombo = "InstanceEvent.UploadMaxCombo";      

            public readonly static string SweepMission = "InstanceEvent.SweepMission";
            public readonly static string GetSweepMissionList = "InstanceEvent.GetSweepMissionList";
            public readonly static string GetSweepTimes = "InstanceEvent.GetSweepTimes";
            public readonly static string GetChestReward = "InstanceEvent.GetChestReward";

            public readonly static string StopAutoFight = "InstanceEvent.StopAutoFight";

            public readonly static string GetBossChestRewardReq = "InstanceEvent.GetBossChestRewardReq";

            public readonly static string EnterRandomMission = "InstanceEvent.EnterRandomMission";
        }

        /// <summary>
        /// 机关事件
        /// </summary>
        static public class GearEvent
        {
            public readonly static string LoadEnd = "GearEvent.LoadEnd";

            public readonly static string SetGearEnable = "GearEvent.SetGearEnable";
            public readonly static string SetGearDisable = "GearEvent.SetGearDisable";
            public readonly static string SetGearStateOne = "GearEvent.SetGearStateOne";
            public readonly static string SetGearStateTwo = "GearEvent.SetGearStateTwo";

            public readonly static string SetGearEventEnable = "GearEvent.SetGearEventEnable";
            public readonly static string SetGearEventDisable = "GearEvent.SetGearEventDisable";
            public readonly static string SetGearEventStateOne = "GearEvent.SetGearEventStateOne";
            public readonly static string SetGearEventStateTwo = "GearEvent.SetGearEventStateTwo";

            public readonly static string FlushGearState = "GearEvent.FlushGearState";

            public readonly static string UploadAllGear = "GearEvent.UploadAllGear";
            public readonly static string DownloadAllGear = "GearEvent.DownLoadALLGear";

            public readonly static string SwitchLightMapFog = "GearEvent.SwitchLightMapFog";
            public readonly static string Teleport = "GearEvent.Teleport";
            public readonly static string Damage = "GearEvent.Damage";
            public readonly static string SpawnPointDead = "GearEvent.SpawnPointDead";

            public readonly static string MotorHandleEnd = "GearEvent.MotorHandleEnd";
            public readonly static string TrapBegin = "GearEvent.TrapBegin";
            public readonly static string TrapEnd = "GearEvent.TrapEnd";
            public readonly static string LiftEnter = "GearEvent.LiftEnter";
            public readonly static string PathPointTrigger = "GearEvent.PathPointTrigger";

            public readonly static string CrockBroken = "GearEvent.CrockBroken";
            public readonly static string ChestBroken = "GearEvent.ChestBroken";

            public readonly static string CongealMagma = "GearEvent.CongealMagma";
        }

        static public class NPCEvent
        {
            public readonly static string FrushIcon = "NPCEvent.FrushIcon";
            public readonly static string TurnToPlayer = "NPCEvent.TurnToPlayer";
            public readonly static string TalkEnd = "NPCEvent.TalkEnd";
        }

        /// <summary>
        /// 副本UI事件
        /// </summary>
        static public class InstanceUIEvent
        {
            public readonly static string UpdateMapName = "InstanceUIEvent.UpdateMap";

            public readonly static string UpdateMissionEnable = "InstanceUIEvent.UpdateGridEnable";
            public readonly static string UpdateMissionName = "InstanceUIEvent.UpdateMissionName";
            public readonly static string UpdateMissionStar = "InstanceUIEvent.UpdateMissionStar";

            public readonly static string UpdateLevelEnable = "InstanceUIEvent.UpdateLevelEnable";
            public readonly static string UpdateLevelTime = "InstanceUIEvent.UpdateLevelTime";

            public readonly static string UpdateLevelStar = "InstanceUIEvent.UpdateLevelStar";

            public readonly static string CheckMissionTimes = "InstanceUIEvent.CheckMissionTimes";

            public readonly static string GetDrops = "InstanceUIEvent.GetDrops";

            public readonly static string GetChestRewardGotMessage = "InstanceUIEvent.GetChestRewardGotMessage";

            public readonly static string ShowResetMissionWindow = "InstanceUIEvent.ShowResetMissionWindow";

            public readonly static string UpdateMercenaryButton = "InstanceUIEvent.UpdateMercenaryButton";

            public readonly static string FlipCard = "InstanceUIEvent.FlipCard";
            public readonly static string FlipRestCard = "InstanceUIEvent.FlipRestCard";
            public readonly static string AutoFlipCard = "InstanceUIEvent.AutoFlipCard";
            public readonly static string AutoFlipRestCard = "InstanceUIEvent.AutoFlipRestCard";

            public readonly static string UpdateLevelRecord = "InstanceUIEvent.UpdateLevelRecord";
            public readonly static string GetBossChestRewardGotMessage = "InstanceUIEvent.GetBossChestRewardGotMessage";

            public readonly static string UpdateChestMessage = "InstanceUIEvent.UpdateChestMessage";
            public readonly static string UpdateBossChestMessage = "InstanceUIEvent.UpdateBossChestMessage";

            public readonly static string ShowCard = "InstanceUIEvent.ShowCard";

            public readonly static string GetFoggyAbyssMessage = "InstanceUIEvent.GetFoggyAbyssMessage";
            public readonly static string EnterFoggyAbyss = "InstanceUIEvent.EnterFoggyAbyss";
        }

        static public class RuneEvent
        {
            public readonly static string GetRuneBag = "RuneEvent.GetRuneBag";
            public readonly static string GetBodyRunes = "RuneEvent.GetBodyRunes";
            public readonly static string GameMoneyRefresh = "RuneEvent.GameMoneyRefresh";
            public readonly static string FullRefresh = "RuneEvent.FullRefresh";
            public readonly static string RMBRefresh = "RuneEvent.RMBRefresh";
            public readonly static string AutoCombine = "RuneEvent.AutoCombine";
            public readonly static string AutoPickUp = "RuneEvent.AutoPickUp";
            public readonly static string UseRune = "RuneEvent.UseRune";
            public readonly static string PutOn = "RuneEvent.PutOn";
            public readonly static string PutDown = "RuneEvent.PutDown";
            public readonly static string ChangeIndex = "RuneEvent.ChangeIndex";
            public readonly static string ChangePosi = "RuneEvent.ChangePosi";
            public readonly static string ShowTips = "RuneEvent.ShowTips";
            public readonly static string CloseDragon = "RuneEvent.CloseDragon";
        }

        static public class TowerEvent
        {
            public readonly static string EnterMap = "TowerEvent.EnterMap";
            public readonly static string NormalSweep = "TowerEvent.NormalSweep";
            public readonly static string VIPSweep = "TowerEvent.VIPSweep";
            public readonly static string SweepAll = "TowerEvent.SweepAll";
            public readonly static string GetInfo = "TowerEvent.GetInfo";
            public readonly static string CreateDoor = "TowerEvent.CreateDoor";
            public readonly static string FinishSingle = "TowerEvent.FinishSingle";
            public readonly static string ClearCD = "TowerEvent.ClearCD";
            //public readonly static string 
        }
        static public class StoryEvent
        {
            public readonly static string CGBegin = "StoryEvent.CGBegin";
            public readonly static string CGEnd = "StoryEvent.CGEnd";
        }

        static public class CommandEvent
        {
            public readonly static string CommandEnd = "CommandEvent.CommandEnd";
        }

        static public class ChallengeUIEvent
        {
            public readonly static string Enter = "ChallengeUIEvent.Enter";
            public readonly static string GetOgreMustDieTime = "ChallengeUIEvent.GetOgreMustDieTime";

            public readonly static string CollectChallengeState = "ChallengeUIEvent.CollectChallengeState";
            public readonly static string ReceiveChallengeUIGridMessage = "ChallengeUIEvent.ReceiveChallengeUIGridMessage";
            public readonly static string FlushChallengeUIGridSortedResult = "ChallengeUIEvent.FlushChallengeUIGridSortedResult";
        }

        static public class NormalMainUIEvent
        {
            public readonly static string ShowChallegeIconTip = "NormalMainUIEvent.ShowChallegeIconTip";
            public readonly static string HideChallegeIconTip = "NormalMainUIEvent.HideChallegeIconTip";
            public readonly static string ShowArenaIconTip = "NormalMainUIEvent.ShowArenaIconTip";
            public readonly static string HideArenaIconTip = "NormalMainUIEvent.HideArenaIconTip";
            public readonly static string ShowMallConsumeIconTip = "NormalMainUIEvent.ShowMallConsumeIconTip";
        }

        static public class SpellEvent
        {
            public readonly static string OpenView = "SpellEvent.OpenView";
            public readonly static string SelectGroup = "SpellEvent.SelectGroup";
            public readonly static string SelectLevel = "SpellEvent.SelectLevel";
            public readonly static string Study = "SpellEvent.Study";
        }

        static public class AssistantEvent
        {
            public readonly static string SkillGridDragToBodyGrid = "AssistantUISkillGridDragToBodyGrid";
            public readonly static string MintmarkGridDragToBodyGrid = "AssistantUIMintmarkGridDragToBodyGrid";
            public readonly static string LevelUpSkillResp = "AssistantUILevelUpSkillResp";
            public readonly static string LevelUpMarkResp = "AssistantUILevelUpMarkResp";
            public readonly static string ClientDragSkillResp = "AssistantUIClientDragSkillResp";
            public readonly static string ClientDragMarkResp = "AssistantUIClientDragMarkResp";
            public readonly static string SkillGridDragOutside = "AssistantUISkillGridDragOutside";
            public readonly static string MintmarkGridDragOutside = "AssistantUIMintmarkGridDragOutside";
            public readonly static string SkillGridDragBegin = "AssistantUISkillGridDragBegin";
            public readonly static string MintmarkGridDragBegin = "AssistantUIMintmarkGridDragBegin";
            public readonly static string PropRefreshResp = "AssistantUIPropRefreshResp";
        }

        static public class OperationEvent
        {
            public readonly static string Charge = "OperationEvent.Charge";
            public readonly static string ChargeGetReward = "OperationEvent.ChargeGetReward";
            public readonly static string EventGetReward = "OperationEvent.EventGetReward";
            public readonly static string EventShareToGetDiamond = "OperationEvent.EventShareToGetDiamond";
            public readonly static string LogInGetReward = "OperationEvent.LogInGetReward";
            public readonly static string LogInBuy = "OperationEvent.LogInBuy";
            public readonly static string AchievementGetReward = "OperationEvent.AchievementGetReward";
            public readonly static string AchievementShareToGetDiamond = "OperationEvent.AchievementShareToGetDiamond";

            public readonly static string GetChargeRewardMessage = "OperationEvent.GetChargeRewardMessage";
            public readonly static string GetActivityMessage = "OperationEvent.GetActivityMessage";
            public readonly static string GetLoginMessage = "OperationEvent.GetLoginMessage";
            public readonly static string GetAchievementMessage = "OperationEvent.GetAchievementMessage";

            public readonly static string CheckEventOpen = "OperationEvent.CheckEventOpen";

            public readonly static string FlushCharge = "OperationEvent.FlushCharge";

            public readonly static string CheckFirstShow = "OperationEvent.CheckFirstShow";

            public readonly static string GetAllActivity = "OperationEvent.GetAllActivity";

            public readonly static string EventTimesUp = "OperationEvent.EventTimesUp";

            public readonly static string GetLoginMarket = "OperationEvent.GetLoginMarket";
        }

        static public class AIEvent
        {
            public readonly static string DummyThink = "AIEvent.DummyThink";
            public readonly static string DummyStiffEnd = "AIEvent.DummyStiffEnd";
            public readonly static string ProcessBossDie = "AIEvent.ProcessBossDie";
            public readonly static string SomeOneDie = "AIEvent.ProcessSomeOneDie";
            public readonly static string WarnOtherSpawnPointEntities = "AIEvent.WarnOtherSpawnPointEntities";
            
        }
        static public class SanctuaryEvent
        {
            public readonly static string RefreshRank = "SanctuaryEvent.RefreshRank";
            public readonly static string RefreshMyInfo = "SanctuaryEvent.RefreshMyInfo";
            public readonly static string EnterSanctuary = "SanctuaryEvent.EnterSanctuary";
            public readonly static string BuyExtraTime = "SanctuaryEvent.BuyExtraTime";
            public readonly static string CanBuyExtraTime = "SanctuaryEvent.CanBuyExtraTime";
            public readonly static string QuerySanctuaryInfo = "SanctuaryEvent.QuerySanctuaryInfo";
        }
        static public class ArenaEvent
        {
            public readonly static string RefreshWeak = "ArenaEvent.RefreshWeak";
            public readonly static string RefreshStrong = "ArenaEvent.RefreshStrong";
            public readonly static string RefreshRevenge = "ArenaEvent.RefreshRevenge";
            public readonly static string RefreshArenaData = "ArenaEvent.RefreshArenaData";
            public readonly static string EnterArena = "ArenaEvent.EnterArena";
            public readonly static string Challenge = "ArenaEvent.Challenge";
            public readonly static string ClearArenaCD = "ArenaEvent.ClearArenaCD";
            public readonly static string AddArenaTimes = "ArenaEvent.AddArenaTimes";
            public readonly static string GetArenaRewardInfo = "ArenaEvent.GetArenaRewardInfo";
            public readonly static string GetArenaReward = "ArenaEvent.GetArenaReward";
            public readonly static string TabSwitch = "ArenaEvent.TabSwitch";
        }
        static public class ComboEvent
        {
            public readonly static string AddCombo = "ComboEnent.AddCombo";
            public readonly static string ResetCombo = "ComboEnent.ResetCombo";
        }

        static public class DailyTaskEvent
        {
            public readonly static string ShowDailyEvent = "DailyTaskEvent.ShowDailyEvent";
            public readonly static string GetDailyEventReward = "DailyTaskEvent.GetDailyEventReward";
            public readonly static string GetDailyEventData = "DailyTaskEvent.GetDailyEventData";
            public readonly static String OpenDailyTaskUI = "DailyTaskSystemController.OpenDailyTaskUI";
            public readonly static String DailyTaskJumpToOtherUI = "DailyTaskSystemController.DailyTaskJumpToOtherUI";
        }

        static public class EnergyEvent
        {
            public readonly static string BuyEnergy = "EnergyEvent.BuyEnergy";
            public readonly static string UpdateVipLevel = "EnergyEvent.UpdateVipLevel";
        }

        static public class EquipmentEvent
        {
            public readonly static string SetEquipmentUICloseValueZ = "EquipmentEvent.SetEquipmentUICloseValueZ";
        }

        static public class DirecterEvent
        {
            public readonly static string DirActive = "DirecterEvent.DirActive";
        }

        static public class DiamondToGoldEvent
        {
            public readonly static string GoldMetallurgy = "DiamondToGoldEvent.GoldMetallurgy";
        }

        public static class LocalServerEvent
        {
            public readonly static string ExitMission = "LocalServerEvent.ExitMission";
            public readonly static string SummonToken = "LocalServerEvent.SummonToken";
        }

        public static class CampaignEvent
        {
            public readonly static string JoinCampaign = "CampaignEvent.JoinCampaign";
            public readonly static string LeaveCampaign = "CampaignEvent.LeaveCampaign";

            public readonly static string MatchCampaign = "CampaignEvent.MatchCampaign";

            public readonly static string ExitCampaign = "CampaignEvent.ExitCampaign";

            public readonly static string GetCampaignLeftTimes = "CampaignEvent.GetCampaignLeftTimes";

            public readonly static string GetCampaignLastTime = "CampaignEvent.GetCampaignLastTime";

            public readonly static string SetPlayerMessage = "CampaignEvent.SetPlayerMessage";
            public readonly static string FlushPlayerBlood = "CampaignEvent.FlushPlayerBlood";
            public readonly static string RemovePlayerMessage = "CampaignEvent.RemovePlayerMessage";

            public readonly static string CrystalAttacked = "CampaignEvent.CrystalAttacked";
        }

        public static class LogicSoundEvent
        {
            public readonly static string OnHitYelling = "LogicSoundEvent.OnHitYelling";

        }

        public static class WingEvent
        {
            public readonly static string Open = "WingEvent.Open";
            public readonly static string Close = "WingEvent.Close";
            public readonly static string Buy = "WingEvent.Buy";
            public readonly static string Upgrade = "WingEvent.Upgrade";
            public readonly static string Active = "WingEvent.Active";
            public readonly static string PutOn = "WingEvent.PutOn";
            public readonly static string Undo = "WingEvent.Undo";
            public readonly static string UnLock = "WingEvent.UnLock";
            public readonly static string CommonWing = "WingEvent.CommonWing";
            public readonly static string MagicWing = "WingEvent.MagicWing";
            public readonly static string OpenTip = "WingEvent.OpenTip";
            public readonly static string OpenBuy = "WingEvent.OpenBuy";
            public readonly static string OpenUpgrade = "WingEvent.OpenUpgrade";
            public readonly static string TipBuyClick = "WingEvent.TipBuyClick";
            public readonly static string ClosePreview = "WingEvent.ClosePreview";
        }

        /// <summary>
        /// PVP事件
        /// </summary>
        public static class OccupyTowerEvent
        {
            public readonly static string GetOccupyTowerStatePoint = "OccupyTowerEvent.GetOccupyTowerStatePoint";

            public readonly static string JoinOccupyTower = "OccupyTowerEvent.JoinOccupyTower";
            public readonly static string LeaveOccupyTower = "OccupyTowerEvent.LeaveOccupyTower";

            public readonly static string ExitOccupyTower = "OccupyTowerEvent.ExitOccupyTower";

            public readonly static string SetOccupyTowerUIScorePoint = "OccupyTowerEvent.SetOccupyTowerUIScorePoint";
        }

        public static class FoggyAbyssEvent
        {
            public readonly static string FoggyAbyssOpen = "FoggyAbyssEvent.FoggyAbyssOpen";
            public readonly static string FoggyAbyssClose = "FoggyAbyssEvent.FoggyAbyssClose";
        }

        public static class RewardEvent
        {
            public readonly static string OpenReward = "RewardEvent.OpenReward";
            public readonly static string WingIcon = "RewardEvent.WingIcon";
            public readonly static string ChargeReward = "RewardEvent.ChargeReward";
            public readonly static string ElfDiamond = "RewardEvent.ElfDiamond";
            public readonly static string LoginReward = "RewardEvent.LoginReward";
            public readonly static string GetLoginReward = "RewardEvent.GetLoginReward";
            public readonly static string SelectReward = "RewardEvent.SelectReward";
            public readonly static string GetChargeReward = "RewardEvent.GetChargeReward";
        }

        public static class MogoGlobleUIManagerEvent
        {
            public readonly static String ShowWaitingTip = "MogoGlobleUIManager.ShowWaitingTip";
        }

        public static class MogoUIManagerEvent
        {
            public readonly static String SetCurrentUI = "MogoUIManager.SetCurrentUI";
            public readonly static String ShowInstanceMissionChooseUI = "MogoUIManager.ShowInstanceMissionChooseUI";
            public readonly static String SwitchStrenthUI = "MogoUIManager.SwitchStrenthUI";
            public readonly static String ShowDiamondToGoldUI = "MogoUIManager.ShowDiamondToGoldUI";
            public readonly static String ShowEnergyUI = "MogoUIManager.ShowEnergyUI";
            public readonly static String SwitchToMarket = "MogoUIManager.SwitchToMarket"; 
        }

        public static class NormalMainUIViewManagerEvent
        {
            public readonly static String PVEPLAYICONUP = "NormalMainUIViewManager.PVEPLAYICONUP";
            public readonly static String PVPPLAYICONUP = "NormalMainUIViewManager.PVPPLAYICONUP"; 
        }

        public static class MFUIManagerEvent
        {
            public readonly static String SwitchUIWithLoad = "MFUIManager.SwitchUIWithLoad";
        }

        public static class ComposeManagerEvent
        {
            public readonly static String SwitchToCompose = "ComposeManager.SwitchToCompose";
        }

        public static class IAPConsumeEvent
        {
            public readonly static String OpenIAPConsumeUI = "IAPConsumeUIController.OpenIAPConsumeUI";
        }

        public static class MonsterEvent
        {
            public readonly static String TowerDamage = "MonsterEvent.TowerDamage";
        }
    }
}


