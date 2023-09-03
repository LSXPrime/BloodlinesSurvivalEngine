using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
    public class EntityInfo : MonoBehaviour
    {
		public GameObject Content;
		public Slider Health;
		public float DetectionRange = 10f;
		
		private HealthSystem Target;
		
		RaycastHit hit;
		Ray ray;
		
        void Update()
        {
			Content.SetActive(Target);
			if (Target != null)
			{
				Health.value = Target.health;
				Health.maxValue = Target.maxHealth;
				
				Vector3 direction = Target.transform.position - CameraHelper.Instance.Camera.transform.position;
				float angle = Vector3.Angle(direction, CameraHelper.Instance.Camera.transform.forward);
				if (Vector3.Distance(Target.transform.position, CameraHelper.Instance.Camera.transform.position) > DetectionRange || angle > 60f || !Target.alive) Target = null;
				return;
			}
			
			ray = new Ray(CameraHelper.Instance.Camera.transform.position, CameraHelper.Instance.Camera.transform.TransformDirection(Vector3.forward));
			if (Physics.Raycast(ray, out hit, DetectionRange))
			{
				if (!hit.collider.gameObject.CompareTag("HitSpot"))
					return;
				
				HitSpot target = hit.collider.gameObject.GetComponent<HitSpot>();
				if (target && target.healthSystem.alive && target.healthSystem != GlobalGameManager.Instance.LocalPlayer.Get<HealthSystem>())
					Target = target.healthSystem;
			}
        }
    }
}
