using UnityEngine;

public class LinearFriction : MonoBehaviour
{
	[SerializeField] private float sidewaysFrictionCoef = 2;
	[SerializeField] private float brakeFrictionCoef = 2;
	[SerializeField] private float fullFrictionSpeed = 0.2f;

	public bool IsBrakeEnabled => Input.GetKey(KeyCode.Space);

	private Configuration config;


	private void Start()
	{
		config = transform.parent.GetComponent<Configuration>();
	}


	private void FixedUpdate()
	{
		foreach (Wheel w in config.Wheels) if (w.IsGrounded)
		{
			ApplyLinearFriction(w, w.transform.right, sidewaysFrictionCoef);
			if(IsBrakeEnabled)
				ApplyLinearFriction(w, w.transform.forward, brakeFrictionCoef);
		}
	}


	private void ApplyLinearFriction(Wheel wheel, Vector3 direction, float frictionCoef)
	{
		direction = Vector3.ProjectOnPlane(direction, wheel.CurrentNormal).normalized;

		Vector3 wheelVelocity = config.RootRigidbody.GetPointVelocity(wheel.transform.position);
		Vector3 slipVel = Vector3.Project(wheel.Rb.velocity, direction);

		float basicFriction = frictionCoef * config.TotalMass;
		float speedFrictionMult = Mathf.Lerp(0, 1, slipVel.magnitude / fullFrictionSpeed);

		config.RootRigidbody.AddForceAtPosition(
			basicFriction * speedFrictionMult * -slipVel.normalized,
			wheel.transform.position
		);
	}
}
