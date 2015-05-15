using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntReactive : ScavengerAnt 
{	
	// Initialization
	protected override void Start() 
	{
		base.Start();
	}

	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvaluateFieldOfView(); //Evaluate view cone
		Move();
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

	protected override void EvaluateFieldOfView()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		List<GameObject> listAux;

		//If we find any sort of food and we are not carrying any, we rotate towards the object
		if (objsInsideCone.TryGetValue("Food", out listAux) && carryingFood) {
				
			for (int i = 0; i < listAux.Count; i++) {

				if (!listAux[i].GetComponent<Food>().Transport) { //If food is not being transported
					transform.LookAt(listAux[i].transform.position); //Pick the first available food object
					proceed = true;
					break;
				}
			}
		}
	}
}