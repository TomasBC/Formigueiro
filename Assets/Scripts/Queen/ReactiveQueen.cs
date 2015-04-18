using UnityEngine;
using System.Collections;

public class ReactiveQueen : MonoBehaviour {

	// Use this for initialization
	private Rigidbody rb;
	private GameObject food;
	
	public float speed;
	private float energy = 100;
	
	void Start() {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		Move();
		energy -= 0.01f;
		if (energy < 0)
			Destroy (this);
	}
	
	public void Attacked(int value) {
		energy -= value;
		if (energy < 0)
			Destroy(this);
	}
	
	//Sensors	
	
	void OnTriggerEnter(Collider collider){
		
		//Load food
		if(collider.gameObject.name.Equals("QueenDoor1"))
			rb.transform.Rotate (0.0f, 180.0f, 0.0f);

		if(collider.gameObject.name.Equals("QueenDoor"))
			rb.transform.Rotate (0.0f, 90.0f, 0.0f);
		
	}
	
	void OnCollisionEnter(Collision collision) {
		
		//Wall collision
		if (collision.gameObject.name.Contains("Wall")) {
			Rotate();
		}

		if (collision.gameObject.name.Contains("Food")) {
			if(collision.gameObject.name.Contains("Orange")){
				if(energy < 100){
					energy += 40;
				}
			}
			Destroy(collision.gameObject);
		}
	}
	
	//Reactors
	private void Move() {
		
		//Move forward
		rb.MovePosition (transform.position + transform.forward * speed * Time.deltaTime);
	}
	
	private void Rotate(){
		float y = Random.Range (0f, 360f);
		rb.transform.Rotate (0.0f, y, 0.0f);
	/*	switch (Random.Range (0, 3)) {
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
