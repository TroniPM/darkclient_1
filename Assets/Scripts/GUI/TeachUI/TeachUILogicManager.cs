using UnityEngine;
using System.Collections;
using Mogo.GameData;
using Mogo.Util;
public class TeachUILogicManager
{
    public static string TEACHUICRASHED = "TeachUICrashed";

    private static TeachUILogicManager m_instance;

    public static TeachUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TeachUILogicManager();
            }

            return TeachUILogicManager.m_instance;

        }
    }

    public void Initialize()
    {

    }

    public void Release()
    {
    }

    string TranslateIdToWidgetName(int id)
    {
        Mogo.Util.LoggerHelper.Debug(id);

        if (UIMapData.dataMap.ContainsKey(id))
        {
            return UIMapData.dataMap[id].control;
        }
        else
        {
            return "fuck";
        }

    }

    public void TruelySetTeachUIFocus(int id, string text = "点一下亲", bool isAnyClick = false, int mask = 1,bool isWidgetCanClick = true,bool isAutoClick = false)
    {
        TimerHeap.AddTimer(300, 0, () =>
        {
            TeachUIViewManager.Instance.ShowTeachUI(true);
            if (mask > 0)
            {
                TeachUIViewManager.Instance.SetFocus(TranslateIdToWidgetName(id), text, isAnyClick, true, true, isWidgetCanClick, isAutoClick);
            }
            else
            {
                TeachUIViewManager.Instance.SetFocus(TranslateIdToWidgetName(id), text, isAnyClick, true, false, isWidgetCanClick, isAutoClick);
            }
        });
    }

    public void SetTeachUIFocus(int id, string text = "点一下亲",bool isAnyClick = false, int mask=1,GameObject baseUI = null)
    {
        if (MogoUIQueue.Instance.IsLocking)
        {
            TruelySetTeachUIFocus(id, text, isAnyClick, mask);
        }
        else
        {
            if (baseUI == null)
            {
                MogoUIQueue.Instance.PushOne(() => { TruelySetTeachUIFocus(id, text, isAnyClick, mask); }, MogoUIManager.Instance.m_NormalMainUI, "SetTeachUIFocus");
            }
            else
            {
                MogoUIQueue.Instance.PushOne(() => { TruelySetTeachUIFocus(id, text, isAnyClick, mask); }, baseUI, "SetTeachUIFocus");
            }
        }
       
    }

    public void SetItemFocus(int itemId, string tipText = "点一下亲", int mask = 1)
    {
        if (mask > 0)
        {
            TeachUIViewManager.Instance.SetItemFocus(itemId, tipText, true, true);
        }
        else
        {
            TeachUIViewManager.Instance.SetItemFocus(itemId, tipText, true, false);
        }
    }
    public void SetTeachUINoneFocus()
    {
        TeachUIViewManager.Instance.ShowTeachUI(true);
        TeachUIViewManager.Instance.SetNoneFocus();
    }                                                                         

    public void ShowFingerAnim(bool isShow)
    {
        TeachUIViewManager.Instance.ShowFingerAnim(true);
    }
}
