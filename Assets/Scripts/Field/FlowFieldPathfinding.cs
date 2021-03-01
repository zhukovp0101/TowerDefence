using System;
using System.Collections.Generic;
using System.Numerics;
using System.Transactions;
using UnityEngine;

namespace Field
{
    public class FlowFieldPathfinding
    {
        private class Connection
        {
            public Connection(Vector2Int coordinate, float weight)
            {
                this.coordinate = coordinate;
                this.weight = weight;
            }

            public Vector2Int coordinate;
            public float weight;
        }
        
        private Grid m_Grid;
        private Vector2Int m_Target;
        private Vector2Int m_Start;

        public FlowFieldPathfinding(Grid grid, Vector2Int target, Vector2Int start)
        {
            m_Grid = grid;
            m_Target = target;
            m_Start = start;
        }

        

        public void UpdateField()
        {
            foreach (Node node in m_Grid.EnumerateAllNodes())
            {
                node.ResetWeight();
            }
            
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            queue.Enqueue(m_Target);
            m_Grid.GetNode(m_Target).PathWeight = 0f;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                Node currentNode = m_Grid.GetNode(current);
                float weightToTarget = currentNode.PathWeight;

                foreach (Connection neighbour in GetNeighbours(current))
                {
                    Node neighbourNode = m_Grid.GetNode(neighbour.coordinate);
                    if (weightToTarget + neighbour.weight < neighbourNode.PathWeight)
                    {
                        neighbourNode.NextNode = currentNode;
                        neighbourNode.PathWeight = weightToTarget + neighbour.weight;
                        queue.Enqueue(neighbour.coordinate);
                    }
                }
            }

            RecalculateAvailableFields();
        }
        
        public bool CanOccupy(Vector2Int coordinate)
        {
            Node node = m_Grid.GetNode(coordinate);
            if (node.OccupationAvailability == OccupationAvailability.CanOccupy)
            {
                return true;
            }
            if (node.OccupationAvailability == OccupationAvailability.CanNotOccupy)
            {
                return false;
            }
            node.IsOccupied = true;

            var used = new bool[m_Grid.Width][];
            for (int index = 0; index < m_Grid.Width; index++)
            {
                used[index] = new bool[m_Grid.Height];
            }

            for (int i = 0; i < m_Grid.Width; i++)
            {
                for (int j = 0; j < m_Grid.Height; j++)
                {
                    used[i][j] = false;
                }
            }
            
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            
            queue.Enqueue(m_Target);
            used[m_Target.x][m_Target.y] = true;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                foreach (Vector2Int neighbour in GetLightNeighbours(current))
                {
                    if (neighbour == m_Start)
                    {
                        node.IsOccupied = false;
                        node.OccupationAvailability = OccupationAvailability.CanOccupy;
                        return true;
                    }
                    
                    if (!used[neighbour.x][neighbour.y])
                    {
                        used[neighbour.x][neighbour.y] = true;
                        queue.Enqueue(neighbour);
                    }
                }
            }
            
            node.IsOccupied = false;
            node.OccupationAvailability = OccupationAvailability.CanNotOccupy;
            return false;
        }
        
        private void RecalculateAvailableFields()
        {
            foreach (Node node in m_Grid.EnumerateAllNodes())
            {
                if (node.IsOccupied)
                {
                    node.OccupationAvailability = OccupationAvailability.CanNotOccupy;
                }
                else
                {
                    node.OccupationAvailability = OccupationAvailability.CanOccupy;
                }
            }

            m_Grid.GetNode(m_Start).OccupationAvailability = OccupationAvailability.CanNotOccupy;
            m_Grid.GetNode(m_Target).OccupationAvailability = OccupationAvailability.CanNotOccupy;

            Node nextNode = m_Grid.GetNode(m_Start).NextNode;
            Node target = m_Grid.GetNode(m_Target);
            while (nextNode != target)
            {
                nextNode.OccupationAvailability = OccupationAvailability.Undefined;
                nextNode = nextNode.NextNode;
            }
        }
        
        private IEnumerable<Vector2Int> GetLightNeighbours(Vector2Int current)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j * i != 0 || ((j == 0) && (i == 0)))
                    {
                        continue;
                    }

                    Vector2Int next = current + new Vector2Int(i, j);
                    
                    if (!CheckGridBoundaries(next))
                    {
                        continue;
                    }

                    if (m_Grid.GetNode(next).IsOccupied)
                    {
                        continue;
                    }
                    
                    yield return next;
                }
            }
        }

        private IEnumerable<Connection> GetNeighbours(Vector2Int current)
        {
            float diagonalWeight = (float) Math.Sqrt(2f);
            const float straightWeight = 1;
            
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0)
                    {
                        continue;
                    }

                    Vector2Int next = current + new Vector2Int(i, j);
                    
                    if (!CheckGridBoundaries(next))
                    {
                        continue;
                    }
                    
                    if (m_Grid.GetNode(next).IsOccupied)
                    {
                        continue;
                    }

                    // diagonal check
                    if (j * i != 0)
                    {
                        if (m_Grid.GetNode(current.x, current.y + j).IsOccupied ||
                            m_Grid.GetNode(current.x + i, current.y).IsOccupied)
                        {
                            continue;
                        }
                    }

                    if (i * j == 0)
                    {
                        yield return new Connection(next, straightWeight);
                    }
                    else
                    {
                        yield return new Connection(next, diagonalWeight);
                    }
                }
            }
        }

        private bool CheckGridBoundaries(Vector2Int coordinate)
        {
            return (0 <= coordinate.x && coordinate.x < m_Grid.Width) &&
                   (0 <= coordinate.y && coordinate.y < m_Grid.Height);
        }
    }
}