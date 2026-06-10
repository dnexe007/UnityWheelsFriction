using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSetup : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private float targetMass;


	public Wheel[] wheels;
	public float TotalMass { get; private set; }


	private void Awake()
	{
		float xCount = 10 + wheels.Length;

		float x =  targetMass / xCount;

		body.mass = x * 10;

		foreach (Wheel wheel in wheels)
			wheel.gameObject.GetComponent<Rigidbody>().mass = x;

		TotalMass = targetMass;
	}
}
