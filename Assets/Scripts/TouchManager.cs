//Created by Erhan Güven at 25.09.2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TouchState { Touching, NotTouching }
public class TouchManager : MonoBehaviour
{
    public static TouchManager instance;
    private void Awake()
    {
        instance = this;
    }

    private TouchState state;
    private Vector2 touchDelta;
    public TouchState GetState()
    {
        return state;
    }

    private bool startedFromUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    state = TouchState.NotTouching;
                    startedFromUI = true;
                    return;
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                startedFromUI = false;
                state = TouchState.NotTouching;
                return;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && !startedFromUI)
            {
                state = TouchState.Touching;
                touchDelta = Input.GetTouch(0).deltaPosition;
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                state = TouchState.NotTouching;
                startedFromUI = true;
                return;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            startedFromUI = false;
            state = TouchState.NotTouching;
            return;
        }
        if (Input.GetMouseButton(0) && !startedFromUI)
        {
            state = TouchState.Touching;
            touchDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
#endif
    }

    public Vector2 GetTouchDelta()
    {
        return touchDelta;
    }
}
