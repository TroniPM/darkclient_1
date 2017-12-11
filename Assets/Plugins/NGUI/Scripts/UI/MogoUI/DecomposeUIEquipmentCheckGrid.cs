using UnityEngine;
using System.Collections;

public class DecomposeUIEquipmentCheckGrid : MonoBehaviour 
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
                DecomposeUIDict.DECOMPOSEUICHECKGRIDUP(id);
            }
            m_bIsDragging = false;
        }
    }
}
