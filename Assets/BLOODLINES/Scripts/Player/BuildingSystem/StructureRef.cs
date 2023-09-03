using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class StructureRef : MonoBehaviour
	{
		public StructureItem Item;
		public Renderer[] Renderers;
		public List<StructureSocket> Sockets = new List<StructureSocket>();

        public StructureRef Parent;
		public Action OnDestroy;

		#region PreviewInfo
		public Transform snapPoint;
		public bool Snap = false;

		public Transform TargetPoint;

		#endregion

		#region Core
        void Awake()
		{
			Renderers = GetComponentsInChildren<Renderer>();
		}

		public void SetParent(StructureRef parent)
		{
			if (Item.Type == StructureType.Foundation)
                return;
			
            Parent = parent;
            Parent.OnDestroy += Destroy;
        }

		 public void Destroy()
		{
			OnDestroy?.Invoke();
			if(Parent != null)
				Parent.OnDestroy -= Destroy;
			
			Destroy(gameObject);
		}

		#endregion

		#region Preview

		public void DisableColliders()
		{
			Collider[] Cols = GetComponentsInChildren<Collider>();
			foreach (Collider col in Cols)
			{
                col.enabled = false;
            }
		}
		
		public void SnapStructure(StructureRef targetStruct, int targetPoint)
		{
			if (!Snap)
				return;
			
			foreach (StructureSocket socket in targetStruct.Sockets)
			{
				if (socket.ID == targetPoint)
				{
                    TargetPoint = socket.transform;
                    break;
                }
			}
			
			transform.position = TargetPoint.transform.position - (transform.rotation * snapPoint.localPosition);

			switch (Item.Type)
			{
				case StructureType.Foundation:
					break;
				case StructureType.Wall:
					if (targetStruct.Item.Type == StructureType.Wall)
						transform.forward = TargetPoint.transform.parent.forward;
					else
						transform.forward = TargetPoint.transform.forward;
					break;
				case StructureType.Floor:
					if (targetStruct.Item.Type == StructureType.Wall)
					{
						transform.forward = TargetPoint.transform.parent.forward * (TargetPoint.transform.parent.IsBlockedBy(GlobalGameManager.Instance.LocalPlayer.transform) ? -1 : 1);
						transform.position = TargetPoint.transform.position - transform.eulerAngles != Vector3.zero ? (transform.rotation * snapPoint.localPosition) : snapPoint.localPosition;
					}
					break;
				case StructureType.Door:
					transform.forward = TargetPoint.transform.forward;
					break;
				case StructureType.Interior:
					transform.forward = TargetPoint.transform.forward;
					transform.position = TargetPoint.transform.position - transform.eulerAngles != Vector3.zero ? (transform.rotation * snapPoint.localPosition) : snapPoint.localPosition;
					break;
			}
		}

		public bool CanBuild()
		{
			Vector3 pos = new Vector3(transform.position.x + Item.OverlapCheckboxCenter.x, transform.position.y + Item.OverlapCheckboxCenter.y, transform.position.z + Item.OverlapCheckboxCenter.z);

			Collider[] colliders = new Collider[0];
			colliders = Physics.OverlapBox(pos, (Item.OverlapCheckBox / 2) * Item.OverlapCheckBoxScale, transform.rotation, Item.ProhibitedOverlap);

			foreach (Collider collider in colliders)
			{
				if (collider.transform.root.TryGetComponent(out StructureRef targetStruct))
				{
					if (Item.AllowedStructures.Contains(targetStruct.Item))
						continue;
					else
						return false;
				}
				else
					return false;
			}

			if(Item.Snapable && !Snap)
				return false;

			if (Item.Overlapable && !Physics.CheckBox(pos, (Item.OverlapCheckBox / 2) * Item.OverlapCheckBoxScale, transform.rotation, Item.RequiredOverlap))
				return false;
			
			return true;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(Item.OverlapCheckboxCenter, Item.OverlapCheckBox * Item.OverlapCheckBoxScale);
		}

		#endregion
	}
}
