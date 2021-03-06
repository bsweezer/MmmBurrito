﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingCalls : MonoBehaviour {

	private static float positionLogCooldown = 1f;
	private static float lastPositionLog;
	
	// Update is called once per frame
	void Update () {
		LogPlayerPosition ();
	}

	// Logs the player's position if a player exists and enough time has elapsed since the last position logging.
	void LogPlayerPosition () {
//		if (GameController.instance.player == null || GameController.instance.levelComplete) {
//			return;
//		}
		if (GameController.instance.player == null || GameController.instance.gamestate!=GameController.GameState.Play) {
			return;
		}
		if (Time.time - lastPositionLog > positionLogCooldown) {
			lastPositionLog = Time.time;
			string positionLogMessage = "Coordinates: " + GameController.instance.player.transform.position.x + 
				", " + GameController.instance.player.transform.position.z;
            if (!GameController.instance.levelEnd)
            {
                LoggingManager.instance.RecordEvent(0, positionLogMessage);
            }
		}
	}
}
