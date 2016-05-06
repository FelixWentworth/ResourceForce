using UnityEngine;
using System.Collections;

public class FTUEController : MonoBehaviour {

    /// <summary>
    /// this class will handle the FTUE during the game, this script will only be needed the first time the game is played
    /// </summary>
    public enum Stages { ShowOfficers, ShowResources, ShowIncidents, ShowButtons, ShowCitizens};
    public Stages m_stages;
}
