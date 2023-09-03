using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class TimeOfDay : MonoBehaviour
    {		
		[Header("References")]
		public Light Sun;
		public Light Moon;
		public Material Skybox;
		public AnimationCurve SkyboxBlend;
		
		[Header("Weather")]
		public float Temperature;
		public float WindSpeed;
		public float RainIntensity;
		
		[Header("General")]
		public bool isEnabled;
		[Range(0, 24)] public int CurrentHour = 6;
		[Tooltip("How many seconds are in a day.")]
		public int DayDuration = 900;
		[Tooltip("On which axis should the moon and sun rotate?")]
		public Vector3 RotationAxis = Vector2.right;
		
		[Header("Fog")]
		public Gradient FogColor;
		public AnimationCurve FogIntensity;
		public FogMode FogMode = FogMode.ExponentialSquared;
		
		[Header("Sun")]
		public Gradient SunColor;
		public AnimationCurve SunIntensity;
		
		[Header("Moon")]
		public Gradient MoonColor;
		public AnimationCurve MoonIntensity;
		
		public float currentGameTime;
		public float prevGameTime;
		public float GameTime { get { return currentGameTime + prevGameTime; } }
		public float NormalizedTime;
		public float TimeIncrement;
	
	
        void Start()
		{
			TimeIncrement = 1f / DayDuration;
			NormalizedTime = CurrentHour / 24f;
			RenderSettings.fogMode = FogMode;
		}

		private void OnEnable()
		{
			Extensions.onSaveData += SaveData;
			Extensions.onLoadData += LoadData;
		}
		
		private void OnDisable()
		{
			Extensions.onSaveData -= SaveData;
			Extensions.onLoadData -= LoadData;
		}
		
        void Update()
		{
			if(!isEnabled)
				return;
			
			currentGameTime = Time.timeSinceLevelLoad;
			NormalizedTime += Time.deltaTime * TimeIncrement;
			CurrentHour = (int)(NormalizedTime * 24f);
			if (CurrentHour > 24)
				NormalizedTime = 0f;
			
			RenderSettings.ambientIntensity = Mathf.Clamp01(Sun.intensity);
			RenderSettings.fogDensity = FogIntensity.Evaluate(NormalizedTime);
			RenderSettings.fogColor = FogColor.Evaluate(NormalizedTime);
			
			Sun.transform.rotation = Quaternion.Euler(RotationAxis * (NormalizedTime * 360f - 90f));
			Sun.intensity = SunIntensity.Evaluate(NormalizedTime);
			Sun.color = SunColor.Evaluate(NormalizedTime);

			Moon.transform.rotation = Quaternion.Euler(-RotationAxis * (NormalizedTime * 360f - 90f));
			Moon.intensity = MoonIntensity.Evaluate(NormalizedTime);
			Moon.color = MoonColor.Evaluate(NormalizedTime);
			
			if(Skybox)
				Skybox.SetFloat("_Blend", SkyboxBlend.Evaluate(NormalizedTime));
		}
		
		void SaveData()
		{
			PlayerPrefs.SetFloat("NormalizedTime", NormalizedTime);
			PlayerPrefs.SetFloat("GameTime", prevGameTime);
		}
		
		void LoadData()
		{
			NormalizedTime = PlayerPrefs.GetFloat("NormalizedTime", 0);
			prevGameTime = PlayerPrefs.GetFloat("GameTime", 0);
		}
		
		private static TimeOfDay instance;
        public static TimeOfDay Instance
        {
            get
            {
                if (instance == null) { instance = FindObjectOfType<TimeOfDay>(); }
                return instance;
            }
        }
    }
}
