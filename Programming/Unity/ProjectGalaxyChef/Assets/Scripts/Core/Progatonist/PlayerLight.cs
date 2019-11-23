using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Transform PlayerTransform;
    public Vector3 Offset;

    void Update()
    {
        transform.position = PlayerTransform.position + Offset;
    }
}
