/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������DoorOfBurySystem
// �����ߣ�Joe Mo
// �޸����б��
// �������ڣ�2013.5.20
// ģ������������֮��ϵͳ
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class DoorOfBurySystem
{
    public const string ON_CHALLENGE_SHOW = "DoorOfBurySystem.ON_CHALLENGE_SHOW";
    public const string ON_ENTER_DOOR = "DoorOfBurySystem.ON_ENTER_DOOR";
    public const string ON_SELECT = "DoorOfBurySystem.ON_SELECT";
    public static DoorOfBurySystem Instance;

    public static string CDING = "CDING";
    public static string DOOR_NO_OPEN = "DOOR_NO_OPEN";
    public static string HAS_NO_DOOR = "HAS_NO_DOOR";
    public static string HAS_DOOR_IN_CD = "HAS_DOOR_IN_CD";
    public static string HAS_DOOR_NO_CD = "HAS_DOOR_NO_CD";
    public static string CD_COUNT = "CD_COUNT";

    private bool m_hasAnyDoor = false;
    //private bool m_isShowingDoorList = false;
    private int m_currentCopyId = -1;
    public bool CanEnterAnyDoor
    {
        get
        {
            return m_hasAnyDoor && CD <= 0;
        }
    }

    public enum DoorOfBurryKillWay
    {
        Kill = 0, // �Լ�����ɱ��Boss��Boss���Լ�ɱ��
        Help = 1, // �Լ�����ɱ��Boss��Boss�������Լ�ɱ��
        None = 2, // �Լ�û�в���ɱ��Boss
    }

    public class DoorOfBurryData
    {
        public uint leftTime { get; set; }

        /// <summary>
        /// 0�����1��ɣ�2��ʱ
        /// </summary>
        public int state { get; set; }
        public string ownerName { get; set; }
        public int ownerVocation { get; set; }
        public int level { get; set; }
        public int bossMode { get; set; }//1��ħ��2а��

        /// <summary>
        /// 0��1��
        /// </summary>
        public int hasEntered { get; set; }
        public double progress { get; set; }
        public ulong killTheBossPlayerId { get; set; }
        public int copyId;

        // ɱ��Boss�ķ�ʽ
        public DoorOfBurryKillWay killWay
        {
            get
            {
                if (state == 1)
                {
                    if (killTheBossPlayerId == MogoWorld.thePlayer.dbid)
                    {
                        return DoorOfBurryKillWay.Kill;
                    }
                    if (hasEntered == 1)
                    {
                        return DoorOfBurryKillWay.Help;
                    }

                }
                return DoorOfBurryKillWay.None;
            }
        }
        // �Ƿ�ʤ��(ʤ�����������)
        public bool isWin
        {
            get
            {
                return progress <= 0;
            }
        }
        // �Ƿ�ʧ��(ʧ�ܣ�����δ�����ʱ�����)
        public bool isLose
        {
            get
            {
                return leftTime <= 0 && progress > 0;
            }
        }
        // ս����Ϣ
        public string battleInfo
        {
            get
            {
                switch (state)
                {
                    case 0:
                        return LanguageData.GetContent(47104);// ����->"ս���У�"
                    case 1:
                        return LanguageData.GetContent(47105);// ���->"�ѽ�����"
                    case 2:
                        return LanguageData.GetContent(47105);// ��ʱ->"�ѽ�����"
                    default:
                        return LanguageData.GetContent(47106);// "δ֪"
                }
            }
        }
        // Boss����
        public string bossName
        {
            get
            {
                if (bossMode == 1)
                    return LanguageData.GetContent(47107);// "��ħ"
                else
                    return LanguageData.GetContent(47108);// "а��"
            }
        }
    }

    public List<DoorOfBurryData> m_doorList = new List<DoorOfBurryData>();

    public DoorOfBurryData m_selectDoor;

    public float time;
    private int m_cd;
    public int CD
    {
        get
        {
            //Mogo.Util.LoggerHelper.Debug("time:" + MogoTime.Instance.GetSecond());
            m_cd = (int)(m_cd - (MogoTime.Instance.GetSecond() - time));
            if (m_cd < 0) m_cd = 0;
            return m_cd;
        }
        set
        {
            m_cd = value;
            time = MogoTime.Instance.GetSecond();
            //Mogo.Util.LoggerHelper.Debug("time:" + time);
        }
    }

    public DoorOfBurySystem()
    {
        Instance = this;
        CDING = LanguageData.GetContent(1001107);
        DOOR_NO_OPEN = LanguageData.GetContent(312); // δ����
        HAS_NO_DOOR = LanguageData.GetContent(47115); // û��Boss->δ���ֶ�ħ
        HAS_DOOR_IN_CD = LanguageData.GetContent(1001109); // ��Boss->����֮���ѿ���
        HAS_DOOR_NO_CD = LanguageData.GetContent(47116); // ��Boss->����ս��ħ
        CD_COUNT = LanguageData.GetContent(47118);//1001110
        AddEventListener();
    }

    public void OnDoorOfBuryCdResp(int cd, bool hasAnyDoor)
    {
        //Debug.LogError("OnDoorOfBuryCdResp");
        CD = cd;
        this.m_hasAnyDoor = hasAnyDoor;

        if (CD <= 0 && hasAnyDoor)
        {
            EventDispatcher.TriggerEvent<int>(Events.NormalMainUIEvent.ShowChallegeIconTip, (int)ChallengeGridID.DoorOfBury);
        }
        else
        {
            EventDispatcher.TriggerEvent<int>(Events.NormalMainUIEvent.HideChallegeIconTip, (int)ChallengeGridID.DoorOfBury);
        }

        if (ChallengeUIViewManager.Instance == null) return;
        if (!MogoUIManager.Instance.IsWindowOpen((int)WindowName.Challenge)) return;
        if (MogoWorld.thePlayer.level >= SystemRequestLevel.DOOROFBURY) // �ѽ��� 
        {
            SetUpCdUI();
            if (CD <= 0 && hasAnyDoor) SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.LimitStarted);
            else SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.LimitFinished);


        }
        else // δ����
        {
            SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Close);
            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.DoorOfBury, true);
            ChallengeUIViewManager.Instance.BeginCountdown(DOOR_NO_OPEN, DOOR_NO_OPEN, DOOR_NO_OPEN, (int)ChallengeGridID.DoorOfBury, 0, 0, 0);
            ChallengeUIViewManager.Instance.SetEndText((int)ChallengeGridID.DoorOfBury, DOOR_NO_OPEN); // 25������
            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.DoorOfBury, SystemUIColorManager.RED);
        }


     
        //AddDoorFx();
    }

    private void SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState challengeState)
    {
        ChallengeUIGridMessage msg = new ChallengeUIGridMessage();
        msg.challengeID = ChallengeGridID.DoorOfBury;
        msg.state = challengeState;
        EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, msg);
    }

    private void AddDoorFx()
    {
        GameObject city = GameObject.Find("10004_City");
        if (city == null) return;
        Transform door = city.transform.Find("transfer");
        if (door == null) return;
        foreach (Transform t in door)
        {
            if (CanEnterAnyDoor)
            {
                SfxHandler sfx = t.GetComponent<SfxHandler>();
                if (sfx == null)
                {
                    sfx = t.gameObject.AddComponent<SfxHandler>();
                }
                sfx.HandleFx(101104);
            }
            if (t.GetComponent<DoorOfBury>() == null)
            {
                t.gameObject.AddComponent<DoorOfBury>();
            }
        }
        //door.GetComponent<DoorOfBury>().BurnTheDoor(CanEnterAnyDoor);
    }

    public void RemoveListener()
    {
        EventDispatcher.RemoveEventListener(ON_CHALLENGE_SHOW, OnChallengeShow);
        EventDispatcher.RemoveEventListener(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.ENTERDOOROFBURY, OnDoorShow);
        EventDispatcher.AddEventListener(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.ENTERDOOROFBURYBYTIP, OnDoorShow);
        EventDispatcher.RemoveEventListener(ON_ENTER_DOOR, OnEnterDoor);
        EventDispatcher.RemoveEventListener<int>(ON_SELECT, OnSelect);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
    }

    private void AddEventListener()
    {
        EventDispatcher.AddEventListener(ON_CHALLENGE_SHOW, OnChallengeShow);
        EventDispatcher.AddEventListener(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.ENTERDOOROFBURYBYTIP, OnDoorShow);
        EventDispatcher.AddEventListener(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.ENTERDOOROFBURY, OnDoorShow);
        EventDispatcher.AddEventListener(ON_ENTER_DOOR, OnEnterDoor);
        EventDispatcher.AddEventListener<int>(ON_SELECT, OnSelect);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);

        EventDispatcher.AddEventListener(Events.ChallengeUIEvent.CollectChallengeState, OnCollectChallengeState);
    }

    private void OnCollectChallengeState()
    {
        if (CD <= 0 && m_hasAnyDoor) SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.LimitStarted);
        else SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.LimitFinished);
    }

    private void InstanceLoaded(int copyId, bool isInCopy)
    {
        if (!isInCopy)
        {
            //����cd
            MogoWorld.thePlayer.RpcCall("OblivionGateReq", 3, 0, 0);
            if (MainUIViewManager.Instance != null)
            {
                MainUIViewManager.Instance.SetHpBottleVisible(true);
                MogoWorld.thePlayer.RemoveFx(6029);
            }
        }
        else
        {
            if (MapData.dataMap.Get(copyId).type == MapType.BURY)
            {
                MainUIViewManager.Instance.SetHpBottleVisible(false);
                MogoWorld.thePlayer.PlayFx(6029);
            }
        }
    }

    private void OnSelect(int index)
    {
        Mogo.Util.LoggerHelper.Debug("OnSelect:" + index);
        m_selectDoor = m_doorList[index];
        m_currentCopyId = m_selectDoor.copyId;
    }

    private void OnEnterDoor()
    {
        Mogo.Util.LoggerHelper.Debug("OnEnterDoor");
        //if (CD > 0)
        //{
        //    MogoMsgBox.Instance.ShowMsgBox("CD��!");
        //    return;
        //}
        //if (m_selectDoor.isWin)
        //{
        //    MogoMsgBox.Instance.ShowMsgBox("ս����ʤ����");
        //    return;
        //}
        //if (m_selectDoor.isLose)
        //{
        //    MogoMsgBox.Instance.ShowMsgBox("ս����ʧ�ܣ�");
        //    return;
        //}
        //todo ֪ͨ������
        if (m_currentCopyId < 0)
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(47109));// "��ѡ������֮��"
        }
        else
        {
            MogoWorld.thePlayer.RpcCall("OblivionGateReq", 1, m_currentCopyId, 0);
        }

    }

    private void OnEnterDoorResp()
    {
        Mogo.Util.LoggerHelper.Debug("OnEnterDoorResp");
        //todo ������Ӧ�ؿ�
        //m_selectDoor.copyId
    }

    public void OnDoorShow()
    {
        Mogo.Util.LoggerHelper.Debug("OnDoorShow");
        if (m_hasAnyDoor)
        {
            //todo ��������ˢ��
            //m_isShowingDoorList = true;
            MogoWorld.thePlayer.RpcCall("OblivionGateReq", 2, 0, 0);

            //AddTestData();
            //OnDoorShowDataResp(m_doorList);
        }
        else
        {
            MogoUIManager.Instance.ShowMogoDoorOfBuryUI(() =>
                {
                    if (DoorOfBuryUIViewManager.Instance != null)
                    {
                        DoorOfBuryUIViewManager.Instance.SetGridNum(0);
                        DoorOfBuryUIViewManager.Instance.SetDoorTitle(LanguageData.GetContent(47111));// "����֮��"
                        DoorOfBuryUIViewManager.Instance.ClearDoorCD();
                    }
                });

            //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(1001002));
        }
    }

    private void AddTestData()
    {
        List<DoorOfBurryData> doorList = new List<DoorOfBurryData>();
        for (int i = 0; i < 2; ++i)
        {
            DoorOfBurryData data = new DoorOfBurryData() { leftTime = 10000, level = 10, bossMode = 1, copyId = 1, ownerName = "����������1", ownerVocation = 1, progress = 0.2, hasEntered = 1, killTheBossPlayerId = 1010, state = 1 };
            doorList.Add(data);
        }

        m_doorList = doorList;
    }

    private void RefreshDoorsUI()
    {
        m_currentCopyId = -1;
        DoorOfBuryUIViewManager view = DoorOfBuryUIViewManager.Instance;

        Mogo.Util.LoggerHelper.Debug("clear");
        view.ClearFriendList();
        Mogo.Util.LoggerHelper.Debug("clear done");

        // ��������֮������
        view.SetGridNum(m_doorList.Count);

        // ��������֮��Grid
        if (m_doorList.Count > 0)
        {
            for (int i = 0; i < m_doorList.Count; ++i)
            {
                FriendGridData fd = new FriendGridData();
                DoorOfBurryData data = m_doorList[i];
                fd.Id = i;

                //LoggerHelper.Error(data.leftTime);
                int leftTime = (int)data.leftTime;

                fd.hour = leftTime / 3600;
                fd.minus = (leftTime % 3600) / 60;
                //Debug.LogError("hour:" + fd.hour);
                //Debug.LogError("minus:" + fd.minus);
                //Mogo.Util.LoggerHelper.Debug("data.progress:" + data.progress);
                fd.bossHP = (int)(data.progress * 100) + "";//* 100 + "%"

                fd.bossLevel = "     level:" + data.level;
                //fd.bossName = LanguageData.dataMap.Get(monster.hpShow.Get(1)).content;
                fd.bossName = data.bossName;
                //Mogo.Util.LoggerHelper.Debug("bossName:" + fd.bossName);
                fd.name = data.ownerName;
                fd.headImg = IconData.GetHeadImgByVocation(data.ownerVocation);
                Mogo.Util.LoggerHelper.Debug(fd.headImg);

                fd.isHelp = false;
                fd.isKill = false;
                fd.isLose = false;
                fd.isWin = false;

                switch ((int)data.killWay)
                {
                    case (int)DoorOfBurryKillWay.Help:
                        fd.isHelp = true;
                        break;
                    case (int)DoorOfBurryKillWay.Kill:
                        fd.isKill = true;
                        break;
                    case (int)DoorOfBurryKillWay.None:
                        break;
                }

                //�������������   
                fd.battleInfo = data.battleInfo;
                fd.isWin = data.isWin;
                fd.isLose = data.isLose;

                view.AddFriendListGrid(fd);
            }
        }


        int minute = CD / 60;
        if (minute > 0)
        {
            view.SetDoorTitle(LanguageData.GetContent(47110));// "����֮�ŷ�ӡ��"
            view.BeginCountDown(minute + 1);
        }
        else
        {
            view.SetDoorTitle(LanguageData.GetContent(47111));// "����֮��"
            view.ClearDoorCD();
        }

        if (MogoUIManager.Instance != null)
        {
            //MogoUIManager.Instance.ShowMogoCommuntiyUI(CommunityUIParent.NormalMainUI, false);

            MogoUIManager.Instance.m_CommunityUI.SetActive(false);
            NormalMainUIViewManager.Instance.ShowCommunityButton(true);
        }

    }

    private void OnDoorShowDataResp(List<DoorOfBurryData> doorList)
    {
        m_doorList = doorList;
        //for (int i = 0; i < m_doorList.Count; i++)
        //{
        //    Debug.LogError(m_doorList[i].leftTime);
        //}
        m_doorList.Sort((DoorOfBurryData a, DoorOfBurryData b) =>
        {
            if (a.state != 0 && b.state == 0) return 1;
            if (a.state == 0 && b.state != 0) return -1;

            if (a.leftTime > b.leftTime) return -1;
            else return 1;

        });
        //if (doorList == null || doorList.Count <= 0)
        //{
        //    MogoMsgBox.Instance.ShowMsgBox("����֮��δ�����");
        //    return;
        //}
        MogoUIManager.Instance.ShowMogoDoorOfBuryUI(RefreshDoorsUI);

    }

    /// <summary>
    /// ����սʱ
    /// </summary>
    private void OnChallengeShow()
    {
        //todo ������������֮����ȴcd
        MogoWorld.thePlayer.RpcCall("OblivionGateReq", 3, 0, 0);
        Mogo.Util.LoggerHelper.Debug("OnChallengeShow");
        m_isShowingChallengeUI = true;
    }

    private void OnChallengeClose()
    {
        m_isShowingChallengeUI = false;
    }

    //private void OnEnterCity()
    //{
    //    Mogo.Util.LoggerHelper.Debug("OnEnterCity");
    //}

    /// <summary>
    /// ��ʾ�ѷ�������֧Ԯ
    /// </summary>
    public void ShowOpenToFriendTip()
    {
        Mogo.Util.LoggerHelper.Debug("ShowOpenToFriendTip");
        NormalMainUIViewManager.Instance.ShowSendHelp(true);
        TimerHeap.AddTimer(10000, 0, () =>
        {
            NormalMainUIViewManager.Instance.ShowSendHelp(false);
        });
    }

    /// <summary>
    /// ��ʾ�õ���������
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="gold"></param>
    /// <param name="exp"></param>
    public void ShowRecordTip(uint damage, uint gold, uint exp)
    {
        //Debug.LogError("ShowRecordTip");
        BattleRecordData recordData = new BattleRecordData();
        recordData.OutputText = LanguageData.dataMap[601].Format(damage);
        recordData.GoldText = LanguageData.dataMap[602].Format(gold);
        recordData.ExpText = LanguageData.dataMap[603].Format(exp);
        recordData.Title = LanguageData.dataMap[600].content;
        recordData.OutputNum = "";
        recordData.GoldNum = "";
        recordData.ExpNum = "";

        BattleRecordUIViewManager.Instance.SetBattleRecordData(recordData);
        BattleRecordUIViewManager.Instance.ShowBattleRecord(true);
    }

    private void OnCloseToDoor()
    {
        Mogo.Util.LoggerHelper.Debug("OnCloseToDoor");
        //todo ��������֮��ʱ
    }

    //private void OnBossDead()
    //{
    //    Mogo.Util.LoggerHelper.Debug("OnBossDead");
    //    //todo boss���˺���ʾ
    //}

    //private void OnReceiveDoor(DoorOfBurryData data)
    //{
    //    m_doorList.Add(data);
    //    if (data.friendId == MogoWorld.thePlayer.dbid)
    //    {
    //        hasDoorOpen = true;
    //    }
    //}

    public void OblivionGateCreate(uint copyId, int source)
    {
        NormalMainUIViewManager.Instance.ShowDoorOfBuryTip();

        if (CD <= 0)
        {
            EventDispatcher.TriggerEvent<int>(Events.NormalMainUIEvent.ShowChallegeIconTip, (int)ChallengeGridID.DoorOfBury);
        }
    }

    public void OblivionGateListResp(uint cd, LuaTable luaTable)
    {
        Mogo.Util.LoggerHelper.Debug("OblivionGateListResp:" + cd);
        CD = (int)cd;

        //SetUpCdUI();
        //AddTestData();
        SetDoorList(luaTable);

        OnDoorShowDataResp(m_doorList);

        //if (m_isShowingDoorList)
        //{
        //    OnDoorShowDataResp(doorList);
        //}
    }

    private void SetDoorList(LuaTable luaTable)
    {
        object obj;
        Mogo.Util.LoggerHelper.Debug(luaTable);
        Utils.ParseLuaTable(luaTable, typeof(Dictionary<int, DoorOfBurryData>), out obj);
        Dictionary<int, DoorOfBurryData> doorDic = obj as Dictionary<int, DoorOfBurryData>;
        if (m_doorList == null) m_doorList = new List<DoorOfBurryData>();
        m_doorList.Clear();
        int count = 0;
        foreach (KeyValuePair<int, DoorOfBurryData> pair in doorDic)
        {
            m_doorList.Add(pair.Value);
            m_doorList[count].copyId = pair.Key;
            count++;
        }
    }

    private void SetUpCdUI()
    {
        ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.DoorOfBury, false);
        ChallengeUIViewManager.Instance.BeginCountdown(HAS_DOOR_IN_CD, HAS_DOOR_IN_CD, HAS_DOOR_IN_CD, (int)ChallengeGridID.DoorOfBury, 0, 0, 0);
        ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.DoorOfBury, false);
        if (CD > 0)
        {
            //int hour = CD / 3600;
            int minute = CD / 60;
            Mogo.Util.LoggerHelper.Debug("minute:" + minute);
            //int second = CD % 60;
            CD_COUNT = LanguageData.GetContent(47118, minute);
            ChallengeUIViewManager.Instance.SetEndText((int)ChallengeGridID.DoorOfBury, CD_COUNT);
            TimerHeap.AddTimer((uint)CD * 1000, 0, () =>
            {
                MogoWorld.thePlayer.RpcCall("OblivionGateReq", 3, 0, 0);
            });
            //ChallengeUIViewManager.Instance.BeginCountdown(CD_COUNT, string.Empty, HAS_DOOR_NO_CD, (int)ChallengeGridID.DoorOfBury, hour, minute, second, 1, () =>
            //{
            //    if (m_hasAnyDoor)
            //    {
            //        ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.DoorOfBury, true);
            //        ChallengeUIViewManager.Instance.SetEndText((int)ChallengeGridID.DoorOfBury, HAS_DOOR_NO_CD);
            //        ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.DoorOfBury, SystemUIColorManager.BROWN);
            //    }
            //    else // m_doorList == null || m_doorList.Count <= 0
            //    {
            //        ChallengeUIViewManager.Instance.SetEndText((int)ChallengeGridID.DoorOfBury, HAS_NO_DOOR);
            //        ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.DoorOfBury, SystemUIColorManager.BROWN);
            //    }
            //});
        }
        else
        {
            if (m_hasAnyDoor)
            {
                ChallengeUIViewManager.Instance.SetEndText((int)ChallengeGridID.DoorOfBury, HAS_DOOR_NO_CD);
                ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.DoorOfBury, SystemUIColorManager.BROWN);
                ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.DoorOfBury, true);
            }
            else //m_doorList == null || m_doorList.Count <= 0
            {
                ChallengeUIViewManager.Instance.SetEndText((int)ChallengeGridID.DoorOfBury, HAS_NO_DOOR);
                ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.DoorOfBury, SystemUIColorManager.BROWN);
            }
        }
    }

    public bool m_isShowingChallengeUI = false;
}

