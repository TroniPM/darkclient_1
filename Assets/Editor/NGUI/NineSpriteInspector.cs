using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NineSprite))]
public class NineSpriteInspector : Editor
{
    NineSprite mNS;
    private UISprite LT;
    private UISprite MT;
    private UISprite RT;
    private UISprite LM;
    private UISprite MM;
    private UISprite RM;
    private UISprite LB;
    private UISprite MB;
    private UISprite RB;

    private void ReDraw()
    {
        mNS = target as NineSprite;

        LT = mNS.transform.Find("LT").GetComponent<UISprite>() as UISprite;
        MT = mNS.transform.Find("MT").GetComponent<UISprite>() as UISprite;
        RT = mNS.transform.Find("RT").GetComponent<UISprite>() as UISprite;
        LM = mNS.transform.Find("LM").GetComponent<UISprite>() as UISprite;
        MM = mNS.transform.Find("MM").GetComponent<UISprite>() as UISprite;
        RM = mNS.transform.Find("RM").GetComponent<UISprite>() as UISprite;
        LB = mNS.transform.Find("LB").GetComponent<UISprite>() as UISprite;
        MB = mNS.transform.Find("MB").GetComponent<UISprite>() as UISprite;
        RB = mNS.transform.Find("RB").GetComponent<UISprite>() as UISprite;

        LB.transform.localPosition = new Vector3(0, 0, 0);
        RB.transform.localPosition = new Vector3(mNS.Width - RB.transform.localScale.x, 0, 0);
        LT.transform.localPosition = new Vector3(0, mNS.Height - LT.transform.localScale.y, 0);
        RT.transform.localPosition = new Vector3(mNS.Width - RT.transform.localScale.x,
            mNS.Height - RT.transform.localScale.y, 0);
        LM.transform.localPosition = new Vector3(0, LB.transform.localScale.y, 0);
        RM.transform.localPosition = new Vector3(mNS.Width - RM.transform.localScale.x,
            RB.transform.localScale.y, 0);
        MT.transform.localPosition = new Vector3(LT.transform.localScale.x,
            mNS.Height - MT.transform.localScale.y, 0);
        MB.transform.localPosition = new Vector3(LB.transform.localScale.x, 0, 0);
        MM.transform.localPosition = new Vector3(LM.transform.localScale.x,
            LB.transform.localScale.y, 0);

        MB.transform.localScale = new Vector3(RB.transform.localPosition.x - LB.cachedTransform.localScale.x,
            MB.transform.localScale.y, MB.transform.localScale.z);
        MT.transform.localScale = new Vector3(RT.transform.localPosition.x - LT.cachedTransform.localScale.x,
            MT.transform.localScale.y, MT.transform.localScale.z);

        LM.transform.localScale = new Vector3(LM.transform.localScale.x,
            LT.transform.localPosition.y - LB.cachedTransform.localScale.y, LM.transform.localScale.z);
        RM.transform.localScale = new Vector3(RM.transform.localScale.x,
            RT.transform.localPosition.y - RB.cachedTransform.localScale.y, RM.transform.localScale.z);

        MM.transform.localScale = new Vector3(RB.transform.localPosition.x - LB.cachedTransform.localScale.x,
            LT.transform.localPosition.y - LB.cachedTransform.localScale.y, MM.transform.localScale.z);
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(80f);
        mNS = target as NineSprite;

        int width = EditorGUILayout.IntField("Width", mNS.Width, GUILayout.Width(110f));
        if (width != mNS.Width)
        { 
            NGUIEditorTools.RegisterUndo("NineSprite Width Change", mNS);
            mNS.Width = width;
            ReDraw();
        }

        int height = EditorGUILayout.IntField("Height", mNS.Height, GUILayout.Width(110f));
        if (height != mNS.Height)
        {
            NGUIEditorTools.RegisterUndo("NineSprite Height Change", mNS);
            mNS.Height = height;
            ReDraw();
        }

    }
}