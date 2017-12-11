using UnityEngine;

public class Initializer : MonoBehaviour
{
	void Start () 
    {
        gameObject.AddComponent<Driver>();
        DestroyImmediate(this);
	}
}
