using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private GameObject cursor;
    [SerializeField] private float rayDistance = 1.0f;

    [SerializeField] private float rayWidth = 0.01f;

    [SerializeField] private LayerMask layersToInclude;

    [SerializeField] private Color rayColorDefaultState = new Color(1f, 1f, 0f, 1.0f);

    [SerializeField] private Color rayColorHoverState = new Color(1f, 1f, 0f, 1.0f);

    private LineRenderer lineRenderer;

    private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();

    public UnityEvent<GameObject> OnHoverObjectChanged = new UnityEvent<GameObject>();

    private string filePath;

    private void OnAwake(){
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        mainCamera = Camera.main.gameObject.transform;
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = Application.persistentDataPath + "/EyeTrackingData_" + timestamp + ".csv";
        string header = "Timestamp,Frame#(Framerate "+Application.targetFrameRate+"),ObjectName,WorldPosX,WorldPosY,WorldPosZ,ScreenPosX,ScreenPosY,CamPosX,CamPosY,CamPosZ,CamRotX,CamRotY,CamRotZ\n";
        File.WriteAllText(filePath, header); // Header row
        Debug.Log("Data will be saved to: " + filePath);
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

    void Update(){

        RaycastHit hit;

        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;

        if(Physics.Raycast(transform.position, rayCastDirection, out hit, Mathf.Infinity, layersToInclude)){
            Unselect();
            lineRenderer.startColor = rayColorHoverState;
            lineRenderer.endColor = rayColorHoverState;

            var eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
            eyeInteractables.Add(eyeInteractable);
            eyeInteractable.IsHovered = true;
            cursor.transform.position = hit.point;
            LogPositionAndTime(cursor.transform);
        }
        else{
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            Unselect();
            //LogPositionAndTime(cursor.transform);
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

    void LogPositionAndTime(Transform hitTransform)
    {
        string objectName = hitTransform.gameObject.name;
        Vector3 worldPosition = hitTransform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Vector3 camPosition = mainCamera.position;
        Vector3 camRotation = mainCamera.rotation.eulerAngles;

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine($"{timestamp},{Time.frameCount},{objectName},{worldPosition.x},{worldPosition.y},{worldPosition.z},{screenPosition.x},{screenPosition.y},{camPosition.x},{camPosition.y},{camPosition.z},{camRotation.x},{camRotation.y},{camRotation.z}");
        }
    }
}
