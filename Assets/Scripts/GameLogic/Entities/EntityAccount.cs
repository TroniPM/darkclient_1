/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityAccount
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.RPC;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    public class EntityAccount : EntityParent
    {
        #region 实体定义属性

        //private UInt32 m_avatarDbid;

        //public UInt32 avatarDbid
        //{
        //    get { return m_avatarDbid; }
        //    set
        //    {
        //        m_avatarDbid = value;
        //    }
        //}
        public LuaTable avatarsInfo
        {
            set
            {
                if (!Mogo.Util.Utils.ParseLuaTable(value, out avatarList))
                    avatarList = new Dictionary<int, AvatarInfo>();
                LoggerHelper.Debug("avatar len: " + avatarList.Count);
            }
        }
        /// <summary>
        /// avatarsInfo转换后实体数据
        /// </summary>
        public Dictionary<int, AvatarInfo> avatarList;

        #endregion

        public const int ERROR_CODE_OFFSET = 21000;
        private string m_currentName;
        //Dictionary<int, EntityParent> theAvatarList = new Dictionary<int, EntityParent>();

        public EntityAccount()
        {
            entityType = "Account";
            AddListener();
            EventDispatcher.AddEventListener<string, byte, byte>(Events.UIAccountEvent.OnCreateCharacter, CreateCharacter);
            EventDispatcher.AddEventListener<int>(Events.UIAccountEvent.OnDelCharacter, DelCharacter);
            EventDispatcher.AddEventListener<byte>(Events.UIAccountEvent.OnGetRandomName, GetRandomName);
            EventDispatcher.AddEventListener<int>(Events.UIAccountEvent.OnStartGame, StartGame);
        }

        override public void OnEnterWorld()
        {
            if (!SystemConfig.Instance.HasUploadInfo)
            {
                UploadPhoneInfo();
                SystemConfig.Instance.HasUploadInfo = true;
                SystemConfig.SaveConfig();
            }
        }

        public override void OnLeaveWorld()
        {
            LoggerHelper.Error("OnLeaveWorld");
            EventDispatcher.RemoveEventListener<string, byte, byte>(Events.UIAccountEvent.OnCreateCharacter, CreateCharacter);
            EventDispatcher.RemoveEventListener<int>(Events.UIAccountEvent.OnDelCharacter, DelCharacter);
            EventDispatcher.RemoveEventListener<byte>(Events.UIAccountEvent.OnGetRandomName, GetRandomName);
            EventDispatcher.RemoveEventListener<int>(Events.UIAccountEvent.OnStartGame, StartGame);
            RemoveListener();
        }

        public void CharaterInfoReq()
        {
            this.RpcCall("CharaterInfoReq", name);
        }

        public void UpdateCharacterList()
        {
            if (avatarList != null && NewLoginUILogicManager.Instance != null)
            {
                var list = new List<ChooseCharacterGridData>();
                foreach (var avatar in avatarList)
                {
                    if (avatar.Value == null)
                    {
                        continue;
                    }
                    list.Add(new ChooseCharacterGridData()
                    {
                        name = avatar.Value.Name,
                        level = avatar.Value.Level.ToString(),
                        headImg = IconData.dataMap.Get((int)IconOffset.Avatar + avatar.Value.Vocation).path,
                        defautText = LanguageData.GetContent((int)LangOffset.Character + (int)CharacterCode.CREATE_CHARACTER)
                    });
                }
                NewLoginUILogicManager.Instance.FillChooseCharacterGridData(list);
                NewLoginUILogicManager.Instance.LoadChooseCharacterSceneAfterDelete();
            }
            //if (ChooseCharacterUILogicManager.Instance != null)
            //{
            //    ChooseCharacterUILogicManager.Instance.ResetCharacterList();
            //    if (avatarList != null)
            //        foreach (var avatar in avatarList)
            //        {
            //            ChooseCharacterUILogicManager.Instance.SetChooseCharacterGridInfo(LanguageData.dataMap[avatar.Value.Vocation].content, avatar.Value.Level, avatar.Value.Name, avatar.Key - 1);
            //        }
            //}
        }

        public AvatarInfo GetAvatarInfo(Int32 index)
        {
            var order = index + 1;
            if (avatarList != null && avatarList.ContainsKey(order))
            {
                return avatarList[order];
            }
            else
                return null;
        }

        // 创建角色
        private void CreateCharacter(string name, byte gender, byte vocation)
        {
            LoggerHelper.Debug("vocation: " + vocation);
            m_currentName = name;
            this.RpcCall("CreateCharacterReq", name, gender, vocation);
        }

        // 删除角色
        private void DelCharacter(int gridID)
        {
            // todo confirm dialog
            var avatar = GetAvatarInfo(gridID);
            if (avatar == null)
            {
                MogoMessageBox.RespError(LangOffset.Character, (int)CharacterCode.NOT_SELECTED);
                //MogoGlobleUIManager.Instance.Confirm("no select role", (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
                return;
            }
            this.RpcCall("DelCharacterReq", avatar.DBID);
            GuideSystem.Instance.DelCharacterGuideConfig(avatar.DBID);
        }

        //获取随机名字
        private void GetRandomName(byte occupation)
        {
            this.RpcCall("RandomNameReq", occupation);
        }
        private void RandomNameResp(string name)
        {
            NewLoginUIViewManager.Instance.SetCharacterNameInputText(name);
        }
        // 选择角色后，进入游戏
        private void StartGame(int gridID)
        {
            var avatar = GetAvatarInfo(gridID);
            if (avatar == null)
            {
                MogoGlobleUIManager.Instance.Info("no select role");
                return;
            }
            this.RpcCall("StartGameReq", avatar.DBID);
#if UNITY_ANDROID
         if (SystemSwitch.UsePlatformSDK)
                AndroidSdkManager.Instance.EnterGameLog(SystemConfig.Instance.SelectedServer.ToString());
#endif
        }

        // 创建角色回调
        private void OnCreateCharacterResp(byte errorID, ulong characterID)
        {
            LoggerHelper.Debug("OnCreateCharacterResp: " + errorID + " " + characterID);
            if (errorID == 0)
            {
                //LoggerHelper.Error("OnCreateCharacterResp: " + avatarList[(int)characterID].Name);
                //Debug.LogError("name:" + m_currentName);
                if (SystemSwitch.UsePlatformSDK)
                    PlatformSdkManager.Instance.CreateRoleLog(m_currentName, SystemConfig.Instance.SelectedServer.ToString());
                SystemConfig.Instance.SelectedCharacter = string.Concat(SystemConfig.Instance.Passport, ",", avatarList.Count);
                SystemConfig.SaveConfig();

                this.RpcCall("StartGameReq", characterID);
                //MogoWorld.scenesManager.LoadCharacterScene(null);
                //CharaterInfoReq();
            }
            else
            {
                MogoMessageBox.RespError(LangOffset.Character, errorID);
            }
        }

        // 删除角色回调
        private void OnDelCharacterResp(byte errorID, ulong characterID)
        {
            LoggerHelper.Debug("OnDelCharacterResp: " + errorID + " " + characterID);
            CharaterInfoReq();
        }

        private void OnCharaterInfoResp(LuaTable luaTable)
        {
            avatarsInfo = luaTable;
            UpdateCharacterList();
            LoggerHelper.Debug("OnCharaterInfoResp: " + luaTable);
        }

        private void OnLoginResp(byte errorID)
        {
            LoggerHelper.Debug("OnLoginResp: " + errorID);
            if (errorID == 0)
            {
                NewLoginUILogicManager.Instance.ReleaseCharacter();
            }
            else
            {
                MogoMessageBox.RespError(LangOffset.StartGame, errorID);
            }
            //MogoWorld.scenesManager.LoadHomeScene((isLoadScene) =>
            //{
            //    if (isLoadScene)
            //        AssetCacheMgr.GetUIInstance("prefab_101.prefab", (prefab, id, go) =>
            //        {
            //            var gameObject = go as GameObject;
            //            gameObject.tag = "Player";
            //            gameObject.AddComponent<ActorMyself>();
            //            gameObject.AddComponent<CueHandler>();
            //        });
            //});
        }

        private void OnCheckVersionResp(byte errorID)
        {
            LoggerHelper.Debug("OnCheckVersionResp: " + errorID);
            switch (errorID)
            {
                case 1:
                    LoggerHelper.Debug("Compatible Version");
                    // MogoGlobleUIManager.Instance.Info("Compatible Version");
                    break;
                case 2:
                    LoggerHelper.Debug("Incompatible Version");
                    //MogoGlobleUIManager.Instance.Confirm("Incompatible Version",
                    //    (b) => 
                    //    {
                    //        LoggerHelper.Debug("Quit");
                    //        Application.Quit();
                    //        LoggerHelper.Debug("Quit2");
                    //    });
                    break;
            }
        }

        public void CheckVersionReq()
        {
            LoggerHelper.Debug("CheckVersionReq: " + VersionManager.Instance.LocalVersion.ToString());
            this.RpcCall("CheckVersionReq", VersionManager.Instance.LocalVersion.ToString());
        }

        private void OnLogoutResp(byte msg_id)
        {
            LoggerHelper.Debug("OnLogoutResp: " + msg_id);
            if (msg_id == 0)
            {
                //if (Application.platform == RuntimePlatform.OSXEditor)
                Application.Quit();
                //UnityEditor.EditorApplication.isPlaying = false;
                //else
            }
        }

        //通知前端被顶号
        private void OnMultiLogin()
        {
            MogoWorld.beKick = true;
            MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(25563), (ok) =>
            {
                MogoGlobleUIManager.Instance.ConfirmHide();
            }, LanguageData.GetContent(25561));
            ServerProxy.Instance.Disconnect();
        }

        private void UploadPhoneInfo()
        {
            this.RpcCall("PhoneInfo", SystemInfo.deviceUniqueIdentifier, Mogo.Util.MogoUtils.GetDeviceInfo());
        }
    }
}