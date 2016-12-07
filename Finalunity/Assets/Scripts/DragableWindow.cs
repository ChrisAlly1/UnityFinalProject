using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class DragableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler {

    public Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - (Vector3)eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector3)eventData.position + offset;
    }
}
