using UnityEngine;

public class EyeStabilizer : MonoBehaviour
{
    
    public Rigidbody agentRb;

    void LateUpdate()
    {
        
        if (agentRb == null) return;

      
        Vector3 direction = agentRb.linearVelocity; 
        direction.y = 0; 

   
        if (direction.magnitude > 0.1f)
        {
  
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            
            Vector3 currentEuler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, currentEuler.y, 0);
        }
    }
}