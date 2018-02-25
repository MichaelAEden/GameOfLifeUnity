using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellRocket : Cell {

	public CellRocket() : base() {
		updateRate = 1;
		cellLife = 4;
	}

	protected override void spawnChildren(Cluster cluster, UnitCell cell) {
		if (cell.getAge() > 1)
			cluster.spawnCell(cell.getPosition() + Vector3Int.left, cell);
	}

	protected override bool shouldDie (Cluster cluster, UnitCell cell) {
		return cell.getAge() > cellLife;
	}
}
