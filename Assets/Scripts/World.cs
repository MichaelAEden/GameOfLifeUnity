using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

	// Gravitational constant
	private const float G = 0.2f;

	// Cell clusters
	public static List<GameObject> clusters = new List<GameObject>();


	public void spawnCluster(GameObject clusterPrefab, Vector3 pos) {
		GameObject cluster = Object.Instantiate(clusterPrefab, pos, Quaternion.identity);
		clusters.Add(cluster);
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
