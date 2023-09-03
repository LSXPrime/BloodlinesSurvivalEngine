using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class ProjectTest : ScriptableObject
    {
        
        #region AI
        public class AIBase
        {
            public bool SetInitPos;
            public bool DistanceToInitPos;
            public bool DistanceToTarget;
            public bool DistanceToPlayer;
            public bool DistanceTargetToInitPos;
            public bool GetRandomArea;
            public bool SearchTarget;
            public bool CheckAttack;
            public bool MeleeAttack;
            public bool RangedAttack;
            public bool InfectedAttack;
        }

        public class AIAnimal
        {
            public bool Start;
            public bool Update;
            public bool NeedsCheck;
            public bool SearchTarget;
            public bool SearchSatisfy;
            public bool MeleeAttack;

        }

        public class AIBoss
        {
            public bool Update;
            public bool MeleeAttack;
            public bool RangedAttack;
        }

        public class AIHuman
        {
            public bool Start;
            public bool Update;
            public bool OnAnimatorIK;
            public bool WeaponHandle;
            public bool MeleeAttack;
            public bool RangedAttack;
            public bool AddType;
            public bool OnDeathDrop;
            public bool GetWeaponIndex;
            public bool SearchTarget;
        }

        public class AIZombie
        {
            public bool Update;
            public bool MeleeAttack;
            public bool RangedAttack;
        }

        public class DropAfterDeath
        {
            public bool Start;
            public bool Update;
            public bool DropItem;
        }
        #endregion

        public class CameraHelper
        {
            public bool Start;
            public bool Update;
            public bool SetSpecialEffects;
            public bool SetEffects;
            public bool Recoil;
            public bool CheckCollider;
        }

        public class AccountInfo
        {
            public bool Start;
            public bool OnEnable;
            public bool OnDisable;
            public bool LoadData;
            public bool SaveData;
        }

        #region Data
        public class NeedEXT
        {
            public bool Clone;
        }

        public class CurrentDisease
        {
            public bool Set;
            public bool Initilize;
            public bool Spread;
            public bool GetActiveStage;
            public bool SaveData;
        }

        public class CurrentMedicine
        {
            public bool Initilize1;
            public bool Initilize2;
            public bool Initilize3;
        }

        public class DiseaseData
        {
            public bool Clone;
        }

        public class DiseaseStage
        {
            public bool GetStageDesc;
            public bool Multiplie;
            public bool Clone;
        }

        public class GameData
        {
            public bool GetDisease;
            public bool GetMedicine;
            public bool GetStructure;
            public bool GetNeed;
            public bool GetQuest;
            public bool GetItem1;
            public bool GetItem2;
            public bool GetWearable;
            public bool GetWeaponData;
            public bool GetWeapon;
            public bool GetWeaponID;
            public bool GetSurfaceTag;
        }

        public class HumanHealthSystem
        {
            public bool Respawn;
            public bool GetVitals;
            public bool Clone;
        }

        public class WearableData
        {
            public bool SetWearable;
            public bool SetFree;
        }
        
        public class BloodlinesManager
        {
            public bool ShowWindow;
            public bool OnEnable;
            public bool OnGUI;
            public bool DrawButton;
            public bool DrawText;
            public bool ChangWindow;
            public bool StarterInfo;
            public bool MakeMortal;
            public bool SetupPlayer;
            public bool SetupPlayerComponents;
            public bool AICreation;
            public bool MakeAI;
            public bool SetupAIComponents;
            public bool MakeWeapon;
            public bool SetupWeaponComponents;
            public bool MakeConsumable;
            public bool SetupConsumableComponents;
            public bool MakeVehicle;
            public bool SetupVehicleComponents;
        }

        public class CreateHitSpotsWizard
        {
            public bool AddBreastColliders;
            public bool AddHeadCollider;
            public bool AddJoint;
            public bool AddMirroredJoint;
            public bool BuildCapsules;
            public bool CalculateDirection;
            public bool CalculateDirectionAxis;
            public bool CheckConsistency;
            public bool Cleanup;
            public bool Clip;
            public bool DecomposeVector;
            public bool FindBone;
            public bool GetBreastBounds;
            public bool LargestComponent;
            public bool Create;
            public bool DetectBones;
            public bool PrepareBones;
            public bool SecondLargestComponent;
            public bool SmallestComponent;
        }

        public class ObjectPool
        {
            public bool Awake;
            public bool Spawn;
            public bool Despawn1;
            public bool Despawn2;
            public bool TimedDespawn;
        }

        public class ObjectPoolItem
        {
            public bool Initialize;
            public bool Spawn;
            public bool Despawn;
            public bool PrespawnItems;
        }

        public class GlobalGameManager
        {
            public bool Start;
            public bool SetLocalPlayer;
            public bool Update;
            public bool PanelsHandler;
            public bool PauseHandler;
            public bool QuickSlotsHandler;
            public bool QuickSlotsClear;
            public bool SetSideText;
            public bool SideText;
            public bool ShowWeaponUI;
            public bool LifeBehaviourUI;
            public bool CollectData;
            public bool Respawn;
            public bool Leave;
            public bool CastEvent;
        }

        public class ObjectManager
        {
            public bool Start;
            public bool RebuildRPC;
            public bool Rebuild;
            public bool GetNearstPlayer;
            public bool GetNearstZombie;
            public bool GetNearstAnimal;
            public bool GetNearstVehicle;
            public bool GetNearstPickup;
            public bool GetNearstWeapon;
            public bool GetNearstEnemySpawner;
            public bool GetNearstItemSpawner;
            public bool GetNearstSatisfyArea;
            public bool GetNearstNPCShop;
            public bool GetNearstNPCQuest;
        }

        #endregion

        private static ProjectTest instance;
		public static ProjectTest Instance
		{
			get
			{
				if (instance == null)
					instance = Resources.Load("ProjectTest", typeof(ProjectTest)) as ProjectTest;

				return instance;
			}
		}
		
		public static IEnumerator LoadData()
		{
			if (instance == null)
			{
				ResourceRequest rr = Resources.LoadAsync("ProjectTest", typeof(ProjectTest));
				while (!rr.isDone) { yield return null; }
				instance = rr.asset as ProjectTest;
			}
		}
    }
}
