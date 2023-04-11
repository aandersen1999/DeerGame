using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public float maxCameraDistance = 1.0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * maxCameraDistance);
    }

    private void OnValidate()
    {

    }

    private void Update()
    {
        int layermask = 1 << 7;
        layermask = ~layermask;
        float dist = (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out RaycastHit hit, maxCameraDistance, layermask)) ?
            hit.distance : maxCameraDistance;

        dist -= 1;
        dist /= cam.transform.lossyScale.z;

        cam.transform.localPosition = Vector3.back * dist;
    }
}
