using Unity.VisualScripting;
using UnityEngine;

public class Wheel : MonoBehaviour
{
	[SerializeField] private Vector3 rotationAxis = Vector3.right;
	[SerializeField] private float wheelDiameter = 0.3f;
	[SerializeField] private float wheelDeccelerationAnimSpeed = 0.3f;


	private readonly AverageNormalData normalData = new();
	private static readonly ContactPoint[] contactsBuffer = new ContactPoint[16];

	public Vector3 CurrentNormal { get; private set; }
	public bool IsGrounded { get; private set; }
	public Rigidbody Rb { get; private set; }



	private Transform model;
	private Vector3 currModelAngles;
	private float currModelRotationSpeed;


	private void Start()
	{
		Rb = GetComponent<Rigidbody>();
		model = transform.Find("Model");
		currModelAngles = model.localEulerAngles;
	}

	private void OnCollisionStay(Collision collision)
	{
		int count = collision.GetContacts(contactsBuffer);
		for (int i = 0; i < count; i++)
		{
			ContactPoint point = contactsBuffer[i];
			float sidewaysAngle = Vector3.Angle(point.normal, transform.right);
			if(sidewaysAngle > 140 || sidewaysAngle < 40) continue;
			normalData.Add(contactsBuffer[i]);
		}
	}

	private void FixedUpdate()
	{
		CurrentNormal = normalData.CalculateAndReset();
		IsGrounded = CurrentNormal != Vector3.zero;

		Debug.DrawRay(transform.position, CurrentNormal, Color.red);
		localSpeed = transform.InverseTransformDirection(Rb.velocity).z;
	}
	float localSpeed;
	private void Update()
	{
		if (!IsGrounded)
			currModelRotationSpeed = Mathf.Lerp(currModelRotationSpeed, 0, Time.deltaTime * wheelDeccelerationAnimSpeed);
		else currModelRotationSpeed = 360 * localSpeed / (wheelDiameter * Mathf.PI);
		currModelAngles += rotationAxis * Time.deltaTime * currModelRotationSpeed;
		model.localEulerAngles = currModelAngles;
	}
}
