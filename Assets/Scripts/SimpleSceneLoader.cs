using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SimpleSceneLoader : MonoBehaviour
{

	public void NormalMode(){
		SceneManager.LoadScene("NormalMode");
	}

	public void TetrisMode(){
		SceneManager.LoadScene("TetrisMode");

	}
}
