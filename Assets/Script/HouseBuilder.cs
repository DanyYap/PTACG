using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HouseBuilder : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class Wall
    {
        public GameObject wallObject; // Parent object for the wall (e.g., front wall)
        public GameObject[] parts; // Parts of the wall (e.g., segments of the front wall)
        public int requiredBranches; // Number of branches needed
        public int requiredRocks; // Number of rocks needed
        [HideInInspector] public int branchesPlaced = 0; // Number of branches placed for this wall
        [HideInInspector] public int rocksPlaced = 0; // Number of rocks placed for this wall
    }

    public Wall[] walls; // Array of main walls (front, back, left, right)
    public TMP_Text gatherText; // Text to display the message for this house
    public Transform buildArea; // Reference to the plane where the house is built
    private int totalLogsNeeded; // Total logs required to build the entire house
    private int logsGathered = 0;
    public GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
        CalculateTotalLogsNeeded();
        UpdateGatherText();
    }

    private void CalculateTotalLogsNeeded()
    {
        totalLogsNeeded = 0;
        foreach (var wall in walls)
        {
            totalLogsNeeded += wall.requiredBranches + wall.requiredRocks;
        }
    }

    private void UpdateGatherText()
    {
        foreach (var wall in walls)
        {
            int itemsPlaced = wall.branchesPlaced + wall.rocksPlaced;
            int itemsRequired = wall.requiredBranches + wall.requiredRocks;

            // If this wall is not fully built, update the text for it
            if (itemsPlaced < itemsRequired)
            {
                gatherText.text = $"To build {wall.wallObject.name}, you need: " +
                                  $"{wall.requiredRocks - wall.rocksPlaced} Rock(s), " +
                                  $"{wall.requiredBranches - wall.branchesPlaced} Branch(es)";
   
             return;
            }
        }

        // If all walls are completed, clear the text
        gatherText.text = "House built!";
        StartCoroutine(RevealEffect());
        UIManager.instance.housebuild++;
        if (UIManager.instance.housebuild >= UIManager.instance.numberofhousestobuild)
        {
            UIManager.instance.winPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.bounds.Intersects(buildArea.GetComponent<Collider>().bounds))
        {
            string itemName = other.gameObject.name;

            if (itemName.Contains("log"))
            {
                ProcessLogDrop("log", other.gameObject);
            }
            else if (itemName.Contains("Brick"))
            {
                ProcessLogDrop("Brick", other.gameObject);
            }
        }

        if (other.CompareTag("Player"))
        {
            panel.SetActive(true);
            gatherText.gameObject.SetActive(true);


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure it's the player exiting
        {
            if (panel.activeSelf) // Only disable if the panel is currently active
            {
                panel.SetActive(false); // Hide the panel
                gatherText.gameObject.SetActive(false);
            }
        }
    }

    private void ProcessLogDrop(string itemType, GameObject droppedItem)
    {
        foreach (var wall in walls)
        {
            int totalPlaced = wall.branchesPlaced + wall.rocksPlaced;
            int totalRequired = wall.requiredBranches + wall.requiredRocks;

            // Check if this wall needs more items
            if (totalPlaced < totalRequired)
            {
                if (itemType == "log" && wall.branchesPlaced < wall.requiredBranches)
                {
                    PlaceItem(wall, droppedItem, ref wall.branchesPlaced);
                    return;
                }
                else if (itemType == "Brick" && wall.rocksPlaced < wall.requiredRocks)
                {
                    PlaceItem(wall, droppedItem, ref wall.rocksPlaced);
                    return;
                }
                else
                {
                    StartCoroutine(DisplayTemporaryMessage($"Incorrect item! You need {MissingItemMessage(wall)}"));
                    return;
                }
            }
        }
    }

    private void PlaceItem(Wall wall, GameObject droppedItem, ref int itemCounter)
    {
        int totalPlaced = wall.branchesPlaced + wall.rocksPlaced;
        wall.parts[totalPlaced].SetActive(true); // Activate the corresponding wall part
        itemCounter++; // Increment the item counter
        logsGathered++; // Increment total logs gathered

        // Disable the gatherText and panel after placing the first item
        if (totalPlaced == 0)
        {
            //gatherText.gameObject.SetActive(false);
            //panel.SetActive(false);
        }

        PlayerControl.Instance.isHoldingObject = false;
        Destroy(droppedItem); // Destroy the dropped item

        // Update the gather text
        UpdateGatherText();
    }

    private string MissingItemMessage(Wall wall)
    {
        int branchesRemaining = wall.requiredBranches - wall.branchesPlaced;
        int rocksRemaining = wall.requiredRocks - wall.rocksPlaced;

        if (branchesRemaining > 0 && rocksRemaining > 0)
        {
            return $"{branchesRemaining} Branch(es), {rocksRemaining} Rock(s)";
        }
        else if (branchesRemaining > 0 && rocksRemaining > 0)
        {
            return $"{branchesRemaining} Branch(es) and {rocksRemaining} Rock(s)";
        }
        else if (branchesRemaining > 0)
        {
            return $"{branchesRemaining} Branch(es)";
        }
        else if (rocksRemaining > 0)
        {
            return $"{rocksRemaining} Rock(s)";
        }
        return "the correct item";
    }

    private IEnumerator DisplayTemporaryMessage(string message)
    {
        gatherText.gameObject.SetActive(true);
        gatherText.text = message;

        yield return new WaitForSeconds(2f);

        gatherText.gameObject.SetActive(false);
    }

    private void CheckForNewWall()
    {
        foreach (var wall in walls)
        {
            int itemsPlaced = wall.branchesPlaced + wall.rocksPlaced;
            int itemsRequired = wall.requiredBranches + wall.requiredRocks;

            if (itemsPlaced < itemsRequired)
            {
                gatherText.gameObject.SetActive(true);
                //panel.SetActive(true);
                UpdateGatherText();
                return;
            }
        }

        gatherText.text = "House built!";
        gatherText.gameObject.SetActive(true);
        panel.SetActive(false);
        StartCoroutine(RevealEffect());
    }

    IEnumerator RevealEffect()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<RevealEffectHouse>().StartRevealCycle();
    }

}
