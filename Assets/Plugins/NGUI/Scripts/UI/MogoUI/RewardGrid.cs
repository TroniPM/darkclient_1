using UnityEngine;
using System.Collections;
using Mogo.Util;

public class RewardGrid : MonoBehaviour 
{

    public int Id = -1;
    bool m_bIsDragging = false;

    void OnPress(bool isPress)
    {
        if (!isPress)
        {
            if (!m_bIsDragging)
            {
                EventDispatcher.TriggerEvent<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDUP, Id);
            }

            m_bIsDragging = false;
        }
        else
        {
            EventDispatcher.TriggerEvent<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDDOWN, Id);
        }
    }

    void OnDrag(Vector2 v)
    {
        m_bIsDragging = true;

        EventDispatcher.TriggerEvent<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDDRAG, Id);
    }
}
