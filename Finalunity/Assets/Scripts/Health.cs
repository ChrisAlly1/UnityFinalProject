using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {
    public const int maxHealth = 100;
    public float timerDMG = 0.5f;
    public GameObject powerup1;
    public GameObject powerup2;
    public bool destroyOnDeath;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth;
    public RectTransform healthBar;
    
    private NetworkStartPosition[] spawnPoints;
   
    void Start() {
        if (isLocalPlayer) {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }

        currentHealth = maxHealth;
    }
    //used for collision with fire
    void OnTriggerStay(Collider c)
    {
        //added this in there cuz other take damage metho had it
        if (!isServer)
        {
            return;
        }
        //checks so it only takes damage if it collided with the fire
        if (c.gameObject.tag == "Fire")
        {
            timerDMG -= Time.deltaTime;
            if (timerDMG <= 0)
            {
                currentHealth -= 1;
                timerDMG = 0.5f;
            }
        }
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                 int i =(Random.Range(0, 100));

                //this spawns first power up
                if (i < 50)
                {
                    GameObject powerup = (GameObject)Instantiate(powerup1, gameObject.transform.position, gameObject.transform.rotation);
                    NetworkServer.Spawn(powerup);
                }
                //this spawns second power up
                if( i > 50)
                 {
                    GameObject powerup2a = (GameObject)Instantiate(powerup2, gameObject.transform.position, gameObject.transform.rotation);
                    NetworkServer.Spawn(powerup2a);
                }
                Destroy(gameObject);
                
            }
            else
            {
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
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
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
}