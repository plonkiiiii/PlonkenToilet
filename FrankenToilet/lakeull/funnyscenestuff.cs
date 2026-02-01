/*using FrankenToilet.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FrankenToilet.lakeull;


[EntryPoint]
public class FunnySceneStuffs
{
    private static GameObject[] packedObjects = [];
    private static AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lakeullsfunnybundle")); // change name of "bundled" to the file name of the bundle
    private static string bundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lakeullsfunnybundle");
    private static GameObject osakaSkyboxObject;

    [EntryPoint]
    public static void Awake()
    {
        SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
    }
    public static void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        if(SceneHelper.CurrentScene == "Level 1-1" || SceneHelper.CurrentScene == "Level 1-2" || SceneHelper.CurrentScene == "Level 1-3" || SceneHelper.CurrentScene == "Level 1-4" || SceneHelper.CurrentScene == "Level 1-E")
        {
            packedObjects = bundle.LoadAllAssets<GameObject>();
            foreach (GameObject gameObject in packedObjects)
            {
                // grabs the name of the specific item I WANT IT!!!!!!!!!!!
                //LogHelper.LogInfo($"{gameObject.name}");
                
                if ($"{gameObject.name}" == "osakaskybox")
                {
                    osakaSkyboxObject = GameObject.Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                }
                LogHelper.LogInfo($"{gameObject.name}");
            }
            Vector3 targetPos = GameObject.Find("LimboSkybox").transform.position;
            osakaSkyboxObject.transform.position = targetPos;
        }
    }
}*/
