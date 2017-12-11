using UnityEngine;
using System.Collections;

public class WingUIGrid : MonoBehaviour {

    Transform m_myTransform;
    public int ID;
    bool isDraging = false;

    void Awake()
    {
        m_myTransform = transform;

        m_myTransform.Find("WingUIListDialogGridBuyBtn").GetComponentsInChildren<MFUIButtonHandler>(true)[0].ClickHandler =
            OnBuyBtnUp;
     
    }
    public void SetIconImage(string imgName)
    {
        m_myTransform.Find("WingUIListDialogGridIconFG").GetComponent<UISprite>().spriteName = imgName;
    }

    public void SetWingName(string name)
    {
        m_myTransform.Find("WingUIListDialogGridName").GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetWingStatus(string status)
    {
        m_myTransform.Find("WingUIListDialogGridLevel").GetComponentsInChildren<UILabel>(true)[0].text = status;
    }

    public void ShowWingLock(bool isShow)
    {
 
    }

    public void SetWingDescripte(string descripe)
    {
        m_myTransform.Find("WingUIListDialogGridDescripe").GetComponentsInChildren<UILabel>(true)[0].text = descripe;
    }

    public void SetStarNum(int num)
    {
        Debug.LogError(num);
    }

    public void SetWingCost(string cost)
    {
        m_myTransform.Find("WingUIListDialogGridCost/WingUIListDialogGridCostNum").GetComponentsInChildren<UILabel>(true)[0].text = cost;
    }

    public void SetWingProgressText(string text)
    {
        m_myTransform.Find("WingUIListDialogGridProgress/WingUIListDialogGridProgressText").GetComponentsInChildren<UILabel>(true)[0].text = text;
    }

    public void SetWingProgressSize(float size)
    {
        m_myTransform.Find("WingUIListDialogGridProgress/WingUIListDialogGridProgressFG").GetComponentsInChildren<UISprite>(true)[0].transform.localScale =
            new Vector3(244 * size, 14.4f, 1);
    }

    public void ShowWingProgress(bool isShow)
    {
        m_myTransform.Find("WingUIListDialogGridProgress").gameObject.SetActive(isShow);
    }

    public void ShowWingIconBlackWhite(bool isBlackWhite)
    {
        m_myTransform.Find("WingUIListDialogGridIconFG").GetComponentsInChildren<UISprite>(true)[0].ShowAsWhiteBlack(isBlackWhite);
    }

    public void ShowBuyBtn(bool isShow)
    {
        m_myTransform.Find("WingUIListDialogGridBuyBtn").gameObject.SetActive(isShow);

    }

    public void ShowUsedSign(bool isShow)
    {
        m_myTransform.Find("WingUIListDialogGridLoadedSign").gameObject.SetActive(isShow);
    }

    public void ShowCost(bool isShow)
    {
        m_myTransform.Find("WingUIListDialogGridCost").gameObject.SetActive(isShow);
    }

    void OnBuyBtnUp(int id)
    {
        //Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.OpenBuy, ID);
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        Mogo.Util.EventDispatcher.TriggerEvent(MarketEvent.OpenWithWing);
    }

    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            if (isDraging == false)
            {
                WingUIViewManager.Instance.ShowWingTip(true);
                Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.OpenTip, ID);
            }

            isDraging = false;
        }
    }

    void OnDrag()
    {
        //isDraging = true;
    }
}
