using UnityEngine;
using System.Collections;

public class ReactiveAnt : MonoBehaviour {

	private Rigidbody rb;

	public float speed;
	private int energy = 100;
	private bool carrying = false;
	private GameObject food;

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
		if (collider.gameObject.name.Contains("Orange")) {
			food = collider.gameObject;
			carrying = true;
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name.Contains("Wall")) {
			RotateAnt();
		} //Wall, Enemy... //Wall, Enemy...
	}

	//Reactors
	private void Move() {
		Vector3 pos;
		rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
		if (carrying) {
			pos = transform.position + new Vector3(0f,1f,0f);
			food.transform.position = pos;
		}
	}

	private void RotateAnt(){
		int rot = Random.Range (0, 3);
		switch (rot) {
		case 0 : rb.gameObject.transform.Rotate(0, -90f, 0); break;
		case 1 : rb.gameObject.transform.Rotate(0, 90f, 0); break;
		case 2 : rb.gameObject.transform.Rotate(0, 180f, 0); break;
		case 3 : rb.gameObject.transform.Rotate(0, -180f, 0); break;
		}
	}
}