using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
	public class GlobalGameManager : MonoBehaviour
	{	
		internal Entity LocalPlayer;
		
		[Header("Settings")]
		public UIState State;
		public Transform[] RespawnLocations;
		public TimeOfDay TimeCycle;

		[Header("Camera")]
		public CameraHelper Camera;

		[Header("Life Behaviour UI")]	
		public Image BloodBar;
		public Image StaminaBar;
		public Image FoodBar;
		public Image WaterBar;
		public Image OxygenBar;
		public Image ExhustionBar;

        [Header("Weapon UI")]
        public CanvasGroup WeaponUI;
        public float WeaponUIFadeSpeed = 5f;
        public Image WeaponIcon;
		public Text WeaponName;
		public Text WeaponAmmo;
		public GameObject[] Crosshairs;
		public CanvasGroup HitMarker;
		public float HitMarkerFadeSpeed = 5f;
		public CanvasGroup DamageIndicator;
		public float DamageIndicatorFadeSpeed = 2f;

        [Header("Elements UI")]
        public List<InventoryQuickSlot> QuickSlots = new List<InventoryQuickSlot>();
        public Gradient QuickSlotsAmmoColor;
        public GameObject SideTextGO;
		public GameObject GameUI;
		public GameObject PausePanel;
		public BankUI BankPanel;
		public NPCShopUI NPCShopPanel;
		public NPCQuestUI NPCQuestPanel;
		public QuestUI QuestPanel;
		public StatusUI StatusPanel;
		public InventoryUI InventoryPanel;
		public CraftUI CraftPanel;
		public BuildBlueprintUI BuildBlueprint;
		public GameObject MobileControls;
		public Image InventoryCapacity;
		public MinimapUI MinimapUI;
		
		[Header("Results UI")]
		public GameObject ResultsUI;
		public Text FinalDeathReasonText;
		public Text FinalKillsText;
		public Text FinalScoreText;
		public Text FinalCoinsText;
		
		float SaveTimeTmp;
        bool showWeaponUI;

        void Start()
		{
			StartCoroutine(GameData.LoadData());
			Application.targetFrameRate = GameData.Instance.TargetFPS;
			if (GameData.Instance.Platform == Platform.MOBILE) MobileControls.SetActive(true);
			
			SetLocalPlayer();
			Extensions.LoadGameData();
		}
		
		public void SetLocalPlayer()
		{
			LocalPlayer = Instantiate(GameData.Instance.Player.gameObject, RespawnLocations[Random.Range(0, RespawnLocations.Length)].position, Quaternion.identity).GetComponent<Entity>();
			Camera.entity = LocalPlayer;
			MinimapUI.player = LocalPlayer.transform;
			QuestPanel.Player = LocalPlayer.Get<PlayerQuestSystem>();
			InventoryPanel.Inventory = LocalPlayer.Get<InventoryManager>();
            BankPanel.player = LocalPlayer.Get<InventoryManager>();
            CraftPanel.craftManager = LocalPlayer.Get<CraftManager>();
			BuildBlueprint.player = LocalPlayer.Get<PlayerBuildingSystem>();
			StatusPanel.player = LocalPlayer.Get<PlayerStatus>();
        }
		
		void Update()
		{
			if (!LocalPlayer)
				return;
			
			LocalPlayer.Enabled = State == UIState.Play;
			MinimapUI.gameObject.SetActive(State == UIState.Play);
            LocalPlayer.Get<PlayerController>().CurrentSpeed = State != UIState.Play ? 0f : LocalPlayer.Get<PlayerController>().CurrentSpeed;
			
			QuickSlotsHandler();
			PanelsHandler();
			LifeBehaviourUI();
			SaveTimeTmp += Time.deltaTime;
			if (SaveTimeTmp >= GameData.Instance.SaveInterval)
			{
				Extensions.SaveGameData();
				SaveTimeTmp = 0f;
				SetSideText("GAME SAVED");
			}
		}
		
		public void PanelsHandler()
		{
			if (InputManager.GetButtonDown("Inventory"))
			{
				State = State == UIState.Inventory ? UIState.Play : UIState.Inventory;
				PauseHandler(State);
			}
			if (InputManager.GetButtonDown("Craft"))
			{
				State = State == UIState.Craft ? UIState.Play : UIState.Craft;
				PauseHandler(State);
			}
			if (InputManager.GetButtonDown("Quests"))
			{
				State = State == UIState.Quest ? UIState.Play : UIState.Quest;
				PauseHandler(State);
			}
			if (BankPanel.bank !=null && InputManager.GetButtonDown("Interact"))
			{
				State = State == UIState.Bank ? UIState.Play : UIState.Bank;
				PauseHandler(State);
			}
			if (NPCShopPanel.NPC !=null && InputManager.GetButtonDown("Interact"))
			{
				State = State == UIState.NPCShop ? UIState.Play : UIState.NPCShop;
				PauseHandler(State);
			}
			if (NPCQuestPanel.NPC !=null && InputManager.GetButtonDown("Interact"))
			{
				State = State == UIState.Quest ? UIState.Play : UIState.Quest;
				PauseHandler(State);
			}
			if (InputManager.GetButtonDown("BuildBlueprint"))
			{
				State = State == UIState.Build ? UIState.Play : UIState.Build;
				PauseHandler(State);
			}
			if (InputManager.GetButtonDown("Status"))
			{
				State = State == UIState.Status ? UIState.Play : UIState.Status;
				PauseHandler(State);
			}
			if (InputManager.GetKeyDown(KeyCode.Escape))
			{
				State = State == UIState.Pause ? UIState.Play : UIState.Pause;
                PauseHandler(State);
            }
		}
		
		public void PauseHandler(UIState state)
		{
			Time.timeScale = state == UIState.Pause ? 0f : 1f;
			Extensions.LockCursor(state == UIState.Play);

			InventoryPanel.gameObject.SetActive(state == UIState.Inventory);
			QuestPanel.gameObject.SetActive(state == UIState.Quest);
			CraftPanel.gameObject.SetActive(state == UIState.Craft);
			PausePanel.gameObject.SetActive(state == UIState.Pause);
			BankPanel.gameObject.SetActive(state == UIState.Bank);
			NPCShopPanel.gameObject.SetActive(state == UIState.NPCShop);
			NPCQuestPanel.gameObject.SetActive(state == UIState.NPCQuest);
			BuildBlueprint.gameObject.SetActive(state == UIState.Build);
			StatusPanel.gameObject.SetActive(state == UIState.Status);
			
			if (state == UIState.Leave)
                Leave();
		}

		public void QuickSlotsHandler()
		{
			if (LocalPlayer == null)
				return;
			
			HitMarker.alpha = Mathf.Lerp(HitMarker.alpha, 0f, HitMarkerFadeSpeed * Time.deltaTime);
			DamageIndicator.alpha = Mathf.Lerp(DamageIndicator.alpha, 0f, DamageIndicatorFadeSpeed * Time.deltaTime);
			WeaponUI.alpha = Mathf.Lerp(WeaponUI.alpha, showWeaponUI ? 1 : 0, WeaponUIFadeSpeed * Time.deltaTime);
			
			foreach (InventoryQuickSlot slot in QuickSlots)
			{
				if (slot.ID != 0 && (slot.Item == null || slot.Item.Amount <= 0))
					slot.Set(null);
				
				if ((slot.ID == 0 || slot.Item == null) && LocalPlayer.Get<InventoryManager>().Weapons.Count > QuickSlots.IndexOf(slot))
					slot.Set(LocalPlayer.Get<InventoryManager>().Weapons[QuickSlots.IndexOf(slot)]);
				
				if (InputManager.GetKeyDown(slot.InteractKey) && slot.Item != null)
                    LocalPlayer.Get<InventoryManager>().UseItem(slot.ID);
				
				if (slot.Item != null && slot.Item.Weapon != null)
				{
                    float ammo = slot.Item.Weapon.CurrentAmmo / slot.Item.Weapon.AmmoPerMag;
                    Debug.Log(ammo);
                    slot.Amount.color = QuickSlotsAmmoColor.Evaluate(ammo);
                    slot.Amount.fillAmount = ammo;
                }
            }
		}
		
		public void QuickSlotsClear()
		{
            foreach (InventoryQuickSlot slot in QuickSlots)
            {
				slot.Set(null);
            }
		}
		
		public void SetSideText(string side)
		{
			StartCoroutine(SideText(side));
		}
		
		IEnumerator SideText(string side)
		{
			SideTextGO.SetActive(true);
			Text SideText = SideTextGO.GetComponentInChildren<Text>();
			SideText.text = side;
			yield return new WaitForSeconds(3);
			SideTextGO.SetActive(false);
			SideText.text = string.Empty;
		}
		
		public void ShowWeaponUI(bool show)
		{
            showWeaponUI = show;
		}

		void LifeBehaviourUI()
		{
			if (LocalPlayer == null)
				return;
			
			BloodBar.fillAmount = LocalPlayer.Get<PlayerStatus>().HealthSystem.BloodPercentage / 100f;
			StaminaBar.fillAmount = LocalPlayer.Get<PlayerStatus>().HealthSystem.StaminaPercentage / 100f;
			FoodBar.fillAmount = LocalPlayer.Get<PlayerStatus>().HealthSystem.FoodPercentage / 100f;
			WaterBar.fillAmount = LocalPlayer.Get<PlayerStatus>().HealthSystem.WaterPercentage / 100f;
			OxygenBar.fillAmount = LocalPlayer.Get<PlayerStatus>().HealthSystem.OxygenPercentage / 100f;
			ExhustionBar.fillAmount = LocalPlayer.Get<PlayerStatus>().HealthSystem.ExhustionPercentage / 100f;
		}
		
		public void CollectData(DeathReason reason)
		{
			int kills = AccountInfo.Instance.Kills;
			int score = AccountInfo.Instance.Score;
			int coins = AccountInfo.Instance.Coins;
			
			switch (reason)
			{
				case DeathReason.Dehyderation:
					FinalDeathReasonText.text = "PLAYER DEHYDERATED";
					break;
				case DeathReason.Starve:
					FinalDeathReasonText.text = "PLAYER STARVED";
					break;
				case DeathReason.Bloodloss:
					FinalDeathReasonText.text = "PLAYER BLEEDED TO DEATH";
					break;
				case DeathReason.HighPressure:
					FinalDeathReasonText.text = "PLAYER PRESSURE REACHED THE SKY";
					break;
				case DeathReason.LowPressure:
					FinalDeathReasonText.text = "PLAYER PRESSURE REACHED THE OCEAN BOTTOM";
					break;
				case DeathReason.Breathloss:
					FinalDeathReasonText.text = "PLAYER DROWN";
					break;
				case DeathReason.HeartFailure:
					FinalDeathReasonText.text = "PLAYER DIED BY HEART ATTACK";
					break;
				case DeathReason.Overheat:
					FinalDeathReasonText.text = "PLAYER WAS TOO HOT";
					break;
				case DeathReason.Overcold:
					FinalDeathReasonText.text = "PLAYER WAS COLD MINDED";
					break;
				case DeathReason.CriticalDisease:
					FinalDeathReasonText.text = "PLAYER DIED BY DISEASE";
					break;
			}
			
			FinalKillsText.text = string.Format("{0}\n<size=100>{1}</size>", "KILLS:", kills);
			FinalScoreText.text = string.Format("{0}\n<size=100>{1}</size>", "SCORE:",score + "<size=100>XP</size>");
			FinalCoinsText.text = string.Format("{0}\n<size=100>{1}</size>", "COINS:", coins, "<size=100>C</size>");
			ResultsUI.SetActive(true);
		}
		
		public void Respawn()
		{
			if (LocalPlayer != null)
			{
                LocalPlayer.Get<HealthSystem>().Respawn();
				ResultsUI.SetActive(false);
			}
		}
		
		public void Leave()
		{
			Extensions.SaveGameData();
			Application.Quit();
		}
		
		public void CastEvent(EventType type, int ID, int Amount)
		{
			if (LocalPlayer == null)
				return;

            LocalPlayer.Get<PlayerQuestSystem>().CheckQuestType(type, ID, Amount);
		}
		
		private static GlobalGameManager instance;
		public static GlobalGameManager Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<GlobalGameManager>(); }
				return instance;
			}
		}
	}
}
