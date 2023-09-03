using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LBSE
{
	public enum WeaponType
	{ 
		Assault = 0,
		Sniper = 1,
		Pistol = 2,
		Heavy = 3,
		Smg = 4,
		Launcher = 5,
		Melee = 6
	}

	public enum AmmoType
	{
		LightAmmo = 0,
		SniperAmmo = 1,
		HeavyAmmo = 2,
		ShotgunShell = 3,
		Rockets = 4
	}

	public enum TargetingType
	{
		Raycast = 0,
		Projectile = 1
	}

	public enum FireType
	{
		Auto = 0,
		Single = 2
	}
	
	[RequireComponent(typeof(AudioSource))]
	public class Weapon : MonoBehaviour
	{
		[Header("PUBLIC DETAILS")]
		public ItemData ItemInfo;
		public int CrosshairType;
		
		public TargetingType targetingType = TargetingType.Raycast;
		public FireType fireType = FireType.Auto;
		public WeaponType weaponType;
		
		[Header("GENERIC DETAILS")]
		internal bool isEnabled;
		internal Entity Entity;
		public AudioSource audioSource;
		
		[Header("AIM DETAILS")]
		public Texture2D Scope;
		public Color ScopeColor;
		public bool ToggleAim;
		public float AimFOV = 45f;
		public MeshRenderer[] MeshToHideOnAim;
		public bool OverrideAimOffest;
		[BoolShowConditional(nameof(OverrideAimOffest), true)]
		public Transform AimPosition;
		public Vector3 AimOffest { get { return CameraHelper.Instance.transform.position - AimPosition.position; } }
		
		[Header("HAND IK DETAILS")]
		public float AnimationSetIndex;
		public Vector3 PositionOffest;
		public Vector3 RotationOffest;
		public Vector3 DisabledPositionOffest;
		public Vector3 DisabledRotationOffest;
		[Range(0, 1)] public float rightHandHintWeight;
		[Range(0, 1)] public float rightHandWeight;
		public Transform rightHandObj;
		[Range(0, 1)] public float leftHandHintWeight;
		[Range(0, 1)] public float leftHandWeight;
		public Transform leftHandObj;
		
		[Header("PROJECTILE SHOT"), StringShowConditional(nameof(targetingType), nameof(TargetingType.Projectile))]
		public Transform LaunchPos;
		[StringShowConditional(nameof(targetingType), nameof(TargetingType.Projectile))]
		public GameObject Projectile;
		
		[Header("DAMAGE")]
		public float Damage = 15f;
		public float ForceMultiplier = 5f;
		
		[Header("BULLET RANGE")]
		public float Range = 500f;
		public float MeleeFOV = 90f;
		
		[Header("FIRE RATE")]
		public float FireRate = 0.1f;
		public float FireRateBurst = 1f;
		private float FireTimer;
		
		[Header("AMMO")]
		public AmmoType ammoType;
		public bool InfinityAmmo = false;
		public bool infinityAmmo
		{
			get
			{
				if (Entity.Get<HealthSystem>().Invincible || weaponType == WeaponType.Melee || Entity.Type == EntityType.HumanAI)
					return true;
				
				return InfinityAmmo;
			}
		}
		public int CurrentAmmo { get { return currentAmmo; } }
		private int currentAmmo;
		public int AmmoPerMag = 30;
		public int AmmoCapacity
		{
			get
			{
				return Entity.Get<InventoryManager>().GetAmmo(ammoType);
			}
			set
			{
				Entity.Get<InventoryManager>().SetAmmo(ammoType, value);
			}
		}
		
		public int ShotsPerRound = 1;
		public float ReloadTime = 2.0f;
		public bool ReloadAutomatically = true;
		
		[Header("Visual Effects")]
		public bool BulletHitEffects = true;
		public ParticleSystem Muzzle;
		
		[Header("ACCURACY")]
		[Range(0, 100)] public float Accuracy = 80.0f;
		public Vector2 Recoil;
		
		[Header("AUDIO")]
		public AudioClip fireSound;
		public AudioClip dryFireSound;
		public AudioClip reloadSound;

		private bool canFire = true;
		private bool isInitlized = false;
        public bool Triggered;

        void Start()
		{
			if(!isInitlized)
			{
				currentAmmo = AmmoPerMag;
				audioSource = GetComponent<AudioSource>();
				if (weaponType == WeaponType.Assault || weaponType == WeaponType.Pistol || weaponType == WeaponType.Smg)
				{
					ammoType = AmmoType.LightAmmo;
				}
				if (weaponType == WeaponType.Sniper)
				{
					ammoType = AmmoType.SniperAmmo;
				}
				if (weaponType == WeaponType.Heavy)
				{
					ammoType = AmmoType.HeavyAmmo;
				}
				if (weaponType == WeaponType.Launcher)
				{
					ammoType = AmmoType.Rockets;
				}
			}

			isInitlized = true;
		}
		
		void Update()
		{
			if (!isEnabled)
				return;
			
			FireTimer += Time.deltaTime;
			InputControls();
			foreach (MeshRenderer mesh in MeshToHideOnAim)
			{
				mesh.enabled = !ToggleAim;
			}
		}

		void InputControls()
		{
			if (!isEnabled|| !Entity || !Entity.Enabled || Entity.Type != EntityType.Player)
				return;
			
			if (FireTimer >= FireRate && canFire && InputManager.GetButton("Fire"))
			{
                Triggered = true;
                switch (targetingType)
				{
					case TargetingType.Raycast:
						Fire();
						break;
					case TargetingType.Projectile:
						Launch();
						break;
				}
			}
			
			if (!infinityAmmo && (InputManager.GetButtonDown("Reload") || (ReloadAutomatically && currentAmmo <= 0)))
				StartCoroutine(Reload());
			
			if (InputManager.GetButtonDown("Aim"))
				ToggleAim = !ToggleAim;

			if (InputManager.GetButtonUp("Fire"))
			{
				canFire = true;
                Triggered = false;
            }
		}
		
		public void AIInputControls()
		{
			if (Entity.Type != EntityType.HumanAI)
				return;
			
			if (FireTimer >= FireRate && canFire)
			{
				switch (targetingType)
				{
					case TargetingType.Raycast:
						Fire();
						break;
					case TargetingType.Projectile:
						Launch();
						break;
				}
			}
			
			if (currentAmmo <= 0 && !InfinityAmmo)
			{
				StartCoroutine(Reload());
			}
		}
		
		////////// MOBILE SETUP BEGINS
		
		public void Aim()
		{
			ToggleAim = !ToggleAim;
		}
		
		////////// MOBILE SETUP END
		
		void MeleeAttack()
		{
			Entity.Get<Animator>().SetTrigger("Melee");
			var colliders = Physics.OverlapSphere (transform.position, Range);
			foreach (var hit in colliders)
			{
				if (hit.gameObject == gameObject || hit.gameObject.tag != "HitSpot")
					continue;
				
				HitSpot health = hit.gameObject.GetComponent<HitSpot>();
				if (health.healthSystem.gameObject == Entity.gameObject)
					continue;
				
				var dir = hit.transform.position - Entity.transform.position;
				var angle = Vector3.Angle(dir, Entity.transform.position);
				if (angle > MeleeFOV)
					continue;
				
				if (health)
				{
					if (fireSound != null)
						AudioSource.PlayClipAtPoint (fireSound, Entity.transform.position);	
										
					health.TakeDamage(Damage, Entity.gameObject, ItemInfo.GlobalID);
					
					if (hit.GetComponent<Rigidbody>())
						hit.GetComponent<Rigidbody>().AddForce(hit.transform.forward * Damage * ForceMultiplier);
					break;
				}
			}
        }

		void Fire()
		{
            FireTimer = 0.0f;
			canFire = (fireType != FireType.Single);
			
			if (currentAmmo <= 0 && !infinityAmmo)
			{
				audioSource.PlayOneShot(dryFireSound);
				return;
			}
			
			if (weaponType == WeaponType.Melee)
			{
				MeleeAttack();
				return;
			}

			if (!infinityAmmo)
				currentAmmo--;
			
			if (Muzzle != null)
				Muzzle.Play();
			
			CameraHelper.Instance.Recoil(Recoil);
			
			for (int i = 0; i < ShotsPerRound; i++)
			{
				Vector3 position = Entity.Type == EntityType.Player ? CameraHelper.Instance.Camera.transform.position : Entity.Get<AIHuman>().AimPos.position;
				Vector3 direction = Entity.Type == EntityType.Player ? CameraHelper.Instance.Camera.transform.TransformDirection(Vector3.forward) : Entity.Get<AIHuman>().AimPos.TransformDirection(Vector3.forward);
				
				float shotAccuracy = Random.Range(-((100 - Accuracy) / 1000), ((100 - Accuracy) / 1000));
				direction += new Vector3(shotAccuracy, shotAccuracy, shotAccuracy);
				Ray ray = new Ray(position, direction);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, Range))
				{
					if (hit.collider.tag == "HitSpot")
					{
						HitSpot health = hit.collider.GetComponent<HitSpot>();
						if(health)
							health.TakeDamage(Damage, Entity.gameObject, ItemInfo.GlobalID);
						
						if (Entity.Type == EntityType.Player)
							GlobalGameManager.Instance.HitMarker.alpha = 1f;
					}
					
					if (BulletHitEffects && hit.collider.tag != "untagged")
					{
						SurfaceTag surface = GameData.Instance.GetSurfaceTag(hit.collider.tag);
						if (surface != null)
						{
							GameObject GO = surface.SurfacePrefab[Random.Range(0, surface.SurfacePrefab.Length)];
							if (GO != null)
							{
								GameObject GOPooled = ObjectPool.Instance.Spawn(GO, hit.point + (hit.normal * 0.1f), Quaternion.LookRotation(hit.normal));
								if (GOPooled != null) ObjectPool.Instance.Despawn(GOPooled, 5f);
							}
						}
					}
					
					if (hit.rigidbody)
						hit.rigidbody.AddForce(ray.direction * Damage * ForceMultiplier);
				}
			}
			
			audioSource.PlayOneShot(fireSound);
			if (Entity.Type == EntityType.HumanAI)
				canFire = true;
        }

		public void Launch()
		{
            FireTimer = 0.0f;
			canFire = (fireType != FireType.Single);

			if (currentAmmo <= 0 && !infinityAmmo)
			{
				audioSource.PlayOneShot(dryFireSound);
				return;
			}
			
			if (weaponType == WeaponType.Melee)
			{
				MeleeAttack();
				return;
			}
			
			if (!infinityAmmo)
				currentAmmo--;
			
			if (Muzzle != null)
				Muzzle.Play();
			
			CameraHelper.Instance.Recoil(Recoil);
			
			for (int i = 0; i < ShotsPerRound; i++)
			{
				// Instantiate the Projectile
				if (Projectile != null)
				{
					Projectile GO = Instantiate(Projectile, LaunchPos.position, LaunchPos.rotation).GetComponent<Projectile>();
					GO.Set(Damage, 10f, GO.Force * ForceMultiplier, 10f);
				}
				else
				{
					Debug.Log("Projectile GameObject is Null please Assign it.");
				}
			}
			
			audioSource.PlayOneShot(fireSound);
			if (Entity.Type == EntityType.HumanAI)
				canFire = true;
        }
		
		IEnumerator Reload()
		{
			if (AmmoCapacity <= 0)
				yield return null;
			
			ToggleAim = false;
			Entity.Get<Animator>().SetTrigger("Reload");
			
			yield return new WaitForSeconds(ReloadTime);
			
			if (AmmoCapacity < AmmoPerMag)
			{
				int NeededAmmo = AmmoPerMag - AmmoCapacity;
				currentAmmo += AmmoCapacity;
				AmmoCapacity = 0;
			}
			else
			{
				int NeededAmmo = AmmoPerMag - currentAmmo;
				currentAmmo += NeededAmmo;
				AmmoCapacity -= NeededAmmo;
			}
			
			if (currentAmmo > AmmoPerMag)
				currentAmmo = AmmoPerMag;
			
			FireTimer = -ReloadTime;
			audioSource.PlayOneShot(reloadSound);
		}
		
		float Alpha = 0;
		void OnGUI()
		{
			if (Scope == null || !isEnabled || Entity.Type != EntityType.Player)
				return;
			
			if (ToggleAim)
				Alpha = Mathf.Lerp(Alpha, 1, Time.deltaTime * 3);
			else
				Alpha = Mathf.Lerp(Alpha, 0, Time.deltaTime * 3);
			
			if(Alpha > 0)
			{
				ScopeColor.a = Alpha;
				GUI.color = ScopeColor;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Scope, ScaleMode.ScaleToFit);
			}
		}
	}
}
