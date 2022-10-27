using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nutmeg.Runtime.Core.SceneManager
{
    //TODO just for testing. REWRITE!
    public class SceneManager : NetworkBehaviour
    {
        public string[] scenes;

        private void Start()
        {
            //DontDestroyOnLoad(this);

            foreach (var scene in scenes)
            {
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }

        public override void OnNetworkSpawn()
        {
            NetworkManager.SceneManager.VerifySceneBeforeLoading = (_, _, _) => false;
            //NetworkManager.SceneManager.VerifySceneBeforeLoading += MainMenuSceneValidation;
            //NetworkManager.SceneManager.VerifySceneBeforeLoading += CoreSceneValidation;
        }

        private bool CoreSceneValidation(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode) => sceneName != "Core";

        private bool MainMenuSceneValidation(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode) => !scenes.Contains(sceneName);
    }
}