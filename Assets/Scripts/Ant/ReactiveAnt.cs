using UnityEngine;
using System.Collections;

public class ReactiveAnt : MonoBehaviour {

	//Constant variables
	public static readonly int SPEED_MAX = 20;
	public static readonly float ENERGY_MAX = 10.0f;

	public static readonly int BASE_ROTATE_MAX = 3;
	public static readonly int LOOP_ROTATE_MAX = 600;

	//Class variables
	private Rigidbody rb;
	private GameObject food;
	private float speed = SPEED_MAX;
	private float energy = ENERGY_MAX;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}

	//Ant cycle
	void FixedUpdate() {
		Move();
		Rotate(LOOP_ROTATE_MAX);
		Energy(-0.001f); //Lose a bit of energy every frame
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

		//UnloadZone?
		if (collider.gameObject.name.Contains ("UnloadZone")) {

			if(CarryingFood()){

				//Gather energy before unloading the object
				Energy(10);

				food.GetComponent<Rigidbody>().AddForce(rb.transform.forward * 150.0f);
				food = null;
			}
			Rotate(BASE_ROTATE_MAX);
		}
		//Door? (Queen Door and Labyrinth
		if (collider.gameObject.name.Contains ("Door")) {
			Rotate(BASE_ROTATE_MAX);
		}
	}

	void OnCollisionEnter(Collision collision) {

		//Wall? (Map Boundaries and Labyrinth Walls)
		if (collision.gameObject.name.Contains("Wall")) {
			Rotate(BASE_ROTATE_MAX);
		}
		//CarryFood?
		if (collision.gameObject.name.Contains("Food") && !CarryingFood()) {
				food = collision.gameObject;
		}
		//Ant?
		if (collision.gameObject.name.Contains("Ant")) {
			Rotate(BASE_ROTATE_MAX);
		}
	}

	//Reactors
	private void Move() {

		//Move forward
		rb.MovePosition(rb.transform.position + rb.transform.forward * speed * Time.deltaTime);

		//If carrying food move it as well
		if (CarryingFood()) {
			food.transform.position = rb.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		}
	}

	private void Rotate(int max) {
			 
		switch (Random.Range (0, max)) {
		case 0:
			rb.transform.Rotate (0.0f, 90.0f, 0.0f);
			break;
		case 1:
			rb.transform.Rotate (0.0f, 180.0f, 0.0f);
			break;
		case 2:
			rb.transform.Rotate (0.0f, -180.0f, 0.0f);
			break;
		case 3:
			rb.transform.Rotate (0.0f, -90.0f, 0.0f);
			break;
		}
	}

	public void Energy(float increment) {
		
		energy += increment;

		if(energy > ENERGY_MAX) { //Cap energy to max
			energy = ENERGY_MAX;
		}
		else if (energy < 0.0f) { //Kill if no energy is available
			Destroy (this.gameObject);
		}
	}
}