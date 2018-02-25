using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private World world;

	// UI
	private int selection = Cell.CELL_BASIC.getId();
	private float mouseSensitivity = 2.0f;

	// Physics
	private Rigidbody rb;
	private float force = 10000.0f;
	private Quaternion upRotation = Quaternion.identity;
	private Quaternion forwardRotation = Quaternion.identity;

	// Prefabs (temp)
	public GameObject prefabCell;
	public GameObject prefabCell2;
	public GameObject prefabCluster;


	void Start () {
		world = new World();
		rb = gameObject.GetComponent<Rigidbody>();

		// UI
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// Prefabs
		// TODO: load prefabs in each respective file
		world.prefabCluster = prefabCluster;
		Cell.CELL_BASIC.prefab = prefabCell;
		Cell.CELL_PLANET.prefab = prefabCell2;
		Cell.CELL_ROCKET.prefab = prefabCell;
	}

	void Update () {
		// Camera tracking
		float x = Input.GetAxis("Mouse X");
		if (Mathf.Abs(x) > 0)
			forwardRotation *= Quaternion.AngleAxis(x * mouseSensitivity, Vector3.up);

		// Current selection
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			selection--;
			if (selection < 0)
				selection = Cell.cellCount() - 1;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			selection++;
			if (selection > Cell.cellCount() - 1)
				selection = 0;
		}

		// Spawning
		if (Input.GetKeyDown(KeyCode.Q))
			world.spawnCluster(Cell.getCell(selection), 50, gameObject.transform.position + new Vector3(2f, 0f, 0f));

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


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Getters and Setters
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public Vector3 getUpwardDirection()  { return upRotation * Vector3.up; }
	public Vector3 getForwardDirection() { return upRotation * forwardRotation * Vector3.forward; }
}
