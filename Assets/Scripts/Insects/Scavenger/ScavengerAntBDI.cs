using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntBDI : ScavengerAnt 
{
	// Navigation agent
	private NavMeshAgent navAgent;
	private bool navigation = false;
	private Vector3 eulerAngleVelocity;

	// Beliefs impact
	public float runDistance = 15f;

	// Beliefs	
	public Transform unloadZone = null;
	public Transform labyrinthDoor = null;

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

		guiStyle.fontSize = 8;
		guiStyle.normal.textColor = Color.white;
	}

	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvalBeliefs();
		EvalDesires();
		Move();
	}

	// Called per event
	protected void OnGUI() {
	
		if (intention != null) {

			worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
			rect.Set(worldToScreen.x, Screen.height - worldToScreen.y, 80, 20);
			GUI.Label(rect, intention.Type.ToString() + " - " + Mathf.Round(energy), guiStyle);
		}
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
			if (Utils.ReachedDestination(navAgent, this.gameObject)) {
				intention = null;
				navigation = false;
			}

			// Carry food
			if(carryingFood) {
				food.transform.position = transform.position + new Vector3(0.0f, 1.5f, 0.0f);
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
			if (!speedRunning) {
				speedRunning = true;
				speed += speedRunIncrement;
			}

			Unload(); // Unload food
			
			if (navAgent.enabled) {
				navigation = false;
				navAgent.ResetPath();
				navAgent.enabled = false;
			}

			if(intention == null || intention.IntentionDest == null || 
			   intention.Type != DesireType.Run || Vector3.Distance(transform.position, intention.IntentionDest.position) > runDistance) {
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
		// Clear desires
		desires.Clear();

		// Gather information about beliefs: Food, Ants (Friends) and Enemies
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		objsInsideCone.TryGetValue("Food", out foood);
		objsInsideCone.TryGetValue("Ant", out friends);
		objsInsideCone.TryGetValue("Enemy", out enemies);
		objsInsideCone.TryGetValue("Labyrinth", out extras);

		// Gather information about labyrinthEntrance and UnloadZone
		if (extras != null && (labyrinthDoor == null || unloadZone == null)) {

			foreach (GameObject extra in extras) {

				if (extra.name.Contains("labyrinth_exit")) {
					labyrinthDoor = extra.transform;
				}
				else if (extra.name.Contains("queen_wall")) {
					unloadZone = extra.transform;
				}
			}
		}

		// Calculate danger and confidence
		int danger = 0;
		int confidence = 0;

		if (friends != null) {
			confidence = friends.Count;
		}

		if (enemies != null) {
			danger = enemies.Count;
			enemies.OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude); //Order by proximity
			desires.Add(new Desire(DesireType.Run, enemies[0].transform, confidence, danger, DesirePriorities.RUN_PRIORITY)); //Add run belief
		}

		// If we are carrying food
		if (carryingFood) {

			if(Vector3.Distance(transform.position, labyrinthDoor.position) > 1f && !insideLabyrinth) {
				desires.Add(new Desire(DesireType.DropFood, labyrinthDoor, danger, confidence, DesirePriorities.DROP_FOOD_PRIORITY));
			} else {
				desires.Add(new Desire(DesireType.DropFood, unloadZone, danger, confidence, DesirePriorities.DROP_FOOD_PRIORITY));
			}
		}

		// If we see some food and we are not carrying any
		else {
			if (foood != null) {

				foreach (GameObject obj in foood) {
					//If food is not being transported
					if (!obj.GetComponent<Food>().Transport) {
						desires.Add(new Desire(DesireType.CatchFood, obj.transform, danger, confidence, DesirePriorities.CATCH_FOOD_PRIORITY)); //Add catch food belief
						break;
					}
				}
			}
		}

		// Default beliefs (Exit or FindFood)  
		if (desires.Count == 0) {

			if(insideLabyrinth) {
				desires.Add(new Desire(DesireType.Exit, labyrinthDoor, danger, confidence, DesirePriorities.EXIT_PRIORITY));
			} else {
				desires.Add(new Desire(DesireType.FindFood, null, danger, confidence, DesirePriorities.FIND_FOOD_PRIORITY));
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

			futureIntention = new Intention(bestDesire);

			if (intention == null || futureIntention.IntentionValue > intention.IntentionValue || (futureIntention.Type == intention.Type)) {

				//Update current intention
				intention = futureIntention;

				switch (intention.Type) {
				case DesireType.FindFood:
					//Just do stuff (Reactive behaviour)
					break;
				case DesireType.Run:
					transform.rotation = Quaternion.Inverse(transform.rotation); //Rotate backwards
					run = true;
					break;
				default : //CatchFood, DropFood and Exit all involve the same behaviour (navigation)
					if(intention.IntentionDest != null) {
						navigation = true; 
					}
					break;
				}
			}
		}
	}
}