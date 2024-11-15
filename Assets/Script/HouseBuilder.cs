using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HouseBuilder : MonoBehaviour
{
    public GameObject[] walls; // Assign the 4 walls and roof in the Inspector
    public TMP_Text gatherText; // Text to display the message
    public Transform buildArea; // Reference to the plane where the house is built
    public int totalLogsNeeded = 4; // Total logs required to build the house
    private int logsGathered = 0;

    private void Start()
    {
        UpdateGatherText();
    }

    private void UpdateGatherText()
    {
        int logsRemaining = totalLogsNeeded - logsGathered;
        gatherText.text = $"Gather {logsRemaining} more logs to build the house";
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the dropped object is a "Log" and is within the build area
        //if (other.CompareTag("Log") && other.bounds.Intersects(buildArea.GetComponent<Collider>().bounds))
        //{
        //    // Process the log drop and update the house
        //    ProcessLogDrop();
        //    Destroy(other.gameObject); // Optionally destroy the log after use
        //} 
        if (other.gameObject.name.Equals("Branch_01(Clone)") || other.gameObject.name.Contains("Branch") && other.bounds.Intersects(buildArea.GetComponent<Collider>().bounds))
        {
            // Process the log drop and update the house
            ProcessLogDrop();
            PlayerControl.Instance.isHoldingObject = false;
            Destroy(other.gameObject); // Optionally destroy the log after use
           
        }
    }

    private void ProcessLogDrop()
    {
        if (logsGathered < walls.Length)
        {
            walls[logsGathered].SetActive(true); // Enable the next wall
            logsGathered++;
            UpdateGatherText();

            if (logsGathered >= totalLogsNeeded)
            {
                gatherText.text = "House built!";
                StartCoroutine(ReavelEffect());
                this.gameObject.GetComponent<Collider>().enabled = false;

            }
        }
    }

    IEnumerator ReavelEffect()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<RevealEffectHouse>().StartRevealCycle();
    }
}
