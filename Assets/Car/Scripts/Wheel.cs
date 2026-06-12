using UnityEngine;

public class Wheel : MonoBehaviour
{
	[SerializeField] private float sidewaysFrictionCoef = 2;
	[SerializeField] private float forwardFrictionCoef = 0.1f;
	[SerializeField] private float fullFrictionSpeed = 0.1f;


	private Configuration config;
	private readonly AverageNormalData normalData = new();


	private static readonly ContactPoint[] contactsBuffer = new ContactPoint[16];


	public Rigidbody WheelRigidoby { get; private set; }
	public Vector3 CurrentNormal { get; private set; }
	public bool IsGrounded { get; private set; }


	private void Start()
	{
		WheelRigidoby = GetComponent<Rigidbody>();
		config = transform.parent.GetComponent<Configuration>();
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
		
		ApplyLinearFriction(-transform.up, sidewaysFrictionCoef);
		ApplyLinearFriction(transform.forward, forwardFrictionCoef);

		Debug.DrawRay(transform.position, CurrentNormal, Color.red);
	}


	private void ApplyLinearFriction(Vector3 direction, float frictionCoef)
	{
		if (!IsGrounded) return;

		direction = Vector3.ProjectOnPlane(direction, CurrentNormal).normalized;
		Vector3 slipVel = Vector3.Project(WheelRigidoby.velocity, direction);

		float basicFriction = frictionCoef * config.TotalMass;
		float speedFrictionMult = Mathf.Lerp(0, 1, slipVel.magnitude / fullFrictionSpeed);

		config.RootRigidbody.AddForceAtPosition(
			basicFriction * speedFrictionMult * -slipVel.normalized,
			transform.position
		);
	}
}
