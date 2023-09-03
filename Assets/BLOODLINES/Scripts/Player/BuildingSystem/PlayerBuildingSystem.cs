using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class PlayerBuildingSystem : MonoBehaviour
    {
		[Header("References")]
		public PlayerController player;
		public InventoryManager Inventory;

		[Header("Building")]
		public float BuildDistance = 10f;
		public LayerMask BuildLayers;
        public Color Buildable = Color.green;
		public Color Unbuildable = Color.red;
		public Color InsuffientMaterials = Color.blue;

		public StructureRef currentStructurePreview;
        public StructureRef currentPlaceholder;

        Collider[] pos = new Collider[1];
        Ray previewRay;
        int CurrentRot;
        public int currentSnapPoint;

        void Start()
        {
			player = GetComponent<PlayerController>();
			Inventory = GetComponent<InventoryManager>();
        }
		
        void Update()
        {
			if (!Continue())
				return;

            Preview();
            HandleInput();
        }
		
		void HandleInput()
		{
			bool placeable = currentStructurePreview.CanBuild();
            bool enoughMaterials = CheckRequiredMaterials(currentStructurePreview.Item);
            bool canBuild = placeable && enoughMaterials;

            foreach (Renderer renderer in currentStructurePreview.Renderers)
			{
				if (!placeable)
					renderer.material.color = Unbuildable;
				else if (!enoughMaterials)
					renderer.material.color = InsuffientMaterials;
				else
					renderer.material.color = Buildable;
			}
			
			if (canBuild && InputManager.GetMouseButtonDown(0))
			{
				Instantiate(currentStructurePreview.Item.Prefab, currentStructurePreview.transform.position, currentStructurePreview.transform.rotation).GetComponent<StructureRef>().SetParent(currentPlaceholder);
				foreach (RequiredMaterials item in currentStructurePreview.Item.requiredMaterials)
				{
					if (Inventory.ItemAmount(item.Material.GlobalID) >= item.Amount)
						Inventory.QuickRemoveByAmount(item.Material.GlobalID, item.Amount, InventoryItemType.Consumable);
				}
		//X		Inventory.UpdateInventory(CurrencyType.COINS, 0);
			}

			if (InputManager.GetButtonDown("DestroyBuilding") && currentPlaceholder != null)
                currentPlaceholder.Destroy();

			if (InputManager.GetKeyDown(KeyCode.R) || InputManager.GetMouseButton(1))
				CurrentRot = (CurrentRot + 1) % currentStructurePreview.Item.Rotations.Length;
			
			currentStructurePreview.Snap = InputManager.GetButton("SnapBuilding");
		}
		public bool Continue()
		{
			if (currentStructurePreview == null || (currentStructurePreview != null && InputManager.GetKeyDown(KeyCode.Escape)) || !BuildBlueprintUI.Instance.gameObject.activeSelf)
			{
				if (currentStructurePreview != null)
					Destroy(currentStructurePreview.gameObject);
				
				return false;
			}
			return true;
		}
		
		public void ChangeStruture(StructureItem structure)
		{
			if (currentStructurePreview != null)
				Destroy(currentStructurePreview.gameObject);
			

            currentStructurePreview = Instantiate(structure.Prefab, PreviewPos(), Quaternion.identity).GetComponent<StructureRef>();
            currentStructurePreview.gameObject.name = structure.Prefab.name;
            currentStructurePreview.DisableColliders();
		}


		
		public void Preview()
		{
			if (currentStructurePreview.Snap && currentPlaceholder != null && currentPlaceholder.Item.AllowedStructures.Contains(currentStructurePreview.Item))
                currentStructurePreview.SnapStructure(currentPlaceholder, currentSnapPoint);
			else
                currentStructurePreview.transform.position = PreviewPos();
			
			currentStructurePreview.transform.rotation = Quaternion.Euler(currentStructurePreview.Item.Rotations[CurrentRot]);
		}

		Vector3 PreviewPos()
        {
            previewRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(previewRay, out RaycastHit hit, BuildDistance, BuildLayers))
            {
                if(hit.transform.TryGetComponent(out StructureSocket sRef))
				{
                    currentSnapPoint = sRef.ID;
                    currentPlaceholder = sRef.Item;
				}
				else if (hit.transform.root.TryGetComponent(out StructureRef Ref))
					currentPlaceholder = Ref;
                else
                    currentPlaceholder = null;
				
                return hit.point;
            }
            else
                currentPlaceholder = null;
			
            return previewRay.GetPoint(BuildDistance);
        }
		

		public bool CheckRequiredMaterials(StructureItem structure)
		{
			if (structure == null)
				return false;
			
			int Ready = 0;
			foreach (RequiredMaterials item in structure.requiredMaterials)
			{
				if (Inventory.ItemAmount(item.Material.GlobalID) >= item.Amount)
					Ready++;
			}
			
			return Ready == structure.requiredMaterials.Length;
		}
		
    }
	
	[System.Serializable]
	public class CustomPos
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public CustomPos(Vector3 pos, Quaternion rot)
		{
			Position = pos;
			Rotation = rot;
		}
	}
}
