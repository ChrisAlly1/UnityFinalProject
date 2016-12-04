using UnityEngine;
using UnityEngine.Networking;

public class FireSpawner : NetworkBehaviour {
    public GameObject firePrefab;
    public int numberOfFires;
    public float fireRespawn = 10.0f;
    [SyncVar]
    Quaternion spawnRotation;
   
    public override void OnStartServer() {
        spawnRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

        Spawn();
    }

    void Update()
    {
        fireRespawn -= Time.deltaTime;
        if (fireRespawn <= 0)
        {
            Spawn();
        }
    }

    void Spawn() {
        for (int i = 0; i < numberOfFires; i++) {
            Vector3 spawnPosition = new Vector3(Random.Range(-32.0f, 32.0f), 1.2f, Random.Range(-32.0f, 32.0f));

            GameObject fire = (GameObject)Instantiate(firePrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(fire);
            fireRespawn = 10.0f;
        }
    }
}