﻿using UnityEngine;
using System.Collections;

public class ReactiveEnemy : MonoBehaviour {
	//Constant variables
	public static readonly int SPEED_MAX = 20;
	public static readonly float ENERGY_MAX = 10.0f;
	
	public static readonly int BASE_ROTATE_MAX = 3;
	public static readonly int LOOP_ROTATE_MAX = 600;

	private Rigidbody rb;
	private float speed = SPEED_MAX;
	private float energy = ENERGY_MAX;

	void Start(){
		rb = GetComponent<Rigidbody> ();
	}

	//Enemy cycle
	void FixedUpdate() {
		Move ();
		Energy(-0.001f); //Lose a bit of energy every frame
	}

	//Reactors
	private void Move() {
		rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
		Rotate(LOOP_ROTATE_MAX);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name.Contains ("Ant")) {
			collision.gameObject.GetComponent<ReactiveAnt>().Energy(-5.0f);
		}
		if (collision.gameObject.name.Contains("Wall")) {
			Rotate(BASE_ROTATE_MAX);
		}
		if (collision.gameObject.name.Contains("Spider")) {
			Rotate(BASE_ROTATE_MAX);
		}
	}

	private void Rotate(int max) {

		switch (Random.Range (0, max)) {
		case 0:
			rb.transform.Rotate (0.0f, -180.0f, 0.0f);
			break;
		case 1:
			rb.transform.Rotate (0.0f, 180.0f, 0.0f);
			break;
		case 2:
			rb.transform.Rotate (0.0f, -90.0f, 0.0f);
			break;
		case 3:
			rb.transform.Rotate (0.0f, 90.0f, 0.0f);
			break;
		}
	}

	public void Energy(float e) {

		energy += e;

		if(energy > ENERGY_MAX) { //Cap energy to max
			energy = ENERGY_MAX;
		}else if (energy < 0.0f) { //Kill if no energy is available
			Destroy (this.gameObject);
		}
	}

}
