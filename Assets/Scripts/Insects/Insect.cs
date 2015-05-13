using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Insect : MonoBehaviour 
{
	protected bool insideLabyrinth = true;

	// Public variables
	public int speed = 20;

	public float energy = 100.0f;
	public float maxEnergy = 100.0f;
	public float frameEnergyLoss = -0.001f;

	public int randomMin = 4;
	public int randomMax = 100;

	protected bool collided = false;
	protected bool proceed = false;

	// Protected variables
	protected Rigidbody rigidBody;

	// Initialization
	protected virtual void Start() 
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	// Called every fixed framerate frame
	protected virtual void FixedUpdate() 
	{
		UpdateEnergy(frameEnergyLoss);
		Death();
	}

	// Sensors
	protected virtual void OnTriggerEnter(Collider collider) 
	{
		//LabyrinthDoor?
		if (collider.gameObject.tag.Equals("Labyrinth")) {

			//LabyrinthExit?
			if(collider.gameObject.name.Equals("labyrinth_exit")) {
				insideLabyrinth = false;
			} else { //LabyrinthEntrance?
				insideLabyrinth = true;
			}
		}
	}
	
	protected virtual void OnCollisionEnter(Collision collision) 
	{
		//SameInsect? or Wall?
		if (collision.gameObject.tag.Equals(this.gameObject.tag) ||
		    collision.gameObject.tag.Equals("Wall")) {
			collided = true;
		}
	}
	
	// Actuators
	protected virtual void Move() 
	{
		rigidBody.MovePosition(rigidBody.transform.position + rigidBody.transform.forward * speed * Time.deltaTime);
	}

	protected void Rotate(int max) 
	{	
		//Rotate within a certain range
		switch (Random.Range (0, max)) {
		case 0:
			transform.Rotate(0.0f, 180.0f, 0.0f);
			break;
		case 1:
			transform.Rotate(0.0f, 90.0f, 0.0f);
			break;
		case 2:
			transform.Rotate(0.0f, -90.0f, 0.0f);
			break;
		case 3:
			transform.rotation = Quaternion.identity; //Reset rotation
			break; 
		}
	}

	protected void RotateTowards(Transform targetPos) 
	{
		Vector3 targetPosition = transform.position;
		targetPosition.y = 0.0f;

		//Rotate towards the target
		transform.LookAt(targetPosition);
	}

	public void UpdateEnergy(float increment)
	{	
		energy += increment;

		if (energy > maxEnergy) { //Cap energy to max
			energy = maxEnergy;
		}
	}

	protected void Death() 
	{
		if (energy <= 0.0f) {
			Destroy(gameObject);
		}
	}

	// Function headers to be overrided by subclasses
	protected virtual Dictionary<string, List<GameObject>> CheckFieldOfView()  { return null; }
	protected virtual void EvaluateFieldOfView() {}
}