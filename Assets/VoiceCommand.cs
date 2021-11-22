using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCommand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateColor(string[] values)
    {
        Debug.Log(values[0]);
        /*var colorString = values[0];
        var shapeString = values[1];

        if (!ColorUtility.TryParseHtmlString(colorString, out var color)) return;
        if (string.IsNullOrEmpty(shapeString)) return;

        foreach (Transform child in transform) // iterate through all children of the gameObject.
        {
            if (child.name.IndexOf(shapeString, StringComparison.OrdinalIgnoreCase) != -1) // if the name exists
            {
                SetColor(child, color);
                return;
            }
        }*/
    }
}
