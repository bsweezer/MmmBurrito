﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SubmissionController : MonoBehaviour {

	public GameObject burritoPrefab;

	private string submissionText; 
	private string winText; 

    private CaughtIngredientSet burritoCaughtIngredients;

    private Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, new Vector3(1, 0, 1));

    private AudioSource audSrc;
    private AudioClip rightOrder;
    private AudioClip wrongOrder;


	void Start () {
		setTextString ("");
		setWinString ("");

        audSrc = gameObject.GetComponent<AudioSource>();
        rightOrder = Resources.Load<AudioClip>("Sound/submit(right)");
        wrongOrder = Resources.Load<AudioClip>("Sound/submit(wrong)");
    }

	public void setTextString (string text) {
		submissionText = text;
	}

	public string getTextString () {
		return submissionText;
	}

	public void setWinString (string text) {
		winText = text;
	}

	public string getWinString () {
		return winText;
	}


	void OnCollisionEnter (Collision collision) {
		// Disregard any collisions that aren't with the burrito
		GameObject burrito = collision.gameObject;
		if (burrito.tag != "Player") {
			return;
		}

		// Prevent submission if we haven't caught any objects.
		if (burrito.GetComponent<ObjectCatcher> ().IsEmpty ()) {
			return;
		}

		SubmitBurrito (burrito);

		if (!GameController.instance.levelComplete) {
			SpawnController.instance.DestroyAndRespawn ();
			OrderUI.instance.ResetAfterDeath ();
		} else {
			SpawnController.instance.DestroyBurrito ();
		}
	}

	/** Submits a burrito */
	void SubmitBurrito (GameObject burrito) {
		Debug.Log ("Submitted a burrito with contents: " + burrito.GetComponent<ObjectCatcher> ().CaughtObjectsToString ());

		// Get a reference to the caught ingredients
		burritoCaughtIngredients = GameController.instance.player.GetComponent<ObjectCatcher> ().getIngredients ();

		// Logic regarding ordering system.
		List<IngredientSet> orders = OrderController.instance.orderList;
		bool matched = false;
		foreach (IngredientSet order in orders) {
            Debug.Log("hi");
            if (compareBurrito (order)) {
                //MATCHES
                audSrc.PlayOneShot(rightOrder);
				matched = true;
				Debug.Log ("Matches one of the orders!");
				setTextString ("Matches one of the orders!");
				int score = burritoCaughtIngredients.getSumOfQualities ()*50;
				GameController.instance.score += score;
                Debug.Log("You just got "+score+" score!");
                LoggingManager.instance.RecordEvent(2, "Submitted ingredients: " + GameController.instance.player.GetComponent<ObjectCatcher>().getIngredients().ToString()
                    + ". Gained score: " + score);
                OrderController.instance.orderList.RemoveAt (OrderController.instance.orderList.FindIndex(i => i.Equivalent(order)));
                if (OrderController.instance.orderList.Count == 0){
                    Debug.Log("All orders completed");
					setWinString ("You Win! Score: "+GameController.instance.score+"\n(Press escape to return to menu)\n(Press enter to go to next level)");
                    LoggingManager.instance.RecordEvent(8, "Won level, timer at " + GameController.instance.gameTime);
                    GameController.instance.levelComplete = true;

                    //Create GoToWinScreen instead?
                    // GameController.instance.GetComponent<LevelLoader>().GoToMenu();
                }
				else {
					Debug.Log ("Remaining " + OrderController.instance.OrderListToString ()); // print remaining orders
				}
				break;
			}
		} 
		if (!matched) {
            //DOES NOT MATCH
            audSrc.PlayOneShot(wrongOrder);
			Debug.Log("Submitted burrito does not match");
			setTextString ("Invalid Burrito Submission");
            LoggingManager.instance.RecordEvent(2, "Submitted ingredients: " + GameController.instance.player.GetComponent<ObjectCatcher>().getIngredients().ToString()
            + ". Did not match.");
        }

		// Update UI
		OrderUI.instance.setSubmissionMessage (getTextString());
		OrderUI.instance.setWinMessage (getWinString());
	}

	bool compareBurrito(IngredientSet targetOrder){
		IngredientSet burritoIngredients = burritoCaughtIngredients.ingredientSet;
		return burritoIngredients.Equivalent(targetOrder);
	}
}
