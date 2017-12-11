using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainUIDict 
{

    public static Dictionary<string, string> ButtonTypeToEventUp = new Dictionary<string, string>();

    static public class MainUIEvent
    {
        public const string NORMALATTACKDOWN = "MainUI_NormalAttackDown";
        public const string NORMALATTACK = "MainUI_NormalAttack";
        public const string POWERCHARGESTART = "MainUI_PowerChargeStart";
        public const string POWERCHARGECOMPLETE = "MainUI_PowerChargeComplete";
        public const string RESETPOWER = "MainUI_ResetPower";
        public const string POWERCHARGEINTERRUPT = "MainUI_PowerChargeInterrupt";
        public const string NORMALATTACTUP = "MainUI_NormalAttackUp";
        public const string OUTPUTUP = "MainUI_OutputUp";
        public const string AFFECTUP = "MainUI_AffectUp";
        public const string MOVEUP = "MainUI_MoveUp";
        public const string SPECIALUP = "MainUI_SpecialUp";
        public const string SPRITESKILLUP = "MainUI_SpriteSkillUp";
        public const string ITEM1UP = "MainUI_Item1Up";
        public const string TASKUP = "MainUI_TaskUp";
        public const string COMMUNITYUP = "MainUI_CommunityUp";
        public const string COMMUNITY = "MainUI_Community";
        public const string PLAYERINFOBGUP = "MainUI_PlayerInfoBGUp";
        public const string MAINUIITEM1BGUP = "MainUI_MainUIItem1BGUP";
        public const string TASKINFOUP = "MainUI_TaskInfoUp";
        public const string CANCELMANAGEDUP = "MainUI_CancelManagedUp";
    }
}
