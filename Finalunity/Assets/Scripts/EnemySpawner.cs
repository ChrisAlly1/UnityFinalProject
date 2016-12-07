using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    private int numberToSpawn;
    static private int currentEnemies;

    void Awake() {
        numberToSpawn = 3;
        currentEnemies = numberToSpawn;
    }

    void Update() {
        if (currentEnemies <= 0) {
            Invoke("SpawnMore", 10f);
            currentEnemies = 1;
        }
    }

    public override void OnStartServer() {
        SpawnMore();
    }

    public void SpawnMore() {
        for (int i = 0; i < numberToSpawn; i++) {
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x + Random.Range(-6.0f, 6.0f), 0.5f, spawnPoint.position.z + Random.Range(-5.0f, 5.0f));

            Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

            GameObject enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(enemy);
        }
        currentEnemies = numberToSpawn;

        numberToSpawn += 2;
    }

    public static void removeEnemy() {
        currentEnemies--;
    }
}