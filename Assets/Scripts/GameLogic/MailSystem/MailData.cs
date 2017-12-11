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
        MAIL_STATE_NONE = 0, --δ��,�޸��� 0 => 2
        MAIL_STATE_HAVE = 1, --δ��,������ 1 => 3 OR 4
        MAIL_STATE_READ = 2, --�Ѷ�,�޸���
        MAIL_STATE_HERE = 3, --�Ѷ�,����δ��ȡ
        MAIL_STATE_RECE = 4, --�Ѷ�,��������ȡ
         */
        public MAIL_STATE state { get; set; }
        public MAIL_TYPE mailType { get; set; }
        public int time { get; set; }
    }
    //���˶�Ӧ���ݽṹ
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
        //�ʼ�������ʾ�ṩ�Ĳ���
        public List<string> args { get; set; }
        public MAIL_STATE state { get; set; }
        public TDBID mailId { get; set; }
    }
}
