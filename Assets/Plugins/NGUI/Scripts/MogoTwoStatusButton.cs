using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoTwoStatusButton : MonoBehaviour
{
    Transform m_transform;
    public GameObject m_bgDown;
    public GameObject m_bgUp;
    private string m_bgUpSpriteName = null;
    UILabel m_lblText;
    private bool m_clickable = true;
    public bool IsTab = false;
    public bool IsSmart = false;
    public Vector3 Vec3Offest = new Vector3(0, 2, 0);
    public Vector3 Vec3DownScale = new Vector3(1, 1, 1);

    public bool Clickable
    {
        get { return m_clickable; }
        set
        {
            m_clickable = value;
            if (value)
            {
                if (string.IsNullOrEmpty(m_bgUpSpriteName))
                    m_bgUp.transform.GetComponentsInChildren<UISprite>(true)[0].spriteName = "btn_03up";
                else
                    m_bgUp.transform.GetComponentsInChildren<UISprite>(true)[0].spriteName = m_bgUpSpriteName;
            }
            else
            {
                m_bgUp.transform.GetComponentsInChildren<UISprite>(true)[0].spriteName = "btn_03grey";
            }
        }
    }

    void Awake()
    {
        m_transform = transform;
        var ssList = m_transform.GetComponentsInChildren<UISprite>(true);
        if (m_bgUp == null)
        {
            m_bgUp = ssList[0].gameObject;
        }
        if (m_bgUp != null)
        {
            m_bgUpSpriteName = m_bgUp.transform.GetComponentsInChildren<UISprite>(true)[0].spriteName;
        }
        if (m_bgDown == null)
        {
            m_bgDown = ssList[1].gameObject;
        }
        m_lblText = m_transform.GetComponentInChildren<UILabel>();
        if (IsSmart)
        {
            m_transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            BoxCollider bc = m_transform.GetComponent<BoxCollider>();
            bc.center = Vector3.zero;
            m_bgUp.transform.localScale = new Vector3(bc.size.x, bc.size.y, 1.0f);
            m_bgDown.transform.localScale = new Vector3(bc.size.x, bc.size.y, 1.0f);
        }
    }

    void OnPress(bool isPressed)
    {
        if (Clickable)
        {
            if (isPressed)
            {
                m_bgDown.SetActive(true);
                EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, gameObject.name);

                if (Vec3Offest != Vector3.zero)
                {
                    m_bgDown.transform.parent.localPosition -= Vec3Offest;
                }
                if (Vec3DownScale != Vector3.one)
                {
                    transform.localScale = new Vector3(transform.localScale.x * Vec3DownScale.x,
                        transform.localScale.y * Vec3DownScale.y, transform.localScale.z * Vec3DownScale.z);
                }
                
            }
            else
            {
                if (!IsTab)
                {
                    m_bgDown.SetActive(false);

                    if (Vec3Offest != Vector3.zero)
                    {
                        m_bgDown.transform.parent.localPosition += Vec3Offest;
                    }

                    if (Vec3DownScale != Vector3.one)
                    {
                        transform.localScale = new Vector3(transform.localScale.x / Vec3DownScale.x,
                            transform.localScale.y / Vec3DownScale.y, transform.localScale.z / Vec3DownScale.z);
                    }
                    EventDispatcher.TriggerEvent(SettingEvent.UIUpPlaySound, gameObject.name);
                }
            }
        }

    }


    public void SetButtonDown(bool down)
    {
        m_bgDown.SetActive(down);
    }
    public void SetButtonText(string text)
    {
        if (m_lblText)
            m_lblText.text = text;
    }


    public void SetButtonEnable(bool isEnable)
    {
        if (isEnable)
        {
            if (string.IsNullOrEmpty(m_bgUpSpriteName))
                m_bgUp.transform.GetComponentsInChildren<UISprite>(true)[0].spriteName = "btn_03up";
            else
                m_bgUp.transform.GetComponentsInChildren<UISprite>(true)[0].spriteName = m_bgUpSpriteName;
            GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            m_bgUp.GetComponentsInChildren<UISprite>(true)[0].spriteName = "btn_03grey";
            GetComponent<BoxCollider>().enabled = false;
        }

    }
};
