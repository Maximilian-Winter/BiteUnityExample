using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine("AssetBundles", "mod_scene"));
        string[] scenePath = bundle.GetAllScenePaths();
        SceneManager.LoadScene(scenePath[0]);
    }
    
}
