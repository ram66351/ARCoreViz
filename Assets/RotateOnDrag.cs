using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnDrag : MonoBehaviour
{
    float speed = 0.005f;
    float friction = 0.5f;
    float lerpSpeed = 1.5f;
    float xDeg;
    float yDeg;
    Quaternion fromRotation;
    Quaternion toRotation;

    public bool isTouched = false;
    void Start()
    {

    }

    void Update()
    {
        if (isTouched)
        {

            xDeg -= Mathf.Clamp(Input.GetAxis("Mouse X"), 0, 2) * speed ;
            yDeg += Mathf.Clamp(Input.GetAxis("Mouse Y"), 0, 2) * speed ;
            fromRotation = transform.rotation;
            toRotation = Quaternion.Euler(yDeg, xDeg, 0);
            Vector3 rotation = new Vector3(yDeg, xDeg, 0);
            transform.Rotate(rotation, Space.World);
            //transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, Time.deltaTime * lerpSpeed);
            Debug.Log("== " + toRotation);
        }
    }

    void OnMouseDown()
    {
        isTouched = true;
    }

    void OnMouseUp()
    {
        isTouched = false;
    }

    //private float _sensitivity;
    //private Vector3 _mouseReference;
    //private Vector3 _mouseOffset;
    //private Vector3 _rotation;
    //private bool _isRotating;

    //void Start()
    //{
    //    _sensitivity = 0.4f;
    //    _rotation = Vector3.zero;
    //}

    //void Update()
    //{
    //    if (_isRotating)
    //    {
    //        // offset
    //        _mouseOffset = (Input.mousePosition - _mouseReference);

    //        // apply rotation
    //        _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

    //        // rotate
    //        transform.Rotate(_rotation, Space.World);

    //        // store mouse
    //        _mouseReference = Input.mousePosition;
    //    }

    //    //if (_isRotating)
    //    //{
    //    //    _mouseOffset = (Input.mousePosition - _mouseReference);
    //    //    // apply rotation
    //    //    //_rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;
    //    //    _rotation.y = -(_mouseOffset.x) * _sensitivity;
    //    //    _rotation.x = -(_mouseOffset.y) * _sensitivity;
    //    //    // rotate
    //    //    //transform.Rotate(_rotation);
    //    //    transform.eulerAngles += _rotation;
    //    //    // store mouse
    //    //    _mouseReference = Input.mousePosition;
    //    //}
    //}

    //void OnMouseDown()
    //{
    //    // rotating flag
    //    _isRotating = true;

    //    // store mouse
    //    _mouseReference = Input.mousePosition;
    //}

    //void OnMouseUp()
    //{
    //    // rotating flag
    //    _isRotating = false;
    //}

}


