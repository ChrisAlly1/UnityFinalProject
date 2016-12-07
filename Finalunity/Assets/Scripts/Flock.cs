using UnityEngine;
using System.Collections;

public class Flock : MonoBehaviour {
    public float speed = 0.01f;
    float rotationSpeed = 20.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighbourDistance = 7.0f;

    bool turning = false;
	// Use this for initialization
	void Start () {
        speed = Random.Range(0.5f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
        
        if(Vector3.Distance(transform.position,Vector3.zero) >=GlobalFlock.tankSize)
        {
           turning = true;
        }
        else        
            turning = false;

        if (turning == true)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotationSpeed * Time.deltaTime);
            speed = Random.Range(0.5f, 1);
        }
        else
        {
            if (Random.Range(0,5) < 1)           
                ApplyRules();          
        }
        transform.Translate(0,0, Time.deltaTime * speed);
       
	}
    void ApplyRules()
    {
        GameObject[] gos;
        gos = GlobalFlock.allFish;
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;
        Vector3 goalpos = GlobalFlock.goalPos;
        float dist;
        int groupsize = 0;
        foreach(GameObject go in gos)
        {
            if(go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupsize++;
                    if(dist<1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }
                    Flock anotherflock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherflock.speed;
                }
            }
        }
        if (groupsize > 0)
        {
            vcentre = vcentre / groupsize + (goalpos - this.transform.position);
            speed = gSpeed / groupsize;
            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

        }
    }
}
