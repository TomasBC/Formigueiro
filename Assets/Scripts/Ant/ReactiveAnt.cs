using UnityEngine;
using System.Collections;

public class ReactiveAnt : MonoBehaviour {

	private Rigidbody rb;
	private GameObject food;

	public float speed;
	private int energy = 100;
	private bool carrying = false;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		Move();
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
		if (collider.gameObject.name.Contains("Orange")) {
			food = collider.gameObject;
			carrying = true;
		}

	}

	void OnCollisionEnter(Collision collision) {

		//Wall collision
		if (collision.gameObject.name.Contains("Wall")) {
			Rotate();
		}
		if (collision.gameObject.name.Contains ("UnloadZone")) {
			carrying = false;
			food.transform.position = food.transform.position - food.transform.forward* 10;
			food = null;
			rb.gameObject.transform.Rotate(0, 180f, 0);

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

		switch (Random.Range (0, 3)) {
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
		}
	}
}