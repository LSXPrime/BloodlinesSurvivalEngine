using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class HelicopterUserControl : MonoBehaviour
    {
		private HelicopterController m_Helicopter;
		
		public float Height;
		public float Pitch;
		public float Yaw;
		public float Roll;
		
        void Start()
        {
			m_Helicopter = GetComponent<HelicopterController>();
        }
		
        void FixedUpdate()
        {
			Height += Input.GetAxis("Mouse Y") * Time.fixedDeltaTime;
			Height = Mathf.Clamp(Height, 0.1f, 1f);
			Pitch += InputManager.GetAxis("Vertical") * Time.fixedDeltaTime;
			Pitch = Mathf.Clamp(Pitch, -1.2f, 1.2f);
			Yaw += Input.GetAxis("Mouse X") * Time.fixedDeltaTime;
			Roll += -InputManager.GetAxis("Horizontal") * Time.fixedDeltaTime;
			Roll = Mathf.Clamp(Roll, -1.2f, 1.2f);
			
			if (Input.GetAxis("Mouse Y") == 0f)
			{
				float Altitude = Mathf.Abs(m_Helicopter.RB.mass * Physics.gravity.y) / m_Helicopter.EngineForce; //to keep altitude.
				Height = Mathf.Lerp(Height, Altitude, Time.deltaTime);
			}
			
			m_Helicopter.Move(Height, Pitch, Yaw, Roll);
        }
    }
}
