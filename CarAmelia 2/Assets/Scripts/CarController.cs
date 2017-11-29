using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public abstract class CarController : MonoBehaviour
{
    // La map dans Unity
    public Transform path;
    protected List<Transform> nodes;
    protected int[,] nodesTable;

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

    // Tous les attributs pour faire rouler la voiture
    protected Renderer carRenderer;
    protected WheelCollider wheelFL;
    protected WheelCollider wheelFR;
    protected WheelCollider wheelRL;
    protected WheelCollider wheelRR;
    protected float maxMotorTorque = 80f;
    protected float maxBrakeTorque = 200f;
    protected float maxSteerAngle = 40f;
    protected float maxSpeed = 280f;
    protected Vector3 centerOfMass;
    protected float currentSpeed;
    public bool isBraking = false;

    // Valeurs pour tester les distances
    protected float distance_frein = 10;
    protected float distance_chgt = 0.5f;
    protected float test_vit = 10;

    // Capteurs [Header("Sensors")]
    protected float sensorLength = 10f;
    protected Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0f);
    protected float frontSideSensorPosition = 0.4f;
    protected float frontSensorAngle = 30f;

    // Textures de la voiture"
    public Texture2D textureNormal;
    public Texture2D textureBraking;

    protected Canvas canvas;
    public Camera cameraView;
    protected Camera cameraBackCar;
    protected Button buttonCanvas;
    protected Text textCanvas;

    protected void StartCar()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        
        cameraBackCar = GetComponentsInChildren<Camera>()[0];
        cameraBackCar.enabled = false;
        canvas = GetComponentsInChildren<Canvas>()[0];

        // On n'affiche pas le canvas
        canvas.enabled = false;
        // Récupération du texte et du bouton du canvas
        textCanvas = canvas.GetComponentsInChildren<Text>()[0];
        buttonCanvas = canvas.GetComponentsInChildren<Button>()[0];
        buttonCanvas.onClick.AddListener(TaskOnClick);

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

    protected void TaskOnClick()
    {
        // Si on clique sur le bouton du canvas celui-ci devient invisible
        canvas.enabled = false;
    }
    protected void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!cameraBackCar.enabled && cameraView)
            {
                cameraView.enabled = false;
                cameraBackCar.enabled = true;
            }
            else if (!cameraBackCar.enabled && !cameraView)
            {

            }
            else
            {
                cameraView.enabled = true;
                cameraBackCar.enabled = false;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Si on clique sur la voiture le canvas devient visible
            canvas.enabled = true;
        }
    }

    public void FixedUpdate()
    {
        //Sensors();
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

    protected void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + frontSensorPosition;

        //front center sensor
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            //isBraking = true;
        }

        //front right sensor
        sensorStartPos.x += frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            //isBraking = true;
        }

        //front right angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            //isBraking = true;
        }

        //front left sensor
        sensorStartPos.x -= 2 * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            //isBraking = true;
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
