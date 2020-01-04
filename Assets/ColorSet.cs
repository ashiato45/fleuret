using UnityEngine;
using System.Collections;

public class ColorSet : MonoBehaviour {
    public Color color;
    public Shader shader;
	// Use this for initialization
	void Start () {
        this.GetComponent<Renderer>().material.shader = shader;
        //UnityEngine.Debug.Log(this.renderer.material.shader.ToString());
        this.GetComponent<Renderer>().material.color = color;
	}
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Renderer>().material.color = color;
	}
}
