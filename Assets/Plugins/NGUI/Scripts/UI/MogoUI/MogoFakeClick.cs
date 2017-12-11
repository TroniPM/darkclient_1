using UnityEngine;
using System.Collections;

public enum ReleadClassType
{
    Type_Default = 0,
    Type_NormalMainUI = 1,
    Type_BattleMainUI = 2,
    Type_ChallengeUI = 3,
    Type_DragonUI = 4,
    Type_RuneUI = 5,
    Type_MenuUI = 6,
    Type_SkillUI = 7,
    Type_SocietyUI = 8,
    Type_ChargeRewardUI = 9,
    Type_CommunityUI = 10,
    Type_ComposeUI= 11,
    Type_DecomposeUI = 12,
    Type_TaskUI = 13,
    Type_StrenthUI = 14,
    Type_SettingsUI = 15,
    Type_InstanceUI = 16,
    Type_InsetUI = 17,
    Type_InstanceLevelChooseUI = 18,
    Type_EnergyUI = 19,
    Type_DiamondToGoldUI = 20,
    Type_DialogUI = 21,
}

public class MogoFakeClick : MonoBehaviour 
{

    public ReleadClassType ReletedClassType = ReleadClassType.Type_NormalMainUI;

    public void FakeIt()
    {
        switch (ReletedClassType)
        {
            case ReleadClassType.Type_NormalMainUI:
                gameObject.GetComponent<NormalMainUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_BattleMainUI:
                gameObject.GetComponent<MainUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_ChallengeUI:
                gameObject.GetComponent<ChallengeUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_DragonUI:
                gameObject.GetComponent<DragonUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_RuneUI:
                gameObject.GetComponent<RuneUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_MenuUI:
                gameObject.GetComponent<MenuUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_SkillUI:
                gameObject.GetComponent<SkillUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_SocietyUI:
                gameObject.GetComponent<SocietyUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_ChargeRewardUI:
                //gameObject.GetComponent<ChargeRewardUIButton>().FakePress(true);            //MaiFeo ×¢ÊÍµÈÅËÄ³
                break;

            case ReleadClassType.Type_CommunityUI:
                gameObject.GetComponent<CommunityUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_ComposeUI:
                gameObject.GetComponent<ComposeUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_DecomposeUI:
                gameObject.GetComponent<DecomposeUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_TaskUI:
                gameObject.GetComponent<TaskUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_StrenthUI:
                gameObject.GetComponent<StrenthenUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_SettingsUI:
                gameObject.GetComponent<SettingsUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_InstanceUI:
                gameObject.GetComponent<InstanceUIButton>().FakePress(true);
                break;

            case ReleadClassType.Type_InsetUI:
                gameObject.GetComponent<InsetUIButton>().FakePress(true);
                break;
        }
    }
}
