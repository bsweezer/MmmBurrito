﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCatcher : MonoBehaviour {

	public bool canCatch;
	private CaughtIngredientSet caughtIngredients;


	private string currentBurritoText; 
	private bool newIngredient=false; 
	public IngredientSet.Ingredients ingredientType;

	void Start () {
		canCatch = true;
		caughtIngredients = new CaughtIngredientSet();
		SetTextString("");
	}

	public void SetTextString (string text) {
		currentBurritoText = text;
	}

	public string GetTextString () {
		return currentBurritoText;
	}

	public void SetnewIngredient(bool boolean) {
		newIngredient = boolean;
	}

	public bool GetnewIngredient() {
		return newIngredient;
	}

	/** Returns true if this ObjectCatcher is empty */
	public bool IsEmpty () {
		return caughtIngredients.IsEmpty();
	}


	/** Handle collisions with falling objects */
	void OnCollisionEnter (Collision collision) {
        if (!canCatch)
        {
            // return; // disabled until we can visually show the burrito's wrap-state
        }

		GameObject gameObj = collision.gameObject;
		if (gameObj.tag == "FallingObject") { //&& caughtIngredients.ingredientSet.GetFullCount() < 6) { 
			CatchObject (gameObj);
			LoggingManager.instance.RecordEvent (6, "Caught ingredient - " + gameObj.name);
			if (gameObj.GetComponent<MoveToScreen> () != null) {
				gameObj.GetComponent<MoveToScreen> ().StartMovingToScreenBottom (true);
			} else {
				Destroy (gameObj);
			}
		}
//		else {
//			Debug.Log ("Cannot catch anymore");
//			SetTextString("Cannot catch anymore");
//		}
	}

	/** Catches an object by updating the caught values for the [caughtObjects] dictionary */
	void CatchObject (GameObject gameObj) {
        //remove from GameController
        GameController.instance.objects.RemoveAt(GameController.instance.objects.IndexOf(gameObj));
        // Catch object
        string objectName = gameObj.name.Replace ("(Clone)", "");
		ingredientType = IngredientSet.StringToIngredient (objectName);
		int quality = gameObj.GetComponent<FallDecayDie> ().getQuality ();
		caughtIngredients.CatchIngredient (ingredientType, quality);
		SetnewIngredient (true);

		// Print out
		Debug.Log (string.Format("Caught a {0}, burrito now contains:\n{1}", objectName, CaughtObjectsToString ()));
		SetTextString(string.Format("Burrito contents: {0}", CaughtObjectsToString ()));
	}

	/** Returns the content of the [caughtObjects] dictionary as a string */
	public string CaughtObjectsToString () {
		return caughtIngredients.ToString ();
	}

	public CaughtIngredientSet getIngredients(){
		return caughtIngredients;
	}
}
