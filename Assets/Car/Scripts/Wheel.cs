using UnityEngine;

public class Wheel : MonoBehaviour
{
	[SerializeField] private float frictionCoef = 2;
	[SerializeField] private float fullFrictionSpeed = 0.1f;
	[SerializeField] private int contactsLength;


	public Rigidbody Rb { get; private set; }
	private MassSetup ms;
	private readonly AverageNormalData _normalData = new();


	public Vector3 CurrentNormal { get; private set; }
	public bool IsGrounded { get; private set; }

	private static readonly ContactPoint[] _contactsBuffer = new ContactPoint[16];


	private void Start()
	{
		Rb = GetComponent<Rigidbody>();
		ms = GetComponentInParent<MassSetup>();
	}


	private void OnCollisionStay(Collision collision)
	{
		int count = collision.GetContacts(_contactsBuffer);
		for (int i = 0; i < count; i++)
		{
			ContactPoint point = _contactsBuffer[i];
			float sidewaysAngle = Vector3.Angle(point.normal, transform.up);
			if(sidewaysAngle > 135 || sidewaysAngle < 40) continue;
			_normalData.Add(_contactsBuffer[i]);
		}
	}

	private void FixedUpdate()
	{
		contactsLength = _normalData.Count;

		CurrentNormal = _normalData.CalculateAndReset();
		IsGrounded = CurrentNormal != Vector3.zero;
		
		ApplyLinearFriction();
	}


	private void ApplyLinearFriction()
	{
		if (!IsGrounded) return;

		Vector3 localVel = transform.InverseTransformDirection(Rb.velocity);

		float sidewaysDir = Mathf.Sign(localVel.y);

		float frictionMult = Mathf.Lerp(0, 1, Mathf.Abs(localVel.y) / fullFrictionSpeed);

		Rb.AddForce(-transform.up * frictionCoef * ms.TotalMass * sidewaysDir * frictionMult);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		if(Application.isPlaying)
			Gizmos.DrawLine(transform.position, transform.position + CurrentNormal * 2);
	}
}
