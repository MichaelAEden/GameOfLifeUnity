using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPlanet : Cell {

	public CellPlanet() : base() {
		updateRate = 100;
	}

	protected override void spawnChildren(Cluster cluster, UnitCell cell) {
		if (cell.getGeneration() > 4)
			return;

		foreach (Vector3Int apos in cluster.adjacentPositions(cell.getPosition()))
			prepareSpawn(cluster, apos, cell);
	}

	protected override bool shouldDie(Cluster cluster, UnitCell cell) {
		return false;
	}
}
