using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuctCharacter : MonoBehaviour
{
    public DuctNode currentNode; // The current node the character is at
    public DuctNode ActiveLure; // The active lure node
    public Transform toTransport; // The transform to move through the ducts
    public int currentVentilator = 0; // The current ventilator status
    public int currentAILevel = 0; // The AI level that determines movement chance (0 by default)

    public bool canGetLured = true; // Whether the character can be lured
    public bool canGetResetByHeater = true; // Whether the character can be reset by the heater
    public float percentChanceToGetLured = 1f; // 100% chance by default
	public string currentClosedDuct;
	public CustomNight CN;
	public string CharacterName;
	public string[] JumpscareAddresses;

    private DuctNode originalNode; // The original node to reset to
    private bool isMoving = false; // To prevent multiple movements at once

    void Start()
    {
        originalNode = currentNode; // Save the original node
		StartCoroutine(MoveThroughDucts());
		PrependJumpscarePath();
    }

	public void PrependJumpscarePath()
	{
    for (int i = 0; i < JumpscareAddresses.Length; i++)
    {
        JumpscareAddresses[i] = "Jumpscares/" + CharacterName + "/" + JumpscareAddresses[i];
    }
	}

    public IEnumerator MoveThroughDucts()
    {
        while (true) // Loop to continuously move through the ducts
        {
            yield return new WaitForSeconds(1.5f); // Wait 1.5 seconds between move attempts

            if (isMoving) yield break; // Prevent simultaneous movement
            isMoving = true;

            int roll = Random.Range(0, 30); // Roll a dice between 0 and 29

            if (currentAILevel > roll) // Only move if AI level is higher than the roll
            {
                DuctNode nextNode = null;

                if (canGetLured && ActiveLure != null)
                {
                    // Check if the character gets lured based on the percent chance
                    if (Random.value <= percentChanceToGetLured)
                    {
                        // If there's an active lure and can be lured, move towards the lure
                        foreach (DuctNode node in currentNode.connectedNodes)
                        {
                            if (node == ActiveLure)
                            {
                                nextNode = node;
                                break;
                            }
                        }
                    }
                }

                if (nextNode == null) // If no lure was found or applicable, move randomly
                {
                    List<DuctNode> possibleNodes = new List<DuctNode>(currentNode.connectedNodes);
                    nextNode = possibleNodes[Random.Range(0, possibleNodes.Count)];
                }

                // Move the transform to the next node
                toTransport.position = nextNode.transform.position;
                currentNode = nextNode;

				// Check if the character should be reset due to the heater
                if (canGetResetByHeater && currentVentilator == 3)
                {
                    ResetCharacter();
                }

                // Check if the current node has the "Finish" tag
                if (currentNode.gameObject.CompareTag("Finish"))
                {
                    HandleFinishNode(currentNode.gameObject);
                }
            }

            isMoving = false;
        }
    }

    private void HandleFinishNode(GameObject go)
    {
        // Code to handle what happens when the "Finish" node is reached
        Debug.Log("Reached the finish node!");
		if (go.name != currentClosedDuct)
		{
			CN.StartJumpscare(JumpscareAddresses, 5, CharacterName);
		}
		else
		{
			ResetCharacter();
		}
        // Implement any other logic here
    }

    private void ResetCharacter()
    {
        // Reset the character to the original node or a random "Respawn" node
        Debug.Log("Resetting character due to heater.");
        List<DuctNode> respawnNodes = new List<DuctNode>();

        foreach (DuctNode node in FindObjectsOfType<DuctNode>())
        {
            if (node.gameObject.CompareTag("Respawn"))
            {
                respawnNodes.Add(node);
            }
        }

        currentNode = respawnNodes.Count > 0 ? respawnNodes[Random.Range(0, respawnNodes.Count)] : originalNode;
        toTransport.position = currentNode.transform.position;
    }
}
