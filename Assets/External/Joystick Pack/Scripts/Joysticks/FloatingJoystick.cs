using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    public event Action OnTouch;
    public event Action OnTouchEnded;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
        OnTouch?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
        OnTouchEnded?.Invoke();
    }
}