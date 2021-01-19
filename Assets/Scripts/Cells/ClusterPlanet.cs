using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterPlanet : Cluster {

	public ClusterPlanet() : base() {
		updatePeriod = 0.5f;
	}

	/**
	 * Update cell spawning and cell death
	 */
	public override void update(UnitCell cell) {
        if (cells.Count > 1024) return;

		cell.age += 1;
        if (cell.age > 1 && cell.canMultiply) {
            spawnChildren(cell);
            cell.canMultiply = false;
        }
		if (shouldDie(cell))
			cell.state = DYING;
	}

    protected override bool shouldSpawn(Vector3Int pos) {
        return true;
	}

	protected override bool shouldDie(UnitCell cell) {
		return cell.age > 2;
	}
}
