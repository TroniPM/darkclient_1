/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������InsetUIEquipmentGrid
// �����ߣ�MaiFeo
// �޸����б�
// �������ڣ�
// ģ��������
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class InsetUIEquipmentGrid : MonoBehaviour
{

    public int id;

    Transform m_myTransform;
    // Ϊȥ��������ʱ�������´���
    //bool m_bIsDragging = false;

    //void OnDrag()
    //{
    //    m_bIsDragging = true;
    //}


    //void OnPress(bool isPressed)
    //{
    //    Mogo.Util.LoggerHelper.Debug("Pressed");
    //    if (!isPressed)
    //    {
    //        if (!m_bIsDragging)
    //        {
    //            InsetUIDict.INSETUIEQUIPMENTGRIDUP(id);
    //        }
    //        m_bIsDragging = false;
    //    }
    //}

    void OnClick()
    {
        InsetUIDict.INSETUIEQUIPMENTGRIDUP(id);
    }

    public void FakeClick()
    {
        InsetUIDict.INSETUIEQUIPMENTGRIDUP(id);
    }
}
