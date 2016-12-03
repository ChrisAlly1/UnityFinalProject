using UnityEngine;

public class Destory1 : MonoBehaviour {
    public float time;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, time);
    }
}