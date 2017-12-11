/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SimplePlatformGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class SimplePlatformGear : GearParent 
{
    public bool isOnPlatform { get; protected set; }
    public Transform playerTransform { get; protected set; }

    void Start()
    {
        isOnPlatform = false;
        triggleEnable = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                isOnPlatform = true;
                playerTransform = other.gameObject.transform;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                playerTransform = other.gameObject.transform;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                isOnPlatform = false;
                playerTransform = null;
            }
        }
    }

    /*
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.tag == "Player")
        {
            LoggerHelper.Debug("OnCollisionEnter");
            isOnPlatform = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.collider.gameObject.tag == "Player")
        {
            LoggerHelper.Debug("OnCollisionExit");
            isOnPlatform = false;
        }
    }
     * */

    public void OnMoveDown()
    {
        transform.parent.gameObject.GetComponent<Animation>().enabled = true;
        transform.parent.gameObject.GetComponent<Animation>().CrossFade("10506_PlatOneDown");
    }

    public void Test()
    {
        isOnPlatform = true;
    }
}
