using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Contains cell attributes
 */
public class UnitCell : MonoBehaviour { 

	private Cluster cluster;
	private Cell cell;

	// Cell attributes
	protected Vector3Int pos;
	protected int state;
	protected int age;
	protected int generation;

	// Updating
	private int tickCount = 0;
	private bool advanceState;


	public void reset() {
		state = Cell.PREPARE_SPAWN;
		age = 0;
		generation = 0;
	}

	void Start() {}

	void OnEnable() {
		reset();
	}

	void OnDisable() {
		// Prevent double disabling
		CancelInvoke();
	}

	void Update() {
		tickCount++;

		if (state == Cell.PREPARE_SPAWN)
			cell.spawn(cluster, this);
		else if (state == Cell.DYING)
			cell.die(cluster, this);
		else if (state == Cell.SPAWNING) {
			cell.moveIntoPlace(cluster, this);
			if (tickCount > cell.getSpawnTime())
				cell.finishSpawn(cluster, this);
		} 

		if (state == Cell.ALIVE)
			gameObject.transform.position = cluster.getWorldPosition(pos);

		if (tickCount > cell.getUpdateRate()) {
			cell.update(cluster, this);
			tickCount = 0;
			age++;
		}
	}


	public void move(Vector3 d) {
		gameObject.transform.position += d;
	}

	public Vector3 getCurrentWorldPosition()	{ return gameObject.transform.position; }
	public Vector3 getTargetWorldPosition()		{ return cluster.getWorldPosition(pos); }
	public void setWorldPosition(Vector3 pos)	{ this.gameObject.transform.position = pos; }

	public int getId()				{ return cell.getId(); }
	public Vector3Int getPosition()	{ return pos; }
	public int getState() 			{ return state; }
	public int getAge()   			{ return age; }
	public int getGeneration()   	{ return generation; }

	public void setCell(Cell cell)				{ this.cell = cell; } 
	public void setCluster(Cluster cluster) 	{ this.cluster = cluster; }
	public void setPosition(Vector3Int pos)		{ this.pos = pos; }
	public void setState(int state) 			{ this.state = state; }
	public void setGeneration(int gen)   		{ this.generation = gen; }
}

