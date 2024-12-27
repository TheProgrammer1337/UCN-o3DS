using System.Collections.Generic;
using UnityEngine;

public class DuctNode : MonoBehaviour
{
    public List<DuctNode> connectedNodes = new List<DuctNode>(); // List of directly connected nodes
    //public List<DuctNode> preferredWayToOffice = new List<DuctNode>(); // Preferred path to the office
    
    // Additional properties can be added here, like node type, position, etc.
}
