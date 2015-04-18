﻿using UnityEngine;
using System.Collections;

public class ReactiveAnt : MonoBehaviour {

	private Rigidbody rb;
	private GameObject food;

	public float speed;
	private float energy = 100;
	private bool carrying = false;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		Move();
		energy -= 0.01f;
	}

	public void Attacked(int value) {
		energy -= value;
		if (energy < 0)
			Destroy(this);
	}

	//Sensors
	public bool CarryingFood() {
		return carrying;
	}

	void OnTriggerEnter(Collider collider){

		//Load food
		if (collider.gameObject.name.Contains ("UnloadZone")) {
			if(carrying){
				if(energy < 100)
					energy += 10;
				carrying = false;	
				food.GetComponent<Rigidbody>().AddForce(rb.transform.forward * 150);
				food = null;
				rb.gameObject.transform.Rotate(0, 180f, 0);
			}else
				rb.gameObject.transform.Rotate(0, 180f, 0);
		}

	}

	void OnCollisionEnter(Collision collision) {

		//Wall collision
		if (collision.gameObject.name.Contains("Wall")) {
			Rotate();
		}
		if (collision.gameObject.name.Contains("Food") && !carrying) {
			food = collision.gameObject;
			carrying = true;
		}
	}

	//Reactors
	private void Move() {

		//Move forward
		rb.MovePosition (transform.position + transform.forward * speed * Time.deltaTime);
		//If carrying food move it as well
		if (CarryingFood()) {
			Vector3 pos = transform.position + new Vector3(0f,1f,0f);
			food.transform.position = pos; 
		}
	}

	private void Rotate(){

		float y = Random.Range (0f, 360f);
		rb.transform.Rotate (0.0f, y, 0.0f);


		/*switch (Random.Range (0, 3)) {
		case 0:
			rb.transform.Rotate (0.0f, -90.0f, 0.0f);
			break;
		case 1:
			rb.transform.Rotate (0.0f, 90.0f, 0.0f);
			break;
		case 2:
			rb.transform.Rotate (0.0f, 180.0f, 0.0f);
			break;
		case 3:
			rb.transform.Rotate (0.0f, -180.0f, 0.0f);
			break;
		}*/
	}
}