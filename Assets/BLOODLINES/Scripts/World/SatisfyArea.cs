using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(SphereCollider))]
    public class SatisfyArea : Entity
    {
		[Tooltip("The Need ID you Assigned in GameData")]
		public int NeedID;
		public float Radius;
		public float SatisfyingMultiplier = 2f;
		public Need Need
		{
			get
			{
				return GameData.Instance.GetNeed(NeedID);
			}
		}
		
		[Tooltip("DEBUG")]
		public Color GizmoColor;
		
		public List<AIAnimal> Creatures = new List<AIAnimal>();

		void Start()
		{
			GetComponent<SphereCollider>().isTrigger = true;
			GetComponent<SphereCollider>().radius = Radius;
		}
		
        void Update()
        {
			for (int i = 0; i < Creatures.Count; i++)
			{
				for (int o = 0; o < Creatures[i].Needs.Count; o++)
				{
					if (Creatures[i].Needs[o].ID == NeedID || Creatures[i].Needs[o].Need == Need)
					{
						if (Creatures[i].Needs[o].CurrentValue < Creatures[i].Needs[o].CriticalValue)
						{
							// Make The Creature Satisfy faster when it's be in urgent need
							Creatures[i].Needs[o].CurrentValue += (Creatures[i].Needs[o].ChangePerSec * SatisfyingMultiplier) * Time.deltaTime * 2;
						}
						else if (Creatures[i].Needs[o].CurrentValue < Creatures[i].Needs[o].MaxValue)
						{
							Creatures[i].Needs[o].CurrentValue += (Creatures[i].Needs[o].ChangePerSec * SatisfyingMultiplier) * Time.deltaTime;
						}
						else if (Creatures[i].Needs[o].CurrentValue >= Creatures[i].Needs[o].MaxValue)
						{
							Creatures[i].Target = null;
                            Creatures[i].SearchTarget();
                        }
					}
				}
			}
        }
		
		public void OnTriggerEnter(Collider other)
		{
            Debug.Log(other.name + " Entered " + gameObject.name);
			if (other.CompareTag("AI"))
			{
				Debug.Log(other.name + " Animal Detected By " + gameObject.name);
				AIAnimal creature = other.GetComponent<AIAnimal>();
				foreach (NeedEXT need in creature.Needs)
				{
					if (need.ID == NeedID)
						need.Immortal = true;
				}
				if (!Creatures.Contains(creature))
					Creatures.Add(creature);
				Debug.Log(other.name + " Animal Added To " + gameObject.name);
			}
		}
		
		
		public void OnTriggerExit(Collider other)
		{
			Debug.Log(other.name + " Exit " + gameObject.name);
			if (other.tag == "AI")
			{
				Debug.Log(other.name + " Animal Exit From " + gameObject.name);
				AIAnimal creature = other.GetComponent<AIAnimal>();
				foreach (NeedEXT need in creature.Needs)
				{
					if (need.ID == NeedID)
						need.Immortal = false;
				}
				if (Creatures.Contains(creature))
					Creatures.Remove(creature);
			}
		}
		
		
		void OnDrawGizmosSelected()
		{
			Gizmos.color = GizmoColor;
			Gizmos.DrawSphere(transform.position, Radius);
		}
    }
}
