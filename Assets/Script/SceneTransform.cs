using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransform : MonoBehaviour
{
    public void ToLevel1()
    {
        SceneManager.LoadScene("Tutorial Level");
    }

    public void ToLevel2()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void ToLevel3()
    {
        SceneManager.LoadScene("Level 2");
    }
    
    
}
