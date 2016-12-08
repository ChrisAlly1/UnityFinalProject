using UnityEngine;

public class Flock : MonoBehaviour {
    public float speed = 0.01f;
    float rotationSpeed = 20.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighbourDistance = 7.0f;

    bool turning = false;
	// Use this for initialization
	void Start () {
        speed = 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.tankSize) {
            turning = true;
        } else {
            turning = false;
        }

        if (turning == true) {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotationSpeed * Time.deltaTime);
            speed = 2.0f;
        } else {
            if (Random.Range(0,5) < 1)           
                ApplyRules();          
        }
        transform.Translate(0,0, Time.deltaTime * speed);
	}

    void ApplyRules() {
        GameObject[] gos = GlobalFlock.allFish;
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;
        Vector3 goalpos = GlobalFlock.goalPos;
        float dist;
        int groupsize = 0;

        for (int z = 0; z < GlobalFlock.numberOfEnemies(); z++) {
            if(gos[z] != gameObject) {
                dist = Vector3.Distance(gos[z].transform.position, transform.position);
                if (dist <= neighbourDistance) {
                    vcentre += gos[z].transform.position;
                    groupsize++;
                    if(dist<1.0f) {
                        vavoid = vavoid + (transform.position - gos[z].transform.position);
                    }
                    Flock anotherflock = gos[z].GetComponent<Flock>();
                    gSpeed = gSpeed + anotherflock.speed;
                }
            }
        }

        if (groupsize > 0) {
            vcentre = vcentre / groupsize + (goalpos - this.transform.position);
            speed = gSpeed / groupsize;
            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }
    }
}