using System;

namespace LBSE
{
   [Serializable]
	public class BankItem
	{
		public int ID;
		public int Amount;
		public InventoryItem Item
		{
			get
			{
				return GameData.Instance.GetItem(ID);
			}
		}
		
	}
}
