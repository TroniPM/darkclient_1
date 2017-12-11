using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NormalMainUIDict 
{

    public static Dictionary<string, Action> ButtonTypeToEventUp = new Dictionary<string, Action>();
    public static Dictionary<string, Action<bool>> ButtonTypeToEventPress = new Dictionary<string, Action<bool>>();
}
