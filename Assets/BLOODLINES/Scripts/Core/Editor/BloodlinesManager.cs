using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LBSE
{
	public class BloodlinesManager : EditorWindow
	{
		//X Player Creation Tool
		private GameObject PlayerInstantiated;
		private GameObject PlayerModel;
		private RuntimeAnimatorController animatorController;
		private Animator PlayerAnimator;
		private Avatar PlayerModelAvatar;
		private ModelImporter ModelInfo;
		
		//X Weapon Creation Tool
		private GameObject WeaponModel = null;
		private GameObject WeaponInstantiated = null;
		private GameObject WeaponPickupInstantiated = null;
		private string weaponName = string.Empty;
		private string weaponDesc = string.Empty;
		private int weaponType = 0;
		private int weaponGlobalID = 0;
		private int weaponPrice = 0;
		private float weaponWeight = 0;
		private float weaponDamage = 0;
		private float weaponRange = 0;
		private Vector2 weaponRecoil = Vector2.zero;
		private float weaponAccuracy = 0;
		private int weaponAmmoPerMag = 0;
		private Sprite weaponIcon = null;
		private AudioClip fireSound = null;
		private AudioClip dryFireSound = null;
		private AudioClip reloadSound = null;
		
		//X Consumable Creation Tool
		private GameObject ConsumableModel = null;
		private GameObject ConsumablePickupInstantiated = null;
		private string consumableName = string.Empty;
		private string consumableDesc = string.Empty;
		private int consumableType = 0;
		private int consumableAmmoType = 0;
		private int consumableGlobalID = 0;
		private int consumablePrice = 0;
		private float consumableWeight = 0;
		private int consumableAmount = 0;
		private float consumableHealth, consumableShield, consumableStamina, consumableThirst, consumableHunger;
		private bool consumableIsBandage = false;
		private bool consumableIsSpoiled = false;
		private bool consumableIsMedicine = false;
		private int consumablePoisonChance, consumableDiseaseID, consumableMedicineID;
		private Sprite consumableIcon = null;
		
		//X Vehicle Creation Tool
		private GameObject VehicleModel = null;
		private GameObject VehicleInstantiated = null;
		private GameObject VehicleDoorMain = null;
		private GameObject VehicleDoor2 = null;
		private GameObject VehicleDoor3 = null;
		private GameObject VehicleDoor4 = null;
		private GameObject VehicleWheel1 = null;
		private GameObject VehicleWheel2 = null;
		private GameObject VehicleWheel3 = null;
		private GameObject VehicleWheel4 = null;
		private string vehicleName = string.Empty;
		private int vehicleType = 0;
		
		//X for all selections
		private int WindowID = 0;
		private int FirstStep = 0;
		private int SecStep = 0;
		
		[MenuItem("LSXGaming/BLOODLINES/Manager", false, 500)]
		private static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(BloodlinesManager));
		}
		
		public void OnEnable()
		{
			animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/BLOODLINES/Art/Animators/Player.controller") as RuntimeAnimatorController;
		}
		
		private void OnGUI()
		{
			EditorStyles.textArea.richText = true;
			EditorStyles.label.richText = true;
			EditorStyles.toolbarButton.richText = true;
			EditorStyles.boldLabel.richText = true;
			GUI.skin.label.richText = true;
			ChangWindow(WindowID, FirstStep);
		}
		
		public bool DrawButton(string text)
		{
			return GUILayout.Button(text, EditorStyles.toolbarButton, new GUILayoutOption[0]);
		}
		
		public void DrawText(string text)
		{
			GUILayout.TextArea(text, GUI.skin.textArea, new GUILayoutOption[0]);
			GUILayout.Space(5f);
		}
		
		public void ChangWindow(int window, int step)
		{
			if (window == 0)
			{
				StarterInfo();
			}
			else if (window == 1)
			{
				if (step == 0)
				{
					MakeMortal();
				}
				else if (step == 1)
				{
					SetupPlayer();
				}
			}
			else if (window == 2)
			{
				if (step == -1)
					AICreation();
				else
					MakeAI(step);
			}
			else if (window == 3)
			{
				if (step == 0)
				{
					MakeWeapon();
				}
				else if (step == 1)
				{
					SetupWeaponComponents();
				}
			}
			else if (window == 4)
			{
				if (step == 0)
				{
					MakeConsumable();
				}
				else if (step == 1)
				{
					SetupConsumableComponents();
				}
			}
			else if (window == 5)
			{
				if (step == 0)
				{
					MakeVehicle();
				}
				else if (step == 1)
				{
					SetupVehicleComponents();
				}
			}
		}
		
		void StarterInfo()
		{
			Texture2D Logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BLOODLINES/Art/UI/Logo.png") as Texture2D;
			GUILayout.Label(Logo, new GUILayoutOption[0]);
			GUILayout.Label("<b><size=26>BLOODLINES MANAGER</size></b>", new GUILayoutOption[0]);
			GUILayout.Label("<b>		Version: 1.0</b>", new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.FlexibleSpace();
			DrawText("Select your Current Task");
			GUILayout.Space(2f);
			
			if (DrawButton("Create Player"))
			{
				SecStep = 0;
				FirstStep = 0;
				WindowID = 1;
			}
			if (DrawButton("Create AI Character"))
			{
				SecStep = 0;
				FirstStep = -1;
				WindowID = 2;
			}
			if (DrawButton("Create Weapon"))
			{
				SecStep = 0;
				FirstStep = 0;
				WindowID = 3;
			}
			if (DrawButton("Create Consumable Item"))
			{
				SecStep = 0;
				FirstStep = 0;
				WindowID = 4;
			}
			if (DrawButton("Create Vehicle"))
			{
				SecStep = 0;
				FirstStep = 0;
				WindowID = 5;
			}
		}
		
		void MakeMortal()
		{
			if (SecStep == 0)
			{
				DrawText("Character Creation Tool helping you to create Main Player Character , you will need:");
				GUILayout.FlexibleSpace();
				DrawText("Player Model , A Humanoid & Rigged & T-Posed 3D Character Model.");
				GUILayout.FlexibleSpace();
				DrawText("If your model ready this tool will create the HitSpot Colliders on the Bones to Detect The Damage.");
				GUILayout.FlexibleSpace();
				DrawText("Drag your model from the Project View");
				PlayerModel = EditorGUILayout.ObjectField("Player Model", PlayerModel, typeof(GameObject), false) as GameObject;
				GUI.enabled = PlayerModel != null;
				
				if (PlayerModel != null)
				{
					if (DrawButton("NEXT"))
					{
						AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(PlayerModel));
						if (assetImporter != null)
						{
							ModelInfo = assetImporter as ModelImporter;
							if (ModelInfo != null)
							{
								if (ModelInfo.animationType != ModelImporterAnimationType.Human)
								{
									ModelInfo.animationType = ModelImporterAnimationType.Human;
									EditorUtility.SetDirty(ModelInfo);
									ModelInfo.SaveAndReimport();
								}
								if (ModelInfo.animationType == ModelImporterAnimationType.Human)
								{
									PlayerInstantiated = PrefabUtility.InstantiatePrefab(PlayerModel) as GameObject;
									if (PrefabUtility.GetPrefabInstanceStatus(PlayerInstantiated) == PrefabInstanceStatus.Connected)
									{
										PrefabUtility.UnpackPrefabInstance(PlayerInstantiated, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
									}
									PlayerAnimator = PlayerInstantiated.GetComponent<Animator>();
									PlayerModelAvatar = PlayerAnimator.avatar;
									var view = (SceneView)SceneView.sceneViews[0];
									view.camera.transform.position = PlayerInstantiated.transform.position + ((PlayerInstantiated.transform.forward * 10) + (new Vector3(0, 0.5f, 0)));
									view.LookAt(PlayerInstantiated.transform.position);
									EditorGUIUtility.PingObject(PlayerInstantiated);
									Selection.activeTransform = PlayerInstantiated.transform;
									SecStep++;
								}
								else
								{
									GUILayout.Label("Your model isn't Humanoid or isn't correctly Rigged");
								}
							}
						}
						else 
						{
							GUILayout.Label("Prefab Detected , you should select the source Model");
						}
					}
				}
				GUI.enabled = true;
			}
			else if (SecStep == 1)
			{
				GUI.enabled = false;
				GUILayout.BeginVertical("box");
				PlayerInstantiated = EditorGUILayout.ObjectField("Player Prefab", PlayerInstantiated, typeof(GameObject), false) as GameObject;
				PlayerModelAvatar = EditorGUILayout.ObjectField("Avatar", PlayerModelAvatar, typeof(Avatar), true) as Avatar;
				GUILayout.EndVertical();
				GUI.enabled = true;
				if (PlayerModelAvatar != null && PlayerAnimator != null)
				{
					GUILayout.FlexibleSpace();
					DrawText("Model looks fine , would you to start ?");
					if (DrawButton("START"))
					{
						if (CreateHitSpotsWizard.Create(PlayerAnimator))
						{
							var view = (SceneView)SceneView.sceneViews[0];
							Debug.Log("HitSpots Created, now u should assign AffectedBone on HitSpots Manually for Inventory!");
							view.ShowNotification(new GUIContent("HitSpots Created, now u should assign AffectedBone on HitSpots Manually!"));
						   SecStep++;
						}
						else
						{
							GUILayout.Label("<color=red>Something looks wrong with your model Avatar</color>", EditorStyles.label);
                            Debug.Log("Something looks wrong with your model Avatar");
                        }
					}
				}
				else
				{
					GUILayout.Label("<color=red>Something looks wrong with your model , does it contains Avatar?.</color>", EditorStyles.label);
				}
			}
			else if (SecStep == 2)
			{
				DrawText("HitSpot Bones Colliders Created:");
				DrawText("HitSpots Created, now u should assign AffectedBone on HitSpots Manually for Inventory!");
				GUILayout.FlexibleSpace();
				DrawText("these Capsule Trigger Colliders are player HitSpots that make him killable.");
				GUILayout.FlexibleSpace();
				GUILayout.FlexibleSpace();
				if (DrawButton("NEXT STEP"))
				{
					SecStep = 0;
					FirstStep = 1;
				}
			}
		}

		void SetupPlayer()
		{
			if (SecStep == 0)
			{
				PlayerInstantiated = EditorGUILayout.ObjectField("Player Prefab", PlayerInstantiated, typeof(GameObject), true) as GameObject;
				if (PlayerInstantiated != null)
				{
					DrawText("All good, click in the button below to setup the model in the player prefab.");
					GUILayout.Space(5);
					if (DrawButton("SETUP MODEL"))
					{
						SetupPlayerComponents();
						SecStep++;
					}
				}
			}
			else if (SecStep == 1)
			{
				DrawText("Mount Points Setup");
				if (DrawButton("Select Weapons Mount Point"))
				{
					if (PlayerInstantiated != null)
					{
						Transform t = PlayerInstantiated.GetComponent<InventoryManager>().HandPos;
						Selection.activeTransform = t;
						EditorGUIUtility.PingObject(t);
					}
				}
				if (DrawButton("Select Back Weapons Mount Point"))
				{
					if (PlayerInstantiated != null)
					{
						Transform t = PlayerInstantiated.GetComponent<InventoryManager>().BackPos;
						Selection.activeTransform = t;
						EditorGUIUtility.PingObject(t);
						if (t != null)
							SecStep++;
					}
				}
				DrawText("Now you have the Weapons Mount Point (HandPos) transform , you should Adjust it's position and rotation as your weapons model commonly require.");
				DrawText("And the Back Weapons Mount Point (BackPos) transform , you should Adjust it's position and rotation to carry unused weapons on the character back as your weapons model commonly require.");

				if (DrawButton("NEXT"))
				{
					SecStep++;
				}
			}
			else if (SecStep == 2)
			{
				DrawText("Now Drag the prefab from the hierarchy to the Resources folder & Rename it as you would.");
				DrawText("Now you should assign it to (Player) variable in GameData.");
				DrawText("Character Creation Process Completed.");
				if (DrawButton("Menu"))
				{
					SecStep = 0;
					FirstStep = 0;
					WindowID = 0;
				}
			}
		}

		void SetupPlayerComponents()
		{
			
			PlayerStatus ps = PlayerInstantiated.AddComponent<PlayerStatus>();
			PlayerController pc = PlayerInstantiated.GetComponent<PlayerController>();
			InventoryManager im = PlayerInstantiated.GetComponent<InventoryManager>();
			HealthSystem hs = PlayerInstantiated.GetComponent<HealthSystem>();
			PlayerAnimator = PlayerInstantiated.GetComponent<Animator>();
			PlayerAnimator.runtimeAnimatorController = animatorController;
			PlayerInstantiated.tag = "Player";
			PlayerInstantiated.layer = 3;
			

			pc.ColliderCenter[0] = new Vector3(0f, 1f, 0f);
			pc.ColliderCenter[1] = new Vector3(0f, 0.6f, 0f);
			pc.ColliderHeight[0] = 1.8f;
			pc.ColliderHeight[1] = 1;

			GameObject HandPos = new GameObject();
			GameObject BackPos = new GameObject();
				
			Transform RightHand = PlayerAnimator.GetBoneTransform(HumanBodyBones.RightHand);

			HandPos.name = "HandPos";
			HandPos.transform.parent = RightHand;
			HandPos.transform.localPosition = Vector3.zero;
			HandPos.transform.rotation = RightHand.rotation;
				
			Transform Spine = PlayerAnimator.GetBoneTransform(HumanBodyBones.Spine);
			BackPos.name = "BackPos";
			BackPos.transform.parent = Spine;
			BackPos.transform.localPosition = Vector3.zero;
			BackPos.transform.rotation = Spine.rotation;
				
			im.HandPos = HandPos.transform;
			im.BackPos = BackPos.transform;
				
			hs.HurtSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/BLOODLINES/Art/Sounds/Characters/PlayerHurt.mp3") as AudioClip;
			
			var view = (SceneView)SceneView.sceneViews[0];
			view.ShowNotification(new GUIContent("Player Setup Done"));
			Debug.Log("Player Setup Done.");
		}
		
		void AICreation()
		{
			Texture2D Logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BLOODLINES/Art/UI/Logo.png") as Texture2D;
			GUILayout.Label(Logo, new GUILayoutOption[0]);
			GUILayout.Label("<b><size=26>BLOODLINES MANAGER</size></b>", new GUILayoutOption[0]);
			GUILayout.Label("<b>		Version: 1.0</b>", new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.FlexibleSpace();
			DrawText("Select AI Type you want to Create");
			GUILayout.Space(2f);
			
			if (DrawButton("Create AI Player / Human"))
			{
				SecStep = 0;
				FirstStep = 0;
				WindowID = 2;
			}
			if (DrawButton("Create AI Zombie / Monster"))
			{
				SecStep = 0;
				FirstStep = 1;
				WindowID = 2;
			}
			if (DrawButton("Create AI Boss"))
			{
				SecStep = 0;
				FirstStep = 2;
				WindowID = 2;
			}
		}
		
		void MakeAI(int step)
		{
			if (SecStep == 0)
			{
				DrawText("AI Creation Tool helping you to create Zombie Characters , you will need:");
				GUILayout.FlexibleSpace();
				DrawText("Charcter Model , A Humanoid & Rigged 3D Model.");
				GUILayout.FlexibleSpace();
				DrawText("If your model ready this tool will create the HitSpot Colliders on the Bones to Detect The Damage.");
				GUILayout.FlexibleSpace();
				DrawText("Drag your model from the Project View");
				PlayerModel = EditorGUILayout.ObjectField("Character Model", PlayerModel, typeof(GameObject), false) as GameObject;
				GUI.enabled = PlayerModel != null;
				
				if (PlayerModel != null)
				{
					if (DrawButton("NEXT"))
					{
						AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(PlayerModel));
						if (assetImporter != null)
						{
							ModelInfo = assetImporter as ModelImporter;
							if (ModelInfo != null)
							{
								if (ModelInfo.animationType != ModelImporterAnimationType.Human)
								{
									ModelInfo.animationType = ModelImporterAnimationType.Human;
									EditorUtility.SetDirty(ModelInfo);
									ModelInfo.SaveAndReimport();
								}
								if (ModelInfo.animationType == ModelImporterAnimationType.Human)
								{
									PlayerInstantiated = PrefabUtility.InstantiatePrefab(PlayerModel) as GameObject;
									if (PrefabUtility.GetPrefabInstanceStatus(PlayerInstantiated) == PrefabInstanceStatus.Connected)
									{
										PrefabUtility.UnpackPrefabInstance(PlayerInstantiated, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
									}
									PlayerAnimator = PlayerInstantiated.GetComponent<Animator>();
									PlayerModelAvatar = PlayerAnimator.avatar;
									var view = (SceneView)SceneView.sceneViews[0];
									view.camera.transform.position = PlayerInstantiated.transform.position + ((PlayerInstantiated.transform.forward * 10) + (new Vector3(0, 0.5f, 0)));
									view.LookAt(PlayerInstantiated.transform.position);
									EditorGUIUtility.PingObject(PlayerInstantiated);
									Selection.activeTransform = PlayerInstantiated.transform;
									SecStep++;
								}
								else
								{
									GUILayout.Label("Your model isn't Humanoid or isn't correctly Rigged");
								}
							}
						}
						else 
						{
							GUILayout.Label("Prefab Detected , you should select the source Model");
						}
					}
				}
				GUI.enabled = true;
			}
			else if (SecStep == 1)
			{
				GUI.enabled = false;
				GUILayout.BeginVertical("box");
				PlayerInstantiated = EditorGUILayout.ObjectField("Player Prefab", PlayerInstantiated, typeof(GameObject), false) as GameObject;
				PlayerModelAvatar = EditorGUILayout.ObjectField("Avatar", PlayerModelAvatar, typeof(Avatar), true) as Avatar;
				GUILayout.EndVertical();
				GUI.enabled = true;
				if (PlayerModelAvatar != null && PlayerAnimator != null)
				{
					GUILayout.FlexibleSpace();
					DrawText("Model looks fine , would you to start ?");
					if (DrawButton("START"))
					{
						if (CreateHitSpotsWizard.Create(PlayerAnimator))
						{
							var view = (SceneView)SceneView.sceneViews[0];
							view.ShowNotification(new GUIContent("Capsules Done Created!"));
						   SecStep++;
						}
					}
				}
				else
				{
					GUILayout.Label("<color=red>Something looks wrong with your model , does it contains Avatar?.</color>", EditorStyles.label);
				}
			}
			else if (SecStep == 2)
			{
				PlayerInstantiated = EditorGUILayout.ObjectField("AI Character Prefab", PlayerInstantiated, typeof(GameObject), true) as GameObject;
				if (PlayerInstantiated != null)
				{
					DrawText("All good, click in the button below to setup the model in the Zombie prefab.");
					GUILayout.Space(5);
					if (DrawButton("SETUP MODEL"))
					{
						SetupAIComponents(step);
						SecStep++;
					}
				}
			}
			else if (SecStep == 3)
			{
				DrawText("Now Drag the prefab from the hierarchy to the Resources folder & Rename it as you would.");
				DrawText("Now you should assign it to Enemies List in ur Scene EnemySpawners or Spawn Manually in scene.");
				DrawText("AI Creation Process Completed.");
				if (DrawButton("Menu"))
				{
					SecStep = 0;
					FirstStep = 0;
					WindowID = 0;
				}
			}
		}

		void SetupAIComponents(int step)
		{
			PlayerAnimator = PlayerInstantiated.GetComponent<Animator>();
			switch (step)
			{
				case 0:
					AIHuman aih = PlayerInstantiated.AddComponent<AIHuman>();
					GameObject AimPos = new GameObject();
					GameObject HandPos = new GameObject();
					GameObject BackPos = new GameObject();
					
					Transform RightHand = PlayerAnimator.GetBoneTransform(HumanBodyBones.RightHand);
					Transform Spine = PlayerAnimator.GetBoneTransform(HumanBodyBones.Spine);
					
					AimPos.name = "AimPos";
					AimPos.transform.parent = PlayerInstantiated.transform;
					AimPos.transform.localPosition = new Vector3(0, 1f, 0.5f);
					AimPos.transform.rotation = RightHand.rotation;
					
					HandPos.name = "HandPos";
					HandPos.transform.parent = RightHand;
					HandPos.transform.localPosition = Vector3.zero;
					HandPos.transform.rotation = RightHand.rotation;
					
					BackPos.name = "BackPos";
					BackPos.transform.parent = Spine;
					BackPos.transform.localPosition = Vector3.zero;
					BackPos.transform.rotation = Spine.rotation;
						
					aih.AimPos = AimPos.transform;
					aih.HandPos = HandPos.transform;
					aih.BackPos = BackPos.transform;
					PlayerAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/BLOODLINES/Art/Animators/AIHuman.controller") as RuntimeAnimatorController;
					break;
				case 1:
					AIZombie aiz = PlayerInstantiated.AddComponent<AIZombie>();
					PlayerAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/BLOODLINES/Art/Animators/AIZombie.controller") as RuntimeAnimatorController;
					break;
				case 2:
					AIBoss aib = PlayerInstantiated.AddComponent<AIBoss>();
					PlayerAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/BLOODLINES/Art/Animators/AIZombie.controller") as RuntimeAnimatorController;
					break;
			}
			AIBase ai = PlayerInstantiated.GetComponent<AIBase>();
			HealthSystem hs = PlayerInstantiated.GetComponent<HealthSystem>();
			PlayerInstantiated.tag = "AI";
			
			if (ai !=null)
			{
				AudioClip attackSO = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/BLOODLINES/Art/Sounds/Characters/ZombieAttack.mp3") as AudioClip;
				ai.AttackSounds = new AudioClip[1];
				ai.AttackSounds[0] = attackSO;
			}

			if (hs !=null)
			{
				hs.characterType = CharacterType.Zombie;
				hs.HurtSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/BLOODLINES/Art/Sounds/Characters/PlayerHurt.mp3") as AudioClip;
			}
			
			var view = (SceneView)SceneView.sceneViews[0];
			view.ShowNotification(new GUIContent("AI Setup Done"));
			Debug.Log("AI Setup Done.");
		}
		
		void MakeWeapon()
		{
			if (SecStep == 0)
			{
				DrawText("Weapon Creation Tool helping you to create Weapons / Firearms:");
				DrawText("If your model ready this tool will create Add The Weapon Components.");
				GUILayout.FlexibleSpace();
				DrawText("Drag your model from the Project View");
				WeaponModel = EditorGUILayout.ObjectField("Weapon Model", WeaponModel, typeof(GameObject), false) as GameObject;
				
				if (WeaponModel != null)
				{
					if (DrawButton("NEXT"))
					{
						AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(WeaponModel));
						if (assetImporter != null)
						{
							ModelInfo = assetImporter as ModelImporter;
							if (ModelInfo != null)
							{
								WeaponInstantiated = PrefabUtility.InstantiatePrefab(WeaponModel) as GameObject;
								if (PrefabUtility.GetPrefabInstanceStatus(WeaponInstantiated) == PrefabInstanceStatus.Connected)
								{
									PrefabUtility.UnpackPrefabInstance(WeaponInstantiated, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
								}
								var view = (SceneView)SceneView.sceneViews[0];
								view.camera.transform.position = WeaponInstantiated.transform.position + ((WeaponInstantiated.transform.forward * 10) + (new Vector3(0, 0.5f, 0)));
								view.LookAt(WeaponInstantiated.transform.position);
								EditorGUIUtility.PingObject(WeaponInstantiated);
								Selection.activeTransform = WeaponInstantiated.transform;
								GUILayout.FlexibleSpace();
								SecStep++;
							}
						}
						else 
						{
							GUILayout.Label("Prefab Detected , you should select the source Model");
						}
					}
				}
			}
			else if (SecStep == 1)
			{
				GUILayout.Label("Weapon Type", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
					if (DrawButton("Rifle"))
					{
						weaponType = 0;
						FirstStep = 1;
						SecStep = 0;
					}
					else if (DrawButton("Sniper"))
					{
						weaponType = 1;
						FirstStep = 1;
						SecStep = 0;
					}
					else if (DrawButton("Pistol"))
					{
						weaponType = 2;
						FirstStep = 1;
						SecStep = 0;
					}
					else if (DrawButton("Heavy"))
					{
						weaponType = 3;
						FirstStep = 1;
						SecStep = 0;
					}
					else if (DrawButton("Smg"))
					{
						weaponType = 4;
						FirstStep = 1;
						SecStep = 0;
					}
					else if (DrawButton("Launcher"))
					{
						weaponType = 5;
						FirstStep = 1;
						SecStep = 0;
					}
					else if (DrawButton("Melee"))
					{
						weaponType = 6;
						FirstStep = 1;
						SecStep = 0;
					}
			}
		}
		
		void SetupWeaponComponents()
		{
			GUILayout.Label("Weapon Details", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			weaponIcon = EditorGUILayout.ObjectField("Weapon Icon", weaponIcon, typeof(Sprite), false) as Sprite;
			GUILayout.Label("Weapon Name", new GUIStyle(), new GUILayoutOption[0]);
			weaponName = EditorGUILayout.TextField(weaponName, new GUILayoutOption[0]);
			GUILayout.Label("Weapon Description", new GUIStyle(), new GUILayoutOption[0]);
			weaponDesc = EditorGUILayout.TextField(weaponDesc, new GUILayoutOption[0]);
			GUILayout.Label("Weapon Unique Global ID", new GUIStyle(), new GUILayoutOption[0]);
			weaponGlobalID = EditorGUILayout.IntField(weaponGlobalID , new GUILayoutOption[0]);
			GUILayout.Label("Weapon Price", new GUIStyle(), new GUILayoutOption[0]);
			weaponPrice = EditorGUILayout.IntField(weaponPrice , new GUILayoutOption[0]);
			GUILayout.Label("Weapon Weight", new GUIStyle(), new GUILayoutOption[0]);
			weaponWeight = EditorGUILayout.FloatField(weaponWeight , new GUILayoutOption[0]);
			GUILayout.Label("Weapon Damage", new GUIStyle(), new GUILayoutOption[0]);
			weaponDamage = EditorGUILayout.FloatField(weaponDamage , new GUILayoutOption[0]);
			GUILayout.Label("Weapon Range", new GUIStyle(), new GUILayoutOption[0]);
			weaponRange = EditorGUILayout.FloatField(weaponRange , new GUILayoutOption[0]);
			GUILayout.Label("Weapon Ammo Per Mag", new GUIStyle(), new GUILayoutOption[0]);
			weaponRange = EditorGUILayout.FloatField(weaponRange , new GUILayoutOption[0]);
			GUILayout.Label("Weapon Accuracy", new GUIStyle(), new GUILayoutOption[0]);
			weaponAccuracy = EditorGUILayout.FloatField(weaponAccuracy , new GUILayoutOption[0]);
		//X	GUILayout.Label("Weapon Recoil", new GUIStyle(), new GUILayoutOption[0]);
			weaponRecoil = EditorGUILayout.Vector2Field("Weapon Recoil", weaponRecoil , new GUILayoutOption[0]);
			fireSound =	EditorGUILayout.ObjectField("Fire Sound", fireSound, typeof(AudioClip), false) as AudioClip;
			dryFireSound = EditorGUILayout.ObjectField("Dry Fire Sound", dryFireSound, typeof(AudioClip), false) as AudioClip;
			reloadSound = EditorGUILayout.ObjectField("Reload Sound", reloadSound, typeof(AudioClip), false) as AudioClip;
			ItemData weaponData = ScriptableObject.CreateInstance<ItemData>();
			
			if (SecStep == 0)
			{
				if (DrawButton("Make Item Data"))
				{
					SecStep = 1;
				}
			}
			
			if (SecStep == 1)
			{
				DrawText("Would you to Start Weapon Creation Process");
				weaponData.Icon = weaponIcon;
				weaponData.Name = weaponName;
				weaponData.Description = weaponDesc;
				weaponData.GlobalID = weaponGlobalID;
				weaponData.price = weaponPrice;
				weaponData.Weight = weaponWeight;
				
				if (DrawButton("SETUP WEAPON"))
				{
					AssetDatabase.CreateAsset(weaponData, "Assets/BLOODLINES/Resources/Items/Weapons/" + weaponName + ".asset");
					
					Weapon wp = WeaponInstantiated.AddComponent<Weapon>();
					
					if (wp != null)
					{
						wp.ItemInfo = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Weapons/" + weaponData.Name + ".asset") as ItemData;
						wp.CrosshairType = weaponType;
						wp.AnimationSetIndex = (float)weaponType;
						wp.Damage = weaponDamage;
						wp.Range = weaponRange;
						wp.AmmoPerMag = weaponAmmoPerMag;
						wp.Accuracy = weaponAccuracy;
						wp.Recoil = weaponRecoil;
						wp.weaponType = (WeaponType)weaponType;
						wp.fireSound = fireSound;
						wp.dryFireSound = dryFireSound;
						wp.reloadSound = reloadSound;
						
						GameObject Muzzle = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/BLOODLINES/Resources/Prefabs/Effects/MuzzleFlash.prefab") as GameObject) as GameObject;
						Muzzle.transform.parent = wp.transform;
						wp.Muzzle = Muzzle.GetComponent<ParticleSystem>();
						
						GameObject RightHandIK = new GameObject();
						GameObject LeftHandIK = new GameObject();
						
						RightHandIK.name = "RightHandIK";
						RightHandIK.transform.parent = WeaponInstantiated.transform;
						LeftHandIK.name = "LeftHandIK";
						LeftHandIK.transform.parent = WeaponInstantiated.transform;
						
						wp.rightHandObj = RightHandIK.transform;
						wp.leftHandObj = LeftHandIK.transform;
						var view = (SceneView)SceneView.sceneViews[0];
						view.ShowNotification(new GUIContent("Weapon Setup Done"));
						Debug.Log("Now you should adjust RightHandIK & LeftHandIK & Muzzle Flash Manually.");
						Debug.Log("Weapon Setup Done.");
					}
					
					WeaponInstantiated.name = weaponData.Name;
					GameObject WeaponPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(WeaponInstantiated, "Assets/BLOODLINES/Resources/Prefabs/Weapons/" + weaponName + ".prefab" , InteractionMode.AutomatedAction);
					ItemData currentWeaponData = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Weapons/" + weaponData.Name + ".asset") as ItemData;
					currentWeaponData.Prefab = WeaponPrefab;
				}

				if (DrawButton("SETUP PICKUP"))
				{
					WeaponPickupInstantiated = PrefabUtility.InstantiatePrefab(WeaponModel) as GameObject;
					if (PrefabUtility.GetPrefabInstanceStatus(WeaponPickupInstantiated) == PrefabInstanceStatus.Connected)
					{
						PrefabUtility.UnpackPrefabInstance(WeaponPickupInstantiated, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
					}
					var view = (SceneView)SceneView.sceneViews[0];
					view.camera.transform.position = WeaponPickupInstantiated.transform.position + ((WeaponPickupInstantiated.transform.forward * 10) + (new Vector3(0, 0.5f, 0)));
					view.LookAt(WeaponPickupInstantiated.transform.position);
					EditorGUIUtility.PingObject(WeaponPickupInstantiated);
					Selection.activeTransform = WeaponPickupInstantiated.transform;
					
					PickupInfo pi = WeaponPickupInstantiated.AddComponent<PickupInfo>();
					SphereCollider sc = WeaponPickupInstantiated.AddComponent<SphereCollider>();
					
					if (pi != null)
					{
						pi.ItemInfo = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Weapons/" + weaponData.Name + ".asset") as ItemData;
						pi.itemType = ItemType.WEAPON;
						pi.ToolTipWidget = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/BLOODLINES/Resources/Prefabs/UI/PickupWidget.prefab"), WeaponPickupInstantiated.transform) as GameObject;
					}
					
					sc.isTrigger = true;
					
					WeaponPickupInstantiated.tag = "Pickup";
					WeaponPickupInstantiated.name = weaponData.Name + "_Pickup";
					GameObject PickupWeapon = PrefabUtility.SaveAsPrefabAssetAndConnect(WeaponPickupInstantiated, "Assets/BLOODLINES/Resources/Prefabs/Weapons/" + WeaponPickupInstantiated.name + ".prefab" , InteractionMode.AutomatedAction);
					ItemData currentWeaponData = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Weapons/" + weaponData.Name + ".asset") as ItemData;
					currentWeaponData.DropPrefab = PickupWeapon;
					SecStep = 2;
				}
			}
			else if (SecStep == 2)
			{
				if (DrawButton("SETUP INVENTORY ITEM"))
				{
					InventoryItem InvItem = new InventoryItem();
					InvItem.ItemInfo = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Weapons/" + weaponData.Name + ".asset");
					InvItem.Dropable = true;
	
					GameData.Instance.InventoryItems.Add(InvItem);
					GameData.Instance.Weapons.Add(InvItem.ItemInfo);
					EditorUtility.SetDirty(GameData.Instance);

					SecStep = 3;
				}
			}
			else if (SecStep == 3)
			{
				DrawText("Weapon Main & Pickup Prefabs created in Resources Folder.");
				DrawText("Now should Adjust the Weapon Hands IK Targets & Weights in the Weapon Prefab if you are using IK.");
				DrawText("Weapon Creation Process Completed.");
				if (DrawButton("Menu"))
				{
					SecStep = 0;
					FirstStep = 0;
					WindowID = 0;
				}
			}
		}
		
		void MakeConsumable()
		{
			if (SecStep == 0)
			{
				DrawText("Consumable Creation Tool helping you to create Consumable / Backpacks Inventory Items:");
				DrawText("If your model ready this tool will create Add The Inventory / Pickup Components.");
				GUILayout.FlexibleSpace();
				DrawText("Drag your model from the Project View");
				ConsumableModel = EditorGUILayout.ObjectField("Consumable Model", ConsumableModel, typeof(GameObject), false) as GameObject;
				
				if (ConsumableModel != null)
				{
					if (DrawButton("NEXT"))
					{
						AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ConsumableModel));
						if (assetImporter != null)
						{
							ModelInfo = assetImporter as ModelImporter;
							if (ModelInfo != null)
							{
								ConsumablePickupInstantiated = PrefabUtility.InstantiatePrefab(ConsumableModel) as GameObject;
								if (PrefabUtility.GetPrefabInstanceStatus(ConsumablePickupInstantiated) == PrefabInstanceStatus.Connected)
								{
									PrefabUtility.UnpackPrefabInstance(ConsumablePickupInstantiated, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
								}
								var view = (SceneView)SceneView.sceneViews[0];
								view.camera.transform.position = ConsumablePickupInstantiated.transform.position + ((ConsumablePickupInstantiated.transform.forward * 10) + (new Vector3(0, 0.5f, 0)));
								view.LookAt(ConsumablePickupInstantiated.transform.position);
								EditorGUIUtility.PingObject(ConsumablePickupInstantiated);
								Selection.activeTransform = ConsumablePickupInstantiated.transform;
								GUILayout.FlexibleSpace();
								SecStep++;
							}
						}
						else 
						{
							GUILayout.Label("Prefab Detected , you should select the source Model");
						}
					}
				}
			}
			else if (SecStep == 1)
			{
				GUILayout.Label("Consumable Type", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (DrawButton("Backpack"))
				{
					consumableType = 0;
					FirstStep = 1;
					SecStep = 0;
				}
				else if (DrawButton("Ammo"))
				{
					consumableType = 1;
					FirstStep = 0;
					SecStep = 2;
				}
				else if (DrawButton("Consumable"))
				{
					consumableType = 2;
					FirstStep = 0;
					SecStep = 3;
				}
			}
			else if (SecStep == 2)
			{
				GUILayout.Label("Ammo Type", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (DrawButton("Light Ammo"))
				{
					consumableAmmoType = 0;
					FirstStep = 1;
					SecStep = 0;
				}
				else if (DrawButton("Sniper Ammo"))
				{
					consumableAmmoType = 1;
					FirstStep = 1;
					SecStep = 0;
				}
				else if (DrawButton("Heavy Ammo"))
				{
					consumableAmmoType = 2;
					FirstStep = 1;
					SecStep = 2;
				}
				else if (DrawButton("Shotgun Ammo"))
				{
					consumableAmmoType = 3;
					FirstStep = 1;
					SecStep = 0;
				}
				else if (DrawButton("Rockets"))
				{
					consumableAmmoType = 4;
					FirstStep = 1;
					SecStep = 0;
				}
			}
			else if (SecStep == 3)
			{
				GUILayout.Label("Affected Life Behaviour", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (DrawButton("Player"))
				{
					consumableType = 2;
					FirstStep = 1;
					SecStep = 0;
				}
				else if (DrawButton("Vehicle"))
				{
					consumableType = 3;
					FirstStep = 1;
					SecStep = 0;
				}
			}
		}
		
		void SetupConsumableComponents()
		{
			GUILayout.Label("Consumable Details", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			consumableIcon = EditorGUILayout.ObjectField("Consumable Icon", consumableIcon, typeof(Sprite), false) as Sprite;
			GUILayout.Label("Consumable Name", new GUIStyle(), new GUILayoutOption[0]);
			consumableName = EditorGUILayout.TextField(consumableName, new GUILayoutOption[0]);
			GUILayout.Label("Consumable Description", new GUIStyle(), new GUILayoutOption[0]);
			consumableDesc = EditorGUILayout.TextField(consumableDesc, new GUILayoutOption[0]);
			GUILayout.Label("Consumable Unique Global ID", new GUIStyle(), new GUILayoutOption[0]);
			consumableGlobalID = EditorGUILayout.IntField(consumableGlobalID , new GUILayoutOption[0]);
			GUILayout.Label("Consumable Price", new GUIStyle(), new GUILayoutOption[0]);
			consumablePrice = EditorGUILayout.IntField(consumablePrice , new GUILayoutOption[0]);
			if (consumableType == 0)
			{
				GUILayout.Label("Backpack Capacity", new GUIStyle(), new GUILayoutOption[0]);
				consumableWeight = EditorGUILayout.FloatField(consumableWeight , new GUILayoutOption[0]);
			}
			else if (consumableType == 1)
			{
				GUILayout.Label("Ammo Weight", new GUIStyle(), new GUILayoutOption[0]);
				consumableWeight = EditorGUILayout.FloatField(consumableWeight , new GUILayoutOption[0]);
				GUILayout.Label("Ammo Amount", new GUIStyle(), new GUILayoutOption[0]);
				consumableAmount = EditorGUILayout.IntField(consumableAmount , new GUILayoutOption[0]);
			}
			else if (consumableType == 2)
			{
				GUILayout.Label("Consumable Weight", new GUIStyle(), new GUILayoutOption[0]);
				consumableWeight = EditorGUILayout.FloatField(consumableWeight , new GUILayoutOption[0]);
				consumableIsBandage = EditorGUILayout.Toggle("Bandage To Stop Bleeding", consumableIsBandage , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Add Player Affected Bone Health", new GUIStyle(), new GUILayoutOption[0]);
				consumableHealth = EditorGUILayout.FloatField(consumableHealth , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Add Player Stamina", new GUIStyle(), new GUILayoutOption[0]);
				consumableStamina = EditorGUILayout.FloatField(consumableStamina , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Add Player Thirst", new GUIStyle(), new GUILayoutOption[0]);
				consumableThirst = EditorGUILayout.FloatField(consumableThirst , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Add Player Hunger", new GUIStyle(), new GUILayoutOption[0]);
				consumableHunger = EditorGUILayout.FloatField(consumableHunger , new GUILayoutOption[0]);
				
				consumableIsSpoiled = EditorGUILayout.Toggle("Can Be Poisoned / Spoiled", consumableIsSpoiled , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Poison Chance (1 - 100)", new GUIStyle(), new GUILayoutOption[0]);
				consumablePoisonChance = EditorGUILayout.IntField(consumablePoisonChance , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Disease / Poison ID (from GameData)", new GUIStyle(), new GUILayoutOption[0]);
				consumableDiseaseID = EditorGUILayout.IntField(consumableDiseaseID , new GUILayoutOption[0]);
				
				consumableIsMedicine = EditorGUILayout.Toggle("Can Be Medicine", consumableIsMedicine , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Medicine ID (from GameData)", new GUIStyle(), new GUILayoutOption[0]);
				consumableMedicineID = EditorGUILayout.IntField(consumableMedicineID , new GUILayoutOption[0]);
			}
			else if (consumableType == 3)
			{
				GUILayout.Label("Consumable Add Vehicle Health", new GUIStyle(), new GUILayoutOption[0]);
				consumableHealth = EditorGUILayout.FloatField(consumableHealth , new GUILayoutOption[0]);
				GUILayout.Label("Consumable Add Vehicle Fuel", new GUIStyle(), new GUILayoutOption[0]);
				consumableStamina = EditorGUILayout.FloatField(consumableStamina , new GUILayoutOption[0]);
			}
			
			ItemData ConsumableData = ScriptableObject.CreateInstance<ItemData>();
			
			if (SecStep == 0)
			{
				if (DrawButton("Make Item Data"))
				{
					SecStep = 1;
				}
			}
			
			if (SecStep == 1)
			{
				DrawText("Would you to Start Consumable Creation Process");
				ConsumableData.Icon = consumableIcon;
				ConsumableData.Name = consumableName;
				ConsumableData.Description = consumableDesc;
				ConsumableData.GlobalID = consumableGlobalID;
				ConsumableData.price = consumablePrice;
				ConsumableData.Weight = consumableWeight;
				ConsumableData.Amount = consumableAmount;
				
				if (DrawButton("SETUP CONSUMABLE"))
				{
					AssetDatabase.CreateAsset(ConsumableData, "Assets/BLOODLINES/Resources/Items/Consumables/" + consumableName + ".asset");
					
					PickupInfo pi = ConsumablePickupInstantiated.AddComponent<PickupInfo>();
					SphereCollider sc = ConsumablePickupInstantiated.AddComponent<SphereCollider>();
					
					if (pi != null)
					{
						pi.ItemInfo = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Consumables/" + ConsumableData.Name + ".asset") as ItemData;
						if (consumableType == 0)
						{
							pi.ItemInfo.Type = ItemType.WEARABLE;
							pi.ItemInfo.InventoryType = InventoryItemType.Wearable;
							pi.itemType = ItemType.WEARABLE;
						}
						else if (consumableType == 1)
						{
							pi.ItemInfo.Type = ItemType.AMMO;
							pi.ItemInfo.ammoType = (AmmoType)consumableAmmoType;
							pi.ItemInfo.InventoryType = InventoryItemType.Consumable;
							pi.itemType = ItemType.AMMO;
						}
						else if (consumableType == 2 || consumableType == 3)
						{
							pi.ItemInfo.Type = ItemType.CONSUMABLE;
							pi.ItemInfo.InventoryType = InventoryItemType.Consumable;
							pi.itemType = ItemType.CONSUMABLE;
						}
						
						pi.ToolTipWidget = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/BLOODLINES/Resources/Prefabs/UI/PickupWidget.prefab"), ConsumablePickupInstantiated.transform) as GameObject;
					}
					
					if (sc != null)
					{
						sc.isTrigger = true;
					}
					
					ConsumablePickupInstantiated.tag = "Pickup";
					ConsumablePickupInstantiated.name = ConsumableData.Name + "_Pickup";
					GameObject PickupConsumable = PrefabUtility.SaveAsPrefabAssetAndConnect(ConsumablePickupInstantiated, "Assets/BLOODLINES/Resources/Prefabs/Consumables/" + ConsumablePickupInstantiated.name + ".prefab" , InteractionMode.AutomatedAction);
					ItemData currentConsumableData = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Consumables/" + ConsumableData.Name + ".asset") as ItemData;
					currentConsumableData.DropPrefab = PickupConsumable;
					SecStep = 2;
				}
			}
			else if (SecStep == 2)
			{
				if (DrawButton("SETUP INVENTORY ITEM"))
				{
					InventoryItem InvItem = new InventoryItem();
					InvItem.Dropable = true;
					
					switch (consumableType)
					{
						case 0:
							InvItem.DestroyOnUse = true;
							break;
						case 1:
							InvItem.AllowMultiple = true;
							break;
						case 2:
							InvItem.AllowMultiple = true;
							InvItem.DestroyOneOnUse = true;
							InvItem.ItemInfo.ConsumableEffect.defaultLifeBehaviour = DefaultLifeBehaviour.Player;
							InvItem.ItemInfo.ConsumableEffect.Stamina = consumableStamina;
							InvItem.ItemInfo.ConsumableEffect.Thirst = consumableThirst;
							InvItem.ItemInfo.ConsumableEffect.Hunger = consumableHunger;
							InvItem.ItemInfo.ConsumableEffect.AffectedBoneHealth = consumableHealth;
							InvItem.ItemInfo.ConsumableEffect.BandageToStopBleeding = consumableIsBandage;
							InvItem.ItemInfo.isSpoiled = consumableIsSpoiled;
							InvItem.ItemInfo.PoisonChance = consumablePoisonChance;
							InvItem.ItemInfo.DiseaseID = consumableDiseaseID;
							InvItem.ItemInfo.isMedicine = consumableIsMedicine;
							InvItem.ItemInfo.MedicineID = consumableMedicineID;
							break;
						case 3:
							InvItem.AllowMultiple = true;
							InvItem.DestroyOneOnUse = true;
							InvItem.ItemInfo.ConsumableEffect.defaultLifeBehaviour = DefaultLifeBehaviour.Vehicle;
							InvItem.ItemInfo.ConsumableEffect.VehicleHealth = consumableHealth;
							InvItem.ItemInfo.ConsumableEffect.Fuel = consumableStamina;
							break;
					}
					
					
					InvItem.ItemInfo = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/BLOODLINES/Resources/Items/Consumables/" + ConsumableData.Name + ".asset") as ItemData;

					GameData.Instance.InventoryItems.Add(InvItem);
					EditorUtility.SetDirty(GameData.Instance);
					
					SecStep = 3;
				}
			}
			else if (SecStep == 3)
			{
				DrawText("Consumable Pickup Prefab created in Resources Folder.");
				DrawText("Consumable Creation Process Completed.");
				if (DrawButton("Menu"))
				{
					SecStep = 0;
					FirstStep = 0;
					WindowID = 0;
				}
			}
		}
		
		void MakeVehicle()
		{
			if (SecStep == 0)
			{
				DrawText("Vehicle Creation Tool helping you to create Vehicles:");
				DrawText("If your model ready this tool will create Add The Vehicle Components.");
				GUILayout.FlexibleSpace();
				DrawText("Drag your model from the Project View");
				VehicleModel = EditorGUILayout.ObjectField("Vehicle Model", VehicleModel, typeof(GameObject), false) as GameObject;
				
				if (VehicleModel != null)
				{
					if (DrawButton("NEXT"))
					{
						AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(VehicleModel));
						if (assetImporter != null)
						{
							ModelInfo = assetImporter as ModelImporter;
							if (ModelInfo != null)
							{
								VehicleInstantiated = PrefabUtility.InstantiatePrefab(VehicleModel) as GameObject;
								if (PrefabUtility.GetPrefabInstanceStatus(VehicleInstantiated) == PrefabInstanceStatus.Connected)
								{
									PrefabUtility.UnpackPrefabInstance(VehicleInstantiated, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
								}
								var view = (SceneView)SceneView.sceneViews[0];
								view.camera.transform.position = VehicleInstantiated.transform.position + ((VehicleInstantiated.transform.forward * 10) + (new Vector3(0, 0.5f, 0)));
								view.LookAt(VehicleInstantiated.transform.position);
								EditorGUIUtility.PingObject(VehicleInstantiated);
								Selection.activeTransform = VehicleInstantiated.transform;
								GUILayout.FlexibleSpace();
								SecStep++;
							}
						}
						else 
						{
							GUILayout.Label("Prefab Detected , you should select the source Model");
						}
					}
				}
			}
			else if (SecStep == 1)
			{
				GUILayout.Label("Vehicle Type", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (DrawButton("Car"))
				{
					vehicleType = 0;
					FirstStep = 1;
					SecStep = 0;
				}
				else if (DrawButton("Airplane"))
				{
					vehicleType = 1;
					FirstStep = 1;
					SecStep = 0;
				}
				else if (DrawButton("Helicopter"))
				{
					vehicleType = 2;
					FirstStep = 1;
					SecStep = 0;
				}
			}
		}
		
		void SetupVehicleComponents()
		{
			GUILayout.Label("Vehicle Creation Tool", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.Label("Vehicle Name", new GUIStyle(), new GUILayoutOption[0]);
			vehicleName = EditorGUILayout.TextField(vehicleName, new GUILayoutOption[0]);
			GUILayout.Label("Vehicle Doors", new GUIStyle(), new GUILayoutOption[0]);
			VehicleDoorMain = EditorGUILayout.ObjectField("Vehicle Door Main", VehicleDoorMain, typeof(GameObject), true) as GameObject;
			VehicleDoor2 = EditorGUILayout.ObjectField("Vehicle Door 2", VehicleDoor2, typeof(GameObject), true) as GameObject;
			VehicleDoor3 = EditorGUILayout.ObjectField("Vehicle Door 3", VehicleDoor3, typeof(GameObject), true) as GameObject;
			VehicleDoor4 = EditorGUILayout.ObjectField("Vehicle Door 4", VehicleDoor4, typeof(GameObject), true) as GameObject;
			GUILayout.Label("Vehicle Wheels", new GUIStyle(), new GUILayoutOption[0]);
			VehicleWheel1 = EditorGUILayout.ObjectField("Vehicle Wheel 1", VehicleWheel1, typeof(GameObject), true) as GameObject;
			VehicleWheel2 = EditorGUILayout.ObjectField("Vehicle Wheel 2", VehicleWheel2, typeof(GameObject), true) as GameObject;
			VehicleWheel3 = EditorGUILayout.ObjectField("Vehicle Wheel 3", VehicleWheel3, typeof(GameObject), true) as GameObject;
			VehicleWheel4 = EditorGUILayout.ObjectField("Vehicle Wheel 4", VehicleWheel4, typeof(GameObject), true) as GameObject;
			
			if (SecStep == 0)
			{
				DrawText("Would you to Start Vehicle Creation Process");
				if (DrawButton("SETUP VEHICLE"))
				{
					VehiclesManager vm = VehicleInstantiated.AddComponent<VehiclesManager>();
					vm.vehicleType = (VehicleType)vehicleType;
					
					if (vm != null)
					{
						GameObject RightHandIK = new GameObject();
						GameObject LeftHandIK = new GameObject();
							
						RightHandIK.name = "RightHandIK";
						RightHandIK.transform.parent = VehicleInstantiated.transform;
						LeftHandIK.name = "LeftHandIK";
						LeftHandIK.transform.parent = VehicleInstantiated.transform;
							
						vm.RightHandTarget = RightHandIK.transform;
						vm.LeftHandTarget = LeftHandIK.transform;
						var view = (SceneView)SceneView.sceneViews[0];
						view.ShowNotification(new GUIContent("Vehicle Setup Done"));
						Debug.Log("Vehicle Setup Done.");
					}
						
					if (vehicleType == 0)
					{
						Rigidbody rb = VehicleInstantiated.GetComponent<Rigidbody>();
						CarController cc = VehicleInstantiated.AddComponent<CarController>();
						if (rb != null)
						{
							rb.mass = 1000f;
							rb.drag = 0.1f;
							rb.angularDrag = 0.05f;
						}
					}
					
					if (vehicleType == 1)
					{
						Rigidbody rb = VehicleInstantiated.GetComponent<Rigidbody>();
						AeroplaneController ac = VehicleInstantiated.AddComponent<AeroplaneController>();
						
						if (rb != null)
						{
							rb.mass = 1;
							rb.drag = 0.02f;
							rb.angularDrag = 0.09f;
						}
					}
					
					if (vehicleType == 2)
					{
						Rigidbody rb = VehicleInstantiated.GetComponent<Rigidbody>();
						HelicopterController hc = VehicleInstantiated.AddComponent<HelicopterController>();
						
						if (rb != null)
						{
							rb.mass = 1;
							rb.drag = 0.02f;
							rb.angularDrag = 0.09f;
						}
						
						if (hc != null)
						{
							GameObject GroundDetector = new GameObject();
							
							GroundDetector.name = "GroundDetector";
							GroundDetector.transform.parent = VehicleInstantiated.transform;
							hc.GroundDetector = GroundDetector.transform;
							
							Debug.Log("Now you should adjust the GroundDetector position to the ground.");
							Debug.Log("if your helicopter have a Rotating Blades you should add HeliRotator Component and select axis to the blades model.");
						}
					}
					
					if (VehicleDoorMain != null)
					{
						VehicleDoorSystem vds  = VehicleDoorMain.AddComponent<VehicleDoorSystem>();
						
						if (vds != null)
						{
							vds.VM = vm;
							vds.isDriver = true;
							
							GameObject SitPos = new GameObject();
							SitPos.name = "SitPos";
							SitPos.transform.parent = VehicleDoorMain.transform;
							vds.SitPosition = SitPos.transform;
							
						}
					}
					
					if (VehicleDoor2 != null)
					{
						VehicleDoorSystem vds  = VehicleDoor2.AddComponent<VehicleDoorSystem>();
						
						if (vds != null)
						{
							vds.VM = vm;
							GameObject SitPos = new GameObject();
							SitPos.name = "SitPos";
							SitPos.transform.parent = VehicleDoor2.transform;
							vds.SitPosition = SitPos.transform;
						}
					}
					
					if (VehicleDoor3 != null)
					{
						VehicleDoorSystem vds  = VehicleDoor3.AddComponent<VehicleDoorSystem>();
						
						if (vds != null)
						{
							vds.VM = vm;
							GameObject SitPos = new GameObject();
							SitPos.name = "SitPos";
							SitPos.transform.parent = VehicleDoor3.transform;
							vds.SitPosition = SitPos.transform;
						}
					}
					
					if (VehicleDoor4 != null)
					{
						VehicleDoorSystem vds  = VehicleDoor4.AddComponent<VehicleDoorSystem>();
						
						if (vds != null)
						{
							vds.VM = vm;
							GameObject SitPos = new GameObject();
							SitPos.name = "SitPos";
							SitPos.transform.parent = VehicleDoor4.transform;
							vds.SitPosition = SitPos.transform;
						}
					}
					
					if (VehicleWheel1 != null)
					{
						WheelCollider wc = VehicleWheel1.AddComponent<WheelCollider>();
					}
					
					if (VehicleWheel2 != null)
					{
						WheelCollider wc = VehicleWheel2.AddComponent<WheelCollider>();
					}
					
					if (VehicleWheel3 != null)
					{
						WheelCollider wc = VehicleWheel3.AddComponent<WheelCollider>();
					}
					
					if (VehicleWheel4 != null)
					{
						WheelCollider wc = VehicleWheel4.AddComponent<WheelCollider>();
					}
					
					VehicleInstantiated.name = vehicleName;
					GameObject VehiclePrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(VehicleInstantiated, "Assets/BLOODLINES/Resources/Prefabs/Vehicles/" + vehicleName + ".prefab" , InteractionMode.AutomatedAction);
					SecStep = 1;
				}
			}
			else if (SecStep == 1)
			{
				DrawText("Vehicle Prefab created in Resources Folder.");
				DrawText("Now should Adjust the Vehicle Hands IK Targets & Weights in the Vehicle Prefab if you are using IK & Adjust the Doors or Wheels Colliders & add your Other Colliders if you need.");
				DrawText("Vehicle Creation Process Completed.");
				if (DrawButton("Menu"))
				{
					SecStep = 0;
					FirstStep = 0;
					WindowID = 0;
				}
			}
		}
	}
}
