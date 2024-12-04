using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject winPanel;
    public TextMeshProUGUI finalTimeText;
    public int numberofhousestobuild;
    public int housebuild;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ShowWinScreen(string finalTime)
    {
        winPanel.SetActive(true);

        if (finalTimeText != null) // Check if the text element is assigned
        {
            finalTimeText.text = "Final Time: " + finalTime;
        }
    }

}
