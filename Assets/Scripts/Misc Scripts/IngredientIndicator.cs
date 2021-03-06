﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientIndicator : MonoBehaviour {

	public GameObject indicatorPrefab;

	private static Vector3 startingScale = new Vector3 (0.01f, 0.01f, 0.01f);
	private static Vector3 endingScale = new Vector3 (1f, 1f, 1f);
	private static float lerpExponent = 24f;
	private float originalDistanceFromIndicator;

	public GameObject indicator;

	// Need this in awake for the same reason we need it in awake in the FallDecayDie script
	void Awake () {
		RaycastHit hit;
		bool raycast = ObjectSpawn.RaycastUntilTerrain(transform.position, Vector3.down, out hit);
		if (raycast) {
			indicator = Instantiate (indicatorPrefab, hit.point, Quaternion.identity) as GameObject;
			Color color = IngredientSet.GetColorForIngredient (name);
			indicator.GetComponent<ColorSetter> ().SetColor (color);
			Texture tex = IngredientSet.GetIndicatorSpriteForIngredient (name).texture;
			indicator.transform.GetChild (0).GetComponent<Renderer> ().material.SetTexture ("_MainTex", tex);
			indicator.transform.localScale = startingScale;
			originalDistanceFromIndicator = CalculateDistanceToIndicator ();
		}
	}

	void Update() {
		if (indicator != null) {
			// Change the indicator's scale.
			indicator.transform.localScale = Vector3.Lerp (endingScale, startingScale, Mathf.Pow(CalculateDistanceToIndicator () / originalDistanceFromIndicator, lerpExponent));
		}
	}
	
	public void DestroyIndicator () {
		Destroy (indicator);
	}

	float CalculateDistanceToIndicator() {
		return Vector3.Distance (transform.position, indicator.transform.position);
	}

}
