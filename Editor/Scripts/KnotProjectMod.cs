using System;
using System.Collections;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
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
            TryContinue();
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


        internal static void SaveState(ModActionState state)
        {
            if (state == null)
                return;

            EditorPrefs.SetString(nameof(ModActionState), JsonUtility.ToJson(state));
        }

        internal static IEnumerator StartEnumerator(ModActionState state)
        {
            if (state == null || string.IsNullOrEmpty(state.PresetAssetGuid))
                yield break;

            var preset =
                AssetDatabase.LoadAssetAtPath<KnotProjectModPreset>(
                    AssetDatabase.GUIDToAssetPath(state.PresetAssetGuid));

            if (preset == null)
                yield break;

            SaveState(state);

            var mods = preset.BuildAllModsChain();
            var startModId = Mathf.Clamp(state.NextModId, 0, mods.Length);
            for (int i = startModId; i < mods.Length; i++)
            {
                if (!(mods[i] is IKnotModAction modAction))
                {
                    state.NextModId++;
                    SaveState(state);
                    continue;
                }

                var actionTitle = $"{CoreName} action [{state.NextModId + 1}]";
                var actionDescription = modAction.BuildDescription();
                var actionProgressId = Progress.Start(actionTitle, actionDescription, Progress.Options.Indefinite | Progress.Options.Sticky);
                Progress.SetTimeDisplayMode(actionProgressId, Progress.TimeDisplayMode.NoTimeShown);

                IKnotModActionResult actionResult = null;
                yield return modAction.Perform((sender, r) =>
                {
                    state.NextModId++;
                    SaveState(state);
                    actionResult = r; 
                    
                    Progress.Finish(actionProgressId, r.IsCompleted ? Progress.Status.Succeeded : Progress.Status.Failed);
                });

                yield return null;
                yield return null;
                
                if ((bool) !actionResult?.IsCompleted)
                    break;
            }
            
            EditorPrefs.DeleteKey(nameof(ModActionState));
            Progress.ShowDetails();
        }

        public static bool TryContinue()
        {
            if (!EditorPrefs.HasKey(nameof(ModActionState)))
                return false;

            var state = JsonUtility.FromJson<ModActionState>(EditorPrefs.GetString(nameof(ModActionState)));
            if (state == null)
                return false;

            Start(state);
            return true;
        }

        public static bool TryStart(KnotProjectModPreset preset)
        {
            if (preset == null || !EditorUtility.IsPersistent(preset))
            {
                Log($"{preset?.name} could not be processed", LogType.Warning, preset);
                return false;
            }

            if (!preset.Any())
                return false;

            var state = new ModActionState(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(preset)));

            if (!EditorUtility.DisplayDialog(CoreName,
                    $"Some {CoreName} actions may lead to unpredictable results. Continue anyways?", "Yes", "No"))
                return false;

            Start(state);
            return true;
        }

        public static void Start(ModActionState actionState)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(StartEnumerator(actionState));
        }


        [Serializable]
        public class ModActionState
        {
            public string PresetAssetGuid;
            public int NextModId;


            public ModActionState() { }

            public ModActionState(string presetAssetGuid)
            {
                PresetAssetGuid = presetAssetGuid;
            }


            public override string ToString()
            {
                return
                    $"{nameof(PresetAssetGuid)}: {PresetAssetGuid}\n" +
                    $"{nameof(NextModId)}: {NextModId}";
            }
        }
    }
}
