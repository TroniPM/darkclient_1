using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

class MarketBuyView
{
    private int STEP = 10;

    private GameObject panel;
    private UILabel name;
    private UILabel total;
    private UIInput num;
    private MogoButton minus;
    private MogoButton minusminus;
    private MogoButton plus;
    private MogoButton plusplus;
    private MogoButton close;
    private MogoButton commit;

    private MarketItem data;

    private int buynum = 0;

    public MarketBuyView(Transform transform)
    {
        panel = transform.gameObject;
        name = transform.Find("name").GetComponent<UILabel>();
        total = transform.Find("total").GetComponent<UILabel>();
        num = transform.Find("num").GetComponent<UIInput>();
        minus = transform.Find("minus").GetComponent<MogoButton>();
        minusminus = transform.Find("minusminus").GetComponent<MogoButton>();
        plus = transform.Find("plus").GetComponent<MogoButton>();
        plusplus = transform.Find("plusplus").GetComponent<MogoButton>();
        close = transform.Find("close").GetComponent<MogoButton>();
        commit = transform.Find("commit").GetComponent<MogoButton>();
        UILabel c = transform.Find("close/Label").GetComponent<UILabel>();
        UILabel cm = transform.Find("commit/Label").GetComponent<UILabel>();
        c.text = Mogo.GameData.LanguageData.GetContent(716); // "取消";
        cm.text = Mogo.GameData.LanguageData.GetContent(717); // "确定";
        
        minus.clickHandler = Minus;
        minusminus.clickHandler = MinusMinus;
        plus.clickHandler = Plus;
        plusplus.clickHandler = PlusPlus;
        close.clickHandler = Close;
        commit.clickHandler = Commit;
        num.validator = Validator;
        num.onSubmit = OnInputSubmit;
    }

    private char Validator(string currString, char currChar)
    {
        if (Char.IsNumber(currChar))
        {
            return currChar;
        }
        return Char.MinValue ;
    }

    private void OnInputSubmit(string v)
    {
        int n = 0;
        if (!Int32.TryParse(v, out n))
        {
            n = 0;
        }
        if (n < 0)
        {
            n = 0;
        }
        if (n > 99)
        {
            n = 99;
        }
        int price = data.resData.priceOrg;
        if (data.resData.priceNow > 0)
        {
            price = data.resData.priceNow;
        }
        buynum = n;
        SetTotal(buynum, buynum * price);
    }

    private void Minus()
    {
        buynum--;
        if (buynum < 0)
        {
            buynum = 0;
        }
        int price = data.resData.priceOrg;
        if (data.resData.priceNow > 0)
        {
            price = data.resData.priceNow;
        }
        SetTotal(buynum, buynum * price);
    }

    private void MinusMinus()
    {
        buynum = buynum - STEP;
        if (buynum < 0)
        {
            buynum = 0;
        }
        int price = data.resData.priceOrg;
        if (data.resData.priceNow > 0)
        {
            price = data.resData.priceNow;
        }
        SetTotal(buynum, buynum * price);
    }

    private void Plus()
    {
        buynum++;
        int price = data.resData.priceOrg;
        if (data.resData.priceNow > 0)
        {
            price = data.resData.priceNow;
        }
        SetTotal(buynum, buynum * price);
    }

    private void PlusPlus()
    {
        buynum = buynum + STEP;
        int price = data.resData.priceOrg;
        if (data.resData.priceNow > 0)
        {
            price = data.resData.priceNow;
        }
        SetTotal(buynum, buynum * price);
    }

    private void SetTotal(int buynum, int price)
    {
        if (price > MogoWorld.thePlayer.diamond)
        {
            num.activeColor = Color.red;
            num.text = buynum + "";
            total.text = "[FF0000]" + price + "[-]";
            MogoMsgBox.Instance.ShowFloatingText(Mogo.GameData.LanguageData.dataMap[307].content);
        }
        else
        {
            num.activeColor = Color.white;
            num.text = buynum + "";
            total.text = price + "";
        }
    }

    private void Commit()
    {
        if (num.text == "")
        {
            Close();
            return;
        }
        int buynum = Convert.ToInt32(num.text);
        if (buynum <= 0)
        {
            //MogoMsgBox.Instance.ShowFloatingText("购买数量不能小于1");
            MogoMsgBox.Instance.ShowFloatingText(Mogo.GameData.LanguageData.GetContent(200018));
            return;
        }
        if (buynum > 99)
        {
            //MogoMsgBox.Instance.ShowFloatingText("一次不能购买超过99个");
            MogoMsgBox.Instance.ShowFloatingText(Mogo.GameData.LanguageData.GetContent(200019));
            return;
        }
        EventDispatcher.TriggerEvent(MarketEvent.BuyNum, buynum, data);
        Close();
    }

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        Clear();
    }

    public void AddToParent(Transform parent, Quaternion rotation)
    {

    }

    public void Remove()
    {

    }

    public void SetProp(MarketItem item)
    {
        data = item;
        name.text = data.name;
        buynum = 1;
        int price = data.resData.priceOrg;
        if (data.resData.priceNow > 0)
        {
            price = data.resData.priceNow;
        }
        SetTotal(buynum, price);
    }

    private void Clear()
    {
        name.text = "";
        data = null;
    }

}
