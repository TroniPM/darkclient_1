/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoPropSheet
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class MogoPropSheet : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public GameObject[] DialogList;
    public GameObject IconList;
    private GameObject m_currentDialog;
    private GameObject m_currentIcon;

    public int m_iCurrentId;

    void Awake()
    {
        m_currentDialog = DialogList[0].gameObject;
    }

    public void SwitchDialog(int id)
    {
        if (id != m_iCurrentId)
        {
            m_currentDialog.SetActive(false);

            DialogList[id].gameObject.SetActive(true);
            IconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(id);

            m_currentDialog = DialogList[id].gameObject;

            m_iCurrentId = id;
        }
    }
}
