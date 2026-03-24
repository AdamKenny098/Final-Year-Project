using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ClassSystem.Classes selectedClass;

    [Header("Scene Names")]
    public string loadingSceneName = "LoadingScreen";
    public AsyncOperation currentLoadOperation;
    public string currentTargetScene;

    [Header("Optional: block double-clicks")]
    public bool isLoading;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetClass(ClassSystem.Classes newClass)
    {
        selectedClass = newClass;
    }

    public void LoadTarget()
    {
        if (isLoading) return;
        StartCoroutine(LoadFlow(currentTargetScene));
    }

    public void LoadByName(string sceneName)
    {
        if (isLoading) return;
        StartCoroutine(LoadFlow(sceneName));
    }

    IEnumerator LoadFlow(string sceneToLoad)
    {
        isLoading = true;

        currentTargetScene = sceneToLoad;

        if (!string.IsNullOrEmpty(loadingSceneName))
            yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);

        currentLoadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        if (currentLoadOperation == null)
        {
            isLoading = false;
            yield break;
        }

        currentLoadOperation.allowSceneActivation = false;

        while (currentLoadOperation.progress < 0.9f)
            yield return null;

        currentLoadOperation.allowSceneActivation = true;

        while (!currentLoadOperation.isDone)
            yield return null;

        currentLoadOperation = null;
        isLoading = false;
    }
}