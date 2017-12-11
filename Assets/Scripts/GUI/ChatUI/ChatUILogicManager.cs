using UnityEngine;
using System.Collections;

public class ChatUILogicManager 
{

    private static ChatUILogicManager m_instance;

    public static ChatUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ChatUILogicManager();
            }

            return ChatUILogicManager.m_instance;

        }
    }

    void OnChatUISendUp()
    {
        ChatUIViewManager.Instance.AddChatUIDialogText(ChatUIViewManager.Instance.GetChatUILogicText());
        ChatUIViewManager.Instance.EmptyChatUIInput();
    }

    void OnChatUIShowUp()
    {
        //ChatUIViewManager.Instance.AddChatUIInputText("<info=(1,1,1)>" + "[FF00FF][³¬¼¶Õ½¸«][-]");
        ChatUIViewManager.Instance.AddChatUIInputText("<info=(1,1,1)>");
    }

    void OnChatUIShow2Up()
    {
        ChatUIViewManager.Instance.AddChatUIInputText("<info=(1,0,0)>");
    }

    void OnChatUICloseUp()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnChatUIShowFaceUp()
    {
        ChatUIViewManager.Instance.AddChatUIInputText("<face=(1)>");
    }

    public void Initialize()
    {
        ChatUIViewManager.Instance.CHATUISENDUP += OnChatUISendUp;
        ChatUIViewManager.Instance.CHATUISHOWUP += OnChatUIShowUp;
        ChatUIViewManager.Instance.CHATUICLOSEUP += OnChatUICloseUp;
        ChatUIViewManager.Instance.CHATUISHOW2UP += OnChatUIShow2Up;
        ChatUIViewManager.Instance.CHATUISHOWFACEUP += OnChatUIShowFaceUp;
    }

    public void Release()
    {
        ChatUIViewManager.Instance.CHATUISENDUP -= OnChatUISendUp;
        ChatUIViewManager.Instance.CHATUISHOWUP -= OnChatUIShowUp;
        ChatUIViewManager.Instance.CHATUICLOSEUP -= OnChatUICloseUp;
        ChatUIViewManager.Instance.CHATUISHOW2UP -= OnChatUIShow2Up;
        ChatUIViewManager.Instance.CHATUISHOWFACEUP -= OnChatUIShowFaceUp;
    }
}
