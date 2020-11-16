using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TurretAI))]
public class FoVEditor_Turret : Editor
{
    void OnSceneGUI()
    {
        TurretAI fov = (TurretAI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius / 3);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);
    }


}
