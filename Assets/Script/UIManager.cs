using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject winPanel;
    public int numberofhousestobuild;
    public int housebuild;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

}
