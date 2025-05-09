using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    public Vector3 playerPos;

    public void Transition(string targetScene)
    {
        StartCoroutine(OnTransition(targetScene));
    }

    public void UnloadScene(string scneName)
    {
        StartCoroutine(UnloadSceneIEn(scneName));
    }

    private IEnumerator UnloadSceneIEn(string scneName)
    {
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    private IEnumerator OnTransition(string sceneName)
    {
        yield return LoadSceneSetActive(sceneName);
        EventHandler.CallAfterSceneLoad();
        EventHandler.CallPlayerMove(playerPos);
    }

    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene scne = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(scne);
    }
}
