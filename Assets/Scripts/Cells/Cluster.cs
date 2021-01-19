using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour {

	protected Dictionary<Vector3Int, GameObject> cells = new Dictionary<Vector3Int, GameObject>();
	public GameObject cellPrefab;
	protected float size = 100.0f;
	protected int age = 0;

	// Static constants
	protected const string CELL_COLOR = "CellColor";
	public const int ALL_CELLS = -1;
	public const int IMMORTAL = -1;

	public const int SPAWNING = 0;
	public const int ALIVE = 1;
	public const int DYING = 2;
	public const int DEAD = 3;

	// Cell attributes and rules
	protected float cellSize = 1.0f;
	protected float cellMass = 1.0f;
	protected int cellLife = IMMORTAL;
	protected int neighboursCausingDeathMax = 0;    // n or more surrounding cells and cell dies
	protected int neighboursCausingDeathMin = 0;    // n or less surrounding cells and cell dies
	protected int neighboursCausingBirthMax = 0;    // Between n - m surrounding cells - inclusive - cell is born
	protected int neighboursCausingBirthMin = 0;

	// Updating
	protected float updatePeriod = 0.5f;
	protected float lastUpdate = 0;
	protected float spawnTime = 1.0f;

	// Object pooling
	private const int POOL_COUNT = 1024;
	private Stack<GameObject> cellPool = new Stack<GameObject>(POOL_COUNT);


	void Start() {
		lastUpdate = Time.time;

		// Populate cell GameObject pool
		expandPool(POOL_COUNT, cellPrefab);

		// Spawn first cell
		for (int i = 0; i < 4; i++) {
			spawn(new Vector3Int(0, 0, 1), null);
			spawn(new Vector3Int(0, 0, -1), null);
			spawn(new Vector3Int(0, 1, 0), null);
			spawn(new Vector3Int(0, -1, 0), null);
		}
	}

	void Update() {
		while (Time.time - lastUpdate > updatePeriod) {
			lastUpdate += updatePeriod;
			age++;

			// Determine which cells will be spawned or killed
			foreach (UnitCell cell in allCells())
				update(cell);

			// Spawn and kill cells
			foreach (UnitCell cell in allCells())
				advanceState(cell);
		}
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Cell behaviour
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
	 * Update cell spawning and cell death
	 */
	public virtual void update(UnitCell cell) {
		cell.age += 1;
		if (cell.canMultiply) spawnChildren(cell);
		if (shouldDie(cell)) cell.state = DYING;
	}

	/**
	 * Update state of spawning cells and dying cells
	 */
	public virtual void advanceState(UnitCell cell) {
		if (cell.state == SPAWNING)
			cell.state = ALIVE;
		if (cell.state == DYING)
			kill(cell.pos);
	}

	/**
	* Attempt to spawn child cells
	*/
	protected virtual void spawnChildren(UnitCell cell) {
		foreach (Vector3Int apos in adjacentPositions(cell.pos))
			if (shouldSpawn(apos))
				spawn(apos, cell);
	}

	/**
     * Returns True if cell at the given position should be spawned
     */
	protected virtual bool shouldSpawn(Vector3Int pos) {
		int adjacentCells = adjacentCellCount(pos);
		return (
			getState(pos) == DEAD
			&& adjacentCells <= neighboursCausingBirthMax
			&& adjacentCells >= neighboursCausingBirthMin
		);
	}

	/**
     * Returns True if given cell should die
     */
	protected virtual bool shouldDie(UnitCell cell) {
		int adjacentCells = adjacentCellCount(cell.pos);
		return (
			(cellLife != IMMORTAL && cell.age >= cellLife)
			|| adjacentCells >= neighboursCausingDeathMax
			|| adjacentCells <= neighboursCausingDeathMin
		);
	}

	/**
     * Spawn cell into world from object pool
     */
	public void spawn(Vector3Int pos, UnitCell parent) {
		// If cell exists in this position, kill it
		if (getCell(pos) != null)
			kill(pos);

		// Expand object pool if required
		if (cellPool.Count == 0)
			expandPool(1, cellPrefab);

		// Get inactive cell GameObject
		GameObject cellObj = cellPool.Pop();

		// Set GameObject position
		cellObj.transform.position = getWorldPosition(pos);
		cellObj.transform.rotation = Quaternion.identity;
		cellObj.SetActive(true);

		UnitCell cell = cellObj.GetComponent<UnitCell>();
		cell.pos = pos;
		cell.state = SPAWNING;
		cell.generation = parent ? parent.generation + 1 : 0;
		setCell(pos, cellObj);
	}

	/**
     * Kill cell at given position and return cell to object pool
     */
	public void kill(Vector3Int pos) {
		GameObject cellObj = getCellObject(pos);
		if (!cellObj)
			return;

		// Deactivate object and add to stack
		cellObj.SetActive(false);
		cellPool.Push(cellObj);

		// Reset cell attributes
		UnitCell cell = cellObj.GetComponent<UnitCell>();
		cell.state = DEAD;
		cell.age = 0;
		cell.generation = 0;

		// Remove reference to GameObject
		setCell(pos, null);
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
	 * Return cell state at given position
	 */
	public int getState(Vector3Int pos) {
		UnitCell cell = getCell(pos);
		if (!cell)
			return DEAD;
		return cell.state;
	}

	/**
	 * Return number of cells of given ID within 3x3x3 box surrounding given position
	 */
	public int adjacentCellCount(Vector3Int pos) {
		int count = 0;
		foreach (UnitCell cell in adjacentCells(pos))
			count += 1;
		return count;
	}

	/**
	 * Return iterator of cells of given ID within 3x3x3 box surrounding given position
	 */
	public IEnumerable<UnitCell> adjacentCells(Vector3Int pos) {
		foreach (Vector3Int cpos in adjacentPositions(pos)) {
			int state = getState(cpos);
			if (state == ALIVE || state == DYING)
				yield return getCell(cpos);
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

	/**
	 * Return iterator of all cells
	 */
	public List<UnitCell> allCells() {
		List<UnitCell> allCells = new List<UnitCell>();
		foreach (GameObject cellObj in cells.Values)
			allCells.Add(cellObj.GetComponent<UnitCell>());
		return allCells;
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
                            Handlers
       - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/*
	* Called upon cell creation or deletion by user
	*/
	public void onCreation(Vector3Int pos) {}
	public void onDeletion(Vector3Int pos) {}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Object Pooling
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
	* Instantiate cell GameObjects and add to object pool
	*/
	public void expandPool(int amt, GameObject prefab) {
		for (int i = 0; i < amt; i++) {
			GameObject cellObj = Object.Instantiate(prefab);
			cellObj.SetActive(false);
			cellPool.Push(cellObj);
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
			pos.x * cellSize,
			pos.y * cellSize,
			pos.z * cellSize
		);
		return posf + getPosition();
	}

	public UnitCell getCell(Vector3Int pos) {
		GameObject cellObj = getCellObject(pos);
		if (!cellObj)
			return null;

		return cellObj.GetComponent<UnitCell>();
	}

	public GameObject getCellObject(Vector3Int pos) {
		if (!cells.ContainsKey(pos))
			return null;

		return cells[pos];
	}

	public void setCell(Vector3Int pos, GameObject cellObj) {
		if (cellObj)
			cells[pos] = cellObj;
		else
			cells.Remove(pos);
	}

	public float getMass() {
		return cells.Count * cellMass;
	}

	public Vector3 getPosition() { 
		return gameObject.transform.position;
	}
}

