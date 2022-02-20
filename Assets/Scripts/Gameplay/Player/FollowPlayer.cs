using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    private Vector3 origin;

    private void Start()
    {
        origin = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(player.position.x + origin.x, origin.y, player.position.z + origin.z);
    }
}
