using System;
using System.Collections.Generic;
using TDBID = System.UInt64;
namespace Mogo.Game 
{
    public enum MAIL_STATE
    {
        NO_ATTACH_NO_READ = 0,
        ATTACH_NO_READ    = 1,
        NO_ATTACH_READ    = 2,
        ATTACH_READ       = 3,
        RECV_ATTACH_READ  = 4,
    }
    public enum MAIL_TYPE
    {
        TEXT = 0,
        ID   = 1,
    }
    public class MailInfo
    {
        public TDBID id { get; set; }
        public string from { get; set; }
        public string title { get; set; }
        /*
        MAIL_STATE_NONE = 0, --未读,无附件 0 => 2
        MAIL_STATE_HAVE = 1, --未读,带附件 1 => 3 OR 4
        MAIL_STATE_READ = 2, --已读,无附件
        MAIL_STATE_HERE = 3, --已读,附件未领取
        MAIL_STATE_RECE = 4, --已读,附件已领取
         */
        public MAIL_STATE state { get; set; }
        public MAIL_TYPE mailType { get; set; }
        public int time { get; set; }
    }
    //与后端对应数据结构
    /*
	title      = 1,
    to         = 2,
    text       = 3,
    from       = 4,
    time       = 5,
    attachment = 7,
    timeout    = 6,
     */
    //public class MailIndex
    //{
    //    public const string TITLE = "1";
    //    public const string TO = "2";
    //    public const string TEXT = "3";
    //    public const string FROM = "4";
    //    public const string TIME = "5";
    //    public const string ATTACH = "7";
    //    //timeout = 6,
    //}

    public class Mail
    {
        public string title { get; set; }
        public string to { get; set; }
        public string text { get; set; }
        public string from { get; set; }
        public int time { get; set; }
        //public List<Dictionary<int, int>> attachment  { get; set; } //= new List<Dictionary<int, int>>();
        public List<int> items { get; set; }
        public List<int> nums { get; set; }

        public MAIL_TYPE mailType { get; set; }
        //邮件内容显示提供的参数
        public List<string> args { get; set; }
        public MAIL_STATE state { get; set; }
        public TDBID mailId { get; set; }
    }
}
