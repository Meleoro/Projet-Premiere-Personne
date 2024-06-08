using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoBouleBoulier : MonoBehaviour
{
   public int index;
   public bool isIn;
   public Material materialOn;
   public Material materialOff;
   public ProtoBoulier master;
   public float moveDuration;
   public bool canMove;
   public enum Directions
   {
      Up,
      Down,
      Left,
      Right,
   }
   public Directions directionToMove;
   
   private MeshRenderer mr;
   private Vector3 outPosition;
   private void Start()
   {
      mr = GetComponent<MeshRenderer>();
      outPosition = transform.position;
   }

   private void OnMouseEnter()
   {
      mr.material = materialOn;
   }
   
   private void OnMouseDown()
   {
      if (canMove)
      {
         if (!isIn)
         {
            StartCoroutine(WaitAndCanMove());
            StartCoroutine(MoveBouleIn());
            master.currentBool[index] = true;
            isIn = true;
         }
         else
         {
            StartCoroutine(WaitAndCanMove());
            StartCoroutine(MoveBouleOut());
            master.currentBool[index] = false;
            isIn = false;
         }
         master.CheckIfGood();
         AudioManager.Instance.PlaySoundOneShot(2,8,0);
      }
   }

   public IEnumerator MoveBouleIn()
   {
      float timer = 0;
      while (timer < moveDuration)
      {
         timer += Time.deltaTime;

         if(directionToMove == Directions.Left)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,0,-0.25f), timer / moveDuration);
         
         if(directionToMove == Directions.Right)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,0,0.25f), timer / moveDuration);
         
         if(directionToMove == Directions.Up)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,0.25f,0), timer / moveDuration);
         
         if(directionToMove == Directions.Down)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,-0.25f,0), timer / moveDuration);
         yield return null;
      }
      
   }
   
   public IEnumerator MoveBouleOut()
   {
      float timer = 0;
      while (timer < moveDuration)
      {
         timer += Time.deltaTime;

         if(directionToMove == Directions.Left)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,0,0.25f), timer / moveDuration);
         
         if(directionToMove == Directions.Right)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,0,-0.25f), timer / moveDuration);
         
         if(directionToMove == Directions.Up)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,-0.25f,0), timer / moveDuration);
         
         if(directionToMove == Directions.Down)
            transform.position = Vector3.Lerp(outPosition, outPosition + new Vector3(0,0.25f,0), timer / moveDuration);

         yield return null;
      }
   }

   IEnumerator WaitAndCanMove()
   {
      canMove = false;
      yield return new WaitForSeconds(moveDuration);
      canMove = true;
      outPosition = transform.position;
   }
   
   private void OnMouseExit()
   {
      mr.material = materialOff;
   }
}
