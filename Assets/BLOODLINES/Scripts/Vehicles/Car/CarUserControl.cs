using UnityEngine;


public class CarUserControl : MonoBehaviour
{

    private CarController m_Car; // the car controller we want to use
	
	[HideInInspector] public float h;
	[HideInInspector] public float v;
	[HideInInspector] public float handbrake;

    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }

    private void FixedUpdate()
    {

        // pass the input to the car!
        h = InputManager.GetAxis("Horizontal");
        v = InputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
        handbrake = (InputManager.GetKey(KeyCode.Space)) ? 1 : 0;
        m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
    }

}