using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    public Transform Target;
    public Transform TrainingArea; 
    public float forceMultiplier = 10;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
       
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.linearVelocity = Vector3.zero;
            this.rBody.angularVelocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

   
        MoveTargetToSafePosition();
    }

  
    void MoveTargetToSafePosition()
    {
        bool safePositionFound = false;
        int attempts = 0;
        Vector3 potentialPosition = Vector3.zero;

        while (!safePositionFound && attempts < 100)
        {
            attempts++;
           
            potentialPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));

           
            if (!Physics.CheckSphere(transform.parent.position + potentialPosition, 0.5f, LayerMask.GetMask("Default")))
            {
              
                safePositionFound = true;
            }
          
            safePositionFound = true; 
        }
        Target.localPosition = potentialPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.linearVelocity);


    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        AddReward(-1f / MaxStep);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

       
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            
            AddReward(-0.1f);

          
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}