using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public ArrayList nodeList = new ArrayList();

    public class Node
    {
        public int m_ID;
        public Node m_nextNode;
        public Vector3 m_position;

        public Node(int id, Vector3 position)
        {
            m_ID = id;
            m_position = position;
        }
    }

    public Graph(List<GameObject> waypoints)
    {
        int identity = 0;
        foreach (GameObject waypoint in waypoints)
        {
            nodeList.Add(new Node(identity, waypoint.transform.position));
            identity++;
        }

        foreach (Node node in nodeList)
        {
            int randomNumber = Random.Range(1, 3);
            node.m_nextNode = (Node)nodeList[(node.m_ID + randomNumber) % nodeList.Count];
            Debug.Log(node.m_ID + " connected to " + node.m_nextNode.m_ID + " Whose coordinates are " + node.m_position.ToString());
        }
    }

    public Node RandomTarget()
    {
        return (Node)nodeList[(Random.Range(0, nodeList.Count))];
    }

    public ArrayList GetRoute()
    {
        return nodeList;
    }

}
