using UnityEngine;
using System.Collections;

public class ReactiveAnt : MonoBehaviour {

	//Constant variables
	public static readonly int SPEED_MAX = 4;
	public static readonly float ENERGY_MAX = 100.0f;

	//Class variables
	private Rigidbody rb;
	private GameObject food;
	
	private float speed = SPEED_MAX;
	private float energy = ENERGY_MAX;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate() {
		Move();
		Energy(-0.01f); //Lose a bit of energy every frame
	}
	
	//Sensors
	public bool CarryingFood() {

		if (food != null) {
			return true;
		} else {
			return false;
		}
	}

	void OnTriggerEnter(Collider collider){

		//Unload food
		if (collider.gameObject.name.Contains ("UnloadZone")) {

			if(CarryingFood()){

				//Gather energy before unloading the object
				Energy(10);

				food.GetComponent<Rigidbody>().AddForce(rb.transform.forward * 150.0f);
				food = null;
			}
			Rotate();
		}
	}

	void OnCollisionEnter(Collision collision) {

		//Wall collision
		if (collision.gameObject.name.Contains("Wall")) {
			Rotate();
		}
		//Carry food
		if (collision.gameObject.name.Contains("Food") && !CarryingFood()) {
				food = collision.gameObject;
		}
	}

	//Reactors
	private void Move() {

		//Move forward
		rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

		//If carrying food move it as well
		if (CarryingFood()) {
			food.transform.position = rb.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		}
	}

	private void Rotate() {

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

	public void Energy(float energy) {

		//Energy below max
		if (energy < ENERGY_MAX) {
			this.energy += energy;

			if(energy > ENERGY_MAX) { //Cap energy to max
				energy = ENERGY_MAX;
			}
		} else if (energy < 0.0f) { //Kill if no energy is available
			Destroy (this);
		}
	}
}