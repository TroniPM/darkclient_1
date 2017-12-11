/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ConveyPointGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class TeleportPointDesGear : GearParent
{
    public string lable;

    public TeleportPointSrcGear srcGear;

    public bool isResetControlStick;
    protected bool hasResetControlStick;

    protected bool autoProcess;

    void Start() 
    {
        gearType = "TeleportPointDes";

        autoProcess = true;

        hasResetControlStick = false;
	}


    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (!hasResetControlStick)
                    if (isResetControlStick && ControlStick.instance != null)
                    {
                        ControlStick.instance.Reset();
                        hasResetControlStick = true;
                    }

                if (autoProcess)
                {
                    autoProcess = false;
                    // EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, gearType);
                    srcGear.ResetTimer();
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (!hasResetControlStick)
                    if (isResetControlStick && ControlStick.instance != null)
                    {
                        ControlStick.instance.Reset();
                        hasResetControlStick = true;
                    }

                if (autoProcess)
                {
                    autoProcess = false;
                    // EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, gearType);
                    srcGear.ResetTimer();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                autoProcess = true;
            }
        }
    }

    #endregion
}
