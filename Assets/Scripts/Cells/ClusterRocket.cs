using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterRocket : Cluster {

	public ClusterRocket() : base() {
		updatePeriod = 0.1f;
		cellLife = 10;
	}

	protected override void spawnChildren(UnitCell cell) {
		spawn(cell.pos + Vector3Int.left, cell);
        cell.canMultiply = false;
	}

	protected override bool shouldDie(UnitCell cell) {
		return cell.age > cellLife;
	}
}
