using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Insect 
{
	public float attackPower;
	public float fieldOfView = 90f;
	public float longViewDistance = 25f; 

	// Initialization
	protected override void Start() 
	{
		base.Start();
	}
	
	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvaluateFieldOfView();
		Move();
	}

	// Sensors
	protected override void OnCollisionEnter(Collision collision) {
		
		base.OnCollisionEnter (collision);

		//Food?
		if (collision.gameObject.tag.Equals("Food")) {
			collided = true;
		}
	}

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Get active food
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Ant").Concat
						   (GameObject.FindGameObjectsWithTag("Enemy")).ToArray();
		
		return Utils.CheckFieldOfView(gameObject, objs, fieldOfView, longViewDistance);
	}
}