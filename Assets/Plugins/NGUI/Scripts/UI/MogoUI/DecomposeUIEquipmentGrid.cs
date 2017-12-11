using UnityEngine;
using System.Collections;

public class DecomposeUIEquipmentGrid : MonoBehaviour
{
    public int id;

    Transform m_myTransform;

    bool m_bIsDragging = false;

    void OnDrag()
    {
        m_bIsDragging = true;
    }

    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            if (!m_bIsDragging)
            {
                DecomposeUIDict.DECOMPOSEUIPACKAGEUP(id);
            }
            m_bIsDragging = false;
        }
    }
}
