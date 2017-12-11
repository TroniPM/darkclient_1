using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;


public class DialogUILogicManager
{
    private static DialogUILogicManager m_instance;

    public static DialogUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DialogUILogicManager();
            }

            return DialogUILogicManager.m_instance;

        }
    }

    string[] m_arrDialogName;
    string[] m_arrDialogImg;
    string[] m_arrDialogText;
    int m_iDialogIndex = 0;
    int m_iDialogTextMaxIndex = 0;

    // Auto Timer
    uint talkingTimer = uint.MaxValue;

    void OnDialogUIDownSignUp()
    {
        LoggerHelper.Debug("DialogUIDownSignUp Logic");

        TimerHeap.DelTimer(talkingTimer);
        ++m_iDialogIndex;

        if (m_iDialogIndex > m_iDialogTextMaxIndex && MogoWorld.thePlayer != null)
        {
            if (MogoWorld.thePlayer.sceneId == MogoWorld.globalSetting.homeScene)
            {
                MogoUIManager.Instance.ShowMogoNormalMainUI();
            }
            else
            {
                MogoUIManager.Instance.ShowMogoBattleMainUI();
            }
            if (GuideSystem.Instance.IsGuideDialog)
            {
                GuideSystem.Instance.IsGuideDialog = false;
                EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.FinishDialog);
            }
        }
        else
        {
            SetDialogUINPCName(m_arrDialogName[m_iDialogIndex]);
            SetDialogUINPCImage(m_arrDialogImg[m_iDialogIndex]);
            SetDialogUINPCDialogText(m_arrDialogText[m_iDialogIndex]);

            // Auto
            talkingTimer = TimerHeap.AddTimer(3000, 0, OnDialogUIDownSignUp);
        }
    }

    public void Initialize()
    {
        EventDispatcher.AddEventListener("DialogUI_DownSignUp", OnDialogUIDownSignUp);
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener("DialogUI_DownSignUp", OnDialogUIDownSignUp);
    }
            
    public void SetDialogUINPCImage(string imgName)
    {
        DialogUIViewManager.Instance.SetDialogUINPCImage(imgName);
    }

    public void SetDialogUINPCName(string name)
    {
        DialogUIViewManager.Instance.SetDialogUINPCName(name);
    }

    public void SetDialogUINPCDialogText(string text)
    {
        DialogUIViewManager.Instance.SetDialogUINPCDialogText(text);
    }

    public void SetDialogInfo(string[] npcName, string[] npcImg, string[] dialogText)
    {
        SetDialogUINPCImage(npcImg[0]);
        SetDialogUINPCName(npcName[0]);
        SetDialogUINPCDialogText(dialogText[0]);

        m_iDialogIndex = 0;

        m_arrDialogName = npcName;
        m_arrDialogImg = npcImg;
        m_arrDialogText = dialogText;

        m_iDialogTextMaxIndex = dialogText.Length - 1;

        // Auto
        talkingTimer = TimerHeap.AddTimer(8000, 0, OnDialogUIDownSignUp);
    }
}
