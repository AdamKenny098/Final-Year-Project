using System.Collections.Generic;
using UnityEngine;

public class PlayerTorchTrigger : MonoBehaviour
{
    private List<TorchLightController> activeTorches = new List<TorchLightController>();

    void OnTriggerEnter(Collider other)
    {
        TorchLightController torch = other.GetComponentInParent<TorchLightController>();

        if (torch == null)
            return;

        if (!activeTorches.Contains(torch))
        {
            activeTorches.Add(torch);
            torch.SetTorchActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        TorchLightController torch = other.GetComponentInParent<TorchLightController>();

        if (torch == null)
            return;

        if (activeTorches.Contains(torch))
        {
            activeTorches.Remove(torch);
            torch.SetTorchActive(false);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < activeTorches.Count; i++)
        {
            if (activeTorches[i] != null)
                activeTorches[i].SetTorchActive(false);
        }

        activeTorches.Clear();
    }
}