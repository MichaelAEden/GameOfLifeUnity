using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	// Prefabs
	public GameObject cell;
	public GameObject cellPlanet;

	// Physics
	private Rigidbody rb;
	private float accel = 10.0f;

	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
		gameObject.transform.rotation;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Q))
			Instantiate(cellPlanet, gameObject.transform.position, Quaternion.identity);

		if (Input.GetKeyDown(KeyCode.A))
			rb.AddForce(new Vector3(-accel, 0, 0));
		else if (Input.GetKeyDown(KeyCode.D))
			rb.AddForce(new Vector3(accel, 0, 0));
		else if (Input.GetKeyDown(KeyCode.W))
			rb.AddForce(new Vector3(0, accel, 0));
		else if (Input.GetKeyDown(KeyCode.S))
			rb.AddForce(new Vector3(0, -accel, 0));
	}
}
