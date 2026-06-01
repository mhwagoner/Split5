using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RunePoint : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public Action<RunePoint> onPointerEnter;
    public Action<RunePoint> onPointerClick;
    public Vector2Int pointPosition;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerClick?.Invoke(this);
    }
}