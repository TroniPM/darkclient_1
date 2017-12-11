using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class DialogUIViewManager : MonoBehaviour
{
    private static DialogUIViewManager m_instance;

    public static DialogUIViewManager Instance
    {
        get
        {

            return DialogUIViewManager.m_instance;
        }
    }

    private Transform m_myTransform;
    UISprite m_spDialogUINPCImage;
    UILabel m_lblDialogUINPCName;
    UILabel m_lblDialogUINPCDialogText;
    UITexture m_texDialogUINPCImage;

    GameObject m_goContext;

    public bool IsNormalMainUI = false;

    public void SetDialogUINPCImage(string imgName)
    {
        if (imgName != "nobody")
        {
            AssetCacheMgr.GetResourceAutoRelease(string.Concat(imgName, ".png"),
                    (obj) =>
                    {
                        m_texDialogUINPCImage.mainTexture = obj as Texture;

                        AssetCacheMgr.GetResourceAutoRelease(string.Concat(imgName, "_A.png"),
                            (obja) =>
                            {
                                m_texDialogUINPCImage.material.SetTexture("_AlphaTex", (Texture)obja);
                                m_texDialogUINPCImage.MakePixelPerfect();

                                switch (imgName)
                                {
                                    case "body01":
                                        m_texDialogUINPCImage.transform.localPosition = new Vector3(-384f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texDialogUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "body02":
                                        m_texDialogUINPCImage.transform.localPosition = new Vector3(-384f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texDialogUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "body03":
                                        m_texDialogUINPCImage.transform.localPosition = new Vector3(-432f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texDialogUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "body04":
                                        m_texDialogUINPCImage.transform.localPosition = new Vector3(-437.3f, 0, 0);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texDialogUINPCImage.gameObject.SetActive(true);
                                        break;

                                    case "npc01":
                                    case "npc02":
                                    case "npc03":
                                    case "npc05":
                                    case "npc06":
                                        m_texDialogUINPCImage.transform.localPosition = new Vector3(-384, 0, 0);
                                        //m_texDialogUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texDialogUINPCImage.gameObject.SetActive(true);
                                        break;


                                    case "npc04":
                                        m_texDialogUINPCImage.transform.localPosition = new Vector3(-384, -80, 0);
                                        //m_texDialogUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
                                        m_goContext.transform.localPosition = Vector3.zero;
                                        m_texDialogUINPCImage.gameObject.SetActive(true);
                                        break;

                                    default:
                                        m_texDialogUINPCImage.transform.localPosition = new Vector3(384, 0, 0);
                                        //m_texDialogUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
                                        m_texDialogUINPCImage.gameObject.SetActive(false);
                                        m_goContext.transform.localPosition = new Vector3(-350f, 0, 0);
                                        break;
                                }
                            });
                    });
        }
        else
        {
            m_texDialogUINPCImage.transform.localPosition = new Vector3(-388.43f, 0, 0);
            m_texDialogUINPCImage.transform.localScale = new Vector3(500f, 500f, 1);
            m_texDialogUINPCImage.gameObject.SetActive(false);
            m_goContext.transform.localPosition = new Vector3(-350f, 0, 0);
        }
    }

    public void SetDialogUINPCName(string name)
    {
        m_lblDialogUINPCName.text = name;
    }

    public void SetDialogUINPCDialogText(string text)
    {
        m_lblDialogUINPCDialogText.text = text;
    }



    void Awake()
    {
        m_myTransform = transform;

        m_instance = m_myTransform.GetComponentsInChildren<DialogUIViewManager>(true)[0];

        Initialize();
        m_lblDialogUINPCDialogText = m_myTransform.Find("DialogUIBottom/DialogUIDialogContext/DialogUINPCDialogText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDialogUINPCName = m_myTransform.Find("DialogUIBottom/DialogUIDialogContext/DialogUINPCName").GetComponentsInChildren<UILabel>(true)[0];
        m_myTransform.Find("DialogUIBottom").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_texDialogUINPCImage = m_myTransform.Find("DialogUIBottom/DialogUINPCImage").GetComponentsInChildren<UITexture>(true)[0];

        m_goContext = m_myTransform.Find("DialogUIBottom/DialogUIDialogContext").gameObject;

    }

    void Initialize()
    {

        DialogUILogicManager.Instance.Initialize();
    }

    public void Release()
    {
        DialogUILogicManager.Instance.Release();
    }
}
