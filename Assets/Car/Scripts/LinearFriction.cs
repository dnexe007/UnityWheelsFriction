using UnityEngine;

public class LinearFriction : MonoBehaviour
{
	[SerializeField] private float sidewaysFrictionCoef = 2;
	[SerializeField] private float forwardFrictionCoef = 0.1f;
	[SerializeField] private float fullFrictionSpeed = 0.2f;

	private Configuration config;


	private void Start()
	{
		config = transform.parent.GetComponent<Configuration>();
	}


	private void FixedUpdate()
	{
		foreach(Wheel w in config.Wheels) if (w.IsGrounded)
		{
			ApplyLinearFriction(w, -w.transform.up, sidewaysFrictionCoef);
			ApplyLinearFriction(w, w.transform.forward, forwardFrictionCoef);
		}
	}


	private void ApplyLinearFriction(Wheel wheel, Vector3 direction, float frictionCoef)
	{
		direction = Vector3.ProjectOnPlane(direction, wheel.CurrentNormal).normalized;
		Vector3 slipVel = Vector3.Project(wheel.Rb.velocity, direction);

		float basicFriction = frictionCoef * config.TotalMass;
		float speedFrictionMult = Mathf.Lerp(0, 1, slipVel.magnitude / fullFrictionSpeed);

		config.RootRigidbody.AddForceAtPosition(
			basicFriction * speedFrictionMult * -slipVel.normalized,
			wheel.transform.position
		);
	}
}
