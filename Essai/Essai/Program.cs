using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Essai
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] table = new int[110, 110];
            string filePath = @"H:\IA\carte_2.csv";
            StreamReader sr = new StreamReader(filePath);
            int row = 0;
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(';');
                for (int i = 0; i < 110; i++)
                {
                    table[row, i] = Convert.ToInt32(line[i]);
                }
                row++;
            }

            Node test = new Node(new Position(105), new Position(0), table);
            Graph graph = new Graph();
            List<Node> chemin = graph.FindPath(test);

            Console.WriteLine(chemin.Count);
            for (int i = 0; i < chemin.Count; i++)
            {
                Console.WriteLine(chemin[i]);
            }

            Console.ReadLine();
        }
    }
}
