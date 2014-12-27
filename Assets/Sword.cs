using UnityEngine;
using System.Collections;
using ExtensionMethod;

public enum ESwordControllers
{
    Arrow,
    WASD,
    Straight,
    NoMove,
}



public class Sword : MonoBehaviour {
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


	// Use this for initialization
	void Start () {
        velocity = new Vector2();
        var tipColor = tip.GetComponent<ColorSet>();
        tipColor.color = color;
        var bodyColor = body.GetComponent<ColorSet>();
        bodyColor.color = color;
        var barColor = bar.GetComponent<ColorSet>();
        barColor.color = color;

        tip.transform.localPosition = body.transform.localPosition;
        tip.transform.Translate(0, initLength, 0);

        SetController(controllerID);
	}

    public void SetController(ESwordControllers esc_)
    {
        //UnityEngine.Debug.Log(controllerID);
        switch (esc_)
        {
            case ESwordControllers.Arrow:
                controller = (s) =>
                {
                    return InputControl.getPower() * Constants.powerRatio;
                };
                break;
            case ESwordControllers.WASD:
                controller = (s) =>
                {
                    return InputControl.getPowerWASD() * Constants.powerRatio;
                };
                break;
            case ESwordControllers.Straight:
                controller = (s) =>
                {
                    var e = s.opponent.GetComponent<Sword>();
                    var eb = e.transform.position.ProjectPlane();
                    return (eb - s.tip.transform.position.ProjectPlane()).normalized * Constants.powerRatio;
                };
                break;
            case ESwordControllers.NoMove:
                controller = (s) =>
                {
                    return Vector2.zero;
                };
                break;
                
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (frozen == false)
        {
            velocity += controller(this);

            tip.transform.position += new Vector3(velocity.x, velocity.y, 0);

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
