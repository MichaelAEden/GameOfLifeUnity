using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Contains cell attributes
 */
public class UnitCell : MonoBehaviour {
	public Vector3Int pos;
	public int state;
	public int age;
	public int generation;
	// TODO: resolve inconsistent wording (birth, spawn, multiply...).
	public bool canMultiply = true;
}
