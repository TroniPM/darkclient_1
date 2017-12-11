using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;


public class TaskUILogicManager
{
    private static TaskUILogicManager m_instance;

    public static TaskUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TaskUILogicManager();
            }

            return TaskUILogicManager.m_instance;

        }
    }

    string[] m_arrDialogName;
    string[] m_arrDialogImg;
    string[] m_arrDialogText;
    int m_iDialogIndex = 0;
    int m_iDialogTextMaxIndex = 0;

    // Auto Timer
    uint talkingTimer = uint.MaxValue;

    void OnTaskUIDownSignUp()
    {
        LoggerHelper.Debug("TaskUIDownSignUp Logic");

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
            else
            {
                EventDispatcher.TriggerEvent(Events.TaskEvent.TalkEnd);
            }
        }
        else
        {
            SetTaskUINPCName(m_arrDialogName[m_iDialogIndex]);
            SetTaskUINPCImage(m_arrDialogImg[m_iDialogIndex]);
            SetTaskUINPCDialogText(m_arrDialogText[m_iDialogIndex]);

            // Auto
            talkingTimer = TimerHeap.AddTimer(8000, 0, OnTaskUIDownSignUp);
        }
    }

    public void Initialize()
    {
        EventDispatcher.AddEventListener("TaskUI_DownSignUp", OnTaskUIDownSignUp);
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener("TaskUI_DownSignUp", OnTaskUIDownSignUp);
    }
            
    public void SetTaskUINPCImage(string imgName)
    {
        TaskUIViewManager.Instance.SetTaskUINPCImage(imgName);
    }

    public void SetTaskUINPCName(string name)
    {
        TaskUIViewManager.Instance.SetTaskUINPCName(name);
    }

    public void SetTaskUINPCDialogText(string text)
    {
        TaskUIViewManager.Instance.SetTaskUINPCDialogText(text);
    }

    public void SetTaskInfo(string[] npcName, string[] npcImg, string[] dialogText)
    {
        SetTaskUINPCImage(npcImg[0]);
        SetTaskUINPCName(npcName[0]);
        SetTaskUINPCDialogText(dialogText[0]);

        m_iDialogIndex = 0;

        m_arrDialogName = npcName;
        m_arrDialogImg = npcImg;
        m_arrDialogText = dialogText;

        m_iDialogTextMaxIndex = dialogText.Length - 1;

        // Auto
        talkingTimer = TimerHeap.AddTimer(8000, 0, OnTaskUIDownSignUp);
    }
}
