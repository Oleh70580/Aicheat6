using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;

    // —юди ми перет€гнемо нашу сферу-ц≥ль
    public Transform Target;

    // Ўвидк≥сть руху
    public float forceMultiplier = 10;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // 1. ѕќ„ј“ќ  ≈ѕ≤«ќƒ” (—киданн€)
    public override void OnEpisodeBegin()
    {
        // якщо агент впав з платформи (y < 0) -> обнул€Їмо його швидк≥сть ≥ ставимо в центр
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.linearVelocity = Vector3.zero; // ” Unity 6 velocity називаЇтьс€ linearVelocity
            this.rBody.angularVelocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // ѕерем≥щуЇмо ц≥ль у випадкове нове м≥сце, щоб агент не запам'€товував одне м≥сце
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    // 2. ўќ Ѕј„»“№ ј√≈Ќ“
    public override void CollectObservations(VectorSensor sensor)
    {
        // јгент знаЇ де ц≥ль (3 числа: x, y, z)
        sensor.AddObservation(Target.localPosition);

        // јгент знаЇ де в≥н сам (3 числа)
        sensor.AddObservation(this.transform.localPosition);

        // јгент знаЇ свою швидк≥сть (ще 6 чисел: л≥н≥йна ≥ кутова швидк≥сть)
        sensor.AddObservation(rBody.linearVelocity);
        sensor.AddObservation(rBody.linearVelocity); // “ут можна додати angularVelocity, але дл€ простоти поки так
    }

    // 3. я  ј√≈Ќ“ –”’ј™“№—я
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // ќтримуЇмо сигнали в≥д мозку (два числа: рух по X ≥ по Z)
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        // «астосовуЇмо ф≥зичну силу до куба
        rBody.AddForce(controlSignal * forceMultiplier);

        // 4. Ќј√ќ–ќƒ»
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // якщо д≥ставс€ до ц≥л≥ (ближче н≥ж 1.42 метра)
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f); // ƒаЇмо нагороду +1
            EndEpisode();    // «авершуЇмо раунд
        }

        // якщо впав з платформи
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode(); // ѕросто перезапускаЇмо, нагорода 0 (або можна дати штраф)
        }
    }

    // –”„Ќ≈  ≈–”¬јЌЌя (щоб ми могли перев≥рити клав≥атурою перед навчанн€м Ў≤)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal"); // —тр≥лочки або A/D
        continuousActionsOut[1] = Input.GetAxis("Vertical");   // —тр≥лочки або W/S
    }
}