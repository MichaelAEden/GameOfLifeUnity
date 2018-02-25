using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {

	private float cameraOffset = 25f;
	private Player player;
	public GameObject playerObj;

	// TODO: add camera smoothing
	// TODO: camera tracks player velocity, similar to "Super Monkey Ball"

	void Start() {
		player = playerObj.GetComponent<Player>();
	}
	
	void Update () {
		transform.position = playerObj.transform.position - player.getForwardDirection() * cameraOffset;
		transform.rotation = Quaternion.LookRotation(player.getForwardDirection(), player.getUpwardDirection());
	}
}
