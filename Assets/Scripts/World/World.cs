using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

	// Gravitational constant
	private const float G = 0.2f;

	// Prefabs
	public GameObject prefabCluster;

	// Cell clusters
	public static List<GameObject> clusters = new List<GameObject>();


	public void spawnCluster(Cell cell, int size, Vector3 pos) {
		GameObject clusterObj = Object.Instantiate(prefabCluster, pos, Quaternion.identity);
		Cluster cluster = clusterObj.GetComponent<Cluster>();
		cluster.setCell(cell);
		cluster.setSize(size);
		clusters.Add(clusterObj);
	}

	public Vector3 getGravity(Vector3 pos) {
		Vector3 field = new Vector3();
		foreach (GameObject clusterObj in clusters) {
			Cluster cluster = clusterObj.GetComponent<Cluster>();
			Vector3 d = cluster.getPosition () - pos;

			// Gravity follows inverse square law
			field += d * (G * cluster.getMass() / Mathf.Pow(d.magnitude, 3));
		}
		return field;
	}
}
