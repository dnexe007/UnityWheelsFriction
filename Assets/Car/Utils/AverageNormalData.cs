using UnityEngine;

public class AverageNormalData
{
	public int Count { get; private set; }

	private Vector3 normalSum;

	public Vector3 CalculateNormal()
	{
		if (Count == 0) return Vector3.zero;
		return normalSum / Count;
	}

	public void Add(ContactPoint point)
	{
		normalSum += point.normal;
		Count++;
	}

	public void Reset()
	{
		Count = 0;
		normalSum = Vector3.zero;
	}

	public Vector3 CalculateAndReset()
	{
		Vector3 normal = CalculateNormal();
		Reset();
		return normal;
	}
}