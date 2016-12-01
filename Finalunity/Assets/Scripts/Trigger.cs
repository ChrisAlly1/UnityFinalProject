using UnityEngine;

public class Trigger : MonoBehaviour {
    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Enemy") || c.CompareTag("Player")) {
            GameObject hit = c.gameObject;

            Health health = hit.GetComponent<Health>();
            if (health != null) {
                health.TakeDamage(10);
            }

            Destroy(gameObject);
        }
    }
}