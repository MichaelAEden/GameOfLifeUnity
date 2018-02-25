using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Determines cell behaviour
 */
public abstract class Cell {

	// Cell types
	private static int idCount = 0;
	public static List<Cell> CELLS = new List<Cell>();
	public static Cell CELL_BASIC = new CellBasic(); 
	public static Cell CELL_ROCKET = new CellRocket();
	public static Cell CELL_PLANET = new CellPlanet(); 

	// Static constants
	protected const string CELL_COLOR = "CellColor";
	public const int ALL_CELLS = -1;
	public const int IMMORTAL = -1;

	public const int PREPARE_SPAWN = 0;
	public const int SPAWNING = 1;
	public const int ALIVE = 2;
	public const int DYING = 3;
	public const int DEAD = 4;

	// Cell attributes and rules
	public GameObject prefab;
	protected int id;
	protected float size = 1.0f;
	protected float mass = 1.0f;
	protected int cellLife = IMMORTAL;
	protected int updateRate = 500;
	protected int spawnTime = 200;
	protected int neighboursCausingDeathMax = 0;    // n or more surrounding cells and cell dies
	protected int neighboursCausingDeathMin = 0;    // n or less surrounding cells and cell dies
	protected int neighboursCausingBirthMax = 0;    // Between n - m surrounding cells - inclusive - cell is born
	protected int neighboursCausingBirthMin = 0;


	public Cell() {
		id = idCount;
		idCount += 1;
		CELLS.Add(this);
	}

	public static Cell getCell(int id) {
		return CELLS[id];
	}

	public static int cellCount() {
		return CELLS.Count;
	}


	/**
	 * Update state, position, and neighbours of given cell
	 */
	public virtual void update(Cluster cluster, UnitCell cell) {
		// Update adjacent cells
		spawnChildren(cluster, cell);

		// Update this cell's death
		if (shouldDie(cluster, cell))
			prepareDeath(cluster, cell);
	}

	/*
     * Operations to update state of cell
     */
	public void prepareSpawn(Cluster cluster, Vector3Int pos, UnitCell parent) {   
		UnitCell cell = cluster.spawnCell(pos, parent);
		cell.setState(PREPARE_SPAWN);
	}
	public void spawn(Cluster cluster, UnitCell cell) {
		cell.setState(SPAWNING);
	}
	public void finishSpawn(Cluster cluster, UnitCell cell) {
		cell.setState(ALIVE);
	}
	public void prepareDeath(Cluster cluster, UnitCell cell) {
		cell.setState(DYING);
	}
	public void die(Cluster cluster, UnitCell cell) {
		cluster.killCell(cell.getPosition());
		cell.setState(DEAD);
	}

	/**
     * Invoked while cell is spawning
     */
	public void moveIntoPlace(Cluster cluster, UnitCell cell) {
		Vector3 d = (cell.getTargetWorldPosition() - cell.getCurrentWorldPosition()) / 3;
		cell.move(d);
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
                            Virtual
       - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
     * Attempt to spawn child cells
     */
	protected virtual void spawnChildren(Cluster cluster, UnitCell cell) {
		foreach (Vector3Int apos in cluster.adjacentPositions(cell.getPosition()))
			if (shouldSpawn(cluster, apos))
				prepareSpawn(cluster, apos, cell);
	}

	/**
     * Returns True if cell at the given position should be spawned
     */
	protected virtual bool shouldSpawn(Cluster cluster, Vector3Int pos) {
		if (cluster.activeCell(pos))
			return false;

		int adjacentCells = cluster.adjacentCellCount(pos, id);
		return (
			cluster.emptyAt(pos)
			&& adjacentCells <= neighboursCausingBirthMax
			&& adjacentCells >= neighboursCausingBirthMin
		);
	}

	/**
     * Returns True if given cell should die
     */
	protected virtual bool shouldDie(Cluster cluster, UnitCell cell) {
		int adjacentCells = cluster.adjacentCellCount(cell.getPosition(), id);
		return (
			(cellLife != IMMORTAL && cell.getAge() >= cellLife)
			|| adjacentCells >= neighboursCausingDeathMax
			|| adjacentCells <= neighboursCausingDeathMin
		);
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
                            Getters and setters
       - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public int getUpdateRate() 		{ return updateRate; }
	public int getSpawnTime() 		{ return spawnTime; }
	public GameObject getPrefab() 	{ return prefab; }
	public int getId() 				{ return id; }
	public float getSize() 			{ return size; }
	public float getMass() 			{ return mass; }
}
