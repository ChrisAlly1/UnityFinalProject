using UnityEngine;
using UnityEngine.Networking;

public class SnowSpawner : NetworkBehaviour {
    public GameObject snowPrefab;
    public int numberOfSnow;
    public float snowRespawn;
    [SyncVar]
    Quaternion spawnRotation;

    public override void OnStartServer() {
        spawnRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

        Spawn();
    }

    void Update() {
        snowRespawn -= Time.deltaTime;
        if (snowRespawn <= 0) {
            Spawn();
        }
    }

    void Spawn() {
        for (int i = 0; i < numberOfSnow; i++) {
            Vector3 spawnPosition = new Vector3(Random.Range(-24.0f, 24.0f), 7.0f, Random.Range(-35.0f, 35.0f));

            GameObject snow = (GameObject)Instantiate(snowPrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(snow);

            snowRespawn = 20.0f;
        }
    }
}