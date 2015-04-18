using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	private float turnSpeed = 6.0f;
	// Speed of camera turning when mouse moves in along an axis
	private float panSpeed = 2.0f;
	// Speed of the camera when being panned
	private float zoomSpeed = 2.0f;
	// Speed of the camera going back and forth
	
	private Vector3 mouseOrigin;
	// Position of cursor when mouse dragging starts
	private bool isPanning;
	// Is the camera being panned?
	private bool isRotating;
	// Is the camera being rotated?
	private bool isZooming;
	// Is the camera zooming?
	public Camera cam;

	//
	// UPDATE
	//
	
	void Update ()
	{
		// Get the left mouse button
		if (Input.GetMouseButtonDown (0)) {
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isRotating = true;
		}
		
		// Get the right mouse button
		if (Input.GetMouseButtonDown (1)) {
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isPanning = true;
		}
		
		// Get the middle mouse button
		if (Input.GetMouseButtonDown (2)) {
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isZooming = true;
		}

		// Disable movements on button release
		if (!Input.GetMouseButton (0))
			isRotating = false;
		if (!Input.GetMouseButton (1))
			isPanning = false;
		if (!Input.GetMouseButton (2))
			isZooming = false;
		
		// Rotate camera along X and Y axis
		if (isRotating) {
			Vector3 pos = cam.ScreenToViewportPoint (Input.mousePosition - mouseOrigin);
			transform.RotateAround (transform.position, transform.right, -pos.y * turnSpeed);
			transform.RotateAround (transform.position, Vector3.up, pos.x * turnSpeed);
		}
		
		// Move the camera on it's XY plane
		if (isPanning) {
			Vector3 pos = cam.ScreenToViewportPoint (Input.mousePosition - mouseOrigin);
			Vector3 move = new Vector3 (pos.x * panSpeed, pos.y * panSpeed, 0);
			transform.Translate (move, Space.Self);
		}
		
		// Move the camera linearly along Z axis
		if (isZooming) {
			Vector3 pos = cam.ScreenToViewportPoint (Input.mousePosition - mouseOrigin);
			Vector3 move = pos.y * zoomSpeed * transform.forward; 
			transform.Translate (move, Space.World);
		}

		//to move the camera whith the wheel or the W A S D buttons
		transform.position += transform.forward * (zoomSpeed * 5) * Input.GetAxis ("Vertical") * Time.deltaTime;
		transform.position += transform.right * (panSpeed * 5) * Input.GetAxis ("Horizontal") * Time.deltaTime;

		transform.Translate (Vector3.forward * Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed * 100 * Time.deltaTime);
			
	}
}