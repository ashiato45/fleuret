using UnityEngine;
using System.Collections;
using ExtensionMethod;

public enum HittingState
{
    TipToTip,
    Tip0ToBar1,
    Tip1ToBar0,
    Crossing,
    None,
}

public enum GameState
{
    Count,
    Battle,
    Gameset0,
    Gameset1,
    Announce,
}

public enum BattleMode
{
    Challenge,
    Battle,
}

public struct PointToSegmentInfo
{
    public float distance;
    public float ratio;
    public bool inside;
    public Vector2 nearest;
    public PointToSegmentInfo(float d_, float r_, Vector2 n_)
    {
        distance = d_;
        ratio = r_;
        inside = (0 <= r_) && (r_ <= 1);
        nearest = n_;
    }
}

public class Referee : MonoBehaviour {
    public Sword[] swords;
    public GameObject redWin;
    public GameObject blueWin;
    public GameObject[] counts;
    float oneMinuteAfter;
    int counter;
    public static GameState state;
    int round;
    public UnityEngine.UI.Text roundBoard;
    public static BattleMode mode;
    public static bool record = false;
    public HitSound sound;
    bool winPlayOnce = false;
    public GameObject fallingStar;

    public static ESwordControllers[] nextSwordControlller = { ESwordControllers.General, ESwordControllers.WASD };

    void InitBattle()
    {
        fallingStar.SetActive(false);
        fallingStar.GetComponent<ParticleSystem>().enableEmission = false;

        winPlayOnce = false;

        swords[0].record = record;
        swords[1].record = record;

        redWin.SetActive(false);
        blueWin.SetActive(false);

        swords[0].frozen = true;
        swords[1].frozen = true;

        oneMinuteAfter = Time.time + 1f;
        counter = 2;
        for (int i = 0; i < 3; i++)
        {
            counts[i].SetActive(false);
        }
        //counts[2].SetActive(true);

        UnityEngine.Debug.Log(nextSwordControlller[0].ToString() + "+" + nextSwordControlller[1].ToString());
        swords[0].SetController(nextSwordControlller[0]);
        swords[1].SetController(nextSwordControlller[1]);

    }

	// Use this for initialization
	void Start () {
        //state = GameState.Count;
        InitBattle();
        round = 1;
        roundBoard.text = "";

        if (record)
        {
            swords[0].sw = new System.IO.StreamWriter(swords[0].recordName);
            swords[1].sw = new System.IO.StreamWriter(swords[1].recordName);

        }

	}

    void Explode(Vector3 v_, float size_)
    {
        size_ *= 1f;
        GameObject go = Instantiate(Resources.Load("Explosions/Flash02"), v_, Quaternion.identity) as GameObject;
        //go.particleSystem.startSpeed = size_;
        go.GetComponent<ParticleSystem>().startSize = size_;
        Destroy(go, 3);

    }

	// Update is called once per frame
	void Update () {
        if (state == GameState.Announce)
        {
            InitBattle();
            swords[0].SetInitialPosition();
            swords[1].SetInitialPosition();
            roundBoard.text = string.Format("Round {0}", round);
            if (InputControl.getOK())
            {
                roundBoard.text = "";
                state = GameState.Count;
                counts[counter].SetActive(true);
            }
        }
        else if(state == GameState.Count)
        {
            for (int i = 0; i < 3; i++)
            {
                counts[i].SetActive(false);
            }
            counts[counter].SetActive(true);

            if (Time.time >= oneMinuteAfter)
            {
                oneMinuteAfter = Time.time + 1f;
                counter--;
                if (counter < 0)
                {
                    state = GameState.Battle;
                    swords[0].frozen = false;
                    swords[1].frozen = false;
                    for (int i = 0; i < 3; i++)
                    {
                        counts[i].SetActive(false);
                    }
                }
            }
        }
        else if (state == GameState.Battle)
        {
            UpdateBattle();
        }
        else if (state == GameState.Gameset0)
        {
            redWin.SetActive(true);
        }
        else if (state == GameState.Gameset1)
        {
            blueWin.SetActive(true);
        }

        if ((state == GameState.Gameset0 || state == GameState.Gameset1) && InputControl.getOK())
        {
            //record
            if (record)
            {
                swords[0].sw.Close();
                swords[1].sw.Close();
            }
            //record

            if (mode == BattleMode.Battle)
            {
                Application.LoadLevel("title");
            }
            else if (mode == BattleMode.Challenge)
            {
                if (state == GameState.Gameset0)
                {
                    state = GameState.Announce;
                    round++;
                    if (round >= 11)
                    {
                        // Congraturation!
                        var n = SaveDataManager.data.winCount;
                        n++;
                        SaveDataManager.data.winCount = n;
                        SaveDataManager.Save();
                        Application.LoadLevel("title");
                    }
                }
                else if (state == GameState.Gameset1)
                {
                    Application.LoadLevel("title");
                }

            }
        }

	}

    void UpdateBattle()
    {
        var tipRad = swords[0].tip.transform.localScale.x / 2;
        var bodyRad = swords[0].body.transform.localScale.x / 2;
        var barWidth = swords[0].bar.transform.localScale.x;

        var hs = HittingState.None;
        var t0 = swords[0].tip.transform.position.ProjectPlane();
        var t1 = swords[1].tip.transform.position.ProjectPlane();
        var b0 = swords[0].body.transform.position.ProjectPlane();
        var b1 = swords[1].body.transform.position.ProjectPlane();
        var tip0tobar1 = GetPointToSegmentInfo(b1, t1, t0);
        var tip1tobar0 = GetPointToSegmentInfo(b0, t0, t1);

        //dcursor1.transform.position = tip0tobar1.nearest;
        //dcursor2.transform.position = tip1tobar0.nearest;

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

        // Tell strategy
        swords[0].lastHit = Sword.ELastHit.None;
        swords[1].lastHit = Sword.ELastHit.None;
        switch(hs){
            case HittingState.TipToTip:
                swords[0].lastHit = Sword.ELastHit.TipToTip;
                swords[1].lastHit = Sword.ELastHit.TipToTip;
                break;
            case HittingState.Tip0ToBar1:
                swords[0].lastHit = Sword.ELastHit.Tip0ToBar1;
                swords[1].lastHit = Sword.ELastHit.Tip0ToBar1;
                break;
            case HittingState.Tip1ToBar0:
                swords[0].lastHit = Sword.ELastHit.Tip1ToBar0;
                swords[1].lastHit = Sword.ELastHit.Tip1ToBar0;
                break;

        }

        // reflection

        float relSpeed = 0f;

        switch (hs)
        {
            case HittingState.TipToTip:
                {
                    var relVel = swords[0].velocity.Copy();
                    swords[0].velocity = Vector2.zero;
                    swords[1].velocity -= relVel;
                    relSpeed = swords[1].velocity.magnitude;
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
                    relSpeed = acting;
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
                    relSpeed = acting;
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

        // explode
        switch (hs)
        {
            case HittingState.TipToTip:
                Explode(((t0 + t1) / 2).Embed(), relSpeed);
                break;
            case HittingState.Tip0ToBar1:
                Explode(tip0tobar1.nearest.Embed(), relSpeed);
                break;
            case HittingState.Tip1ToBar0:
                Explode(tip1tobar0.nearest.Embed(), relSpeed);
                break;
        }

        // gameset?
        var gameset = false;
        if ((t0 - b1).magnitude < tipRad + bodyRad)
        {
            state = GameState.Gameset0;
            swords[0].frozen = true;
            swords[1].frozen = true;
            gameset = true;

            fallingStar.SetActive(true);
            fallingStar.GetComponent<ParticleSystem>().enableEmission = true;
        }
        else if ((t1 - b0).magnitude < tipRad + bodyRad)
        {
            state = GameState.Gameset1;
            swords[0].frozen = true;
            swords[1].frozen = true;
            //fallingStar.startColor = swords[1].color;
            gameset = true;
            if (mode == BattleMode.Battle)
            {
                fallingStar.SetActive(true);
                fallingStar.GetComponent<ParticleSystem>().enableEmission = true;
            }

        }


        // sound
        if (hs == HittingState.Tip0ToBar1 || hs == HittingState.Tip1ToBar0 || hs == HittingState.TipToTip)
        {
            //UnityEngine.Debug.Log(relSpeed);
            if (relSpeed > 0.1)
            {
                sound.strong.Play();
            }
            else
            {
                sound.weak.Play();
            }
        }

        if (gameset && !winPlayOnce)
        {
            winPlayOnce = true;
            sound.win.Play();
        }


        // fix

        swords[0].Fix();
        swords[1].Fix();

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

}
