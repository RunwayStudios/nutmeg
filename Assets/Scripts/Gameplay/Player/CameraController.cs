using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float arcOffset;
    [SerializeField] private Vector3 lowerPosition;
    [SerializeField] private Vector3 upperPosition;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private InputEventChannel input;

    [SerializeField] private CinemachineVirtualCamera cvc;
    
    private float scrollVal;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        input.onScrollActionPerformed += OnScrollActionPerformed;
    }

    private void UpdateDollyPosition()
    {
        
    }

    private void OnScrollActionPerformed(Vector2 value)
    {
        Debug.Log(value);
    }

    private void UpdateArcPosition()
    {
        Vector3 centerPivot = (lowerPosition + upperPosition) * 0.5f;

        centerPivot -= new Vector3(0, arcOffset, 0);

        Vector3 startRelativeCenter = lowerPosition - centerPivot;
        Vector3 endRelativeCenter = upperPosition - centerPivot;

        transform.position = Vector3.Slerp(startRelativeCenter, endRelativeCenter,
            scrollVal += scrollSpeed);
        transform.position += centerPivot;
    }

    // Update is called once per frame
    void Update()
    {
    }
}