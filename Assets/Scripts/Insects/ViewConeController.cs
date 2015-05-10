using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewConeController : MonoBehaviour 
{
	public static bool CheckIfInsideCone(GameObject obj, GameObject objToCompareTo, float fieldOfView, float longViewDistance, float closeViewDistance) 
	{
		RaycastHit hit; //Out hit
		Vector3 rayDirection = objToCompareTo.transform.position - obj.transform.position;

		// If the objToCompareTo is close to this object and is in front of it, then return true
		if(Vector3.Angle(rayDirection, obj.transform.forward) < 90 && Vector3.Distance(obj.transform.position, objToCompareTo.transform.position) <= closeViewDistance) {
			return true;
		}

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

	public static Dictionary<string, List<GameObject>> CheckFieldOfView(GameObject obj, GameObject[] objs, float fieldOfView, float longViewDistance, float closeViewDistance) 
	{
		Dictionary<string, List<GameObject>> result = new Dictionary<string, List<GameObject>>();
		List<GameObject> auxList;

		for(int i = 0; i < objs.Length; i++) {
			
			if(ViewConeController.CheckIfInsideCone(obj, objs[i], fieldOfView, longViewDistance, closeViewDistance)) {

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
}