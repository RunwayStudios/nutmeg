using System;
using System.Collections;
using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Player;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;

public class RigController : MonoBehaviour
{
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float aimTargetHeight;

    private void Update()
    {
        UpdateAimTarget();
    }

    private void UpdateAimTarget()
    {
        Vector3 targetPoint = MouseController.ShootRayFromCameraToMouse(~0)?.point ??
                              MouseController.GetLastMouseLookTargetPoint();
        aimTarget.position = new Vector3(targetPoint.x, aimTargetHeight, targetPoint.z);
    }
}