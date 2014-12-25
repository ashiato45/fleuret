using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitList : MonoBehaviour {
    public HashSet<GameObject> hittingSet;
	// Use this for initialization
	void Start () {
        hittingSet = new HashSet<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        hittingSet.Add(collider.gameObject);
        UnityEngine.Debug.Log("hit");
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        hittingSet.Remove(collider.gameObject);
        UnityEngine.Debug.Log(hittingSet.Count);
    }
}
