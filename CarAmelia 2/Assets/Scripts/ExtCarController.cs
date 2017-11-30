using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ExtCarController : CarController
{
    // Correspond à son degré de confiance (1 pas confiant dans les infos qu'il reçoit
    // et 5 confiance aveugle) et de générosité (1 pas généreux dans le partage
    // d'informations avec une voiture de même couleur et 5 très généreux)
    private int trust;
    private int generosity;

    // Correspond à son objectif final ==> destruction de la voiture !
    private Transform exit;

    // Permet de définir si la voiture a un noeud objectif ou si elle roule aléatoirement dans la map
    private bool aleaMode;

    // Correspond aux noeuds qu'a parcourus la voiture
    private List<Position> crossedNodes = new List<Position>();

    // nodesToCross; : limité à 6 noeuds car la voiture a une petite mémoire

    // Permet de savoir si la voiture échange de l'information
    private bool endExchange = true;

    // Objets pour afficher la flèche d'échange d'information
    // et le trait qui indique sa position objectif
    private LineRenderer lineRenderer;
    public Material arrowMat;
    public Material lineMat;
    private Canvas canvasCross;
    private Canvas canvasCheck;
    private Image cross;
    private Image check;

    public Transform Exit
    {
        get { return exit; }
        set { exit = value; }
    }


    /// <summary>
    /// "Constructeur" pour initialiser les paramètres de la voiture extérieure
    /// </summary>
    public void Start()
    {
        StartCar();

        // Récupération de sa position initiale
        position = new Position(111);

        // Définition aléatoire de son degré de confiance et de générosité
        trust = alea.Next(1, 6);
        generosity = alea.Next(1, 6);

        // La voiture démarre en mode aléatoire ==> pas de position objectif
        aleaMode = true;

        // Détermination aléatoire de la prochaine position
        SuccessorAlea();
        // On ajoute cette première position aux positions que la voiture a parcourues
        crossedNodes.Add(position);

        // Définition du texte à afficher dans le canevas (informations relatives à la voiture extérieure)
        textCanvas.text = "Générosité : " + generosity.ToString() + "\nConfiance : " + trust.ToString();
        textCanvas.text += "\nProchaine position : " + nextPosition.ToString();

        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        // Initialisation des objets pour afficher la flèche d'échange d'information
        canvasCross = GetComponentsInChildren<Canvas>()[1];
        canvasCheck = GetComponentsInChildren<Canvas>()[2];
        cross = canvasCross.GetComponentsInChildren<Image>()[0];
        check = canvasCheck.GetComponentsInChildren<Image>()[0];
        canvasCross.enabled = false;
        canvasCheck.enabled = false;
    }

    /// <summary>
    /// Clair...
    /// </summary>
    public void Update()
    {
        // On part du principe que la voiture n'échange d'information
        endExchange = true;
        // On vérifie cette hypothèse
        SensorsObstacle();
        // Cas où la voiture n'échange réellement pas d'information
        if (endExchange)
        {
            // On n'affiche pas la flèche d'échange d'information
            lineRenderer.enabled = false;
            canvasCross.enabled = false;
            canvasCheck.enabled = false;
        }
        // Cas où la voiture a une position objectif et n'échange pas d'information
        if (!aleaMode && endExchange)
        {
            // Affichage du trait qui indique la position objectif
            DrawLine();
        }
    }

    /// <summary>
    /// Méthode pour actualiser la position de la voiture
    /// </summary>
    protected override void CheckWaypoint()
    {
        // On fait freiner la voiture avant l'arrivée sur le point
        if (Vector3.Distance(transform.position, nodes[nextPosition.Number].position) < distance_frein)
        {
            isBraking = true;

            // Dès que la voiture est assez proche de sa destination, on lui définit une nouvelle destination 
            if (Vector3.Distance(transform.position, nodes[nextPosition.Number].position) < distance_chgt)
            {
                // Si la voiture a atteint sa sortie (même par hasard), elle a gagné ! (Et donc est détruite, c'est très logique.)
                if (nodes[nextPosition.Number] == exit)
                {
                    Destroy(gameObject);
                }

                // Actualisation de sa position et ajout aux positions déjà parcourues
                position = nextPosition;
                crossedNodes.Add(position);

                // Cas où la voiture est en mode aléatoire
                if (aleaMode)
                {
                    SuccessorAlea();
                }

                // Cas où la voiture a une position objectif
                else
                {
                    // Cas où la voiture n'a pas atteint sa position objectif
                    if (indexNode < nodesToCross.Count)
                    {
                        // Détermination aléatoire d'une prochaine position
                        nextPosition = nodesToCross[indexNode].name;
                        indexNode++;
                    }

                    // Cas où elle a atteint sa position objectif
                    else
                    {
                        indexNode = 1;
                        // Passage en mode aléatoire
                        aleaMode = true;
                        // Détermination aléatoire d'une prochaine position
                        SuccessorAlea();
                    }
                }
            }
        }
        else
        {
            isBraking = false;
        }
    }

    /// <summary>
    /// Méthode pour détecter si la voiture croise une autre voiture
    /// </summary>
    public void SensorMeeting()
    {
        RaycastHit hit;

        // Position du capteur à gauche
        Vector3 sensorStartPos = transform.position + frontSensorPosition;
        sensorStartPos.x -= 2 * frontSideSensorPosition;

        // Capteur oblique de gauche
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            DetectionMeeting(hit.transform.gameObject);
        }
    }

    /// <summary>
    /// Méthode appelée lorsque la voiture détecte une autre voiture arrivant en sens inverse
    /// </summary>
    /// <param name="hitCar"></param>
    public void DetectionMeeting(GameObject hitCar)
    {
        ExtCarController extCarHit = hitCar.gameObject.GetComponent<ExtCarController>();
        IntCarController intCarHit = hitCar.gameObject.GetComponent<IntCarController>();

        // Cas où la voiture est en mode aléatoire et n'a donc pas de position objectif
        if (aleaMode)
        {
            // Cas où la voiture est une voiture intérieure
            if (hitCar.tag == "IntCar")
            {
                isBraking = true;
                intCarHit.IsBraking = true;
                ReceivingInformation(intCarHit);
            }

            // Cas où la voiture est une voiture extérieure de même couleur
            else if (hitCar.tag == "ExtCar" && extCarHit.exit == this.exit)
            {
                isBraking = true;
                extCarHit.IsBraking = true;
                ReceivingInformation(extCarHit);
            }
        }

        // Cas où la voiture a une position objectif
        else
        {
            // Si la voiture n'est pas confiante et demande confirmation (ou pas ^^) de l'information
            if (alea.Next(1, 5) >= trust)
            {
                // Cas où la voiture est une voiture intérieure
                if (hitCar.tag == "IntCar")
                {
                    isBraking = true;
                    intCarHit.IsBraking = true;
                    ReceivingInformation(intCarHit);
                }

                // Cas où la voiture est une voiture extérieure de même couleur
                else if (hitCar.tag == "ExtCar" && extCarHit.exit == this.exit)
                {
                    isBraking = true;
                    extCarHit.IsBraking = true;
                    ReceivingInformation(extCarHit);
                }
            }
        }

        // La voiture est en train d'échanger de l'information
        endExchange = false;
    }

    /// <summary>
    /// Méthode appelée lorsque la voiture reçoit de l'information d'une autre voiture (intérieure ou extérieure)
    /// </summary>
    /// <param name="car">Voiture captée pour échanger de l'information</param>
    private void ReceivingInformation(CarController car)
    {
        // Affichage de la flèche d'échange d'information
        DrawArrow(car);

        // Cas où la voiture avec qui on échange de l'information est une voiture extérieure
        if (car is ExtCarController)
        {
            ExtCarController extCar = (ExtCarController)car;

            // Si la voiture est assez généreuse pour partager de l'information (elle n'est donc pas en mode aléatoire)
            if (extCar.alea.Next(1, 5) >= extCar.generosity && !extCar.aleaMode)
            {
                // ... alors la voiture prend la position objectif de la voiture croisée
                this.target = car.Target;

                // Échange réussi ! ^^ ==> Affichage de la checkmark
                canvasCheck.enabled = true;

                // Calcul du chemin le plus court pour atteindre sa position objectif
                FindingPath();
                aleaMode = false;
            }
            else
            {
                // Échec de l'échange ! :'( ==> Affichage de la crossmark ==> Il ne se passe rien
                canvasCross.enabled = true;
            }
        }

        // Cas où la voiture avec qui on échange de l'information est une voiture intérieure
        if (car is IntCarController)
        {
            IntCarController intCar = (IntCarController)car;

            // Cas où la voiture intérieure croisée est sincère
            if (intCar.Sincerity)
            {
                bool extCarKnowsExit = false;

                // Vérification que la voiture intérieure croisée connaît la sortie de "notre" voiture
                foreach (Position exitKnown in intCar.ExitsKnown)
                {
                    if (nodes[exitKnown.Number] == exit)
                    {
                        extCarKnowsExit = true;

                        // La voiture prend la position objectif de la voiture croisée
                        target = new Position(exitKnown.Number);

                        // Échange réussi ! ^^ ==> Affichage de la checkmark
                        canvasCheck.enabled = true;

                        // Calcul du chemin le plus court pour atteindre sa position objectif
                        FindingPath();
                        aleaMode = false;
                    }
                }

                // Cas où la voiture intérieure croisée ne connaît pas la sortie de "notre" voiture
                if (!extCarKnowsExit)
                {
                    // Échec de l'échange ! :'( ==> Affichage de la crossmark ==> Il ne se passe rien
                    canvasCross.enabled = true;
                }
            }

            // Cas où la voiture intérieure croisée est malhonnête
            else
            {
                bool extCarGivesExit = false;

                // Vérification que la voiture intérieure croisée connaît au moins une sortie
                if (intCar.ExitsKnown.Count != 0)
                {
                    // Cas où la voiture intérieure croisée ne connaît qu'une seule sortie
                    if (intCar.ExitsKnown.Count == 1)
                    {
                        // Vérification que la sortie connue n'est pas seule de "notre" voiture
                        if (nodes[intCar.ExitsKnown[0].Number] != exit)
                        {
                            extCarGivesExit = true;

                            // La voiture prend la position objectif de la voiture croisée
                            target = new Position(intCar.ExitsKnown[0].Number);

                            // Échange réussi ! ^^ ==> Affichage de la checkmark
                            canvasCheck.enabled = true;

                            // Calcul du chemin le plus court pour atteindre sa position objectif
                            FindingPath();
                            aleaMode = false;
                        }

                    }

                    // Cas où la voiture intérieure croisée connaît plusieurs sorties
                    else
                    {
                        // On en prend une aléatoirement qui ne soit pas celle de "notre" voiture
                        int exitGiven = alea.Next(intCar.ExitsKnown.Count);
                        while (nodes[intCar.ExitsKnown[exitGiven].Number] == exit)
                        {
                            exitGiven = alea.Next(intCar.ExitsKnown.Count);
                        }

                        extCarGivesExit = true;

                        // La voiture prend la position objectif de la voiture croisée
                        target = new Position(intCar.ExitsKnown[exitGiven].Number);

                        // Échange réussi ! ^^ ==> Affichage de la checkmark
                        canvasCheck.enabled = true;

                        // Calcul du chemin le plus court pour atteindre sa position objectif
                        FindingPath();
                        aleaMode = false;
                    }
                }

                // Cas où la voiture intérieure croisée ne connaît pas de sorties
                // ou ne connaît que la sortie de "notre" voiture
                if (!extCarGivesExit)
                {
                    // Échec de l'échange ! :'( ==> Affichage de la crossmark ==> Il ne se passe rien
                    canvasCross.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Méthode pour trouver le chemin le plus court vers la position objectif
    /// </summary>
    private void FindingPath()
    {
        indexNode = 1;
        PathCalculation();
        // On définit la prochaine position que la voiture doit atteindre
        nextPosition = nodesToCross[indexNode].name;
        indexNode++;
    }

    /// <summary>
    /// Méthode pour dessiner une flèche d'échange d'information
    /// </summary>
    /// <param name="car">Voiture captée pour échanger de l'information</param>
    private void DrawArrow(CarController car)
    {
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 0f);
        curve.AddKey(1f, 1f);

        // Définition des paramètres décoratifs de la flèche
        lineRenderer.material = arrowMat;
        lineRenderer.numCapVertices = 1;
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.widthCurve = curve;

        // Définition de la position de départ et d'arrivée de la flèche
        lineRenderer.SetPosition(0, car.transform.position);
        lineRenderer.SetPosition(1, this.transform.position);

        lineRenderer.enabled = true;

        // Récupération des coordonnées de l'image sur la SceneView
        Vector3 imageScene = new Vector3((this.transform.position.x + car.transform.position.x) / 2.0f, (this.transform.position.y + car.transform.position.y) / 2.0f, (this.transform.position.z + car.transform.position.z) / 2.0f);
        Vector3 imageGame = new Vector3();

        // Cas où la caméra générale est activée
        if (cameraView.enabled)
        {
            // Définition des coordonnées de l'image sur la GameView
            imageGame = cameraView.WorldToViewportPoint(imageScene);
            cross.rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            check.rectTransform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        // Cas où c'est une caméra arrière qui est activée (pas forcément celle de "notre" voiture)
        else
        {
            // Recherche de la caméra arrière activée (la voiture dont la caméra arrière est activée)
            Camera[] cameras = FindObjectsOfType<Camera>();
            Camera cameraActive = new Camera();
            foreach (Camera camera in cameras)
            {
                if (camera.enabled && camera.transform.position != cameraView.transform.position)
                {
                    cameraActive = camera;
                }
            }

            // Définition des coordonnées de l'image sur la GameView
            imageGame = cameraActive.WorldToViewportPoint(imageScene);
            cross.rectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            check.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        // Positionnement des images sur la GameView
        cross.rectTransform.anchorMax = imageGame;
        cross.rectTransform.anchorMin = imageGame;
        check.rectTransform.anchorMax = imageGame;
        check.rectTransform.anchorMin = imageGame;

    }

    /// <summary>
    /// Méthode pour dessiner le trait qui indique la position objectif
    /// </summary>
    public void DrawLine()
    {
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(1f, 1f);
        curve.AddKey(1f, 1f);

        // Définition des paramètres décoratifs du trait
        lineRenderer.material = lineMat;
        lineRenderer.numCapVertices = 0;
        lineRenderer.widthMultiplier = 0.5f;
        lineRenderer.widthCurve = curve;

        // Définition de la position de départ et d'arrivée du trait
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, nodes[target.Number].position);

        lineRenderer.enabled = true;
    }
}
