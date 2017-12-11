using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DialogUIButton : MonoBehaviour
{
    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_DialogUI;
    }
    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            EventDispatcher.TriggerEvent("DialogUI_DownSignUp");
        }
    }

    public void FakePress(bool isPressed)
    {
        EventDispatcher.TriggerEvent("DialogUI_DownSignUp");
    }
}
