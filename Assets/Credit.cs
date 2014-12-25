using UnityEngine;
using System.Collections;

public class Credit : MonoBehaviour {
    CursorState state;
    public GameObject[] items;
    public GameObject cursor;
    public int weight = 100;
    int remaining;
    int pos = 0;
	// Use this for initialization
	void Start () {
        state = CursorState.Ready;
        remaining = weight;
	}
	
	// Update is called once per frame
	void Update () {
	    var power = InputControl.getPower();
        if (power.magnitude < 0.01)
        {
            state = CursorState.Ready;
        }
        if (power.magnitude > 0.9)
        {
            var inc = power.x > 0 ? -1 : 1;
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

        cursor.transform.position = items[pos].transform.position;

        if (InputControl.getOK())
        {
            switch (pos)
            {
                case 0:
                    Application.LoadLevel("creditIPA");
                    break;
                case 1:
                    Application.LoadLevel("title");
                    break;
            }
        }

	}
}
