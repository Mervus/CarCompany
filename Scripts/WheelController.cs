using UnityEngine;
using UnityEngine.InputSystem;

namespace CarCompany.Scripts;

public class WheelController : MonoBehaviour
{
    private WheelCollider frontRight;
    private WheelCollider frontLeft;
    private WheelCollider backRight;
    private WheelCollider backLeft;

    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;
    
    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;
    
    private void Start()
    {
        WheelCollider[] colliders = GetComponentsInChildren<WheelCollider>();
        Debug.Log(colliders.Length);
        if (colliders.Length != 4)
        {
            Debug.Log("Error could not find every wheel");
        }
        else
        {
            foreach (var wheelCollider in colliders)
            {
                Debug.Log(wheelCollider.gameObject.name);

                switch (wheelCollider.gameObject.name)
                {
                    case "FrontRightCollider":
                        frontRight = wheelCollider;
                        break;
                    case "FrontLeftCollider":
                        frontLeft = wheelCollider;
                        break;
                    case "BackRightCollider":
                        backRight = wheelCollider;
                        break;
                    case "BackLeftCollider":
                        backLeft = wheelCollider;
                        break;
                }
            }
        }
    }
    
    private void FixedUpdate()
    {
        currentAcceleration = acceleration * GetVerticalAxis();
        
        if (Keyboard.current[Key.RightShift].isPressed)
        {
            currentBrakeForce = breakingForce;
        }
        else
        {
            currentBrakeForce = 0f;
        }
        
        frontRight.motorTorque = currentAcceleration;
        frontLeft.motorTorque = currentAcceleration;

        frontRight.brakeTorque = currentBrakeForce;
        frontLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;
        backLeft.brakeTorque = currentBrakeForce;

        currentTurnAngle = maxTurnAngle * GetHorizontalAxis();
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
    }

    public float GetVerticalAxis()
    {
        if (Keyboard.current[Key.UpArrow].isPressed)
        {
            return 1f;
        }

        if (Keyboard.current[Key.DownArrow].isPressed)
        {
            return -1f;
        }

        return 0f;
    }

    public float GetHorizontalAxis()
    {
        if (Keyboard.current[Key.LeftArrow].isPressed)
        {
            return -1f;
        }

        if (Keyboard.current[Key.RightArrow].isPressed)
        {
            return 1f;
        }

        return 0f;
    }
   
}
