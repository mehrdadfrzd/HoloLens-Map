using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HandDragging : MonoBehaviour, IManipulationHandler
{

    public Text textHolder;

    [SerializeField]
    float DragSpeed = 1.5f;

    [SerializeField]
    float DragScale = 1.5f;

    [SerializeField]
    float MaxDragDistance = 3f;
        
    Vector3 lastPosition;

    [SerializeField]
    bool draggingEnabled = true;
    public void SetDragging(bool enabled)
    {
        draggingEnabled = enabled;
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
        lastPosition = transform.position;
         textHolder.text = string.Format("Position:{0:0.00},{1:0.00},{2:0.00}\n Velocity: -,-,-", transform.position.x, transform.position.y, transform.position.z);
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (draggingEnabled)
        {         
            Drag(eventData.CumulativeDelta);
            Debug.LogError("Dragging update...");
            //sharing & messaging
            //SharingMessages.Instance.SendDragging(Id, eventData.CumulativeDelta);
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    void Drag(Vector3 positon)
    {
        Debug.LogError("Dragging...");
        var targetPosition = lastPosition + positon * DragScale;

        textHolder.text = string.Format("Position:{0:0.00},{1:0.00},{2:0.00}\n Velocity: -,-,-", targetPosition.x, targetPosition.y, targetPosition.z);

        if (Vector3.Distance(lastPosition, targetPosition) <= MaxDragDistance)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, DragSpeed);
        }
    }

    void Start()
    {

        textHolder = GameObject.Find("HoloLensCamera/Canvas/Text").GetComponent<Text>();
        textHolder.text = "mnp starting..";

    }
}
