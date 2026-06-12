using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration : MonoBehaviour
{
    [SerializeField] private float targetMass;

	public Wheel[] Wheels { get; private set; }
	public Rigidbody RootRigidbody { get; private set; }
	public float TotalMass { get; private set; }

	private void Awake()
	{
		RootRigidbody = transform.Find("Body").GetComponent<Rigidbody>();
		Wheels = GetComponentsInChildren<Wheel>();

		float xCount = 10 + Wheels.Length;

		float x =  targetMass / xCount;

		RootRigidbody.mass = x * 10;

		foreach (Wheel wheel in Wheels)
			wheel.gameObject.GetComponent<Rigidbody>().mass = x;

		TotalMass = targetMass;
	}
}
