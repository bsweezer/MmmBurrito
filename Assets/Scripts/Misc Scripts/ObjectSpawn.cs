﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour {
	public GameObject[] fallingObjectList;
	public float spawnInterval;

	public bool onlyOneSpawn = false;

	public float spawnYOffset = 25f;
	public static float maxSpawnHeight = 200f;

	private float[] spawnRangeX;
	private float[] spawnRangeZ;

	void Start () {
		if (fallingObjectList.Length == 0) {
			return;
		}

		float xScale = transform.localScale.x;
		float zScale = transform.localScale.z;
		spawnRangeX = new float[2]{transform.position.x - xScale/2, transform.position.x + xScale/2};
		spawnRangeZ = new float[2]{transform.position.z - zScale/2, transform.position.z + zScale/2};

		if (onlyOneSpawn) {
			SpawnObject ();
		} else {
			StartCoroutine (SpawnFallingObjects ());
		}
	}

	IEnumerator SpawnFallingObjects () {
		while (true) {
			while (GameController.instance.gamestate!=GameController.GameState.Play) {
				yield return null;
			}
				

			// Determine Position
			Vector3 spawnPosition = new Vector3 (Random.Range (spawnRangeX[0], spawnRangeX[1]), maxSpawnHeight, Random.Range (spawnRangeZ[0], spawnRangeZ[1]));
			// Snap to tiled position
			spawnPosition.x = Mathf.Round(spawnPosition.x / TiledFloor.tileWidth) * TiledFloor.tileWidth;
			spawnPosition.z = Mathf.Round(spawnPosition.z / TiledFloor.tileHeight) * TiledFloor.tileHeight;
			// Offset position a certain distance above ground below that position
			RaycastHit hit;
			bool raycast = RaycastUntilTerrain(spawnPosition, Vector3.down, out hit, maxSpawnHeight);
			if (!raycast) {
				// Debug.LogError ("Oops! An object spawn region is hovering over the void! Spawning object at height "+maxSpawnHeight);
			} else {
				spawnPosition.y = hit.point.y + spawnYOffset;
			}
			SpawnObject ();

			yield return new WaitForSeconds (spawnInterval);
		}
	}

	private void SpawnObject() {
		// Determine Position
		Vector3 spawnPosition = new Vector3 (Random.Range (spawnRangeX[0], spawnRangeX[1]), maxSpawnHeight, Random.Range (spawnRangeZ[0], spawnRangeZ[1]));
		// Snap to tiled position
		spawnPosition.x = Mathf.Round(spawnPosition.x / TiledFloor.tileWidth) * TiledFloor.tileWidth;
		spawnPosition.z = Mathf.Round(spawnPosition.z / TiledFloor.tileHeight) * TiledFloor.tileHeight;
		// Offset position a certain distance above ground below that position
		RaycastHit hit;
		bool raycast = RaycastUntilTerrain(spawnPosition, Vector3.down, out hit, maxSpawnHeight);
		if (!raycast) {
			// Debug.LogError ("Oops! An object spawn region is hovering over the void! Spawning object at height "+maxSpawnHeight);
		} else {
			spawnPosition.y = hit.point.y + spawnYOffset;
		}

		// Spawn the object
		GameObject objectToSpawn = GetObjectToSpawn();
		Quaternion spawnRotation = Quaternion.identity;
		GameObject obj = Instantiate (objectToSpawn, spawnPosition, spawnRotation) as GameObject;
	}

	private GameObject GetObjectToSpawn(int numRetries = 2) {
		GameObject objectToSpawn = fallingObjectList [Random.Range (0, fallingObjectList.Length)];

		// If not retrying (i.e. not cheating to favour needed ingredients), return.
		if (numRetries == 0) {
			return objectToSpawn;
		}

		// We're cheating, so retry for [numRetries] times until we find a needed ingredient.
		IngredientSet cumulativeIngredientSet = OrderController.instance.GetCumulativeIngredientSet ();
		for (int i = 0; i < numRetries; i++) {
			IngredientSet.Ingredients ingredientToSpawn = IngredientSet.StringToIngredient(objectToSpawn.name);
			if (cumulativeIngredientSet.GetCount (ingredientToSpawn) > 0) {
				return objectToSpawn;
			}
			objectToSpawn = fallingObjectList [Random.Range (0, fallingObjectList.Length)];
		}
		return objectToSpawn;
	}

	public static bool RaycastUntilTerrain(Vector3 position, Vector3 direction, out RaycastHit outHit, float maxDistance = Mathf.Infinity) {
		outHit = new RaycastHit();
		RaycastHit[] hits = Physics.RaycastAll (position, direction);
		if (hits.Length == 0) {
			return false;
		}
		float minDist = float.MaxValue;
		bool returnValue = false;
		foreach (RaycastHit hit in hits) {
			if (hit.transform.gameObject.tag == "Terrain" && hit.distance < minDist) {
				outHit = hit;
				minDist = hit.distance;
				returnValue = true;
			}
		}
		return returnValue;
	}
}
