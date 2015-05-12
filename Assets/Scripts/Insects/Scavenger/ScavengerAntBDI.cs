﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntBDI : ScavengerAnt 
{
	public float desireValue = -1000f;

	// Navigation agent
	private NavMeshAgent navAgent;
	private bool navigation = false;
	private Vector3 eulerAngleVelocity;

	// Beliefs impact
	public int enemyImpact = 10;
	public int friendsImpact = 5;

	// Beliefs	
	public Transform unloadZone;
	public Transform labyrinthDoor;

	private List<GameObject> foood;
	private List<GameObject> friends;
	private List<GameObject> enemies;
	
	// Intentions and desires
	private Intention intention;
	private List<Desire> desires;


	// Initialization
	protected override void Start() 
	{
		base.Start();

		desires = new List<Desire>();
		eulerAngleVelocity = Vector3.zero;
		navAgent = GetComponent<NavMeshAgent>();
	}

	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvalBeliefs();
		EvalDesires();
		Move();
	}
	
	// Reactors
	protected override void Move() 
	{
		/*
		 * Path finding navigation:
		 * 
		 * 1º situation - occurs when the agent (scavenger ant), just 
		 * spawned inside the labyrinth and needs to find its way out.
		 * 
		 * 2º situation - occurs when the agent carries some piece of
		 * food and needs to get back to the labyrinth entrance and
		 * unload it.
		 * 
		 */
		if (navigation) {

			// Re-enable navmeshAgent
			navAgent.enabled = true;

			navAgent.destination = intention.IntentionDest.transform.position; //Apply destination to navAgent
			Utils.SmoothNavigationRot(navAgent, rigidBody, eulerAngleVelocity);
		}

		/*
		 *  Colision situation:
		 *
		 *	This situation occurs when the agent colides with some obstacle,
		 *	i.e walls, enemies, ants, and provides a random rotation across
		 *	three different values (90º, -90º, 180º). This allows the agent
		 *	to proceed with its actions.
		 * 
		 */
		else if (collided && !proceed) {

			Rotate(randomMin);
			collided = false;
		}

		/*
		 * Proceed situation:
		 * 
		 * This situation occurs when the agent has an intention i.e
		 * grabbing food, run...and provides the correct orientation
		 * for the task fulfilment, rotating the agent and moving it.
		 * This relies on the view cone evaluation.
		 * 
		 */
		else if (!collided && proceed) {

			RotateTowards(intention.IntentionDest);
			base.Move(); //Move forward
			proceed = false;
		} 

		/*
		 * Generic move situation:
		 * 
		 * This happens when the agent (scavenger ant), doesn't really
		 * know its purpose i.e, no beliefs exist. The agent randomly
		 * navigates across the map using a randomMax value for rotation.
		 * 
		 */ 
		else {			
			Rotate(randomMax);
			base.Move();

		}
	}

	// BDI Evaluators	
	protected void EvalBeliefs()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();

		// Clear desires
		if(desires != null) 
			desires.Clear();

		// Gather information about beliefs: Food, Ants (Friends) and Enemies
		objsInsideCone.TryGetValue("Food", out foood);
		objsInsideCone.TryGetValue("Ant", out friends);
		objsInsideCone.TryGetValue("Enemy", out enemies);

		// Calculate danger and confidence
		float danger = 0f;
		float confidence = 0f;

		if (enemies != null) {
			danger = enemies.Count * enemyImpact;
		}

		if (friends != null) {
			confidence = friends.Count * friendsImpact;
		}

		// If we are carrying food
		if (CarryingFood()) {
			desires.Add(new Desire(DesireType.DropFood, unloadZone, danger, confidence));
		}

		// If we see some food and we are not carrying any
		if (foood != null && !CarryingFood()) {

			foreach (GameObject obj in foood) {
				//If food is not being transported
				if (!obj.GetComponent<Food>().Transport) {
					desires.Add(new Desire(DesireType.CatchFood, obj.transform, danger, confidence)); //Add catch food belief
				}
			}
		}

		if (enemies != null) { //Nearby enemies

			foreach (GameObject obj in enemies) {
				desires.Add(new Desire(DesireType.Run, obj.transform, danger, confidence)); //Add run belief
			}
		}

		// Default belief (FindFood)  
		if (desires.Count == 0) {

			if(insideLabyrinth) {
				desires.Add(new Desire(DesireType.FindFood, labyrinthDoor, danger, confidence));
			}
			else {
				desires.Add(new Desire(DesireType.FindFood, null, danger, confidence));
				//TODO: Add reactive agent behaviour in this situation? 
			}
		}
	}

	private void EvalDesires()
	{
		Desire bestDesire = null;
		Intention futureIntention = null;

		//Get best desire
		foreach (Desire desire in desires) {

			if (desire.DesireValue >= desireValue) {
				bestDesire = desire;
				desireValue = desire.DesireValue;
			}
		}

		if (bestDesire != null) {

			futureIntention = new Intention(bestDesire);
		
			if (intention == null || futureIntention.IntentionValue >= intention.IntentionValue) {

				//Update current intention
				intention = futureIntention;

				switch (intention.Type) {
				case DesireType.FindFood:

					if(intention.IntentionDest != null) { 
						navigation = true; 

					} else {

						//Deactivate navMeshAgent
						if(navAgent.enabled) {
							navAgent.ResetPath();
							navAgent.enabled = false;
						}

						navigation = false;
					}
					break;
				case DesireType.CatchFood:
					proceed = true;
					break;
				case DesireType.DropFood:
					navAgent.destination = unloadZone.transform.position;
					break;
				case DesireType.Run:
					transform.Rotate(0.0f, 180.0f, 0.0f); //Inverse direction and run
					run = true;
					break;
				}
			}
		}
	}
}