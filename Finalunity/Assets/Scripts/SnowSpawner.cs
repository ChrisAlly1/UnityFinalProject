using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SnowSpawner : NetworkBehaviour {

    public GameObject snowPrefab;
    public int numberOfSnow;
    public float snowRespawn = 10.0f;
    public override void OnStartServer()
    {
        for (int i = 0; i < numberOfSnow; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-32.0f, 32.0f), 7.0f, Random.Range(-32.0f, 32.0f));

            Quaternion spawnRotation = Quaternion.Euler(90.0f, 90.0f, -90.0f);

            GameObject fire = (GameObject)Instantiate(snowPrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(fire);
        }
    }
    void Update()
    {
        snowRespawn -= Time.deltaTime;
        if (snowRespawn <= 0)
        {
            for (int i = 0; i < numberOfSnow; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-32.0f, 32.0f), 7.0f, Random.Range(-32.0f, 32.0f));

                Quaternion spawnRotation = Quaternion.Euler(90.0f, 90.0f, -90.0f);

                GameObject snow = (GameObject)Instantiate(snowPrefab, spawnPosition, spawnRotation);

                NetworkServer.Spawn(snow);
                snowRespawn = 10.0f;

            }
        }
    }
}

