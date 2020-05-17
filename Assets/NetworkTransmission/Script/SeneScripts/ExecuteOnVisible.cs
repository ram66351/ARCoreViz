using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteOnVisible : MonoBehaviour
{
    public float thresholdDistance = 80;
    public bool enable = false;
    public float distance = 0;
    //public FaceCamera FaceCameraScript;
    //public BoxCollider bColl;
    public Collider[] colliders;
    public VisibilityDependantComponent[] viz_Comps;
    // Start is called before the first frame update
    void Awake()
    {
        EnableDependentComp(false);
    }

    void OnBecameVisible()
    {
        
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (distance < thresholdDistance)
        {
            enable = true;
            EnableDependentComp(true);
        }
            
       
    }
    void OnBecameInvisible()
    {
        enable = false;
        EnableDependentComp(false);
    }

    void EnableDependentComp(bool enable)
    {
        for(int i=0; i< viz_Comps.Length; i++)
        {
            viz_Comps[i].enabled = enable;
        }

        for(int i=0; i< colliders.Length; i++)
        {
            colliders[i].enabled = enable;
        }
        //FaceCameraScript.enabled = enable;
        //bColl.enabled = enable;
    }
}
