using UnityEngine;
using System.Collections;

public class ReactiveAnt : MonoBehaviour {
	private bool carrying = false;
	private int energy = 100;
	public GameObject ant;
	public float speed = 1;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		move ();
	}

	public void attacked(int value){
		energy -= value;
		if (energy < 0)
			Destroy (ant);
	}

	//Sensors
	public bool carryingFood(){
		return carrying;
	}

	void OnTriggerEnter(Collider other){
		Destroy (other.gameObject);
	}

	//Reactors
	private void move(){
		//Vector3 fw = ant.transform.position + ant.transform.forward * 0.3;
		ant.transform.position += speed * ant.transform.forward * (float)Time.deltaTime;
	}
}
