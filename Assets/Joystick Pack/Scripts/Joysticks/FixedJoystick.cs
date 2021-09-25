using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    public bool isTouching;
    public bool pointerUp, pointerDown;

    public float totalValue;

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
            yield return new WaitForSeconds(.1f);
            pointerUp = false;
        }
    }

    private void Update()
    {
        if (isTouching)
        {
            totalValue = Mathf.Abs(Vertical) + Mathf.Abs(Horizontal);
        }
    }
}