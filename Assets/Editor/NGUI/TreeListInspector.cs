using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TreeList))]
public class TreeListInspector : Editor
{
    TreeList mTL;

    private int m_oldRow;
    private int m_newRow;
    private int m_oldColumn;
    private int m_newColumn;

    void ReDraw(int oldRow, int newRow,int oldColumn,int newColumn)
    {
        Mogo.Util.LoggerHelper.Debug(oldRow);
        Mogo.Util.LoggerHelper.Debug(newRow);
        Mogo.Util.LoggerHelper.Debug(oldColumn);
        Mogo.Util.LoggerHelper.Debug(newColumn);

        //处理列
        if (oldRow != newRow)
        {
            if (oldRow > newRow)    //删除列
            {
                for (int i = 0; i < oldRow - newRow; ++i)
                {
                    var iconList = mTL.transform.GetChild(0).GetComponentsInChildren<UIImageButton>(true);

                    DestroyImmediate(iconList[iconList.Length - (oldRow - 1 - i)].transform.parent.gameObject); //反序

                    var valueList = mTL.transform.GetChild(1).GetComponentsInChildren<TreeListGird>(true);

                    for (int j = 0; j < valueList.Length; ++j)
                    {
                        if (valueList[j].RowNum == (oldRow - 1 - i))
                        {
                            Mogo.Util.LoggerHelper.Debug(valueList[j].name);
                            DestroyImmediate(valueList[j].gameObject);
                        }
                    }
                    
                }
            }
            else                   //添加列
            {
                for (int i = oldRow; i < newRow; ++i)
                {
                    GameObject go = (GameObject)Instantiate(mTL.transform.GetChild(0).GetChild(0).gameObject);
                    go.transform.parent = mTL.transform.GetChild(0).transform;
                    go.name = "TitleBGC" + i;
                    go.transform.localScale = mTL.transform.GetChild(0).GetChild(0).transform.localScale;
                    go.transform.localPosition = new Vector3(mTL.transform.GetChild(0).GetChild(0).transform.localPosition.x +
                    mTL.transform.GetChild(0).GetChild(0).GetComponentInChildren<BoxCollider>().size.x * i, 
                    mTL.transform.GetChild(0).GetChild(0).transform.localPosition.y,mTL.transform.GetChild(0).GetChild(0).transform.localPosition.z);
                }

                for (int i = oldRow; i < newRow; ++i)
                {
                    for (int j = 0; j < m_newColumn; ++j)
                    {

                        GameObject gov = (GameObject)Instantiate(mTL.transform.GetChild(1).GetChild(0).gameObject);
                        gov.transform.parent = mTL.transform.GetChild(1).transform;
                        gov.name = "Value" + i + j;
                        gov.GetComponentInChildren<TreeListGird>().RowNum = i;
                        gov.GetComponentInChildren<TreeListGird>().ColumnNum = j;
                        gov.transform.localScale = mTL.transform.GetChild(0).GetChild(i).localScale;
                        gov.transform.localPosition = new Vector3(mTL.transform.GetChild(1).GetChild(0).transform.localPosition.x +
                        mTL.transform.GetChild(0).GetChild(0).GetComponentInChildren<BoxCollider>().size.x * i,
                            mTL.transform.GetChild(1).GetChild(0).transform.localPosition.y -
                            mTL.transform.GetChild(1).GetChild(0).GetComponentInChildren<BoxCollider>().size.y * j,
                            gov.transform.localPosition.z);
                    }
                }
            }
        }
        //处理行
        else if (oldColumn != newColumn)
        {
            //删除行
            if (oldColumn > newColumn)  
            {
                for (int i = 0; i < oldColumn - newColumn; ++i)
                {
                    var valueList = mTL.transform.GetChild(1).GetComponentsInChildren<TreeListGird>(true);

                    for (int j = 0; j < valueList.Length; ++j)
                    {
                        if (valueList[j].ColumnNum == (oldColumn - 1 - i))
                        {
                            Mogo.Util.LoggerHelper.Debug(valueList[j].name);
                            DestroyImmediate(valueList[j].gameObject);
                        }
                    }

                }
            }
            //添加行
            else if (oldColumn < newColumn)
            {
                for (int i = 0; i < newRow; ++i)
                {
                    for (int j = oldColumn; j < newColumn; ++j)
                    {
                        GameObject go = (GameObject)Instantiate(mTL.transform.GetChild(1).GetChild(0).gameObject);
                        go.transform.parent = mTL.transform.GetChild(1).transform;
                        go.name = "Value" + i + j;
                        go.GetComponentInChildren<TreeListGird>().RowNum = i;
                        go.GetComponentInChildren<TreeListGird>().ColumnNum = j;
                        go.transform.localScale = mTL.transform.GetChild(1).GetChild(0).transform.localScale;
                        go.transform.localPosition = new Vector3(mTL.transform.GetChild(1).GetChild(0).transform.localPosition.x + 
                        mTL.transform.GetChild(0).GetChild(0).transform.GetComponentInChildren<BoxCollider>().size.x * i,
                        mTL.transform.GetChild(1).GetChild(0).transform.localPosition.y - 
                        mTL.transform.GetChild(1).GetChild(0).transform.GetComponentInChildren<BoxCollider>().size.y * j,
                        mTL.transform.GetChild(1).GetChild(0).transform.localPosition.z);
                    }
                }
            }
        }        
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(80f);
        mTL = target as TreeList;

        m_oldRow = mTL.Row;
        m_oldColumn = mTL.Column;

        int row = EditorGUILayout.IntField("Row", mTL.Row, GUILayout.Width(110f));
        if (row != mTL.Row)
        {
            if (row < 1)
                row = 1;

            NGUIEditorTools.RegisterUndo("TreeList Row Change", mTL);            
            mTL.Row = row;
        }

        int column = EditorGUILayout.IntField("Column", mTL.Column, GUILayout.Width(110f));
        if (column != mTL.Column)
        {
            if (column < 1)
                column = 1;

            NGUIEditorTools.RegisterUndo("TreeList Column Change", mTL);
            mTL.Column = column;
        }

        m_newRow = row;
        m_newColumn = column;

        if (m_newColumn == m_oldColumn && m_newRow == m_oldRow)
        {
            return;
        }
        else
        {  
            ReDraw(m_oldRow, m_newRow, m_oldColumn, m_newColumn);
        }
    }
}