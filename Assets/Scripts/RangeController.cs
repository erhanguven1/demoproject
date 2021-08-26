using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if ((other.GetComponentInParent<Unit>() || other.GetComponent<Unit>()) && other.transform.parent != transform.parent)
        {
            GetComponent<MeshRenderer>().material.color = Color.red - Color.black * .5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Unit>() || other.GetComponent<Unit>())
        {
            GetComponent<MeshRenderer>().material.color = Color.green - Color.black * .5f;
        }
    }
}
