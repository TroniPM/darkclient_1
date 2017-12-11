// ģ����   :  ControlStick
// ������   :  Ī׿��
// �������� :  2012-1-18
// ��    �� :  ҡ��

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ControlStick : MonoBehaviour
{
    public Camera RelatedCamera;
    public static ControlStick instance;
    //public Texture2D textureBg;
    //public Texture2D textureStick;
    //public float radiusBg = 0.08f;
    //public float radiusStick = 0.03f;
    public float rangeStick = 0.08f;//stick�����ƶ���Χ�뾶
    //public Vector2 centerBg = new Vector2(0.25f, 0.25f);
    //public Vector2 centerStick = new Vector2(0.25f, 0.25f);


    float actualRadiusBg;
    float actualRadiusStick;

    Rect rectCanTouch;
    float actualRangeStick;
    Vector2 actualCenterBg;
    Vector2 actualCenterStick;
    Rect actualRectBg;
    Rect actualRectStick;
    public float canMoveRange = 0.1f;
    bool m_isDraging = false;

    public bool isDraging
    {
        get
        {
            return m_isDraging;
        }
        set
        {
            m_isDraging = value;
        }
    }

    public bool IsDraging
    {
        get
        {
            return isDraging && (strength > canMoveRange);
        }
    }

    public bool IsTuring
    {
        get
        {
            return isDraging && (strength <= canMoveRange);
        }
    }

    public Vector2 direction = Vector2.zero;
    public float strength = 0f;
    int fingerId = -100;

    bool isFirstLanuch = true;

    int screenWidth = Screen.width;
    int screenHeight = Screen.height;
    Transform controllerButton;
    Transform controllerBg;
    Transform bgOriginalPos;

    void OnEnable()
    {        
        instance = this;
    }

    void Awake()
    {
        //ȷ���ֻ��豸�ϻ�ȡ��ȷ����Ļ�ֱ���
        Invoke("InitSize", 0.1f);
        controllerButton = transform.Find("ControllerButton");
        controllerBg = transform.Find("ControllerBG");
        bgOriginalPos = transform.Find("ControllerBGOriginalPos");
        controllerBg.gameObject.SetActive(true);
    }

    void Start()
    {
        //controllerBg.gameObject.SetActive(false);
    }
    void InitSize()
    {
        if (!(screenWidth != Screen.width || screenHeight != Screen.height || isFirstLanuch))
        {
            return;
        }

        screenWidth = Screen.width;
        screenHeight = Screen.height;

        //BoxCollider c = GetComponent<BoxCollider>();
        //c.center = new Vector3(screenWidth / 4, screenHeight / 2, 10);
        //c.size = new Vector3(screenWidth, screenHeight*2, 1);

        //�ٷֱ�ת����ʵ������
        //actualCenterBg.x = centerBg.x * Screen.width;
        //actualCenterBg.y = centerBg.y * Screen.height;
        //actualCenterStick.x = centerStick.x * Screen.width;
        //actualCenterStick.y = centerStick.y * Screen.height;

        actualCenterBg.x = controllerBg.localPosition.x * Screen.width / RelatedCamera.orthographicSize;
        actualCenterBg.y = controllerBg.localPosition.y * Screen.height / (RelatedCamera.orthographicSize / RelatedCamera.aspect);

        float size = Mathf.Max(Screen.width, Screen.height);
        //actualRadiusBg = radiusBg * size;
        //actualRadiusStick = radiusStick * size;
        actualRangeStick = rangeStick * size;

        //����ԭ��ת��Ϊ���½�,��Ϊҡ��ͨ�����������½�
        actualCenterBg = new Vector2(actualCenterBg.x, Screen.height - actualCenterBg.y);
        actualCenterStick = new Vector2(actualCenterStick.x, Screen.height - actualCenterStick.y);

        actualRectBg = new Rect(actualCenterBg.x - actualRadiusBg, actualCenterBg.y - actualRadiusBg,
                                                 actualRadiusBg * 2, actualRadiusBg * 2);

        actualRectStick = new Rect(actualCenterStick.x - actualRadiusStick, actualCenterStick.y - actualRadiusStick,
                                              actualRadiusStick * 2, actualRadiusStick * 2);

        BoxCollider box = GetComponent<BoxCollider>();
        float aspect = Screen.height / 720;
        rectCanTouch = new Rect(0, Screen.height - box.size.y * aspect, box.size.x * aspect, box.size.y * aspect);
        isFirstLanuch = false;
        isDraging = false;
    }

    // Update is called once per frame
    void Update()
    {
        //direction = Vector2.zero;
        //strength = 0;
        if (isDraging)
        {

            Vector2 touchPosition = GetTouchPosition();
            //MogoMsgBox.Instance.ShowFloatingText(touchPosition + "");
            ChangeStickPositon(touchPosition);

            direction = (actualCenterStick - actualCenterBg);
            strength = direction.magnitude / actualRangeStick;
            direction = direction.normalized;

            controllerButton.localPosition = new Vector3((actualRectStick.xMax + actualRectStick.xMin) * 0.5f *
                RelatedCamera.orthographicSize / Screen.width, (Screen.height - (actualRectStick.yMin + actualRectStick.yMax) * 0.5f) *
                    (RelatedCamera.orthographicSize / RelatedCamera.aspect) / Screen.height, 0);


            controllerBg.localPosition = new Vector3((actualRectBg.xMax + actualRectBg.xMin) * 0.5f *
                RelatedCamera.orthographicSize / Screen.width, (Screen.height - (actualRectBg.yMin + actualRectBg.yMax) * 0.5f) *
                    (RelatedCamera.orthographicSize / RelatedCamera.aspect) / Screen.height, 0);
        }

    }

    private void TouchBegin(Vector2 touchPosition)
    {
        isDraging = true;

        actualRectBg.center = touchPosition;
        actualCenterBg = touchPosition;

        actualRectStick.center = touchPosition;
        actualCenterStick = touchPosition;

        controllerButton.gameObject.SetActive(true);
        controllerBg.gameObject.SetActive(true);
    }

    private void ChangeStickPositon(Vector2 touchPosition)
    {
        Vector2 v = touchPosition - actualRectBg.center;
        if (v.magnitude > actualRangeStick)
        {
            v = v.normalized;
            v = v * actualRangeStick;
            v = actualRectBg.center + v;
            actualRectStick.center = v;
            actualCenterStick = v;
        }
        else
        {
            actualRectStick.center = touchPosition;
            actualCenterStick = touchPosition;
        }
    }

    private Vector2 GetTouchPosition(bool touchBegin = false)
    {
        Vector2 touchPosition = Vector2.zero;
        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
            //{
            //    for (int i = 0; i < Input.touchCount; i++)
            //    {
            //        Touch touch = Input.GetTouch(i);
            //        Vector2 temp = new Vector2(touch.position.x, touch.position.y);
            //        if (touchBegin)
            //        {

            //            //if (actualRectBg.Contains(temp))
            //            //{
            //            touchPosition = temp;
            //            fingerId = touch.fingerId;
            //            //}
            //        }
            //        else
            //        {
            //            if (touch.fingerId == fingerId)
            //            {
            //                touchPosition = temp;
            //            }
            //        }
            //    }

            //    break;
            //}
            case RuntimePlatform.Android:
                {
                    if (touchBegin)
                    {
                        for (int i = 0; i < Input.touchCount; i++)
                        {
                            Touch touch = Input.GetTouch(i);

                            if (touch.phase == TouchPhase.Began)
                            {
                                //ת�����Ͻ�
                                Vector2 temp = new Vector2(touch.position.x, screenHeight - touch.position.y);
                                if (rectCanTouch.Contains(temp))
                                {
                                    touchPosition = temp;
                                    fingerId = touch.fingerId;
                                    break;
                                }

                            }
                        }

                    }
                    else
                    {
                        for (int i = 0; i < Input.touchCount; i++)
                        {
                            Touch touch = Input.GetTouch(i);

                            if (touch.fingerId == fingerId)
                            {
                                Vector2 temp = new Vector2(touch.position.x, screenHeight - touch.position.y);
                                touchPosition = temp;
                                break;
                            }
                        }
                    }

                    break;
                }
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                {
                    touchPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    touchPosition.y = Screen.height - touchPosition.y;
                    break;
                }
            default:
                break;
        }
        return touchPosition;
    }

    public void Reset()
    {
        actualRectStick.center = actualRectBg.center;
        actualCenterStick = actualRectBg.center;
        isDraging = false;
        direction = Vector2.zero;

        fingerId = -100;
        strength = 0;
        controllerButton.gameObject.SetActive(false);
        controllerBg.position = bgOriginalPos.position;
        //controllerBg.gameObject.SetActive(false);
    }

    public void InitInstance()
    {
        instance = this;
    }

    void OnPress(bool isPressed)
    {
        //Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ isPressed = " + isPressed);
        if (isPressed)
        {
            Vector2 touchPosition = GetTouchPosition(true);
            TouchBegin(touchPosition);

            EventDispatcher.TriggerEvent("MainUIControllStickPressed");
        }
        else
        {
            Reset();
        }
    }

    public void FakePress()
    {
        Vector2 touchPosition = GetTouchPosition(true);
        TouchBegin(touchPosition);
    }
}