using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GlobeSceneManager : MonoBehaviour
{
    public void LoadNetworkScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Network_ObjectManipulation", LoadSceneMode.Single);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("Globe_ObjectManipulation 1", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
