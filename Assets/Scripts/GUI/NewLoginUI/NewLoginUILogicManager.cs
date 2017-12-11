using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewLoginUILogicManager
{
    private const int LANGUAGE_DELETE = 708;
    private static NewLoginUILogicManager m_instance;
    private Dictionary<int, EtyAvatar> m_avatarList = new Dictionary<int, EtyAvatar>();
    private MogoLoginCamera m_loginCamera;
    private Transform m_defaultCameraSlot;
    private Dictionary<int, Transform> m_cameraSlots = new Dictionary<int, Transform>();
    public Shader PlayerShader;
    public Shader FakeLightShader;

    protected Dictionary<int, Animator> m_Animators = new Dictionary<int, Animator>();
    protected Dictionary<int, SfxHandler> m_sfxHandlers = new Dictionary<int, SfxHandler>();
    protected Dictionary<int, uint> m_fxTimes = new Dictionary<int, uint>();

    protected uint actionTimer;

    public static NewLoginUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NewLoginUILogicManager();
            }

            return NewLoginUILogicManager.m_instance;

        }
    }

    private int m_selectedCharacterId = 0;
    public EtyAvatar m_lastSelectedCharacter;
    private Action<int> m_onChooseServerGridUp;
    private Action m_onChooseServerUIBackBtnUp;

    public static class NewLoginUIEvent
    {
        public const string CHOOSECHARACTERGRIDUP = "NewLoginUI_ChooseCharacterGridUp";
        public const string CHOOSESERVERGRIDUP = "NewLoginUI_ChooseServerGridUp";
    }

    public void Initialize()
    {
        EventDispatcher.AddEventListener<int>(NewLoginUIEvent.CHOOSECHARACTERGRIDUP, OnChooseCharacterGridUp);
        EventDispatcher.AddEventListener<int>(NewLoginUIEvent.CHOOSESERVERGRIDUP, OnChooseServerGridUp);
        EventDispatcher.AddEventListener<GameObject>("NewLoginUIViewManager.CreateCharacterChooseModel", OnCreateCharacterSelected);

        NewLoginUIViewManager.Instance.ENTERGAMEBTNUP += OnEnterGameBtnUp;
        NewLoginUIViewManager.Instance.DELETECHARACTERBTNUP += OnDeleteCharacterBtnUp;
        NewLoginUIViewManager.Instance.ChooseCharacterUIServerBtnUp += OnChooseCharacterUIServerBtnUp;
        NewLoginUIViewManager.Instance.ChooseServerUIBackBtnUp += ChooseServerUIBackBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIEnterBtnUp += CreateCharacterDetailUIEnterBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIBackBtnUp += OnCreateCharacterDetailUIBackBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterUIBackBtnUp += OnCreateCharacterUIBackBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIJobIconUp += OnCreateCharacterDetailUIJobIconUp;

        NewLoginUIViewManager.Instance.CreateCharacterDetailUISwitch += OnCreateCharacterDetailUISwitch;


        string[] temp = SystemConfig.Instance.SelectedCharacter.Split(',');
        if (temp.Length > 1)
        {

            int index = int.Parse(temp[1]);
            if (temp[0] == SystemConfig.Instance.Passport && index >= 0)
            {
                if (MogoWorld.theAccount.GetAvatarInfo(index) != null)
                {
                    m_selectedCharacterId = index;
                }
            }
            else
            {
                m_selectedCharacterId = 0;
            }
        }
        else
        {
            m_selectedCharacterId = 0;
        }
        //Debug.LogError("m_selectedCharacterId:" + m_selectedCharacterId);
        NewLoginUIViewManager.Instance.SetCharacterGridDown(m_selectedCharacterId);
    }



    public void Release()
    {
        m_selectedCharacterId = 0;
        EventDispatcher.RemoveEventListener<int>(NewLoginUIEvent.CHOOSECHARACTERGRIDUP, OnChooseCharacterGridUp);
        EventDispatcher.RemoveEventListener<int>(NewLoginUIEvent.CHOOSESERVERGRIDUP, OnChooseServerGridUp);
        EventDispatcher.RemoveEventListener<GameObject>("NewLoginUIViewManager.CreateCharacterChooseModel", OnCreateCharacterSelected);


        NewLoginUIViewManager.Instance.CreateCharacterDetailUISwitch -= OnCreateCharacterDetailUISwitch;
        NewLoginUIViewManager.Instance.ENTERGAMEBTNUP -= OnEnterGameBtnUp;
        NewLoginUIViewManager.Instance.DELETECHARACTERBTNUP -= OnDeleteCharacterBtnUp;
        NewLoginUIViewManager.Instance.ChooseCharacterUIServerBtnUp -= OnChooseCharacterUIServerBtnUp;
        NewLoginUIViewManager.Instance.ChooseServerUIBackBtnUp -= ChooseServerUIBackBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIEnterBtnUp -= CreateCharacterDetailUIEnterBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIJobIconUp -= OnCreateCharacterDetailUIJobIconUp;
    }

    #region ѡ���ɫ

    public void LoadChooseCharacterSceneAfterDelete()
    {
        while (m_selectedCharacterId >= 0)
        {
            var avatarInfo = MogoWorld.theAccount.GetAvatarInfo(m_selectedCharacterId);
            if (avatarInfo == null)
            {
                m_selectedCharacterId--;
            }
            else
            {
                LoadChooseCharacter();
                return;
            }
        }


        m_selectedCharacterId = 0;
        LoadChooseCharacter();
    }

    public void FillChooseCharacterGridData(List<ChooseCharacterGridData> list)
    {
        NewLoginUIViewManager.Instance.FillChooseCharacterGridData(list);
    }

    public void UpdateSelectedAvatarModel()
    {
        //MogoMessageBox.ShowLoading();
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        var avatarInfo = MogoWorld.theAccount.GetAvatarInfo(m_selectedCharacterId);
        NewLoginUIViewManager.Instance.SetCharacterGridDown(m_selectedCharacterId);
        if (avatarInfo != null && avatarInfo.Vocation != 0)
        {
            LoadChooseCharacter(avatarInfo.Vocation, (avatar) =>
            {
                //if (m_lastSelectedCharacter == avatar)
                //    return;
                if (m_lastSelectedCharacter != null)
                    m_lastSelectedCharacter.Hide();

                avatar.RemoveAll();

                var list = new List<int>();
                if (avatarInfo.Weapon != 0)
                    list.Add(avatarInfo.Weapon);
                if (avatarInfo.Cuirass != 0)
                    list.Add(avatarInfo.Cuirass);
                if (avatarInfo.Shoes != 0)
                    list.Add(avatarInfo.Shoes);
                if (avatarInfo.Armguard != 0)
                    list.Add(avatarInfo.Armguard);
                avatar.Equip(list, () =>
                {
                    //MogoMessageBox.HideLoading();
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    avatar.Show();
                });

                m_lastSelectedCharacter = avatar;
            });
        }
    }

    void OnChooseCharacterGridUp(int id)
    {
        OnChooseCharacterGridUp(id, false);
    }

    void OnChooseCharacterGridUp(int id, bool forceUpdate)
    {
        if (!forceUpdate && m_selectedCharacterId == id)
            return;
        var avatarInfo = MogoWorld.theAccount.GetAvatarInfo(id);
        if (avatarInfo == null)
        {
            MogoWorld.scenesManager.LoadCharacterScene(null, null, true);
        }
        else
        {
            m_selectedCharacterId = id;
            UpdateSelectedAvatarModel();
        }

        Mogo.Util.LoggerHelper.Debug(id);
    }

    void OnEnterGameBtnUp()
    {
        //Debug.LogError("OnEnterGameBtnUp:" + string.Concat(SystemConfig.Instance.Passport, "_", m_selectedCharacterId));
        //Debug.LogError("OnEnterGameBtnUp");
        SystemConfig.Instance.SelectedCharacter = string.Concat(SystemConfig.Instance.Passport, ",", m_selectedCharacterId);
        SystemConfig.SaveConfig();
        TimerHeap.DelTimer(actionTimer);
        EventDispatcher.TriggerEvent<int>(Events.UIAccountEvent.OnStartGame, m_selectedCharacterId);
    }

    void OnDeleteCharacterBtnUp()
    {
        var avatarInfo = MogoWorld.theAccount.GetAvatarInfo(m_selectedCharacterId);
        if (avatarInfo == null)
            return;
        MogoMessageBox.Confirm(LanguageData.GetContent((int)LangOffset.Character + (int)CharacterCode.DELETE_CHARACTER, avatarInfo.Level, avatarInfo.Name),
              LanguageData.GetContent(MogoMessageBox.LANGUAGE_CANCEL),
              LanguageData.GetContent(LANGUAGE_DELETE),
              (flag) =>
              {
                  if (!flag)
                      EventDispatcher.TriggerEvent<int>(Events.UIAccountEvent.OnDelCharacter, m_selectedCharacterId);
              }, -1, ButtonBgType.Blue, ButtonBgType.Brown);
    }

    void OnChooseCharacterUIServerBtnUp()
    {
        ShowChooseServerUIFormCreateChr();
    }

    void LoadCamera()
    {
        var camera = Camera.main;
        m_loginCamera = camera.GetComponent<MogoLoginCamera>();
        if (m_loginCamera == null)
        {
            m_loginCamera = camera.gameObject.AddComponent<MogoLoginCamera>();
            camera.transform.parent = null;
            var go = new GameObject();
            go.transform.position = camera.transform.position;
            go.transform.rotation = camera.transform.rotation;
            m_defaultCameraSlot = go.transform;
            for (int i = 1; i <= 4; i++)
            {
                var tran = camera.transform.Find(i.ToString());
                tran.parent = null;
                m_cameraSlots[i] = tran;
            }
        }

        EventDispatcher.TriggerEvent(SettingEvent.BuildSoundEnvironment, 10003);
    }

    public void LoadChooseCharacter(int vocation, Action<EtyAvatar> loaded)
    {
        MogoWorld.inCity = true;
        var ci = CharacterInfoData.dataMap.GetValueOrDefault(0, new CharacterInfoData());
        LoadCharacter(vocation, () =>
        {
            var avatar = m_avatarList.GetValueOrDefault(vocation, null);
            if (avatar != null)
            {
                avatar.Show();
                if (avatar.gameObject)
                    avatar.gameObject.transform.position = ci.Location;
                avatar.SetChooseMode();
                avatar.Hide();
                avatar.Unfocus();
                if (loaded != null)
                    loaded(avatar);
            }
        });
    }

    public void LoadChooseCharacter()
    {
        AssetCacheMgr.GetResource("PlayerShader.shader", (obj) =>
        {
            AssetCacheMgr.GetResource("MogoFakeLight.shader", (obj1) =>
            {
                FakeLightShader = (Shader)obj1;
                PlayerShader = (Shader)obj;
                ClearChooseCharacterModel();
                OnChooseCharacterGridUp(m_selectedCharacterId, true);
            });
        });

    }

    #endregion

    #region ������ɫ
    public int m_currentPos = -1;

    void OnCreateCharacterSelected(GameObject go)
    {
        OnCreateCharacterSelected(go, false);
    }

    /// <summary>
    /// ѡ�񴴽���ɫ
    /// </summary>
    /// <param name="go"></param>
    void OnCreateCharacterSelected(GameObject go, bool needFadeIn)
    {
        var vocation = Int32.Parse(go.name);

        #region ����

        var goAnimator = m_Animators[vocation];
        if (goAnimator != null)
        {
            goAnimator.SetInteger("Action", 35);
            actionTimer = TimerHeap.AddTimer(500, 0, () =>
            {
                if (go && goAnimator)
                    goAnimator.SetInteger("Action", 34);
            });
        }

        #endregion

        #region ��Ч

        switch (vocation)
        {
            case (int)Vocation.Warrior:
                if (m_sfxHandlers.ContainsKey(vocation) && m_fxTimes.ContainsKey(vocation))
                {
                    foreach (var item in m_sfxHandlers)
                        item.Value.RemoveAllFX();

                    foreach (var item in m_fxTimes)
                        TimerHeap.DelTimer(item.Value);

                    m_sfxHandlers[vocation].HandleFx(10135);
                    m_fxTimes[vocation] = TimerHeap.AddTimer(3000, 0, () =>
                    {
                        if (m_sfxHandlers.ContainsKey(vocation))
                            m_sfxHandlers[vocation].RemoveAllFX();
                    });
                }
                break;

            case (int)Vocation.Archer:
                if (m_sfxHandlers.ContainsKey(vocation) && m_fxTimes.ContainsKey(vocation))
                {
                    foreach (var item in m_sfxHandlers)
                        item.Value.RemoveAllFX();

                    foreach (var item in m_fxTimes)
                        TimerHeap.DelTimer(item.Value);

                    m_sfxHandlers[vocation].HandleFx(10335);
                    m_fxTimes[vocation] = TimerHeap.AddTimer(3000, 0, () =>
                    {
                        if (m_sfxHandlers.ContainsKey(vocation))
                            m_sfxHandlers[vocation].RemoveAllFX();
                    });
                }
                break;

            case (int)Vocation.Assassin:
                if (m_sfxHandlers.ContainsKey(vocation) && m_fxTimes.ContainsKey(vocation))
                {
                    foreach (var item in m_sfxHandlers)
                        item.Value.RemoveAllFX();

                    foreach (var item in m_fxTimes)
                        TimerHeap.DelTimer(item.Value);

                    m_sfxHandlers[vocation].HandleFx(10435);
                    m_fxTimes[vocation] = TimerHeap.AddTimer(3000, 0, () =>
                    {
                        if (m_sfxHandlers.ContainsKey(vocation))
                            m_sfxHandlers[vocation].RemoveAllFX();
                    });
                }
                break;


            case (int)Vocation.Mage:
                if (m_sfxHandlers.ContainsKey(vocation) && m_fxTimes.ContainsKey(vocation))
                {
                    foreach (var item in m_sfxHandlers)
                        item.Value.RemoveAllFX();

                    foreach (var item in m_fxTimes)
                        TimerHeap.DelTimer(item.Value);

                    m_sfxHandlers[vocation].HandleFx(10235);
                    m_sfxHandlers[vocation].HandleFx(10239, null, (fxgo, fxid) =>
                    {
                        fxgo.transform.parent = m_avatarList[vocation].gameObject.transform.Find("Bip_master/bip_weapon_separate/equip_mage_staff_02_p(Clone)");
                        fxgo.transform.localPosition = Vector3.zero;
                        fxgo.transform.localEulerAngles = new Vector3(270, 0, 0);
                        fxgo.transform.localScale = new Vector3(1, 1, 1);
                    });

                    m_fxTimes[vocation] = TimerHeap.AddTimer(3000, 0, () =>
                    {
                        if (m_sfxHandlers.ContainsKey(vocation))
                            m_sfxHandlers[vocation].RemoveFXs(10235);
                    });
                }
                break;

            default:
                break;
        }

        #endregion

        var cameraSlot = m_cameraSlots.GetValueOrDefault(vocation, null);
        m_loginCamera.SetTarget(cameraSlot);
        var avatar = m_avatarList.GetValueOrDefault(vocation, null);
        if (avatar == null)
            return;

        if (m_lastSelectedCharacter != null)
            m_lastSelectedCharacter.Unfocus();
        avatar.Focus();
        if (needFadeIn)
        {
            m_lastSelectedCharacter.FadeOut();
            avatar.FadeIn();
        }
        else
        {
            foreach (var item in m_avatarList)
            {
                if (item.Value != avatar)
                    item.Value.FadeOut();
            }
        }
        m_lastSelectedCharacter = avatar;
        NewLoginUIViewManager.Instance.ShowCreateCharacterDetailUI();
        var chrInfo = CharacterInfoData.dataMap.GetValueOrDefault(int.Parse(go.name), null);
        if (chrInfo != null)
        {
            NewLoginUIViewManager.Instance.SetCreateCharacterJobDetailName(LanguageData.dataMap[chrInfo.vocation].content);
            NewLoginUIViewManager.Instance.SetCreateCharacterJobDetailInfo(LanguageData.dataMap[chrInfo.discription].content);
            List<JobAttrGridData> listd = new List<JobAttrGridData>() { 
                new JobAttrGridData(){attrName = LanguageData.GetContent(46802), level = chrInfo.attack},
                new JobAttrGridData(){attrName = LanguageData.GetContent(46803), level = chrInfo.defence},
                new JobAttrGridData(){attrName = LanguageData.GetContent(46804), level = chrInfo.range},
            };

            NewLoginUIViewManager.Instance.FillJobAttrGridData(listd);
        }

        #region ��Ч

        SoundManager.PlaySoundByID(19);

        #endregion

        #region ���ƴ���

        switch (vocation)
        {
            case 1:
            case 2:
            case 4:
            case 3:
                NewLoginUIViewManager.Instance.SetEnterButtonLabel(LanguageData.GetContent(46800), true);
                NewLoginUIViewManager.Instance.ShowDiceAndName(true);
                NewLoginUIViewManager.Instance.SetCharacterNameInputText("");
                EventDispatcher.TriggerEvent<byte>(Events.UIAccountEvent.OnGetRandomName, (byte)avatar.vocation);
                break;

       
                //NewLoginUIViewManager.Instance.SetEnterButtonLabel(LanguageData.GetContent(46801), false);
                //NewLoginUIViewManager.Instance.ShowDiceAndName(false);
                // TO DO
                //NewLoginUIViewManager.Instance.SetCharacterNameInputText("");
                //EventDispatcher.TriggerEvent<byte>(Events.UIAccountEvent.OnGetRandomName, (byte)avatar.vocation);
                //break;
        }

        switch (vocation)
        {
            case 1:
                m_currentPos = 3;
                NewLoginUIViewManager.Instance.SelectCreateCharacterDetailUIJobIcon(3);
                break;
            case 2:
                m_currentPos = 4;
                NewLoginUIViewManager.Instance.SelectCreateCharacterDetailUIJobIcon(4);
                break;
            case 3:
                m_currentPos = 1;
                NewLoginUIViewManager.Instance.SelectCreateCharacterDetailUIJobIcon(1);
                break;
            case 4:
                m_currentPos = 2;
                NewLoginUIViewManager.Instance.SelectCreateCharacterDetailUIJobIcon(2);
                break;
        }

        #endregion
        GameProcManager.SetGameProgress(ProcType.CreateCharacter, "SelectCharacter");
    }

    void CreateCharacterDetailUIEnterBtnUp()
    {
        #region ���ƴ���

        //if (m_lastSelectedCharacter.vocation == (int)Vocation.Archer)
        //// || m_lastSelectedCharacter.vocation == (int)Vocation.Mage)
        //{
        //    return;
        //}

        #endregion

        string characterName = NewLoginUIViewManager.Instance.GetCharacterNameInputText();
        if (string.IsNullOrEmpty(characterName))
        {
            MogoMessageBox.RespError(LangOffset.Character, (int)CharacterCode.INPUT_NAME);
            return;
        }
        var isMatch = System.Text.RegularExpressions.Regex.IsMatch(characterName, @"[\s]+")
            || characterName.Contains(@"\")
            || characterName.Contains(@"/")
            || characterName.Contains(@"[")
            || characterName.Contains(@"]");
        LoggerHelper.Debug("characterName: " + characterName + " isMatch: " + isMatch);
        if (isMatch)
        {
            MogoMessageBox.RespError(LangOffset.Character, (int)CharacterCode.INVALID_NAME);
            return;
        }

        EventDispatcher.TriggerEvent<string, byte, byte>(Events.UIAccountEvent.OnCreateCharacter, characterName, 1, (byte)m_lastSelectedCharacter.vocation);
        GameProcManager.SetGameProgress(ProcType.CreateCharacter, "ChreateCharacter");
    }

    void OnCreateCharacterDetailUIBackBtnUp()
    {
        if (m_lastSelectedCharacter != null)
            m_lastSelectedCharacter.Unfocus();
        foreach (var item in m_avatarList)
        {
            if (item.Value != m_lastSelectedCharacter)
                item.Value.FadeIn();

            int vocation = item.Value.vocation;
            if (vocation == (int)Vocation.Mage && m_sfxHandlers.ContainsKey(vocation))
                m_sfxHandlers[(int)Vocation.Mage].HandleFx(10239, null, (go, fxid) =>
                {
                    go.transform.parent = m_avatarList[vocation].gameObject.transform.Find("Bip_master/bip_weapon_separate/equip_mage_staff_02_p(Clone)");
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localEulerAngles = new Vector3(270, 0, 0);
                    go.transform.localScale = new Vector3(1, 1, 1);
                });
        }
        NewLoginUIViewManager.Instance.ShowCreateCharacterUI();
        m_loginCamera.SetTarget(m_defaultCameraSlot);
        //MogoWorld.scenesManager.LoadCharacterScene(null, true);
    }

    void OnCreateCharacterUIBackBtnUp()
    {
        MogoWorld.LoadCharacterScene();
    }
    private void OnCreateCharacterDetailUISwitch(int dir)
    {
        int pos = m_currentPos;
        if (dir > 0)
        {
            if (pos > 1) --pos;

        }
        else
        {
            if (pos < 4) ++pos;
        }
        OnCreateCharacterDetailUIJobIconUp(pos, true);
    }

    void OnCreateCharacterDetailUIJobIconUp(int pos)
    {
        OnCreateCharacterDetailUIJobIconUp(pos, true);
    }

    void OnCreateCharacterDetailUIJobIconUp(int pos, bool needFadeIn)
    {
        if (pos == m_currentPos) return;
        m_currentPos = pos;
        int vocation = 1;
        switch (pos)
        {
            case 1:
                vocation = 3;
                break;
            case 2:
                vocation = 4;
                break;
            case 3:
                vocation = 1;
                break;
            case 4:
                vocation = 2;
                break;
        }
        OnCreateCharacterSelected(m_avatarList[vocation].gameObject, needFadeIn);
        NewLoginUIViewManager.Instance.SelectCreateCharacterDetailUIJobIcon(pos);
    }

    public void LoadCreateCharacter(Action callback)
    {
        AssetCacheMgr.GetResource("PlayerShader.shader", (obj) =>
        {
            AssetCacheMgr.GetResource("MogoFakeLight.shader", (obj1) =>
            {
                FakeLightShader = (Shader)obj1;
                PlayerShader = (Shader)obj;
                LoadCamera();
                MogoWorld.inCity = false;
                m_lastSelectedCharacter = null;
                LoadCharacters(() =>
                {
                    foreach (var item in m_avatarList)
                    {
                        item.Value.Init();
                        item.Value.SetCreateMode();
                    }

                    var avatarList = m_avatarList.Values.ToList();
                    var counter = 0;
                    for (int i = 0; i < avatarList.Count; i++)
                    {
                        var item = avatarList[i];
                        item.gameObject.transform.position = item.CreatePosition;
                        item.Equip(item.equipList, () =>
                        {
                            counter++;
                            if (counter == m_avatarList.Count * 2)
                                if (callback != null)
                                    callback();
                        });

                        int vocation = item.vocation;

                        item.Equip(item.weapon, () =>
                        {
                            if (vocation == (int)Vocation.Mage && m_sfxHandlers.ContainsKey(vocation))
                                m_sfxHandlers[vocation].HandleFx(10239, null, (go, fxid) =>
                                {
                                    go.transform.parent = m_avatarList[vocation].gameObject.transform.Find("Bip_master/bip_weapon_separate/equip_mage_staff_02_p(Clone)");
                                    go.transform.localPosition = Vector3.zero;
                                    go.transform.localEulerAngles = new Vector3(270, 0, 0);
                                    go.transform.localScale = new Vector3(1, 1, 1);
                                });
                            counter++;
                            if (counter == m_avatarList.Count * 2)
                                if (callback != null)
                                    callback();
                        });
                    }
                });
            });
        });
    }

    #endregion

    #region ѡ�������

    private bool m_isConnectSuccess;

    public void ShowChooseServerUIFormCreateChr(bool isConnectSuccess = true)
    {
        ShowChooseServerUI(isConnectSuccess);
        m_onChooseServerUIBackBtnUp = () =>
        {
            if (!m_isConnectSuccess)
            {
                MogoMessageBox.Info("Please select a server.");
                return;
            }
            MogoWorld.LoadCharacterScene();
        };

        m_onChooseServerGridUp = (index) =>
        {
            if (index != 0 && m_isConnectSuccess && SystemConfig.SelectedServerIndex == index)
            {
                MogoWorld.LoadCharacterScene();
                return;
            }

            //Mogo.Util.LoggerHelper.Debug(index);
            SystemConfig.SelectedServerIndex = index;
            var server = SystemConfig.GetServerInfoByIndex(index);
            Mogo.Util.LoggerHelper.Debug("server.id: " + server.id);
            if (server != null)
                SystemConfig.Instance.SelectedServer = server.id;
            SystemConfig.SaveConfig();
            MogoWorld.Login();
            //EventDispatcher.TriggerEvent<int>(Events.UIAccountEvent.OnChangeServer, id);
        };
    }

    public void ShowChooseServerUIFormLogin()
    {
        ShowChooseServerUI(false);
        m_onChooseServerUIBackBtnUp = () =>
        {
            MogoWorld.LoadLoginScene();
        };

        m_onChooseServerGridUp = (index) =>
        {
            SystemConfig.SelectedServerIndex = index;
            var server = SystemConfig.GetServerInfoByIndex(index);
            Mogo.Util.LoggerHelper.Debug("server.id: " + server.id);
            if (server != null)
                SystemConfig.Instance.SelectedServer = server.id;
            SystemConfig.SaveConfig();
            MogoWorld.LoadLoginScene();
            //EventDispatcher.TriggerEvent<int>(Events.UIAccountEvent.OnChangeServer, id);
        };
    }

    private void ShowChooseServerUI(bool isConnectSuccess = true)
    {
        ClearChooseCharacterModel();
        m_isConnectSuccess = isConnectSuccess;
        NewLoginUIViewManager.Instance.ShowChooseServerUI();
        NewLoginUIViewManager.Instance.ClearServerList();
        var list = SystemConfig.ServersList;
        for (int i = 0; i < list.Count; i++)
        {
            var info = list[i];
            NewLoginUIViewManager.Instance.AddChooseServerGrid(new ChooseServerGridData() { serverName = info.name, serverStatus = (ServerType)info.flag });
        }
        var serverInfo = SystemConfig.GetSelectedServerInfo();
        if (serverInfo != null)
            NewLoginUIViewManager.Instance.ShowLatelyServer(serverInfo.name, 0, true);
        NewLoginUIViewManager.Instance.ShowLatelyServer("", 1, false);
    }

    void OnChooseServerGridUp(int id)
    {
        if (m_onChooseServerGridUp != null)
            m_onChooseServerGridUp(id);
    }

    void ChooseServerUIBackBtnUp()
    {
        if (m_onChooseServerUIBackBtnUp != null)
            m_onChooseServerUIBackBtnUp();
    }

    #endregion

    #region ģ�͹���

    private void ClearChooseCharacterModel()
    {
        var ci = CharacterInfoData.dataMap.GetValueOrDefault(0, new CharacterInfoData());
        foreach (var item in m_avatarList)
        {
            item.Value.Show();
            if (item.Value.gameObject)
                item.Value.gameObject.transform.position = ci.Location;
            item.Value.Hide();
            item.Value.Unfocus();
        }
    }

    private void LoadCharacters(Action loaded)
    {
        LoadCharacter((int)Vocation.Warrior, () =>
        {
            LoadCharacter((int)Vocation.Assassin, () =>
            {
                LoadCharacter((int)Vocation.Archer, () =>
                {
                    LoadCharacter((int)Vocation.Mage, () =>
                    {
                        if (loaded != null)
                            loaded();
                    });
                });
            });
        });
    }

    private void LoadCharacter(int vocation, Action loaded)
    {
        var ci = CharacterInfoData.dataMap.GetValueOrDefault(vocation, new CharacterInfoData());
        var ai = AvatarModelData.dataMap.GetValueOrDefault(vocation, new AvatarModelData());
        CreateCharacterModel(ai, vocation, ci, loaded);
    }

    private void CreateCharacterModel(AvatarModelData ai, int vocation, CharacterInfoData ci, Action loaded)
    {
        if (m_avatarList.ContainsKey(vocation) && loaded != null)
        {
            if (m_avatarList[vocation].actorParent != null)
            {
                loaded();
                return;
            }
            if (m_avatarList[vocation].gameObject != null)
            {
                AssetCacheMgr.ReleaseInstance(m_avatarList[vocation].gameObject);
            }
        }
        AssetCacheMgr.GetInstanceAutoRelease(ai.prefabName, (prefab, id, go) =>
        {
            var ety = new EtyAvatar();
            ety.vocation = vocation;
            ety.equipList = ci.EquipList;
            ety.weapon = ci.Weapon;
            ety.CreatePosition = ci.Location;
            var avatar = (go as GameObject);
            ety.gameObject = avatar;
            avatar.name = vocation.ToString();
            var cc = avatar.GetComponent<Collider>() as CharacterController;
            cc.radius = 0.5f;
            ety.animator = avatar.GetComponent<Animator>();
            ety.animator.applyRootMotion = false;
            ety.PlayerShader = PlayerShader;
            ety.FakeLightShader = FakeLightShader;
            //MogoFXManager.Instance.AttachShadow(avatar, string.Concat(avatar.name, "_Shadow"), 0, 0, 0, 1.5f, 1.5f, 1);

            if (vocation == 1)
            {
                MogoFXManager.Instance.AttachShadow(avatar, string.Concat(avatar.name, "_Shadow"), 0, 0, 0, 1.5f, 1.5f, 1, -0.4f, 0.01f, -0.5f);
            }
            else if (vocation == 2)
            {
                MogoFXManager.Instance.AttachShadow(avatar, string.Concat(avatar.name, "_Shadow"), 0, 0, 0, 1.5f, 1.5f, 1, 0, 0.01f, 0.2f);
            }
            else if(vocation == 3)
            {
                MogoFXManager.Instance.AttachShadow(avatar, string.Concat(avatar.name, "_Shadow"), 0, 0, 0, 1.5f, 1.5f, 1, 0, 0.01f, 0.2f);
            }
            else if (vocation == 4)
            {
                MogoFXManager.Instance.AttachShadow(avatar, string.Concat(avatar.name, "_Shadow"), 0, 0, 0, 1.5f, 1.5f, 1, 0, 0.01f, 0.2f);
            }
            else if (vocation == 3 || vocation == 4)
            {
                // �����ڴ�
                AssetCacheMgr.GetInstanceAutoRelease("fx_jqqd.prefab", (prefabb, idd, goo) =>
                    {
                        GameObject goFX = (GameObject)goo;
                        goFX.transform.parent = avatar.transform;
                        goFX.transform.localScale = Vector3.one;

                        if (vocation == 3)
                        {
                            goFX.transform.localPosition = new Vector3(0, 2.2f, 0);
                        }
                        else
                        {
                            goFX.transform.localPosition = new Vector3(0, 2f, 0);
                        }
                    });
            }

            #region ����

            if (m_Animators.ContainsKey(vocation))
                m_Animators[vocation] = avatar.GetComponent<Animator>();
            else
                m_Animators.Add(vocation, avatar.GetComponent<Animator>());

            #endregion

            avatar.transform.position = ci.Location;
            avatar.transform.localScale = Vector3.one;

            ety.sfxHandler = avatar.AddComponent<SfxHandler>();

            #region ��Ч �� ��Чɾ��������ʱ��

            if (!m_sfxHandlers.ContainsKey(vocation))
            {
                SfxHandler handler = avatar.GetComponent<SfxHandler>();
                if (handler == null)
                    handler = avatar.AddComponent<SfxHandler>();
                m_sfxHandlers.Add(vocation, handler);
                uint timer = uint.MaxValue;
                m_fxTimes.Add(vocation, timer);
            }

            #endregion

            ety.actorParent = avatar.AddComponent<ActorParent>();
            ety.actorParent.SetNakedEquid(ai.nakedEquipList);
            ety.InitEquip();

            if (m_avatarList.ContainsKey(vocation))
            {
                m_avatarList[vocation].sfxHandler.RemoveAllFX();
                AssetCacheMgr.ReleaseInstance(m_avatarList[vocation].gameObject);
            }
            ety.Hide();
            m_avatarList[vocation] = ety;
            AssetCacheMgr.GetResource(ci.controller,
           (obj) =>
           {
               var controller = obj as RuntimeAnimatorController;
               if (ety.animator)
                   ety.animator.runtimeAnimatorController = controller;
               if (loaded != null)
                   loaded();
           });
        });
    }

    public void ReleaseCharacter()
    {
        try
        {
            foreach (var item in m_avatarList)
            {
                item.Value.ResetFade();
                item.Value.RemoveOld();
                AssetCacheMgr.ReleaseInstance(item.Value.gameObject);
                item.Value.Release();
                //item.Value.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].sharedMaterial.shader =
                //    PlayerShader;

            }

            m_avatarList.Clear();

            AssetCacheMgr.ReleaseResourceImmediate("fx_jqqd.prefab");

            #region ��Ч �� ��Чɾ��������ʱ��

            foreach (var item in m_fxTimes)
                TimerHeap.DelTimer(item.Value);

            foreach (var item in m_sfxHandlers)
                item.Value.RemoveAllFX();

            m_sfxHandlers.Clear();
            m_fxTimes.Clear();

            #endregion

            #region ����

            foreach (var item in CharacterInfoData.dataMap)
            {
                //if (item.Value.runtimeAnimatorController != null)
                //{
                if (!String.IsNullOrEmpty(item.Value.controller))
                    AssetCacheMgr.ReleaseResourceImmediate(item.Value.controller);
                //}
            }

            m_Animators.Clear();

            #endregion

            //ж��ģ����Դ
            foreach (var item in AvatarModelData.dataMap)
            {
                if (!String.IsNullOrEmpty(item.Value.prefabName) && item.Value.prefabName.EndsWith(".prefab"))
                    AssetCacheMgr.ReleaseResourceImmediate(item.Value.prefabName);
            }

            MogoFXManager.Instance.RemoveAllShadow();
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    #endregion
}

public class EtyAvatar
{
    public int vocation { get; set; }
    public List<int> equipList { get; set; }
    public int weapon { get; set; }
    public ActorParent actorParent { get; set; }
    public SfxHandler sfxHandler { get; set; }
    public Animator animator { get; set; }
    public GameObject gameObject { get; set; }
    public Vector3 CreatePosition { get; set; }
    private bool m_isShown = true;
    public SkinnedMeshRenderer smr;
    public Shader PlayerShader;
    public Shader FakeLightShader;

    public void Init()
    {
        Show();
        Unfocus();
    }

    public void Release()
    {
        actorParent = null;
        sfxHandler = null;
        animator = null;
        gameObject = null;
        PlayerShader = null;
        FakeLightShader = null;
        smr = null;
    }

    public void ResetFade()
    {
        // return; //MaiFeo
        MogoFXManager.Instance.ResetAlpha(gameObject);

        Transform trans = gameObject.transform.Find("fx_jqqd(Clone)");
        if (trans != null)
        {
            trans.gameObject.SetActive(true);
        }
    }

    public void FadeIn(bool noDelay = false)
    {
        //return;//MaiFeo
        MogoFXManager.Instance.AlphaFadeIn(gameObject, noDelay ? 0 : 1);
        //MogoFXManager.Instance.AlphaFadeIn(weaponGo, 1);
        //GameObject.Destroy(sfxHandler);
        //sfxHandler = gameObject.AddComponent<SfxHandler>();
        //sfxHandler.HandleFx(999902);

        Transform trans = gameObject.transform.Find("fx_jqqd(Clone)");
        if (trans != null)
        {
            trans.gameObject.SetActive(true);
        }
    }

    public void FadeOut(bool noDelay = false)
    {
        //return;//MaiFeo
        MogoFXManager.Instance.AlphaFadeOut(gameObject, noDelay ? 0 : 1);
        //MogoFXManager.Instance.AlphaFadeOut(weaponGo, 1);
        //GameObject.Destroy(sfxHandler);
        //sfxHandler = gameObject.AddComponent<SfxHandler>();
        //sfxHandler.HandleFx(999901);

        Transform trans = gameObject.transform.Find("fx_jqqd(Clone)");
        if (trans != null)
        {
            trans.gameObject.SetActive(false);
        }
    }

    public void Focus()
    {
        //if (sfxHandler)
        //    sfxHandler.HandleFx(51001);
        try
        {
            if (gameObject == null)
            {
                return;
            }

            if (smr == null)
            {
                smr = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true)[0];

            }

            if (FakeLightShader == null)
                return;

            if (smr.sharedMaterial == null)
                return;

            smr.sharedMaterial.shader = FakeLightShader;
            smr.sharedMaterial.SetColor("_RimColor", new Color(1, 0.9f, 0f));
            smr.sharedMaterial.SetColor("_Color", new Color(1, 1, 1));


            if (vocation == 1)
            {
                smr.sharedMaterial.SetFloat("_RimPower", 6);
                smr.sharedMaterial.SetFloat("_FinalPower", 1.2f);
            }
            else
            {
                smr.sharedMaterial.SetFloat("_RimPower", 12);
                smr.sharedMaterial.SetFloat("_FinalPower", 1.15f);
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    public void Unfocus()
    {
        try
        {
            if (gameObject == null)
            {
                return;
            }

            if (smr == null)
            {
                smr = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true)[0];

            }
            if (PlayerShader == null)
                return;

            if (smr.sharedMaterial == null)
                return;
            smr.sharedMaterial.shader = PlayerShader;
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
        //if (sfxHandler)
        //    sfxHandler.RemoveFXs(51001);
    }

    public void Show()
    {
        if (!m_isShown && gameObject)
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 100, gameObject.transform.position.z);
        m_isShown = true;
    }

    public void Hide()
    {
        if (m_isShown && gameObject)
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 100, gameObject.transform.position.z);
        m_isShown = false;
    }

    public void RemoveOld()
    {
        if (actorParent)
            actorParent.RemoveOld();
    }

    public void RemoveAll()
    {
        if (actorParent)
            actorParent.RemoveAll();
    }

    public void InitEquip()
    {
        try
        {
            if (actorParent)
                actorParent.InitNakedEquid();
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    public void Equip(List<int> equips, Action onDone = null)
    {
        try
        {
            if (actorParent)
                actorParent.Equip(equips, onDone);
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    public void Equip(int equipId, Action onDone = null)
    {
        try
        {
            if (actorParent && equipId != 0 && EquipData.dataMap.ContainsKey(equipId))
                actorParent.Equip(equipId, onDone);
            else
            {
                if (onDone != null)
                    onDone();
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    public void SetChooseMode()
    {
        if (animator)
        {
            //actorParent.AddCallbackInFrames(() =>
            //{
            animator.SetInteger("Action", -1);
            actorParent.AddCallbackInFrames(() =>
            {
                if (animator)
                    animator.SetInteger("Action", 0);
            });
            //}, 10);
        }
    }

    public void SetCreateMode()
    {
        if (animator)
        {
            //actorParent.AddCallbackInFrames(() =>
            //{
            animator.SetInteger("Action", 34);
            actorParent.AddCallbackInFrames(() =>
            {
                if (animator)
                    animator.SetInteger("Action", 0);
            });
            //}, 20);
        }
    }
}