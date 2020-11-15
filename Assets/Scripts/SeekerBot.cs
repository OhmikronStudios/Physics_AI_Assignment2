using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerBot : MonoBehaviour
{
    public AIHelper.MovementBehaviors activeMovementBehavior = AIHelper.MovementBehaviors.None;
    public GameObject targetObject;
    public GameObject seekObject;
    public float maxSpeed = 3.0f;

    public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AIHelper.InputParameters inputData = new AIHelper.InputParameters(gameObject.transform, targetObject.transform, Time.deltaTime, maxSpeed);
        AIHelper.MovementResult movementResult = new AIHelper.MovementResult();

        switch (activeMovementBehavior)
        {
            case AIHelper.MovementBehaviors.FleeKinematic:
                AIHelper.FleeKinematic(inputData, ref movementResult);
                break;
            case AIHelper.MovementBehaviors.SeekKinematic:
                AIHelper.SeekKinematic(inputData, ref movementResult);
                break;
            case AIHelper.MovementBehaviors.WanderKinematic:

                AIHelper.WanderKinematic(inputData, ref movementResult);

                break;
            default:
                //AIHelpers.SeekKinematic(inputData, ref movementResult);
                movementResult.newPosition = transform.position;
                break;
        }

        gameObject.transform.position = movementResult.newPosition;
        transform.LookAt(targetObject.transform);
    }

    public void ActivateWander()
    {
        activeMovementBehavior = AIHelper.MovementBehaviors.WanderKinematic;
    }

    public void ActivateSeek()
    {
        targetObject = seekObject;
        activeMovementBehavior = AIHelper.MovementBehaviors.SeekKinematic;
    }

    public void ActivateLeave()
    {
        activeMovementBehavior = AIHelper.MovementBehaviors.FleeKinematic;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    public bool TargetIsVisible()
    {
        Vector3 dirToTarget = (seekObject.transform.position - transform.position).normalized;
        float dstToTarget = Vector3.Distance(transform.position, seekObject.transform.position);
        return (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2 && dstToTarget < viewRadius);
    }

    public bool Attack()
    {
        //Vector3 dirToTarget = (seekObject.transform.position - transform.position).normalized;
        float dstToTarget = Vector3.Distance(transform.position, seekObject.transform.position);
        return (dstToTarget < viewRadius / 3);
       
    }




}
