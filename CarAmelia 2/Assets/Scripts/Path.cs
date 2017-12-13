using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Path : MonoBehaviour
{
    // Couleur du chemin
    public Color lineColor;

    // Les noeuds du chemin
    private List<Transform> nodes = new List<Transform>(); // Noeuds réels sur la SceneView
    private int[,] nodesTable = new int[118, 118]; // Tableau Excel des noeuds avec les relations

    // Fonction qui dessine dans l'éditeur de Unity (OnDrawGizmosSelected)
    public void OnDrawGizmos()
	{
		// Initialisation de la map
		TextAsset file = Resources.Load<TextAsset>("excel");
		string[] lines = file.text.Split('\n');
		for (int i = 0; i < lines.Length; i ++)
		{
			string[] columns = lines[i].Split(';');
			for (int j = 0; j < columns.Length; j++)
			{
				nodesTable[i, j] = Convert.ToInt32(columns[j]);
			}
		}

        // Couleur de la ligne
        Gizmos.color = lineColor;

        // Récupération de tous les objets "Transform" du "Path" et de ses enfants
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        // Récupération des objets "Transform" (les noeuds) uniquement dans les enfants du "Path"
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        // On dessine les traits entre les noeuds
        for (int i = 0; i < nodes.Count; i++)
        {
            // On dessine une sphère autour du noeud
            Gizmos.DrawWireSphere(nodes[i].position, 0.3f);

            // Pour tous les prochains noeuds accessibles, on dessine une ligne
            for (int j = 0; j < nodesTable.GetLength(1); j++)
            {
                if (nodesTable[i, j] == 1)
                {
                    Gizmos.DrawLine(nodes[i].position, nodes[j].position);
                }
            }
        }
    }
}