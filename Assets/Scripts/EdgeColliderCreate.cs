using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
public class EdgeColliderCreate : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private int resolution;
    void Start()
    {
        Spline mySpline = splineContainer.Spline;
        List<Vector2> localPointPositions = new List<Vector2>();

        for (int pointForEdgeCollider = 0 ; pointForEdgeCollider < resolution ; pointForEdgeCollider++)
        {
            Vector3 worldPositionOfPointOnSpline = splineContainer.EvaluatePosition((float)pointForEdgeCollider / resolution);
            
            //Debug.DrawRay(worldPositionOfPointOnSpline, Vector2.down, Color.blue , 70f);
            Debug.Log("World " + worldPositionOfPointOnSpline);
            
            Vector2 localPositionOfPointOnSpline = transform.InverseTransformPoint(worldPositionOfPointOnSpline);
            Debug.Log("Local " + localPositionOfPointOnSpline);

            Debug.DrawRay(transform.position, localPositionOfPointOnSpline, Color.green , 70f);

            localPointPositions.Add(localPositionOfPointOnSpline);
        }

        edgeCollider.points = localPointPositions.ToArray();
    }
}
