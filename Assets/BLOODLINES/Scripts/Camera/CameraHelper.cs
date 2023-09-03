using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;

namespace LBSE
{
	public class CameraHelper : MonoBehaviour
	{
		[Header("Referances")]
        public Camera Camera;
		public Transform Pivot;
        public Transform AimTarget;

        [Header("Post Processing")]
		public PostProcessVolume EffectsVolume;
		public PostProcessProfile DefaultEffectsProfile;
		public PostProcessProfile WaterEffectsProfile;

		[Header("Camera Movement")]
		public float Speed = 5f;
		public float Sensitivity = 5f;
		public Vector2 RotationAngle = new Vector2(-30f, 70f);

        [Header("Camera Field Of View")]
		public float DefaultFOV = 60f;
		public float SprintFOV = 45f;
		
		[Header("Camera Position")]
		public Vector3 Default = new Vector3(0.75f, 1.5f, -2.5f);
		public Vector3 Aim = new Vector3(0.5f, 1.65f, -1.1f);
		public Vector3 Vehicle = new Vector3(0f, 1.5f, -7.5f);
		public float RecoilRestoreSpeed = 3f;
		public float FovLerpSpeed = 2f;

        [Header("Collision Detection")]
        public LayerMask ColliderLayers;
        public float DetectionDistance = 0.1f;

        private Vector3 currentCameraPosition;
		private Vector3 currentCameraRotation;
		internal Entity entity;
		internal bool inEffect;
		
		private void Start()
		{
			if (GlobalGameManager.Instance.LocalPlayer != null)
				entity = GlobalGameManager.Instance.LocalPlayer;
		}
		
		private void Update()
        {
			if (GlobalGameManager.Instance.State != UIState.Play)
				return;

            Position();
            Rotation();
            CheckCollider();
            EntityHandler();
        }

        private void Position()
        {
            transform.position = entity.transform.position;
			Pivot.localPosition = currentCameraPosition;
        }

        private void Rotation()
        {
            currentCameraRotation.x += InputManager.GetAxis("Mouse X") * Sensitivity;
            currentCameraRotation.y += InputManager.GetAxis("Mouse Y") * Sensitivity;
            currentCameraRotation.x = Mathf.Repeat(currentCameraRotation.x, 360f);
            currentCameraRotation.y = Mathf.Clamp(currentCameraRotation.y, RotationAngle.x, RotationAngle.y);
            Quaternion localRotation = Quaternion.Slerp(Pivot.localRotation, Quaternion.Euler(currentCameraRotation.y, 0f, currentCameraRotation.x), Speed * Time.deltaTime);
            Pivot.localRotation = localRotation;
            Camera.transform.localRotation = Quaternion.Slerp(Camera.transform.localRotation, Quaternion.Euler(Vector3.zero), RecoilRestoreSpeed * Time.deltaTime);
        }

        private void CheckCollider()
        {
            Vector3 vector = transform.position - Pivot.position;
            float maxDistance = Mathf.Abs(Default.z);
            if (Physics.SphereCast(Pivot.position, DetectionDistance, vector, out RaycastHit hit, maxDistance, ColliderLayers))
            {
                float distance = hit.distance;
                currentCameraPosition = Pivot.position + vector.normalized * distance;
                Debug.Log("CheckCollider SphereCast");
            }

        }

        private void EntityHandler()
        {
            if (entity.Get<InventoryManager>().CurrentItem != null && entity.Get<InventoryManager>().CurrentItem.ToggleAim)
            {
                if (entity.Get<InventoryManager>().CurrentItem.OverrideAimOffest)
                    currentCameraPosition = entity.Get<InventoryManager>().CurrentItem.AimOffest;
                else
                    currentCameraPosition = Aim;

                Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, entity.Get<InventoryManager>().CurrentItem.AimFOV, FovLerpSpeed * Time.deltaTime);
            }
            else if (entity.Get<PlayerController>().State == PlayerState.Mounted && entity.Get<PlayerMountSystem>().CurrentVehicle != null)
            {
                currentCameraPosition = Vehicle;
                Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, DefaultFOV, FovLerpSpeed * Time.deltaTime);
            }
            else if (entity.Get<PlayerController>().State == PlayerState.Sprint)
            {
                currentCameraPosition = Default;
                Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, SprintFOV, FovLerpSpeed * Time.deltaTime);
            }
            else
            {
                currentCameraPosition = Default;
                Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, DefaultFOV, FovLerpSpeed * Time.deltaTime);
            }

            EffectsVolume.profile = entity.Get<PlayerController>().State == PlayerState.Swimming ? WaterEffectsProfile : DefaultEffectsProfile;

            // Workaround for setting the MultiAimConstraintData Source Objects at runtime.
            entity.Get<InventoryManager>().AimPos.position = AimTarget.position;
            entity.Get<InventoryManager>().AimPos.rotation = AimTarget.rotation;
        }

        public void SetSpecialEffects(int effect, float timer)
		{
			StartCoroutine(SetEffects(effect, timer));
		}
		
		IEnumerator SetEffects(int effect, float timer)
		{
			switch (effect)
			{
				case 0:
					if (EffectsVolume.profile.HasSettings<DepthOfField>())
						EffectsVolume.profile.GetSetting<DepthOfField>().active = true;
					break;
				case 1:
					if (EffectsVolume.profile.HasSettings<Vignette>())
						EffectsVolume.profile.GetSetting<Vignette>().active = true;
						EffectsVolume.profile.GetSetting<Vignette>().intensity = new FloatParameter { value = 1f };
						EffectsVolume.profile.GetSetting<Vignette>().smoothness = new FloatParameter { value = 1f };
					break;
			}
			
			inEffect = true;
			yield return new WaitForSeconds(timer);
			inEffect = false;
			
			switch (effect)
			{
				case 0:
					if (EffectsVolume.profile.HasSettings<DepthOfField>())
						EffectsVolume.profile.GetSetting<DepthOfField>().active = false;
					break;
				case 1:
					if (EffectsVolume.profile.HasSettings<Vignette>())
						EffectsVolume.profile.GetSetting<Vignette>().intensity = new FloatParameter { value = 0.45f };
						EffectsVolume.profile.GetSetting<Vignette>().smoothness = new FloatParameter { value = 0f };
					break;
			}
		}
		
		public void Recoil(Vector2 recoil)
        {
            Camera.transform.localRotation *= Quaternion.Euler(-recoil);
        }

        public void Shake(float power, float duration)
        {
			StartCoroutine(ShakeCamera(power, duration));
        }

        IEnumerator ShakeCamera(float power, float duration)
        {
            while (duration >= 0f)
            {
                Vector2 vector = Random.insideUnitCircle * power;
                Camera.transform.position = new Vector3(Camera.transform.position.x + vector.x, Pivot.position.y + vector.y, Camera.transform.position.z);
                duration -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
            }
			
			while (Camera.transform.localPosition != Vector3.zero)
			{
				Camera.transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, new Vector3(0f, 0f, 0f), RecoilRestoreSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
	
		private static CameraHelper instance;
		public static CameraHelper Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<CameraHelper>(); }
				return instance;
			}
		}
	}
}