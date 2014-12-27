using UnityEngine;
using System.Collections;

public enum InputDevice
{
    Keyboard,
    Joypad,
}

public class InputControl
{
    void Update()
    {

    }

    void Start()
    {

    }

    public static InputDevice inputDevice = InputDevice.Keyboard;
    


    public static Vector2 getPower()
    {
        Vector2 power = new Vector2(0, 0);

        if (Input.GetJoystickNames().Length > 0)
        {
            power = getPowerStick();
        }
        else
        {
            power = getPowerArrow();
        }

        return power;
    }

    public static Vector2 getPowerArrow()
    {
        Vector2 power = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            power -= Vector2.right;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            power += Vector2.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            power += Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            power -= Vector2.up;
        }
        power = power.normalized;
        //UnityEngine.Debug.Log(power);

        return power;
    }


    public static Vector2 getPowerWASD()
    {
        Vector2 power = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.A))
        {
            power -= Vector2.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            power += Vector2.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            power += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            power -= Vector2.up;
        }
        power = power.normalized;
        //UnityEngine.Debug.Log(power);

        return power;
    }

    public static Vector2 getPowerStick()
    {
        Vector2 power = Vector2.zero;
        power.x = Input.GetAxis("Horizontal");
        power.y = Input.GetAxis("Vertical");

        UnityEngine.Debug.Log(power.x.ToString() + "." + power.y.ToString());

        //power = power / Mathf.Sqrt(2);

        return power;
    }

    public static Vector2 getPowerStickRight()
    {
        Vector2 power = Vector2.zero;
        power.x = Input.GetAxis("Horizontal2");
        power.y = Input.GetAxis("Vertical2");
        UnityEngine.Debug.Log(power.x.ToString() + "," + power.y.ToString());

        //power = power / Mathf.Sqrt(2);

        return power;
    }


    public static bool getOK()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            
            return Input.GetButtonDown("Fire1");
        }
        
        return Input.GetKeyDown(KeyCode.Z);
    }


    public static bool getCancel()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            return Input.GetButtonDown("Fire2");
        }
        return Input.GetKeyUp(KeyCode.X);
    }
}

