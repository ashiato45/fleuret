using UnityEngine;
using System.Collections;

public class InputNotifier : MonoBehaviour {
    public UnityEngine.UI.Text board;
    public int weight = 60;
    int remaining = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (remaining <= 0)
        {
            board.text = "";
        }

        bool start = false;
        if (Input.GetJoystickNames().Length > 0 && Input.anyKey)
        {
            start = true;
        }
        if (Input.GetMouseButton(0))
        {
            start = true;
        }

        if (start)
        {
            remaining = weight;
        }

        Show();

	}

    void Show()
    {
        remaining--;
        if (remaining <= 0)
        {
            board.text = "";
        }
        else
        {
            if (Input.GetJoystickNames().Length > 0)
            {
                board.text = "現在の入力はJoypadです。";
            }
            else
            {
                board.text = "現在の入力はKeyboardです。";
            }
        }
    }
}
