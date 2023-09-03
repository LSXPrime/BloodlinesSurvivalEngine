using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	public class Helirotator : MonoBehaviour
	{
		public HelicopterController helicopter;
		public Axis axis;
		public float Speed;
		
		void Update()
		{
			if (!helicopter.EngineON)
				return;
			
			float speed = Speed * Time.deltaTime;

			if (speed > 360f) speed = 0f;
			
			switch (axis)
			{
				case Axis.X:
					transform.Rotate(speed, 0f, 0f);
					break;
				case Axis.Y:
					transform.Rotate(0f, speed, 0f);
					break;
				case Axis.Z:
					transform.Rotate(0f, 0f, speed);
					break;
			}
		}
		
		public enum Axis
		{
			X,
			Y,
			Z
		}
	}
}