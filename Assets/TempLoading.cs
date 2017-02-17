using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempLoading : MonoBehaviour
{

    private static GameObject _loadingScreen;

    void Start()
    {
        _loadingScreen = transform.GetChild(0).gameObject;
        _loadingScreen.SetActive(false);
    }

    public static void Show()
    {
        _loadingScreen.SetActive(true);
    }

    public static void Hide()
    {
        _loadingScreen.SetActive(false);
    }
}
