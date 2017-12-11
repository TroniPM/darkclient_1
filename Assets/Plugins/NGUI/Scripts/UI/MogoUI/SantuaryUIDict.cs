using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SantuaryUIDict 
{

    public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    public static class SanctuaryUIEvent
    {
        public const string REWARDGRIDUP = "SancturayUI_RewardGridUp";
        public const string REWARDGRIDDOWN = "SancturayUI_RewardGridDown";
        public const string REWARDGRIDDRAG = "SancturayUI_RewardGridDrag";
    }
}
