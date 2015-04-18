using UnityEngine;
using System.Collections;

public class ReactiveAnt : MonoBehaviour {

	private Rigidbody rb;

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

	void OnCollisionEnter(Collision collision) {
		//if (collision.gameObject.tag == "") {} //Wall, Enemy...
	}

	//Reactors
	private void Move() {
		rb.AddForce(transform.forward * speed * Time.deltaTime);
	}
}