﻿using UnityEngine;
using System.Collections;
using ExtensionMethod;

public enum ESwordControllers
{
    General,
    Arrow,
    WASD,
    LStick,
    RStick,
    NoMove,
    Straight,
    Percentage,
}


public class Sword : MonoBehaviour {
    public enum ELastHit
    {
        None,
        Tip0ToBar1,
        Tip1ToBar0,
        TipToTip,
        Init,
    }

    public GameObject tip;
    public GameObject body;
    public GameObject bar;
    public Vector2 velocity;
    public Color color;
    public GameObject opponent;
    public System.Func<Sword, Vector2> controller;
    public int initLength;
    public ESwordControllers controllerID;
    public bool frozen = false;
    public ELastHit lastHit = ELastHit.None;
    public bool record = false;
    public string recordName;
    public System.IO.StreamWriter sw;

    public void SetInitialPosition()
    {
        tip.transform.localPosition = body.transform.localPosition;
        tip.transform.Translate(0, initLength, 0);

        velocity = Vector2.zero;
    }

	// Use this for initialization
	void Start () {
        velocity = new Vector2();
        var tipColor = tip.GetComponent<ColorSet>();
        tipColor.color = color;
        var bodyColor = body.GetComponent<ColorSet>();
        bodyColor.color = color;
        var barColor = bar.GetComponent<ColorSet>();
        barColor.color = color;


        SetInitialPosition();

        //SetController(controllerID);
	}

    public void SetController(ESwordControllers esc_)
    {
        //UnityEngine.Debug.Log(controllerID);
        switch (esc_)
        {
            case ESwordControllers.General:
                UnityEngine.Debug.Log("SET GEN");
                controller = (s) =>
                {
                    return InputControl.getPower();
                };
                break;
            case ESwordControllers.Arrow:
                UnityEngine.Debug.Log("SET ARROW");
                controller = (s) =>
                {
                    return InputControl.getPowerArrow();
                };
                break;
            case ESwordControllers.WASD:
                controller = (s) =>
                {
                    return InputControl.getPowerWASD();
                };
                break;
            case ESwordControllers.LStick:
                controller = (s) =>
                {
                    return InputControl.getPowerStick();
                };
                break;
            case ESwordControllers.RStick:
                controller = (s) =>
                {
                    return InputControl.getPowerStickRight();
                };
                break;
            case ESwordControllers.Straight:
                controller = (s) =>
                {
                    var e = s.opponent.GetComponent<Sword>();
                    var eb = e.transform.position.ProjectPlane();
                    return (eb - s.tip.transform.position.ProjectPlane()).normalized;
                };
                break;
            case ESwordControllers.NoMove:
                controller = (s) =>
                {
                    return Vector2.zero;
                };
                break;
            case ESwordControllers.Percentage:
                float a = Random.value + 1f;
                float b = Random.value;
                float c = Random.value;
                UnityEngine.Debug.Log(string.Format("{0},{1},{2}", a, b, c));
                float total = a + b + c;
                a /= total;
                b /= total;
                c /= total;
                int strategy = 0;
                float maxBlur = tip.transform.localScale.x;
                float blur = 0;
                float blurDirection = 0;
                lastHit = ELastHit.Init;
                //controller = (s) =>
                //{
                //    if (lastHit != ELastHit.None)
                //    {
                //        strategy = 1;
                //    }
                //    if (strategy == 0)
                //    {
                //        var e = s.opponent.GetComponent<Sword>();
                //        var eb = e.transform.position.ProjectPlane();
                //        return (eb - s.tip.transform.position.ProjectPlane()).normalized * Constants.powerRatio;
                //    }
                //    return Vector2.zero;
                //};
                controller = (s) =>
                {
                    if (lastHit != ELastHit.None)
                    {
                        var r = Random.value;
                        if (r <= a)
                        {
                            strategy = 0;
                        }
                        else if (r <= a + b)
                        {
                            strategy = 1;
                        }
                        else
                        {
                            strategy = 2;
                        }

                        blur = Random.value * maxBlur;
                        blurDirection = Random.value * Mathf.PI * 2f;
                        //UnityEngine.Debug.Log("Strategy:" + strategy.ToString());
                    }

                    var e = s.opponent.GetComponent<Sword>();
                    //var eb = e.transform.position.ProjectPlane();
                    var eb = e.body.transform.position.ProjectPlane();
                    Vector2 target = Vector2.zero;
                    switch (strategy)
                    {
                        case 0:
                            target = e.body.transform.position.ProjectPlane();
                            break;
                        case 1:
                            target = (e.body.transform.position.ProjectPlane() + e.tip.transform.position.ProjectPlane()) / 2f;
                            break;
                        case 2:
                            target = e.tip.transform.position.ProjectPlane();
                            break;
                    }
                    target += (new Vector2(Mathf.Cos(blurDirection), Mathf.Sin(blurDirection)))*blur;
                    return (target - tip.transform.position.ProjectPlane()).normalized;
                };
                break;
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (frozen == false)
        {
            var c = controller(this);
            velocity += c * Constants.powerRatio;

            tip.transform.position += new Vector3(velocity.x, velocity.y, 0);

            if (record)
            {
                sw.WriteLine(c.x.ToString() + "*" + c.y.ToString());
            }
        }
        this.Fix();

	}

    public void Fix()
    {
        bar.transform.localPosition = (tip.transform.localPosition + body.transform.localPosition) / 2;
        bar.transform.LookAt(tip.transform.position);
        var temp = bar.transform.localScale;
        temp.x = tip.transform.localScale.x / 2;
        temp.y = tip.transform.localScale.y / 2;
        temp.z = (body.transform.localPosition - tip.transform.localPosition).magnitude;
        bar.transform.localScale = temp;

    }
}
