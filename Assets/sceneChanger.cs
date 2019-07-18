using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChanger : MonoBehaviour {


	public void PlayGame1 ()
	{
		modeKeeper.Mode = 0;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		Debug.Log("Opening Mode 0");
	}	

	public void PlayGame2 ()
	{
		modeKeeper.Mode = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		Debug.Log("Opening Mode 1");
	}	

	public void PlayGame3 ()
	{
		modeKeeper.Mode = 4;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		Debug.Log("Opening Mode 4");
	}	

}
