using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControlResolucion : MonoBehaviour
{
public float aspect;
public float rounded;
UnityEngine.UI.CanvasScaler ccv;
    // Start is called before the first frame update
    void Start()
    {
        ccv= this.GetComponent <UnityEngine.UI.CanvasScaler>();
        UpdateRes();
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    
    void UpdateRes()
    {
	       
    aspect = Camera.main.aspect;
    rounded = (float)System.Math.Round(aspect, 2);
    UnityEngine.Debug.Log("Aspect: " + rounded); 
    
    // Diferentes aspects
    if(rounded == 1.65f || rounded == 1.66f || rounded ==1.57)
    	Addratios (0,5.34f);
    else if (rounded == 2.04f || rounded == 2.05f || rounded ==2.06f)
    	Addratios(0.88f,4.86f);
    else if (rounded == 1.70f || rounded == 1.71f || rounded ==1.69f)
    	Addratios(0,5.21f);
    else if (rounded == 1.33f || rounded == 1.32f || rounded ==1.34f)
    	Addratios(0,6.77f);
    else if (rounded == 1.85f)
    	Addratios(1,5);
    else if (rounded == 2.11f)
    	Addratios(1,4.5f);
    else if (rounded == 1.78f)
    {
	    Addratios(0, 4.5f);
	    UnityEngine.Debug.Log("Modificado");
    }
    else
    	Addratios(0,5);
    }
    
    
    void Addratios(float canvasmatch, float camerasize){
	    ccv.matchWidthOrHeight = canvasmatch;
	    Camera.main.orthographicSize = camerasize;
    }
}
