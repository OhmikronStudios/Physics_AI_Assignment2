using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBot : MonoBehaviour
{
    //Patrol and chase targets
    [SerializeField] Graph.Node target;
    Graph route;

    //Unit stats
    [SerializeField] float patrolSpeed = 10.0f;

    //Unique spawn factors
    Color color;
    [SerializeField] GameObject body;
    [SerializeField] GameObject head;

    //Observer & Object Pool link
    GameManager gm;



    // Start is called before the first frame update
    void Start()
    {

        color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), 100);
        body.GetComponent<MeshRenderer>().material.color = color;
        head.GetComponent<MeshRenderer>().material.color = color;

        //Constructs it's patrol route randomly between all existing waypoints 
        List<GameObject> waypointList = new List<GameObject>();
        waypointList.AddRange(GameObject.FindGameObjectsWithTag("Waypoint"));
        route = new Graph(waypointList);

        //Starts with a randomly selected node in the patrol route
        target = route.RandomTarget();
    }

    // Update is called once per frame
    void Update()
    {

            transform.LookAt(target.m_position);
            transform.position = Vector3.MoveTowards(transform.position, target.m_position, patrolSpeed * Time.deltaTime);
            float distToTarget = Vector3.Distance(transform.position, target.m_position);
            if (distToTarget < 1.0f)
            {
                target = target.m_nextNode;
            }
        

        //SceneMode allows you to view each patrolbot's route in it's own specific colour
        foreach (Graph.Node node in route.GetRoute())
        {
            Debug.DrawLine(node.m_position, node.m_nextNode.m_position, color);
        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Projectile")
    //    {
    //        health -= 1;
    //        gm.Notify();

    //        if (health <= 0)
    //        {
    //            gm.NotifyDead(gameObject);
    //            //gameObject.SetActive(false);
    //            //Destroy(gameObject);
    //            //alive = false;
    //        }
    //    }
    //}

   
}
