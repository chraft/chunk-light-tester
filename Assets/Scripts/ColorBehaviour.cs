using UnityEngine;
using System.Collections;

public class ColorBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		renderer.material.color = Color.cyan;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
