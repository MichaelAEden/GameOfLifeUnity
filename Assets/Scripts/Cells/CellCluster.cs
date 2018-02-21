using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCluster : MonoBehaviour {

	// Object pooling
	private const int POOL_COUNT = 512;
	private Stack<GameObject> cells;

	private Dictionary<Vector3Int, GameObject> cluster;
	private Vector3 pos;
	private Vector3 vel;
	private int size;


	public CellCluster(GameObject prefab, int size, Vector3 pos, Vector3 vel) {
	}

	void Start() {
		this.size = size;
		this.pos = pos;
		this.vel = vel;

		cluster = new Dictionary<Vector3Int, GameObject>();
		cells = new Stack<GameObject>(POOL_COUNT);

		// Populate cell GameObject pool if empty
		if (cells.Count == 0)
			expandPool(POOL_COUNT, prefab);

		// Spawn first cell
		spawnCell(new Vector3Int (0, 0, 0));
	}

	/**
	 * Create cell at given position
	 */
	public GameObject spawnCell(Vector3Int pos) {
		// If cell exists in this position, kill it
		if (getCell(pos))
			killCell(pos);

		if (cells.Count == 1)
			// Expand object pool
			expandPool(1, cells.Peek());

		// Get inactive cell GameObject
		GameObject cellObj = cells.Pop();

		// Set GameObject position
		cellObj.transform.position = getWorldPosition(pos);
		cellObj.transform.rotation = Quaternion.identity;
		cellObj.SetActive(true);

		// Update Cell and CellCluster
		Cell cell = cellObj.GetComponent<Cell>();
		cell.setPosition(pos);
		cell.setCluster(this);
		setCell(pos, cellObj);

		return cellObj;
	}

	/**
	 * Kill cell at given position
	 */
	public void killCell(Vector3Int pos) {
		GameObject cellObj = getCellObject(pos);
		if (!cellObj)
			return;

		// Deactivate object and add to stack
		cellObj.SetActive(false);
		cells.Push(cellObj);

		// Remove reference to GameObject
		setCell(pos, null);
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Object Pooling
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
	 * Instantiate cell GameObjects and add to object pool
	 */
	public static void expandPool(int amt, GameObject prefab) {
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
		Cell cell = getCell(pos);

		// A cell is active if it is alive or dying, not spawning or dead
		return cell && (cell.getState() == Cell.ALIVE || cell.getState() == Cell.DYING);
	}

	/**
	 * Return True if no cell exists at the given position
	 */
	public bool emptyAt(Vector3Int pos) {
		return !getCell(pos);
	}

	/**
	 * Return average generation of adjacent cells
	 */
	public int adjacentCellGeneration(Vector3Int pos, int id) {
		int gen = 0;
		foreach (Cell cell in adjacentCells(pos, id))
			gen = cell.getGeneration() > gen ? cell.getGeneration() : gen;
		return gen;
	}

	/**
	 * Return number of cells of given ID within 3x3x3 box surrounding given position
	 */
	public int adjacentCellCount(Vector3Int pos, int id) {
		// Note that IEnumerable does not support size evaluation operations
		int count = 0;
		foreach (Cell cell in adjacentCells(pos, id))
			count += 1;
		return count;
	}

	/**
	 * Return iterator of cells of given ID within 3x3x3 box surrounding given position
	 */
	public IEnumerable<Cell> adjacentCells(Vector3Int pos, int id) {
		foreach (Vector3Int cpos in adjacentPositions(pos)) {
			if (!activeCell (cpos)) continue;

			Cell cell = getCell(cpos);

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
	* Return position of cell GameObject
	*/
	public Vector3 getWorldPosition(Vector3Int pos) {
		Vector3 posf = new Vector3(
			pos.x * CELL_SIZE,
			pos.y * CELL_SIZE,
			pos.z * CELL_SIZE
		);
		return posf + this.pos + this.vel * (Time.time - timeCreated);
	}

	public Vector3 getVelocity() {
		return vel;
	}

	public Cell getCell(Vector3Int pos) {
		GameObject cellObj = getCellObject(pos);
		if (!cellObj)
			return null;

		return cellObj.GetComponent<Cell>();
	}

	public GameObject getCellObject(Vector3Int pos) {
		if (!cluster.ContainsKey(pos))
			return null;
		
		return cluster[pos];
	}

	public void setCell(Vector3Int pos, GameObject cellObj) {
		cluster[pos] = cellObj;
	}
}

