using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeviceLocation : MonoBehaviour {

    public Location loc;
    public GameObject startScreen;

    public void SetLocation(Dropdown dropdown)
    {
        int value = dropdown.value;

        //check a location has been set
        if (value == 0)
            return;

        loc.SetFilePath(dropdown.value - 1);

        startScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
