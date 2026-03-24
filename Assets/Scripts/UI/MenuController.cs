using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void SelectClass(int classIndex)
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.SetClass((ClassSystem.Classes)classIndex);
    }

    public void LoadScene()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = GameObject.FindObjectOfType<GameManager>();
        }

        GameManager.Instance.LoadTarget();
    }
}
