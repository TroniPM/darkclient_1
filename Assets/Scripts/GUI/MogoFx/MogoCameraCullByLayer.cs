using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MogoCameraCullByLayer : MonoBehaviour {

    public List<int> LayerList;
    public List<float> DistanceList;

    void Start()
    {
        float[] distances = new float[32];

        for (int i = 0; i < LayerList.Count; ++i)
        {
            distances[LayerList[i]] = DistanceList[i];
        }

        GetComponent<Camera>().layerCullDistances = distances;
        GetComponent<Camera>().layerCullSpherical = true;
    }
}
