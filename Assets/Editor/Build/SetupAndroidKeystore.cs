using System.IO;
using GameWork.Unity.Editor.Build;
using UnityEditor;
using UnityEngine;
using EventType = GameWork.Unity.Editor.Build.EventType;

namespace Editor.Build
{
    public static class SetupAndrodKeystore
    {
        [MenuItem("Tools/Build/Android/Setup Keystore")]
        [BuildEvent(EventType.Pre)]
        public static void Setup()
        {
            PlayerSettings.Android.keystoreName = Application.dataPath + "/rfkeystore.keystore";
            PlayerSettings.Android.keystorePass = "kpsLsTorAygEeN";

            PlayerSettings.Android.keyaliasName = "releasebuildrf";
            PlayerSettings.Android.keyaliasPass = "kpsLsTorAygEeN";
        }
    }
}