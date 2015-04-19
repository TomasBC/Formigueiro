using UnityEngine;
using System.Collections;

public class ReactiveQueen : MonoBehaviour {

	//Constant variables
	public static readonly int SPEED_MAX = 2;
	public static readonly float ENERGY_MAX = 100.0f;

	public static readonly int BASE_ROTATE_MAX = 3;
	public static readonly int LOOP_ROTATE_MAX = 200;
	
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
		Rotate(LOOP_ROTATE_MAX);
		Energy(-0.01f); //Lose a bit of energy every frame
	}

	//Sensors	
	void OnTriggerEnter(Collider collider){

		//QueenDoor?
		if (collider.gameObject.name.Contains("Door")) {
			Rotate(BASE_ROTATE_MAX);
		}
	}

	void OnCollisionEnter(Collision collision) {
		
		//Wall?
		if (collision.gameObject.name.Contains("Wall")) {
			Rotate(BASE_ROTATE_MAX);
		}
		//Food?
		if (collision.gameObject.name.Contains("Food")) {

			if(collision.gameObject.name.Contains("Orange")){
				Energy(40.0f);
			}
			Destroy(collision.gameObject);
		}
		//Ant?
		if (collision.gameObject.name.Contains("Ant")) {
			Rotate(BASE_ROTATE_MAX);
		}
	}
	
	//Reactors
	private void Move() {
		
		//Move forward
		rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
	}
	
	private void Rotate(int max) {

		switch (Random.Range (0, max)) {
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