using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class FireSpawner : NetworkBehaviour
{

    public GameObject firePrefab;
    public int numberOfFires;
    public float fireRespawn = 10.0f;
    public override void OnStartServer()
    {
        for (int i = 0; i < numberOfFires; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-32.0f, 32.0f), 1.2f, Random.Range(-32.0f, 32.0f));

            Quaternion spawnRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

            GameObject fire = (GameObject)Instantiate(firePrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(fire);           
        }
    }
    void Update()
    {
        fireRespawn -= Time.deltaTime;
        if (fireRespawn <= 0)
        {
            for (int i = 0; i < numberOfFires; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-32.0f, 32.0f), 1.2f, Random.Range(-32.0f, 32.0f));

                Quaternion spawnRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

                GameObject fire = (GameObject)Instantiate(firePrefab, spawnPosition, spawnRotation);

                NetworkServer.Spawn(fire);
                fireRespawn = 10.0f;
             
            }
        }
    }
}

