using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{

    private Vector3 dist;
    private Vector2 pos;
    private float initialFingersDistance;
    private Vector3 initialScale;
    public float MIN = 0.5f;
    public float MAX = 1.5f;

    public bool isMouseDown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if ( isMouseDown && transform.localScale.x > MIN && transform.localScale.x < MAX)
        {

            int fingersOnScreen = 0;

            foreach (Touch touch in Input.touches)
            {
                fingersOnScreen++; //Count fingers (or rather touches) on screen as you iterate through all screen touches.

                //You need two fingers on screen to pinch.
                if (fingersOnScreen == 2)
                {

                    //First set the initial distance between fingers so you can compare.
                    if (touch.phase == TouchPhase.Began)
                    {
                        initialFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        initialScale = transform.localScale;
                    }
                    else
                    {
                        float currentFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

                        float scaleFactor = currentFingersDistance / initialFingersDistance;

                        float value = (initialScale * scaleFactor).x;

                        if (value > MIN && value < MAX)
                        {
                            transform.localScale = initialScale * scaleFactor;
                        }
                    }
                }
            }
        }
    }


    void OnMouseDown()
    {
        //GenericScaling.ScaleTransform = this.transform;

        dist = Camera.main.WorldToScreenPoint(transform.position);
        pos = new Vector2(Input.mousePosition.x - dist.x, Input.mousePosition.y - dist.y);

        isMouseDown = true;
    }

    void OnMouseUp()
    {
        isMouseDown = false;
    }

    void OnMouseDrag()
    {
        Vector3 curPos =
            new Vector3(Input.mousePosition.x - pos.x,
                Input.mousePosition.y - pos.y, dist.z);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);
        transform.position = worldPos;

    }
}
