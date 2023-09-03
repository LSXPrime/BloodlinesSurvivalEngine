using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class PickupTrigger : MonoBehaviour
    {
        public float WeaponID;
        public Transform CurrentItem;
        /*
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (InputManager.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine(EquipWeapon(ID));
			}
        }

        IEnumerator EquipWeapon(int ID)
		{
			WeaponIdleLayer.weight = 0;
            WeaponAimLayer.weight = 0;
            BlockedAimLayer.weight = 0;
            HandsLayer.weight = 0;

            animator.SetTrigger("SwitchWeapon");
			while (animator.GetCurrentAnimatorStateInfo(1).IsName("Holster"))
			{
				
				if (CurrentItem != null)
					CurrentItem.transform.parent = HandPos;
				
				 yield return new WaitForSeconds(Time.deltaTime);
			}

			if (CurrentItem != null)
				CurrentItem.transform.parent = BackPos;

            CurrentItemID = ID;

            while (animator.GetCurrentAnimatorStateInfo(1).IsName("Pick"))
			{
				
				if (CurrentItem != null)
					CurrentItem.transform.parent = HandPos;
				
				 yield return new WaitForSeconds(Time.deltaTime);
			}
			

            GlobalGameManager.Instance.ShowWeaponUI(true);
			while (WeaponSwitchLayer.weight < 1)
			{
                WeaponSwitchLayer.weight = Mathf.Lerp(WeaponSwitchLayer.weight, 1, PosLerpTime * Time.deltaTime);
				yield return new WaitForSeconds(Time.deltaTime);
            }
            
        }
        */
    }
}
