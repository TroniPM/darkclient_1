using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

using TDBID = System.UInt64;

namespace Mogo.Game
{
   

        public partial class EntityMyself
        {
            #region 被动回调
            /// <summary>
            /// 被动
            /// </summary>
            /// <param name="luatable"></param>
            private void OnFriendRecvBeAddResp(LuaTable luatable)
            {
                LoggerHelper.Debug("OnFriendRecvBeAddResp");
                friendManager.OnFriendRecvBeAddResp(luatable);
            }
            /*
            private void OnFriendTipResp(UInt32 id, UInt32 isOnline)
            {

                LoggerHelper.Debug("OnFriendTipResp");
            }
            */
            private void OnFriendRecvNoteResp(LuaTable luatable)
            {
                LoggerHelper.Debug("OnFriendRecvNoteResp");
                friendManager.OnFriendRecvNoteResp(luatable);
            }

            private void OnFriendBeBlessResp(LuaTable luatable)
            {
                LoggerHelper.Debug("OnFriendBeBlessResp");
                friendManager.OnFriendBeBlessResp(luatable);
            }
            #endregion

            #region RPC方法回调
            private void OnFriendAddReqResp(UInt32 errorid)
            {
                friendManager.FriendAddReqResp((int)errorid);
            }

            private void OnFriendDelResp(UInt32 errorid)
            {
                friendManager.FriendDelResp((int)errorid);
            }

            private void OnFriendListResp(LuaTable luatable, UInt32 errorid)
            {
                try
                {
                    friendManager.FriendListResp(luatable, (int)errorid);
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
            }

            private void OnFriendResearchReqResp(LuaTable luatable, UInt32 errorid)
            {
                friendManager.FriendResearchReqResp(luatable, (int)errorid);
            }

            private void OnFriendReqListResp(LuaTable luatable, UInt32 errorid)
            {
                friendManager.FriendReqListResp(luatable, (int)errorid);
            }

            private void OnFriendAcceptResp(UInt32 errorid)
            {
                friendManager.FriendAcceptResp((int)errorid);
            }


            private void OnFriendRejectResp(UInt32 errorid)
            {
                friendManager.FriendRejectResp((int)errorid);
            }

            private void OnFriendSendNoteResp(UInt32 errorid)
            {
                friendManager.FriendSendNoteResp((int)errorid);
            }

            private void OnFriendReadNoteResp(LuaTable luatable, UInt32 errorid)
            {
                friendManager.FriendReadNoteResp(luatable, (int)errorid);
            }

            private void OnFriendBlessResp(TDBID friendId)
            {
                friendManager.OnFriendBlessResp(friendId);
            }

            private void OnFriendRecvBlessResp(UInt32 energy, TDBID friendId, UInt32 errorid)
            {
                friendManager.OnFriendRecvBlessResp((int)energy, (TDBID)friendId, (int)errorid);
            }

            private void OnFriendRecvAllBlessResp(UInt32 energy, LuaTable luatable, UInt32 errorid)
            {
                friendManager.OnFriendRecvAllBlessResp((int)energy, luatable, (int)errorid);
            }

             #endregion
        }
            
    }
