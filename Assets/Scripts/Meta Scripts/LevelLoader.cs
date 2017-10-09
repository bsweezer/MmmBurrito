﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

	public GameObject canvasHUD;
	public GameObject canvasMenu;

	private int loadingLevelNumber;
	private bool inMenu;

	void Awake () {
		loadingLevelNumber = -1;
		inMenu = true;
	}

	public void GoToLevel(int levelNumber) {
		inMenu = false;
		loadingLevelNumber = levelNumber;
		SceneManager.LoadScene ("Level_"+levelNumber);
	}

	public void GoToMenu () {
		inMenu = true;
		SceneManager.LoadScene ("LevelSelection");
	}

	// http://answers.unity3d.com/questions/1174255/since-onlevelwasloaded-is-deprecated-in-540b15-wha.html
	void OnEnable()
	{
		//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{	
		if (scene.name.Contains ("Level_")) {
			// Loaded a level.
			InitializeLevel (loadingLevelNumber);
			loadingLevelNumber = -1;
			SetActiveCanvas ();
		} else {
			// Loaded a menu.
			SetActiveCanvas ();
		}
	}

	void SetActiveCanvas () {
		canvasHUD.SetActive(!inMenu);
		canvasMenu.SetActive(inMenu);
	}

	void InitializeLevel (int levelNumber) {
		SetupLevelVars (levelNumber);

		SpawnController.instance.SpawnBurrito ();

		OrderUI.instance.ResetUIFields();

		GetComponent<Timer> ().startTimer ();
	}

	// Setup the level variables for the specified level.
	void SetupLevelVars (int levelNumber) {
		OrderController.instance.orderList.Clear ();
		Timer timer = GetComponent<Timer> ();
		switch (levelNumber) {
			case 1:
				OrderController.instance.AddOrder (0, 1);
				timer.TimerInit (120);
				break;
			case 2:
				OrderController.instance.AddOrder (0, 1);
				OrderController.instance.AddOrder (1, 1);
				OrderController.instance.AddOrder (2, 1);
				timer.TimerInit (120);
				break;
			case 3:
				OrderController.instance.AddOrder (2, 2);
				OrderController.instance.AddOrder (3, 1);
				OrderController.instance.AddOrder (4, 1);
				timer.TimerInit (230);
				break;
		}
		Debug.Log (OrderController.instance.OrderListToString ());
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && !inMenu) {
			GoToMenu ();
		}
	}


}