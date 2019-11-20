using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CameraVolumeArea : MonoBehaviour
{
    public CinemachineVirtualCamera TargetVitualCamera;

    private Collider2D m_Col2D;

    void Start()
    {
        m_Col2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && TargetVitualCamera.Priority != 20)
        {
            TargetVitualCamera.Priority = 20;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && TargetVitualCamera.Priority != 5)
        {
            TargetVitualCamera.Priority = 5;
        }
    }
}
