using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargeRewardUIGameObjectGroup : MonoBehaviour
{
    public GameObject[] items;

    protected Dictionary<int, GameObject> itemPress = new Dictionary<int,GameObject>();
    protected Dictionary<int, UILabel> itemText = new Dictionary<int, UILabel>();

    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            itemPress.Add(i, items[i].GetComponentsInChildren<UISprite>(true)[1].gameObject);
            itemText.Add(i, items[i].GetComponentsInChildren<UILabel>(true)[0]);
        }
    }

    public void InitData()
    {
        itemPress.Clear();
        itemText.Clear();

        for (int i = 0; i < items.Length; i++)
        {
            itemPress.Add(i, items[i].GetComponentsInChildren<UISprite>(true)[1].gameObject);
            itemText.Add(i, items[i].GetComponentsInChildren<UILabel>(true)[0]);
        }
    }

    public void SetItemPress(int id)
    {
        foreach (var item in itemPress)
        {
            if (item.Key == id)
                itemPress[id].SetActive(true);
            else
                itemPress[id].SetActive(false);
        }
    }

    public void SetItemTextNum(int i, int num)
    {
        itemText[i].text = num.ToString();
    }
}

