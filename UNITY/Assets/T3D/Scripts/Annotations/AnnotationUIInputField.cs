using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnnotationUIInputField : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private AnnotationUI ui;
    [SerializeField]
    private IntEvent reselectAnnotationEvent;

    public void OnSelect(BaseEventData eventData)
    {
        print("selecting2: " + eventData.selectedObject);
        reselectAnnotationEvent.Invoke(ui.Id);
    }
}
