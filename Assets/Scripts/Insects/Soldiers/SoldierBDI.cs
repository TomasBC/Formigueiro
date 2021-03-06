﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SoldierBDI : SoldierAnt 
{
	// Beliefs impact
	private List<GameObject> ants;
	private List<GameObject> enemies;
	private List<GameObject> friends;
	private List<GameObject> scavengers;
	
	// Intentions and desires
	private Intention intention;
	private List<Desire> desires;

	// Flags
	private bool run = false;
	private bool follow = false;
	private bool attack = false;
	
	// Called per event
	protected void OnGUI() {
		
		if (intention != null) {
			
			worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
			rect.Set(worldToScreen.x, Screen.height - worldToScreen.y, 80, 20);
			GUI.Label(rect, intention.Type.ToString() + " - " + Mathf.Round(energy), guiStyle);
		}
	}
	
	// Initialization
	protected override void Start() 
	{
		base.Start();

		desires = new List<Desire>();
		friends = new List<GameObject>();
		scavengers = new List<GameObject>();

		guiStyle.fontSize = 8;
		guiStyle.normal.textColor = Color.blue;
	}
	
	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvalBeliefs();
		EvalDesires();
		Move();
	}

	// Sensors
	protected void OnCollisionStay(Collision collision)
	{
		//Enemy?
		if (collision.gameObject.tag.Equals("Enemy")) {
			Debug.Log (collision.gameObject.GetComponent<EnemyBDI>().attackPower);
			UpdateEnergy(-(collision.gameObject.GetComponent<EnemyBDI>().attackPower)); //Lose health
		}
	}
	
	// Reactors
	protected override void Move() 
	{
		/*
		 * Attack/Follow situation:
		 * 
		 * This situation occurs when the agent spots an enemy. The agent
		 * starts an attack against the enemy. This also works for follow 
		 * where a similar behaviour is triggered.
		 * 
		 */
		if (attack || follow) {

			if (intention == null || intention.IntentionDest == null) {
				follow = false;
				attack = false;
				intention = null;
			}	
			else if (follow && Vector3.Distance(transform.position, intention.IntentionDest.position) < 5f) {
				/* Keep a certain distance when following or attacking */ 
			}
			else {
				transform.LookAt(intention.IntentionDest.transform.position);
				transform.position = Vector3.MoveTowards(transform.position, intention.IntentionDest.transform.position, 0.6f * this.speed * Time.fixedDeltaTime);
			}
		}
		/*
		 *  Collision situation:
		 *
		 *	This situation occurs when the agent collides with some obstacle,
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
		/*
		 * Run situation:
		 * 
		 * This situation occurs when the agent spots an enemy. The agent
		 * starts running for his life and drops the food if transporting
		 * any. It runs for a certain distance and eventually leaves the 
		 * run state.
		 * 
		 */
		if (run) {

			// Increment speed
			if(!speedRunning) {
				speedRunning = true;
				speed += speedRunIncrement;
			}

			if(intention == null || intention.IntentionDest == null || intention.Type != DesireType.Run || 
			   Vector3.Distance(transform.position, intention.IntentionDest.position) > 15f) {

				speedRunning = false;
				speed -= speedRunIncrement;
				
				run = false;
				intention = null;
			}
		}
	}

	// BDI Evaluators	
	protected void EvalBeliefs()
	{
		// Clear
		desires.Clear();
		friends.Clear();
		scavengers.Clear();

		// Gather information about beliefs: Ants (Friends) and Enemies
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		objsInsideCone.TryGetValue("Ant", out ants);
		objsInsideCone.TryGetValue("Enemy", out enemies);

		// Calculate danger and confidence
		float danger = 0f;
		float confidence = 0f;

		if (enemies != null) {

			// 1vs1 situation
			if(enemies.Count > 1) {
				danger = enemies.Count;
			}
		}

		if (ants != null) {

			foreach (GameObject ant in ants) {
				if (ant.name.Contains("soldier")) {
					friends.Add(ant);
				} else {
					scavengers.Add(ant);
				}
			}
			confidence = friends.Count;

			//Order by proximity and follow the closer one
			if(scavengers.Count > 0) {
				scavengers.OrderBy(scavenger => (scavenger.transform.position - transform.position).sqrMagnitude); 
				desires.Add(new Desire(DesireType.Follow, scavengers[0].transform, danger + (0.2f * maxEnergy), confidence + energy, DesirePriorities.FOLLOW_PRIORITY));
			}
		}

		if (enemies != null) {
			//Order by proximity and run/attack the closer one
			enemies.OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude);
			desires.Add(new Desire(DesireType.Attack, enemies[0].transform, danger + (0.2f * maxEnergy), confidence + energy, DesirePriorities.ATTACK_PRIORITY));
			desires.Add(new Desire(DesireType.Run, enemies[0].transform, confidence + (0.2f * maxEnergy), danger + energy, DesirePriorities.RUN_PRIORITY));
		}
		// Default beliefs (Exit or FindFood)  
		if (desires.Count == 0) {
			desires.Add(new Desire(DesireType.Patrol, null, danger, confidence, DesirePriorities.PATROL_PRIORITY));
		}
	}
	
	private void EvalDesires()
	{
		float desireValue = -1000f;
		
		Desire bestDesire = null;
		Intention futureIntention = null;
		
		//Get best desire
		foreach (Desire desire in desires) {
			if (desire.DesireValue > desireValue) {
				bestDesire = desire;
				desireValue = desire.DesireValue;
			}
		}
		
		if (bestDesire != null) {

			futureIntention = new Intention(bestDesire);

			if (intention == null || futureIntention.IntentionValue > intention.IntentionValue || futureIntention.Type == intention.Type) {
				
				//Update current intention
				intention = futureIntention;

				switch (intention.Type) {
				case DesireType.Attack:
					attack = true;
					follow = false;
					break;
				case DesireType.Follow:
					follow = true;
					attack = false;
					break;
				case DesireType.Run:
					transform.rotation = Quaternion.Inverse(transform.rotation); //Rotate backwards
					run = true;
					follow = false;
					attack = false;
					break;
				case DesireType.Patrol:
					run = false;
					follow = false;
					attack = false;
					break;
				}
			}
		}
	}
}