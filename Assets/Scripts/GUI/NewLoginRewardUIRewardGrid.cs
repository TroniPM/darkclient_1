using UnityEngine;
using System.Collections;

public class NewLoginRewardUIRewardGrid : MonoBehaviour
{

    Transform m_myTransform;
    UILabel m_lblDays;
    UILabel m_lblSendToMail;
    GameObject m_goGetBtn;

    void Awake()
    {
        m_myTransform = transform;

        m_lblDays = m_myTransform.Find("NewLoginRewardUIRewardGridDays").GetComponentsInChildren<UILabel>(true)[0];

        m_lblSendToMail = m_myTransform.Find("NewLoginRewardUIRewardGridSendToMail").GetComponentsInChildren<UILabel>(true)[0];

        m_goGetBtn = m_myTransform.Find("NewLoginRewardUIGetBtn").gameObject;


    }

    public void SetDays(string days)
    {
        m_lblDays.text = days;
    }

    public void ShowSendToMailText(bool isShow)
    {
        m_lblSendToMail.gameObject.SetActive(isShow);
    }

    public void ShowGetBtn(bool isShow)
    {
        m_goGetBtn.SetActive(isShow);
    }

    public void SetItemImg(string imgName, int id)
    {
        m_myTransform.Find(string.Concat("NewLoginRewardUIRewardGridItemList/NewLoginRewardUIRewardGridItem", id, "/NewLoginRewardUIRewardGridItem", id, "FG")).GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
    }

    public void SetItemNum(string num, int id)
    {
        m_myTransform.Find(string.Concat("NewLoginRewardUIRewardGridItemList/NewLoginRewardUIRewardGridItem", id, "/NewLoginRewardUIRewardGridItem", id, "Text")).GetComponentsInChildren<UILabel>(true)[0].text = string.Concat("x",num);
    }

    public void SetItemID(int itemId, int id)
    {
        InventoryManager.SetIcon(itemId, m_myTransform.Find(string.Concat("NewLoginRewardUIRewardGridItemList/NewLoginRewardUIRewardGridItem", id, "/NewLoginRewardUIRewardGridItem", id, "FG")).GetComponentsInChildren<UISprite>(true)[0],
            0, null, m_myTransform.Find(string.Concat("NewLoginRewardUIRewardGridItemList/NewLoginRewardUIRewardGridItem", id, "/NewLoginRewardUIRewardGridItem", id, "BG")).GetComponentsInChildren<UISprite>(true)[0]);
    }

    public void ShowItemGrid(bool isShow,int id)
    {
        m_myTransform.Find(string.Concat("NewLoginRewardUIRewardGridItemList/NewLoginRewardUIRewardGridItem", id)).gameObject.SetActive(isShow);
    }

    public void ShowItemGotSign(bool isShow)
    {
        for (int i = 0; i < 4; ++i)
        {
            m_myTransform.Find(string.Concat("NewLoginRewardUIRewardGridItemList/NewLoginRewardUIRewardGridItem", i, "/NewLoginRewardUIRewardGridItem", i, "GetSign")).gameObject.SetActive(isShow);
        }
    }
}
