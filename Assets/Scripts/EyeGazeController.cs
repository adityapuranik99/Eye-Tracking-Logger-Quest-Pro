using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Utilities;
using Oculus.Interaction.GrabAPI;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class EyeGazeController : MonoBehaviour
{
    
    [SerializeField] private float rayDistance = 1.0f;

    [SerializeField] private float rayWidth = 0.01f;

    [SerializeField] private LayerMask layersToInclude;

    [SerializeField] private Color rayColorDefaultState = Color.yellow;

    [SerializeField] private Color rayColorHoverState = Color.red;

    private LineRenderer lineRenderer;

    private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();
    
    // [SerializeField] string eyeName;

    // [SerializeField] GameObject eye;
    // [SerializeField] OVREyeGaze eyeGaze;
    // public bool inVR = false;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, new Vector3 (transform.position.x, transform.position.y, transform.position.z + rayDistance));
    }

    void FixedUpdate(){
        RaycastHit hit;

        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;

        if(Physics.Raycast(transform.position, rayCastDirection, out hit, Mathf.Infinity, layersToInclude)){
            Unselect();
            lineRenderer.startColor = rayColorHoverState;
            lineRenderer.endColor = rayColorHoverState;

            var eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
            eyeInteractables.Add(eyeInteractable);
            eyeInteractable.IsHovered = true;
        }
        else{
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            Unselect();
        }
    }

    void Unselect(bool clear = false){
        foreach(var interactable in eyeInteractables){
            interactable.IsHovered = false;
        }
        if(clear) {
            eyeInteractables.Clear();
        }
    }


    // Update is called once per frame
    // void Update()
    // {
    //     //eye.transform.localRotation = Quaternion.Euler(0, Time.time * 10f, 0);
    //     if (eyeGaze == null) return; //If there's no eyeGaze, return

    //     if (!inVR) //Just to check the rotation capabilities of the eye when not in VR
    //     {
    //         eye.transform.localRotation = Quaternion.Euler(0f, Time.time * 15, 0f);
    //         return;
    //     }

    //     if (eyeGaze.EyeTrackingEnabled) //Turn the gaze of the user's eye into the gaze of the avatar's eye
    //     {
    //         eye.transform.rotation = eyeGaze.transform.rotation;
    //     }
    // }
}
