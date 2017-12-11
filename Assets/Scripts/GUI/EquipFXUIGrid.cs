using UnityEngine;
using System.Collections;

public class EquipFXUIGrid : MonoBehaviour 
{
    public int ID;
    UISprite m_spIconFG;
    UILabel m_lblTextNotGet;
    UILabel m_lblTextGot;
    UILabel m_lblProgress;
    UISprite m_spProgressFG;
    GameObject m_goProgress;
    GameObject m_goActiveBtn;
    UILabel m_lblActiveBtnText;
    UISprite m_spGetSign;

    Transform m_myTransform;

    void Awake()
    {
        m_myTransform = transform;

        m_spIconFG = m_myTransform.Find("EquipFXUIDialogFXGridIconFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblTextNotGet = m_myTransform.Find("EquipFXUIDialogFXGridNotGetText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblTextGot = m_myTransform.Find("EquipFXUIDialogFXGridGotText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblProgress = m_myTransform.Find("EquipFXUIDialogFXGridProcess/EquipFXUIDialogFXGridProcessText").GetComponentsInChildren<UILabel>(true)[0];
        m_spProgressFG = m_myTransform.Find("EquipFXUIDialogFXGridProcess/EquipFXUIDialogFXGridProcessFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblActiveBtnText = m_myTransform.Find("EquipFXUIDialogFXGridActiveBtn/EquipFXUIDialogFXGridActiveBtnText").GetComponentsInChildren<UILabel>(true)[0];

        m_goProgress = m_myTransform.Find("EquipFXUIDialogFXGridProcess").gameObject;
        m_goActiveBtn = m_myTransform.Find("EquipFXUIDialogFXGridActiveBtn").gameObject;
        m_spGetSign = m_myTransform.Find("EquipFXUIDialogFXGridGetSign").GetComponentsInChildren<UISprite>(true)[0];

       //InventoryManager.Instance.EquipOnDic[1-11].jewelSlots //-1没宝石 宝石ID
        //BodyEnhanceManager.Instance.myEnhance[1-10]
        //InventoryManager.Instance.EquipOnDic[1-11].quality



        
    }

    public void SetActiveBtnUpHandler(System.Action<int> act)
    {
        m_myTransform.Find("EquipFXUIDialogFXGridActiveBtn").GetComponentsInChildren<MFUIButtonHandler>(true)[0].ID = this.ID;
        m_myTransform.Find("EquipFXUIDialogFXGridActiveBtn").GetComponentsInChildren<MFUIButtonHandler>(true)[0].ClickHandler = act;
    }
    public void SetIconFG(string imgName)
    {
        m_spIconFG.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        m_spIconFG.spriteName = imgName;
    }

    public void SetNotGetText(string text)
    {
        m_lblTextNotGet.text = text;
    }

    public void SetGotText(string text)
    {
        m_lblTextGot.text = text;
    }

    public void SetProgressText(string progress)
    {
        m_lblProgress.text = progress;
    }

    public void SetPrgressSize(float size)
    {
        m_spProgressFG.transform.localScale = new Vector3(size * 224f, 22f, 1);
    }

    public void ShowNotGetText(bool isShow)
    {
        m_lblTextNotGet.gameObject.SetActive(isShow);
    }

    public void ShowGotText(bool isShow)
    {
        m_lblTextGot.gameObject.SetActive(isShow);
    }

    public void ShowActiveBtn(bool isShow)
    {
        m_goActiveBtn.SetActive(isShow);
    }

    public void ShowProgress(bool isShow)
    {
        m_goProgress.SetActive(isShow);
    }

    public void SetActiveBtnText(string text)
    {
        m_lblActiveBtnText.text = text;
    }

    public void ShowGetSign(bool isShow)
    {
        m_spGetSign.gameObject.SetActive(isShow);
    }
}
