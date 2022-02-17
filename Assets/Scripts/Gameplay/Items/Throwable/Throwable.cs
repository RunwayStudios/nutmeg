using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Throwable : MonoBehaviour
{
    [SerializeField] private float throwOffset;
    [SerializeField] private float throwSpeed;
    [SerializeField] private GameObject impactFX;
    [SerializeField] private WeaponStats stats;
    
    private Vector3 origin;
    private Vector3 target;
    private float elapsedTime;

    void Update()
    {
        Vector3 centerPivot = (origin + target) * 0.5f;

        centerPivot -= new Vector3(0,  throwOffset, 0);

        Vector3 startRelativeCenter = origin - centerPivot;
        Vector3 endRelativeCenter = target - centerPivot;
        
        transform.position = Vector3.Slerp(startRelativeCenter, endRelativeCenter,
            elapsedTime += Time.deltaTime / Vector3.Distance(origin, target) * throwSpeed);
        transform.position += centerPivot;
    }

    public void Start()
    {
        target =
            MouseController.ShootRayFromCameraToMouse(LayerMask.GetMask("MouseTargetable"))?.point ??
            MouseController.GetLastMouseLookTargetPoint();

        origin = transform.position;
    }

    private void Damage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, stats.range);

        foreach (Collider collider in colliders)
        {
            if(collider.CompareTag("DamageReceiver") && collider.TryGetComponent(typeof(DamageReceiver), out Component c))
                c.GetComponent<DamageReceiver>().ReceiveDamage(stats.damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(impactFX, transform.position,  Quaternion.Euler(Vector3.left * 90));
        Damage();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, stats.range);
    }
}