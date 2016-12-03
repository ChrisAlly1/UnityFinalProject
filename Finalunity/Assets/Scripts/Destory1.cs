using UnityEngine;
using System.Collections;

public class Destory1 : MonoBehaviour {
    public GameObject self;
    public float time;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       Destroy(self, time);
	}
}
