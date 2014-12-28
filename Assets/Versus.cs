using UnityEngine;
using System.Collections;

public class Versus : MonoBehaviour {
    public enum EHeight
    {
        Red,
        Blue,
        Start,
    }

    public enum EOperation
    {
        Arrow,
        WASD,
        LStick,
        RStick,
    }
    bool ready;
    public int height;
    public static EOperation[] operations = new EOperation[2];
    string[] operationMessage;
    public UnityEngine.UI.Text[] boards;
    public GameObject cursor;
    public GameObject diff;

	// Use this for initialization
	void Start () {
        ready = true;
        height = 0;
        operationMessage = new string[4];
        operationMessage[0] = "矢印キー";
        operationMessage[1] = "WASD";
        operationMessage[2] = "左スティック";
        operationMessage[3] = "右スティック";
        diff.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        bool coherent = operations[0] == operations[1];

        if (InputControl.getPower().magnitude < 0.5f)
        {
            ready = true;
        }
        if (InputControl.getPower().magnitude > 0.9f && ready == true)
        {
            ready = false;
            if(Mathf.Abs(InputControl.getPower().y) >= Mathf.Abs(InputControl.getPower().x)){
                var d = InputControl.getPower().y > 0 ? -1 : 1;
                height = (height + d + 3) % 3;

            }
            else
            {
                if (0 <= height && height <= 1)
                {
                    var d = InputControl.getPower().x > 0 ? 1 : -1;
                    operations[height] = (EOperation)((operations[height].GetHashCode() + d + 4) % 4);
                }
            }
        }

        // go
        if (InputControl.getOK() && height == 2 && !coherent)
        {
            for (int i = 0; i < 2; i++)
            {
                switch (operations[i])
                {
                    case EOperation.Arrow:
                        Referee.nextSwordControlller[i] = ESwordControllers.Arrow;
                        break;
                    case EOperation.WASD:
                        Referee.nextSwordControlller[i] = ESwordControllers.WASD;
                        break;
                    case EOperation.LStick:
                        Referee.nextSwordControlller[i] = ESwordControllers.LStick; 
                        break;
                    case EOperation.RStick:
                        Referee.nextSwordControlller[i] = ESwordControllers.RStick;
                        break;
                }
            }
            Referee.mode = BattleMode.Battle;
            Referee.state = GameState.Count;
            Application.LoadLevel("main_2");
        }

        //show
        for (int i = 0; i < 2; i++)
        {
            boards[i].text = operationMessage[operations[i].GetHashCode()];
        }
        cursor.transform.position = boards[height].transform.position;
        diff.SetActive(coherent);

	}
}
