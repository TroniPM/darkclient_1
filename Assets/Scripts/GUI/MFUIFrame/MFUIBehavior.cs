using UnityEngine;
using System.Collections;

public class MFUIBehavior : MonoBehaviour 
{
    protected GameObject m_myGameObject;
    protected Transform m_myTransform;

    public virtual void MFUIAwake()
    {
 
    }

    public virtual void MFUIStart()
    { }

    public virtual void MFUIUpdate()
    { }


    public virtual void MFUIEnable()
    { }

    public virtual void MFUIDisable()
    { }

    public virtual void MFUIDestroy()
    { }

    public virtual void MFUIPress(bool isPressed)
    { }

    public virtual void MFUIClick()
    { }

    public virtual void MFUIDrag(Vector2 dir)
    { }

    void Awake()
    {
        m_myGameObject = gameObject;
        m_myTransform = transform;

        MFUIAwake();
    }

    void Start()
    {
        MFUIStart();
    }

    void Update()
    {
        MFUIUpdate();
    }

    void OnEnable()
    {
        MFUIEnable();
    }

    void OnDisable()
    {
        MFUIDisable();
    }

    void OnDestroy()
    {
        MFUIDestroy();
    }

    void OnPress(bool isPressed)
    {
        MFUIPress(isPressed);
    }

    void OnClick()
    {
        MFUIClick();
    }

    void OnDrag(Vector2 dir)
    {
        MFUIDrag(dir); 
    }
}
