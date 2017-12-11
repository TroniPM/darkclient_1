using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StrenthUIDict 
{

    public static Dictionary<string, Action> ButtonTypeToEventUp = new Dictionary<string, Action>();

    public static Action<int> STRENTHENEQUIPMENTGRIDUP;

    public static  Action STRENTHENUP;
    public static Action EQUIPMENTGRIDUP;
    public static Action MATERIALTIPUP;
    public static Action MATERIALOBTAINTIPCLOSEUP;
}
