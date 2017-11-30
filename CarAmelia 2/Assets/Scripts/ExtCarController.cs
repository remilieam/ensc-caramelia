using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ExtCarController : CarController
{
    // Correspond aux noeuds qu'a parcourus la voiture
    private List<Position> crossedNodes;

    // Correspond à son degré de confiance (1 pas confiant dans les infos qu'il reçoit
    // et 5 confiance aveugle) et de générosité (1 pas généreux dans le partage
    // d'informations avec une voiture de même couleur et 5 très généreux)
    private int trust;
    private int generosity;

    private LineRenderer lineRenderer;

    // Permet de définir si la voiture a un noeud objectif ou si elle roule aléatoirement dans la carte
    private bool aleaMode;

    public Transform exit;

    // nodesToCross; : limité à 6 noeuds car la voiture a une petite mémoire

    private bool endExchange = true;

    public Material arrowMat;
    public Material lineMat;

    private Canvas canvasCross;
    private Canvas canvasCheck;
    private Image cross;
    private Image check;


    public void Start()
    {
        StartCar();
        canvasCross = GetComponentsInChildren<Canvas>()[1];
        canvasCheck = GetComponentsInChildren<Canvas>()[2];
        cross = canvasCross.GetComponentsInChildren<Image>()[0];
        check = canvasCheck.GetComponentsInChildren<Image>()[0];
        canvasCross.enabled = false;
        canvasCheck.enabled = false;

        // Voiture démarre à l'entrée de la map et de manière aléatoire
        position = new Position(111);
        aleaMode = true;

        // On définit la future position
        nextPosition = new Position(SuccessorAlea(position).Row);

        // On ajoute cette première position aux positions que la voiture a parcourues
        crossedNodes = new List<Position>();
        crossedNodes.Add(position);

        // Confiance & générosité
        trust = alea.Next(1, 6);
        generosity = alea.Next(1, 6);

        // Définition du texte et de l'action quand on clique sur le bouton
        textCanvas.text = "Générosité : " + generosity.ToString() + "\nConfiance : " + trust.ToString();

        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
    }

    public void Update()
    {
        endExchange = true;
        Sensors();

        if (endExchange)
        {
            lineRenderer.enabled = false;
            canvasCross.enabled = false;
            canvasCheck.enabled = false;
        }

        if (!aleaMode && endExchange)
        {
            lineRenderer.SetPosition(0, this.transform.position);
            lineRenderer.SetPosition(1, nodes[target.Row].position);
            DrawLine();
            lineRenderer.enabled = true;
        }
    }

    // Permet d'actualiser le noeud
    protected override void CheckWaypoint()
    {
        // On fait freiner la voiture avant l'arrivée sur le point
        if (Vector3.Distance(transform.position, nodes[nextPosition.Row].position) < distance_frein)
        {
            isBraking = true;
            if (Vector3.Distance(transform.position, nodes[nextPosition.Row].position) < distance_chgt)
            {
                // Si la voiture a atteint son objectif (même par hasard), elle a gagné ! (Et donc est détruite, c'est très logique.)
                if (nodes[nextPosition.Row] == exit)
                {
                    Destroy(gameObject);
                }

                position = nextPosition;
                crossedNodes.Add(position);

                if (aleaMode)
                {
                    nextPosition = SuccessorAlea(position);
                }
                else
                {
                    if (indexNode < nodesToCross.Count)
                    {
                        nextPosition = nodesToCross[indexNode].name;
                        indexNode++;
                    }
                    else
                    {
                        indexNode = 1;
                        aleaMode = true;
                        nextPosition = SuccessorAlea(position);
                    }
                }
            }
        }
        else
        {
            isBraking = false;

        }
    }

    private void FindingPath()
    {
        PathCalculation(target);
        // On définit le prochain objectif
        indexNode = 1;
        nextPosition = nodesToCross[indexNode].name;
        indexNode++;
    }

    private void ReceivingInformation(CarController car)
    {
        lineRenderer.SetPosition(0, car.transform.position);
        lineRenderer.SetPosition(1, this.transform.position);
        DrawArrow();
        lineRenderer.enabled = true;

        Vector3 imageScene = new Vector3((this.transform.position.x + car.transform.position.x) / 2.0f, (this.transform.position.y + car.transform.position.y) / 2.0f, (this.transform.position.z + car.transform.position.z) / 2.0f);
        Vector3 imageGame = new Vector3();

        if (cameraView.enabled)
        {
            imageGame = cameraView.WorldToViewportPoint(imageScene);
            cross.rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            check.rectTransform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        else
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            Camera cameraActive = new Camera();
            foreach (Camera camera in cameras)
            {
                if (camera.enabled && camera.transform.position != cameraView.transform.position)
                {
                    cameraActive = camera;
                }
            }
            imageGame = cameraActive.WorldToViewportPoint(imageScene);
            cross.rectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            check.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        
        cross.rectTransform.anchorMax = imageGame;
        cross.rectTransform.anchorMin = imageGame;
        
        check.rectTransform.anchorMax = imageGame;
        check.rectTransform.anchorMin = imageGame;
        
        if (car is ExtCarController)
        {
            ExtCarController extCar = (ExtCarController)car;
            int prob = extCar.alea.Next(1, 5);
            if (prob >= extCar.generosity && !extCar.aleaMode)
            {
                // Je reçois de l'info que si la voiture extérieure que je crois
                // a assez généreuse et n'est pas en mode aléatoire
                this.target = car.target;
                
                canvasCheck.enabled = true;

                FindingPath();
                aleaMode = false;
            }
            else
            {
                canvasCross.enabled = true;
            }
        }
        if (car is IntCarController)
        {
            IntCarController intCar = (IntCarController)car;
            if (intCar.sincerity)
            {
                bool extCarKnowsExit = false;

                foreach (Position exitKnown in intCar.exitsKnown)
                {
                    if (nodes[exitKnown.Row] == exit)
                    {
                        target = new Position(exitKnown.Row); ;
                        extCarKnowsExit = true;
                        canvasCheck.enabled = true;
                        FindingPath();
                        aleaMode = false;
                    }
                }
                if(!extCarKnowsExit)
                {
                    canvasCross.enabled = true;
                }
            }

            else
            {

                bool extCarGivesExit = false;

                if(intCar.exitsKnown.Count != 0)
                {
                    if (intCar.exitsKnown.Count == 1)
                    {
                        if(nodes[intCar.exitsKnown[0].Row] != exit)
                        {
                            target = new Position(intCar.exitsKnown[0].Row);

                            extCarGivesExit = true;
                            canvasCheck.enabled = true;
                            FindingPath();
                            aleaMode = false;
                        }

                    }
                    else
                    {
                        int exitGiven = alea.Next(intCar.exitsKnown.Count);

                        while (nodes[intCar.exitsKnown[exitGiven].Row] == exit)
                        {
                            exitGiven = alea.Next(intCar.exitsKnown.Count);
                        }
                        target = new Position(intCar.exitsKnown[exitGiven].Row);

                        extCarGivesExit = true;
                        canvasCheck.enabled = true;
                        FindingPath();
                        aleaMode = false;
                    }
                }
                
                if(!extCarGivesExit)
                {
                    canvasCross.enabled = true;
                }
            }
        }
    }


    public override void Stop(GameObject hitCar)
    {
        ExtCarController extCarHit = hitCar.gameObject.GetComponent<ExtCarController>();
        IntCarController intCarHit = hitCar.gameObject.GetComponent<IntCarController>();

        // Pas de target
        if (aleaMode)
        {
            if (hitCar.tag == "IntCar")
            {
                isBraking = true;
                intCarHit.isBraking = true;
                ReceivingInformation(intCarHit);
            }
            else if (hitCar.tag == "ExtCar" && extCarHit.exit == this.exit)
            {
                isBraking = true;
                extCarHit.isBraking = true;
                ReceivingInformation(extCarHit);
            }
        }

        // A une target
        else
        {
            int prob = alea.Next(1, 5);
            if (prob >= trust)
            {
                // Réception d'infos ==> pas confiance
                if (hitCar.tag == "IntCar")
                {
                    isBraking = true;
                    intCarHit.isBraking = true;
                    ReceivingInformation(intCarHit);
                }
                else if (hitCar.tag == "ExtCar" && extCarHit.exit == this.exit)
                {
                    isBraking = true;
                    extCarHit.isBraking = true;
                    ReceivingInformation(extCarHit);
                }
            }
            else
            {
                // Pas de réception d'infos ==> confiance
            }
        }

        endExchange = false;
    }

    private void DrawArrow()
    {
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 0f);
        curve.AddKey(1f, 1f);

        lineRenderer.material = arrowMat;
        lineRenderer.numCapVertices = 1;
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.widthCurve = curve;
        
    }

    public void DrawLine()
    {
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(1f, 1f);
        curve.AddKey(1f, 1f);
        
        lineRenderer.material = lineMat;
        lineRenderer.numCapVertices = 0;
        lineRenderer.widthMultiplier = 0.5f;
        lineRenderer.widthCurve = curve;
    }
       
}
