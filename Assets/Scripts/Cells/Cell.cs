using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

	// Static constants
	protected const string CELL_COLOR = "CellColor";
	public const int ALL_CELLS = -1;
	public const int IMMORTAL = -1;
	public const int SPAWNING = 0;
	public const int ALIVE = 1;
	public const int DYING = 2;
	public const int DEAD = 3;

	// Cell manager
	protected CellCluster cluster;

	// Cell position
	protected Vector3Int pos;

	// Cell attributes and rules
	protected int id;
	protected float size;
	protected float mass;
	protected int cellLife;
	protected Color color;
	protected int updateRate;
	protected int neighboursCausingDeathMax;	// n or more surrounding cells and cell dies
	protected int neighboursCausingDeathMin;	// n or less surrounding cells and cell dies
	protected int neighboursCausingBirthMax;	// Between n - m surrounding cells - inclusive - cell is born
	protected int neighboursCausingBirthMin;

	// Cell updates
	protected int state;
	protected int tickCount;	// TODO: replace with Time.time
	protected int age;
	protected int generation;


	protected void reset() {
		state = SPAWNING;
		tickCount = 0;
		age = 0;
		generation = 0;
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Overriding MonoBehaviour
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	void Start() {
		reset();

		cluster.setCell(pos, gameObject);

		id = 0;
		cellLife = IMMORTAL;
		color = Color.blue;
		updateRate = 25;

		neighboursCausingDeathMax = 8;
		neighboursCausingDeathMin = 2;
		neighboursCausingBirthMax = 2;
		neighboursCausingBirthMin = 0;
	}

	void Update() {
		gameObject.transform.SetPositionAndRotation(
			cluster.getWorldPosition(pos),
			Quaternion.identity);

		tickCount++;
		if (tickCount % updateRate == 0) {
			// Update cell state and spawn other cells, if possible
			updateCell();
			tickCount = 0;
			age++;
		} else if (tickCount % updateRate == 1) {
			// Progress cell into next appropriate state
			if (state == SPAWNING)
				spawn ();
			else if (state == DYING)
				die ();
		}
	}

	void OnEnable() {
		reset();
	}

	void OnDisable() {
		// Prevent double disabling
		CancelInvoke();
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Cell behaviour
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/**
	 * Updates cell behaviour
	 */
	protected void updateCell() {
		// Update this cell's appearance, if required
//		Material material = gameObject.GetComponent<Material>();
//		Color color = getColor();
//		if (color != material.GetColor(CELL_COLOR))
//			material.SetColor(CELL_COLOR, color);

		// Update adjacent cell spawning
		foreach (Vector3Int pos in cluster.adjacentPositions(this.pos))
			if (shouldSpawn(pos))
				prepareSpawn(pos);

		// Update this cell's death
		if (shouldDie())
			prepareDeath();
	}

	/**
	 * Returns True if cell at the given position should be spawned
	 */
	protected virtual bool shouldSpawn(Vector3Int pos) {
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
	 * Returns True if this cell should die
	 */
	protected virtual bool shouldDie() {
		int adjacentCells = cluster.adjacentCellCount(pos, id);
		return (
			(cellLife != IMMORTAL && age >= cellLife)
			|| adjacentCells >= neighboursCausingDeathMax
			|| adjacentCells <= neighboursCausingDeathMin
		);
	}

	/*
	 * Operations to change state of cell
	 */
	protected void prepareSpawn(Vector3Int pos) { 	
		cluster.spawnCell(pos);
		setState(SPAWNING);
	}
	protected void spawn() {
		generation = cluster.adjacentCellGeneration(pos, id) + 1;
		setState(ALIVE);
	}
	protected void prepareDeath() {
		setState(DYING);
	}
	protected void die() {
		cluster.killCell(pos);
		setState(DEAD);
	}


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Getters and Setters
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void setCluster(CellCluster cluster) {	this.cluster = cluster; } 
	public void setPosition(Vector3Int pos) { 		this.pos = pos; }
	public void setState(int state) { 		  		this.state = state; }

	public CellCluster getCluster() {		return cluster; } 
	public int getId() { 					return id; }
	public int getSize() { 					return size; }
	public int getMass() { 					return mass; }
	public int getState() { 				return state; }
	public int getGeneration() {			return generation; }
	public Color getColor() {				return color; }
	public static GameObject getPrefab() {	return null; }


	/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
						    Handlers
	   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	/*
	 * Called upon cell creation or deletion by user
	 */
	public void onCreation(Vector3Int pos) {}
	public void onDeletion(Vector3Int pos) {}
}
