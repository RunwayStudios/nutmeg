using System.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nutmeg.Runtime.Core.SceneManagement
{
    //TODO just for testing. REWRITE!
    public class NutmegSceneManager : NetworkBehaviour
    {
        public string scene;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            StartCoroutine(LoadSceneAsyncEnumerator(scene));
        }

        public IEnumerator LoadSceneAsyncEnumerator(string scene)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

            while (!op.isDone)
                yield return null;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            //NetworkManager.SceneManager.VerifySceneBeforeLoading = (_, _, _) => false;
            //if (NetworkManager.IsClient)
            //    NetworkManager.SceneManager.VerifySceneBeforeLoading += MainMenuSceneValidation;
            NetworkManager.SceneManager.VerifySceneBeforeLoading += CoreSceneValidation;
        }

        public static void MoveGameObjectToScene(GameObject obj, Scene scene) =>
            SceneManager.MoveGameObjectToScene(obj, scene);

        public static void MoveGameObjectToScene(GameObject obj, string sceneName) =>
            SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(sceneName));

        private bool CoreSceneValidation(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode) =>
            sceneName != "Core";

        private bool MainMenuSceneValidation(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
        {
            return true;
        }
    }
}