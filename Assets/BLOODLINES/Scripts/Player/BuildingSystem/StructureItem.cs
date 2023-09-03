using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    [CreateAssetMenu(menuName="LSXGaming/Bloodlines/Structure Data")]
    public class StructureItem : ScriptableObject
    {
		[Header("Structure")]
		public int ID;
		public string Name;
		public string Description
		{
			get
			{
				string desc = string.Empty;
				foreach (RequiredMaterials material in requiredMaterials)
				{
					desc += string.Format("X{0} {1} \n", material.Amount, material.Material.Name);
				}
				
				return desc;
			}
		}
		public Sprite Icon;
		public GameObject Prefab;
        public StructureType Type;

        [Header("Building")]
        public bool Overlapable = false;
        public bool Snapable = true;
	    public LayerMask RequiredOverlap;
		public LayerMask ProhibitedOverlap;
        public Vector3 OverlapCheckBox = new Vector3(2.5f, 1f, 2.5f);
        public Vector3 OverlapCheckboxCenter = Vector3.zero;
		[Range(0 , 2)] public float OverlapCheckBoxScale = .75f;

        public List<StructureItem> AllowedStructures = new List<StructureItem>();

        public Vector3[] Rotations = new Vector3[4] {new Vector3(0f, 0f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 270f, 0f)};
		
		[Header("Requirements")]
		public RequiredMaterials[] requiredMaterials;
    }

    public enum StructureType : short
    {
        Foundation = 0,
        Wall = 1,
        Floor = 2,
        Door = 3,
        Interior
    }

    [System.Serializable]
	public class RequiredMaterials
	{
		public ItemData Material;
		public int Amount;
	}
}
