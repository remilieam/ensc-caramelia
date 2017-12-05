using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public abstract class CarController : MonoBehaviour
{
    // La map dans Unity
	protected Transform path;
    protected List<Transform> nodes = new List<Transform>(); // Liste des noeuds de la map de Unity
    protected int[,] nodesTable = new int[118, 118]; // Cf. excel

    // Chiffre aléatoire
    protected System.Random alea = new System.Random();

    // Le chemin que la voiture doit parcourir pour atteindre une position objectif
    protected List<Node> nodesToCross = new List<Node>();
    // Là où la voiture en est sur le chemin pour atteindre une position objectif
    protected int indexNode = 1;
    // Correspond à la position d'un noeud dans la map-tableau de la voiture
    protected Position position;
    // Prochaine position à atteindre pour la voiture
    protected Position nextPosition;
    // Position objectif de la voiture
    protected Position target;
    // Utilisé pour trouver le chemin le plus court pour atteindre une position objectif
    protected Graph graph = new Graph();

    // Attributs inhérents à la voiture
    protected Renderer carRenderer;
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    protected WheelCollider wheelFL;
    protected WheelCollider wheelFR;
    protected WheelCollider wheelRL;
    protected WheelCollider wheelRR;
    protected float maxMotorTorque = 80f;
    protected float maxBrakeTorque = 200f;
    protected float maxSteerAngle = 40f;
    protected float maxSpeed = 280f;
    protected float brakeDistance = 10f;
    protected float changeNodeDistance = 3f;
    protected bool isBraking = false;
    protected bool isStopped = false;
    protected bool isSteering = false;
    protected float minSpeed = 5f;
    public float currentSpeed; 

    // Capteurs [Header("Sensors")]
    protected Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0f);
    protected float frontSideSensorPosition = 0.4f;

    // Canevas et caméras
	protected Camera cameraView;
    protected Canvas canvas;
    protected Camera cameraBackCar;
    protected Button buttonCanvas;
    protected Text textCanvas;

    public float newSteer;

    public Position Target
    {
        get { return target; }
    }

    public bool IsBraking
    {
        set { isBraking = value; }
    }

	public Camera CameraView
	{
		get { return cameraView; }
		set { cameraView = value; }
	}

	public Transform Path
	{
		get { return path; }
		set { path = value; }
	}

    /// <summary>
    /// "Constructeur" pour initialiser les paramètres de la voiture
    /// </summary>
    protected void StartCar()
    {
        // Création de la liste contenant les noeuds de la map
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        // Initialisation de la map
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

        // Initalisation des attributs inhérents à la voiture
        WheelCollider[] wheelColliders = GetComponentsInChildren<WheelCollider>();
        wheelRL = wheelColliders[0];
        wheelRR = wheelColliders[1];
        wheelFL = wheelColliders[2];
        wheelFR = wheelColliders[3];
        carRenderer = GetComponentsInChildren<Renderer>()[0];

        // Initialisation de la caméra arrière
        cameraBackCar = GetComponentsInChildren<Camera>()[0];
        cameraBackCar.enabled = false;

        // Initialisation du canevas affichant le comportement de la voiture
        canvas = GetComponentsInChildren<Canvas>()[0];
        canvas.enabled = false;
        textCanvas = canvas.GetComponentsInChildren<Text>()[0];
        buttonCanvas = canvas.GetComponentsInChildren<Button>()[0];
        buttonCanvas.onClick.AddListener(TaskOnClick);
    }

    /// <summary>
    /// Assez clair...
    /// </summary>
    public void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWaypoint();
    }

    /// <summary>
    /// Méthode appelée lorsqu'on clique sur le bouton du canevas de la voiture
    /// </summary>
    protected void TaskOnClick()
    {
        canvas.enabled = false;
    }

    /// <summary>
    /// Méthode appelée lorsqu'on passe le pointeur de la souris sur la voiture
    /// pour afficher le canevas ou changer de caméra
    /// </summary>
    protected void OnMouseOver()
    {
        // Si on clique sur le bouton gauche
        if (Input.GetMouseButtonDown(0))
        {
            // Cas où on passe de la caméra générale à la caméra arrière
            if (!cameraBackCar.enabled && cameraView.enabled)
			{
				cameraBackCar.enabled = true;
                cameraView.enabled = false;
            }

            // Cas où on passe de la caméra arrière d'une autre voiture à "notre" caméra arrière
            else if (!cameraBackCar.enabled && !cameraView.enabled)
			{
				cameraBackCar.enabled = true;

				// On désactive toutes les caméras arrières activées en épargnant la caméra de "notre" voiture
                Camera[] cameras = FindObjectsOfType<Camera>();
                foreach (Camera camera in cameras)
                {
					if(camera.enabled && camera.transform.position != cameraBackCar.transform.position)
                    {
                        camera.enabled = false;
                    }
                }
            }

            // Cas où on passe de la caméra arrière à la caméra générale
            else
            {

                cameraView.enabled = true;
                cameraBackCar.enabled = false;
            }
        }

        // Si on clique sur le bouton droit
        if (Input.GetMouseButtonDown(1))
        {
            canvas.enabled = true;
        }
    }

    /// <summary>
    /// Méthode pour faire tourner la voiture
    /// </summary>
    protected void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[nextPosition.Number].position);
        // Calcul que nous n'avons pas compris...
        newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
        if (newSteer > 1.5f || newSteer < -1.5f)
        {
            isSteering = true;
        }
        else
        {
            isSteering = false;
        }
    }

    /// <summary>
    /// Méthode pour faire avancer/freiner/stopper la voiture
    /// </summary>
    protected void Drive()
    {
        // Réglage de la vitesse
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (isStopped)
        {
            //textCanvas.text = "Stop";
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
            wheelFL.brakeTorque = maxBrakeTorque * 100f;
            wheelFR.brakeTorque = maxBrakeTorque * 100f;
            wheelRL.brakeTorque = maxBrakeTorque * 100f;
            wheelRR.brakeTorque = maxBrakeTorque * 100f;
        }
        else
        {
            // Vérification que la voiture roule en-dessous de la vitesse maximale autorisée
            if (isBraking)
            {
                carRenderer.material.mainTexture = textureBraking;

                if (currentSpeed < minSpeed)
                {
                    //textCanvas.text = "Frein + currentSpeed < minSpeed";
                    wheelFL.motorTorque = maxMotorTorque;
                    wheelFR.motorTorque = maxMotorTorque;
                    wheelRL.brakeTorque = 0;
                    wheelRR.brakeTorque = 0;
                    wheelFL.brakeTorque = 0;
                    wheelFR.brakeTorque = 0;
                }
                else
                {
                    //textCanvas.text = "Frein + currentSpeed > minSpeed";
                    wheelFL.motorTorque = 0;
                    wheelFR.motorTorque = 0;
                    wheelRL.brakeTorque = maxBrakeTorque;
                    wheelRR.brakeTorque = maxBrakeTorque;
                    wheelFL.brakeTorque = 0;
                    wheelFR.brakeTorque = 0;
                }
            }

            if (!isBraking)
            {
                carRenderer.material.mainTexture = textureNormal;

                if (currentSpeed < maxSpeed)
                {
                    //textCanvas.text = "!Frein + currentSpeed < maxSpeed";

                    wheelFL.motorTorque = maxMotorTorque;
                    wheelFR.motorTorque = maxMotorTorque;
                    wheelRL.brakeTorque = 0;
                    wheelRR.brakeTorque = 0;
                    wheelFL.brakeTorque = 0;
                    wheelFR.brakeTorque = 0;
                }
                else
                {
                    //textCanvas.text = "!Frein + currentSpeed > maxSpeed";

                    wheelFL.motorTorque = 0;
                    wheelFR.motorTorque = 0;
                    wheelRL.brakeTorque = 0;
                    wheelRR.brakeTorque = 0;
                    wheelFL.brakeTorque = 0;
                    wheelFR.brakeTorque = 0;
                }
            }
        }
    }

    /// <summary>
    /// Méthode pour réguler la vitesse de la voiture
    /// </summary>
    protected void SpeedRegulation()
    {
    }

    /// <summary>
    /// Méthode qui définit aléatoirement la prochaine position de la voiture
    /// </summary>
    /// <param name="currentPosition">Position actuelle de la voiture</param>
    /// <returns></returns>
    protected void SuccessorAlea()
    {
        List<Position> successors = new List<Position>();

        // Récupération de toutes les prochaines positions possibles à partir de la position actuelle
        for (int i = 0; i < nodesTable.GetLength(1); i++)
        {
            if (nodesTable[position.Number, i] == 1)
            {
                successors.Add(new Position(i));
            }
        }

        // Choix aléatoire d'une position possible
        nextPosition = successors[alea.Next(successors.Count)];
    }
    
    /// <summary>
    /// Méthode pour trouver le chemin le plus court vers la position objectif
    /// à l'aide de l'A*
    /// </summary>
    protected void PathCalculation()
    {
        Node node = new Node(position, target, nodesTable);
        nodesToCross = graph.FindPath(node);
    }

    /// <summary>
    /// Méthode pour détecter les autres voitures
    /// </summary>
    protected void SensorsObstacle()
    {
        // Permet de savoir si la voiture détècte une autre voiture
        bool detected = false;

        //textCanvas.text = "";
        if (!isSteering)
        {
            float sensorLengthObstacle = 10f;
            float frontSensorAngle = 5f;
            float obliqueSensorAngle = 30f;
            RaycastHit hit;

            // Position du capteur
            Vector3 sensorStartPos = transform.position + frontSensorPosition;

            // Capteur frontal du milieu
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthObstacle))
            {
                //textCanvas.text = "Milieu";
                detected = true;
                Debug.DrawLine(sensorStartPos, hit.point);
                DetectionFront(hit.transform.gameObject);
            }

            // Repositionnement du capteur vers la droite
            sensorStartPos.x += frontSideSensorPosition;

            // Capteur frontal de droite
            if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLengthObstacle))
            {
                detected = true;
                Debug.DrawLine(sensorStartPos, hit.point);
                DetectionFront(hit.transform.gameObject);
            }

            // Capteur oblique de droite
            if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(obliqueSensorAngle, transform.up) * transform.forward, out hit, sensorLengthObstacle))
            {
                //textCanvas.text = "Droite";
                detected = true;
                Debug.DrawLine(sensorStartPos, hit.point);
                DetectionRight(hit.transform.gameObject);
            }

            //// Repositionnement du capteur vers la gauche
            //sensorStartPos.x -= 2 * frontSideSensorPosition;

            // Capteur frontal de gauche
            if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLengthObstacle))
            {
                detected = true;
                Debug.DrawLine(sensorStartPos, hit.point);
                DetectionFront(hit.transform.gameObject);
            }

            //// Capteur oblique de gauche
            //if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-obliqueSensorAngle, transform.up) * transform.forward, out hit, sensorLengthObstacle))
            //{
            //    detected = true;
            //    Debug.DrawLine(sensorStartPos, hit.point);
            //    DetectionFront(hit.transform.gameObject);
            //}
        }
        if (!detected)
        {
            isStopped = false;
        }
    }

    /// <summary>
    /// Méthode qui fait stopper la voiture lorsqu'elle détecte
    /// une voiture en face d'elle
    /// </summary>
    /// <param name="hitCar">Voiture détéctée</param>
    public void DetectionFront(GameObject hitCar)
    {
        //if (hitCar.GetComponent<CarController>().isStopped)
        //{
            isStopped = true;
        //}
        //else
        //{
        //    isStopped = false;
        //}
    }

    /// <summary>
    /// Méthode qui fait stopper la voiture lorsqu'elle détecte
    /// une voiture dans une priorité à droite
    /// </summary>
    /// <param name="hitCar">Voiture détéctée</param>
    public void DetectionRight(GameObject hitCar)
    {
        isStopped = true;
    }

    protected abstract void CheckWaypoint();

    /// <summary>
    /// Méthode pour écrire les informations dans le canevas de la voiture
    /// </summary>
    public abstract void WriteInformation();
}
