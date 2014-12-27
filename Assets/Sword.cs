using UnityEngine;
using System.Collections;
using ExtensionMethod;

public enum ESwordControllers
{
    Arrow,
    WASD,
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
                        UnityEngine.Debug.Log("Strategy:" + strategy.ToString());
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
                    return (target - tip.transform.position.ProjectPlane()).normalized * Constants.powerRatio;
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
