﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    // Start is called before the first frame update

    static HexMapCamera instance;

    static GameObject ArthusGobj;

    public static bool Locked
    {
        set
        {
            instance.enabled = !value;
        }
    }

    Transform swivel, stick;

    float zoom = 1f;

    public Camera cmr;

    public float stickMinZoom, stickMaxZoom;    //视角缩放限度

    public float swivelMinZoom, swivelMaxZoom;  //视角旋转限度

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotationSpeed;

    float rotationAngle;

    public HexGrid grid;

    private void Awake()
    {
        instance = this;
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
        cmr = stick.GetChild(0).GetComponent<Camera>();


    }

    private void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if(zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");
        if(rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if(xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            AdjustZoom(difference * 0.01f);
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            float speed = 0.1F;
            transform.Translate(-touchDeltaPosition.x * speed, 0, -touchDeltaPosition.y * speed);
        }
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if(rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    void AdjustZoom (float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angel = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angel, 0f, 0f);
    }

    void AdjustPosition (float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition (Vector3 position)
    {
        float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }

    public static void ValidatePosition()
    {
        instance.AdjustPosition(0f, 0f);
    }

    public static void moveToArthus()
    {
        ArthusGobj = GameObject.FindGameObjectWithTag("2234");
        instance.gameObject.transform.position = new Vector3(ArthusGobj.transform.position.x - 20, 0f, ArthusGobj.transform.position.z);
    }

    public static void moveToTarget(GameObject target)
    {
        instance.gameObject.transform.position = new Vector3(target.transform.position.x - 20, 0f, target.transform.position.z);
    }
}
