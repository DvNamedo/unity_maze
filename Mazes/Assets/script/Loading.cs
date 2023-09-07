using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField]
    Image prograssBar;

    static string nextSceneName;

    public static void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(sceneLoadProcess(nextSceneName));
    }

    IEnumerator sceneLoadProcess(string nextScene) {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float t = 0f;
        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.9f)
            {
                prograssBar.fillAmount = op.progress;
            }
            else
            {
                t += Time.unscaledTime;
                prograssBar.fillAmount = Mathf.Lerp(0.9f, 1.0f, t);
                if(prograssBar.fillAmount >= 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
