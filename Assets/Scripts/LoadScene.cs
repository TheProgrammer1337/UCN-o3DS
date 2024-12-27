using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class LoadScene : MonoBehaviour
{
    public string SceneToLoad;
    void Start()
    {
        Resources.UnloadUnusedAssets();
		System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        SceneManager.LoadScene(SceneToLoad);
    }
}