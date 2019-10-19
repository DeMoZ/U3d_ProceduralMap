using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour
{




    void OnCollisionEnter(Collision c)
    {
        //collision.collider;
        Debug.Log("Collision "+c.gameObject.name);
        foreach (ContactPoint contact in c.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }

    }
    void OnTriggerEnter(Collider c)
    {
        Debug.Log("Trigger "+c.gameObject.name);
        
           // Debug.DrawRay(c.ClosestPointOnBounds, c.ClosestPointOnBounds., Color.white);
        
    }
}
