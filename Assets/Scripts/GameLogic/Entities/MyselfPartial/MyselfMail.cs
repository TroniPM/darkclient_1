/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityMyself
// 创建者：Win.J H
// 修改者列表：
// 创建日期：
// 模块描述：与好友resp接口相关的partial class EntityMyself
//----------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.Game
{

    public partial class EntityMyself
    {
        #region 被动回调
        private void OnReceiveMailResp(LuaTable luatable)
        {
            MailManager.Instance.OnReceiveMailResp(luatable);
        }
        #endregion

        #region 回调
        private void OnMailInfoResp(LuaTable luatable)
        {
            MailManager.Instance.OnMailInfoResp(luatable);
        }

        private void OnMailReadResp(LuaTable aMail, int errorId)
        {
            MailManager.Instance.OnMailReadResp(aMail, errorId);
        }

        private void OnMailDelResp(int errorId)
        {
            MailManager.Instance.OnMailDelResp(errorId);
        }
        private void OnMailAttachGetResp(int errorId)
        {
            MailManager.Instance.OnMailAttachGetResp(errorId);
        }
        #endregion
    }
}
