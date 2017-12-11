using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuUIDict 
{

    public static Dictionary<string, string> ButtonTypeToEventUp = new Dictionary<string, string>();

    public static class MenuUIEvent
    {
        public const string PACKAGEICONUP = "MenuUI_PackageIconUp";
        public const string PLAYERICONUP = "MenuUI_PlayerIconUp";
        public const string SETTINGSICONUP = "MenuUI_SettingsIconUp";
        public const string SKILLICONUP = "MenuUI_SkillIconUp";
        public const string SOCIALICONUP = "MenuUI_SocialIconUp";
        public const string TONGICONUP = "MenuUI_TongIconUp";

        public const string PLAYERINFODETAILUP = "MenuUI_PlayerInfoDetailUp";
        public const string PLAYERINFOEQUIPFXUP = "MenuUI_PlayerInfoEquipFXUp";
        public const string PLAYERDETAILATTRIBUTECLOSEUP = "MenuUI_PlayerDetailAttributeCloseUp";

        public const string PACKAGEGRIDUP = "MenuUI_PackageGridUp";
        public const string PACKAGEGRIDUPSIGNUP = "MenuUI_PackageGridSignUp";

        public const string PACKAGESORTUP = "MenuUI_PacakgeSortUp";
        public const string PACKAGERESOLVEUP = "MenuUI_PacakgeSolveUp";

        public const string PACKAGEEQUIPINFODETAILPUTUPUP = "MenuUI_PackageEquipInfoDetailPutUpUp";
        public const string PACKAGEEQUIPINFODETAILSAILUP = "MenuUI_PackageEquipInfoDetailSailUp";
        public const string PACKAGEEQUIPINFODETAILSHOWUPUP = "MenuUI_PackageEquipInfoDetailShowUpUp";

        public const string PACKAGEEQUIPINFODETAILINSETUP = "MenuUI_PackageEquipInfoDetailInsetUp";
        public const string PACKAGEEQUIPINFODETAILSTRENTHUP = "MenuUI_PacakgeEquipInfoDetailStrenthUp";
        public const string PACKAGEEQUIPINFODETAILUNLOADUP = "MenuUI_PacakgeEquipInfoDetailUnLoadUp";

        public const string PACKAGEEQUIPINFOCLOSEUP = "MenuUI_PackageEquipInfoCloseUp";

        public const string MENUUICLOSEUP = "MenuUI_CloseUp";

        public const string PLAYERUIEQUPMENTGRIDUP = "MenuUI_EquipmentGridUp";

        public const string GEMICONUP = "MenuUI_GemIconUp";
        public const string NORMALICONUP = "MenuUI_NormalIconUp";
        public const string MATERIALICONUP = "MenuUI_MaterialIconUp";
        public const string PACKAGEOUTOFBOUNDSMAXUP = "MenuUI_PackageOutOfBoundsMaxUp";
        public const string PACKAGEOUTOFBOUNDSMINUP = "MenuUI_PackageOutOfBoundsMinUp";

        public const string DIAMONDTIPINSETUP = "MenuUI_DiamondTipInsetUp";
        public const string DIAMONDTIPCLOSEUP = "MenuUI_DiamondTipCloseUp";
        public const string DIAMONDTIPCOMPOSEUP = "MenuUI_DiamondTipComposeUp";
        public const string DIAMONDTIPHISTROYUP = "MenuUI_DiamondTipHistroyUp";
        public const string DIAMONDTIPBUYUP = "MenuUI_DiamondTipBuyUp";

        public const string MATERIL_TIP_CLOSE = "menuUI_MATERIL_TIP_CLOSET";
        public const string MATERIL_TIP_ENHANCE = "menuUI_MATERIL_TIP_ENHANCE";
        public const string MATERIL_TIP_SELL = "menuUI_MATERIL_TIP_SELL";

        public const string PROP_TIP_CLOSE = "menuUI_PROP_TIP_CLOSE";
        public const string PROP_TIP_SELL = "menuUI_PROP_TIP_SELL";
        public const string PROP_TIP_USE = "menuUI_PROP_TIP_USE";

        public const string PLAYERBASICATTRBUTE = "menuUI_PLAYER_BASIC_ATTRBUTE";
        public const string PLAYERELEMENTATTRBUTE = "menuUI_PLAYER_ELEMENT_ATTRBUTE";
    }
}
