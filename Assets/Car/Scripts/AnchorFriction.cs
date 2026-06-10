using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorFriction : MonoBehaviour
{
    public float frictionCoef = 8;
	public float dampCoef = 16;


	public float slipBreakPoint = 0.04f;
	public float slipClamp = 0.05f;

	public bool dontBreakSlip;



	public Wheel[] wheels;


    private Rigidbody rb;

	public float slip;
	public float slipBreakMult;

	public float slipBreakState;
	public float groundCountMult;



	private float TotalMass
	{
		get
		{
			float totalMass = rb.mass;
			foreach (Wheel wheel in wheels)
				totalMass += wheel.Rb.mass;
			return totalMass;
		}
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}


	private void FixedUpdate()
	{

		int groundCount = 0;

		foreach (Wheel w in wheels)
			groundCount += (w.IsGrounded ? 1 : 0);
	
		Vector3 localVel = transform.InverseTransformDirection(rb.velocity);

		if (groundCount == 0)
		{
			slip = 0;
			return;
		}

		slip += localVel.x * Time.fixedDeltaTime;
		slip = Mathf.Clamp(slip, -slipClamp, slipClamp);


		float basicFriction = TotalMass * frictionCoef;
		float frictionMult = -slip / slipClamp;


		float basicDamp = TotalMass * dampCoef;
		float dampMult = -localVel.x;


		groundCountMult = groundCount / wheels.Length;
		slipBreakState = Mathf.Clamp01(
			Mathf.InverseLerp(
				slipBreakPoint,
				slipClamp,
				Mathf.Abs(slip)
			)
		);
		if (dontBreakSlip) slipBreakState = 0;
		slipBreakMult = Mathf.Lerp( 1, 0, slipBreakState);
		float totalForceMult = (
			groundCountMult
			*
			slipBreakMult
		);


		rb.AddForce(
			(
				basicFriction * frictionMult
				+
				basicDamp * dampMult
			)
			*
			totalForceMult * transform.right
		);
	}
}
