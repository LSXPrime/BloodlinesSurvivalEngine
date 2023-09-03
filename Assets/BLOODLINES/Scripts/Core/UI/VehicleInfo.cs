using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
    public class VehicleInfo : MonoBehaviour
    {
		public GameObject Content;
		public Text Speed;
		public Slider Fuel;
		public Slider Health;

        void Update()
        {
			if (GlobalGameManager.Instance.LocalPlayer == null)
				return;
			
			Content.SetActive(GlobalGameManager.Instance.LocalPlayer.Get<PlayerController>().State == PlayerState.Mounted);
			if (GlobalGameManager.Instance.LocalPlayer.Get<PlayerController>().State == PlayerState.Mounted)
			{
				VehiclesManager VM = GlobalGameManager.Instance.LocalPlayer.Get<PlayerMountSystem>().CurrentVehicle.VM;
				Speed.text = string.Format("{0} KM/H", ((int)VM.Speed).ToString());
				Fuel.maxValue = VM.MaxFuel;
				Fuel.value = VM.Fuel;
				Health.maxValue = VM.healthSystem.maxHealth;
				Health.value = VM.healthSystem.health;
			}
        }
    }
}
