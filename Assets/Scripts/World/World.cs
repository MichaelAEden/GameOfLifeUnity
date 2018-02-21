using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

	// Player
	public static Player player;

	// Cell clusters
	public static List<CellCluster> clusters;


	public void spawnCluster(GameObject prefab, int size, Vector3 pos, Vector3 vel) {
		CellCluster cluster = new CellCluster(prefab, size, pos, vel);
		clusters.Add(cluster);
	}

	public void getForce(Vector3 pos) {
		// TODO
	}
}
