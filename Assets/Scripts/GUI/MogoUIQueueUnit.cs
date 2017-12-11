using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;

public class MogoUIQueueUnit
{
    public Action act;

    public uint Id;
    public ulong Priority = 0;
    public GameObject BaseUI;

    public void JustDoIt()
    {
        if (act != null)
        {
            act();
        }
    }
}
