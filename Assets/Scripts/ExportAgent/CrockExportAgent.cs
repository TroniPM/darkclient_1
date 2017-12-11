using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;

public class CrockExportAgent : ExportAgent
{
    public int type_i;
    public float mapx_i { get; protected set; }
    public float mapy_i { get; protected set; }
    public string rotation_l { get; protected set; }

    void Start()
    {
        type = "Crock";
        theTransform = transform;
        temp = gameObject;
    }

    void Update()
    {
        mapx_i = transform.position.x;
        mapy_i = transform.position.z;
        rotation_l = (new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z)).ToString();
    }
}
