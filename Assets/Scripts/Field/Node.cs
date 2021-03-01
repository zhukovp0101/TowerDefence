using UnityEngine;

namespace Field
{
    public enum OccupationAvailability
    {
        CanOccupy,
        CanNotOccupy,
        Undefined
    }
    public class Node
    {
        public Vector3 Position;
        
        public Node NextNode;
        public bool IsOccupied;
        public OccupationAvailability OccupationAvailability;

        public float PathWeight;

        public Node(Vector3 position)
        {
            Position = position;
            OccupationAvailability = OccupationAvailability.Undefined;
        }

        public void ResetWeight()
        {
            PathWeight = float.MaxValue;
        }
    }
}