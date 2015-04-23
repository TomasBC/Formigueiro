using UnityEngine;
using System.Collections;

public class Insect : MonoBehaviour 
{
	// Public variables
	public int speed = 20;
	
	public float energy = 100.0f;
	public float maxEnergy = 100.0f;
	public float frameEnergyLoss = -0.001f;

	public int randomMin = 3;
	public int randomMax = 250;

	protected bool collided = false;

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
	
	protected virtual void OnCollisionEnter(Collision collision) 
	{
		//SameInsect? or Wall?
		if (collision.gameObject.tag.Equals(this.gameObject.tag) ||
		    collision.gameObject.name.Contains("wall")) {
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
		switch (Random.Range (0, max)) {
		case 0:
			rigidBody.transform.Rotate(0.0f, 180.0f, 0.0f);
			break;
		case 1:
			rigidBody.transform.Rotate(0.0f, 90.0f, 0.0f);
			break;
		case 2:
			rigidBody.transform.Rotate(0.0f, -90.0f, 0.0f);
			break;
		}
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
			Destroy(this);
		}
	}
}