using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnemyReactive : Insect {

	public float attackPower = 5.0f;

	public float fieldOfView = 90f;
	public float longViewDistance = 20f; 
	public float closeViewDistance = 5f;

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

		base.OnCollisionEnter(collision);

		//Ant?
		if (collision.gameObject.tag.Equals("Ant")) {
			collision.gameObject.GetComponent<Insect>().UpdateEnergy(-attackPower); //Attack
			UpdateEnergy(attackPower * 0.5f); // Gain energy with the attack

			collided = true;
		}
		//Food?
		if (collision.gameObject.tag.Equals("Food")) {
			collided = true;
		}
	}
	
	// Reactors
	protected override void Move() 
	{
		if (!collided && !proceed) {
			base.Move(); //Move forward and rotate max
			Rotate(randomMax);
		} else if (!collided && proceed) {
			base.Move(); //Move forward
			proceed = false;
		} else {
			Rotate(randomMin); //Rotate min
			collided = false;
		}
	}

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Get active ants
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Ant").ToArray();
		Dictionary<string, List<GameObject>> result;

		result = ViewConeController.CheckFieldOfView(this.gameObject, objs, fieldOfView, longViewDistance, closeViewDistance);

		return result;
	}

	protected override void EvaluateFieldOfView()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		List<GameObject> listAux;

		//If we find any ant, we rotate towards it
		if (objsInsideCone.TryGetValue("Ant", out listAux)) {

			RotateTowards(listAux[Random.Range(0, listAux.Count)]); //Randomly pick a ant
			proceed = true;
		}
	}
}