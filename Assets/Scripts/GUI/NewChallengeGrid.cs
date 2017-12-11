using UnityEngine;
using System.Collections;

public class NewChallengeGrid : MonoBehaviour 
{
    Transform m_myTransform;
    UISprite m_spIcon;
    GameObject m_goFXList;
    UILabel m_lblName;
    UILabel m_lblStatusText;
    UISprite m_spFG;

    public System.Action ClickHandler;

    void Awake()
    {
        m_myTransform  = transform;
        m_spIcon = m_myTransform.Find("NewChallengeUIGridBG").GetComponentsInChildren<UISprite>(true)[0];
        m_goFXList = m_myTransform.Find("NewChallengeUIGridFXList").gameObject;
        m_lblName = m_myTransform.Find("NewChallengeUIGridName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblStatusText = m_myTransform.Find("NewChallengeUIGridText").GetComponentsInChildren<UILabel>(true)[0];
        m_spFG = m_myTransform.Find("NewChallengeUIGridNameFG").GetComponentsInChildren<UISprite>(true)[0];
    }

    public void SetIcon(string imgName)
    {
        m_spIcon.spriteName = imgName;
    }

    public void ShowFX(bool isShow)
    {
        m_goFXList.SetActive(isShow);
    }

    public void SetName(string name)
    {
        m_lblName.text = name;
    }

    public void SetStatusText(string text)
    {
        m_lblStatusText.text = text;
    }

    public void ShowFG(bool isShow)
    {
        m_spFG.gameObject.SetActive(isShow);
    }

    public void SetEnable(bool isEnable)
    {
        m_spIcon.ShowAsWhiteBlack(!isEnable);
    }

    void OnClick()
    {
        if (ClickHandler != null)
            ClickHandler();
    }
}
