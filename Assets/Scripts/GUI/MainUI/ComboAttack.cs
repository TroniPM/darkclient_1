using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboAttack : MonoBehaviour
{
    UISprite m_spComboAttackBG;
    UISprite m_spComboAttackHit;
    List<UISprite> m_listComboAttackNum = new List<UISprite>();
    GameObject m_goComboAttackNumList;
    Transform m_myTransform;
    TweenScale m_tsCombo;

    void Awake()
    {
        m_myTransform = transform;

        m_spComboAttackBG = m_myTransform.Find("ComboAttackBG").GetComponentsInChildren<UISprite>(true)[0];
        m_spComboAttackHit = m_myTransform.Find("ComboAttackHit").GetComponentsInChildren<UISprite>(true)[0];
        m_goComboAttackNumList = m_myTransform.Find("ComboAttackNumList").gameObject;
        m_tsCombo = m_myTransform.GetComponentsInChildren<TweenScale>(true)[0];
    }

    void AddComboAttackNum(string num)
    {
        //for (int i = 0; i < num.Length; ++i)
        //{
        //    int index = i;
        //    AssetCacheMgr.GetUIInstance("ComboAttackNum.prefab", (prefab, guid, gameObject) =>
        //        {
        //            GameObject go = (GameObject)gameObject;
        //            go.transform.parent = m_goComboAttackNumList.transform;
        //            go.transform.localScale = new Vector3(40, 52, 1);
        //            go.transform.localPosition = new Vector3(m_listComboAttackNum.Count * go.transform.localScale.x, 0, 0);
        //            go.transform.localEulerAngles = Vector3.zero;
        //            UISprite sp = go.GetComponentsInChildren<UISprite>(true)[0];
        //            sp.spriteName = "bj-" + num[index];
        //            m_listComboAttackNum.Add(sp);
        //        });
        //}

        //Mogo.Util.LoggerHelper.Debug("AddComboAttack " + num);

        if (num.Length > m_listComboAttackNum.Count)
        {
            for (int i = 0; i < m_listComboAttackNum.Count; ++i)
            {
                m_listComboAttackNum[i].spriteName = string.Concat("bj-", num[i]);

                if (m_listComboAttackNum[i].gameObject.activeSelf == false)
                {
                    m_listComboAttackNum[i].gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < num.Length - m_listComboAttackNum.Count; ++i)
            {
                int index = i;
                AssetCacheMgr.GetUIInstance("ComboAttackNum.prefab", (prefab, guid, gameObject) =>
                    {
                        GameObject go = (GameObject)gameObject;
                        go.transform.parent = m_goComboAttackNumList.transform;
                        go.transform.localScale = new Vector3(40, 52, 1);
                        go.transform.localPosition = new Vector3(m_listComboAttackNum.Count * go.transform.localScale.x, 0, 0);
                        go.transform.localEulerAngles = Vector3.zero;
                        UISprite sp = go.GetComponentsInChildren<UISprite>(true)[0];
                        //Mogo.Util.LoggerHelper.Debug("Adding sprite " + num[num.Length-index-1]);
                        sp.spriteName = string.Concat("bj-", num[num.Length - index - 1]);
                        m_listComboAttackNum.Add(sp);
                    });
            }
        }
        else
        {
            for (int i = 0; i < m_listComboAttackNum.Count; ++i)
            {
                if (i < num.Length)
                {
                    m_listComboAttackNum[i].spriteName = string.Concat("bj-", num[i]);

                    if (m_listComboAttackNum[i].gameObject.activeSelf == false)
                    {
                        m_listComboAttackNum[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (m_listComboAttackNum[i].gameObject.activeSelf == true)
                    {
                        m_listComboAttackNum[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    int activeNum = 0;

    void RecalculateListPos()
    {

        activeNum = 0;
        for (int i = 0; i < m_listComboAttackNum.Count; ++i)
        {
            if (m_listComboAttackNum[i].gameObject.activeSelf)
            {
                ++activeNum;
            }
        }

        if (m_listComboAttackNum.Count > 0)
        {
            m_goComboAttackNumList.transform.localPosition = new Vector3(-m_listComboAttackNum[0].transform.localScale.x * activeNum * 0.5f, 10, 0);
        }
    }

    void PlayComboAttackAnim()
    {
        m_tsCombo.Reset();
        m_tsCombo.enabled = true;
        m_tsCombo.Play(true);
    }

    public void SetComboAttackNum(int num)
    {
        string strNum = num.ToString();


        //for (int i = 0; i < m_listComboAttackNum.Count; ++i)
        //{
        //    int index = i;
        //    AssetCacheMgr.ReleaseInstance(m_listComboAttackNum[index].gameObject);
        //}

        //m_listComboAttackNum.Clear();

        AddComboAttackNum(strNum);

        PlayComboAttackAnim();

        RecalculateListPos();

    }


}
