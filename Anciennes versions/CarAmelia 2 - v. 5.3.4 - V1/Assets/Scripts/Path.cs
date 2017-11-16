using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    // Couleur du chemin
    public Color lineColor;

    // Les noeuds du chemin
    private List<Transform> nodes = new List<Transform>();

    // Fonction qui dessine dans l'éditeur de Unity
    void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        // Liste des enfants dans l'objet "Path"
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        // Pour chaque noeud du chemin, on ajoute les noeuds dans une nouvelles liste
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
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0)
            {
                previousNode = nodes[i - 1].position;
            }
            // Si on est au premier noeud
            else if (i == 0 && nodes.Count > 1)
            {
                previousNode = nodes[nodes.Count - 1].position;
            }

            // Dessine la ligne
            Gizmos.DrawLine(previousNode, currentNode);
            // Dessine un rond autour du noeud
            Gizmos.DrawWireSphere(currentNode, 0.3f);
        }
    }

}

