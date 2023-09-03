using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class StructureSocket : MonoBehaviour
    {
		public StructureRef Item;
        public int ID;
        public Vector3[] Rotations = new Vector3[4] {new Vector3(0f, 0f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 270f, 0f)};
	}
}
