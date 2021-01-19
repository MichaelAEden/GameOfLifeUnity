using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private World world;

	// UI
	private float mouseSensitivity = 2.0f;

	// Physics
	private Rigidbody rb;
    private float maxVelocity = 10.0f;
	private float force = 10000.0f;
	private Quaternion upOffset = Quaternion.identity;
	private Quaternion forwardOffset = Quaternion.identity;
	private Quaternion upRotation = Quaternion.identity;

	// Prefabs (temp)
	public GameObject prefabClusterBasic;
	public GameObject prefabClusterPlanet;
	public GameObject prefabClusterRocket;


	void Start () {
		world = new World();
		rb = gameObject.GetComponent<Rigidbody>();

		// UI
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update () {
		// Limit player velocity
		if (rb.velocity.magnitude > maxVelocity)
			rb.velocity = Vector3.Normalize(rb.velocity) * maxVelocity;

		// Camera tracking
		float x = Input.GetAxis("Mouse X");
		if (Mathf.Abs(x) > 0)
			forwardOffset *= Quaternion.AngleAxis(x * mouseSensitivity, Vector3.up);

		float y = Input.GetAxis("Mouse Y");
		if (Mathf.Abs(y) > 0)
			upOffset *= Quaternion.AngleAxis(y * mouseSensitivity, Vector3.left);

		// Current selection
		if (Input.GetKeyDown(KeyCode.Alpha1))
			spawnCluster(prefabClusterBasic);
		else if (Input.GetKeyDown(KeyCode.Alpha2))
			spawnCluster(prefabClusterPlanet);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
			spawnCluster(prefabClusterRocket);

		// Movement
		Vector3 forceDirection = new Vector3();
		if (Input.GetKeyDown(KeyCode.A))
			forceDirection = Vector3.Cross(getForwardDirection(), getUpwardDirection());
		else if (Input.GetKeyDown(KeyCode.D))
			forceDirection = -Vector3.Cross(getForwardDirection(), getUpwardDirection());
		else if (Input.GetKeyDown(KeyCode.W))
			forceDirection = getForwardDirection();
		else if (Input.GetKeyDown(KeyCode.S))
			forceDirection = -getForwardDirection();
		else if (Input.GetKeyDown(KeyCode.Space))
			forceDirection = getUpwardDirection();
		else if (Input.GetKeyDown(KeyCode.LeftShift))
			forceDirection = -getUpwardDirection();
		rb.AddForce(forceDirection * force * Time.deltaTime);

		// Account for gravity of bodies of cells
		Vector3 gravity = world.getGravity(rb.worldCenterOfMass);
		rb.AddForce(gravity * rb.mass);

		// Compute relative directions
		upRotation = Quaternion.FromToRotation(Vector3.up, -gravity);
	}

	private void spawnCluster(GameObject clusterPrefab) {
		world.spawnCluster(clusterPrefab, gameObject.transform.position + new Vector3(0f, 0f, 2f));
	}

	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Getters and Setters
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public Vector3 getUpwardDirection()  { return upRotation * upOffset * Vector3.up; }
	public Vector3 getForwardDirection() { return upRotation * upOffset * forwardOffset * Vector3.forward; }
}
