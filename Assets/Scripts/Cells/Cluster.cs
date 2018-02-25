using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour {

	private Dictionary<Vector3Int, GameObject> cluster;
	private Cell cell;
	private int size;

	// Object pooling
	private const int POOL_COUNT = 512;
	private Stack<GameObject> cells;


	void Start() {
		cluster = new Dictionary<Vector3Int, GameObject>();
		cells = new Stack<GameObject>(POOL_COUNT);

		// Populate cell GameObject pool if empty
		if (cells.Count == 0)
			expandPool(POOL_COUNT, cell.getPrefab());

		// Spawn first cell
		spawnCell(new Vector3Int (0, 0, 0), null);
	}

	/**
	 * Create cell at given position
	 */
	public UnitCell spawnCell(Vector3Int pos, UnitCell parent) {
		// If cell exists in this position, kill it
		if (getUnitCell(pos) != null)
			killCell(pos);

		if (cells.Count == 0)
			// Expand object pool
			expandPool(1, this.cell.getPrefab());

		// Get inactive cell GameObject
		GameObject cellObj = cells.Pop();

		// Set GameObject position
		cellObj.transform.position = parent ? parent.getTargetWorldPosition() : getWorldPosition(pos);
		cellObj.transform.rotation = Quaternion.identity;
		cellObj.SetActive(true);

		// Update Cell and Cluster
		UnitCell cell = cellObj.GetComponent<UnitCell>();
		cell.setCell(this.cell);
		cell.setCluster(this);
		cell.setPosition(pos);
		cell.setGeneration(parent ? parent.getGeneration() + 1 : 0);
		setUnitCell(pos, cellObj);

		return cell;
	}

	/**
	 * Kill cell at given position
	 */
	public void killCell(Vector3Int pos) {
		GameObject cellObj = getUnitCellObject(pos);
		if (!cellObj)
			return;

		// Deactivate object and add to stack
		cellObj.SetActive(false);
		cells.Push(cellObj);

		// Remove reference to GameObject
		setUnitCell(pos, null);
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Object Pooling
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
	 * Instantiate cell GameObjects and add to object pool
	 */
	public void expandPool(int amt, GameObject prefab) {
		Debug.Log("EXPANDING DONG");

		GameObject cellObj = null;
		for (int i = 0; i < amt; i++) {
			cellObj = Object.Instantiate(prefab);
			cellObj.SetActive(false);
			cells.Push(cellObj);
		}
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Map Reading
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
	* Return True if position exists within grid bounds
	*/
	public bool inBounds(Vector3Int pos) {
		return (pos.magnitude < size);
	}

	/**
	 * Return True if cell is alive at given position
	 */
	public bool activeCell(Vector3Int pos) {
		UnitCell cell = getUnitCell(pos);

		// A cell is active if it is alive or dying, not spawning or dead
		return cell && (cell.getState() == Cell.ALIVE || cell.getState() == Cell.DYING);
	}

	/**
	 * Return True if no cell exists at the given position
	 */
	public bool emptyAt(Vector3Int pos) {
		return !getUnitCell(pos);
	}

	/**
	 * Return number of cells of given ID within 3x3x3 box surrounding given position
	 */
	public int adjacentCellCount(Vector3Int pos, int id) {
		// Note that IEnumerable does not support size evaluation operations
		int count = 0;
		foreach (UnitCell cell in adjacentUnitCells(pos, id))
			count += 1;
		return count;
	}

	/**
	 * Return iterator of cells of given ID within 3x3x3 box surrounding given position
	 */
	public IEnumerable<UnitCell> adjacentUnitCells(Vector3Int pos, int id) {
		foreach (Vector3Int cpos in adjacentPositions(pos)) {
			if (!activeCell (cpos)) continue;

			UnitCell cell = getUnitCell(cpos);

			// Return cells which match given ID
			if (cell.getId() == Cell.ALL_CELLS || cell.getId() == id)
				yield return cell;
		}
	}

	/**
	 * Return iterator of adjacent positions within 3x3x3 box surrounding given position
	 */
	public IEnumerable<Vector3Int> adjacentPositions(Vector3Int pos) {
		for (int tx = pos.x - 1; tx <= pos.x + 1; tx++) {
			for (int ty = pos.y - 1; ty <= pos.y + 1; ty++) {
				for (int tz = pos.z - 1; tz <= pos.z + 1; tz++) {
					Vector3Int apos = new Vector3Int(tx, ty, tz);
					if (apos == pos) continue;
					if (!inBounds(apos)) continue;
					yield return apos;
				}
			}
		}
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Getters and Setters
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
	 * Return world position of cell at given grid point
	 */
	public Vector3 getWorldPosition(Vector3Int pos) {
		Vector3 posf = new Vector3(
			pos.x * cell.getSize(),
			pos.y * cell.getSize(),
			pos.z * cell.getSize()
		);
		return posf + getPosition();
	}

	public UnitCell getUnitCell(Vector3Int pos) {
		GameObject cellObj = getUnitCellObject(pos);
		if (!cellObj)
			return null;

		return cellObj.GetComponent<UnitCell>();
	}

	public GameObject getUnitCellObject(Vector3Int pos) {
		if (!cluster.ContainsKey(pos))
			return null;
		
		return cluster[pos];
	}

	public void setUnitCell(Vector3Int pos, GameObject cellObj) {
		cluster[pos] = cellObj;
	}

	public float getMass() {
		return cluster.Count * cell.getMass();
	}

	public Vector3 getPosition() { 
		return gameObject.transform.position;
	}

	public void setCell(Cell cell)	{ this.cell = cell; }
	public void setSize(int size) 	{ this.size = size; }
}

