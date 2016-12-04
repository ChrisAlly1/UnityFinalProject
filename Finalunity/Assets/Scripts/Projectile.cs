using UnityEngine;

public class Projectile : MonoBehaviour {
    int damage = 10;

    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Enemy") || c.CompareTag("Player")) {
            GameObject hit = c.gameObject;

            Health health = hit.GetComponent<Health>();
            if (health != null) {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }

    public void SetDoubleDamage() {
        damage = 20;
    }
}