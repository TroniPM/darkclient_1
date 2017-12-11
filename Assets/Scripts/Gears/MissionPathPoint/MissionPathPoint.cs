using UnityEngine;
using System.Collections;

public class MissionPathPoint : MonoBehaviour
{
    public enum EnableType : int
    {
        None = 0,
        Wait = 1
    }

    public enum PointerType : int
    {
        Normal = 0,
        Pointer = 1
    }

    public enum MissionPathPointType : int
    {
        Normal = 0,
        Teleprot = 1,
        Move
    }

    public int id;
    public int preID = 0;
    public float range = 2f;
    public int[] deleteList;

    public PointerType pointerType = PointerType.Normal;
    public EnableType isEnable = EnableType.None;
    public MissionPathPointType isNormalType = MissionPathPointType.Normal;

    public Vector3 pathPointPosition { get; protected set; }
    public Vector3 pathPointRotation { get; protected set; }
    public Vector3 pathPointSize { get; protected set; }

    public Vector3 movePosition = Vector3.zero;

    void Awake()
    {
        pathPointPosition = transform.position;
        pathPointRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        pathPointSize = transform.lossyScale;
    }
}