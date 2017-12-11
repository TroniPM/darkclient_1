using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;
using System;

public class CommunityMessageData
{
    public ChannelId Channel;
    public ulong DBid;
    public string SenderName;
    public byte SenderLevel;
    public string Message;
}

public enum ChannelId : byte
{
    PERSONAL = 1,
    WORLD = 2,
    TEAM = 3,
    UNION = 4,
    SYSTEM = 5,
    TOWER_DEFENCE = 6,
    OCCUPY_TOWER = 7
}

public class CommunityUILogicManager
{
    private static CommunityUILogicManager m_instance;

    public static CommunityUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new CommunityUILogicManager();
            }

            return CommunityUILogicManager.m_instance;

        }
    }

    #region 频道颜色

    // 私聊
    readonly static string PERSONAL_COLOR = "ED79D3";
    public readonly static string PERSONAL_OUTLINE = "0D0010";
    // 世界
    readonly static string WORLD_COLOR = "FFFFFF";
    public readonly static string WORLD_OUTLINE = "20130E";  
    // 队伍
    // 为去除警告暂时屏蔽以下代码
    //readonly static string TEAM_COLOR = "";
    public readonly static string TEAM_OUTLINE = "";
    // 公会
    readonly static string UNION_COLOR = "1A98F0";
    public readonly static string UNION_OUTLINE = "000000";
    // 系统
    readonly static string SYSTEM_COLOR = "FBB400";
    public readonly static string SYSTEM_OUTLINE = "000000"; 
    // 人名
    public readonly static string NAME_COLOR = "13FFD5";
    public readonly static string MYNAME_COLOR = "33BEFF";

    // 频道名改为图标显示后，label需要加上的空格数
    public readonly static string CHANNEL_BLANK = "          ";// 10个空格

    #endregion

    ChannelId m_iCurrentChannel = ChannelId.WORLD;
    ulong m_strSenderId;
    ulong m_strFriendId;
    int m_iPrivateTalkHead = 0;

    bool m_bIsSendCDing = false;

    //float m_fLabelOffect = 270f;
    //float m_fPanelClipSize = 1300f;
    string m_lastWords;
    Dictionary<ulong, string> m_dictIDToName = new Dictionary<ulong, string>();
    Dictionary<ulong, byte> m_dictIDToLevel = new Dictionary<ulong, byte>();

    void OnPrivateChannelIconUp()
    {
        LoggerHelper.Debug("PrivateChannel");
        MogoUIManager.Instance.SwitchPrivateChannelUI();

        m_iCurrentChannel = ChannelId.PERSONAL;
    }

    void OnTongChannelIconUp()
    {
        LoggerHelper.Debug("TongChannel");
        MogoUIManager.Instance.SwitchTongChannelUI();

        m_iCurrentChannel = ChannelId.UNION;
    }

    void OnWorldChannelIconUp()
    {
        LoggerHelper.Debug("WorldChannel");
        MogoUIManager.Instance.SwitchWorldChannelUI();

        m_iCurrentChannel = ChannelId.WORLD;
    }

    void OnSendButtonUp()
    {
        if (CommunityUIViewManager.Instance.GetChatUILogicText() == "")
        {
            return;
        }

        if (CommunityUIViewManager.Instance.GetInputLabelContentLength() > 50)
        {
            //MogoFXManager.Instance.FloatText("Too Many Words...");
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7527));
            return;
        }
        if (m_bIsSendCDing)
        {
            //MogoFXManager.Instance.FloatText("Slow Down Please...");
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7526));
            return;
        }

        
        TimerHeap.AddTimer(5000, 0, () => { m_bIsSendCDing = false; });
        
        LoggerHelper.Debug("Send");
        
        m_bIsSendCDing = true;
        //TimerHeap.AddTimer(3000, 0, () => { m_bIsSendCDing = false; });


        //if (m_iCurrentChannel == 2)
        //{
        //    EventDispatcher.TriggerEvent<string>("SendCommunityMessage",
        //        m_iCurrentChannel + m_strFriendId + CommunityUIViewManager.Instance.GetChatUILogicText().Substring(m_iPrivateTalkHead));
        //}
        //else
        //{
        //    EventDispatcher.TriggerEvent<string>("SendCommunityMessage",
        //        m_iCurrentChannel + CommunityUIViewManager.Instance.GetChatUILogicText().Substring(m_iPrivateTalkHead));
        //}

        string data = CommunityUIViewManager.Instance.GetChatUILogicText().Replace("\\n", "");
        data = data.Replace("[", "［");
        data = data.Replace("]", "］");

        for (int i = 0; i < data.Length; ++i)
        {
            if (data[i] == '<')
            {
                if (data.Length > i + 6)
                {
                    if (data.Substring(i, 7) == "<info=(")
                    {
                        if (data.Length > i + 13)
                        {
                            string itemId = data.Substring(i + 7, 7);

                            if (InventoryManager.Instance.GetItemByItemID(int.Parse(itemId),true) == null)
                            {
                                MogoMsgBox.Instance.ShowFloatingText("Not Have This Item");
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (m_iCurrentChannel == ChannelId.PERSONAL)
        {
            string chatStr = data.Substring(m_iPrivateTalkHead);
            if (chatStr.Length > 0 && chatStr[chatStr.Length - 1] == '|')
            {
                chatStr = chatStr.Substring(0, chatStr.Length - 1);
            }
            //Debug.LogError(m_strFriendId);

            MogoWorld.thePlayer.RpcCall("Chat", (byte)m_iCurrentChannel, (ulong)m_strFriendId, chatStr);

            if (m_dictIDToName.ContainsKey(m_strFriendId))
            {
                string sendLevel = "";

                FriendMessageGridData friendMessageGridData = FriendManager.Instance.GetFriendInfo(m_strFriendId);
                if (friendMessageGridData != null) // 私聊-好友等级
                {
                    sendLevel = friendMessageGridData.level;
                }
                else if(m_dictIDToLevel.ContainsKey(m_strFriendId))// 私聊-非好友等级
                {
                    sendLevel = m_dictIDToLevel[m_strFriendId].ToString();
                }
                else
                {
                    sendLevel = "**";
                }

                string LvAndNameString = "";
                if (MogoUIManager.IsShowLevel)
                {
                    LvAndNameString = string.Concat("[", NAME_COLOR, "]", "LV", sendLevel, " ", m_dictIDToName[m_strFriendId], "[-]");
                }
                else
                {
                    LvAndNameString = string.Concat("[", NAME_COLOR, "]", m_dictIDToName[m_strFriendId], "[-]");
                }

                //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + PERSONAL_COLOR + "][" + LanguageData.GetContent(47302) + "] : 你对" + LvAndNameString + "说 : " + chatStr + "[-]", ChannelId.PERSONAL, (ulong)m_strFriendId);
                CommunityUIViewManager.Instance.AddChatUIDialogText("[" + PERSONAL_COLOR + "]" + CHANNEL_BLANK + 
                    LanguageData.GetContent(47305) + LvAndNameString + LanguageData.GetContent(47306) + chatStr + "[-]", ChannelId.PERSONAL, (ulong)m_strFriendId, 
                    ChannelId.PERSONAL, LvAndNameString, NAME_COLOR, 105);
            }

            m_lastWords = data.Substring(m_iPrivateTalkHead);
            //MogoWorld.thePlayer.RpcCall("Chat", (byte)m_iCurrentChannel, (uint)m_strFriendId,
            //    CommunityUIViewManager.Instance.GetChatUILogicText().Substring(m_iPrivateTalkHead));
        }
        else
        {
            //string chatStr = data.Substring(m_iPrivateTalkHead);
            string chatStr = data;
            if (chatStr.Length > 0 && chatStr[chatStr.Length - 1] == '|')
            {
                chatStr = chatStr.Substring(0, chatStr.Length - 1);
            }

            MogoWorld.thePlayer.RpcCall("Chat", (byte)m_iCurrentChannel, (ulong)0, chatStr);

            m_lastWords = data;

            //MogoWorld.thePlayer.RpcCall("Chat", (byte)m_iCurrentChannel, (uint)0,
            //    CommunityUIViewManager.Instance.GetChatUILogicText().Substring(m_iPrivateTalkHead));
        }

        EventDispatcher.TriggerEvent<string>(Events.OtherEvent.ClientGM, m_lastWords);
        m_iPrivateTalkHead = 0;
        CommunityUIViewManager.Instance.EmptyChatUIInput();
    }

    public void SetLastWords()
    {
        CommunityUIViewManager.Instance.AddChatUIInputText(m_lastWords);
    }
    void OnEquipmentShowupUp(int id, List<int> list)
    {
        //string str = "";

        //switch (m_iCurrentChannel)
        //{
        //    case 0:
        //        str = "[FFFFFF][世界] : [-]";               
        //        break;

        //    case 1:
        //        str = "[42BE85][公会] : [-]";
        //        break;

        //    case 2:
        //        str = "[F9799A][私聊] : [-]";
        //        break;

        //    case 4:
        //        str = "[F9C557][系统] : [-]";
        //        break;

        //}

        string text = "<info=(" + id.ToString() + ",";
        for (int i = 0; i < list.Count; ++i)
        {
            text += list[i].ToString();
            text += ",";
        }

        text = text.Substring(0, text.Length - 1);
        text += ")>";

        LoggerHelper.Debug(text);
        CommunityUIViewManager.Instance.AddChatUIInputText(text);

    }

    void OnCommunityUIDialogLabelUp(ulong userId)
    {
        LoggerHelper.Debug(userId);

        string name = "";
        if (m_dictIDToName.ContainsKey(userId))
            name = m_dictIDToName[userId];

        CommunityUIViewManager.Instance.SetAddFriendTipName(name);
        CommunityUIViewManager.Instance.ShowAddFriendTip(true);
        m_strSenderId = userId;
    }

    void OnAddFriendButtonUp()
    {
        LoggerHelper.Debug("AddFriend " + m_strSenderId);
        MogoWorld.thePlayer.RpcCall("FriendAddReq", m_strSenderId);
        CommunityUIViewManager.Instance.ShowAddFriendTip(false);

        //EventDispatcher.TriggerEvent<string>("CommunityAddFriend", m_strSenderId);
    }

    void OnPrivateTalkButtonUp()
    {
        CommunityUIViewManager.Instance.EmptyChatUIInput();
        LoggerHelper.Debug("PrivateTalk");
        CommunityUIViewManager.Instance.ShowAddFriendTip(false);

        CommunityUIViewManager.Instance.AddChatUIInputText("@" + m_dictIDToName[m_strSenderId] + " : ");
        m_iPrivateTalkHead = 1 + m_dictIDToName[m_strSenderId].Length + 3;
        MogoUIManager.Instance.SwitchPrivateChannelUI();
        m_iCurrentChannel = ChannelId.PERSONAL;
        m_strFriendId = m_strSenderId;
    }

    void OnCommunityUIFriendGridUp(ulong id)
    {
        CommunityUIViewManager.Instance.EmptyChatUIInput();
        m_strFriendId = id;
        CommunityUIViewManager.Instance.ShowFriendList();
        CommunityUIViewManager.Instance.AddChatUIInputText("@" + m_dictIDToName[id] + " : ");
        m_iPrivateTalkHead = 1 + m_dictIDToName[id].Length + 3;
    }

    void OnReciveCommunityMessage(CommunityMessageData data)
    {
        if (data == null)
        {
            LoggerHelper.Debug("Message Is NULL");
            return;
        }

        ChannelId channel = data.Channel;
        ulong senderId = data.DBid;
        //Debug.LogError(senderId + "!!!!!!!!!");
        //int senderNameLength = int.Parse(data.SenderName);
        string sendName = data.SenderName;
        byte sendLevel = data.SenderLevel;
        string mess = data.Message;

        m_dictIDToName[senderId] = sendName;
        m_dictIDToLevel[senderId] = sendLevel;

        string LvAndNameString = "";
        string underLineColor = NAME_COLOR;
        if (sendName.Equals(MogoWorld.thePlayer.name))
        {
            if (MogoUIManager.IsShowLevel && channel != ChannelId.TOWER_DEFENCE && channel != ChannelId.OCCUPY_TOWER)
            {
                LvAndNameString = string.Concat("[", MYNAME_COLOR, "]", "LV", sendLevel, " ", sendName, ":", "[-]");
            }
            else
            {
                LvAndNameString = string.Concat("[", MYNAME_COLOR, "]", sendName, ":", "[-]");
            }

            underLineColor = MYNAME_COLOR;
        }
        else
        {
            if (MogoUIManager.IsShowLevel && channel != ChannelId.TOWER_DEFENCE && channel != ChannelId.OCCUPY_TOWER)
            {
                LvAndNameString = string.Concat("[", NAME_COLOR, "]", "LV", sendLevel, " ", sendName, ":", "[-]");
            }
            else
            {
                LvAndNameString = string.Concat("[", NAME_COLOR, "]", sendName, ":", "[-]");
            }

            underLineColor = NAME_COLOR;
        }

        switch (channel)
        {
            // 世界
            case ChannelId.WORLD:
                {
                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + WORLD_COLOR + "][" + LanguageData.GetContent(47300) + "]" +
                    //  LvAndNameString + mess + "[-]", ChannelId.WORLD, senderId);

                    CommunityUIViewManager.Instance.AddChatUIDialogText("[" + WORLD_COLOR + "]" + CHANNEL_BLANK +
                        LvAndNameString + mess + "[-]", ChannelId.WORLD, senderId, ChannelId.WORLD, LvAndNameString, underLineColor);
                    NormalMainUIViewManager.Instance.SetNewChatLabel(string.Concat(sendName, " : ", mess));
                }
                break;

            // 公会
            case ChannelId.UNION:
                {
                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + UNION_COLOR + "][" + LanguageData.GetContent(47301) + "]" +
                    //    LvAndNameString + mess + "[-]", ChannelId.UNION, senderId);
                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + UNION_COLOR + "][" + LanguageData.GetContent(47301) + "]" +
                    //    LvAndNameString + mess + "[-]", ChannelId.WORLD, senderId);

                    CommunityUIViewManager.Instance.AddChatUIDialogText("[" + UNION_COLOR + "]" + CHANNEL_BLANK +
                        LvAndNameString + mess + "[-]", ChannelId.UNION, senderId, ChannelId.UNION, LvAndNameString, underLineColor);
                    CommunityUIViewManager.Instance.AddChatUIDialogText("[" + UNION_COLOR + "]" + CHANNEL_BLANK +
                        LvAndNameString + mess + "[-]", ChannelId.WORLD, senderId, ChannelId.UNION, LvAndNameString, underLineColor);
                    NormalMainUIViewManager.Instance.SetNewChatLabel(string.Concat(sendName, " : ", mess));
                }
                break;

            // 私聊
            case ChannelId.PERSONAL:
                {
                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + PERSONAL_COLOR + "][" + LanguageData.GetContent(47302) + "]" +
                    //    LvAndNameString + mess + "[-]", ChannelId.PERSONAL, senderId);
                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + PERSONAL_COLOR + "][" + LanguageData.GetContent(47302) + "]" +
                    //    LvAndNameString + mess + "[-]", ChannelId.WORLD, senderId);

                    CommunityUIViewManager.Instance.AddChatUIDialogText("[" + PERSONAL_COLOR + "]" + CHANNEL_BLANK +
                        LvAndNameString + mess + "[-]", ChannelId.PERSONAL, senderId, ChannelId.PERSONAL, LvAndNameString, underLineColor);
                    CommunityUIViewManager.Instance.AddChatUIDialogText("[" + PERSONAL_COLOR + "]" + CHANNEL_BLANK +
                        LvAndNameString + mess + "[-]", ChannelId.WORLD, senderId, ChannelId.PERSONAL, LvAndNameString, underLineColor);
                    NormalMainUIViewManager.Instance.SetNewChatLabel(string.Concat(sendName, " : ", mess));
                }
                break;

            // 系统
            case ChannelId.SYSTEM:
                {
                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + SYSTEM_COLOR + "][" + LanguageData.GetContent(47303) + "]" +
                    //    sendName + ": " + mess + "[-]", ChannelId.SYSTEM, senderId);
                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + SYSTEM_COLOR + "][" + LanguageData.GetContent(47303) + "]" +
                    //    sendName + ": " + mess + "[-]", ChannelId.WORLD, senderId);

                    //CommunityUIViewManager.Instance.AddChatUIDialogText("[" + SYSTEM_COLOR + "]" + CHANNEL_BLANK +
                    //    sendName + ": " + mess + "[-]", ChannelId.SYSTEM, senderId);
                    CommunityUIViewManager.Instance.AddChatUIDialogText("[" + SYSTEM_COLOR + "]" + CHANNEL_BLANK +
                        sendName + ": " + mess + "[-]", ChannelId.WORLD, senderId, ChannelId.SYSTEM);
                    NormalMainUIViewManager.Instance.SetNewChatLabel(string.Concat(sendName, " : ", mess));
                }
                break;

            case ChannelId.TOWER_DEFENCE:
                {
                    int td_messageCode = Convert.ToInt32(mess);
                    MainUIViewManager.Instance.AddShortCutMessage(LvAndNameString, td_messageCode);
                }
                break;

            case ChannelId.OCCUPY_TOWER:
                {
                    int ot_messageCode = Convert.ToInt32(mess);
                    MainUIViewManager.Instance.AddShortCutMessage(LvAndNameString, ot_messageCode);
                }
                break;
        }

        // NormalMainUIViewManager.Instance.SetNewChatLabel(string.Concat(sendName," : ",mess));
        NormalMainUIViewManager.Instance.SetCommunityIconBG(true);
    }

    void OnReciveCommunityAddFriend(ulong id, string name)
    {
        CommunityUIViewManager.Instance.AddFriendListGrid(name, id);
        //m_dictIDToName.Add(id, name);
        m_dictIDToName[id] = name;
    }

    void OnFriendButtonUp()
    {
        //EventDispatcher.TriggerEvent("RefreshCommunityFriendList");

        List<FriendMessageGridData> list = FriendManager.Instance.GetFriendList();
        LoggerHelper.Debug("Here " + list.Count);

        CommunityUIViewManager.Instance.ClearFriendList();

        //m_dictIDToName.Clear();
        for (int i = 0; i < list.Count; ++i)
        {
            CommunityUIViewManager.Instance.AddFriendListGrid(list[i].name, (ulong)list[i].id);
            m_dictIDToName[(ulong)list[i].id] = list[i].name;
        }
    }

    //void OnReciveRefreshCommunityFriendList(List<string> list)
    //{
    //    CommunityUIViewManager.Instance.ClearFriendList();

    //    m_dictIDToName.Clear();

    //    for (int i = 0; i < list.Count; ++i)
    //    {
    //        CommunityUIViewManager.Instance.AddFriendListGrid(list[i].Substring(4), list[i].Substring(0, 4));
    //        //m_dictIDToName.Add(list[i].Substring(0, 4), list[i].Substring(4));
    //        m_dictIDToName[list[i].Substring(0, 4)] = list[i].Substring(4);
    //    }
    //}

    void OnChatUIEquipmentGridUp(string logicText)
    {

        LoggerHelper.Debug(logicText);
        int[] data = new int[5];

        if (logicText.Substring(0, 7) == "<info=(")
        {
            logicText = logicText.Substring(7);
            logicText = logicText.Substring(0, logicText.Length - 2);

            string tmp = "";


            for (int i = 0; i < 5; ++i)
            {
                data[i] = -1;
            }

            int j = 0;

            for (int i = 0; i < logicText.Length; ++i)
            {
                if (logicText[i] != ',')
                {
                    tmp += logicText[i];
                }
                else
                {
                    data[j] = int.Parse(tmp);
                    ++j;
                    tmp = "";
                }
            }

            data[j] = int.Parse(tmp);

            for (int i = 0; i < 5; ++i)
            {
                LoggerHelper.Debug(data[i]);
            }

        }

        int itemId = data[0];
        List<int> listHole = new List<int>();
        listHole.Add(data[1]);
        listHole.Add(data[2]);
        listHole.Add(data[3]);
        listHole.Add(data[4]);
        //MaiFeo =================================================
        CommunityUIViewManager.Instance.ShowEquipmentInfo(true);

        InventoryManager.Instance.ShowEquipTip(itemId, null, listHole, MogoWorld.thePlayer.level);
    }

    public float GetLabelOffect()
    {
        float fTmp = 0;

        switch (m_iCurrentChannel)
        {
            case ChannelId.WORLD:
            case ChannelId.UNION:
                fTmp = 400f;
                break;

            case ChannelId.PERSONAL:
                fTmp = 285f;
                break;
        }

        return fTmp;
    }

    public void SetTextCameraPosX(float posX)
    {
        CommunityUIViewManager.Instance.SetTextCameraPosX(posX);
    }

    public Vector2 GetCameraSize()    //Vector2.X is when the (lable.width == 0) camera's Pos,Vector2.Y is lable.width == max camera's Pos(Horizontaly)
    {
        Vector2 vec2 = new Vector2();

        switch (m_iCurrentChannel)
        {
            case ChannelId.WORLD:
            case ChannelId.UNION:
                vec2.x = 460f;
                vec2.y = 80f;
                break;
            case ChannelId.PERSONAL:
                vec2.x = 395f;
                vec2.y = 142f;
                break;
        }

        return vec2;
    }

    public float GetPanelClipSize()
    {
        float fTmp = 0;

        switch (m_iCurrentChannel)
        {
            case ChannelId.WORLD:
            case ChannelId.UNION:
                fTmp = 2250f;
                break;

            case ChannelId.PERSONAL:
                fTmp = 1300f;
                break;
        }

        return fTmp;
    }

    public void Initialize()
    {
        CommunityUIViewManager.Instance.PRIVATECHANNELICONUP += OnPrivateChannelIconUp;
        CommunityUIViewManager.Instance.TONGCHANNELICONUP += OnTongChannelIconUp;
        CommunityUIViewManager.Instance.WORLDCHANNELICONUP += OnWorldChannelIconUp;
        CommunityUIViewManager.Instance.SENDBTNUP += OnSendButtonUp;
        CommunityUIViewManager.Instance.ADDFRIENDBTNUP += OnAddFriendButtonUp;
        CommunityUIViewManager.Instance.PRIVATETALKBTNUP += OnPrivateTalkButtonUp;
        CommunityUIViewManager.Instance.FRIENDBTNUP += OnFriendButtonUp;

        EventDispatcher.AddEventListener<int, List<int>>("EquipmentShowupUp", OnEquipmentShowupUp);
        EventDispatcher.AddEventListener<ulong>("CommunityUIDialogLabelUp", OnCommunityUIDialogLabelUp);

        EventDispatcher.AddEventListener<ulong>("CommunityUIFriendGridUp", OnCommunityUIFriendGridUp);
        EventDispatcher.AddEventListener<CommunityMessageData>("ReciveCommunityMessage", OnReciveCommunityMessage);
        EventDispatcher.AddEventListener<ulong, string>("ReciveCommunityAddFriend", OnReciveCommunityAddFriend);
        //EventDispatcher.AddEventListener<List<string>>("ReciveRefreshCommunityFriendList", OnReciveRefreshCommunityFriendList);
        EventDispatcher.AddEventListener<string>("ChatUIEquipmentGridUp", OnChatUIEquipmentGridUp);
    }

    public void Release()
    {
        CommunityUIViewManager.Instance.PRIVATECHANNELICONUP -= OnPrivateChannelIconUp;
        CommunityUIViewManager.Instance.TONGCHANNELICONUP -= OnTongChannelIconUp;
        CommunityUIViewManager.Instance.WORLDCHANNELICONUP -= OnWorldChannelIconUp;
        CommunityUIViewManager.Instance.SENDBTNUP -= OnSendButtonUp;
        CommunityUIViewManager.Instance.ADDFRIENDBTNUP -= OnAddFriendButtonUp;
        CommunityUIViewManager.Instance.PRIVATETALKBTNUP -= OnPrivateTalkButtonUp;
        CommunityUIViewManager.Instance.FRIENDBTNUP -= OnFriendButtonUp;

        EventDispatcher.RemoveEventListener<int, List<int>>("EquipmentShowupUp", OnEquipmentShowupUp);
        EventDispatcher.RemoveEventListener<ulong>("CommunityUIDialogLabelUp", OnCommunityUIDialogLabelUp);

        EventDispatcher.RemoveEventListener<ulong>("CommunityUIFriendGridUp", OnCommunityUIFriendGridUp);
        EventDispatcher.RemoveEventListener<CommunityMessageData>("ReciveCommunityMessage", OnReciveCommunityMessage);
        EventDispatcher.RemoveEventListener<ulong, string>("ReciveCommunityAddFriend", OnReciveCommunityAddFriend);
        //EventDispatcher.RemoveEventListener<List<string>>("ReciveRefreshCommunityFriendList", OnReciveRefreshCommunityFriendList);
        EventDispatcher.AddEventListener<string>("ChatUIEquipmentGridUp", OnChatUIEquipmentGridUp);
    }

}
