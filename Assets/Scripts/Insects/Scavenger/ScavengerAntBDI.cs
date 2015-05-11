using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntBDI : ScavengerAnt 
{
	public float desireValue = -1000f;

	public int enemyImpact = 10;
	public int friendsImpact = 5;

	private bool nav = false;
	private Vector3 eulerAngleVelocity;

	private NavMeshAgent navAgent;

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
		if (!nav) {

			if (!collided && !proceed) {
				base.Move (); //Move forward and rotate max
				Rotate (randomMax);
			} else if (!collided && proceed) {
				base.Move (); //Move forward
				proceed = false;
			} else {
				Rotate (randomMin); //Rotate min
				collided = false;
			}
		} else {
			Utils.SmoothNavigationRot(navAgent, rigidBody, eulerAngleVelocity);
		}
	}

	// BDI Evaluators	
	protected void EvalBeliefs()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();

		// Clear desires
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
			desires.Add(new Desire(DesireType.DropFood, null, danger, confidence));
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
		
			if(intention == null) {
				intention = futureIntention;
			}

			Debug.Log(intention.Type);

			if (futureIntention.IntentionValue >= intention.IntentionValue) {

				//Update current intention
				intention = futureIntention;

				switch (intention.Type) {
				case DesireType.FindFood:

					Transform intentionDest = intention.IntentionDest;

					if(intentionDest != null) {
						nav = true;
						navAgent.destination = intentionDest.position;
					}
					break;
				case DesireType.CatchFood:
					RotateTowards(intention.IntentionDest);
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