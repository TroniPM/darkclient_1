using UnityEngine;
using System.Collections;

public class MogoDebugWidget : MonoBehaviour {

    bool beginCountdown = false;
    float lastTime = 0f;

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            beginCountdown = true;
        }
        else
        {
            beginCountdown = false;
            lastTime = 0f;
        }
    }

    void Update()
    {
        if (beginCountdown)
        {
            lastTime += Time.deltaTime;

            if (lastTime >= 30)
            {
                beginCountdown = false;
                lastTime = 0f;

                MogoUIManager.Instance.ShowMogoCommuntiyUI(CommunityUIParent.MainUI, true);
            }
        }
    }
}
