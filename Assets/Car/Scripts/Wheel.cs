using UnityEngine;

public class Wheel : MonoBehaviour
{
	private readonly AverageNormalData normalData = new();
	private static readonly ContactPoint[] contactsBuffer = new ContactPoint[16];


	public Rigidbody Rb { get; private set; }
	public Vector3 CurrentNormal { get; private set; }
	public bool IsGrounded { get; private set; }


	private void Start()
	{
		Rb = GetComponent<Rigidbody>();
	}


	private void OnCollisionStay(Collision collision)
	{
		int count = collision.GetContacts(contactsBuffer);
		for (int i = 0; i < count; i++)
		{
			ContactPoint point = contactsBuffer[i];
			float sidewaysAngle = Vector3.Angle(point.normal, transform.up);
			if(sidewaysAngle > 135 || sidewaysAngle < 40) continue;
			normalData.Add(contactsBuffer[i]);
		}
	}

	private void FixedUpdate()
	{
		CurrentNormal = normalData.CalculateAndReset();
		IsGrounded = CurrentNormal != Vector3.zero;

		Debug.DrawRay(transform.position, CurrentNormal, Color.red);
	}
}
