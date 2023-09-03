using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public enum DeathReason : short
    {
		Dehyderation = 0,
		Starve = 1,
		Bloodloss = 2,
		HighPressure = 3,
		LowPressure = 4,
		Breathloss = 5,
		HeartFailure = 6,
		Overheat = 7,
		Overcold = 8,
		Overdose = 9,
		CriticalDisease = 10
    }
}
