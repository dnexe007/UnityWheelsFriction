using UnityEngine;

public class Wheel : MonoBehaviour
{
	[SerializeField] private float sidewaysFrictionCoef = 2;
	[SerializeField] private float forwardFrictionCoef = 0.1f;
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
		
		ApplyLinearFriction(-transform.up, sidewaysFrictionCoef);
		ApplyLinearFriction(transform.forward, forwardFrictionCoef);
	}


	private void ApplyLinearFriction(Vector3 direction, float frictionCoef)
	{
		if (!IsGrounded) return;

		Vector3 sidewaysDir = Vector3.ProjectOnPlane(direction, CurrentNormal).normalized;
		Vector3 slipVel = Vector3.Project(Rb.velocity, sidewaysDir);

		float basicFriction = frictionCoef * ms.TotalMass;
		float speedFrictionMult = Mathf.Lerp(0, 1, slipVel.magnitude / fullFrictionSpeed);

		Rb.AddForce(basicFriction * speedFrictionMult * -slipVel.normalized);
	}

	private void OnDrawGizmos()
	{
		if(!Application.isPlaying) return;
		Gizmos.color = Color.red;

		Gizmos.DrawLine(transform.position, transform.position + CurrentNormal);
	}
}
