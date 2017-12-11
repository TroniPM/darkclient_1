using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Mogo.Util;
using Mogo.Game;

namespace Mogo.GameLogic.LocalServer
{
    #region 各种数据类

    public class SpawnPoint
    {
        public int ID;
        public SpawnPointState state;

        public SpawnPoint()
        {
            state = SpawnPointState.inactive;
        }

        public SpawnPoint(int theID, int triggerType = 0)
        {
            ID = theID;

            switch (triggerType)
            {
                case (int)SpawnPointType.step:
                    state = SpawnPointState.inactive;
                    break;

                case (int)SpawnPointType.start:
                    state = SpawnPointState.activeWhenStart;
                    break;
            }
        }
    }


    // 故意写得和MissionEventData有些许不同，他不可被替代
    // 因为这个类不应该知道GameData的东西，所有与GameData相关的部分类型都是var
    public class LocalServerMissionEvent
    {
        public List<int> spawnPoint = new List<int>();
        public List<int> notifyToClient = new List<int>();
        public List<int> notifyOtherSpawnPoint = new List<int>();

        public LocalServerMissionEvent()
        {
            spawnPoint = new List<int>();
            notifyToClient = new List<int>();
            notifyOtherSpawnPoint = new List<int>();
        }
    }

    #region Entity

    public class LocalServerEntity
    {
        public uint ID;
    }

    public class LocalServerAvatar : LocalServerEntity
    {
        public uint curHp;
        public int deathFlag;
        public int defaultReviveTimes;
        public int reviveTimes;

        protected LocalServerAvatar()
            : base()
        {
            curHp = MogoWorld.thePlayer.curHp;
            deathFlag = 0;
        }

        public LocalServerAvatar(int theReviveTimes)
            : this()
        {
            defaultReviveTimes = theReviveTimes;
            reviveTimes = theReviveTimes;
        }
    }

    public class LocalServerDummy : LocalServerEntity
    {
        public int spawnPointID;
        public int monsterID;
        public MonsterState state;
        public int x;
        public int y;
        public int exp;

        public LocalServerDummy()
            : base()
        {
            state = MonsterState.invalid;
        }

        public LocalServerDummy(LocalServerDummy template)
            : base()
        {
            spawnPointID = template.spawnPointID;
            monsterID = template.monsterID;
            state = template.state;
            x = template.x;
            y = template.y;
            exp = template.exp;
        }
    }

    #endregion

    #endregion


    public class LocalServerSceneManager
    {
        protected static readonly int tokenJudgeMaxNum = 6;

        private static LocalServerSceneManager m_instance;
        public static LocalServerSceneManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new LocalServerSceneManager();
                }

                return LocalServerSceneManager.m_instance;
            }
        }

        protected static uint IDPool = 0;

        // 所有的Entity，不包含召唤怪
        protected Dictionary<uint, LocalServerEntity> localServerEntity = new Dictionary<uint, LocalServerEntity>();

        // 副本关注事件，可能某些副本不关注任何事件
        protected List<LocalServerMissionEvent> localServerEvent = new List<LocalServerMissionEvent>();

        // 副本目标，一个副本必须有至少一个目标
        protected List<int> localServerMissionTarget = new List<int>();

        // 副本SpawnPoint的状态，用来处理PreSpawnPoint
        protected Dictionary<int, SpawnPoint> localServerSpawnPoint = new Dictionary<int, SpawnPoint>();

        // 副本怪物，格式是<SpawnPointID, <MonsterID, LocalServerMonsterData>>
        protected Dictionary<int, Dictionary<uint, LocalServerEntity>> localServerMonster = new Dictionary<int, Dictionary<uint, LocalServerEntity>>();

        // 副本容器
        protected Dictionary<int, int> localServerContainer = new Dictionary<int, int>();

        // 怪物模板
        protected Dictionary<int, List<LocalServerEntity>> localServerTokenTemplete = new Dictionary<int, List<LocalServerEntity>>();

        // 召唤怪物
        protected Dictionary<uint, LocalServerEntity> localServerSummonToken = new Dictionary<uint, LocalServerEntity>();

        //本地拾取到的道具总额 itemId<=>num
        protected Dictionary<int, int> localAvatarCollectedDrops = new Dictionary<int, int>();
        //本地拾取的金币总额
        protected int localAvatarCollectedMoney = 0;
        //本地得到的经验总额
        protected int localAvatarCollectedExp = 0;

        // 副本的连击数
        protected int localAvatarCombo = 0;
        // 副本使用的血瓶数
        protected int localAvatarBottle = 0;

        //服务器给的拾取道具池
        protected Dictionary<int, int> srvPreCollectedDrops = new Dictionary<int, int>();
        //服务器给的金币总额
        protected int preMoney = 0;
        //服务器给的经验总额
        protected int preExp = 0;

        // 副本内玩家状态，主要用于处理死亡
        protected LocalServerAvatar localServerAvatar = null;

        protected int sceneId = -1;


        #region 初始化，加载和卸载

        public void Initialize()
        {
            localServerEntity = new Dictionary<uint, LocalServerEntity>();
            localServerEvent = new List<LocalServerMissionEvent>();
            localServerMissionTarget = new List<int>();
            localServerSpawnPoint = new Dictionary<int, SpawnPoint>();

            localServerMonster = new Dictionary<int, Dictionary<uint, LocalServerEntity>>();
            localServerContainer = new Dictionary<int, int>();
            localServerTokenTemplete = new Dictionary<int, List<LocalServerEntity>>();
            localServerSummonToken = new Dictionary<uint, LocalServerEntity>();

            localServerAvatar = null;

            AddListeners();
        }


        public void Release()
        {
            RemoveListeners();
        }

        protected void AddListeners()
        {
            EventDispatcher.AddEventListener(Events.LocalServerEvent.ExitMission, ExitMission);
            EventDispatcher.AddEventListener<int>(Events.LocalServerEvent.SummonToken, SummonToken);
        }

        protected void RemoveListeners()
        {
            EventDispatcher.RemoveEventListener(Events.LocalServerEvent.ExitMission, ExitMission);
            EventDispatcher.RemoveEventListener<int>(Events.LocalServerEvent.SummonToken, SummonToken);
        }

        #endregion


        #region RPC相关

        public static void RPCCall(string FuncName, params object[] Args)
        {
            TimerHeap.AddTimer(10, 0, (funcName, args) =>
            {
                EventDispatcher.TriggerEvent<object[]>(Util.Utils.RPC_HEAD + funcName, args);
            }, FuncName, Args);
        }

        public void MissionReq(byte handleCode, ushort arg1, ushort arg2, string arg3)
        {
            switch (handleCode)
            {
                case (byte)MissionHandleCode.ENTER_MISSION:
                    EnterMission((int)arg1, (int)arg2);
                    break;

                case (byte)MissionHandleCode.START_MISSION:
                    List<int> pos = LocalServerResManager.GetEnterXY(sceneId);
                    if (pos.Count == 0)
                    {
                        LoggerHelper.Error(string.Format("sceneId {0} is enter xy is error", sceneId));
                        return;
                    }
                    var info = new Mogo.RPC.CellAttachedInfo();
                    info.x = (short)pos[0];
                    info.y = (short)pos[1];
                    MogoWorld.thePlayer.SetEntityCellInfo(info);
                    MogoWorld.thePlayer.UpdatePosition();

                    CheckSpawnPointDefaultSpawn();
                    break;

                case (byte)MissionHandleCode.EXIT_MISSION:
                    ServerProxy.SomeToLocal = false;
                    ExitMission();
                    MogoWorld.thePlayer.RpcCall("MissionReq", (byte)MissionHandleCode.EXIT_MISSION, (ushort)1, (ushort)1, String.Empty);
                    break;

                case (byte)MissionHandleCode.QUIT_MISSION:
                    ServerProxy.SomeToLocal = false;
                    ExitMission();
                    MogoWorld.thePlayer.RpcCall("MissionReq", (byte)MissionHandleCode.QUIT_MISSION, (ushort)1, (ushort)1, String.Empty);
                    break;

                case (byte)MissionHandleCode.GO_TO_INIT_MAP:
                    ServerProxy.SomeToLocal = false;
                    ExitMission();
                    MogoWorld.thePlayer.RpcCall("MissionReq", (byte)MissionHandleCode.GO_TO_INIT_MAP, (ushort)1, (ushort)1, String.Empty);
                    break;

                case (byte)MissionHandleCode.SPAWNPOINT_START:
                    TriggerSpawnPoint((int)arg1);
                    break;

                case (byte)MissionHandleCode.CREATE_CLIENT_DROP:
                    CreateClientDrop(arg1, arg3);
                    break;

                case (byte)MissionHandleCode.GET_REVIVE_TIMES:
                    LoggerHelper.Debug("GET_REVIVE_TIMES");
                    NotifyReviveTime();
                    break;

                case (byte)MissionHandleCode.REVIVE:
                    LoggerHelper.Debug("REVIVE");
                    AvatarRevive();
                    break;

                case (byte)MissionHandleCode.UPLOAD_COMBO:
                    SetCombo(arg1);
                    NotifyClientMissionWon();
                    break;

                case (byte)MissionHandleCode.GET_MISSION_REWARDS:
                    ServerProxy.SomeToLocal = false;
                    SendClientMissionMessage();
                    GetMissionReward();
                    break;
            }
        }


        public void MissionResp(MissionHandleCode code, LuaTable luaTable)
        {
            RPCCall("MissionResp", (byte)code, luaTable);
        }


        public void CliEntityActionReq(uint eid, uint actId, uint arg1, uint arg2)
        {
            switch (actId)
            {
                case (uint)CliEntityActionHandleCode.DummyDie:
                    if (localServerSummonToken.ContainsKey(eid))
                        SummonTokenDead(eid);
                    else
                        DummyDead(eid, (int)arg1, (int)arg2);
                    break;

                case (uint)CliEntityActionHandleCode.HitAvatar:
                    HitAvatar(arg1);
                    break;
            }
        }


        public void CliEntitySkillReq(uint skillType, uint arg1)
        {
            switch (skillType)
            {
                case (uint)CliEntitySkillHandleCode.SummonToken:
                    SummonToken((int)arg1);
                    break;
            }
        }

        public void UseHpBottleReq(int num)
        {//使用血瓶

        }

        #endregion


        #region ID池

        protected static void ResetIDPool()
        {
            IDPool = 0;
        }

        public static uint getNextEntityId()
        {
            IDPool++;
            return IDPool;
        }

        #endregion


        #region 进出副本

        #region 进入副本

        public void EnterMission(int missionID, int difficulty)
        {
            var missionData = LocalServerResManager.MissionDataMap.FirstOrDefault(t => t.Value.mission == missionID && t.Value.difficulty == difficulty);

            if (missionData.Value == null)
                return;

            ClearSceneData();
            LocalServerResManager.ResetSpaceData(missionData.Value);
            ConstructEventData(missionData.Value.events);
            ConstructMissionTargetData(missionData.Value.target);
            ConstructEntityData(difficulty);
            ConstructContainerData(missionData.Value.drop);
            EnterResetAvatarData(missionData.Value.reviveTimes);
            sceneId = LocalServerResManager.GetSceneId(missionID, difficulty);
        }



        protected void ConstructEventData(List<int> eventIDs)
        {
            if (eventIDs == null)
                return;

            foreach (var eventID in eventIDs)
            {
                if (!LocalServerResManager.MissionEventDataMap.ContainsKey(eventID))
                    continue;

                LocalServerMissionEvent newEvent = new LocalServerMissionEvent();

                var missionData = LocalServerResManager.MissionEventDataMap[eventID];
                if (missionData.type == 1 && missionData.param != null)
                {
                    foreach (var spawnPointID in missionData.param)
                        newEvent.spawnPoint.Add(spawnPointID);
                }

                newEvent.notifyToClient = missionData.notifyToClient;
                newEvent.notifyOtherSpawnPoint = missionData.notifyOtherSpawnPoint;

                localServerEvent.Add(newEvent);
            }
        }

        protected void ConstructMissionTargetData(List<int> targetIDs)
        {
            foreach (var targetID in targetIDs)
            {
                localServerMissionTarget.Add(targetID);
            }
        }

        protected void ConstructEntityData(int difficulty)
        {
            ConstructMonsterData(difficulty);
        }

        protected void ConstructMonsterData(int difficulty)
        {
            foreach (var item in LocalServerResManager.SpaceDataMap)
            {
                var spaceData = item.Value;
                if (spaceData.type != SpaceType.SpawnPoint.ToString())
                    continue;

                if (spaceData.levelID.Count < difficulty)
                    continue;

                int levelID = spaceData.levelID[difficulty - 1];
                if (!LocalServerResManager.SpawnPointLevelDataMap.ContainsKey(levelID))
                    continue;

                List<int> monsterIDs = LocalServerResManager.SpawnPointLevelDataMap[levelID].monsterId;
                List<int> monsterNums = LocalServerResManager.SpawnPointLevelDataMap[levelID].monsterNumber;

                if (monsterIDs.Count != monsterNums.Count)
                    continue;

                if (item.Value.monsterSpawntPoint.Count % 2 != 0)
                    continue;

                List<KeyValuePair<int, int>> place = new List<KeyValuePair<int, int>>();
                for (int i = 0; i < item.Value.monsterSpawntPoint.Count; i += 2)
                {
                    KeyValuePair<int, int> coordinate = new KeyValuePair<int, int>(item.Value.monsterSpawntPoint[i], item.Value.monsterSpawntPoint[i + 1]);
                    place.Add(coordinate);
                }

                if (!localServerSpawnPoint.ContainsKey(item.Key))
                    localServerSpawnPoint.Add(item.Key, new SpawnPoint(item.Key, item.Value.triggerType));

                if (!localServerMonster.ContainsKey(item.Key))
                    localServerMonster.Add(item.Key, new Dictionary<uint, LocalServerEntity>());

                if (!localServerTokenTemplete.ContainsKey(item.Key))
                    localServerTokenTemplete.Add(item.Key, new List<LocalServerEntity>());

                int monsterCount = 0;
                for (int i = 0; i < monsterIDs.Count; i++)
                {
                    for (int j = 0; j < monsterNums[i]; j++)
                    {
                        if (monsterCount < place.Count)
                        {
                            LocalServerDummy dummy = new LocalServerDummy();
                            dummy.ID = getNextEntityId();
                            dummy.spawnPointID = item.Key;
                            dummy.monsterID = monsterIDs[i];
                            dummy.state = MonsterState.sleep;
                            dummy.x = place[monsterCount].Key;
                            dummy.y = place[monsterCount].Value;
                            dummy.exp = LocalServerResManager.GetMonsterData(dummy.monsterID).exp;

                            monsterCount++;

                            if (!localServerEntity.ContainsKey(dummy.ID)
                                && !localServerMonster[item.Key].ContainsKey(dummy.ID))
                            {
                                localServerEntity.Add(dummy.ID, dummy);
                                localServerMonster[item.Key].Add(dummy.ID, dummy);
                                localServerTokenTemplete[item.Key].Add(dummy);
                            }
                            //else
                            //{
                            //    dummy.state = MonsterState.invalid;
                            //    dummy.x = 0;
                            //    dummy.x = 0;
                            //}
                        }
                        //else
                        //{
                        //}
                    }
                }
            }
        }

        protected void ConstructContainerData(Dictionary<int, int> containers)
        {
            if (containers == null)
                return;

            foreach (var item in containers)
                localServerContainer.Add(item.Key, item.Value);
        }

        protected void EnterResetAvatarData(int reviveTimes)
        {
            localServerAvatar = new LocalServerAvatar(reviveTimes);
        }

        #endregion
         

        #region 开始副本

        protected void CheckSpawnPointDefaultSpawn()
        {
            foreach (var item in localServerSpawnPoint)
                if (item.Value.state == SpawnPointState.activeWhenStart)
                    TriggerSpawnPoint(item.Key);
        }

        #endregion


        #region 退出副本

        public void ExitMission()
        {
            ClearSceneData();
            ClearDropData();
            ExitResetAvatarData();
            ResetIDPool();
        }

        public void ClearSceneData()
        {
            CliEntityManager.Instance.ClearData();

            localServerEntity.Clear();
            localServerEvent.Clear();
            localServerMissionTarget.Clear();
            localServerSpawnPoint.Clear();

            localServerMonster.Clear();
            localServerContainer.Clear();
            localServerTokenTemplete.Clear();
            localServerSummonToken.Clear();
        }

        public void ClearDropData()
        {
            localAvatarCollectedDrops.Clear();
            localAvatarCollectedMoney = 0;
            localAvatarCollectedExp = 0;

            srvPreCollectedDrops.Clear();
            preMoney = 0;
            preExp = 0;
        }

        public void ExitResetAvatarData()
        {
            localServerAvatar = null;
            localAvatarCombo = 0;
            localAvatarBottle = 0;

            MogoWorld.thePlayer.stateFlag = Mogo.Util.Utils.BitReset(MogoWorld.thePlayer.stateFlag, 0);
            MogoWorld.thePlayer.curHp = MogoWorld.thePlayer.hp;
        }

        #endregion

        #endregion


        #region 怪物出生点，怪物，容器，召唤怪和掉落

        public void TriggerSpawnPoint(int spawnPointID)
        {
            if (!localServerSpawnPoint.ContainsKey(spawnPointID))
                return;

            if (!localServerMonster.ContainsKey(spawnPointID))
                return;

            List<int> preSpawnPointIds = LocalServerResManager.SpaceDataMap[spawnPointID].preSpawnPointId;

            if (preSpawnPointIds != null)
            {
                bool isAllTrigger = true;
                foreach (int preID in preSpawnPointIds)
                {
                    if (!localServerSpawnPoint.ContainsKey(preID))
                        continue;
                    if (localServerSpawnPoint[preID].state == SpawnPointState.active)
                        continue;
                    isAllTrigger = false;
                }

                if (!isAllTrigger)
                    return;
            }

            localServerSpawnPoint[spawnPointID].state = SpawnPointState.active;

            foreach (var item in localServerMonster[spawnPointID])
            {
                if (item.Value is LocalServerDummy)
                    WakeUpDummy(item.Value as LocalServerDummy, spawnPointID);
            }
        }


        protected void WakeUpDummy(LocalServerDummy dummy, int spawnPointID)
        {
            if (dummy.state != MonsterState.sleep)
                return;

            dummy.state = MonsterState.active;

            List<List<int>> dummys = new List<List<int>>();
            List<int> dummyMessage = new List<int>();
            dummyMessage.Add((int)(EntityType.Dummy));
            dummyMessage.Add((int)dummy.ID);
            dummyMessage.Add(dummy.x);
            dummyMessage.Add(dummy.y);
            dummyMessage.Add(dummy.monsterID);
            dummyMessage.Add(0);
            dummyMessage.Add(spawnPointID);

            dummys.Add(dummyMessage);

            LuaTable args;
            Mogo.RPC.Utils.PackLuaTable(dummys, out args);

            RPCCall("CreateCliEntityResp", args);

            // 据说血量由前端读取，所以这里直接丢个0
            // MogoWorld.CreateDummy(dummy.ID, dummy.x, dummy.y, dummy.monsterID, 0);
        }


        protected void DummyDead(uint dummyID, int x = 0, int y = 0)
        {
            if (!localServerEntity.ContainsKey(dummyID))
                return;

            if (localServerEntity[dummyID] is LocalServerDummy)
            {
                LocalServerDummy dummy = localServerEntity[dummyID] as LocalServerDummy;
                dummy.state = MonsterState.dead;
                dummy.x = x;
                dummy.y = y;
                localAvatarCollectedExp += GetOneDropExpFromPool(dummy.exp);

                CliEntityManager.Instance.onActionDie(dummy, (int)MogoWorld.thePlayer.vocation);

                int spawnPointID = dummy.spawnPointID;
                if (localServerMonster.ContainsKey(spawnPointID) && localServerMonster[spawnPointID].ContainsKey(dummyID))
                {
                    if (localServerMonster[spawnPointID][dummyID] is LocalServerDummy)
                    {
                        LocalServerDummy dummyData2 = (localServerMonster[spawnPointID][dummyID] as LocalServerDummy);
                        dummyData2.state = MonsterState.dead;
                        dummyData2.x = x;
                        dummyData2.y = y;
                    }
                    localServerMonster[spawnPointID].Remove(dummyID);
                    CheckSpawnPointEvent(spawnPointID);
                }
            }

            localServerEntity.Remove(dummyID);
        }


        protected void CheckSpawnPointEvent(int spawnPointID)
        {
            if (!localServerMonster.ContainsKey(spawnPointID))
                return;

            if (localServerMonster[spawnPointID].Count == 0)
            {
                CheckMissionWon(spawnPointID);

                List<int> result = new List<int>();
                result.Add(spawnPointID);
                LuaTable luaTable;
                Mogo.RPC.Utils.PackLuaTable(result, out luaTable);
                MissionResp(MissionHandleCode.NOTIFY_TO_CLENT_SPAWNPOINT, luaTable);

                CheckMissionEvent(LocalServerMissionEventType.SpawnPoint, spawnPointID);
            }
        }


        protected void CheckMissionEvent(LocalServerMissionEventType type, int number)
        {
            foreach (var item in localServerEvent)
            {
                if (item.spawnPoint == null)
                    continue;

                if (item.spawnPoint.Contains(number))
                {
                    item.spawnPoint.Remove(number);

                    if (item.spawnPoint.Count != 0)
                        continue;

                    if (item.notifyToClient != null)
                    {
                        foreach (var trapEventID in item.notifyToClient)
                        {
                            List<int> result = new List<int>();
                            result.Add(trapEventID);
                            LuaTable luaTable;
                            Mogo.RPC.Utils.PackLuaTable(result, out luaTable);
                            MissionResp(MissionHandleCode.GET_NOTIFY_TO_CLENT_EVENT, luaTable);

                            // ClientEventData.TriggerGearEvent(trapEventID);
                        }
                    }

                    if (item.notifyOtherSpawnPoint != null)
                    {
                        foreach (var spawnPointID in item.notifyOtherSpawnPoint)
                            TriggerSpawnPoint(spawnPointID);
                    }
                }
            }
        }


        public void AvatarCollectDropItem(int itemId)
        {
            if (!localAvatarCollectedDrops.ContainsKey(itemId))
                localAvatarCollectedDrops[itemId] = 0;

            localAvatarCollectedDrops[itemId]++;
        }


        protected void CreateClientDrop(ushort arg1, string coordinateString)
        {
            string[] coordinate = coordinateString.Split('_');
            if (coordinate.Length != 2)
                return;

            if (!localServerContainer.ContainsKey(arg1))
                return;

            if (localServerContainer[arg1] <= 0)
                return;

            localServerContainer[arg1]--;

            int x = Convert.ToInt32(coordinate[0]);
            int z = Convert.ToInt32(coordinate[1]);

            CliEntityManager.Instance.onActionDie(arg1, x, z, (int)MogoWorld.thePlayer.vocation);
        }


        protected void SummonToken(int spawnPointID)
        {
            if (GetCurrentAllMonsterNum() > tokenJudgeMaxNum)
                return;

            if (!localServerTokenTemplete.ContainsKey(spawnPointID))
                return;

            foreach (var monsterTemplete in localServerTokenTemplete[spawnPointID])
            {
                if (monsterTemplete is LocalServerDummy)
                {
                    LocalServerDummy summonTemplate = monsterTemplete as LocalServerDummy;
                    LocalServerDummy token = new LocalServerDummy(summonTemplate);
                    token.ID = getNextEntityId();
                    localServerSummonToken.Add(token.ID, token);

                    List<List<int>> tokens = new List<List<int>>();
                    List<int> tokenMessage = new List<int>();
                    tokenMessage.Add((int)(EntityType.Dummy));
                    tokenMessage.Add((int)token.ID);
                    tokenMessage.Add(token.x);
                    tokenMessage.Add(token.y);
                    tokenMessage.Add(token.monsterID);
                    tokenMessage.Add(0);
                    tokenMessage.Add(spawnPointID);
                    LoggerHelper.Error("SummonToken:" + spawnPointID);

                    tokens.Add(tokenMessage);

                    LuaTable args;
                    Mogo.RPC.Utils.PackLuaTable(tokens, out args);

                    RPCCall("CreateCliEntityResp", args);
                }
            }
        }

        protected int GetCurrentAllMonsterNum()
        {
            int entityNum = 0;
            int tokenNum = 0;

            if (localServerEntity != null)
            {
                foreach (var entityData in localServerEntity)
                {
                    if (entityData.Value is LocalServerDummy)
                    {
                        if ((entityData.Value as LocalServerDummy).state == MonsterState.active)
                            entityNum++;
                    }
                }
            }

            if (localServerSummonToken != null)
                tokenNum = localServerSummonToken.Count;

            return entityNum + tokenNum;
        }


        protected void SummonTokenDead(uint tokenID)
        {
            if (localServerSummonToken.ContainsKey(tokenID))
                localServerSummonToken.Remove(tokenID);
        }

        #endregion


        #region 钱经验道具池以及掉落相关

        public void InitSrvPreCollect(Dictionary<int, int> srvPreCollectedDrops, int preMoney, int preExp)
        {
            //LoggerHelper.Error("InitSrvPreCollect preMoney:" + preMoney + " preExp:" + preExp);
            this.srvPreCollectedDrops = srvPreCollectedDrops;
            this.preMoney = preMoney;
            this.preExp = preExp;
        }

        //获取真正的掉落物信息
        public List<int> GetDropByEntityId(uint entityId)
        {
            return CliEntityManager.Instance.GetDropByEntityId(entityId);
        }

        //从掉落物池中取出一个掉落物品
        public bool GetOneDropItemFromPool(int itemId)
        {
            if (itemId > 0)
            {
                if (srvPreCollectedDrops.ContainsKey(itemId) && srvPreCollectedDrops[itemId] > 0)
                {
                    AvatarCollectDropItem(itemId);//增加一个拾取的道具
                    srvPreCollectedDrops[itemId]--;
                    return true;
                }
            }


            return false;
        }

        //从掉落物池中取出一些钱
        public int GetOneDropMoneyFromPool(int money)
        {
            if (money > 0 && preMoney > 0)
            {
                if (preMoney >= money)
                {
                    preMoney -= money;
                    localAvatarCollectedMoney += money;
                    //LoggerHelper.Error("money:" + money);
                    return money;
                }
                else
                {
                    preMoney = 0;
                    localAvatarCollectedMoney += preMoney;
                    //LoggerHelper.Error("money:" + preMoney);
                    return preMoney;
                }

            }
            //LoggerHelper.Error("money:" + 0);
            return 0;
        }

        //从掉落物池中取出一些经验
        public int GetOneDropExpFromPool(int exp)
        {
            if (exp > 0 && preExp > 0)
            {
                if (preExp >= exp)
                {
                    preExp -= exp;
                    localAvatarCollectedExp += exp;
                    //LoggerHelper.Error("exp:" + exp);
                    return exp;
                }
                else
                {
                    //LoggerHelper.Error("exp:" + preExp);
                    preExp = 0;
                    localAvatarCollectedExp += preExp;
                    return preExp;
                }

            }

            //LoggerHelper.Error("exp:" + 0);
            return 0;
        }

        #endregion//钱经验道具池以及掉落相关


        #region 胜利与结算相关

        protected void CheckMissionWon(int spawnPointID)
        {
            if (!localServerMissionTarget.Contains(spawnPointID))
                return;

            localServerMissionTarget.Remove(spawnPointID);
            if (localServerMissionTarget.Count == 0)
                MissionWon();
        }

        protected void MissionWon()
        {
            MissionResp(MissionHandleCode.NOTIFY_TO_CLIENT_TO_UPLOAD_COMBO, new LuaTable());
            RPCCall("BossDieResp");
        }

        protected void SetCombo(int combo)
        {
            localAvatarCombo = combo;
        }

        protected void NotifyClientMissionWon()
        {
            MissionResp(MissionHandleCode.NOTIFY_TO_CLIENT_RESULT_SUCCESS, new LuaTable());
        }

        protected void SendClientMissionMessage()
        {
            // 真正向服务器发, to do
            LuaTable result = new LuaTable();

            LuaTable temp;
            Mogo.RPC.Utils.PackLuaTable(localAvatarCollectedDrops, out temp);
            result.Add(1, temp);
            result.Add(2, localAvatarCollectedMoney);
            result.Add(3, localAvatarCollectedExp);

            MogoWorld.thePlayer.RpcCall("MissionExReq", (byte)MissionHandleCode.UPLOAD_COMBO_AND_BOTTLE, (ushort)localAvatarCombo, (ushort)localAvatarBottle, (ushort)(localServerAvatar.defaultReviveTimes - localServerAvatar.reviveTimes), Mogo.RPC.Utils.PackLuaTable(result));
        }

        protected void GetMissionReward()
        {
            TimerHeap.AddTimer(10, 0, () =>
            {
                // 真正向服务器发, to do
                MogoWorld.thePlayer.RpcCall("MissionReq", (byte)MissionHandleCode.GET_MISSION_REWARDS, (ushort)1, (ushort)1, "");
            });
        }

        #endregion


        #region 玩家

        protected void HitAvatar(uint curHp)
        {
            if (localServerAvatar == null)
                return;

            if (MogoWorld.thePlayer.IsNewPlayer)
                return;

            localServerAvatar.curHp = curHp;
            MogoWorld.thePlayer.curHp = curHp;

            if (curHp == 0)
            {
                localServerAvatar.deathFlag = 1;
                //MogoWorld.thePlayer.deathFlag = 1;
                MogoWorld.thePlayer.stateFlag = Mogo.Util.Utils.BitSet(MogoWorld.thePlayer.stateFlag, 0);
            }
        }

        protected void NotifyReviveTime()
        {
            if (localServerAvatar == null)
                return;

            int hasRevivedTimes = localServerAvatar.defaultReviveTimes - localServerAvatar.reviveTimes;

            List<int> result = new List<int>();
            result.Add(hasRevivedTimes);
            LuaTable luaTable;
            Mogo.RPC.Utils.PackLuaTable(result, out luaTable);
            MissionResp(MissionHandleCode.GET_NOTIFY_TO_CLENT_EVENT, luaTable);

            MissionResp(MissionHandleCode.GET_REVIVE_TIMES, luaTable);
        }

        protected void AvatarRevive()
        {
            if (localServerAvatar == null)
                return;

            if (InventoryManager.Instance.GetItemNumById(1100070) < 0)
            {
                List<int> result = new List<int>();
                result.Add(-1);
                LuaTable luaTable;
                Mogo.RPC.Utils.PackLuaTable(result, out luaTable);
                MissionResp(MissionHandleCode.REVIVE, luaTable);
                return;
            }

            if (localServerAvatar.deathFlag == 0)
            {
                List<int> result = new List<int>();
                result.Add(-2);
                LuaTable luaTable;
                Mogo.RPC.Utils.PackLuaTable(result, out luaTable);
                MissionResp(MissionHandleCode.REVIVE, luaTable);
                return;
            }

            if (localServerAvatar.reviveTimes <= 0)
            {
                List<int> result = new List<int>();
                result.Add(-3);
                LuaTable luaTable;
                Mogo.RPC.Utils.PackLuaTable(result, out luaTable);
                MissionResp(MissionHandleCode.REVIVE, luaTable);
                return;
            }

            // to do, 没去扣减道具
            localServerAvatar.reviveTimes--;
            localServerAvatar.curHp = MogoWorld.thePlayer.hp;
            if (localServerAvatar.curHp != 0)
            {
                localServerAvatar.deathFlag = 0;
                //MogoWorld.thePlayer.deathFlag = 0;
                MogoWorld.thePlayer.curHp = localServerAvatar.curHp;
                MogoWorld.thePlayer.stateFlag = Mogo.Util.Utils.BitReset(MogoWorld.thePlayer.stateFlag, 0);

                List<int> result = new List<int>();
                result.Add(0);
                LuaTable luaTable;
                Mogo.RPC.Utils.PackLuaTable(result, out luaTable);
                MissionResp(MissionHandleCode.REVIVE, luaTable);
            }
        }

        #endregion


        #region 测试

        #endregion

    }
}
