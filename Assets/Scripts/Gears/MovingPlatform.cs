using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    public float speed = 2.0F;
	public Transform start;
	public Transform end;
	
	private Vector3 startPos;
	private Vector3 endPos;
	private float distance;
	private Vector3 direction;
	private Rigidbody rb;
	
	
	public void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
		startPos = start.position;
		endPos = end.position;
		distance = Vector3.Distance(endPos, startPos);
		direction = Vector3.Normalize(endPos - startPos);
	}
	
	public void FixedUpdate ()
	{
		rb.MovePosition(Mathf.PingPong(Time.time * speed, distance) * direction + startPos);
	}
}