#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ContentDefine
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.28
// 模块描述：文本显示定义。
//----------------------------------------------------------------*/
#endregion

using System;

static public class ContentDefine
{
    public const string Unkown = "Unkown";

    static public class Character
    {
        public const int Warrior = 0;
        public const int Assassin = 1;
        public const int Wizard = 2;
        public const int Archer = 3;
    }
    //=>社交模块700~800
    /*
    700	好友
    701	请求
    702	一键领取
    703	添加
    704	LV %d
    705	战斗力 x %d
    706	祝福
    707	留言
    708	删除
    709	回复
    710	发送请求
    711	添加
    712	该好友不存在
    713	好友名称
    714	祝福成功
    715	你确定删除%s吗？
    716	取消
    717	确定
    718	成功领取祝福，体力+%d，立即回赠，好人有好报……
    719	没有好友请求
    720 搜索
    721	回赠
    722 请求加你为好友
    750	邮件
     */
    static public class Friend
    {
        public const int TITLE = 700;
        public const int BTN_REQ = 701;
        public const int BTN_RECV_ALL = 702;
        public const int BTN_ADD = 703;
        public const int LV = 704;
        public const int POWER = 705;
        public const int WISH_BUTTON = 706;
        public const int WISH_BUTTON_WISHED = 48700;
        public const int MESSAGE_BUTTON = 707;
        public const int DEL_BUTTON = 708;
        public const int BTN_RETURN = 709;

        public const int SEND_BUTTON = 710;
        public const int ADD_TITLE = 711;
        public const int NOT_EXISTS_TIP = 712;
        public const int NAME_TITLE = 713;
        public const int WISH_SUCCEED = 714;
        public const int DEL_CONFIRM = 715;
        public const int CANCEL_BUTTON = 716;
        public const int YES_BUTTON = 717;
        public const int RECV_WISH_SUCCEED = 718;
        public const int NO_REQ = 719;

        public const int BTN_FIND = 720;
        public const int RETURN_WISH = 721;
        public const int REQ_U_AS_FRIEND = 722;
        public const int NO_FRIENDS = 723;

        public const int TEXT_SEND_REQ_SUCCEED = 724;	//发送好友请求成功
        public const int TEXT_ALREADY_HAVE = 725;	//对方已存在好友列表中
        public const int TEXT_SEND_REQ_FAIL = 726;	//发送好友请求失败
        public const int TEXT_FULL = 727;	//我的好友个数已满
        public const int TEXT_THE_PLAYER_FRIEND_FULL = 728;	//对方好友个数已满
        public const int TEXT_SEND_NOTE_SUCCEED = 729;	//发送留言成功
        public const int TEXT_SEND_NOTE_FAIL = 730;	//发送留言失败
        public const int TEXT_SEND_NOTE_NOT_FRIEND = 731;	//不是好友不能留言
        public const int TEXT_RECV_BLESS_FULL = 732;	//今日可领体力已满
        public const int TEXT_RECV_ALL_BLESS_SUCCEED = 733; //领取所有祝福成功

    }
    static public class Mail
    {
        public const int TITLE = 750;
    }
}
