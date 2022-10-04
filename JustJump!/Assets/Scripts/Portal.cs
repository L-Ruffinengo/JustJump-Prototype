using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField]
    float secondsToLoad = 5f;
    private enum SceneSelector{
        Level01,
        Level02,
        Level03,
    }
    [SerializeField]
    private SceneSelector scene;

    private string selectedScene;

    private void Awake() {
        SetScene();
    }

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            Invoke("LoadScene", secondsToLoad);
        }
    }

    private void SetScene(){
        switch (scene)
        {
            case SceneSelector.Level01:
                selectedScene = "Level01";
                break;

            case SceneSelector.Level02:
                selectedScene = "Level02";
                break;

            case SceneSelector.Level03:
                selectedScene = "Level03";
                break;
        }
    }
    private void LoadScene(){
        SceneManager.LoadScene(selectedScene);
    }
}
