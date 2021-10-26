using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 _pos;
    [SerializeField] private Transform _player;

    void Update()
    {
        transform.position = _player.position + _pos;
    }
}
