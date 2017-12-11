using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class TeachUIViewManager : MonoBehaviour
{
    private static TeachUIViewManager m_instance;

    public static TeachUIViewManager Instance
    {
        get
        {
            return TeachUIViewManager.m_instance;
        }
    }

    private Transform m_myTransform;

    GameObject m_goPanel;
    Camera m_camTeachUI;
    Camera m_camMainUI;

    GameObject m_goFocusWidget;
    GameObject m_oriWidget;
    GameObject m_goMaskBG;

    GameObject m_goFingerAnim;

    GameObject m_goTipTR;
    GameObject m_goTipTL;
    GameObject m_goTipBR;
    GameObject m_goTipBL;

    GameObject m_goTip;

    GameObject m_goLeftArr;
    GameObject m_goRightArr;

    GameObject m_goTeachUICamera;

    UILabel m_lblTipText;

    float m_fLastTime = 0f;
    float m_fAutoLastTime = 0f;
    bool m_bBeginCount = false;
    bool m_bStartCount = false;

    bool m_bIsMaskClickable = false;

    void Awake()
    {
        m_myTransform = transform;

        m_instance = m_myTransform.GetComponentsInChildren<TeachUIViewManager>(true)[0];

        m_goPanel = gameObject;
        m_camTeachUI = m_myTransform.parent.parent.GetComponentsInChildren<Camera>(true)[0];

        m_goMaskBG = m_myTransform.Find("TeachUIMaskBG").gameObject;
        m_goFingerAnim = m_myTransform.Find("TeachUIFingerAnim").gameObject;

        m_goTip = m_myTransform.Find("TeachUITip").gameObject;
        m_goTipBL = m_myTransform.Find("TeachUIFingerAnim/TeachUITipBL").gameObject;
        m_goTipBR = m_myTransform.Find("TeachUIFingerAnim/TeachUITipBR").gameObject;
        m_goTipTL = m_myTransform.Find("TeachUIFingerAnim/TeachUITipTL").gameObject;
        m_goTipTR = m_myTransform.Find("TeachUIFingerAnim/TeachUITipTR").gameObject;

        m_lblTipText = m_goTip.transform.Find("TeachUITipText").GetComponentsInChildren<UILabel>(true)[0];

        m_goLeftArr = m_myTransform.Find("TeachUITip/TeachUITipArrLeft").gameObject;
        m_goRightArr = m_myTransform.Find("TeachUITip/TeachUITipArrRight").gameObject;

        //m_goTeachUICamera = m_myTransform.parent.parent.gameObject;

        Initialize();
    }

    void OnWaitingWidgetFinished()
    {
        Mogo.Util.LoggerHelper.Debug("WaitingFinishend" + MogoUIManager.Instance.WaitingWidgetName);
        SetFocus(MogoUIManager.Instance.WaitingWidgetName, m_tipText, m_bIsMaskClickable, m_bIsShowTip, m_bIsShowMask);
        MogoUIManager.Instance.WaitingWidgetName = "";
    }

    void Initialize()
    {
        EventDispatcher.AddEventListener("TeachUIFocusDown", OnTeachUIFocusDown);

        EventDispatcher.AddEventListener("WaitingWidgetFinished", OnWaitingWidgetFinished);

        m_goMaskBG.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnMaskBtnUp;

        TeachUILogicManager.Instance.Initialize();
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener("TeachUIFocusDown", OnTeachUIFocusDown);
        EventDispatcher.RemoveEventListener("WaitingWidgetFinished", OnWaitingWidgetFinished);
        m_goMaskBG.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnMaskBtnUp;
        TeachUILogicManager.Instance.Release();
    }

    void Start()
    {


        m_camMainUI = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_myTransform.parent.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_camMainUI;
        ShowTeachUI(false);
    }

    void OnTeachUIFocusDown()
    {
       // Debug.LogError("OnTeachUIFocusDown");
        if (m_oriWidget != null)
        {
            Mogo.Util.LoggerHelper.Debug(m_oriWidget.name);
            if (m_oriWidget.GetComponentsInChildren<MogoButton>(true).Length > 0)
            {
                m_oriWidget.GetComponentsInChildren<MogoButton>(true)[0].FakeClick();
            }

            if (m_oriWidget.GetComponentsInChildren<EquipTipBtn>(true).Length > 0)
            {
                m_oriWidget.GetComponentsInChildren<EquipTipBtn>(true)[0].FakeClick();
            }

            if (m_oriWidget.GetComponentsInChildren<ChallengeUIGrid>(true).Length > 0)
            {
                m_oriWidget.GetComponentsInChildren<ChallengeUIGrid>(true)[0].FakeIt();
            }

            if (m_oriWidget.GetComponentsInChildren<InsetUIEquipmentGrid>(true).Length > 0)
            {
                m_oriWidget.GetComponentsInChildren<InsetUIEquipmentGrid>(true)[0].FakeClick();
            }

            if (m_oriWidget.GetComponentsInChildren<ControlStick>(true).Length > 0)
            {
                m_oriWidget.GetComponentsInChildren<ControlStick>(true)[0].FakePress();

                MogoFXManager.Instance.ReleaseParticleAnim("ControllerFX");

                m_goFocusWidget.GetComponent<TeachUIFocusWidget>().OriWidget = m_oriWidget;

                ShowTeachUI(false);
                ShowFingerAnim(false);
                if (m_goFocusWidget != null)
                {
                    GameProcManager.GuideUI(m_goFocusWidget.name, MogoWorld.thePlayer.CurMissionID);
                }
                ShowTip(Vector3.zero, "", false);

                return;
            }

            if (m_oriWidget.GetComponentsInChildren<PackageItemBox>(true).Length > 0)
            {
                m_oriWidget.GetComponentsInChildren<PackageItemBox>(true)[0].FakeClick();
            }

           // Debug.LogError("Destroy OnTeachUIFocusDown");
            ShowTeachUI(false);
            ShowFingerAnim(false);
            if (m_goFocusWidget != null)
            {
                GameProcManager.GuideUI(m_goFocusWidget.name, MogoWorld.thePlayer.CurMissionID);
            }

            ShowTip(Vector3.zero, "", false);

            Destroy(m_goFocusWidget);

            m_bStartCount = false;
            m_fAutoLastTime = 0f;
        }

        //m_goTeachUICamera.SetActive(false);
    }

    void OnMaskBtnUp()
    {
        //if (m_bIsMaskClickable)
        //{
        //    ShowTeachUI(false);
        //    ShowFingerAnim(false);
        //    ShowTip(Vector3.zero, "", false);
        //    EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.ButtonClick);
        //    if (m_goFocusWidget != null)
        //    {
        //        Destroy(m_goFocusWidget);
        //    }
        //}
    }


    string m_tipText;
    bool m_bIsShowTip;
    bool m_bIsShowMask;
    bool m_bCanClickMask;

    bool m_bIsTeaching = false;

    public void SetItemFocus(int itemId, string tipText, bool isShowTip = false, bool isShowMask = true, bool isCanClickMask = false)
    {
        int gridid = InventoryManager.Instance.GetIndexByItemID(itemId);
        MenuUIViewManager.Instance.SetCurrentPage(gridid / 10);

        TimerHeap.AddTimer(500, 0, () => { SetFocus("PackageItemGrid" + gridid, tipText, isCanClickMask, isShowTip, isShowMask); });
    }

    private void TruelyPrepareShowTeachUI()
    {
        Mogo.Util.LoggerHelper.Debug("Locking~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        NormalMainUIViewManager.Instance.OpenIconPlaysBottomRight();
        MogoUIQueue.Instance.IsLocking = true;
        EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.StartGuide);
        MogoUIManager.Instance.LockMainCamera(true);
        
        TimerHeap.AddTimer(3000, 0, () => { MogoUIManager.Instance.LockMainCamera(false); });
    }
    public void PrepareShowTeachUI(GameObject baseUI = null)
    {
        if (baseUI == null)
        {
            MogoUIQueue.Instance.PushOne(() => { TruelyPrepareShowTeachUI(); }, MogoUIManager.Instance.m_NormalMainUI, "PrepareShowTeachUI", 10);
        }
        else
        {
            MogoUIQueue.Instance.PushOne(() => { TruelyPrepareShowTeachUI(); }, baseUI, "PrepareShowTeachUI", 10);
        }
    }

    public void SetFocus(string widgetName, string tipText, bool isCanClickMask = false, bool isShowTip = false, bool isShowMask = true, bool isWidgetCanClick = true, bool isNeedAutoClick = false)
    {


        //MogoUIQueue.Instance.IsLocking = true;
        m_bIsTeaching = true;
        m_bIsMaskClickable = isCanClickMask;
        m_oriWidget = GameObject.Find(widgetName);

        if (m_oriWidget == null)
        {
            if (GameObject.Find(widgetName) == null)
            {
                Mogo.Util.LoggerHelper.Debug("Can not find widget " + widgetName);
                MogoUIManager.Instance.WaitingWidgetName = widgetName;
                m_tipText = tipText;
                m_bIsShowTip = isShowTip;
                m_bIsShowMask = isShowMask;
                m_bIsMaskClickable = isCanClickMask;
                m_bBeginCount = true;
                return;
            }
            else
            {
                m_oriWidget = GameObject.Find(widgetName);
                m_bBeginCount = false;
                m_fLastTime = 0;
            }
        }


        m_bBeginCount = false;
        m_fLastTime = 0;

        m_bStartCount = true;//by maifeo
        m_fAutoLastTime = 0;

        m_goFocusWidget = (GameObject)GameObject.Instantiate(m_oriWidget);

        m_goFocusWidget.name = m_oriWidget.name;

        if (m_goFocusWidget.name == "StrenthenIcon" || m_goFocusWidget.name == "ComposeIcon" || m_goFocusWidget.name == "DecomposeIcon" || m_goFocusWidget.name == "InsetIcon")
        {
            m_goFocusWidget.transform.Find(m_goFocusWidget.name + "Down").GetComponentsInChildren<UISprite>(true)[0].spriteName = "btn_down";
        }

        if (m_goFocusWidget.GetComponentsInChildren<NormalMainUIButton>(true).Length > 0)
        {
            m_goFocusWidget.GetComponentsInChildren<NormalMainUIButton>(true)[0].RelatedCam = m_camTeachUI;
        }

        if (m_goFocusWidget.GetComponentsInChildren<MenuUIButton>(true).Length > 0)
        {
            m_goFocusWidget.GetComponentsInChildren<MenuUIButton>(true)[0].RelatedCamera = m_camTeachUI;
        }

        if (m_goFocusWidget.GetComponentsInChildren<MyDragCamera>(true).Length > 0)
        {
            Destroy(m_goFocusWidget.GetComponentsInChildren<MyDragCamera>(true)[0]);
        }

        if (m_goFocusWidget != null && m_goPanel != null)
        {
            m_goFocusWidget.transform.parent = m_goPanel.transform;
        }

        Camera m_cam;

        if (m_goFocusWidget.GetComponentsInChildren<MyDragCamera>(true).Length > 0)
        {
            m_cam = m_goFocusWidget.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera;
        }
        else if (m_oriWidget.transform.parent.GetComponents<MyDragCamera>().Length > 0)
        {
            m_cam = m_oriWidget.transform.parent.GetComponents<MyDragCamera>()[0].RelatedCamera;
        }
        else if (m_oriWidget.transform.parent.parent.parent.GetComponents<MyDragCamera>().Length > 0)
        {
            m_cam = m_oriWidget.transform.parent.parent.parent.GetComponents<MyDragCamera>()[0].RelatedCamera;
        }
        else if (m_oriWidget.GetComponent<InstanceUIButton>() != null)
        {
            GameObject cam = GameObject.Find("InstanceLevelChooseUICamera");

            if (cam != null)
            {
                m_cam = cam.GetComponent<Camera>();
            }
            else
            {
                m_cam = m_camMainUI;
            }
        }
        else
        {
            m_cam = m_camMainUI;
        }

        if (m_goFocusWidget.GetComponentsInChildren<MogoSingleButton>(true).Length > 0)
        {
            m_goFocusWidget.GetComponentsInChildren<MogoSingleButton>(true)[0].ButtonListTransform =
                m_oriWidget.GetComponentsInChildren<MogoSingleButton>(true)[0].ButtonListTransform;
        }

        if (m_goFocusWidget.GetComponentsInChildren<TweenPosition>(true).Length > 0)
        {
            m_goFocusWidget.GetComponentsInChildren<TweenPosition>(true)[0].enabled = false;
        }

        if (m_oriWidget.name == "SkillIcon" || m_oriWidget.name == "PackageIcon")
        {
            m_goFocusWidget.transform.localEulerAngles = new Vector3(m_goFocusWidget.transform.localEulerAngles.x,
                m_goFocusWidget.transform.localEulerAngles.y, 90);
        }

        Vector3 pos = Vector3.zero;

        if (m_cam != null)
        {
            pos = m_cam.WorldToScreenPoint(m_oriWidget.transform.position);
        }

        if (m_camTeachUI != null)
        {
            m_goFocusWidget.transform.position = m_camTeachUI.ScreenToWorldPoint(pos);
        }

        if (m_camTeachUI != null && m_goFingerAnim != null)
        {
            if (m_oriWidget.name == "Controller")
            {
                //pos = m_cam.WorldToScreenPoint(m_oriWidget.transform.position + new Vector3(130, 130, 0));
                float offset = 200f / 1280f * Screen.width;

                m_goFingerAnim.transform.position = m_camTeachUI.ScreenToWorldPoint(pos + new Vector3(-20, 0, 0) + new Vector3(offset, offset, 0));

                //MogoFXManager.Instance.AttachUIFX(9, MogoUIManager.Instance.GetMainUICamera(), 4f, -118.5f, 0,
                //    GameObject.Find("MainUI").transform.FindChild("BottomLeft/Controller").gameObject);

                //MogoFXManager.Instance.AttachParticleAnim("fx_ui_skill_yes.prefab", "ControllerFX", GameObject.Find("MainUI").transform.FindChild(
                //    "BottomLeft/Controller/ControllerBGOriginalPos").position, MogoUIManager.Instance.GetMainUICamera(), 4, -118.5f, 0);

            }
            else
            {
                m_goFingerAnim.transform.position = m_camTeachUI.ScreenToWorldPoint(pos + new Vector3(-20, 0, 0));
            }
        }
        Vector3 center = m_goFocusWidget.GetComponentsInChildren<BoxCollider>(true)[0].center;


        m_goFocusWidget.transform.localPosition = new Vector3(m_goFocusWidget.transform.localPosition.x,
            m_goFocusWidget.transform.localPosition.y, -5);
        m_goFocusWidget.GetComponentsInChildren<BoxCollider>(true)[0].center = new Vector3(center.x, center.y, -50);

        if (m_goFocusWidget.transform.GetChildCount() > 0)
        {
            m_goFocusWidget.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (m_goFocusWidget.GetComponent<UISprite>() != null)
            {
                m_goFocusWidget.transform.localScale = new Vector3(54, 54, 1);
            }
        }

        if (m_goFocusWidget.name == "Controller")
        {
            m_goFocusWidget.AddComponent<TeachUIFocusWidget>().PressHappen = true;
        }
        else if (m_goFocusWidget.GetComponent<BoxCollider>() == null)
        {
            BoxCollider[] arr = m_goFocusWidget.GetComponentsInChildren<BoxCollider>(true);

            for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i].gameObject.GetComponent<NewInstanceGrid>() != null)
                {
                    arr[i].gameObject.AddComponent<TeachUIFocusWidget>().PressHappen = false;
                    break;
                }
            }
        }
        else
        {
            m_goFocusWidget.AddComponent<TeachUIFocusWidget>().PressHappen = false;
        }


        if (isShowTip)
        {
            ShowTip(m_goFocusWidget.transform.position, tipText, true);
        }
        else
        {
            ShowTip(Vector3.zero, "", false);
        }

        if (m_goMaskBG)
        {
            m_goMaskBG.gameObject.SetActive(isShowMask);
        }

        //m_bIsMaskClickable = false;

        TeachUILogicManager.Instance.ShowFingerAnim(true);

        //if (m_goTeachUICamera.activeSelf == false)
        //{
        //    m_goTeachUICamera.SetActive(true);
        //}

        //Debug.LogError(isWidgetCanClick);
        //if (isWidgetCanClick)
        //{
        m_goFocusWidget.GetComponentsInChildren<BoxCollider>(true)[0].enabled = isWidgetCanClick;
        //}

    }

    public void SetNoneFocus(bool isAutoClick = false)
    {
        Vector3 pos = m_camTeachUI.ViewportToWorldPoint(new Vector3(0.8f, 0.2f, 0));
        m_goFingerAnim.transform.position = pos + new Vector3(-20, 0, 0);
        m_bIsMaskClickable = true;
        m_bIsTeaching = true;

        m_bStartCount = isAutoClick;
        m_fAutoLastTime = 0;

        //if (m_goTeachUICamera.activeSelf == false)
        //{
        //    m_goTeachUICamera.SetActive(true);
        //}
    }

    public void ShowTeachUI(bool isShow)
    {
        if (m_goMaskBG != null)
        {
            m_goMaskBG.gameObject.SetActive(isShow);
        }

        //if (m_goTeachUICamera.activeSelf != isShow)
        //{
        //    m_goTeachUICamera.SetActive(isShow);
        //}
    }

    public void ShowFingerAnim(bool isShow)
    {

        if (m_goFingerAnim != null)
        {
            m_goFingerAnim.gameObject.SetActive(isShow);
        }


        //if (m_goTeachUICamera.activeSelf != isShow)
        //{
        //    m_goTeachUICamera.SetActive(isShow);
        //}
    }

    public void ShowTip(Vector3 widgetPos, string text, bool isShow)
    {
        if (m_goTip != null)
        {
            m_goTip.SetActive(isShow);
        }

        if (isShow)
        {
            m_lblTipText.text = text;

            Vector3 screenPos = m_camTeachUI.WorldToViewportPoint(widgetPos);

            if (screenPos.x > 0.5f)
            {
                if (screenPos.y > 0.5f)
                {
                    m_goTip.transform.position = m_goTipBL.transform.position;
                }
                else
                {
                    m_goTip.transform.position = m_goTipTL.transform.position;
                }

                m_goRightArr.SetActive(true);
                m_goLeftArr.SetActive(false);
            }
            else
            {
                if (screenPos.y > 0.5f)
                {
                    m_goTip.transform.position = m_goTipBR.transform.position;
                }
                else
                {
                    m_goTip.transform.position = m_goTipTR.transform.position;
                }

                m_goRightArr.SetActive(false);
                m_goLeftArr.SetActive(true);
            }
        }

        //if (m_goTeachUICamera.activeSelf != isShow)
        //{
        //    m_goTeachUICamera.SetActive(isShow);
        //}
    }


    public void DestroyCloneObject()
    {
        Destroy(m_goFocusWidget);
    }

    public void CallWhenTeachUICrashed()
    {
        MogoUIManager.Instance.WaitingWidgetName = "";
        ShowTeachUI(false);
        ShowFingerAnim(false);
        //m_goTeachUICamera.SetActive(false);

        if (m_goFocusWidget != null)
        {
            Destroy(m_goFocusWidget);
        }

        //MogoMsgBox.Instance.ShowFloatingText("TeachUI Crashed ~~(>_<)~~ ");
    }

    public void CallWhenTimeOut()
    {
        if (m_goFocusWidget == null)
            return;

        if (m_goFocusWidget.GetComponent<MogoFakeClick>() != null)
        {
            m_goFocusWidget.GetComponent<MogoFakeClick>().FakeIt();
        }
        EventDispatcher.TriggerEvent("TeachUIFocusDown");
        EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.ButtonClick);
        //MogoMsgBox.Instance.ShowFloatingText("TeachUI TimeOut O(��_��)O~");

    }

    void Update()
    {
        if (m_bBeginCount)
        {
            m_fLastTime += Time.deltaTime;

            if (m_fLastTime > 5)
            {
                EventDispatcher.TriggerEvent(TeachUILogicManager.TEACHUICRASHED);
                CallWhenTeachUICrashed();
                m_bBeginCount = false;
                m_fLastTime = 0f;
                MogoUIManager.Instance.WaitingWidgetName = "";



            }
        }

        if (m_bStartCount)
        {
            m_fAutoLastTime += Time.deltaTime;

            if (m_fAutoLastTime > 10)
            {
                m_bStartCount = false;
                m_fAutoLastTime = 0f;
                CallWhenTimeOut();
            }
        }

        if (m_bIsTeaching)
        {
            if (m_goFocusWidget == null)
                return;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    if (Input.touchCount > 0)
                    {
                        if (m_bIsMaskClickable)
                        {
                            BoxCollider bc = m_goFocusWidget.GetComponentsInChildren<BoxCollider>(true)[0];

                            RaycastHit hit = new RaycastHit();


                            if (bc.Raycast(m_camTeachUI.ScreenPointToRay(Input.GetTouch(0).position), out hit, 1000000.0f))
                            {
                                return;
                            }

                            ShowTeachUI(false);
                            ShowFingerAnim(false);
                            ShowTip(Vector3.zero, "", false);
                            EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.ButtonClick);

                            if (m_goFocusWidget != null)
                            {
                                if (m_goFocusWidget.name == "Controller")
                                {
                                    m_oriWidget.GetComponentsInChildren<ControlStick>(true)[0].Reset();
                                    m_oriWidget.GetComponentsInChildren<ControlStick>(true)[0].InitInstance();
                                    MogoFXManager.Instance.ReleaseParticleAnim("ControllerFX");
                                }
                                Destroy(m_goFocusWidget);
                            }

                            m_bIsTeaching = false;
                        }
                    }
                    break;

                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (m_bIsMaskClickable)
                        {
                            if (m_goFocusWidget == null)
                                return;

                            BoxCollider bc = m_goFocusWidget.GetComponentsInChildren<BoxCollider>(true)[0];

                            if (bc == null)
                            {
                                return;
                            }

                            RaycastHit hit = new RaycastHit();


                            if (bc.Raycast(m_camTeachUI.ScreenPointToRay(Input.mousePosition), out hit, 1000000.0f))
                            {
                                return;
                            }


                            ShowTeachUI(false);
                            ShowFingerAnim(false);
                            ShowTip(Vector3.zero, "", false);
                            EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.ButtonClick);
                            if (m_goFocusWidget != null)
                            {
                                if (m_goFocusWidget.name == "Controller")
                                {
                                    m_oriWidget.GetComponentsInChildren<ControlStick>(true)[0].Reset();
                                    m_oriWidget.GetComponentsInChildren<ControlStick>(true)[0].InitInstance();
                                    MogoFXManager.Instance.ReleaseParticleAnim("ControllerFX");
                                }
                                Destroy(m_goFocusWidget);
                            }

                            m_bIsTeaching = false;
                        }
                    }

                    break;

            }

        }
    }
}