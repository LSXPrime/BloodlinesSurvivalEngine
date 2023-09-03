using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
    public class BuildBlueprintUI : MonoBehaviour
    {
		[Header("Referances")]
		public GameObject ItemUIPrefab;
		internal PlayerBuildingSystem player;
		
		[Header("Structures Data")]
		public Transform ItemsPanel;
		public List<BuildStructureUI> Structures;
		
		[Header("Structure Details")]
		public Image ItemIcon;
		public Text ItemName;
		public Text ItemRequirements;
		
		private int currentStructure;
		
		void OnEnable()
		{
            ItemsInstantiate();
        }
		
        public void ItemsInstantiate()
		{
			for (int i = 0; i < ItemsPanel.childCount; i++) { Destroy(ItemsPanel.GetChild(i).gameObject); }
			foreach (BuildStructureUI structure in Structures) { Destroy(structure.gameObject); }
			Structures.Clear(); ClearPreview();
			
			for (int i = 0; i < GameData.Instance.BuildStructures.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(ItemsPanel, false);
				BuildStructureUI Item = GO.GetComponent<BuildStructureUI>();
				Item.Set(GameData.Instance.BuildStructures[i]);
				Structures.Add(Item);
			}
			
		}
		
        void Update()
        {
			currentStructure = (currentStructure + (int)InputManager.GetAxis("Mouse ScrollWheel")) % (Structures.Count - 1);
			if (player.currentStructurePreview == null || player.currentStructurePreview.Item != Structures[currentStructure].Item)
			{
				player.ChangeStruture(Structures[currentStructure].Item);
				Preview(Structures[currentStructure].Item);
			}
        }

		public void Preview(StructureItem item)
		{
			ItemIcon.sprite = item.Icon;
			ItemName.text = item.Name;
			ItemRequirements.text = item.Description;
		}
		
		public void ClearPreview()
		{
			ItemIcon.sprite = null;
			ItemName.text = string.Empty;
			ItemRequirements.text = string.Empty;
		}
		
		private static BuildBlueprintUI instance;
		public static BuildBlueprintUI Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<BuildBlueprintUI>(); }
				return instance;
			}
		}
    }
}
