using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targeting
{
   public static Transform GetClosestTarget(Transform[] targets, Vector2 point)
   {
      Transform bestTarget = null;
      float closestDistanceSqr = Mathf.Infinity;

      foreach (Transform potentialTarget in targets)
      {
         Vector2 directionToTarget = (Vector2)potentialTarget.position - point;
         float dSqrToTarget = directionToTarget.sqrMagnitude;

         if (dSqrToTarget < closestDistanceSqr)
         {
            closestDistanceSqr = dSqrToTarget;
            bestTarget = potentialTarget;
         }
      }

      return bestTarget;
   }
}


//
// public abstract class Targeting
// {
//    public static Vector2 GetClosestTarget(Collider2D[] targets, Vector2 point)
//    {
//       Vector2 bestTarget = point;
//       float closestDistanceSqr = Mathf.Infinity;
//
//       foreach (Collider2D potentialTarget in targets)
//       {
//          Vector2 directionToTarget = (Vector2)potentialTarget.transform.position - point;
//          float dSqrToTarget = directionToTarget.sqrMagnitude;
//
//          if (dSqrToTarget < closestDistanceSqr)
//          {
//             closestDistanceSqr = dSqrToTarget;
//             bestTarget = potentialTarget.transform.position;
//          }
//       }
//       return bestTarget;
//    }
// }