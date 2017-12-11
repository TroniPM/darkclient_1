using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class TaskUIViewManager : MonoBehaviour
{
    private static TaskUIViewManager m_instance;

    public static TaskUIViewManager Instance
    {
        get
        {

            return TaskUIViewManager.m_instance;
        }
    }

    private Transform m_myTransform;
    UISprite m_spTaskUINPCImage;
    UILabel m_lblTaskUINPCName;
    UILabel m_lblTaskUINPCDialogText;
    UITexture m_texTaskUINPCImage;

    GameObject m_goContext;

    public bool IsNormalMainUI = false;

    public void SetTaskUINPCImage(string imgName)
    {
        if (imgName != "nobody")
        {
            AssetCacheMgr.GetResourceAutoRelease(string.Concat(imgName, ".png"),
                    (obj) =>
                    {
                        m_texTaskUINPCImage.mainTexture = obj as Texture;

                        AssetCacheMgr.GetResourceAutoRelease(string.Concat(imgName, "_A.png"),
                            (obja) =>
                            {
                                m_texTaskUINPCImage.material.SetTexture("_AlphaTex", (Texture)obja);
                                m_texTaskUINPCImage.MakePixelPerfect();

                                switch (imgName)
                                {
                                    case "body01":
                                        m_texTaskUINPCImage.transform.localPosition = new Vector3(-384f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texTaskUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "body02":
                                        m_texTaskUINPCImage.transform.localPosition = new Vector3(-384f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texTaskUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "body03":
                                        m_texTaskUINPCImage.transform.localPosition = new Vector3(-432f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texTaskUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "body04":
                                        m_texTaskUINPCImage.transform.localPosition = new Vector3(-437.3f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texTaskUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "npc01":
                                    case "npc02":
                                    case "npc03":
                                    case "npc05":
                                    case "npc06":
                                        m_texTaskUINPCImage.transform.localPosition = new Vector3(-384, 0, 0);
                                        //m_texTaskUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texTaskUINPCImage.gameObject.SetActive(true);
                                        break;


                                    case "npc04":
                                        m_texTaskUINPCImage.transform.localPosition = new Vector3(-384, -80, 0);
                                        //m_texTaskUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texTaskUINPCImage.gameObject.SetActive(true);
                                        break;

                                    default:
                                        m_texTaskUINPCImage.transform.localPosition = new Vector3(384, 0, 0);
                                        //m_texTaskUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
                                        m_texTaskUINPCImage.gameObject.SetActive(false);
                                        m_goContext.transform.localPosition = new Vector3(-350f, 0, 0);
                                        break;
                                }
                            });


                      


                    });
        }
        else
        {
            m_texTaskUINPCImage.transform.localPosition = new Vector3(-388.43f, 0, 0);
            m_texTaskUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
            m_texTaskUINPCImage.gameObject.SetActive(false);
            m_goContext.transform.localPosition = new Vector3(-350f, 0, 0);
        }
    }

    public void SetTaskUINPCName(string name)
    {
        m_lblTaskUINPCName.text = name;
    }

    public void SetTaskUINPCDialogText(string text)
    {
        m_lblTaskUINPCDialogText.text = text;
    }

    void OnTaskUIDownSignUp()
    {
        LoggerHelper.Debug("DownSignUp View");
    }

    void Awake()
    {
        m_myTransform = transform;

        m_instance = m_myTransform.GetComponentsInChildren<TaskUIViewManager>(true)[0];

        Initialize();



        //m_spTaskUINPCImage = m_myTransform.FindChild("TaskUIBottom/TaskUINPCImage").GetComponentsInChildren<UISprite>(true)[0];
        m_lblTaskUINPCDialogText = m_myTransform.Find("TaskUIBottom/TaskUIDialogContext/TaskUINPCDialogText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblTaskUINPCName = m_myTransform.Find("TaskUIBottom/TaskUIDialogContext/TaskUINPCName").GetComponentsInChildren<UILabel>(true)[0];
        m_myTransform.Find("TaskUIBottom").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_texTaskUINPCImage = m_myTransform.Find("TaskUIBottom/TaskUINPCImage").GetComponentsInChildren<UITexture>(true)[0];

        m_goContext = m_myTransform.Find("TaskUIBottom/TaskUIDialogContext").gameObject;

    }

    void Initialize()
    {

        TaskUILogicManager.Instance.Initialize();


        EventDispatcher.AddEventListener("TaskUIMaskBG", OnTaskUIDownSignUp);
    }

    public void Release()
    {
        TaskUILogicManager.Instance.Release();

        EventDispatcher.RemoveEventListener("TaskUIMaskBG", OnTaskUIDownSignUp);
    }
}
