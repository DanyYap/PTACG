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
    public TMP_Text gatherText; // Text to display the message
    public Transform buildArea; // Reference to the plane where the house is built
    private int totalLogsNeeded; // Total logs required to build the entire house
    private int logsGathered = 0;
    public GameObject panel;

    private void Start()
    {
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
                                  $"{wall.requiredRocks - wall.rocksPlaced} Rock(s) and " +
                                  $"{wall.requiredBranches - wall.branchesPlaced} Branch(es)";
                return;
            }
        }

        // If all walls are completed, clear the text
        gatherText.text = "House built!";
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
    }
    
    //Here need sync
    private void ProcessLogDrop(string itemType, GameObject droppedItem)
    {
        foreach (var wall in walls)
        {
            int totalPlaced = wall.branchesPlaced + wall.rocksPlaced;
            int totalRequired = wall.requiredBranches + wall.requiredRocks;

            if (totalPlaced < totalRequired)
            {
                if (itemType == "log" && wall.branchesPlaced < wall.requiredBranches)
                {
                    // Trigger the networked update for branches
                    PhotonView photonView = PhotonView.Get(this);
                    int wallIndex = Array.IndexOf(walls, wall);
                    photonView.RPC("AddBranch", RpcTarget.AllBuffered, wallIndex, totalPlaced);

                    PlayerControl.Instance.isHoldingObject = false;
                    Destroy(droppedItem); // Destroy the local object

                    return;
                }
                else if (itemType == "Brick" && wall.rocksPlaced < wall.requiredRocks)
                {
                    // Trigger the networked update for rocks
                    PhotonView photonView = PhotonView.Get(this);
                    int wallIndex = Array.IndexOf(walls, wall);
                    photonView.RPC("AddRock", RpcTarget.AllBuffered, wallIndex, totalPlaced);

                    PlayerControl.Instance.isHoldingObject = false;
                    Destroy(droppedItem); // Destroy the local object

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

    [PunRPC]
    private void AddBranch(int wallIndex, int partIndex)
    {
        Wall wall = walls[wallIndex];
        wall.parts[partIndex].SetActive(true); // Activate the corresponding wall part
        wall.branchesPlaced++; // Increment the branches placed
        logsGathered++; // Increment total logs gathered

        // Update the UI and check for new walls
        UpdateGatherText();
        CheckForNewWall();
    }
    
    [PunRPC]
    private void AddRock(int wallIndex, int partIndex)
    {
        Wall wall = walls[wallIndex];
        wall.parts[partIndex].SetActive(true); // Activate the corresponding wall part
        wall.rocksPlaced++; // Increment the rocks placed
        logsGathered++; // Increment total logs gathered

        // Update the UI and check for new walls
        UpdateGatherText();
        CheckForNewWall();
    }
    
    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     PhotonView photonView = PhotonView.Get(this);
    //     for (int i = 0; i < walls.Length; i++)
    //     {
    //         photonView.RPC("SyncWallState", RpcTarget.AllBuffered, i, walls[i].branchesPlaced, walls[i].rocksPlaced);
    //     }
    // }

    [PunRPC]
    private void SyncWallState(int wallIndex, int branches, int rocks)
    {
        Wall wall = walls[wallIndex];
        wall.branchesPlaced = branches;
        wall.rocksPlaced = rocks;

        // Update the visual state of the wall
        for (int i = 0; i < branches + rocks; i++)
        {
            wall.parts[i].SetActive(true);
        }
    }

    private string MissingItemMessage(Wall wall)
    {
        int branchesRemaining = wall.requiredBranches - wall.branchesPlaced;
        int rocksRemaining = wall.requiredRocks - wall.rocksPlaced;

        if (branchesRemaining > 0 && rocksRemaining > 0)
        {
            return $"{rocksRemaining} Rock(s) and {branchesRemaining} Branch(es)";
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
        gatherText.gameObject.SetActive(true); // Temporarily show the text for the message
        panel.SetActive(true); // Keep panel hidden
        gatherText.text = message;

        yield return new WaitForSeconds(2f); // Show the message for 2 seconds

        gatherText.gameObject.SetActive(false); // Hide the text again
        panel.SetActive(false); // Keep panel hidden

    }

    private void CheckForNewWall()
    {
        foreach (var wall in walls)
        {
            int itemsPlaced = wall.branchesPlaced + wall.rocksPlaced;
            int itemsRequired = wall.requiredBranches + wall.requiredRocks;

            // If a new wall needs resources, enable the text and panel
            if (itemsPlaced < itemsRequired)
            {
                gatherText.gameObject.SetActive(true);
                panel.SetActive(true);
                UpdateGatherText();
                return;
            }
        }

        // If all walls are built, show the "House built!" message and trigger the reveal effect
        gatherText.text = "House built!";
        gatherText.gameObject.SetActive(true);
        panel.SetActive(false); // Hide panel after the house is built
        StartCoroutine(RevealEffect());
    }

    IEnumerator RevealEffect()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<RevealEffectHouse>().StartRevealCycle();
    }

}
