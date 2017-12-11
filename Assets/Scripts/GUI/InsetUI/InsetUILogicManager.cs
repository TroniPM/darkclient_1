using UnityEngine;
using System.Collections;

using Mogo.Util;

public class InsetUILogicManager
{

    private static InsetUILogicManager m_instance;

    public static InsetUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new InsetUILogicManager();
            }

            return InsetUILogicManager.m_instance;

        }
    }

    void OnInsetEquipmentGridUp(int id)
    {
        LoggerHelper.Debug(id + " Up");
        EventDispatcher.TriggerEvent<int>(InsetManager.ON_EQUIP_SELECT, id);
    }

    void OnInsetPackageGridUp(int id)
    {
        LoggerHelper.Debug(id + " Up");
        EventDispatcher.TriggerEvent<int>(InsetManager.ON_JEWEL_SELECT, id);
    }

    void OnInsetDiamondGridUp(int id)
    {
        LoggerHelper.Debug(id + " Grid UP");
        EventDispatcher.TriggerEvent<int>(InsetManager.ON_JEWEL_SLOT_SELECT,id);

    }

    void OnInsetDiamonUnLoadUp(int id)
    {
        LoggerHelper.Debug(id + " UnLoad Up");
        EventDispatcher.TriggerEvent<int>(InsetManager.DISASSEMBLE_JEWEL, id);
    }

    void OnInsetDiamondUpdateUp(int id)
    {
        LoggerHelper.Debug(id + " Update Up");
        EventDispatcher.TriggerEvent<int>(InsetManager.ON_JEWEL_UPGRADE, id);
       
    }

    void OnInsetDiamondGridUpDouble(int id)
    {
        LoggerHelper.Debug(id + " Grid up Double");
    }

    void OnInsetPacakgeGridDragBegin(int id)
    {

        LoggerHelper.Debug(id + "Begin");
        EventDispatcher.TriggerEvent<int>(InsetManager.ON_JEWEL_DRAG, id);
    }

    void OnInsetPackageGridDrag(int newId, int oldId)
    {

        LoggerHelper.Debug(newId + " " + oldId);
        EventDispatcher.TriggerEvent<int>(InsetManager.ON_INSET_JEWEL, newId);
    }

    void OnInsetDialogDiamondTipInsetUp(int i)
    {
        LoggerHelper.Debug("Inset");
        EventDispatcher.TriggerEvent(InsetManager.ON_INSET_JEWEL,-1);
    }

    public void Initialize()
    {
        InsetUIDict.INSETUIEQUIPMENTGRIDUP += OnInsetEquipmentGridUp;
        InsetUIDict.INSETUIPACKAGEGRIDUP += OnInsetPackageGridUp;

        InsetUIDict.INSETDIAMONDGRIDUP += OnInsetDiamondGridUp;
        InsetUIDict.INSETDIAMONDGRIDUPDOUBLE += OnInsetDiamondGridUpDouble;
        InsetUIDict.INSETDIAMONDUNLOADUP += OnInsetDiamonUnLoadUp;
        InsetUIDict.INSETDIAMONDUPDATEUP += OnInsetDiamondUpdateUp;

        InsetUIDict.INSETPACKAGEGRIDDRAGBEGIN += OnInsetPacakgeGridDragBegin;
        InsetUIDict.INSETPACKAGEGRIDDRAG += OnInsetPackageGridDrag;

        InsetUIDict.INSETDIALOGDIAMONDTIPINSETUP += OnInsetDialogDiamondTipInsetUp;
    }

    public void Release()
    {
        InsetUIDict.INSETUIEQUIPMENTGRIDUP -= OnInsetEquipmentGridUp;
        InsetUIDict.INSETUIPACKAGEGRIDUP -= OnInsetPackageGridUp;

        InsetUIDict.INSETDIAMONDGRIDUP -= OnInsetDiamondGridUp;
        InsetUIDict.INSETDIAMONDGRIDUPDOUBLE -= OnInsetDiamondGridUpDouble;
        InsetUIDict.INSETDIAMONDUNLOADUP -= OnInsetDiamonUnLoadUp;
        InsetUIDict.INSETDIAMONDUPDATEUP -= OnInsetDiamondUpdateUp;

        InsetUIDict.INSETPACKAGEGRIDDRAGBEGIN -= OnInsetPacakgeGridDragBegin;
        InsetUIDict.INSETPACKAGEGRIDDRAG -= OnInsetPackageGridDrag;
        InsetUIDict.INSETDIALOGDIAMONDTIPINSETUP -= OnInsetDialogDiamondTipInsetUp;
    }
}
