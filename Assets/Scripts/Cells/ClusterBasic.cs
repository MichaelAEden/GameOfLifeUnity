using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterBasic : Cluster {
	
	public ClusterBasic() : base() {
		neighboursCausingDeathMax = 7;
		neighboursCausingDeathMin = 2;
		neighboursCausingBirthMax = 6;
		neighboursCausingBirthMin = 3;
	}
}
