using UnityEngine;
using System.Collections;

public class AssistantUISkillGridTip
{
    UILabel m_lblSkillName;
    UILabel m_lblBaseAttr;
    UILabel m_lblAdditiveAttr;
    UILabel m_lblDescrText;
    UILabel m_lblDescr;
    UILabel m_lblGetText;
    UILabel m_lblGet;

    UISprite m_spBaseAttr;
    UISprite m_spAddtiveAttr;
    UISprite m_spDescr;
    UISprite m_spGet;

    private GameObject m_skillGridTip;

    public AssistantUISkillGridTip(GameObject obj)
    {
        m_skillGridTip = obj;

        m_lblSkillName = m_skillGridTip.transform.Find("SkillsContractTipNameText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseAttr = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipAttr0").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDescrText = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipDescripteText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDescr = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipDescripteMessage").GetComponentsInChildren<UILabel>(true)[0];
        m_lblGetText = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipGetText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblGet = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipGetMessage").GetComponentsInChildren<UILabel>(true)[0];
        m_lblAdditiveAttr = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipAttr1").GetComponentsInChildren<UILabel>(true)[0];

        m_spBaseAttr = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipAttr0Sign").GetComponentsInChildren<UISprite>(true)[0];
        m_spAddtiveAttr = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipAttr1Sign").GetComponentsInChildren<UISprite>(true)[0];
        m_spDescr = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipDescripteSign").GetComponentsInChildren<UISprite>(true)[0];
        m_spGet = m_skillGridTip.transform.Find("SkillsContractTipAttrList/SkillsContractTipGetSign").GetComponentsInChildren<UISprite>(true)[0];
    }

    string m_skillName;
    string m_baseAttr;
    string m_additiveAttr;
    string m_descrText;
    string m_descr;
    string m_getText;
    string m_get;

    public string AdditiveAttr
    {
        get { return m_additiveAttr; }
        set 
        { 
            m_additiveAttr = value;
            m_lblAdditiveAttr.text = m_additiveAttr;
        }
    }
    public string DescrText
    {
        get { return m_descrText; }
        set
        {
            m_descrText = value;
            m_lblDescrText.text = m_descrText;
        }
    }
    public string Descr
    {
        get { return m_descr; }
        set
        {
            m_descr = value;
            m_lblDescr.text = m_descr;
        }
    }
    public string GetText
    {
        get { return m_getText; }
        set
        {
            m_getText = value;
            m_lblGetText.text = m_getText;
        }
    }
    public string Get
    {
        get { return m_get; }
        set 
        {
            m_get = value;
            m_lblGet.text = m_get;
        }
    }
    public string BaseAttr
    {
        get { return m_baseAttr; }
        set
        {
            m_baseAttr = value;
            m_lblBaseAttr.text = m_baseAttr;
        }
    }
    public string SkillName
    {
        get { return m_skillName; }
        set 
        { 
            m_skillName = value;
            m_lblSkillName.text = m_skillName;
        }
    }
};
