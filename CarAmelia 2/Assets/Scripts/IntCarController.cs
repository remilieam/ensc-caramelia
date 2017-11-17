using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IntCarController : CarController
{
    public void Start()
    {
        StartCar();

        float positionX = this.transform.position.x;
        float positionZ = this.transform.position.z;

        // Voiture démarre à une position aléatoire sur la map
        position = new Position(findNode(positionX, positionZ));
        // Position aléatoire de sa destination
        target = new Position(alea.Next(nodesTable.GetLength(1)));
        // On définit la future position de manière aléatoire
        PathCalculation(target);
        // On définit la prochaine cible
        nextPosition = nodesToCross[i].name;
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
                if (i < nodesToCross.Count)
                {
                    nextPosition = nodesToCross[i].name;
                    i++;
                }
                // S'il a atteint son objectif
                else
                {
                    // On re définit une position aléatoire
                    target = new Position(alea.Next(nodesTable.GetLength(1)));
                    // On recalcule
                    PathCalculation(target);
                    // On définit le prochain objectif
                    i = 0;
                    nextPosition = nodesToCross[i].name;
                    i++;
                }
            }

        }
        else
        {
            isBraking = false;

        }
    }
}
