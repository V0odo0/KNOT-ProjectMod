using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.ProjectMod.Editor
{
    public static class KnotProjectMod
    {
        internal const string CoreName = "KNOT ProjectMod";
        internal const string CorePath = "KNOT/ProjectMod/";
        
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            Debug.Log("Init on load");
            AssemblyReloadEvents.beforeAssemblyReload += AssemblyReloadEventsOnbeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEventsOnafterAssemblyReload;
        }

        private static void AssemblyReloadEventsOnafterAssemblyReload()
        {
            Debug.Log(nameof(AssemblyReloadEventsOnafterAssemblyReload));
        }

        private static void AssemblyReloadEventsOnbeforeAssemblyReload()
        {
            Debug.Log(nameof(AssemblyReloadEventsOnbeforeAssemblyReload));
        }

        internal static void Log(object message, LogType type, Object context = null)
        {
            message = $"{CoreName}: {message}";

            switch (type)
            {
                default:
                    Debug.Log(message, context);
                    break;
                case LogType.Error:
                    Debug.LogError(message, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message, context);
                    break;
            }
        }


        public static void ProcessPreset(KnotProjectModPreset preset)
        {
            if (preset == null || !EditorUtility.IsPersistent(preset))
            {
                Log($"{preset?.name} could not be processed", LogType.Warning, preset);
                return;
            }

            if (!preset.Any())
                return;

            var state = new ProcessModState(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(preset)));

        }


        [DidReloadScripts]
        public static void Did()
        {
            Debug.Log("DID");
        }


        [Serializable]
        public class ProcessModState
        {
            public string PresetAssetGuid;
            public DateTime Timestamp;
            public int ModId;


            public ProcessModState() { }

            public ProcessModState(string presetAssetGuid)
            {
                PresetAssetGuid = presetAssetGuid;
            }
        }
    }
}
