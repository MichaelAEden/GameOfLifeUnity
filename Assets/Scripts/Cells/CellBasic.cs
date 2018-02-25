using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBasic : Cell {
	
	public CellBasic() : base() {
		neighboursCausingDeathMax = 8;
		neighboursCausingDeathMin = 2;
		neighboursCausingBirthMax = 2;
		neighboursCausingBirthMin = 0;
	}
}

