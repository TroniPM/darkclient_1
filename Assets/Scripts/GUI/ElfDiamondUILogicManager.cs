using UnityEngine;
using System.Collections;

public class ElfDiamondUILogicManager : MFUILogicUnit
{

    static ElfDiamondUILogicManager m_instance;

    public static ElfDiamondUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ElfDiamondUILogicManager();
            }

            return ElfDiamondUILogicManager.m_instance;
        }
    }

    System.Action dirtyAct;
    string m_strNumCanGet;
    string m_strNumTotalCost;
    string m_strNumTotalGot;
    string m_strCostNum;

    public override void FillBufferedData()
    {
        if (dirtyAct != null)
            dirtyAct();

        SetDiamondNumCanGet(m_strNumCanGet);
        SetTotalCostDiamondNum(m_strNumTotalCost);
        SetTotalGotDiamondNum(m_strNumTotalGot);
        SetCostDiamondNum(m_strCostNum);
    }

    public void SetUIDirty()
    {
        dirtyAct = MFUIUtils.SafeDoAction(ElfDiamondUIViewManager.Instance,
            () => { ElfDiamondUIViewManager.Instance.SetUIDirty(); });
    }

    public void SetDiamondNumCanGet(string num)
    {
        m_strNumCanGet = (string)MFUIUtils.SafeSetValue(ElfDiamondUIViewManager.Instance,
            () => { ElfDiamondUIViewManager.Instance.SetDiamondNumCanGet(num); }, num);
    }


    public void SetTotalCostDiamondNum(string num)
    {
        m_strNumTotalCost =(string) MFUIUtils.SafeSetValue(ElfDiamondUIViewManager.Instance,
            () => { ElfDiamondUIViewManager.Instance.SetTotalCostDiamondNum(num); }, num);
    }

    public void SetTotalGotDiamondNum(string num)
    {
        m_strNumTotalGot = (string)MFUIUtils.SafeSetValue(ElfDiamondUIViewManager.Instance,
            () => { ElfDiamondUIViewManager.Instance.SetTotalGotDiamondNum(num); }, num);
    }

    public void SetCostDiamondNum(string num)
    {
        m_strCostNum = (string)MFUIUtils.SafeSetValue(ElfDiamondUIViewManager.Instance,
            () => { ElfDiamondUIViewManager.Instance.SetCostDiamondNum(num); }, num);
    }
}
