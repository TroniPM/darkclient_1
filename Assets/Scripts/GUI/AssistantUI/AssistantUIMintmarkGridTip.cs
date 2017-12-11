using UnityEngine;
using System.Collections;

public class AssistantUIMintmarkGridTip
{
    UILabel m_lblSkillName;
    UILabel m_lblElemAttack;
    UILabel m_lblElemDef;
    UILabel m_lblAddtiveAttack;
    UILabel m_lblAddtiveDef;
    UILabel m_lblGetText;
    UILabel m_lblGet;

    UISprite m_spElemAttack;
    UISprite m_spElemDef;
    UISprite m_spAddtiveAttack;
    UISprite m_spAddtiveDef;
    UISprite m_spGet;

    private GameObject m_skillGridTip;

    public AssistantUIMintmarkGridTip(GameObject obj)
    {
        m_skillGridTip = obj;

        m_lblSkillName = m_skillGridTip.transform.Find("ElementMintmarkTipNameText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblElemAttack = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr0").GetComponentsInChildren<UILabel>(true)[0];
        m_lblElemDef = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr1").GetComponentsInChildren<UILabel>(true)[0];
        m_lblAddtiveAttack = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr2").GetComponentsInChildren<UILabel>(true)[0];
        m_lblAddtiveDef = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr3").GetComponentsInChildren<UILabel>(true)[0];
        m_lblGetText = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipGetText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblGet = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipGetMessage").GetComponentsInChildren<UILabel>(true)[0];

        m_spElemAttack = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr0Sign").GetComponentsInChildren<UISprite>(true)[0];
        m_spElemDef = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr1Sign").GetComponentsInChildren<UISprite>(true)[0];
        m_spAddtiveAttack = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr2Sign").GetComponentsInChildren<UISprite>(true)[0];
        m_spAddtiveDef = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipAttr3Sign").GetComponentsInChildren<UISprite>(true)[0];
        m_spGet = m_skillGridTip.transform.Find("ElementMintmarkTipAttrList/ElementMintmarkTipGetSign").GetComponentsInChildren<UISprite>(true)[0];
    }

    string m_skillName;

    public string SkillName
    {
        get { return m_skillName; }
        set 
        {
            m_skillName = value;
            m_lblSkillName.text = m_skillName;
        }
    }
    string m_elemAttack;

    public string ElemAttack
    {
        get { return m_elemAttack; }
        set
        { 
            m_elemAttack = value;
            m_lblElemAttack.text = m_elemAttack;
        }
    }

    string m_elemDef;

    public string ElemDef
    {
        get { return m_elemDef; }
        set 
        { 
            m_elemDef = value;
            m_lblElemDef.text = m_elemDef;
        }
    }
    string m_addtiveAttack;

    public string AddtiveAttack
    {
        get { return m_addtiveAttack; }
        set
        { 
            m_addtiveAttack = value;
            m_lblAddtiveAttack.text = m_addtiveAttack;
        }
    }
    string m_addtiveDef;

    public string AddtiveDef
    {
        get { return m_addtiveDef; }
        set 
        { 
            m_addtiveDef = value;
            m_lblAddtiveDef.text = m_addtiveDef;
        }
    }
    string m_getText;

    public string GetText
    {
        get { return m_getText; }
        set
        {
            m_getText = value;
            m_lblGetText.text = m_getText;
        }
    }
    string m_get;

    public string Get
    {
        get { return m_get; }
        set 
        {
            m_get = value;
            m_lblGet.text = m_get;
        }
    }

};
