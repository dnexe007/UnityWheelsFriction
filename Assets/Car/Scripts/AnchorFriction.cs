using System.Collections;
using UnityEngine;
using System;

public class AnchorFriction : MonoBehaviour
{
	[SerializeField] private float frictionCoef = 8;
	[SerializeField] private float dampCoef = 8;
	[SerializeField] private AnimationCurve frictionMultOverSlipSpeed = new(
		new(0.06f, 1),
		new(0.1f, 0)
	);
	[SerializeField] private float offsetClamp = 0.05f;
	[SerializeField] private bool dontBreakFriction;

	private static readonly AverageNormalData avgNormal = new();

	private Configuration config;
	private LinearFriction linearFriction;
	
	private float sidewaysOffset;
	private float forwardOffset;

	private void Start()
	{
		config = GetComponentInParent<Configuration>();
		linearFriction = GetComponent<LinearFriction>();
	}

	private Vector3 GetAverageGroundNormal()
	{
		Vector3 result = GetAverageNormal(w => w.CurrentNormal);
		Debug.DrawRay(transform.position, result, Color.green);
		return result;
	}

	private Vector3 GetAverageRight()
	{
		Vector3 result = GetAverageNormal(w => w.transform.right);
		Debug.DrawRay(transform.position, result, Color.red);
		return result;
	}

	private Vector3 GetAverageForward()
	{
		Vector3 result = GetAverageNormal(w => w.transform.forward);
		Debug.DrawRay(transform.position, result, Color.blue);
		return result;
	}

	private Vector3 GetAverageNormal(Func<Wheel, Vector3> func)
	{
		foreach (Wheel w in config.Wheels) if (w.IsGrounded)
			avgNormal.Add(func(w));
		return avgNormal.CalculateAndReset();
	}

	private void ApplyAnchorFriction(Vector3 axis, Vector3 normal, ref float offset)
	{
		Vector3 axisVector = Vector3.ProjectOnPlane(axis, normal).normalized;

		Vector3 avgVelocity = Vector3.zero;
		int wheelsOnGround = 0;
		foreach (Wheel w in config.Wheels) if (w.IsGrounded)
		{
			wheelsOnGround++;
			avgVelocity += w.Rb.velocity;
		}
		avgVelocity /= wheelsOnGround;

		Vector3 axisVelocity = Vector3.Project(avgVelocity, axisVector);

		float velocityDirection = Mathf.Sign(Vector3.Dot(axisVector, axisVelocity));
		offset += axisVelocity.magnitude * velocityDirection * Time.fixedDeltaTime;
		offset = Mathf.Clamp(offset, -offsetClamp, offsetClamp);

		float slipBreakMult = frictionMultOverSlipSpeed.Evaluate(Mathf.Abs(axisVelocity.magnitude));
		if (dontBreakFriction) slipBreakMult = 1;

		Vector3 friction = frictionCoef * config.TotalMass * offset / offsetClamp * -axisVector;
		Vector3 damp = dampCoef * config.TotalMass * -axisVelocity;
		Vector3 totalForce = friction + damp;

		foreach (Wheel w in config.Wheels) if (w.IsGrounded)
			config.RootRigidbody.AddForceAtPosition(
				totalForce * slipBreakMult / config.Wheels.Length,
				w.transform.position
			);
	}

	private void FixedUpdate()
	{
		Vector3 normal = GetAverageGroundNormal();
		if (normal == Vector3.zero) return;

		ApplyAnchorFriction(GetAverageRight(), normal, ref sidewaysOffset);
		if(linearFriction.IsBrakeEnabled)
			ApplyAnchorFriction(GetAverageForward(), normal, ref forwardOffset);
	}
}
