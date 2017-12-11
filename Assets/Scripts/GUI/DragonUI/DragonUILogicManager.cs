using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DragonUILogicManager
{

    private static DragonUILogicManager m_instance;

    public static DragonUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DragonUILogicManager();
            }

            return DragonUILogicManager.m_instance;

        }
    }

    void OnDiamondWishUp()
    {
        LoggerHelper.Debug("DiamondWishUp");
        EventDispatcher.TriggerEvent(Events.RuneEvent.RMBRefresh);
    }

    void OnGoldWishUp()
    {
        LoggerHelper.Debug("GoldWishUp");
        EventDispatcher.TriggerEvent(Events.RuneEvent.GameMoneyRefresh);
    }

    void OnGotoRuneUp()
    {
        LoggerHelper.Debug("GotoRuneUp");
    }

    void OnOneKeyComposeUp()
    {
        LoggerHelper.Debug("OnekeyComposeUp");
        EventDispatcher.TriggerEvent<bool>(Events.RuneEvent.AutoCombine, true);
    }

    void OnOneKeyPickUpUp()
    {
        LoggerHelper.Debug("OnekeyPickUpUp");
        EventDispatcher.TriggerEvent(Events.RuneEvent.AutoPickUp);
    }

    void OnDragonUICloseUp()
    {
        LoggerHelper.Debug("OnDragonUICloseUp");
        EventDispatcher.TriggerEvent(Events.RuneEvent.CloseDragon);
    }

    void OnDragonUIPackageGridUp(int id)
    {
        LoggerHelper.Debug("DragonUIPackageGridUp " + id);
        EventDispatcher.TriggerEvent<int, bool>(Events.RuneEvent.UseRune, id, true);

    }

    void OnDragonUIPackageGridUpDouble(int id)
    {
        LoggerHelper.Debug("DragonUIPackageGridUpDouble " + id);
    }

    void OnDragonUIPackageGridDrag(int newGrid, int oldGrid)
    {
        LoggerHelper.Debug(newGrid + " " + oldGrid);
        EventDispatcher.TriggerEvent<int, int, bool>(Events.RuneEvent.ChangeIndex, oldGrid, newGrid, true);
    }


    public void Initialize()
    {
        DragonUIViewManager.Instance.DIAMONDWISHUP += OnDiamondWishUp;
        DragonUIViewManager.Instance.GOLDWISHUP += OnGoldWishUp;
        DragonUIViewManager.Instance.GOTORUNEUP += OnGotoRuneUp;
        DragonUIViewManager.Instance.ONEKEYCOMPOSEUP += OnOneKeyComposeUp;
        DragonUIViewManager.Instance.ONEKEYPICKUPUP += OnOneKeyPickUpUp;
        DragonUIViewManager.Instance.DRAGONUICLOSEUP += OnDragonUICloseUp;
        DragonUIViewManager.Instance.DRAGONUIPACKAGEGRIDUP += OnDragonUIPackageGridUp;
        DragonUIViewManager.Instance.DRAGONUIPACKAGEGRIDUPDOUBLE += OnDragonUIPackageGridUpDouble;

        EventDispatcher.AddEventListener<int, int>("DragonUIPackageGridDrag", OnDragonUIPackageGridDrag);
    }

    public void Release()
    {
        DragonUIViewManager.Instance.DIAMONDWISHUP -= OnDiamondWishUp;
        DragonUIViewManager.Instance.GOLDWISHUP -= OnGoldWishUp;
        DragonUIViewManager.Instance.GOTORUNEUP -= OnGotoRuneUp;
        DragonUIViewManager.Instance.ONEKEYCOMPOSEUP -= OnOneKeyComposeUp;
        DragonUIViewManager.Instance.ONEKEYPICKUPUP -= OnOneKeyPickUpUp;
        DragonUIViewManager.Instance.DRAGONUICLOSEUP -= OnDragonUICloseUp;
        DragonUIViewManager.Instance.DRAGONUIPACKAGEGRIDUP -= OnDragonUIPackageGridUp;
        DragonUIViewManager.Instance.DRAGONUIPACKAGEGRIDUPDOUBLE -= OnDragonUIPackageGridUpDouble;

        EventDispatcher.RemoveEventListener<int, int>("DragonUIPackageGridDrag", OnDragonUIPackageGridDrag);
    }
}
