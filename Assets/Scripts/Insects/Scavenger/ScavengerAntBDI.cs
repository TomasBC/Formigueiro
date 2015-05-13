using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntBDI : ScavengerAnt 
{
	// Navigation agent
	private NavMeshAgent navAgent;
	private bool navigation = false;
	private Vector3 eulerAngleVelocity;

	// Beliefs impact
	public int enemyImpact = 10;
	public int friendsImpact = 5;
	public float runDistance = 10f;

	// Beliefs	
	public Transform unloadZone;
	public Transform labyrinthDoor;

	private List<GameObject> foood;
	private List<GameObject> extras;
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
		navAgent.enabled = false;
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

			// Define navAgent destination
			if (!navAgent.enabled) {

				navAgent.enabled = true;
				navAgent.ResetPath();

				navAgent.destination = intention.IntentionDest.transform.position;
				Utils.SmoothNavigationRot(navAgent, rigidBody, eulerAngleVelocity);	
			}

			// Check if we reached out destination
			if(Utils.ReachedDestination(navAgent, this.gameObject)) {

				intention = null;
				navigation = false;
			}
		
			// Move food along with the agent
			if(CarryingFood()) {
				food.transform.position = transform.position + new Vector3(0.0f, 1.5f, 0.0f);
			}
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
				base.Move();

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
		
			if(navAgent.enabled) {
				navigation = false;

				navAgent.ResetPath();
				navAgent.enabled = false;
			}

			if(Vector3.Distance(transform.position, intention.IntentionDest.position) > 10) {
				run = false;
				intention = null;
			}
		}
	}

	// BDI Evaluators	
	protected void EvalBeliefs()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();

		// Clear desires
		if (desires != null) {
			desires.Clear ();
		}
	
		// Gather information about beliefs: Food, Ants (Friends) and Enemies
		objsInsideCone.TryGetValue("Food", out foood);
		objsInsideCone.TryGetValue("Ant", out friends);
		objsInsideCone.TryGetValue("Enemy", out enemies);
		objsInsideCone.TryGetValue("Labyrinth", out extras);

		// Gather information about labyrinthEntrance and UnloadZone
		if (extras != null && (labyrinthDoor == null || unloadZone == null)) {

			foreach (GameObject extra in extras) {

				if (extra.name.Contains("labyrinth_door")) {
					labyrinthDoor = extra.transform;
				}
				else {
					unloadZone = extra.transform;
				}
			}
		}

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
			desires.Add(new Desire(DesireType.DropFood, unloadZone, danger, confidence + DesirePriorities.DROP_FOOD_PRIORITY));
		}

		// If we see some food and we are not carrying any
		if (foood != null && !CarryingFood()) {

			foreach (GameObject obj in foood) {
				//If food is not being transported
				if (!obj.GetComponent<Food>().Transport) {
					desires.Add(new Desire(DesireType.CatchFood, obj.transform, danger, confidence + DesirePriorities.CATCH_FOOD_PRIORITY)); //Add catch food belief
				}
			}
		}

		if (enemies != null) { //Nearby enemies
			Debug.Log("Entrei");
			foreach (GameObject obj in enemies) {
				desires.Add(new Desire(DesireType.Run, obj.transform, confidence, danger + DesirePriorities.RUN_PRIORITY)); //Add run belief
			}
		}

		// Default beliefs (Exit or FindFood)  
		if (desires.Count == 0) {

			if(insideLabyrinth) {
				desires.Add(new Desire(DesireType.Exit, labyrinthDoor, danger, confidence + DesirePriorities.EXIT_PRIORITY));
			} else {
				desires.Add(new Desire(DesireType.FindFood, null, danger, confidence + DesirePriorities.FIND_FOOD_PRIORITY));
			}
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

			futureIntention = new Intention (bestDesire);
		
			if (intention == null || futureIntention.IntentionValue > intention.IntentionValue || (futureIntention.Type == intention.Type)) {

				//Update current intention
				intention = futureIntention;

				switch (intention.Type) {
				case DesireType.FindFood:
					//Just do stuff
					break;
				case DesireType.Run:
					run = true;
					break;
				default : //CatchFood, DropFood and Exit all involve the same behaviour (navigation)
				
					if (intention.IntentionDest != null) { 
						navigation = true; 
					} 
					break;
				}
			}
		}
		Debug.Log(intention.Type);
	}
}