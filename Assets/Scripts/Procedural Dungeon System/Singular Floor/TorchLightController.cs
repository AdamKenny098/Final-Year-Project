using UnityEngine;

public class TorchLightController : MonoBehaviour
{
    [SerializeField] private GameObject fireRoot;

    public void SetTorchActive(bool state)
    {
        if (fireRoot != null)
            fireRoot.SetActive(state);
    }
}