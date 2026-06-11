using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnchorFriction : MonoBehaviour
{
	public float frictionCoef = 8;
	public float dampCoef = 16;


	public float slipPeakSpeed = 0.4f;
	public float slipBreakSpeed = 0.5f;


	public float slipClamp = 0.05f;

	public bool dontBreakSlip;



	private Rigidbody rb;
	private MassSetup ms;

	public float slip;
	public float slipBreakMult;

	public float slipBreakState;

	public float currentSidewaysSpeed;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		ms = GetComponentInParent<MassSetup>();
	}


	private void FixedUpdate()
	{
		int groundCount = 0;
		foreach (Wheel w in ms.wheels)
			groundCount += (w.IsGrounded ? 1 : 0);
		if (groundCount == 0)
		{
			slip = 0;
			return;
		}

		Vector3 localVel = transform.InverseTransformDirection(rb.velocity);


		slip += localVel.x * Time.fixedDeltaTime;
		slip = Mathf.Clamp(slip, -slipClamp, slipClamp);


		float basicFriction = ms.TotalMass * frictionCoef;
		float frictionMult = -slip / slipClamp;


		float basicDamp = ms.TotalMass * dampCoef;
		float dampMult = -localVel.x;


		slipBreakState = Mathf.Clamp01(
			Mathf.InverseLerp(
				slipPeakSpeed,
				slipBreakSpeed,
				Mathf.Abs(localVel.x)
			)
		);
		currentSidewaysSpeed = MathF.Round(Mathf.Abs(localVel.x), 4);

		if (dontBreakSlip) slipBreakState = 0;
		slipBreakMult = Mathf.Lerp( 1, 0, slipBreakState);
		float totalForceMult = slipBreakMult / ms.wheels.Length;


		foreach(Wheel w in ms.wheels) if (w.IsGrounded)
		{
			rb.AddForceAtPosition(
				(
					basicFriction * frictionMult
					+
					basicDamp * dampMult
				)
				*
				totalForceMult * transform.right,
				w.transform.position
			);
		}
	}


}
