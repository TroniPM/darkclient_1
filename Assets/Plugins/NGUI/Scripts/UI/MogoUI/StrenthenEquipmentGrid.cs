using UnityEngine;
using System.Collections;

public class StrenthenEquipmentGrid : MonoBehaviour
{
    public int id;

    Transform m_myTransform;
    // 为去除警告暂时屏蔽以下代码
    //bool m_bIsDragging = false;

    //void OnDrag()
    //{
    //    m_bIsDragging = true;
    //}

    //void OnPress(bool isPressed)
    //{
    //    if (!isPressed)
    //    {
    //        if (!m_bIsDragging)
    //        {
    //            if (StrenthUIDict.STRENTHENEQUIPMENTGRIDUP != null)
    //                StrenthUIDict.STRENTHENEQUIPMENTGRIDUP(id);
    //        }
    //        m_bIsDragging = false;
    //    }
    //}

    void OnClick()
    {
        if (StrenthUIDict.STRENTHENEQUIPMENTGRIDUP != null)
            StrenthUIDict.STRENTHENEQUIPMENTGRIDUP(id);
    }
}
