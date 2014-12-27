using UnityEngine;
using System.Collections;
using ExtensionMethod;

public enum TutorialState
{
    Welcome,
    Move,
    Foin,
    Reflect,
    End,
}

public class Tutor : MonoBehaviour {
    public Sword[] swords;
    TutorialState state;
    Vector3[] initPos;
    string[] messages;
    public UnityEngine.UI.Text description;
    public GameObject ok;

 	// Use this for initialization
	void Start () {
        ok.SetActive(false);

        initPos = new Vector3[2];
        swords[0].gameObject.SetActive(false);
        swords[1].gameObject.SetActive(false);
        initPos[0] = swords[0].tip.transform.position;
        initPos[1] = swords[1].tip.transform.position;
        state = TutorialState.Welcome;

        messages = new string[5];
        messages[0] = @"Fleuretへようこそ！
チュートリアルは[決定]ボタンで先へ進みます。";
        messages[2] = @"剣先で相手の胴を突くと勝利となります。
画面上にある相手の胴を突いてみましょう。";
        messages[3] = @"双方の剣がぶつかると、各々の勢いに応じて跳ね返ります。
防御や攻撃に活用できます。";
        messages[4] = @"チュートリアルは以上です。
お疲れさまでした。
Fleuretをお楽しみください。";
        if (InputControl.inputDevice == InputDevice.Keyboard)
        {
            messages[1] = @"上下左右キーで剣先を動かしてみましょう。
剣先は動かすと勢いがつくので、うまく操りましょう。";
        }
        else
        {
            messages[1] = @"スティックで剣先を動かしてみましょう。
剣先は動かすと勢いがつくので、うまく操りましょう。";
        }
	}


    void InitializePosition()
    {
        swords[0].tip.transform.position = initPos[0];
        swords[0].velocity = Vector2.zero;
        swords[1].tip.transform.position = initPos[1];
        swords[1].velocity = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        description.text = messages[state.GetHashCode()];
        if (state == TutorialState.Welcome)
        {
            if (InputControl.getOK())
            {
                state = TutorialState.Move;
                swords[0].controllerID = ESwordControllers.Arrow;
                swords[0].gameObject.SetActive(true);
                InitializePosition();
            }
        }
        else if (state == TutorialState.Move)
        {
            WallStop();
            if (InputControl.getOK())
            {
                state = TutorialState.Foin;
                InitializePosition();
                swords[1].gameObject.SetActive(true);
                swords[1].controllerID = ESwordControllers.NoMove;
            }
        }
        else if (state == TutorialState.Foin)
        {
            swords[1].tip.transform.position = swords[1].body.transform.position;
            WallStop();
            if (InputControl.getOK())
            {
                state = TutorialState.Reflect;
                InitializePosition();
            }

            var tipRad = swords[0].tip.transform.localScale.x / 2;
            var bodyRad = swords[0].body.transform.localScale.x / 2;
            var t0 = swords[0].tip.transform.position.ProjectPlane();
            //var t1 = swords[1].tip.transform.position.ProjectPlane();
            //var b0 = swords[0].body.transform.position.ProjectPlane();
            var b1 = swords[1].body.transform.position.ProjectPlane();

            if ((t0 - b1).magnitude < tipRad + bodyRad)
            {
                ok.SetActive(true);
            }
            else
            {
                ok.SetActive(false);
            }

        }
        else if (state == TutorialState.Reflect)
        {
            UpdateBattle();
            if (InputControl.getOK())
            {
                state = TutorialState.End;
                InitializePosition();
            }
        }
        else if (state == TutorialState.End)
        {
            UpdateBattle();
            if (InputControl.getOK())
            {
                Application.LoadLevel("title");
            }
        }



	}

    void UpdateBattle()
    {
        var tipRad = swords[0].tip.transform.localScale.x / 2;
        //var bodyRad = swords[0].body.transform.localScale.x / 2;
        var barWidth = swords[0].bar.transform.localScale.x;

        var hs = HittingState.None;
        var t0 = swords[0].tip.transform.position.ProjectPlane();
        var t1 = swords[1].tip.transform.position.ProjectPlane();
        var b0 = swords[0].body.transform.position.ProjectPlane();
        var b1 = swords[1].body.transform.position.ProjectPlane();
        var tip0tobar1 = GetPointToSegmentInfo(b1, t1, t0);
        var tip1tobar0 = GetPointToSegmentInfo(b0, t0, t1);

        System.Action<string> f = (s) =>
        {
            //return;
            //UnityEngine.Debug.Log(Time.frameCount.ToString() + ":" + s);
        };

        if ((swords[0].tip.transform.position - swords[1].tip.transform.position).magnitude <= tipRad * 2)
        {
            hs = HittingState.TipToTip;
            f("tip to tip");
        }
        else if (AreCrossing())
        {
            hs = HittingState.Crossing;
            f("crossing");
        }
        else if (tip0tobar1.inside && tip0tobar1.distance <= tipRad + barWidth / 2)
        {
            hs = HittingState.Tip0ToBar1;
            f("tip0 is touching bar1");

        }
        else if (tip1tobar0.inside && tip1tobar0.distance <= tipRad + barWidth / 2)
        {
            hs = HittingState.Tip1ToBar0;
            f("tip0 is touching bar1");
        }

        // reflection
        switch (hs)
        {
            case HittingState.TipToTip:
                {
                    var relVel = swords[0].velocity.Copy();
                    swords[0].velocity = Vector2.zero;
                    swords[1].velocity -= relVel;
                    swords[0].velocity = swords[1].velocity;
                    swords[1].velocity = Vector2.zero;
                    swords[0].velocity += relVel;
                    swords[1].velocity += relVel;
                }
                break;
            case HittingState.Tip0ToBar1:
                {
                    var relVel = swords[1].velocity.Copy();
                    swords[0].velocity -= relVel;
                    swords[1].velocity -= relVel;
                    var n = (t1 - b1).Rotate90CCW().normalized.Copy();
                    var acting = swords[0].velocity.Dot(n);
                    swords[1].velocity += n * (acting);
                    swords[0].velocity -= n * (acting);
                    swords[0].velocity += relVel;
                    swords[1].velocity += relVel;
                }
                break;
            case HittingState.Tip1ToBar0:
                {
                    var relVel = swords[0].velocity.Copy();
                    swords[0].velocity -= relVel;
                    swords[1].velocity -= relVel;
                    var n = (t0 - b0).Rotate90CCW().normalized.Copy();
                    var acting = swords[1].velocity.Dot(n);
                    swords[0].velocity += n * (acting);
                    swords[1].velocity -= n * (acting);
                    swords[1].velocity += relVel;
                    swords[0].velocity += relVel;
                }
                break;
            case HittingState.Crossing:
                {
                    swords[0].velocity = Vector2.zero;
                    swords[1].velocity = Vector2.zero;
                }
                break;
        }

        // embedding recover
        if (hs == HittingState.Tip0ToBar1)
        {
            var removing = (tip0tobar1.nearest - t0).normalized.Copy();
            var removingLength = (barWidth / 2 + tipRad) * 1.2f;
            swords[0].tip.transform.position = tip0tobar1.nearest - removing * removingLength;
        }
        else if (hs == HittingState.Tip1ToBar0)
        {
            var removing = (tip1tobar0.nearest - t1).normalized.Copy();
            var removingLength = (barWidth / 2 + tipRad) * 1.2f;
            swords[1].tip.transform.position = tip1tobar0.nearest - removing * removingLength;
        }
        else if (hs == HittingState.Crossing)
        {
            if (tip0tobar1.distance <= tip1tobar0.distance)
            {
                var removing = (tip0tobar1.nearest - t0).normalized.Copy();
                var removingLength = (barWidth / 2 + tipRad) * 1.2f;
                swords[0].tip.transform.position = tip0tobar1.nearest - removing * removingLength;
                if (AreCrossing() == true)
                {
                    swords[0].tip.transform.position = tip0tobar1.nearest + removing * removingLength;

                }

            }
            else
            {
                var removing = (tip1tobar0.nearest - t1).normalized.Copy();
                var removingLength = (barWidth / 2 + tipRad) * 1.2f;
                swords[1].tip.transform.position = tip1tobar0.nearest - removing * removingLength;
                if (AreCrossing() == true)
                {
                    swords[1].tip.transform.position = tip1tobar0.nearest + removing * removingLength;
                }
            }
        }
        else if (hs == HittingState.TipToTip)
        {
            var center = (t0 + t1) / 2f;
            var removing = (t1 - t0).normalized.Copy();
            var removingLength = tipRad * 1.2f;
            swords[0].tip.transform.position = center - removing * removingLength;
            swords[1].tip.transform.position = center + removing * removingLength;
        }

        WallStop();

        // gameset?
        //if (state == TutorialState.Foin)
        //{
        //    if ((t0 - b1).magnitude < tipRad + bodyRad)
        //    {
        //        ok.SetActive(true);
        //        //state = GameState.Gameset0;
        //        //swords[0].frozen = true;
        //        //swords[1].frozen = true;
        //    }
        //    else
        //    {
        //        ok.SetActive(false);
        //    }
        //    //else if ((t1 - b0).magnitude < tipRad + bodyRad)
        //    //{
        //    //    state = GameState.Gameset1;
        //    //    swords[0].frozen = true;
        //    //    swords[1].frozen = true;
        //    //}
        //}

        // fix

        swords[0].Fix();
        swords[1].Fix();

    }

    void WallStop()
    {
        var tipRad = swords[0].tip.transform.localScale.x / 2;
        //var bodyRad = swords[0].body.transform.localScale.x / 2;
        //var barWidth = swords[0].bar.transform.localScale.x;

        //var hs = HittingState.None;
        var t0 = swords[0].tip.transform.position.ProjectPlane();
        var t1 = swords[1].tip.transform.position.ProjectPlane();
        //var b0 = swords[0].body.transform.position.ProjectPlane();
        //var b1 = swords[1].body.transform.position.ProjectPlane();
        //var tip0tobar1 = GetPointToSegmentInfo(b1, t1, t0);
        //var tip1tobar0 = GetPointToSegmentInfo(b0, t0, t1);

        // wall stop
        var w = Camera.main.aspect * Camera.main.orthographicSize;
        var h = Camera.main.orthographicSize;
        for (int i = 0; i < 2; i++)
        {
            Vector2 t = i == 0 ? t0 : t1;
            //if (t.x < -w + tipRad)
            if (t.x < -w)
            {
                t.x = -w + tipRad;
                swords[i].tip.transform.position = t.Embed();
                swords[i].velocity = Vector2.zero;
            }
            //if (t.x > w - tipRad)
            if (t.x > w)
            {
                t.x = w - tipRad;
                swords[i].tip.transform.position = t.Embed();
                swords[i].velocity = Vector2.zero;
            }
            //if (t.y < -h + tipRad)
            if (t.y < -h)
            {
                t.y = -h + tipRad;
                swords[i].tip.transform.position = t.Embed();
                swords[i].velocity = Vector2.zero;
            }
            //if (t.y > h - tipRad)
            if (t.y > h)
            {
                t.y = h - tipRad;
                swords[i].tip.transform.position = t.Embed();
                swords[i].velocity = Vector2.zero;
            }
        }

    }

    public bool AreCrossing()
    {
        var p0 = swords[0].body.transform.position.ProjectPlane();
        var p1 = swords[0].tip.transform.position.ProjectPlane();
        var q0 = swords[1].body.transform.position.ProjectPlane();
        var q1 = swords[1].tip.transform.position.ProjectPlane();

        var qbetweenp0p1 = ((q1 - q0).Determinant(p1 - q0) * (q1 - q0).Determinant(p0 - q0) <= 0);
        var pbetweenq0q1 = ((p1 - p0).Determinant(q1 - p0) * (p1 - p0).Determinant(q0 - p0) <= 0);
        return qbetweenp0p1 && pbetweenq0q1;
    }

    public static PointToSegmentInfo GetPointToSegmentInfo(Vector2 p1_, Vector2 p2_, Vector2 p_)
    {
        float d = (p2_ - p1_).magnitude;
        float d1 = (p_ - p1_).magnitude;
        float d2 = (p_ - p2_).magnitude;
        float ratio = (d1 * d1 - d2 * d2 + d * d) / (2 * d * d);
        Vector2 nearest = (1 - ratio) * p1_ + ratio * p2_;
        float distance = (nearest - p_).magnitude;

        return new PointToSegmentInfo(distance, ratio, nearest);

    }

}
