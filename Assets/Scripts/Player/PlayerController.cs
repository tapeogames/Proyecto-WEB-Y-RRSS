﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : SceneObject
{
    private GameObject playerAvatar;


    private Vector3 zPos = new Vector3(0.00f , 0.00f, 1.00f);
    private Vector3 zNeg = new Vector3(0.00f, 0.00f, -1.00f);
    private Vector3 xPos = new Vector3(1.00f, 0.00f, 0.00f);
    private Vector3 xNeg = new Vector3(-1.00f, 0.00f, 0.00f);

    public PushableObjectController pushObject;
    private SphereCollider forward;

    public static bool grabbing;
    public static bool canGrab;

    public Vector3 playerLookAt;
    public float speed;
    public float rotationSpeed;
    public bool moving;
    public bool rotating;
    public bool rotatingNormal;
    public Vector3 movingDirection;
    public Vector3 targetPosition;
    public float distanceToTarget;

    Quaternion rotationTarget;


    void Start()
    {
        playerAvatar = sceneObject;
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        if (moving)
        {
            PerformMovement();
        }
        if (rotating && !grabbing)
        {
            RotateTowardsDirection();
        }
        if(!moving && !rotating && rotatingNormal)
        {
            NormalRotateTowardsDirection();
        }
        
    }


    void ReadInput() {

        
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D");
            if (!grabbing && leftTile != null && leftTile.activate)
            {
                MoveToPosition(leftTile.GetTilePosition());
            } 
            else if (grabbing && leftTile.activate && pushObject.leftTile != null)
            {
                Debug.Log(xNeg + " " + xPos);
                Debug.Log(playerLookAt);
                if (Vector3.Distance(playerLookAt, xNeg) < 0.05f || Vector3.Distance(playerLookAt, xPos) < 0.05f)
                {
                    Debug.Log("entra en el if");
                    MoveToPosition(leftTile.GetTilePosition());
                }
            }
            else if (leftTile==null || !leftTile.activate)
            {
                StartRotation(xNeg);
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A");
            if (!grabbing && rightTile != null && rightTile.activate)
            {
                MoveToPosition(rightTile.GetTilePosition());
            }
            else if (grabbing && rightTile.activate && pushObject.rightTile != null)
            {
                Debug.Log(xNeg + " " + xPos);
                Debug.Log(playerLookAt);
                if (Vector3.Distance(playerLookAt, xNeg) < 0.05f || Vector3.Distance(playerLookAt, xPos) < 0.05f)
                {
                    MoveToPosition(rightTile.GetTilePosition());
                }
            }
            else if (rightTile == null || !rightTile.activate)
            {
                //Debug.Log("ha entrado rigth");
                StartRotation(xPos);

            }
        }



        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S");
            if (!grabbing && forwardTile != null && forwardTile.activate)
            {
                MoveToPosition(forwardTile.GetTilePosition());
            }
            else if (grabbing && forwardTile.activate && pushObject.forwardTile != null)
            {
                Debug.Log(zNeg + " " + zPos);
                Debug.Log(playerLookAt);
                if (Vector3.Distance(playerLookAt, zNeg) < 0.05f || Vector3.Distance(playerLookAt, zPos) < 0.05f)
                {
                    MoveToPosition(forwardTile.GetTilePosition());
                }
            }
            else if ( forwardTile == null || !forwardTile.activate)
            {
                //Debug.Log("ha entrado forward");
                StartRotation(zPos);

            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W");
            if (!grabbing && backwardTile != null && backwardTile.activate)
            {
                MoveToPosition(backwardTile.GetTilePosition());
            }
            else if (grabbing && backwardTile.activate && pushObject.backwardTile != null)
            {
                Debug.Log(zNeg + " " + zPos);
                Debug.Log(playerLookAt);
                if (Vector3.Distance(playerLookAt, zNeg) < 0.05f || Vector3.Distance(playerLookAt, zPos) < 0.05f)
                {
                    MoveToPosition(backwardTile.GetTilePosition());
                }
            }
            else if (backwardTile == null || !backwardTile.activate)
            {
                //Debug.Log("ha entrado backward");
                StartRotation(zNeg);

            }
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE");
            if (canGrab)
            {
                if (!pushObject.isGrabbed)
                {
                    playerLookAt = playerAvatar.transform.forward;
                    playerLookAt.Normalize();
                    Debug.Log(playerLookAt);
                    pushObject.GrabObject();
                    grabbing = true;
                    pushObject.currentTile.activate = true;
                }
                else
                {
                    grabbing = false;
                    pushObject.ReleaseObject();
                    pushObject.currentTile.activate = false;

                }
            }
            
        }
    }



    void PerformMovement()
    {
        float currentDistance = (targetPosition - transform.position).magnitude;

        if (distanceToTarget < currentDistance)
        {
            moving = false;
            transform.position = targetPosition;
        }
        else
        {
            distanceToTarget = currentDistance;
            transform.position += movingDirection * speed * Time.deltaTime;
        }
    }

    void RotateTowardsDirection()
    {
        movingDirection.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(movingDirection);
        playerAvatar.transform.rotation = Quaternion.Slerp(playerAvatar.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        float angleDifference = Quaternion.Angle(playerAvatar.transform.rotation, targetRotation);
        if (angleDifference < 0.1f)
        {
            rotating = false;
        }

    }


    void StartRotation(Vector3 v)
    {
        Quaternion targetRotation = Quaternion.LookRotation(v);
        rotationTarget = targetRotation;
        rotatingNormal = true;
    }

    void NormalRotateTowardsDirection()
    {
        float step = rotationSpeed * Time.deltaTime;
        playerAvatar.transform.rotation = Quaternion.Slerp(playerAvatar.transform.rotation, rotationTarget, step);
        float angleDifference = Quaternion.Angle(playerAvatar.transform.rotation, rotationTarget);

        if (angleDifference < 0.1f)
        {
            rotatingNormal = false;
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        targetPosition = position;
        movingDirection = (position - transform.position);
        distanceToTarget = movingDirection.magnitude;
        movingDirection = movingDirection.normalized;
        rotating = true;
        moving = true;
        
    }



}
