﻿using UnityEngine;
using System.Collections;

public class WalkingChef : MonoBehaviour{
    public Vector2[] coordinates;

    public Vector3 spawnPoint;

    public int waitTime;

    private float tolerance = 0.1f;
    private float rotationSpeedFactor = 0.2f;
    private int coordPointer = 0;

    private int pauseTime = 15;
    private int decrement = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		if (GameController.instance.gamestate!=GameController.GameState.Play) {
			if (gameObject.GetComponent<Animator> () != null) {
				gameObject.GetComponent<Animator> ().StartPlayback ();
			}
			return;
		}
		if (gameObject.GetComponent<Animator> () != null) {
			gameObject.GetComponent<Animator> ().StopPlayback ();
		}
        if (waitTime <= 0)
        {
            if (waitTime == 0)
            {
                transform.position = spawnPoint;
                waitTime--;
            }
            else {
                //probably set velocity and stuff up here
                Vector3 targetDirection = Vector3.Normalize(new Vector3(coordinates[coordPointer].x - transform.position.x, 0, coordinates[coordPointer].y - transform.position.z));
                transform.forward = Vector3.Lerp(transform.forward, targetDirection, rotationSpeedFactor);

                Rigidbody rb = transform.GetComponent<Rigidbody>();
                rb.velocity = targetDirection * 5f;

                //don't know if I need tolerance
                if (transform.position.x < coordinates[coordPointer].x + tolerance && transform.position.x > coordinates[coordPointer].x - tolerance &&
                    transform.position.z < coordinates[coordPointer].y + tolerance && transform.position.z > coordinates[coordPointer].y - tolerance)
                {
                    coordPointer++;
                    decrement = pauseTime;
                }
                if (coordPointer >= coordinates.Length)
                {
                    coordPointer = 0;
                }
            }
        }
        else
        {
            waitTime--;
        }
    }

	void FixedUpdate() {
		Rigidbody rb = transform.GetComponent<Rigidbody>();
        if (decrement == 0)
        {
            if (GameController.instance.gamestate != GameController.GameState.Play)
            {
                if (!rb.IsSleeping())
                {
                    rb.Sleep();
                }
                return;
            }
            if (rb.IsSleeping())
            {
                rb.WakeUp();
            }
        }
        else
        {
            decrement--;
            if (!rb.IsSleeping())
            {
                rb.Sleep();
            }
            return;
        }
	}
}
