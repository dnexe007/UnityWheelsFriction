using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float scale = 0.2f;

    private void Update()
	{
		Time.timeScale = Input.GetKey(KeyCode.T) ? scale : 1;
		if(Input.GetKeyDown(KeyCode.E)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
