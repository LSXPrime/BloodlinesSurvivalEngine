using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LBSE
{
	public class AccountInfo : MonoBehaviour
	{
		public int Kills = 0;
		public int Score = 0;
		public int Coins = 0;
		public int Cash = 0;
		public int Tokens = 0;
		
		void Start()
		{
			DontDestroyOnLoad(gameObject);
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
		
		void LoadData()
		{
			Kills = PlayerPrefs.GetInt("Kills", 0);
			Score = PlayerPrefs.GetInt("Score", 0);
			Coins = PlayerPrefs.GetInt("Coins", 0);
			Cash = PlayerPrefs.GetInt("Cash", 0);
			Tokens = PlayerPrefs.GetInt("Tokens", 0);
		}
		
		void SaveData()
		{
			PlayerPrefs.SetInt("Kills", Kills);
			PlayerPrefs.SetInt("Score", Score);
			PlayerPrefs.SetInt("Coins", Coins);
			PlayerPrefs.SetInt("Cash", Cash);
			PlayerPrefs.SetInt("Tokens", Tokens);
		}
		
		private static AccountInfo _Instance;
		public static AccountInfo Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = FindObjectOfType<AccountInfo>();
				}
				return _Instance;
			}
		}
	}
}
