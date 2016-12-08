using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GlobalFlock : NetworkBehaviour {
    public GameObject objPrefab;
    public static int tankSize = 20;
    static int numFish = 30;
    public static GameObject[] allFish = new GameObject[999];
    private GameObject[] player;
    private GameObject target;

    public static Vector3 goalPos = Vector3.zero;

    public Transform spawnPoint;
    private int numberToSpawn;
    static private int currentEnemies;

    void Awake() {
        numberToSpawn = 3;
        currentEnemies = numberToSpawn;
    }

    public override void OnStartServer() {
        SpawnMore();
    }

    public void SpawnMore() {
        for (int i = 0; i < numberToSpawn; i++) {
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x + Random.Range(-6.0f, 6.0f), 0.5f, spawnPoint.position.z + Random.Range(-5.0f, 5.0f));

            Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

            allFish[i] = (GameObject)Instantiate(objPrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(allFish[i]);
        }
        currentEnemies = numberToSpawn;

        numberToSpawn += 2;
    }

    public static void removeEnemy() {
        currentEnemies--;
    }
	
	// Update is called once per frame
	void Update () {
        if (currentEnemies <= 0) {
            Invoke("SpawnMore", 10f);
            currentEnemies = 1;
        }

        player = GameObject.FindGameObjectsWithTag("Player");

        float distance = 999999;
        float dist;
        for (int x = 0; x < player.Length; x++) {
            dist = Vector3.Distance(player[x].transform.position, transform.position);
            if (dist < distance) {
                distance = dist;
                target = player[x];
            }
        }

        if (target != null) {
            goalPos = target.transform.position;
        }
    }

    public static int numberOfEnemies() {
        return currentEnemies;
    }
}