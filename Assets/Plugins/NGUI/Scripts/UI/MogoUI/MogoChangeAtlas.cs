
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class MogoChangeAtlas : MonoBehaviour {

    public UIAtlas at;

    void ChangeWidgetAtlas(GameObject go)
    {
        if (go.transform.GetComponentsInChildren<UISprite>(true).Length > 0)
        {
            go.transform.GetComponentsInChildren<UISprite>(true)[0].atlas = at;

        }

        if (go.transform.GetComponentsInChildren<UISlicedSprite>(true).Length > 0)
        {
            go.transform.GetComponentsInChildren<UISlicedSprite>(true)[0].atlas = at;
        }

        if (go.transform.GetComponentsInChildren<UIFilledSprite>(true).Length > 0)
        {
            go.transform.GetComponentsInChildren<UIFilledSprite>(true)[0].atlas = at;
        }
    }

    void ChangeAllWidget(Transform rootTrans)
    {
        for (int i = 0; i < rootTrans.GetChildCount(); ++i)
        {
            ChangeWidgetAtlas(rootTrans.GetChild(i).gameObject);
            ChangeAllWidget(rootTrans.GetChild(i));
        }
    }

    void Start()
    {
        ChangeAllWidget(transform);
    }
}
