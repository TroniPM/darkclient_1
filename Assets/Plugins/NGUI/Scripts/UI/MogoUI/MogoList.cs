using System;
using System.Collections.Generic;
using UnityEngine;


public class MogoList : MonoBehaviour
{
    public float width;
    public float height;
    public float stageSize;
    public int cameraDepth;
    public Action<int> turnPage;

    private Vector3 preDrag;
    private GameObject c;
    private BoxCollider bc;

    private bool tweenAble = false;
    private float tweenSpeed = 0.1f;
    private float tweenTimes = 10.0f; //tween步数
    private Vector3 tweenEnd;
    private float stageHeight = 720.0f;
    private Camera m_myCamera = null;
    public Camera MogoListCamera
    {
        get
        {
            return m_myCamera;
        }
    }

    void Awake()
    {

    }

    void Start()
    {
        c = new GameObject();
        c.transform.parent = transform;
        c.transform.localScale = new Vector3(1, 1, 1);
        bc = gameObject.AddComponent<BoxCollider>();
        bc.size = new Vector3(width, height, 0);
        bc.isTrigger = true;
        MogoButton cameraBtn = gameObject.AddComponent<MogoButton>();
        cameraBtn.pressHandler = PressHandler;
        cameraBtn.dragHandler = DragHandler;

        m_myCamera = c.AddComponent<Camera>();
        UICamera uiCamera = c.AddComponent<UICamera>();
        c.AddComponent<TweenPosition>();

        m_myCamera.clearFlags = CameraClearFlags.Depth;
        m_myCamera.cullingMask = 1 << gameObject.layer;
        m_myCamera.orthographic = true;
        m_myCamera.orthographicSize = stageSize;
        m_myCamera.nearClipPlane = -999;
        m_myCamera.farClipPlane = 999;
        float dx = width / stageSize;
        float w = Screen.width;
        float h = Screen.height;
        float r = w / h;
        float ro = stageSize / stageHeight;
        if (r > ro)
        {
            float wn = width * (h / stageHeight);
            dx = wn / w;
            m_myCamera.rect = new Rect((1 - dx) / 2, 0, dx, 1);
        }
        else
        {
            m_myCamera.rect = new Rect((1 - dx) / 2, 0, dx, 1);
        }
        m_myCamera.depth = cameraDepth;

        uiCamera.eventReceiverMask = 1 << gameObject.layer;

        Mogo.Util.TimerHeap.AddTimer<Vector3>(500, 0, ResetPos, transform.localPosition);
    }

    void Update()
    {
        if (m_myCamera != null)
        {
            float dx = width / stageSize;
            float w = Screen.width;
            float h = Screen.height;
            float r = w / h;
            float ro = stageSize / stageHeight;
            if (r > ro)
            {
                float wn = width * (h / stageHeight);
                dx = wn / w;
                m_myCamera.rect = new Rect((1 - dx) / 2, 0, dx, 1);
            }
            else
            {
                m_myCamera.rect = new Rect((1 - dx) / 2, 0, dx, 1);
            }
        }
        if (tweenAble)
        {
            c.transform.localPosition = c.transform.localPosition + new Vector3(tweenSpeed, 0, 0);
            if (Math.Abs(c.transform.localPosition.x - tweenEnd.x) <= Math.Abs(tweenSpeed))
            {
                c.transform.localPosition = new Vector3(tweenEnd.x, c.transform.localPosition.y, 0);
                bc.center = new Vector3(tweenEnd.x, 0, 0);
                tweenAble = false;
            }
        }
    }

    private void ResetPos(Vector3 p)
    {
        c.transform.localPosition = Vector3.zero;

        transform.localPosition = new Vector3(100000, 100000, 0);
    }

    public void TweenTo(int page)
    {
        float dis = page * width;
        tweenEnd = new Vector3(dis, 0, 0);
        tweenSpeed = (dis - c.transform.localPosition.x) / tweenTimes;
        tweenAble = true;
    }

    public void FastTweenTo(int page)
    {
        if (c == null) return;
        float dis = page * width;
        tweenEnd = new Vector3(dis, 0, 0);
        tweenSpeed = (dis - c.transform.localPosition.x) / 1;
        tweenAble = true;
    }

    #region 事件

    // 外部监听
    public void PressHandlerOutSide(bool pressed)
    {
        PressHandler(pressed);
    }

    // 外部监听
    public void DragHandlerOutSide(Vector2 delta)
    {
        DragHandler(delta);
    }

    // 内部监听
    private void PressHandler(bool pressed)
    {
        if (pressed)
        {
            preDrag = c.transform.localPosition;
            Mogo.Util.EventDispatcher.TriggerEvent(MarketEvent.LigthArrow, true);
        }
        else
        {
            Mogo.Util.EventDispatcher.TriggerEvent(MarketEvent.LigthArrow, false);
            if (preDrag.x > c.transform.localPosition.x)
            {//左翻页
                turnPage(-1);
            }
            else if (preDrag.x < c.transform.localPosition.x)
            {//右翻页
                turnPage(1);
            }
        }
    }

    // 内部监听
    private void DragHandler(Vector2 delta)
    {
        c.transform.localPosition = c.transform.localPosition - new Vector3(delta.x, 0, 0);
    }

    #endregion
}
