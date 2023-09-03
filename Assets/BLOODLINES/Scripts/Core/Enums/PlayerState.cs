using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public enum PlayerState : short
	{
		Idle = 0,
		Walk = 1,
		Sprint = 2,
		Crouch = 3,
		Jump = 4,
		Mounted = 5,
		Swimming = 6,
		Sleeping = 7
	}
}
