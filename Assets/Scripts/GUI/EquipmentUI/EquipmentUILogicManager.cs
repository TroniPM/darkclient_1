using UnityEngine;
using System.Collections;
using Mogo.Util;

public class EquipmentUILogicManager 
{
    private static EquipmentUILogicManager m_instance;

    public static EquipmentUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EquipmentUILogicManager();
            }

            return EquipmentUILogicManager.m_instance;
        }
    }

    void OnInsetIconUp()
    {
        LoggerHelper.Debug("Inset");
        MogoUIManager.Instance.SwitchInsetUI();
        InventoryManager.Instance.m_currentEquipmentView = InventoryManager.View.InsetView;
        InventoryManager.Instance.CurrentView = InventoryManager.View.InsetView;
    }

    void OnDecomposeIconUp()
    {
        LoggerHelper.Debug("Decompose");
        MogoUIManager.Instance.SwitchDecomposeUI();
        InventoryManager.Instance.m_currentEquipmentView = InventoryManager.View.DecomposeView;
        InventoryManager.Instance.CurrentView = InventoryManager.View.DecomposeView;

        if (MogoUIManager.Instance.m_DecomposeUI != null)
        {
            DecomposeUIViewManager.Instance.ShowDecomposeJewlTip(false);
        }
    }

    void OnComposeIconUp()
    {
        LoggerHelper.Debug("Compose");
        MogoUIManager.Instance.SwitchComposeUI();

        InventoryManager.Instance.m_currentEquipmentView = InventoryManager.View.ComposeView;
        InventoryManager.Instance.CurrentView = InventoryManager.View.ComposeView;
    }

    void OnStrenthIconUp()
    {
        LoggerHelper.Debug("Strenthen");
        MogoUIManager.Instance.SwitchStrenthUI();
        InventoryManager.Instance.m_currentEquipmentView = InventoryManager.View.BodyEnhanceView;
        InventoryManager.Instance.CurrentView = InventoryManager.View.BodyEnhanceView;
        EventDispatcher.TriggerEvent(BodyEnhanceManager.ON_SHOW);
    }

    void SetEquipmentUICloseValueZ(bool isTop)
    {
        if (EquipmentUIViewManager.Instance != null)
            EquipmentUIViewManager.Instance.SetEquipmentUICloseValueZ(isTop);
    }

    public void Initialize()
    {
        EquipmentUIViewManager.Instance.INSETICONUP += OnInsetIconUp;
        EquipmentUIViewManager.Instance.DECOMPOSEICONUP += OnDecomposeIconUp;
        EquipmentUIViewManager.Instance.COMPOSEICONUP += OnComposeIconUp;
        EquipmentUIViewManager.Instance.STRENTHICONUP += OnStrenthIconUp;
        EventDispatcher.AddEventListener<bool>(Events.EquipmentEvent.SetEquipmentUICloseValueZ, SetEquipmentUICloseValueZ);
    }

    public void Release()
    {
        EquipmentUIViewManager.Instance.INSETICONUP -= OnInsetIconUp;
        EquipmentUIViewManager.Instance.DECOMPOSEICONUP -= OnDecomposeIconUp;
        EquipmentUIViewManager.Instance.COMPOSEICONUP -= OnComposeIconUp;
        EquipmentUIViewManager.Instance.STRENTHICONUP -= OnStrenthIconUp;
        EventDispatcher.RemoveEventListener<bool>(Events.EquipmentEvent.SetEquipmentUICloseValueZ, SetEquipmentUICloseValueZ);
    }
}
