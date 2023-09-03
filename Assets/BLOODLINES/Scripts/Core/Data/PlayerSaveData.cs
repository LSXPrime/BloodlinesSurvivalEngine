using System;
using System.Collections.Generic;

namespace LBSE
{
    [Serializable]
	public struct PlayerSaveData
	{
		public Vector3S Position;
		public QuaternionS Rotation;
		public HumanHealthSystem HealthSystem;
		public List<CurrentDisease> ActiveDiseases;
		public List<CurrentMedicine> ActiveMedicines;
		public List<QuestDataEXT> Quests;
		public List<InventoryList> Weapons;
		public List<InventoryList> Consumables;
		public List<WearableData> Wearables;
	}
}
