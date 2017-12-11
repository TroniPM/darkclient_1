using UnityEngine;
using System.Collections;

public class PassCountDownUIViewManager : MFUIUnit
{
    private static PassCountDownUIViewManager m_instance;

    public static PassCountDownUIViewManager Instance
    {
        get
        {
            return PassCountDownUIViewManager.m_instance;
        }
    }
 
    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.PassCountDownUI;
        m_myGameObject.name = "PassCountDownUI";
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    public override void CallWhenUpdate()
    {
        if (startCountDown == false)
            return;

        deltaTime += Time.deltaTime;

        if (deltaTime >= 1f)
        {
            deltaTime = 0f;

            SetCountDownNum(--countDown);

            if (countDown <= 0)
            {
                startCountDown = false;
                countDown = 5;

                if (OnCountDownFinished != null)
                {
                    OnCountDownFinished();
                }
            }
        }
    }

    public void SetCountDownNum(int num)
    {
        SetLabelText("PassCountDownUILeftTimeNum", num.ToString());
    }

    public void StartCountDown()
    {
        startCountDown = true;
    }


    float deltaTime = 0f;
    int countDown = 5;
    bool startCountDown = false;

    public System.Action OnCountDownFinished;
}
