using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DecomposeUIDict 
{

    public static Dictionary<string, Action> ButtonTypeToEventUp = new Dictionary<string, Action>();

    public static Action<int> DECOMPOSEUIPACKAGEUP;
    public static Action<int> DECOMPOSEUICHECKGRIDUP;
    public static Action DECOMPOSEBUTTONUP;
    public static Action UNLOCKBTNUP;

    public static Action DECOMPOSECHOOSEEQUIPWASTE;
    public static Action DECOMPOSECHOOSEEQUIPALL;
}
