using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TImeScale : MonoBehaviour
{
    public float scale = 0.2f;

	private void Update()
	{
		Time.timeScale = Input.GetKey(KeyCode.T) ? scale : 1;
	}
}
