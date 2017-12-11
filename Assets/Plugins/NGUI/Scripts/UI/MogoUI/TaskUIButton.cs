using UnityEngine;
using System.Collections;
using Mogo.Util;

public class TaskUIButton : MonoBehaviour
{
    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_TaskUI;
    }
    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            EventDispatcher.TriggerEvent("TaskUI_DownSignUp");
        }
    }

    public void FakePress(bool isPressed)
    {
        EventDispatcher.TriggerEvent("TaskUI_DownSignUp");
    }
}
