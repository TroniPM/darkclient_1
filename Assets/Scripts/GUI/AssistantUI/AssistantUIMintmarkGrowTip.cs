using UnityEngine;
using System.Collections;

public class AssistantUIMintmarkGrowTip
{
    UILabel m_lblMoney;
    UILabel m_lblContractText;
    UILabel m_lblContractLevel;
    UILabel m_lblLevText;
    UILabel m_lblLev;
    UILabel m_lblAttr0;
    UILabel m_lblAttr1;
    UILabel m_lblAttr2;
    UILabel m_lblAttr3;

    UILabel m_lblNextLevText;
    UILabel m_lblNextLev;
    UILabel m_lblNextAttr0;
    UILabel m_lblNextAttr1;
    UILabel m_lblNextAttr2;
    UILabel m_lblNextAttr3;

    UILabel m_lblCostText;
    UILabel m_lblCost;

    UISprite m_spAttr0;
    UISprite m_spAttr1;
    UISprite m_spAttr2;
    UISprite m_spAttr3;

    UISprite m_spNewAttr0;
    UISprite m_spNewAttr1;
    UISprite m_spNewAttr2;
    UISprite m_spNewAttr3;

    GameObject m_skillGrowTip;

    public AssistantUIMintmarkGrowTip(GameObject obj)
    {
        m_skillGrowTip = obj;

        m_lblMoney = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipMoneyText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblContractText = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblContractLevel = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLevText = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLev = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipCurrentDetail/ElementMintmarkGrowTipCurrentDetailLevelNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblAttr0 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipCurrentDetail/ElementMintmarkGrowTipCurrentDetailAttr0/ElementMintmarkGrowTipCurrentDetailAttr0Text").GetComponentsInChildren<UILabel>(true)[0];
        m_lblAttr1 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipCurrentDetail/ElementMintmarkGrowTipCurrentDetailAttr1/ElementMintmarkGrowTipCurrentDetailAttr1Text").GetComponentsInChildren<UILabel>(true)[0];
        m_lblAttr2 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipCurrentDetail/ElementMintmarkGrowTipCurrentDetailAttr2/ElementMintmarkGrowTipCurrentDetailAttr2Text").GetComponentsInChildren<UILabel>(true)[0];
        m_lblAttr3 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipCurrentDetail/ElementMintmarkGrowTipCurrentDetailAttr3/ElementMintmarkGrowTipCurrentDetailAttr3Text").GetComponentsInChildren<UILabel>(true)[0];

        m_lblNextLevText = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipNextDetail/ElementMintmarkGrowTipNextDetailLevelText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNextLev = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipNextDetail/ElementMintmarkGrowTipNextDetailLevelNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNextAttr0 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipNextDetail/ElementMintmarkGrowTipNextDetailAttr0/ElementMintmarkGrowTipNextDetailAttr0Text").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNextAttr1 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipNextDetail/ElementMintmarkGrowTipNextDetailAttr1/ElementMintmarkGrowTipNextDetailAttr1Text").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNextAttr2 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipNextDetail/ElementMintmarkGrowTipNextDetailAttr2/ElementMintmarkGrowTipNextDetailAttr2Text").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNextAttr3 = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipNextDetail/ElementMintmarkGrowTipNextDetailAttr3/ElementMintmarkGrowTipNextDetailAttr3Text").GetComponentsInChildren<UILabel>(true)[0];

        m_lblCostText = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipCostText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblCost = m_skillGrowTip.transform.Find("ElementMintmarkGrowTipCostNum").GetComponentsInChildren<UILabel>(true)[0];
    }

    string money;

    public string Money
    {
        get { return money; }
        set
        {
            money = value;
            m_lblMoney.text = money;
        }
    }
    string contractText;

    public string ContractText
    {
        get { return contractText; }
        set
        {
            contractText = value;
            m_lblContractText.text = contractText;
        }
    }
    string contractLevel;

    public string ContractLevel
    {
        get { return contractLevel; }
        set
        {
            contractLevel = value;
            m_lblContractLevel.text = contractLevel;
        }
    }
    string levText;

    public string LevText
    {
        get { return levText; }
        set
        {
            levText = value;
            m_lblLevText.text = levText;
        }
    }
    string lev;

    public string Lev
    {
        get { return lev; }
        set
        {
            lev = value;
            m_lblLev.text = lev;
        }
    }
    string attr0;

    public string Attr0
    {
        get { return attr0; }
        set
        {
            attr0 = value;
            m_lblAttr0.text = attr0;
        }
    }
    string attr1;

    public string Attr1
    {
        get { return attr1; }
        set
        {
            attr1 = value;
            m_lblAttr1.text = attr1;
        }
    }
    string attr2;

    public string Attr2
    {
        get { return attr2; }
        set
        {
            attr2 = value;
            m_lblAttr2.text = attr2;
        }
    }
    string attr3;

    public string Attr3
    {
        get { return attr3; }
        set
        {
            attr3 = value;
            m_lblAttr3.text = attr3;
        }
    }

    string nextLevText;

    public string NextLevText
    {
        get { return nextLevText; }
        set
        {
            nextLevText = value;
            m_lblNextLevText.text = levText;
        }
    }
    string nextLev;

    public string NextLev
    {
        get { return nextLev; }
        set
        {
            nextLev = value;
            m_lblNextLev.text = lev;
        }
    }
    string nextAttr0;

    public string NextAttr0
    {
        get { return nextAttr0; }
        set
        {
            nextAttr0 = value;
            m_lblNextAttr0.text = nextAttr0;
        }
    }
    string nextAttr1;

    public string NextAttr1
    {
        get { return nextAttr1; }
        set
        {
            nextAttr1 = value;
            m_lblNextAttr1.text = nextAttr1;
        }
    }
    string nextAttr2;

    public string NextAttr2
    {
        get { return nextAttr2; }
        set
        {
            nextAttr2 = value;
            m_lblNextAttr2.text = nextAttr2;
        }
    }
    string nextAttr3;
    public string NextAttr3
    {
        get { return nextAttr3; }
        set
        {
            nextAttr3 = value;
            m_lblNextAttr3.text = nextAttr3;
        }
    }

    string costText;

    public string CostText
    {
        get { return costText; }
        set
        {
            costText = value;
            m_lblCostText.text = costText;
        }
    }
    string cost;

    public string Cost
    {
        get { return cost; }
        set
        {
            cost = value;
            m_lblCost.text = cost;
        }
    }
}
