using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    public bool isTouching;
    public bool pointerUp, pointerDown;

    public override void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        isTouching = true;
        base.OnPointerDown(eventData);
        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForEndOfFrame();
            pointerDown = false;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        pointerUp = true;
        isTouching = false;
        base.OnPointerUp(eventData);
        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForEndOfFrame();
            pointerUp = false;
        }
    }
}