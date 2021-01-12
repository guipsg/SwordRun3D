using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleActivateObject : MonoBehaviour
{
    public GameObject activateObject;

    public void ActivateObject()
    {
        activateObject.SetActive(true);
    }
    public void DeactivateObject()
    {
        activateObject.SetActive(false);
    }
}
