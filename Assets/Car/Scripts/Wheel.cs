using UnityEngine;

public class Wheel : MonoBehaviour
{
	public float frictionCoef = 2;
	public float fullFrictionSpeed = 0.1f;
	public float groundResetTime = 0.02f;

	public Rigidbody Rb { get; private set; }
	private MassSetup ms;
	public bool IsGrounded => Time.fixedTime - lastGroundTime <= groundResetTime;


	private float lastGroundTime;


	private void Start()
	{
		Rb = GetComponent<Rigidbody>();
		ms = GetComponentInParent<MassSetup>();
	}


	private void OnCollisionStay(Collision collision)
	{
		lastGroundTime = Time.fixedTime;
	}


	private void FixedUpdate()
	{
		ApplyLinearFriction();
	}


	private void ApplyLinearFriction()
	{
		if(!IsGrounded) return;

		Vector3 localVel = transform.InverseTransformDirection(Rb.velocity);

		float sidewaysDir = Mathf.Sign(localVel.y);

		float frictionMult = Mathf.Lerp(0, 1, Mathf.Abs(localVel.y) / fullFrictionSpeed);

		Rb.AddForce(-transform.up * frictionCoef * ms.TotalMass * sidewaysDir * frictionMult);
	}
}
