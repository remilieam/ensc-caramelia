using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public abstract class CarController : MonoBehaviour
{
    // La map dans Unity
    public Transform path;
    protected List<Transform> nodes;
    protected int[,] nodesTable;

    // Tous les attributs pour faire rouler la voiture
    private WheelCollider wheelFL;
    private WheelCollider wheelFR;
    private WheelCollider wheelRL;
    private WheelCollider wheelRR;
    public float maxMotorTorque = 2000f;//80f;
    public float maxBrakeTorque = 20000f;//150f;
    public float maxSteerAngle = 25f;//45f;
    public float currentSpeed;
    public float maxSpeed = 140f;//100f;
    public Vector3 centerOfMass;
    public bool isBraking = false;
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    private Renderer carRenderer;
    // Valeurs pour tester les distances
    public float distance_frein = 10;
    public float distance_chgt = 0.5f;
    public float test_vit = 10;

    // Chiffre aléatoire
    protected System.Random alea;

    // Prochaine position objectif pour la voiture
    protected Position nextPosition;
    // Position de l'objectif à atteindre (sur un long chemin)
    public Position target;
    // Correspond à la position d'un noeud dans la map-tableau de la voiture
    protected Position position;

    // Le chemin que la voiture doit parcourir pour atteindre un position objective
    protected List<Node> nodesToCross;
    protected Graph graph;
    protected int indexNode = 1;

    [Header("Sensors")]
    public float sensorLength = 10f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0f);
    public float frontSideSensorPosition = 0.4f;
    public float frontSensorAngle = 30f;

    protected void StartCar()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;


        WheelCollider[] wheelColliders = GetComponentsInChildren<WheelCollider>();
        wheelRL = wheelColliders[0];
        wheelRR = wheelColliders[1];
        wheelFL = wheelColliders[2];
        wheelFR = wheelColliders[3];

        carRenderer = GetComponentsInChildren<Renderer>()[0];

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


        // Initialisation de la map
        nodesTable = new int[118, 118];
        string filePath = @"Assets\Scripts\Files\carte_2.csv";
        StreamReader sr = new StreamReader(filePath);
        int row = 0;
        while (!sr.EndOfStream)
        {
            string[] line = sr.ReadLine().Split(';');
            for (int i = 0; i < 118; i++)
            {
                nodesTable[row, i] = Convert.ToInt32(line[i]);
            }
            row++;
        }

        // Initialisation
        alea = new System.Random();
        nodesToCross = new List<Node>();
        graph = new Graph();
    }

    public void FixedUpdate()
    {
        Sensors();
        ApplySteer();
        Drive();
        CheckWaypoint();
        Braking();

    }

    protected void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[nextPosition.Row].position);
        // Calcul que je n'ai pas compris
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }


    protected void Drive()
    {
        // Vitesse
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isBraking)
        {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }

    protected void Braking()
    {
        if (isBraking && currentSpeed > test_vit)
        {
            carRenderer.material.mainTexture = textureBraking;
            wheelRL.brakeTorque = maxBrakeTorque;
            wheelRR.brakeTorque = maxBrakeTorque;
        }
        else
        {
            carRenderer.material.mainTexture = textureNormal;
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    protected abstract void CheckWaypoint();

    // Méthode qui définit le successeur aléatoire de la position de la voiture
    protected Position SuccessorAlea(Position currentPosition)
    {
        List<Position> successors = new List<Position>();

        for (int i = 0; i < nodesTable.GetLength(1); i++)
        {
            if (nodesTable[currentPosition.Row, i] == 1)
            {
                successors.Add(new Position(i));
            }
        }

        Position position = successors[alea.Next(successors.Count)];

        return position;
    }

    // Calcul du chemin pour atteindre un objectif
    protected void PathCalculation(Position final)
    {
        Node node = new Node(position, final, nodesTable);
        nodesToCross = graph.FindPath(node);
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + frontSensorPosition;

        //front center sensor
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }

        //front right sensor
        sensorStartPos.x += frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }

        //front right angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }

        //front left sensor
        sensorStartPos.x -= 2 * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
        }

        //front left angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
			Stop (hit.transform.gameObject);
        }
    }

	public abstract void Stop(GameObject hitCar);

}
