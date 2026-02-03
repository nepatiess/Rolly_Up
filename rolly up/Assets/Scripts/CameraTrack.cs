using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float trackSmoothness = .125f;
    [SerializeField] float offSetZ;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new 
            Vector3(transform.position.x,transform.position.y, _target.position.z + offSetZ), trackSmoothness);
    }
}

