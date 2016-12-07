using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class GlobalFlock : NetworkBehaviour {

    public GameObject objPrefab;
    public static int tankSize = 20;
    static int numFish = 30;
    public static GameObject[] allFish = new GameObject[numFish];
    public GameObject player;

    public static Vector3 goalPos = Vector3.zero;

	// Use this for initialization
	void Start () {
	  for(int i = 0; i < numFish;i++)
        {
            Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize),
                                      Random.Range(-tankSize, tankSize),
                                      0);
            allFish[i] = (GameObject)Instantiate(objPrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(allFish[i]);               
        }
	}
	
	// Update is called once per frame
	void Update () {
        player = GameObject.FindGameObjectWithTag("Player");
	if (Input.GetKeyDown("q"))
        {
            for (int i = 0; i < numFish; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize),
                                          Random.Range(-tankSize, tankSize),
                                          0);
                allFish[i] = (GameObject)Instantiate(objPrefab, pos, Quaternion.identity);

            }
        }
    
        goalPos = player.transform.position;
        /*   if (Random.Range(0,10000) < 50)
           {
               goalPos = new Vector3(Random.Range(-tankSize, tankSize),
                                         Random.Range(-tankSize, tankSize),
                                         0);
               player.transform.position = goalPos;
           }*/
    }
}
