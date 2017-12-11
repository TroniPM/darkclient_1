using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;
public class BattleMenuUILogicManager
{
    
    private static BattleMenuUILogicManager m_instance;
    public bool m_TowerFinishSingle=false;
    public static BattleMenuUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new BattleMenuUILogicManager();
            }

            return BattleMenuUILogicManager.m_instance;

        }
    }

    void OnQuitInstanceButtonUp()
    {
        MogoUIManager.Instance.ShowMogoBattleMainUI();
        LoggerHelper.Debug("QuitInstance");

        switch (MapData.dataMap.Get(MogoWorld.thePlayer.sceneId).type)
        {
            case MapType.TOWERDEFENCE:
                EventDispatcher.TriggerEvent(Events.CampaignEvent.ExitCampaign);
                break;

            case MapType.ClimbTower:
                if (m_TowerFinishSingle)
                {
                    MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(816), (rst) =>
                    {
                        if (rst)
                        {
                            EventDispatcher.TriggerEvent(Events.InstanceEvent.ReturnHome);
                            MainUIViewManager.Instance.ShowBossTarget(false);
                            //InstanceUIViewManager.Instance.ResetFriendShip();
                            //MogoUIManager.Instance.ShowMogoNormalMainUI();
                            MogoGlobleUIManager.Instance.ConfirmHide();
                            m_TowerFinishSingle = false;
                        }
                        else
                        {
                            MogoUIManager.Instance.ShowMogoBattleMainUI();
                            MogoGlobleUIManager.Instance.ConfirmHide();
                        }

                    }
                    );
                }
                else 
                {
                    MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(817), (rst) =>
                    {
                        if (rst)
                        {
                            EventDispatcher.TriggerEvent(Events.InstanceEvent.ReturnHome);
                            MainUIViewManager.Instance.ShowBossTarget(false);
                            //InstanceUIViewManager.Instance.ResetFriendShip();
                            //MogoUIManager.Instance.ShowMogoNormalMainUI();
                            MogoGlobleUIManager.Instance.ConfirmHide();
                        }
                        else
                        {
                            MogoUIManager.Instance.ShowMogoBattleMainUI();
                            MogoGlobleUIManager.Instance.ConfirmHide();
                        }

                    }
                    );
                }
                break;
            default:
                EventDispatcher.TriggerEvent(Events.InstanceEvent.ReturnHome);
                MainUIViewManager.Instance.ShowBossTarget(false);
                //MogoUIManager.Instance.ShowMogoNormalMainUI();
                break;
        }
    }

    public void Initialize()
    {
        BattleMenuUIViewManager.Instance.QUITINSTANCEBUTTONUP += OnQuitInstanceButtonUp;
    }
    public void FinishSingle()
    {
        m_TowerFinishSingle = true;
    }

    public void Release()
    {
        BattleMenuUIViewManager.Instance.QUITINSTANCEBUTTONUP -= OnQuitInstanceButtonUp;
    }
}
