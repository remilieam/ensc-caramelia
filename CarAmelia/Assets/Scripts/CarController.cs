using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}

public class CarController : MonoBehaviour
{
    // Le chemin
    public Transform path;
    private List<Transform> nodes;
    private int currectNode = 0;

    public float maxSteerAngle = 45f;
    public float maxMotorTorque = 80f;
    public float currentSpeed;
    public float maxSpeed = 100f;

	public List<AxleInfo> axleInfos;
    //public float maxMotorTorque;
    //public float maxSteeringAngle;

    void Start()
    {
        // On recrée notre liste de noeuds
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

	void FixedUpdate()
	{
        ApplySteer();
        Drive();
        CheckWaypointDistance();		
	}

    void ApplySteer()
    {
        //float motor = maxMotorTorque * Input.GetAxis("Vertical");
        //float steering = maxSteeringAngle * Input.GetAxis("Horizontal");


        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currectNode].position);
        // Calcul que je n'ai pas compris
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = newSteer;
                axleInfo.rightWheel.steerAngle = newSteer;
            }

            //if (axleInfo.motor)
            //{
            //    axleInfo.leftWheel.motorTorque = motor;
            //    axleInfo.rightWheel.motorTorque = motor;
            //}
        }
    }

    private void Drive()
    {
        // Vitesse
        currentSpeed = 2 * Mathf.PI * axleInfos[0].leftWheel.radius * axleInfos[0].leftWheel.rpm * 60 / 1000;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                if (currentSpeed < maxSpeed)
                {
                    axleInfo.leftWheel.motorTorque = maxMotorTorque;
                    axleInfo.rightWheel.motorTorque = maxMotorTorque;
                }
                else
                {
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.rightWheel.motorTorque = 0;
                }
            }
        }
        
    }
    // Permet d'actualiser le noeud
    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currectNode].position) < 0.5f)
        {
            if (currectNode == nodes.Count - 1)
            {
                currectNode = 0;
            }
            else
            {
                currectNode++;
            }
        }
    }
}
