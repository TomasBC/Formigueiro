using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SoldierBDI : SoldierAnt 
{
	// Navigation agent
	private NavMeshAgent navAgent;
	private bool navigation = false;
	private Vector3 eulerAngleVelocity;
	
	// Beliefs impact
	public int enemyImpact = 10;
	public int friendsImpact = 5;
	public float runDistance = 10f;
	
	private List<GameObject> ants;
	private List<GameObject> enemies;
	private List<GameObject> friends;
	private List<GameObject> aux;
	
	// Intentions and desires
	private Intention intention;
	private List<Desire> desires;
	
	private bool run = false;
	private bool attacking = false;
	
	// Called per event
	protected void OnGUI() {
		
		if (intention != null) {
			
			worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
			rect.Set(worldToScreen.x, Screen.height - worldToScreen.y, 80, 20);
			GUI.Label(rect, intention.Type.ToString(), guiStyle);
		}
	}
	
	// Initialization
	protected override void Start() 
	{
		base.Start();
		
		ants = new List<GameObject>();
		friends = new List<GameObject>();
		
		desires = new List<Desire>();
		eulerAngleVelocity = Vector3.zero;
		
		navAgent = GetComponent<NavMeshAgent>();
		navAgent.enabled = false;
		
		guiStyle.fontSize = 10;
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
		/*
		 * Run situation:
		 * 
		 * This situation occurs when the agent spots an enemy. The agent
		 * starts running for his life and drops the food if transporting
		 * any. It runs for a certain distance and eventually leaves the 
		 * run state.
		 * 
		 */
		
		if (attacking) {
			if(intention != null && intention.Type == DesireType.Attack){
				if(Vector3.Distance(transform.position, intention.IntentionDest.position) > 15) {
					attacking = false;
					intention = null;
					return;
				}
				
				Vector3 targetDir = intention.IntentionDest.transform.position - transform.position;
				float step = speed * Time.deltaTime;
				Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
				transform.rotation = Quaternion.LookRotation(newDir);
				transform.position = Vector3.MoveTowards(transform.position, intention.IntentionDest.transform.position, this.speed*Time.fixedDeltaTime);
				return;
			}
		}
		
		if (navigation) {
			
			// Define navAgent destination
			if (!navAgent.enabled) {
				
				navAgent.enabled = true;
				navAgent.ResetPath();
				
				navAgent.destination = intention.IntentionDest.transform.position;
				Utils.SmoothNavigationRot(navAgent, rigidBody, eulerAngleVelocity);	
			}
			
			// Check if we reached out destination
			if(Utils.ReachedDestination(navAgent, this.gameObject) && !(intention.Type == DesireType.Attack)) {
				
				intention = null;
				navigation = false;
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
	
	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Get active food
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Ant").Concat(GameObject.FindGameObjectsWithTag("Enemy")).ToArray();
		
		return Utils.CheckFieldOfView(gameObject, objs, fieldOfView, longViewDistance, closeViewDistance);
	}
	
	// BDI Evaluators	
	protected void EvalBeliefs()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		
		friends.Clear ();
		ants.Clear ();
		
		// Clear desires
		if (desires != null) {
			desires.Clear ();
		}
		
		// Gather information about beliefs: Food, Ants (Friends) and Enemies
		objsInsideCone.TryGetValue("Ant", out aux);
		objsInsideCone.TryGetValue("Enemy", out enemies);
		
		if (aux != null) {
			foreach (GameObject ant in aux) {
				if (ant.name.Contains ("Soldier"))
					friends.Add (ant);
				else
					ants.Add (ant);
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
		
	/*	if (ants.Count != 0) {
			foreach(GameObject obj in ants)
				desires.Add(new Desire(DesireType.Attack, obj.transform, danger, confidence + DesirePriorities.ATTACK_PRIORITY));
		}*/

		if (enemies != null && enemies.Count != 0) {
			foreach(GameObject obj in ants){
				if(danger > confidence || energy < 20)
					desires.Add(new Desire(DesireType.Run, obj.transform, confidence, danger + DesirePriorities.RUN_PRIORITY)); 
				else
					desires.Add(new Desire(DesireType.Attack, obj.transform, danger, confidence + DesirePriorities.ATTACK_PRIORITY)); 
			}
		}
		
		// Default beliefs (Exit or FindFood)  
		if (desires.Count == 0) {
			desires.Add(new Desire(DesireType.Patrol, null, danger, confidence + DesirePriorities.PATROL_PRIORITY));
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
			
			if (intention == null || 
			    futureIntention.IntentionValue > intention.IntentionValue || 
			    (futureIntention.Type == intention.Type)) {
				
				//Update current intention
				intention = futureIntention;
				
				switch (intention.Type) {
				case DesireType.Attack:
					attacking = true;
					break;
				case DesireType.Patrol:
					//Just do stuff
					break;
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