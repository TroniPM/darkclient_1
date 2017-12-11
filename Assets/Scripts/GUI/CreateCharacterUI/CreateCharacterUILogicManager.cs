/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：CreateCharacterUILogicManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.Game;

public class CreateCharacterUILogicManager
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion


    private static CreateCharacterUILogicManager m_instance;

    public static CreateCharacterUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new CreateCharacterUILogicManager();
            }

            return CreateCharacterUILogicManager.m_instance;

        }
    }

    private byte m_iJobID = 1;

    void OnCreateCharacterUp()
    {
        LoggerHelper.Debug("Create Character " + CreateCharacterUIViewManager.Instance.GetNewCharacterName() + " With Job " + m_iJobID);

        string characterName = CreateCharacterUIViewManager.Instance.GetNewCharacterName();
        if (string.IsNullOrEmpty(characterName))
        {
            MogoMessageBox.RespError(LangOffset.Character, (int)CharacterCode.INPUT_NAME);
            return;
        }
        EventDispatcher.TriggerEvent<string, byte, byte>(Events.UIAccountEvent.OnCreateCharacter, characterName, 1, m_iJobID);
    }

    void OnCreateCharacterBackUp()
    {
        MogoUIManager.Instance.ShowMogoChooseCharacterUI();
    }

    void OnCreateCharacterJobGrid0Up()
    {
        m_iJobID = 1;
        LoggerHelper.Debug("Job Grid 1 Up");
    }

    void OnCreateCharacterJobGrid1Up()
    {
        m_iJobID = 2;
        LoggerHelper.Debug("Job Grid 2 Up");
    }

    void OnCreateCharacterJobGrid2Up()
    {
        m_iJobID = 3;
        LoggerHelper.Debug("Job Grid 3 Up");
    }

    void OnCreateCharacterJobGrid3Up()
    {
        m_iJobID = 4;
        LoggerHelper.Debug("Job Grid 4 Up");
    }

    public void Initialize()
    {
        CreateCharacterUIViewManager.Instance.CREATECHARACTERUP += OnCreateCharacterUp;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERBACKUP += OnCreateCharacterBackUp;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID0UP += OnCreateCharacterJobGrid0Up;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID1UP += OnCreateCharacterJobGrid1Up;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID2UP += OnCreateCharacterJobGrid2Up;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID3UP += OnCreateCharacterJobGrid3Up;
    }

    public void Release()
    {
        CreateCharacterUIViewManager.Instance.CREATECHARACTERUP -= OnCreateCharacterUp;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERBACKUP -= OnCreateCharacterBackUp;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID0UP -= OnCreateCharacterJobGrid0Up;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID1UP -= OnCreateCharacterJobGrid1Up;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID2UP -= OnCreateCharacterJobGrid2Up;
        CreateCharacterUIViewManager.Instance.CREATECHARACTERJOBGRID3UP -= OnCreateCharacterJobGrid3Up;
    }
}
