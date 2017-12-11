using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DecomposeUILogicManager
{

    private static DecomposeUILogicManager m_instance;

    public static DecomposeUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DecomposeUILogicManager();
            }

            return DecomposeUILogicManager.m_instance;
        }
    }

    ChooseDecomposeEquip m_ChooseDecomposeEquipType = ChooseDecomposeEquip.Waste;// 默认选择垃圾装备
    public ChooseDecomposeEquip ChooseDecomposeEquipType
    {
        get 
        {
            return m_ChooseDecomposeEquipType;
        }
        set
        {
            m_ChooseDecomposeEquipType = value;
            EventDispatcher.TriggerEvent<int>(DecomposeManager.ON_CHOOSE_EQUIP_UP, (int)ChooseDecomposeEquipType);
        }
    }

    void OnDecomposeUIPackageUp(int id)
    {
        LoggerHelper.Debug(id + " Up");
        EventDispatcher.TriggerEvent<int>(DecomposeManager.ON_EQUIP_SELECT,id);
    }

    void OnDecomposeUIButtonUp()
    {
        LoggerHelper.Debug("Decompose UP");
        EventDispatcher.TriggerEvent(DecomposeManager.ON_DECOMPOSE);
    }

    void OnDecomposeUIPackageCheckUp(int id)
    {
        LoggerHelper.Debug(id + " Up check");
        EventDispatcher.TriggerEvent<int>(DecomposeManager.ON_CHECK_UP,id);
    }

    void OnUnlockBtnUp()
    {
        EventDispatcher.TriggerEvent(DecomposeManager.ON_LOCK_CHANGE);
    }

    void OnDecomposeChooseEquipWasteUp()
    {
        ChooseDecomposeEquipType = ChooseDecomposeEquip.Waste;        
    }

    void OnDecomposeChooseEquipAllUp()
    {
        ChooseDecomposeEquipType = ChooseDecomposeEquip.All;
    }

    public void Initialize()
    {
        DecomposeUIDict.DECOMPOSEUIPACKAGEUP += OnDecomposeUIPackageUp;
        DecomposeUIDict.DECOMPOSEBUTTONUP += OnDecomposeUIButtonUp;
        DecomposeUIDict.DECOMPOSEUICHECKGRIDUP += OnDecomposeUIPackageCheckUp;
        DecomposeUIDict.UNLOCKBTNUP += OnUnlockBtnUp;
        DecomposeUIDict.DECOMPOSECHOOSEEQUIPWASTE += OnDecomposeChooseEquipWasteUp;
        DecomposeUIDict.DECOMPOSECHOOSEEQUIPALL += OnDecomposeChooseEquipAllUp;
    }

    public void Release()
    {
        DecomposeUIDict.DECOMPOSEUIPACKAGEUP -= OnDecomposeUIPackageUp;
        DecomposeUIDict.DECOMPOSEBUTTONUP -= OnDecomposeUIButtonUp;
        DecomposeUIDict.DECOMPOSEUICHECKGRIDUP -= OnDecomposeUIPackageCheckUp;
        DecomposeUIDict.UNLOCKBTNUP -= OnUnlockBtnUp;
        DecomposeUIDict.DECOMPOSECHOOSEEQUIPWASTE -= OnDecomposeChooseEquipWasteUp;
        DecomposeUIDict.DECOMPOSECHOOSEEQUIPALL -= OnDecomposeChooseEquipAllUp;
    }
}
