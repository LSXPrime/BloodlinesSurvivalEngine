using System;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.Events;

namespace LBSE
{
	static class Extensions
	{
		public delegate void SaveData();
		public static SaveData onSaveData;
		
		public static void SaveGameData()
		{
			if (onSaveData != null)
				onSaveData();
		}
		
		public delegate void LoadData();
		public static LoadData onLoadData;
		
		public static void LoadGameData()
		{
			if (onLoadData != null)
				onLoadData();
		}
		
		public static void RandomizeAudioOutput(this AudioSource source)
		{
			source.pitch = UnityEngine.Random.Range(0.92f, 1.1f);
			source.spread = UnityEngine.Random.Range(0.98f, 1.25f);
		}
		
		public static float Lerp(float value1, float value2, float by)
		{
            return value1 * (1f - by) + value2 * by;
        }

		public static bool IsBlockedBy(this Transform source, Transform target)
		{
            return Vector3.Dot(source.forward, (target.position - source.position).normalized) > 0 ? true : false;
        }
		
		public static Vector3 XVector3(this Vector3 vector, int op)
		{
            switch (op)
			{
				case 0:
					vector.x = 0;
                    break;
				case 1:
					vector.y = 0;
                    break;
				case 2:
					vector.z = 0;
                    break;
            }
            
            return vector;
        }

		public static Vector3 XVectors3(this Vector3 vector, int op)
		{
            switch (op)
			{
				case 0:
					vector.y = 0;
					vector.z = 0;
                    break;
				case 1:
					vector.x = 0;
					vector.z = 0;
                    break;
				case 2:
					vector.x = 0;
					vector.y = 0;
                    break;
            }
            
            return vector;
        }

		public static void LockCursor(bool Lock)
		{
			if (Lock == true)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}

		public static string GetMD5(this string text)
		{
			byte[] encodedPassword = new System.Text.UTF8Encoding().GetBytes(text);
			byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
			return System.BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
		}
	}
}
