using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class IntCarController : CarController
{
    // Tableau des sorties connues (1 connaît, 0 connaît pas)
    // Index 0 : bleu, 1 : orange, 2 : jaune
    public List<Position> exitsKnown = new List<Position>();

    public bool sincerity;
    
    private Canvas canvas;
    private Text textCanvas;

    public void Start()
    {
        StartCar();
        canvas = GetComponentsInChildren<Canvas>()[0];
        textCanvas = canvas.GetComponentsInChildren<Text>()[0];
        float positionX = this.transform.position.x;
        float positionZ = this.transform.position.z;

        // Voiture démarre à une position aléatoire sur la map
        position = new Position(findNode(positionX, positionZ));
        // Position aléatoire de sa destination
        Position targetTemp = new Position(alea.Next(nodesTable.GetLength(1)));
        while (targetTemp.SamePosition(position))
        {
            targetTemp = new Position(alea.Next(nodesTable.GetLength(1)));
        }
        target = targetTemp;

        // On définit la future position de manière aléatoire
        PathCalculation(target);

        // On définit la prochaine cible
        nextPosition = nodesToCross[indexNode].name;
        
        if (alea.Next(0, 2) == 1)
        {
            exitsKnown.Add(new Position(113));
        }
        if (alea.Next(0, 2) == 1)
        {
            exitsKnown.Add(new Position(115));
        }
        if (alea.Next(0, 2) == 1)
        {
            exitsKnown.Add(new Position(117));
        }

        // On n'affiche pas le canvas
        canvas.enabled = false;
        // Récupération du texte et du bouton du canvas
        Button buttonCanvas = canvas.GetComponentsInChildren<Button>()[0];
        // Définition du texte et de l'action quand on clique sur le bouton
        string message = "";
        for (int j = 0; j < exitsKnown.Count; j++)
        {
            if(exitsKnown[j].Row == 113)
            {
                message += "\n- Sortie Bleue";
            }
            if (exitsKnown[j].Row == 115)
            {
                message += "\n- Sortie Orange";
            }
            if (exitsKnown[j].Row == 117)
            {
                message += "\n- Sortie Blanche";
            }
        }
        textCanvas.text = "Les sorties connues : " + message + "\nTarget : " + target.ToString() + ", Noeuds : " + nodesToCross.Count.ToString();
        buttonCanvas.onClick.AddListener(TaskOnClick);

    }

    public void Update()
    {
        Sensors();
    }
    
    
    private int findNode(float x, float z)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if(x == nodes[i].transform.position.x && z == nodes[i].transform.position.z)
            {
                return i;
            }
        }

        return 0;
    }

    // Permet d'actualiser le noeud
    protected override void CheckWaypoint()
    {
        // On fait freiner la voiture avant l'arrivée sur le point
        if (Vector3.Distance(transform.position, nodes[nextPosition.Row].position) < distance_frein)
        {
            isBraking = true;
            // On change le point objectif dès qu'il est suffisament proche 
            if (Vector3.Distance(transform.position, nodes[nextPosition.Row].position) < distance_chgt)
            {
                position = nextPosition;

                // S'il est toujours sur le chemin pour atteindre son objectif
                if (indexNode < nodesToCross.Count)
                {
                    nextPosition = nodesToCross[indexNode].name;
                    indexNode++;
                }
                // S'il a atteint son objectif
                else
                {
                    // On re définit une position aléatoire
                    target = new Position(alea.Next(nodesTable.GetLength(1)));
                    // On recalcule
                    PathCalculation(target);
                    // On définit le prochain objectif
                    indexNode = 1;
                    nextPosition = nodesToCross[indexNode].name;
                    indexNode++;
                }
            }

        }
        else
        {
            isBraking = false;

        }

	}

    void TaskOnClick()
    {
        // Si on clique sur le bouton du canvas celui-ci devient invisible
        canvas.enabled = false;
    }
    void OnMouseDown()
    {
        // Si on clique sur la voiture le canvas devient visible
        canvas.enabled = true;
    }

    public override void Stop (GameObject hitCar)
	{
	}

}
