using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.GameData;
using Mogo.Util;
using TDBID = System.UInt64;

namespace Mogo.Game
{
    public partial class EntityMyself
    {
        #region RPC方法回调

        #region 套装激活
        public void ActivedSuitEquipmentResp(byte errorId, uint typeId, byte bagType)
        {
            //Debug.LogError("ActivedSuitEquipmentResp ,errorId:" + errorId + ",typeId:" + typeId + ",bagType:" + bagType);
            inventoryManager.ActivedSuitEquipmentResp(errorId, typeId, bagType);
        }
        #endregion

        #region 附魔系统
        public void GetFumoInfoResp(byte slot, LuaTable info)
        {
            fumoManager.GetFumoInfoResp(slot, info);
        }

        public void fumoResp(byte slot, byte errorId)
        {
            fumoManager.FumoResp(slot, errorId);
        }

        public void fumo_replaceResp(byte slot, byte index, byte errorId)
        {
            fumoManager.FumoReplaceResp(slot, index, errorId);
        }

        #endregion

        #region 飞龙比赛

        public void DragonStatusResp(byte hasReward, byte isConvoying, byte leftTime)
        {
            DragonMatchManager.Instance.DragonStatusResp(hasReward, isConvoying, leftTime);
        }

        public void ExploreDragonEventResp(byte errorId, byte eventId)
        {
            DragonMatchManager.Instance.ExploreDragonEventResp(errorId, eventId);
        }


        public void DragonConvoyResp(byte errorId, uint timeLeft)
        {
            DragonMatchManager.Instance.DragonConvoyResp(errorId, timeLeft);
        }

        public void RemainConvoyTimesResp(byte times)
        {
            DragonMatchManager.Instance.RemainConvoyTimesResp(times);
        }

        public void DragonInfoResp(LuaTable data)
        {
            DragonMatchManager.Instance.OnMatchDataResp(data);
        }
        public void EventListAvatarNameResp(LuaTable data)
        {
            DragonMatchManager.Instance.EventListAvatarNameResp(data);
        }
        public void DragonCvyRewardResp(byte errorId)
        {
            DragonMatchManager.Instance.DragonCvyRewardResp(errorId);
        }

        public void FreshConvoyRewardResp(uint exp, uint gold)
        {
            DragonMatchManager.Instance.FreshConvoyRewardResp(exp, gold);
        }
        public void DragonAttackResp(byte errorId)
        {
            DragonMatchManager.Instance.DragonAttackResp(errorId);
        }

        public void DragonRelatedResp(byte errorId)
        {
            DragonMatchManager.Instance.DragonRelatedResp(errorId);
        }

        public void FreshDragonQualityResp(byte errorId, byte quality)
        {
            DragonMatchManager.Instance.OnUpgradeDragonResp(errorId, quality);
        }

        public void BuyAtkTimesResp(byte errorId)
        {
            DragonMatchManager.Instance.BuyAtkTimesResp(errorId);
        }

        public void ClearAtkCdResp(byte errorId)
        {
            DragonMatchManager.Instance.ClearAtkCdResp(errorId);
        }



        #endregion

        #region 激活码
        private void AddGiftBagResp(uint id, uint errorId)
        {
            MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
            switch (errorId)
            {
                case 0:
                    break;
                case 16:
                case 17:
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(50007));
                    break;
                case 18:
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(50006));
                    break;
            }
        }
        #endregion

        #region 公告板

        private void Billboard(LuaTable items)
        {
            List<MogoNotice2.Notice> list;

            if (Utils.ParseLuaTable(items, out list))
            {
                MogoNotice2.Instance.ShowNotice(list);
            }

        }
        #endregion

        #region 湮灭之门

        private void OblivionGateSpread(uint copyId)
        {
            Mogo.Util.LoggerHelper.Debug("OblivionGateSpread:" + copyId);
            DoorOfBurySystem.Instance.ShowOpenToFriendTip();
        }

        private void OblivionGateCreate(uint copyId, int source)
        {
            Mogo.Util.LoggerHelper.Debug("OblivionGateCreate:" + copyId);
            DoorOfBurySystem.Instance.OblivionGateCreate(copyId, source);
        }

        private void OblivionGateListResp(uint cd, LuaTable luaTable)
        {
            Mogo.Util.LoggerHelper.Debug("OblivionGateListResp");
            DoorOfBurySystem.Instance.OblivionGateListResp(cd, luaTable);
        }

        private void OblivionAwardResp(uint damage, uint exp, uint gold)
        {
            Mogo.Util.LoggerHelper.Debug("OblivionAwardResp");
            DoorOfBurySystem.Instance.ShowRecordTip(damage, gold, exp);
        }

        private void OblivionQueryStateResp(uint cd, byte hasDoor)
        {
            //Mogo.Util.LoggerHelper.Debug("OblivionQueryStateResp");
            DoorOfBurySystem.Instance.OnDoorOfBuryCdResp((int)cd, hasDoor == 1);
        }

        #endregion

        #region 任务系统
        public void TaskCompleteResp(uint taskID)
        {
            // 如果是副本任务，设置延时，否则会在副本内弹出
            if (taskManager.PlayerCurrentTask.conditionType == 1)
                taskManager.delayHandleTaskID = (int)taskID;
            else
                taskManager.CheckTaskRewardShow();
        }

        public void TaskErrorResp(byte msg)
        {
            Mogo.Util.LoggerHelper.Debug("Msg: " + msg);
            return;
        }
        #endregion

        #region 关卡系统
        private void MissionResp(byte msg, LuaTable luaTable)
        {
            missionManager.MissionResp(msg, luaTable);
        }

        private void MercenaryInfoResp(LuaTable luaTable)
        {
            LoggerHelper.Debug("MercenaryInfoResp");

            missionManager.OnSetMercenaryInfo(luaTable);
        }

        public void UseHpBottle()
        {
            if (hpCount > 0)
            {
                MogoWorld.thePlayer.RpcCall("CliEntityActionReq", ID, (uint)2, curHp, (uint)0);
                if (MogoWorld.IsClientMission)
                {
                    buffManager.ClientAddBuff(1);
                    UseHpBottleResp(0);
                    hpCount--;
                    return;
                }
                this.RpcCall("UseHpBottleReq", 1);
            }
            else
            {
                if (MogoWorld.thePlayer.buyCount >= PrivilegeData.dataMap.Get(VipLevel).hpMaxCount)
                {
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap.Get(515).content);
                }
                else
                {
                    if (MogoWorld.IsClientMission)
                    {
                        return;
                    }
                    MogoMessageBox.Confirm(LanguageData.GetContent(516, PriceListData.GetPrice(1, MogoWorld.thePlayer.buyCount)), (flag) =>
                    {
                        if (flag)
                            this.RpcCall("BuyHpBottleReq", 1);
                    });
                }

            }
        }

        public void UseHpBottleResp(byte errID)
        {
            if (errID == 0)
            {
                MainUIViewManager.Instance.HpBottleCD = HpTypesData.GetHpBottleCD(MogoWorld.thePlayer.VipLevel);
            }
        }

        #endregion

        #region 强化系统
        /// <summary>
        /// 返回身体各个部位的level
        /// </summary>
        /// <param name="_info"></param>
        /// <param name="errorId"></param>
        private void OnBodyEnhaLevResp(LuaTable _info, byte errorId)
        {
            bodyenhanceManager.OnBodyEnhaLevResp(_info, errorId);
        }

        /// <summary>
        /// 返回升级身体某个部位
        /// </summary>
        /// <param name="errorId"></param>
        private void OnBodyEnhaUpgResp(byte errorId, LuaTable table)
        {
            //Mogo.Util.LoggerHelper.Debug();
            //LoggerHelper.Error(table);
            Dictionary<int, int> dic = new Dictionary<int, int>();
            Utils.ParseLuaTable(table, out dic);
            int num = 0;
            string name = string.Empty;
            foreach (KeyValuePair<int, int> pair in dic)
            {
                //Mogo.Util.LoggerHelper.Debug(pair.Key + "," + pair.Value);
                num = pair.Value;
                name = ItemMaterialData.GetItem(pair.Key).Name;
                break;
            }
            bodyenhanceManager.OnBodyEnhaUpgResp(errorId, num, name);
        }

        /// <summary>
        /// 返回身体某个部位对应等级所有信息
        /// </summary>
        /// <param name="_info">所需物品id，num</param>
        /// <param name="errorId">错误id</param>
        private void OnBodyEnhaInfoResp(LuaTable _info, byte errorId)
        {
        }

        /// <summary>
        /// 返回身体某个部位产生的属性影响
        /// </summary>
        /// <param name="_info">体属性name， val</param>
        /// <param name="errorId">错误id</param>
        private void OnBodyEnhaPropResp(LuaTable _info, byte errorId)
        {
        }
        #endregion

        #region 试炼之塔
        private void TowerResp(byte msgID, LuaTable info)
        {
            towerManager.TowerResp(msgID, info);
        }
        #endregion

        #region 圣域守卫战
        //圣域守卫战开启
        private void OnSanctuaryStartResp()
        {
            if (MogoWorld.thePlayer.level >= SystemRequestLevel.SANCTUARY)
            {
                NormalMainUIViewManager.Instance.ShowSanctuaryOpenTip();
                EventDispatcher.TriggerEvent<int>("NormalMainUIEvent.ShowSanctuaryOpenTip", (int)ChallengeGridID.Sanctuary);
            }
        }

        //下次开启时间，正在开启，返回0
        private void SanctuaryDefenseTimeResp(uint nextTimeSec, byte canEnterTime, uint endTime)
        {
            LoggerHelper.Debug("SanctuaryDefenseTimeResp:" + nextTimeSec + ":" + canEnterTime);
            ChallengeUILogicManager.Instance.nextTimeSec = nextTimeSec;
            ChallengeUILogicManager.Instance.canEnterTime = canEnterTime;
            ChallengeUILogicManager.Instance.endTime = endTime;
            ChallengeUILogicManager.Instance.RefreshUI((int)ChallengeGridID.Sanctuary);
        }

        //申请加入圣域守卫战返回
        private void EnterSanctuaryDefenseResp(byte errID)
        {
            //NormalMainUIViewManager.Instance.ShowContributeTip();
        }

        //圣域守卫战该玩家信息
        private void SanctuaryDefenseMyInfoResp(uint dayContri, uint weekContri, uint weekLevel, LuaTable playerInfo)
        {
            Debug.LogError(playerInfo);
            List<int> data;
            //weekContribution, dayContribution(当天没参加置0), nextLvNeedContribution(week), canEnterTime
            if (Utils.ParseLuaTable(playerInfo, out data))
            {
                Debug.LogError(data);
                SanctuaryUILogicManager.Instance.dayContri = dayContri;
                SanctuaryUILogicManager.Instance.weekContri = weekContri;
                SanctuaryUILogicManager.Instance.weekLevel = weekLevel;
                SanctuaryUILogicManager.Instance.alreadyGetList = data;
                MogoUIManager.Instance.ShowMogoSanctuaryUI((() => { SanctuaryUILogicManager.Instance.RefreshUI(0); }));

            }
        }

        //排行榜申请返回
        private void SanctuaryDefenseRankResp(LuaTable week, LuaTable day, uint myWeek, uint myDay)
        {
            List<SanctuaryRankData> weekData;
            List<SanctuaryRankData> dayData;
            if (Utils.ParseLuaTable(week, out weekData))
            {
                SanctuaryUILogicManager.Instance.weekData = weekData;
            }
            else { LoggerHelper.Debug("parse 1 error"); }
            if (Utils.ParseLuaTable(day, out dayData))
            {
                SanctuaryUILogicManager.Instance.dayData = dayData;
            }
            else { LoggerHelper.Debug("parse 2 error"); }
            SanctuaryUILogicManager.Instance.MyDay = myDay;
            SanctuaryUILogicManager.Instance.MyWeek = myWeek;
        }
        //战斗时更新定时更新贡献排名
        private void OnRankingListUpdateResp(LuaTable rank)
        {
            LoggerHelper.Debug("OnRankingListUpdateResp" + rank.ToString());
            List<SanctuaryRankData> battleData;
            if (Utils.ParseLuaTable(rank, out battleData))
            {
                SanctuaryUILogicManager.Instance.battleData = battleData;
                SanctuaryUILogicManager.Instance.RefreshBattleUI();
            }
        }

        private void CanBuySanctuaryDefenseTimeResp(byte errID, uint diamond, uint gold)
        {
            switch (errID)
            {
                case 0:
                    {
                        MogoMessageBox.Confirm(LanguageData.GetContent(24002, diamond), (flag) =>
                        {
                            if (flag)
                                EventDispatcher.TriggerEvent(Events.SanctuaryEvent.BuyExtraTime);
                        });
                    }
                    break;
                case 1:
                    break;
                case 2:
                    {
                        MogoMessageBox.Confirm(LanguageData.GetContent(24001), (flag) => { });
                    }
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }

        public void SetSanctuaryDefenseRewardData(SanctuaryRewardData rewardData, uint week)
        {
            // 设置当前金币奖励
            NormalMainUIViewManager.Instance.SetContributeTipCurrentRewardNum(rewardData.gold);
            // 设置下级奖励还需贡献
            NormalMainUIViewManager.Instance.SetContributeTipNextRewardConditionNum(SanctuaryRewardXMLData.GetAccuNextRankContribution(rewardData.contribution) - rewardData.contribution);
            // 设置下级金币奖励
            NormalMainUIViewManager.Instance.SetContributeNextRewardNum(SanctuaryRewardXMLData.GetAccuNextGold(rewardData.contribution));

            NormalMainUIViewManager.Instance.SetContributeTipTitle(String.Format("贡献达到{0}", rewardData.contribution));
            NormalMainUIViewManager.Instance.SetContributeTipWeekRank(String.Format("这周排名:{0}", week));
            NormalMainUIViewManager.Instance.SetContributeTipTodayRank(String.Empty);
            NormalMainUIViewManager.Instance.ShowContributeTip(true);
        }

        public void ShowSanctuaryDefenseReward(List<SanctuaryRewardData> rewardData, uint week, int index)
        {
            SetSanctuaryDefenseRewardData(rewardData[index], week);
            NormalMainUIViewManager.Instance.CONTRIBUTETIPOKUP = (
                    () =>
                    {
                        if (index != rewardData.Count - 1)
                        {
                            ShowSanctuaryDefenseReward(rewardData, week, ++index);
                        }

                    });
        }

        public void OnSanctuaryDefenseRewardResp(LuaTable reward, uint week)
        {
            LoggerHelper.Debug("OnSanctuaryDefenseRewardResp" + reward.ToString());
            List<SanctuaryRewardData> rewardData;
            if (Utils.ParseLuaTable(reward, out rewardData))
            {
                ShowSanctuaryDefenseReward(rewardData, week, 0);
            }
        }

        #endregion

        #region 个人竞技场

        public void RefreshWeakResp(LuaTable data)
        {
            if (MogoUIManager.Instance.IsWindowOpen((int)WindowName.Arena))
            {
                ArenaPlayerData playerData;
                if (Utils.ParseLuaTable(data, out playerData))
                {
                    ArenaUILogicManager.Instance.SetArenaPlayerData(playerData, 1);
                }
            }

        }

        public void RefreshStrongResp(LuaTable data)
        {
            if (MogoUIManager.Instance.IsWindowOpen((int)WindowName.Arena))
            {
                ArenaPlayerData playerData;
                if (Utils.ParseLuaTable(data, out playerData))
                {
                    ArenaUILogicManager.Instance.SetArenaPlayerData(playerData, 2);
                }
            }
        }

        public void RefreshRevengeResp(LuaTable data)
        {
            if (MogoUIManager.Instance.IsWindowOpen((int)WindowName.Arena))
            {
                ArenaPlayerData playerData;
                if (Utils.ParseLuaTable(data, out playerData))
                {
                    ArenaUILogicManager.Instance.SetArenaPlayerData(playerData, 3);
                }
            }
        }

        public void RefreshArenaDataResp(LuaTable data)
        {
            bool isOpen = MogoUIManager.Instance.IsWindowOpen((int)WindowName.Arena);

            MogoUIManager.Instance.OpenWindow((int)WindowName.Arena,
            () =>
            {
                if (!isOpen)
                {
                    EventDispatcher.TriggerEvent(Events.ArenaEvent.EnterArena);
                }
                ArenaUILogicManager.Instance.SetDefaultPage(1);

                ArenaUILogicManager.Instance.SetAttrData();
                ArenaPersonalData personalData;
                if (Utils.ParseLuaTable(data, out personalData))
                {
                    ArenaUILogicManager.Instance.SetArenaPersonalData(personalData);

                    // 在主界面显示CD
                    if (personalData.cd > 0)
                    {
                        if (NormalMainUIViewManager.Instance != null)
                        {
                            int nextTimeSec = personalData.cd;
                            int hour = (int)nextTimeSec / 3600;
                            int minute = (int)nextTimeSec % 3600 / 60;
                            int sec = (int)nextTimeSec % 60;
                            NormalMainUIViewManager.Instance.ArenaBeginCountDown(hour, minute, sec);
                        }
                    }
                    else
                    {
                        if (NormalMainUIViewManager.Instance != null)
                        {
                            NormalMainUIViewManager.Instance.ShowArenaBtnOpen();
                        }
                    }
                }
            }
            );
        }


        public void GetArenaRewardInfoResp(LuaTable data, LuaTable reward)
        {
            //LoggerHelper.Debug(data.ToString());
            //LoggerHelper.Debug(reward.ToString());
            List<int> hasGetArenaRewardList;
            Dictionary<int, ArenaRewardData> allRewardDataList;
            if (Utils.ParseLuaTable(data, out hasGetArenaRewardList) && Utils.ParseLuaTable(reward, out allRewardDataList))
            {
                ArenaRewardUILogicManager.Instance.AddRewardUnit(hasGetArenaRewardList, allRewardDataList, () => { });
            }
        }

        /// <summary>
        /// 鼓舞Buff
        /// </summary>
        /// <param name="data"></param>
        public void RevengeBuffResp(LuaTable data)
        {

        }

        public void GetArenaRewardResp(byte errID)
        {

        }

        public void CanGetScoreRewardsResp(byte flag)
        {
            if (arenaManager != null)
            {
                arenaManager.CanGetScoreRewardsResp(flag);
            }
        }

        /// <summary>
        /// ShowRewardForms
        /// </summary>
        /// <param name="items">items</param>
        /// <param name="cd">cd:为零的时候就是不带退出</param>
        /// <param name="title">titleid</param>
        /// <param name="text">textid</param>
        /// <param name="result">0:failed,1:win</param>
        public void ShowRewardForms(LuaTable items, uint cd, uint title, uint text, byte result)
        {
            //LoggerHelper.Error(items.ToString());
            bool bWin = result == 1 ? true : false;

            Dictionary<int, int> reward;
            if (Utils.ParseLuaTable(items, out reward))
            {
                List<PassRewardGridData> list = new List<PassRewardGridData>();
                int iter = 0;
                foreach (var item in reward)
                {
                    list.Add(new PassRewardGridData()
                    {
                        id = item.Key,
                        num = item.Value,
                        imgName = ItemParentData.GetItem(item.Key).Icon,
                        iconName = ItemParentData.GetItem(item.Key).Name
                    });
                    iter++;
                }
                if (cd >= 0)
                {
                    TimerShow(cd);
                }

                uint waitSeconds = 2000;
                if (bWin &&
                    (MapData.dataMap.Get(MogoWorld.thePlayer.sceneId).type == MapType.ARENA
                    || MapData.dataMap.Get(MogoWorld.thePlayer.sceneId).type == MapType.ASSAULT))
                {
                    TimerHeap.AddTimer(1500, 0, () => { MogoMainCamera.Instance.PlayVictoryCG(); });
                    waitSeconds = 5000;

                }
                TimerHeap.AddTimer(waitSeconds, 0, () =>
                {

                    MogoGlobleUIManager.Instance.ShowPassRewardUI(true);
                    MogoGlobleUIManager.Instance.FillPassRewardItemData(list,
                        () =>
                        {
                            MogoGlobleUIManager.Instance.ShowPassRewardUI(false);
                            EventDispatcher.TriggerEvent(Events.InstanceEvent.ReturnHome);
                            MainUIViewManager.Instance.ShowBossTarget(false);
                        }, bWin);
                    if (bWin)
                    {
                        MogoMainCamera.Instance.LockSight();
                        EventDispatcher.TriggerEvent(SettingEvent.ChangeMusic, 60, SoundManager.PlayMusicMode.Single);
                    }
                });

            }
            return;
        }

        private void TimerShow(uint totalSeconds)
        {
            TimerHeap.AddTimer(totalSeconds * 1000, 0,
                () =>
                {
                    MogoGlobleUIManager.Instance.ShowPassRewardUI(false);
                    MainUIViewManager.Instance.ShowBossTarget(false);
                });
        }

        #endregion

        #region 背包系统
        private void UseItemResp(uint id, byte errorCode, LuaTable info)
        {
            Mogo.Util.LoggerHelper.Debug("UseItemResp:" + errorCode);
            inventoryManager.UseItemResp((int)id, errorCode, info);
            Mogo.Util.LoggerHelper.Debug("UseItemResp:" + errorCode);
        }

        private void SellForResp(byte errorCode)
        {
            inventoryManager.SellForResp(errorCode);
        }

        /// <summary>
        /// 换装和卸装的返回码
        /// </summary>
        /// <param name="id">装备id</param>
        /// <param name="errorCode">返回码</param>
        private void RespsForChgAndRmEquip(int id, byte errorCode)
        {
            inventoryManager.RespsForChgAndRmEquip(id, errorCode);
        }

        /// <summary>
        /// 返回背包更新
        /// </summary>
        private void UpdateArrayItem(LuaTable _info)
        {
            inventoryManager.UpdateItemAllGrid(_info);
        }

        /// <summary>
        /// 返回背包更新
        /// </summary>
        private void UpdateItem(byte updateType, LuaTable _info)
        {
            inventoryManager.UpdateItemGrid(updateType, _info);
        }
        private void ReqForLockResp(byte index, byte errorId)
        {
            LoggerHelper.Debug("ReqForLockResp");
            DecomposeManager.Instance.LockChange(index, errorId);
        }

        private void DeEquipmentResp(byte index, byte errorId, byte hasJewel)
        {
            DecomposeManager.Instance.DecomposeResp(index, errorId, hasJewel);
        }
        #endregion

        #region 装备兑换
        public void PurpleExchangeResp(int errorCode)
        {
            EquipExchangeManager.Instance.PurpleExchangeResp(errorCode);
        }
        #endregion

        #region 聊天系统

        private void ChatResp(byte channel, ulong dbid, String name, byte level, String message)
        {
            LoggerHelper.Debug("dbid = " + dbid + " name = " + name + "level = " + level);
            CommunityMessageData data = new CommunityMessageData();

            data.Channel = (ChannelId)channel;
            data.DBid = dbid;
            data.SenderName = name;
            data.SenderLevel = level;
            data.Message = message;

            EventDispatcher.TriggerEvent<CommunityMessageData>("ReciveCommunityMessage", data);
        }

        #endregion

        #region 运营系统

        public void get_reward_recharge_Resp(UInt16 tableID, UInt32 errorCode)
        {
            LoggerHelper.Debug("get_reward_recharge_Resp");

            if (errorCode != 0)
            {
                operationSystem.HandleErrorMsg((int)errorCode, "get_reward_recharge_Resp");
                return;
            }

            operationSystem.RpcGetDoneCharge();
        }

        public void get_done_recharge_Resp(LuaTable luaTable)
        {
            LoggerHelper.Debug("get_done_recharge_Resp");
            rewardManager.UpdateDoneChargeReward(luaTable);
            //operationSystem.OnGetDoneRecharge(luaTable);
        }

        public void EventOpenResp(UInt32 id)
        {
            LoggerHelper.Debug("EventOpenResp");
            operationSystem.OnAddActivityID(id);
        }

        public void EventOpenListResp(LuaTable luaTable)
        {
            LoggerHelper.Debug("EventOpenListResp");

            operationSystem.OnGetAllActivityEnable(luaTable);
        }

        public void get_event_ing_Resp(LuaTable luaTable, LuaTable luaTableToDo)
        {
            LoggerHelper.Debug("EventOpenListResp");

            operationSystem.OnGetAllActivityDoing(luaTable, luaTableToDo);
        }

        public void finish_task(UInt16 achievementID)
        {
            MogoMsgBox.Instance.ShowFloatingText("活动完成");
            operationSystem.RpcGetAllActivityEnable();
        }

        public void join_event_Resp(UInt32 eventID, UInt32 errorCode)
        {
            if (errorCode != 0)
            {
                operationSystem.HandleErrorMsg((int)errorCode, "join_event_Resp" + eventID);
                return;
            }

            operationSystem.OnJoinEvent((int)eventID);
        }

        public void leave_event_Resp(UInt32 eventID, UInt32 errorCode)
        {
            if (errorCode != 0)
            {
                operationSystem.HandleErrorMsg((int)errorCode, "leave_event_Resp" + eventID);
                return;
            }

            operationSystem.OnLeaveEvent((int)eventID);
        }

        public void get_reward_Resp(UInt16 eventID, UInt32 errorCode)
        {
            LoggerHelper.Debug("get_reward_Resp: " + errorCode);

            if (errorCode != 0)
            {
                operationSystem.HandleErrorMsg((int)errorCode, "get_reward_Resp");
            }

            operationSystem.OnEventGetReward((uint)eventID);
        }

        public void get_reward_login_Resp(UInt32 errorCode)
        {
            LoggerHelper.Debug("get_reward_login_Resp");

            if (errorCode != 0)
            {
                operationSystem.HandleErrorMsg((int)errorCode, "get_reward_login_Resp");
                return;
            }

            operationSystem.OnLoginGetReward();
        }

        public void get_reward_achievement_Resp(UInt16 achievementID, UInt32 errorCode)
        {
            LoggerHelper.Debug("get_reward_achievement_Resp");

            if (errorCode != 0)
            {
                operationSystem.HandleErrorMsg((int)errorCode, "get_reward_achievement_Resp");
                return;
            }

            operationSystem.OnAchievemenShareToGetDiamond((int)achievementID);
        }

        public void get_achievement_Resp(UInt16 index, LuaTable luaTable)
        {
            LoggerHelper.Debug("get_achievement_Resp");

            switch (index)
            {
                case 0:
                    operationSystem.OnGetAllAchievement(luaTable);
                    break;

                default:
                    operationSystem.OnGetAchievementID(luaTable, index);
                    break;
            }
        }

        public void finish_achievement(UInt16 achievementID)
        {
            LoggerHelper.Debug("finish_achievement");
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(47007)); // 成就完成
        }

        private void HotSalesBuyResp(ushort errorCode)
        {
            LoggerHelper.Debug("HotSaleBuyResp: " + errorCode);

            if (errorCode != 0)
            {
                operationSystem.HandleLoginMarketErrorMsg((int)errorCode);
                return;
            }
            else
                operationSystem.OnLoginMarketBuy();
        }

        #endregion

        #region 日常活动

        public void finish_day_task(UInt16 id)
        {
            dailyEventSystem.OnFinishDayTask((int)id);
        }

        public void get_reward_day_task_Resp(UInt16 id, UInt32 errorCode)
        {
            if (errorCode == 0)
                dailyEventSystem.OnGetDailyEventReward((int)id);
            else
                dailyEventSystem.OnHandleErrorCode((int)errorCode);
        }

        public void day_task_change(UInt16 handleID, LuaTable luaTable)
        {
            int id = (int)handleID;
            if (id == 0)
                dailyEventSystem.OnFlushAllData(luaTable);
            else
                dailyEventSystem.OnFlushTaskData(id, luaTable);
        }

        #endregion

        #region 竞技活动

        private void CampaignResp(ushort msg, ushort err, LuaTable luaTable)
        {
            campaignSystem.RpcCampaignResp(msg, err, luaTable);
        }

        #endregion

        #region PVP占塔

        public void DefensePvPApplyResp(ushort arg)
        {
            occupyTowerSystem.DefensePvPApplyResp(arg);
        }

        public void DefensePvPOpened()
        {
            occupyTowerSystem.DefensePvPOpened();
        }

        public void DefensePvpEnterResp(uint arg, LuaTable luaTable)
        {
            occupyTowerSystem.DefensePvpEnterResp(arg, luaTable);
        }

        public void DefensePvpPointRefresh(LuaTable playerScoreluaTable, LuaTable towerBloodAndScoreluaTable)
        {
            occupyTowerSystem.DefensePvpPointRefresh(playerScoreluaTable, towerBloodAndScoreluaTable);
        }

        public void DefensePvpAward(byte arg1, byte arg2)
        {
            occupyTowerSystem.DefensePvpAward(arg1, arg2);
        }

        public void DefensePvPStateResp(int arg1, uint arg2)
        {
            occupyTowerSystem.DefensePvPStateResp(arg1, arg2);
        }

        #endregion

        public void ActiveSepciaclEffectsResp(byte id,byte errCode)
        {

            EventDispatcher.TriggerEvent<byte,byte>("ActiveSepciaclEffectsResp",id, errCode);
        }

        public void SyncSepcialEffectsResp(LuaTable data)
        {
            EventDispatcher.TriggerEvent<LuaTable>("SyncSepcialEffectsResp", data);
        }
        public void ElfAreaTearProgResp(LuaTable data)
        {
            //刷新所有领域使用女神之泪情况
            //用来更新总数，分数，流动
            Debug.LogError("elf" + data);
            Dictionary<int, int> elfData;
            if (Utils.ParseLuaTable(data, out elfData))
            {
                Debug.LogError("elf" + elfData.PackMap());
                ElfSystemData.dataMap = elfData;
                ElfSystem.Instance.SetElfTear(inventoryManager.GetItemNumById(1100076));
            }
        }
        public void ElfSysMsgResp(byte errCode)
        {
            //错误信息提示
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap.Get(29602 + errCode).content);
        }

        public void UpdateElfSkillPoint(int skillPoint)
        {
            //更新技能点
            //存储
            ElfSystem.Instance.SkillPoint = skillPoint;
            //刷新UI
            SpriteUIViewManager.Instance.SetLearnTimesNum(skillPoint);
        }
        public void UpdateElfLearnedSkillInfo(LuaTable data,uint skillId)
        {
            
            //技能学习情况
            
            List<List<int>> tempData;
            Dictionary<int, int> skillData = new Dictionary<int, int>();
            if (Utils.ParseLuaTable(data, out tempData))
            {
                foreach (var item in tempData)
                {
                    skillData.Add(item[0], item[1]);
                }

                ElfSystem.Instance.RefreshLearnedSkill(skillData,skillId);
               
            }
        }
        public void GuildResp(byte id, UInt16 errCode, LuaTable respInfo)
        {
            TongManager.Instance.GuildResp(id, errCode, respInfo);
        }

        public void NPCResp(UInt32 errorCode)
        {
            // to do 不知道这个函数是用来做什么的
        }

        // 蓄力通知，参数为：施放者ID，开始（1）或取消（0）
        public void ChargeSkillResp(uint dbid, byte flag)
        {
            if (dbid == this.ID)
            {
                LoggerHelper.Debug("RPCResp ChargeSkillResp: myself");
                return;
            }
            EntityPlayer entity = (EntityPlayer)MogoWorld.GetEntity(dbid);
            if (entity == null)
            {
                LoggerHelper.Error("RPCResp PowerUp args error : not found entity");
                return;
            }
            if (flag > 0)
            {
                entity.OnPowerChargeInterrupt();
            }
            else
            {
                entity.OnPowerCharge();
            }
        }

        private void PickDropResp(UInt32 errId, LuaTable items)
        {
            Dictionary<int, int> data;
            if (errId > 100)
            {
                if (sfxHandler && EntityDrop.PickUpFxId > 0)
                {
                    sfxHandler.HandleFx(EntityDrop.PickUpFxId);
                }
                if (Utils.ParseLuaTable(items, out data))
                {
                    if (data.ContainsKey(1))
                    {
                        BillboardLogicManager.Instance.AddAloneBattleBillboard(MogoWorld.thePlayer.Transform.Find("slot_billboard").position, data[1], AloneBattleBillboardType.Gold);
                    }
                }

                //if (!MogoWorld.Entities.ContainsKey(errId))
                //{
                return;
                //}
                //EntityParent drop = MogoWorld.Entities[errId];

                //TimerHeap.AddTimer<string, uint>(30, 0, EventDispatcher.TriggerEvent, Events.FrameWorkEvent.AOIDelEvtity, errId);

                //(drop as EntityDrop).OnLeaveWorld();
            }
        }

        #region 精灵系统
        private void LevelUpSkillResp(byte errorCode)
        {
            EventDispatcher.TriggerEvent<byte>(Events.AssistantEvent.LevelUpSkillResp, errorCode);
        }

        private void LevelUpMarkResp(byte errorCode)
        {
            EventDispatcher.TriggerEvent<byte>(Events.AssistantEvent.LevelUpMarkResp, errorCode);
        }

        private void ClientDragSkillResp(UInt32 skillId, UInt32 gridId, byte errorCode)
        {
            EventDispatcher.TriggerEvent<UInt32, UInt32, byte>(Events.AssistantEvent.ClientDragSkillResp, skillId, gridId, errorCode);
        }

        private void ClientDragMarkResp(UInt32 mintmarkId, UInt32 gridId, byte errorCode)
        {
            EventDispatcher.TriggerEvent<UInt32, UInt32, byte>(Events.AssistantEvent.ClientDragMarkResp, mintmarkId, gridId, errorCode);
        }

        private void SpiritPropRefreshResp(byte type, LuaTable lt)
        {
            EventDispatcher.TriggerEvent<byte, LuaTable>(Events.AssistantEvent.PropRefreshResp, type, lt);
        }
        #endregion

        private void MarketNeedUpdate()
        {
            marketManager.DownloadForce();
        }

        private void LevelGiftRecordResp(LuaTable gifts)
        {
            List<int> ids;
            if (Utils.ParseLuaTable<List<int>>(gifts, out ids))
            {
                marketManager.UpdateGifts(ids);
            }
        }

        private void TransferMarketDataResp(int version, LuaTable items)
        {
            marketManager.UpdateItems(version, items);
        }

        private void HotSalesNeedUpdate()
        {
            marketManager.DownloadLogin();
        }

        private void MarketGridDataResp(UInt16 id, UInt32 num)
        {
            marketManager.UpdateNum((int)id, (int)num);
        }

        private void OnLogoutResp(byte msg_id)
        {
            LoggerHelper.Debug("OnLogoutResp: " + msg_id);
            if (msg_id == 0)
            {
                //if (Application.platform == RuntimePlatform.OSXEditor)
                Application.Quit();
                //UnityEditor.EditorApplication.isPlaying = false;
                //else
            }
            else
                MogoWorld.BackToChooseCharacter();
        }

        private void ShowTextID(byte type, uint msgId)
        {
            int id = (int)msgId;
            string msg = String.Empty;
            if (LanguageData.dataMap.ContainsKey(id))
            {
                msg = LanguageData.dataMap[id].content;
            }
            else
            {
                msg = id + "";
            }

            ShowText(type, msg);
        }

        private void ShowTextIDWithArgs(byte type, uint msgId, LuaTable info)
        {
            //Debug.LogError("msgId:" + msgId);
            //Debug.LogError("info:" + info);
            int id = (int)msgId;
            string msg = String.Empty;
            if (LanguageData.dataMap.ContainsKey(id))
            {
                msg = LanguageData.dataMap[id].content;
                //Dictionary<string,string> temp = new  Dictionary<string,string>();
                List<string> strList = new List<string>();

                if (info != null)//&& Utils.ParseLuaTable(info, out temp)s
                {
                    foreach (var pair in info)
                    {
                        Mogo.Util.LoggerHelper.Debug(pair.Value);
                        if (pair.Key.StartsWith("item_id")) strList.Add(ItemParentData.GetItem(int.Parse(pair.Value.ToString())).Name);
                        else strList.Add(pair.Value.ToString());
                    }
                    Mogo.Util.LoggerHelper.Debug(msg);
                    msg = string.Format(msg, strList.ToArray());
                }

            }
            else
            {
                msg = id + "";
            }
            ShowText(type, msg);
        }


        private void ShowText(byte type, string msg)
        {
            switch (type)
            {
                case 1:
                    MogoMsgBox.Instance.ShowMsgBox(msg);
                    break;
                case 2:
                    MogoMsgBox.Instance.ShowFloatingText(msg);
                    break;
                case 3:
                    MogoMsgBox.Instance.ShowWaveText(msg);
                    break;
                case 4:
                    MogoMsgBox.Instance.ShowFloatingTextQueue(msg);
                    break;
                case 5:
                    CommunityUIViewManager.Instance.AddChatUIDialogText(msg, ChannelId.SYSTEM, 0, ChannelId.SYSTEM, CommunityUILogicManager.CHANNEL_BLANK);
                    break;
            }
        }

        private void SkillUpResp(byte code)
        {
            LoggerHelper.Debug("SkillUpResp " + code);
            switch (code)
            {
                case 0:
                    {//成功
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[14501].content);
                        break;
                    }
                case 1:
                    {//不存在下一级
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[14503].content);
                        LoggerHelper.Error("skillupresp code " + code);
                        break;
                    }
                case 2:
                    {//没有技能ID
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[14502].content);
                        LoggerHelper.Error("skillupresp code " + code);
                        break;
                    }
                case 3:
                    {//金钱不足
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[14505].content);
                        LoggerHelper.Error("skillupresp code " + code);
                        break;
                    }
                case 4:
                    {//当前组ID和下一级组ID不一致
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[14504].content);
                        LoggerHelper.Error("skillupresp code " + code);
                        break;
                    }
                case 5:
                    {//已学习
                        //MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[14506].content);
                        LoggerHelper.Error("has study");
                        LoggerHelper.Error("skillupresp code " + code);
                        break;
                    }
                case 6:
                    {//越级升级
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[14507].content);
                        LoggerHelper.Error("skillupresp code " + code);
                        break;
                    }
            }
        }

        private void SkillSyncToClient(LuaTable skills)
        {
            SetObjectAttr("skillBag", skills);
            (skillManager as PlayerSkillManager).UpdateSkillData(skills);
        }

        //使用者自身不会收到
        //参数为：施放者ID， 技能ID
        private void CastSkillResp(UInt32 id, Int16 x, Int16 z, byte face, UInt16 skillID)
        {
            if (!MogoWorld.Entities.ContainsKey(id))
            {
                LoggerHelper.Debug("skill player not exist " + id);
                return;
            }
            EntityParent entity = MogoWorld.Entities[id];
            if (entity is EntityMercenary)
            {
                return;
            }

            if (entity is EntityMonster && entity.stiff)
            {
                return;
            }
            //使用技能前强同步
            float x_ = (short)x * 0.01f;
            float z_ = (short)z * 0.01f;
            float f_ = face * 2;
            if (entity.Transform == null)
            {//模型还没有
                return;
            }


            float dis = UnityEngine.Vector2.Distance(new UnityEngine.Vector2(x_, z_),
        new UnityEngine.Vector2(entity.Transform.position.x, entity.Transform.position.z));
            if (dis > 2.0f)
            {
                //LoggerHelper.Error("distance(server xy , client xy) too far, need correction:" + dis);
                entity.Transform.position = new Vector3(x_, entity.Transform.position.y, z_);
            }

            // Debug.LogError(GameObject.name + " " + ID + " Myself CastSkillResp: " + Transform.position);

            if (entity.GetIntAttr("notTurn") != 1)
            {
                Quaternion q = new Quaternion();
                q.eulerAngles = new Vector3(0, f_, 0);
                entity.Transform.localRotation = q;
            }
            //if (entity.currSpellID != -1)            //{
            //    return;
            //}
            entity.CastSkill(skillID);
        }

        // 提供给 服务器的技能效果回调
        // 参数为：施放者ID，技能ID，动作序列，连击次数，技能目标列表{目标ID={伤害类型, 伤害值} x N个对象}，
        // 伤害类型，具体说明如下：
        // {1代表Miss}
        // {2代表普通伤害}
        // {3代表破击伤害}
        // {4代表暴击伤害}
        // {5代表同时破击和暴击伤害}
        private void SkillHarmResp(UInt32 id, UInt16 skillID, UInt16 actionID, UInt16 hitNum, LuaTable harms)
        {
            //var ety = MogoWorld.Entities.GetValueOrDefault(id, null);
            //if (ety == null)
            //    return;
            //if (ety != null && ety != MogoWorld.thePlayer && ety.GetType() == typeof(EntityPlayer))
            //    return;
            Dictionary<int, List<int>> args;
            Mogo.Util.Utils.ParseLuaTable(harms, out args);
            Dictionary<uint, List<int>> wounded = new Dictionary<uint, List<int>>();
            foreach (var i in args)
            {
                wounded.Add((uint)i.Key, i.Value);

                if ((uint)i.Key == this.ID)
                    EventDispatcher.TriggerEvent(Events.ComboEvent.ResetCombo);
            }

            if ((uint)id == this.ID)
                EventDispatcher.TriggerEvent(Events.ComboEvent.AddCombo, wounded.Count);

            SkillData s = SkillData.dataMap[skillID];
            int actID = s.skillAction[actionID];
            // 通知受击
            foreach (var i in wounded)
            {
                if (id != ID && !MogoWorld.Entities.ContainsKey(id))
                {//非主角，且不在entities
                    continue;
                }

                //if (id != ID && (MogoWorld.Entities[id] is EntityPlayer))
                //{//非主角玩家
                //    continue;
                //}

                if ((i.Key != ID && !MogoWorld.Entities.ContainsKey(i.Key)))
                    // || (i.Key != ID && (MogoWorld.Entities[i.Key] is EntityPlayer)))
                {//非主角玩家的伤害
                    continue;
                }

                if (MogoWorld.Entities.ContainsKey(i.Key) &&
                    (MogoWorld.Entities[i.Key] is EntityMonster || MogoWorld.Entities[i.Key] is EntityMercenary || MogoWorld.Entities[i.Key] is EntityPlayer))
                {
                    if (skillManager != null)
                    {
                        skillManager.AttackEnemyGenAnger(MogoWorld.Entities[i.Key]);
                    }
                }
                if (i.Key == ID && skillManager != null)
                {
                    skillManager.AttackSelfGenAnger(this);
                }

                EventDispatcher.TriggerEvent<int, uint, uint, List<int>>(Events.FSMMotionEvent.OnHit,
                    actID, id, i.Key, i.Value);
            }
        }

        private void SkillBuffResp(UInt32 targetId, UInt16 buffId, byte isAdd, UInt32 time)
        {
            //LoggerHelper.Error("SkillBuffResp:" + targetId + ",buffId:" + buffId);
            EntityParent entity = null;

            if (MogoWorld.Entities.ContainsKey(targetId))
            {
                entity = MogoWorld.Entities[targetId];
            }
            else if (targetId == MogoWorld.thePlayer.ID)
            {
                entity = MogoWorld.thePlayer;
            }

            if (entity != null)
            {
                entity.HandleBuff(buffId, isAdd, time);
            }
        }

        private void ClientCastSkillResp(uint a, uint b, byte c)
        {
        }

        private void err_resp(uint opId, uint errId)
        {
        }

        private void ClientCastMarkResp(uint a, uint b, byte c)
        {
        }

        private void QueryClientTickReq(uint sid)
        {
            ulong t = 0;
            DateTime d = DateTime.Now;
            t = (ulong)d.Day * 24 * 3600 * 1000 + (ulong)d.Hour * 3600 * 1000 + (ulong)d.Minute * 60 * 1000 + (ulong)d.Second * 1000 + (ulong)d.Millisecond;
            RpcCall("QueryClientTickResp", sid, t);
        }

        private void BuyOrdinaryWingResp(byte code)
        {
            switch (code)
            {
                case 0:
                    {//购买成功
                        break;
                    }
                case 1:
                    {//配置数据错误
                        break;
                    }
                case 2:
                    {//翅膀类型错误
                        break;
                    }
                case 3:
                    {//已经拥有该翅膀
                        break;
                    }
                case 4:
                    {//职业受限
                        break;
                    }
                case 5:
                    {//VIP等级不足
                        break;
                    }
                case 6:
                    {//金币不足
                        break;
                    }
                case 7:
                    {//钻石不足
                        break;
                    }
                case 8:
                    {//道具消耗不够
                        break;
                    }
            }
        }

        private void TrainOrdinaryWingResp(byte code)
        {
            switch (code)
            {
                case 0:
                    {//培养OK
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200003].content);
                        break;
                    }
                case 1:
                    {//没有购买该翅膀
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200004].content);
                        break;
                    }
                case 2:
                    {//等级配置错误
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200005].content);
                        break;
                    }
                case 3:
                    {//钻石或道具不足
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200006].content);
                        break;
                    }
                case 4:
                    {//己达到顶级
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200007].content);
                        break;
                    }
            }
        }

        private void UnlockMagicWingResp(byte code)
        {
            switch (code)
            {
                case 0:
                    {//解锁OK
                        break;
                    }
                case 1:
                    {//翅膀类型错误
                        break;
                    }
                case 2:
                    {//职业不符
                        break;
                    }
                case 3:
                    {//VIP等级不足
                        break;
                    }
                case 4:
                    {//普能翅膀等级不足
                        break;
                    }
                case 5:
                    {//解锁翅膀配置错误
                        break;
                    }
                case 7:
                    {//金币不足
                        break;
                    }
                case 8:
                    {//钻石不足
                        break;
                    }
                case 9:
                    {//已经解锁
                        break;
                    }
            }
        }

        private void MagicWingActiveResp(byte code)
        {
            switch (code)
            {
                case 0:
                    {//激活成功
                        break;
                    }
                case 1:
                    {//没有该翅膀
                        break;
                    }
                case 2:
                    {//配置错误
                        break;
                    }
                case 3:
                    {//金币不足
                        break;
                    }
                case 4:
                    {//钻石不足
                        break;
                    }
                case 5:
                    {//道具不足
                        break;
                    }
                case 6:
                    {//已被激活
                        break;
                    }
            }
        }

        private void WingExchangeResp(byte code)
        {
            switch (code)
            {
                case 0:
                    {//换装OK
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200000].content);
                        break;
                    }
                case 1:
                    {//没有该翅膀
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200001].content);
                        break;
                    }
                case 2:
                    {//已穿该翅膀
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[1200002].content);
                        break;
                    }
            }
        }

        private void WingBagSyncClientResp(LuaTable wings)
        {
            wingBag = wings;
        }

        private void OnNotifyChargeResp(LuaTable ordList)
        {
            LoggerHelper.Debug("charge list " + ordList);
            if (ordList.Count <= 0)
            {
                return;
            }
            uint time = UInt32.Parse((string)((LuaTable)ordList["1"])["create_time"]);
            uint diamond = UInt32.Parse((string)((LuaTable)ordList["1"])["diamond"]);
            preorddbid = ulong.Parse((string)((LuaTable)ordList["1"])["ord_dbid"]);
            DateTime d = DateTime.FromFileTimeUtc(time);
            prewithdraw = diamond;
            MogoMessageBox.Confirm(LanguageData.GetContent(4003, d.Month, d.Day, diamond) + "\n" + LanguageData.GetContent(4004, name), LanguageData.GetContent(4101), LanguageData.GetContent(4104), withdrawcb);
        }

        private uint prewithdraw = 0;
        private ulong preorddbid = 0;
        private void withdrawcb(bool b)
        {
            if (b)
            {
                MogoMessageBox.Confirm(LanguageData.GetContent(4005, prewithdraw) + "\n" + LanguageData.GetContent(4006, name), LanguageData.GetContent(4103), LanguageData.GetContent(4104), (rb) => { if (rb) { EventDispatcher.TriggerEvent(Events.OtherEvent.Withdraw, preorddbid); } preorddbid = 0; prewithdraw = 0; });
            }
            else
            {
                preorddbid = 0;
                prewithdraw = 0;
            }
        }

        private void OnChargeDiamondResp(uint diamond)
        {
            MogoGlobleUIManager.Instance.Info(LanguageData.GetContent(4000, diamond), LanguageData.GetContent(4102), "CANCEL", -1, ButtonBgType.Blue, ButtonBgType.Brown, () => { MogoUIManager.Instance.ShowVIPInfoUI(); });
        }

        private void OnWithdrawResp(uint rmb, uint diamond)
        {
        }

        private void DiamondMineInfoResp(LuaTable info)
        {
            //LoggerHelper.Error("reee " + info);
            ElfDiamondUILogicManager.Instance.SetCostDiamondNum("x " + info["cost"]);
            ElfDiamondUILogicManager.Instance.SetDiamondNumCanGet((string)info["get"]);
            ElfDiamondUILogicManager.Instance.SetTotalCostDiamondNum(LanguageData.GetContent(7808, info["sumCost"]));
            ElfDiamondUILogicManager.Instance.SetTotalGotDiamondNum(LanguageData.GetContent(7809, info["sumGet"]));
            ElfDiamondUILogicManager.Instance.SetUIDirty();
        }

        private void JewlMailResp(TDBID mailDBId)
        {
            TDBID mailGridId;
            bool rntFlag = MailManager.Instance.GetGridByDBId(mailDBId, out mailGridId);
            if (rntFlag)
            {
                //LoggerHelper.Error("Get mailDBId  mailGridId SUCCEEDED " + mailGridId + " mailDBID:" + mailDBId);
                TipManager.Instance.AddMailHasJewelTip(mailGridId);
            }
            else
            {
                //LoggerHelper.Error("Get mailDBId  mailGridId FAILED " + mailGridId + " mailDBID:" + mailDBId); 
                //TipManager.Instance.AddMailHasJewelTip(0); 
            }
        }
        #endregion

        #region Dummy相关
        //创建前端怪
        private void CreateCliEntityResp(LuaTable entity)
        {
            
            List<List<int>> args;

            Mogo.Util.Utils.ParseLuaTable(entity, out args);
            for (int i = 0; i < args.Count; i++)
            {
                if (MogoWorld.Entities.ContainsKey((uint)args[i][1]))
                    continue;

                switch (args[i][0])
                {
                    case (int)(CliEntityType.CLI_ENTITY_TYPE_DUMMY):
                        MogoWorld.CreateDummy((uint)args[i][1], args[i][2], args[i][3], args[i][4], args[i][5], args[i][6]);
                        break;
                    case (int)(CliEntityType.CLI_ENTITY_TYPE_DROP):
                        MogoWorld.CreateDrop((uint)args[i][1], args[i][2], args[i][3], args[i][4], args[i][5], args[i][6]);
                        break;
                }
            }
        }

        //boss死亡了导致dummy都要死亡处理
        private void BossDieResp()
        {
            MissionData md = MissionData.GetMissionData(curMissionID, curMissionLevel);
            if (md != null && md.passEffect != 1 && !StoryManager.Instance.HasFinalCG())
            {
                EventDispatcher.TriggerEvent(Events.OtherEvent.BossDie);
            }
            EventDispatcher.TriggerEvent(Events.AIEvent.ProcessBossDie);
        }

        //通知前端被顶号
        private void OnMultiLogin()
        {
            MogoWorld.beKick = true;
            MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(25563), (ok) =>
            {
                MogoGlobleUIManager.Instance.ConfirmHide();
            }, LanguageData.GetContent(25561));
            ServerProxy.Instance.Disconnect();
        }
        #endregion

        #region 排行榜系统

        /// <summary>
        /// 排行榜数据列表
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="luaTable"></param>
        /// <param name="time"></param>
        /// <param name="hasData"></param>
        private void RankListResp(byte flag, LuaTable luaTable, uint time, byte hasData)
        {
            rankManager.OnRankListResp(flag, luaTable, time, hasData);
        }

        /// <summary>
        /// 角色基本信息
        /// </summary>
        /// <param name="luaTable"></param>
        public void RankAvatarInfoResp(LuaTable luaTable, byte sex, byte isMyIdol)
        {
            rankManager.RankAvatarInfoResp(luaTable, sex, isMyIdol);
        }

        public void HasOnRankResp(byte rankNum)
        {
            rankManager.HasOnRankResp(rankNum);
        }

        /// <summary>
        /// 变更偶像(询问能否变更和变更偶像两个RPC回调)
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="oldIdolName"></param>
        public void FansIdolResp(byte flag, string oldIdolName)
        {
            rankManager.FansIdolResp(flag, oldIdolName);
        }

        /// <summary>
        /// 获取玩家偶像
        /// </summary>
        /// <param name="selfIdol"></param>
        public void SelfIdolResp(TDBID selfIdol)
        {
            rankManager.SelfIdolResp(selfIdol);
        }

        #endregion

        #region 等级不足显示

        private void CanGetExpResp(uint exp)
        {
            LevelNoEnoughUILogicManager.Instance.RpcGetArenaExpResp((int)exp);
        }

        #endregion
    }
}
