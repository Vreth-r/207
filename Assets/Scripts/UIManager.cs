using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Prefab to Instantiate")]
    public GameObject prefab;

    public GameObject AddPrefab()
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefab or parent is missing.");
            return null;
        }

        GameObject instance = Instantiate(prefab, gameObject.transform);
        instance.transform.localPosition = Vector3.zero;
        //instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        return instance;
    }

    public void RemovePrefab(string childName = "")
    {
        Transform targetChild = null;

        if (string.IsNullOrEmpty(childName))
        {
            if (gameObject.transform.childCount > 0)
                targetChild = gameObject.transform.GetChild(0);
        }
        else
        {
            foreach (Transform child in gameObject.transform)
            {
                if (child.name.Contains(childName))
                {
                    targetChild = child;
                    break;
                }
            }
        }

        if (targetChild != null)
        {
            Destroy(targetChild.gameObject);
        }
        else
        {
            Debug.Log("No child prefab found to remove.");
        }
    }

    public void RemoveAll()
    {
        // Create a temporary list to store children to avoid issues with modifying the collection during iteration
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in transform)
        {
            childrenToDestroy.Add(child.gameObject);
        }

        // Destroy each child GameObject
        foreach (GameObject child in childrenToDestroy)
        {
            Destroy(child);
        }
    }
}
