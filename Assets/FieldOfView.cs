using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    //FieldOfView
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public GameObject target;


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public bool FindVisibleTargets()
    {
        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
        float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

            if(Vector3.Angle(transform.forward, dirToTarget)<viewAngle/2 && dstToTarget < viewRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
    }
}
