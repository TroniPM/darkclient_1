using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PropSheet))]
public class PropSheetInspector : Editor
{
    PropSheet mPS;

    void ReDraw(int oldNum,int newNum)
    {
        if (oldNum > newNum)
        {
            for (int i = 0; i < oldNum - newNum; ++i)
            {
                var iconList = mPS.transform.GetChild(0).GetComponentsInChildren<UISprite>(true);
                var dialogList = mPS.transform.GetChild(1).GetComponentsInChildren<UIPanel>(true);
                
                DestroyImmediate(iconList[iconList.Length - (oldNum - 1 - i)].gameObject);
                DestroyImmediate(dialogList[dialogList.Length - (oldNum - 1 - i)].gameObject);

                
            }
        }
        else if (oldNum < newNum)
        {
            for (int i = oldNum;i < newNum;++i)
            {
                UISprite spIcon = Instantiate(mPS.transform.GetChild(0).GetComponentInChildren<UISprite>()) as UISprite;
                spIcon.transform.parent = mPS.transform.GetChild(0).transform;
                spIcon.MakePixelPerfect();
                spIcon.name = "Icon" + i;
                spIcon.pivot = UIWidget.Pivot.BottomLeft;
                spIcon.atlas = NGUISettings.atlas;
                //spIcon.GetComponentInChildren<PropPageIcon>().Id = i;


                //UISprite spIcon0 = mPS.transform.GetChild(0).GetComponentInChildren<UISprite>();
                spIcon.transform.localPosition = new Vector3(spIcon.transform.position.x + i * spIcon.transform.localScale.x,
                    spIcon.transform.position.y, spIcon.transform.position.z);

                //UISprite spDialog = Instantiate(mPS.transform.GetChild(1).GetComponentInChildren<UISprite>()) as UISprite;
                //spDialog.transform.parent = mPS.transform.GetChild(1).transform;
                //spDialog.MakePixelPerfect();
                //spDialog.name = "Dialog" + i;
                //spDialog.pivot = UIWidget.Pivot.BottomLeft;
                //spDialog.atlas = NGUISettings.atlas;
                //spDialog.gameObject.SetActive(false);

                //spDialog.transform.localPosition = new Vector3(0, 0, 0);

                GameObject goDialog = (GameObject)Instantiate(mPS.transform.GetChild(1).GetChild(1).gameObject);
                goDialog.transform.parent = mPS.transform.GetChild(1).transform;
                goDialog.name = "Dialog" + i;
                goDialog.SetActive(false);

                goDialog.transform.localPosition = new Vector3(0, 0, 0);
                goDialog.transform.localScale = mPS.transform.GetChild(1).GetChild(1).localScale;
                
            }
        }

        var dialogList0 = mPS.transform.GetChild(1).GetComponentsInChildren<UIPanel>();

        if (dialogList0.Length < 2)
        {
            mPS.transform.GetChild(1).Find("Dialog0").gameObject.SetActive(true);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(80f);
        mPS = target as PropSheet;

        int width = EditorGUILayout.IntField("Width", mPS.Width, GUILayout.Width(110f));
        if (width != mPS.Width)
        {
            NGUIEditorTools.RegisterUndo("PropSheet Width Change", mPS);
            mPS.Width = width;
        }

        int height = EditorGUILayout.IntField("Height", mPS.Height, GUILayout.Width(110f));
        if (height != mPS.Height)
        {
            NGUIEditorTools.RegisterUndo("PropSheet Height Change", mPS);
            mPS.Height = height;
        }

        int pageNum = EditorGUILayout.IntField("PageNum", mPS.PageNum, GUILayout.Width(110f));
        if (pageNum != mPS.PageNum)
        {
            NGUIEditorTools.RegisterUndo("PropSheet PageNum Change", mPS);
            
            if (pageNum < 1)
                pageNum = 1;

            ReDraw(mPS.PageNum, pageNum);
            mPS.PageNum = pageNum;
            
        }

        
    }
}