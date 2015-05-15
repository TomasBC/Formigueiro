using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils : MonoBehaviour 
{
	public static int FindGameObjectsByName(string name, string tag)
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
		int count=0;
		for(int i = 0; i<objs.Length; i++){
			if(objs[i].name.Contains(name)){
				count++;
			}
		}
		return count;
	}

	// View cone related methods
	public static bool CheckIfInsideCone(GameObject obj, GameObject objToCompareTo, float fieldOfView, float longViewDistance) 
	{
		RaycastHit hit; //Out hit
		Vector3 rayDirection = objToCompareTo.transform.position - obj.transform.position;

		// Detect if objToCompareTo is within the field of view
		if(Vector3.Angle(rayDirection, obj.transform.forward) < fieldOfView * 0.5f) {

			//Shoot ray with a slight offset
			Vector3 rayOrigin = obj.transform.position + new Vector3(0.0f, 0.1f, 0.0f);

			if(Physics.Raycast(rayOrigin, rayDirection, out hit, longViewDistance)) {

				if(hit.collider.gameObject == objToCompareTo) {
					return true; //Can see the object
				} else {
					return false; //Can't see the object
				}
			}
		}
		return false;
	}

	public static Dictionary<string, List<GameObject>> CheckFieldOfView(GameObject obj, GameObject[] objs, float fieldOfView, float longViewDistance) 
	{
		Dictionary<string, List<GameObject>> result = new Dictionary<string, List<GameObject>>();
		List<GameObject> auxList;

		for(int i = 0; i < objs.Length; i++) {
			
			if(Utils.CheckIfInsideCone(obj, objs[i], fieldOfView, longViewDistance)) {

				if(result.TryGetValue(objs[i].tag, out auxList)) { //If keyTag exists we add the object to the corresponding list
					auxList.Add(objs[i]);
					result[objs[i].tag] = auxList;
				} else { //Otherwise we create a new key and object list
					auxList = new List<GameObject>();
					auxList.Add(objs[i]);
					result.Add(objs[i].tag, auxList);
				}
			}
		}
		return result;
	}
	
	// Method that provides smooth navigation for a certain navAgent
	public static void SmoothNavigationRot(NavMeshAgent navAgent, Rigidbody rb, Vector3 eulerAngleVelocity) 
	{	
		float angle = Vector3.Angle(navAgent.velocity.normalized, rb.transform.forward);
		
		if (navAgent.velocity.normalized.x < rb.transform.forward.x) 
		{
			angle *= -1;
		}
		
		eulerAngleVelocity.Set(0f, -angle, 0f);
		Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
		rb.MoveRotation(rb.rotation * deltaRotation);
	}

	// Check if navAgent reached its destination
	public static bool ReachedDestination(NavMeshAgent navAgent, GameObject gameObject) 
	{
		if (!navAgent.pathPending) {
			if (navAgent.remainingDistance <= navAgent.stoppingDistance) {
				if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f) {

					// Disable navmeshAgent
					navAgent.ResetPath();
					navAgent.enabled = false;
				
					// Reset rotation
					gameObject.transform.rotation = Quaternion.identity;
					return true;
				}
			}
		}
		return false;
	}
}