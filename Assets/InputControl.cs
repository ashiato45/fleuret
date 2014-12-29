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
            var s = getPowerStick();
            var k = getPowerArrow();
            if (s.magnitude > 0.1)
            {
                power = getPowerStick();
            }
            else
            {
                power = getPowerArrow();
            }
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

    public static Vector2 normalizeStick(Vector2 power)
    {
        if (Mathf.Abs(power.x) > 0.01 || Mathf.Abs(power.y) > 0.01)
        {
            if (Mathf.Abs(power.x) > Mathf.Abs(power.y))
            {
                var nowd = power.magnitude;
                var bigd = nowd / Mathf.Abs(power.x);
                power.x /= bigd;
                power.y /= bigd;
            }
            else if (Mathf.Abs(power.x) < Mathf.Abs(power.y))
            {
                var nowd = power.magnitude;
                var bigd = nowd / Mathf.Abs(power.y);
                power.x /= bigd;
                power.y /= bigd;
            }
            else
            {
                power.x /= Mathf.Sqrt(2);
                power.y /= Mathf.Sqrt(2);
            }
        }
        return power;
    }

    public static Vector2 getPowerStick()
    {
        Vector2 power = Vector2.zero;

        power.x = Input.GetAxis("Horizontal");
        power.y = Input.GetAxis("Vertical");

        power = normalizeStick(power);
        //UnityEngine.Debug.Log("L:" + power.x.ToString() + "," + power.y.ToString() + "," + power.magnitude);


        //power = power / Mathf.Sqrt(2);

        return power;
    }

    public static Vector2 getPowerStickRight()
    {
        Vector2 power = Vector2.zero;
        power.x = Input.GetAxis("Horizontal2");
        power.y = Input.GetAxis("Vertical2");
        power = normalizeStick(power);
        //UnityEngine.Debug.Log("R:" + power.x.ToString() + "," + power.y.ToString() + "," + power.magnitude);

        //power = power / Mathf.Sqrt(2);

        return power;
    }


    public static bool getOK()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            
            return (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Z));
        }
        
        return Input.GetKeyDown(KeyCode.Z);
    }


    public static bool getCancel()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            return (Input.GetButtonDown("Fire2") || Input.GetKey(KeyCode.X));
        }
        return Input.GetKeyUp(KeyCode.X);
    }
}

