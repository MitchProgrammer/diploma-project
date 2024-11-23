using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directonGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Draw Rotation Direction in magenta
        Gizmos.color = Color.magenta;
        Vector3 endpoint = transform.position + transform.forward * 2f;
        Gizmos.DrawLine(transform.position, endpoint);
    }
}
