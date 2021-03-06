using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public ISelectable selected;

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && Input.GetMouseButtonDown(0))
        {
            if (hit.collider.GetComponentInParent<ISelectable>() != null)
            {
                if (selected != null)
                {
                    //If we changed the selected by clicking on another selectable object, deselect it and select new one
                    if (selected != hit.collider.GetComponentInParent<ISelectable>())
                    {
                        Deselect();
                        Select(hit.collider.GetComponentInParent<ISelectable>());
                    }
                    else
                    {
                        Select(hit.collider.GetComponentInParent<ISelectable>());
                    }
                }
                else
                {
                    Select(hit.collider.GetComponentInParent<ISelectable>());
                }
            }
            else
            {
                if (selected != null)
                {
                    Deselect();
                }
            }
        }
    }

    void Select(ISelectable _selectable)
    {
        if (_selectable == selected) return;

        selected = _selectable;
        selected.Select();
    }

    void Deselect()
    {
        selected.Deselect();
        selected = null;
    }
}
