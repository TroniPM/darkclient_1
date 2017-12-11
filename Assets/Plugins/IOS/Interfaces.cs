using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IUIView
{
    void UpdateView<T>(List<T> datas);
    void CleanUp();
}
