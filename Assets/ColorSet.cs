using UnityEngine;
using System.Collections;

public class ColorSet : MonoBehaviour {
    public Color color;
    public Shader shader;
	// Use this for initialization
	void Start () {
        this.renderer.material.shader = shader;
        //UnityEngine.Debug.Log(this.renderer.material.shader.ToString());
        this.renderer.material.color = color;
	}
	
	// Update is called once per frame
	void Update () {
        this.renderer.material.color = color;
	}
}
