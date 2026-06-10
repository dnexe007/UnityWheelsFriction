using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMass : MonoBehaviour
{
    public Rigidbody body;

    public Rigidbody[] wheels;

    public float totalMass;


	private void Awake()
	{
		float xCount = 10 + wheels.Length;

		float x =  totalMass / xCount;

		body.mass = x * 10;

		foreach (Rigidbody wheel in wheels)
			wheel.mass = x;
	}
}
