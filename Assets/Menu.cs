﻿using UnityEngine;
using System.Collections;

public enum CursorState
{
    Ready,
    Heavy
}

public class Menu : MonoBehaviour {
    public GameObject cursor;
    public GameObject[] items;
    public string[] messages;
    //public GameObject description;
    public int weight = 100;
    public UnityEngine.UI.Text description;
    int remaining;
    CursorState state;
    public static int pos = 0;
    public GameObject crown;

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 30;

        SaveDataManager.Load();

        state = CursorState.Ready;

        cursor.transform.position = items[pos].transform.position;

        var win = SaveDataManager.data.winCount;
        if (win == 0)
        {
            crown.SetActive(false);
        }
        else
        {
            crown.SetActive(true);
        }

	}
	
	// Update is called once per frame
	void Update () {
        crown.transform.position = items[0].transform.position + new Vector3(0, crown.transform.localScale.y, 0);

        var power = InputControl.getPower();
        if (Mathf.Abs( power.y) < 0.5)
        {
            state = CursorState.Ready;
        }
        if (Mathf.Abs( power.y) > 0.9)
        {
            var inc = power.y > 0 ? -1 : 1;
            if (state == CursorState.Ready)
            {
                pos = (pos + items.Length + inc) % items.Length;
                state = CursorState.Heavy;
                remaining = weight;
            }
            else if (state == CursorState.Heavy)
            {
                if (remaining <= 0)
                {
                    remaining = weight;
                    pos = (pos + items.Length + inc) % items.Length;
                }
                else
                {
                    remaining--;
                }

            }
        }


        if (InputControl.getOK())
        {
            switch (pos)
            {
                case 0:
                    Referee.nextSwordControlller[0] = ESwordControllers.General;
                    Referee.nextSwordControlller[1] = ESwordControllers.Percentage;
                    Referee.state = GameState.Announce;
                    Referee.mode = BattleMode.Challenge;
                    //Ttemp
                    Referee.record = false;
                    Application.LoadLevel("main_2");
                    break;
                case 1:
                    //Referee.nextSwordControlller[0] = ESwordControllers.General;
                    //Referee.nextSwordControlller[1] = ESwordControllers.WASD;
                    //Referee.state = GameState.Count;
                    //Referee.mode = BattleMode.Battle;
                    Application.LoadLevel("versus");
                    break;
                case 2:
                    Application.LoadLevel("tutorial");
                    break;
                case 3:
                    Application.LoadLevel("credit");
                    break;
                case 4:
                    Application.Quit();
                    break;
            }
        }

        if (InputControl.getCancel())
        {
            pos = 4;
        }

        cursor.transform.position = items[pos].transform.position;
        description.text = messages[pos];


	}
}
