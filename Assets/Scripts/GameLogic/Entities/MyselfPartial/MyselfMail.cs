/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������EntityMyself
// �����ߣ�Win.J H
// �޸����б�
// �������ڣ�
// ģ�������������resp�ӿ���ص�partial class EntityMyself
//----------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.Game
{

    public partial class EntityMyself
    {
        #region �����ص�
        private void OnReceiveMailResp(LuaTable luatable)
        {
            MailManager.Instance.OnReceiveMailResp(luatable);
        }
        #endregion

        #region �ص�
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
