using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSetup : MonoBehaviour
{
    [SerializeField] private float targetMass;

	[HideInInspector] public Wheel[] wheels;
	private Rigidbody body;

	public float TotalMass { get; private set; }

	private void Awake()
	{
		body = transform.Find("Body").GetComponent<Rigidbody>();
		wheels = GetComponentsInChildren<Wheel>();

		float xCount = 10 + wheels.Length;

		float x =  targetMass / xCount;

		body.mass = x * 10;

		foreach (Wheel wheel in wheels)
			wheel.gameObject.GetComponent<Rigidbody>().mass = x;

		TotalMass = targetMass;
	}
}
