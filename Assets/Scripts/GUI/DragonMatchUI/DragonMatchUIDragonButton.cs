#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DragonMatchUIDragonButton : MonoBehaviour 
{
    public int m_index;
    public int Index
    {
        get { return m_index; }
        set
        {
            m_index = value;

            if (cc != null)
            {
                cc.center = new Vector3(0, 3.5f, 0);
                cc.radius = 1.5f;
                cc.height = 5;
            }
        }
    }

    public Camera DragonCamera;
    private CharacterController cc;

    void Awake()
    {
        cc = transform.GetComponentInChildren<CharacterController>();     
    }

    void Start()
    {
        Index = Index;
    }

    void OnPress(bool isOver)
    {
        if (DragonCamera == null || cc == null)
            return;

        if (isOver)
        {

        }
        else
        {  
            RaycastHit hit = new RaycastHit();
            if (cc.Raycast(DragonCamera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
            {
                if (transform.name == "DragonMatchUIDragon")
                {
                    LoggerHelper.Debug("OnSelect DragonMatchUIDragon" + Index);
                    if (DragonMatchUIViewManager.Instance != null
                        && DragonMatchUIViewManager.Instance.DRAGONMATCHUIPLAYERUP != null)
                        DragonMatchUIViewManager.Instance.DRAGONMATCHUIPLAYERUP(Index);
                }
            }
        }
    }
}
