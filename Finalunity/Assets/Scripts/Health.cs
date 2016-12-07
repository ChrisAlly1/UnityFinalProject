using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {
    public const int maxHealth = 100;
    public float timerDMG = 0.5f;
    public GameObject[] powerups;
    public bool destroyOnDeath;
    bool invincibility;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth;
    public RectTransform healthBar1;
    public RectTransform healthBar2;

    private NetworkStartPosition[] spawnPoints;

    void Start() {
        if (isLocalPlayer) {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();

            GameObject[] rectTransforms = GameObject.FindGameObjectsWithTag("HealthBar2");
            foreach (GameObject item in rectTransforms) {
                healthBar2 = item.GetComponent<RectTransform>();
            }
        }

        currentHealth = maxHealth;
        invincibility = false;
    }

    //used for collision with fire
    void OnTriggerStay(Collider c) {
        //added this in there cuz other take damage metho had it
        if (!isServer) {
            return;
        }

        //checks so it only takes damage if it collided with the fire
        if (c.gameObject.tag == "Fire") {
            timerDMG -= Time.deltaTime;
            if (timerDMG <= 0) {
                currentHealth -= 1;
                timerDMG = 0.5f;
            }
        }

        if (currentHealth <= 0) {
            if (destroyOnDeath) {
                int z = Random.Range(1, 10);
                if (z <= 5) {
                    int i = (Random.Range(0, 4));

                    GameObject powerup = (GameObject)Instantiate(powerups[i], gameObject.transform.position, gameObject.transform.rotation);
                    NetworkServer.Spawn(powerup);

                    Destroy(powerup, 10);
                }

                EnemySpawner.removeEnemy();
                Destroy(gameObject);
            } else {
                currentHealth = maxHealth;
                RpcRespawn();
            }
        }
    }

    public void TakeDamage(int amount) {
        if (!isServer) {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0) {
            if (destroyOnDeath) {
                Destroy(gameObject);
            } else {
                currentHealth = maxHealth;
                RpcRespawn();
            }
        }
    }

    void OnChangeHealth(int currentHealth) {
        if (!gameObject.CompareTag("Enemy") && healthBar2 != null) {
            healthBar2.sizeDelta = new Vector2(currentHealth * 2, healthBar2.sizeDelta.y);
        } else if (gameObject.CompareTag("Enemy")) {
            healthBar1.sizeDelta = new Vector2(currentHealth, healthBar1.sizeDelta.y);
        }
    }

    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer) {
            Vector3 spawnPoint = Vector3.zero;

            if (spawnPoints != null && spawnPoints.Length > 0) {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            transform.position = spawnPoint;
        }
    }

    public void SwitchInvincibility() {
        invincibility = !invincibility;
    }
}