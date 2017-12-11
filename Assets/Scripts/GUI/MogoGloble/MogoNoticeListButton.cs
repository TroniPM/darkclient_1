using UnityEngine;
using System.Collections;

public class MogoNoticeListButton : MonoBehaviour
{
    void Start()
    {
    }
    void Update()
    {
    }

    public int id = 0;

    void OnClick()
    {
        MogoNotice2.Instance.RefreshRightUI(id);
    }
}
