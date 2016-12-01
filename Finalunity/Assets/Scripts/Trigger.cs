using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {
    public GameObject self;

    void OnCollisionEnter(Collision c) {
        GameObject hit = c.gameObject;
        Health health = hit.GetComponent<Health>();
        if(health != null) {
            health.TakeDamage(10);
        }

        Destroy(self);
    }
}