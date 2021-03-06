﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {


	public Image circle;

	public GameObject timeLeftWarningContainer;
	public GameObject TimerObject;
	private Text timeLeftWarningText;


	private float time;
	private float maxT;

	private bool isfast;
	private bool isvfast;

	private float totalSeconds;

	private string timeDisplay;
	public Text timeDisplayText;


	private bool running;

	private const float SIGNAL_COOLDOWN = 9f;
	private float lastSignalTime = -SIGNAL_COOLDOWN*2f;
	private bool alreadySignaledLevelStart;


	// Make this class a singleton
	public static Timer instance = null;
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (this);
		}
		animManager = TimerObject.GetComponent<UIAnimationManager> ();
	}

	// showing time left
	private bool signalingTimeLeft;
	private float[] timeLeftSignals = {30f, 10f, 0f};
	private UIAnimationManager animManager;
	Vector2 DEFAULT_SIGNALING_POS = new Vector2 (0, -Screen.height * 0.5f);
	Vector3 DEFAULT_SIGNALING_SCALE = new Vector3(1.5f, 1.5f, 1.5f);
	Vector2 timerLevelStartPos = new Vector2 (0, -275f);
	float timerLevelStartScale = 2.5f;
	Color signalingTint = new Color(1, 1, 1, 0.7f);

	bool thirty;
	bool ten;

	//	void Awake() {
	//		animManager = TimerObject.GetComponent<UIAnimationManager> ();
	//	}

	void Start () {
		running = false;
		timeLeftWarningContainer.SetActive (false);
		thirty = false;
		ten = false;

		isfast = false;
		isvfast = false;

		timeLeftWarningText = timeLeftWarningContainer.transform.GetChild (0).GetComponent<Text> ();
	}

	public void TimerInit (int maxTime) {
		running = false;
		time = maxTime;
		maxT = maxTime;
		timeLeftWarningContainer.SetActive (false);
		UpdateDisplayedTime ();

		// Reset animations.
		animManager.StopAllAnimations ();
		animManager.ResetToInitialValues();
	}

	public void startTimer () {
		timeDisplayText.color = Color.black;
		running = true;
		isfast = false;
		isvfast = false;
		signalingTimeLeft = false;
		lastSignalTime = Time.time;
	}

	public void TimerUpdate () {
		if (GameController.instance.gamestate!=GameController.GameState.Play) {
			return;
		}

		time -= Time.deltaTime;
		if (time < 0) {
			time = 0.0f;
			StartCoroutine (DisplayLoseScreen ());
			LoggingManager.instance.RecordEvent(7, "Level quit, timer at 0");
			GameController.instance.gamestate = GameController.GameState.Lose;
			SoundController.instance.audSrc.PlayOneShot(SoundController.instance.lose, SoundController.instance.SoundEffectVolume.value);
			//            GameController.instance.levelEnd = true;
			//            GameController.instance.levelComplete = true;
			SignalTimeLeft ();

		}

		UpdateDisplayedTime ();

		GameController.instance.gameTime = (int)time;

		// Signal the time left (clock animation).
		bool tryToSignal = false;
		// check if at one of the schedules signals
		if (IsSignalCooldownOver () && !signalingTimeLeft) {
			foreach (float timeTarget in timeLeftSignals) {
				if (Mathf.Abs (time - timeTarget) < 0.25f) {
					tryToSignal = true;
					break;
				}
			}
		}
		// signal if appropriate
		if (tryToSignal) {
			SignalTimeLeft ();
		}
	}

	void UpdateDisplayedTime() {
		// fill
		circle.fillAmount = time/maxT;

		// text
		if (time <= 30) {
			timeDisplayText.color = Color.red;
		}
		timeDisplayText.text = ConvertTimeToDisplay(time);
	}

	public string ConvertTimeToDisplay(float secondsToConvert) {
		totalSeconds = Mathf.Ceil (secondsToConvert);
		int minutes = (int) totalSeconds / 60;
		int seconds = (int)totalSeconds % 60;
		string secondsDisplay;
		if (seconds < 10) {
			secondsDisplay = "0" + seconds.ToString();
		} else {
			secondsDisplay = seconds.ToString ();
		}
		return minutes.ToString() +":" +secondsDisplay;
	}


	IEnumerator DisplayLoseScreen() {
		yield return new WaitForSeconds (2.2f);
		//OrderUI.instance.setLoseMessage("You Lose! No time left!\nPress escape to return to the main menu");
		OrderUI.instance.setScore (GameController.instance.score.ToString ());
		OrderUI.instance.textfields.currentLevelLose.text = "Level "+GameController.instance.currentLevel;
		LevelLoader.instance.SetEndCanvas (); 
		OrderUI.instance.gameobjectfields.LoseScreen.gameObject.SetActive (true);
	}

	private void signalingTimeEnd() {
		signalingTimeLeft = false;
	}

	// Animates the clock to the middle of the screen, and then back to its default position.
	public void SignalTimeLeft(float duration = 1.75f, float tween1 = 0.75f, float tween2 = 0.5f) {
		SignalTimeLeft (DEFAULT_SIGNALING_POS, DEFAULT_SIGNALING_SCALE, duration, tween1, tween2);
	}

	// Animates the clock to the middle of the screen, and then back to its default position.
	public void SignalTimeLeft(Vector2 signalPos, Vector3 signalScale, float duration = 1.75f, float tween1 = 0.75f, float tween2 = 0.5f) {
		lastSignalTime = Time.time;
		signalingTimeLeft = true;
		animManager.MoveToPosAndBack    (signalPos,   duration, tween1, tween2, signalingTimeEnd);
		animManager.ScaleToValueAndBack (signalScale, duration, tween1, tween2);
		// animManager.TintToColorAndBack (signalingTint, duration, tween1, tween2);
	}

	// Update is called once per frame
	void Update () {
		if (running) {
			TimerUpdate ();
		}
	}

	public void AddSeconds(float seconds) {
		time += seconds;
	}

	public float getTime() {
		return totalSeconds;
	}

	public string getDisplayTime() {
		return ConvertTimeToDisplay(time);
	}

	private bool LevelJustStarted() {
		return time >= (maxT - 1);
	}

	private bool IsSignalCooldownOver() {
		return (lastSignalTime + SIGNAL_COOLDOWN) <= Time.time;
	}

	public void LevelStartSpawnAnimation() {
        animManager.LevelStartSpawnAnimation(timerLevelStartPos, 1f, timerLevelStartScale);
        animManager.pulse();
	}

	public void StopAnimations() {
		animManager.SkipToTargetPos ();
	}
}
