using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.Game;
using System.Text;
using Mogo.GameData;

public class InstanceHelperGrid : MonoBehaviour
{
    public int id;

    Transform helperTransform;

    UISprite helperBG;
    UISprite helperBGDown;
    UISprite helperImage;

    UILabel helperName;
    UILabel helperLevel;
    UILabel helperAttack;
    UILabel helperHeartNumber;

    bool isMoved = false; 

    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            if (isMoved)
            {
                isMoved = false;
            }
            else
            {
                // InstanceUILogicManager.Instance.SelectMercenary(id);
                // InstanceUIViewManager.Instance.SetChooseHelper(id);
                InstanceHelpChooseUIViewManager.Instance.SetChooseHelper(id);
            }
        }
    }

    void OnDrag(Vector2 offset)
    {
        isMoved = true;
    }

    void Awake()
    {
        helperTransform = transform;

        helperBG = helperTransform.Find("InstanceLevelChooseUIPlayerGridBG/InstanceLevelChooseUIPlayerGridBGUp").GetComponentsInChildren<UISprite>(true)[0];
        helperBGDown = helperTransform.Find("InstanceLevelChooseUIPlayerGridBG/InstanceLevelChooseUIPlayerGridBGDown").GetComponentsInChildren<UISprite>(true)[0];

        helperImage = helperTransform.Find("GOInstanceLevelChooseUIPlayerGridImage/InstanceLevelChooseUIPlayerGridImage").GetComponentsInChildren<UISprite>(true)[0];

        helperName = helperTransform.Find("InstanceLevelChooseUIPlayerGridName").GetComponentsInChildren<UILabel>(true)[0];
        helperLevel = helperTransform.Find("InstanceLevelChooseUIPlayerGridLevel").GetComponentsInChildren<UILabel>(true)[0];
        helperAttack = helperTransform.Find("InstanceLevelChooseUIPlayerGridAttack").GetComponentsInChildren<UILabel>(true)[0];
        helperHeartNumber = helperTransform.Find("InstanceLevelChooseUIPlayerGridHeartNumber").GetComponentsInChildren<UILabel>(true)[0];
    }

    public void SetHelperImage(Vocation vocation)
    {
        LoggerHelper.Debug("Vocation: " + vocation);
        helperImage.spriteName = IconData.dataMap.Get((int)IconOffset.Avatar + (int)vocation).path;
    }

    public void SetHelper(string UIName, string name, string level, string attack, int number)
    {
        SetHelperUIName(UIName);
        SetHelperName(name);
        SetHelperLevel(level);
        SetHelperAttack(attack);
        SetHelperHeartNumber(number);
    }

    public void SetHelper(Vocation vocation, string name, string level, string attack, int number)
    {
        SetHelperImage(vocation);
        SetHelperName(name);
        SetHelperLevel(level);
        SetHelperAttack(attack);
        SetHelperHeartNumber(number);
    }

    public void SetHelperUIName(string UIName)
    {
        helperImage.spriteName = UIName;
    }

    public void SetHelperName(string name)
    {
        helperName.text = name;
    }

    public void SetHelperLevel(string level)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("LV：");
        sb.Append(level);
        helperLevel.text = sb.ToString();
    }

    public void SetHelperAttack(string attack)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(LanguageData.GetContent(46963));// 战力：
        sb.Append(attack);
        helperAttack.text = sb.ToString();
    }

    public void SetHelperHeartNumber(int number)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("x ");
        sb.Append(number);
        helperHeartNumber.text = sb.ToString();
    }

    public void SetEnable(bool enable)
    {
        if (enable)
        {
            //helperBG.color = new Color32(255, 255, 255, 255);
            helperBGDown.gameObject.SetActive(true);
        }
        else
        {
            //helperBG.color = new Color32(255, 255, 255, 255);

            helperBGDown.gameObject.SetActive(false);
        }
    }
}

