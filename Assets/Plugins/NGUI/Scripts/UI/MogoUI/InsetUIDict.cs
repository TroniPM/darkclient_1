using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InsetUIDict 
{

    public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    public static Action<int> INSETUIEQUIPMENTGRIDUP;
    public static Action<int> INSETUIPACKAGEGRIDUP;
    public static Action<int> INSETPACKAGEGRIDDRAGBEGIN;
    public static Action<int, int> INSETPACKAGEGRIDDRAG;
    public static Action<int> INSETDIALOGDIAMONDTIPINSETUP;

    public static Action<int> INSETDIAMONDGRIDUP;
    public static Action<int> INSETDIAMONDGRIDUPDOUBLE;
    public static Action<int> INSETDIAMONDUNLOADUP;
    public static Action<int> INSETDIAMONDUPDATEUP;
}
