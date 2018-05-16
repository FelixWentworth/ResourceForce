using UnityEditor;

namespace Editor.Tools
{
    public class PlayerPrefs
    { 
        [MenuItem("Tools/Player Prefs/Delete All")]
        public static void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }
    }
}
