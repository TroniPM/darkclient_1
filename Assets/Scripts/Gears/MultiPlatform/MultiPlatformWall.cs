using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MogoSimpleMotor))]
public class MultiPlatformWall : GearParent
{
    public Vector3 sourcePosition { get; protected set; }

    void Start()
    {
        gearType = "MultiPlatformWall";
        sourcePosition = transform.localPosition;
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == GearParent.MogoPlayerTag || collision.collider.gameObject.tag == "Gear")
        {
            gameObject.GetComponent<MogoSimpleMotor>().StopMoveTo();
        }
    }

    public void BeginMoveTo(Vector3 targetPosition)
    {
        gameObject.GetComponent<MogoSimpleMotor>().MoveTo(targetPosition);
    }
    
    public void SetLocalPosition(Vector3 v)
    {

    }
}
