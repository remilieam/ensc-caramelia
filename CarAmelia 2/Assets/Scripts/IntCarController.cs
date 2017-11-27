using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class IntCarController : CarController
{
	public bool sincerity;
	// Tableau des sorties connues (1 connaît, 0 connaît pas)
	// Index 0 : bleu, 1 : orange, 2 : jaune
	public List<Position> exitsKnown = new List<Position>();

    LineRenderer lineRenderer;
    public Canvas caneva;
    Text textCanvas;

    public void Start()
    {
        StartCar();

        textCanvas = caneva.GetComponentsInChildren<Text>()[0];
        float positionX = this.transform.position.x;
        float positionZ = this.transform.position.z;

        // Voiture démarre à une position aléatoire sur la map
        position = new Position(findNode(positionX, positionZ));
        // Position aléatoire de sa destination
        Position targetTemp = new Position(alea.Next(nodesTable.GetLength(1)));
        while(targetTemp == position)
        {
            targetTemp = new Position(alea.Next(nodesTable.GetLength(1)));
        }
        target = targetTemp;
        

        

        // On définit la future position de manière aléatoire
        PathCalculation(target);


        textCanvas.text = target.ToString() + "      " + nodesToCross.Count.ToString();

        // On définit la prochaine cible
        nextPosition = nodesToCross[indexNode].name;

        lineRenderer = this.gameObject.AddComponent<LineRenderer>();


        if (alea.Next (0, 2) == 1) {
			exitsKnown.Add (new Position (113));
		}
		if (alea.Next (0, 2) == 1) {
			exitsKnown.Add (new Position (115));
		}
		if (alea.Next (0, 2) == 1) {
			exitsKnown.Add (new Position (117));
		}

        


    }

    public void OnMouseDown()
    {
        caneva.enabled = false;
    }

    public void Update()
    {
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, nodes[target.Row].position);

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
                    indexNode = 0;
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

	public override void Stop (GameObject hitCar)
	{
	}

}
