using System;
using System.Collections.Generic;

using TDBID = System.UInt64;

namespace Mogo.Game
{

    public class FriendData
    {
        public TDBID id { get; set; }
        public int vocation { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int fight { get; set; }
        public int degree { get; set; }
        public int IsOnline { get; set; }
        public int IsBlessedByFriend { get; set; }
        public int IsNote { get; set; }
        public int IsBlessedToFriend { get; set; }
    }

    public class FriendReqData
    {
        public TDBID id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int vocation { get; set; }
        public int timeout { get; set; }
    }

    public class FriendNoteData
    {
        public TDBID id { get; set; }
        //public string name { get; set; }
        public string content { get; set; }
        public int time { get; set; }
        //public int timeout { get; set; }
    }

    /*
    public  class FriendData : GameData
    {
        public TDBID id { get; protected set; }
        public string icon { get; protected set; }
        public string name { get; protected set; }
        public int level { get; protected set; }
        public int fight { get; protected set; }
        public bool IsNote { get; protected set; }
       
        //public static readonly string fileName = "xml/Friend";
        public static Dictionary<int, FriendData> dataMap { get; set; }
    }
    */
}
