using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ChatListData : GameData<ChatListData>
    {
        public List<int> chatList { get; set; }

        public static readonly string fileName = "xml/ChatList";
    }
}
