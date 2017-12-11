using UnityEngine;
using System.Collections;

public class MogoGray : MonoBehaviour
{
    public Material Mat;
    RenderTexture rt;
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {

        Graphics.Blit(source, dest, Mat);
    }


    void OnDisable()
    {
        if (rt)
        {
            rt.Release();
            rt = null;
        }
    }
}
