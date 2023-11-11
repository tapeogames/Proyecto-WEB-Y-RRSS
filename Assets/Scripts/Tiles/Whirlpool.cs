using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Whirlpool : Tile
{
    public Transform partnerPosition;
    Vector3 posY = new Vector3(0, 1, 0);

    private Vector3 posFront = new Vector3(0,0,1);
    private Vector3 posRight = new Vector3(1, 0, 0);

    private bool IsPositionOccupied(Vector3 position)
    {
        // Realiza un raycast para verificar si hay alg�n objeto en la posici�n dada.
        RaycastHit hit;
        return Physics.Raycast(position, Vector3.up, out hit, 1f); // Puedes ajustar la longitud del raycast seg�n tus necesidades.
    }

    public void nextPos(SceneObject obj)
    {
        if (!IsPositionOccupied(partnerPosition.position - posFront))
        {
            obj.transform.position = partnerPosition.position - posFront + posY;
        } 
        else if(!IsPositionOccupied(partnerPosition.position + posRight))
        {
            obj.transform.position = partnerPosition.position + posRight + posY;
        }
        else if (!IsPositionOccupied(partnerPosition.position + posFront))
        {
            obj.transform.position = partnerPosition.position + posFront + posY;
        }
        else if (!IsPositionOccupied(partnerPosition.position - posRight))
        {
            obj.transform.position = partnerPosition.position - posRight + posY;
        }

    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("PlayerDetectorCurrent") && !PlayerController.grabbing )
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            player.enabled= false;


            if (IsPositionOccupied(partnerPosition.position - posFront) && IsPositionOccupied(partnerPosition.position + posRight) && IsPositionOccupied(partnerPosition.position + posFront) && IsPositionOccupied(partnerPosition.position - posRight))
            {
                player.enabled= true;
            }
            else
            {
                nextPos(player);
            }
        }
                
        if (other.CompareTag("ObjectDetectorCurrent") && !PlayerController.grabbing )
        {
            PushableObjectController obj = other.GetComponentInParent<PushableObjectController>();
            obj.transform.position = partnerPosition.transform.position + posY;
            nextPos(obj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerDetectorCurrent") && !PlayerController.grabbing)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            player.enabled = true;
        }
    }
}