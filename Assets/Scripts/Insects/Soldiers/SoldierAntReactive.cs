using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierAntReactive : SoldierAnt
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
		EvaluateFieldOfView();
		Move();
	}

	// Reactors
	protected override void Move() 
	{
		if(!collided && !proceed) {
			base.Move(); //Move forward and rotate max
			Rotate(randomMax);
		} else if(!collided && proceed) {
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
		
		//If we find any sort of enemy, we rotate towards it
		if(objsInsideCone.TryGetValue("Enemy", out listAux)) {	
			transform.LookAt(listAux[Random.Range(0, listAux.Count)].transform.position); //Randomly pick an enemy
			proceed = true;
		}
	}
}