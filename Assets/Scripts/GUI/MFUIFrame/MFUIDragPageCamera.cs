using UnityEngine;
using System.Collections;

public class MFUIDragPageCamera : MFUIDragCamera
{
    int m_iCurrentPage = 0;
    public int ItemNumPerPage = 1;
    int m_iTotalPages = 1;

    System.Collections.Generic.List<Vector3> m_listPagePos = new System.Collections.Generic.List<Vector3>();

   
    
    public override void MoveHorizontal(bool isForward,float length)
    {
        int targetPage;

        if (isForward)
        {
            targetPage = m_iCurrentPage + 1;

            if (targetPage > m_iTotalPages - 1)
            {
                targetPage = m_iTotalPages - 1;
            }
        }
        else
        {
            targetPage = m_iCurrentPage - 1;

            if (targetPage < 0)
            {
                targetPage = 0;
            }
        }

        m_iCurrentPage = targetPage;

        MoveToTarget(m_listPagePos[targetPage]);
    }

    public override void MovePerpendicular(bool isForward, float length)
    {
        int targetPage;

        if (isForward)
        {
            targetPage = m_iCurrentPage + 1;

            if (targetPage > m_iTotalPages - 1)
            {
                targetPage = m_iTotalPages - 1;
            }
        }
        else
        {
            targetPage = m_iCurrentPage - 1;

            if (targetPage < 0)
            {
                targetPage = 0;
            }

        }
        m_iCurrentPage = targetPage;
        MoveToTarget(m_listPagePos[targetPage]);
    }

    public override void CallWhenRegisterDragItem()
    {
        m_iTotalPages = CalculateTotalPages();

        m_listPagePos.Clear();

        for (int i = 0; i < m_iTotalPages; ++i)
        {
            if (IsHorizontal)
            {
                Vector3 tmpPos = new Vector3((GetDragItemList()[(i + 1) * ItemNumPerPage - 1].transform.localPosition.x -
                    GetDragItemList()[ItemNumPerPage * ((i + 1) - 1)].transform.localPosition.x) * 0.5f +
                    GetDragItemList()[ItemNumPerPage * ((i + 1) - 1)].transform.localPosition.x,
                    GetDragItemList()[0].transform.localPosition.y, GetDragItemList()[0].transform.localPosition.z);

                m_listPagePos.Add(tmpPos);
            }
            else
            {
                Vector3 tmpPos = new Vector3(GetDragItemList()[0].transform.localPosition.x,
                    GetDragItemList()[ItemNumPerPage * ((i + 1) - 1)].transform.localPosition.y -
                    (GetDragItemList()[ItemNumPerPage * ((i + 1) - 1)].transform.localPosition.y -
                    GetDragItemList()[(i + 1) * ItemNumPerPage - 1].transform.localPosition.y) * 0.5f,
                    GetDragItemList()[0].transform.localPosition.z);

                m_listPagePos.Add(tmpPos);
            }
        }
    }

    int CalculateTotalPages()
    {
        int pages = GetDragItemList().Count / ItemNumPerPage;

        if (GetDragItemList().Count % ItemNumPerPage != 0)
        {
            ++pages;
        }

        return pages;
    }
	
}
