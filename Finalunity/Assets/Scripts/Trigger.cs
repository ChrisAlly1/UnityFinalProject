using UnityEngine;
using UnityEngine.Networking;

    public class Trigger :  MonoBehaviour
    {
        public bool dmg = false;
        public int damageamt = 10;
        public GameObject self;
        void OnTriggerEnter(Collider c)
        {
            if (c.CompareTag("Enemy") || c.CompareTag("Player"))
            {
                GameObject hit = c.gameObject;
              
                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damageamt);
                }
                if (dmg == true)
                {
                    health.TakeDamage(damageamt);
                }

                Destroy(gameObject);
            }
        }
    }