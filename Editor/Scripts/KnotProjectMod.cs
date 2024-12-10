using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Core.Editor;
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

        internal static void CombineMods(List<IKnotMod> mods)
        {
            var combinedModActions = mods.OfType<IKnotCombinedModAction>().GroupBy(a => a.GetType()).
                ToDictionary(a => a.Key, a => a.ToList());

            foreach (var modType in combinedModActions.Keys)
            {
                var lastMod = combinedModActions[modType].LastOrDefault();
                if (lastMod == null)
                    continue;

                var lastModActionIdx = mods.IndexOf(lastMod);
                var finalModActions = lastMod.Combine(combinedModActions[modType]);
                mods.InsertRange(lastModActionIdx, finalModActions);
                mods.RemoveAll(m => combinedModActions[modType].Contains(m) && mods.IndexOf(m) < lastModActionIdx);
            }
        }

        internal static IEnumerator StartEnumerator(ModActionState state)
        {
            if (state == null || string.IsNullOrEmpty(state.PresetAssetGuid))
                yield break;

            var preset = state.GetPreset();
            if (preset == null)
                yield break;

            SaveState(state);

            var mods = preset.BuildAllModsChain();
            CombineMods(mods);

            var startModId = Mathf.Clamp(state.NextModId, 0, mods.Count);
            for (int i = startModId; i < mods.Count; i++)
            {
                if (!(mods[i] is IKnotModAction modAction))
                {
                    state.NextModId++;
                    SaveState(state);
                    continue;
                }

                var actionTitle = $"{i + 1}. {modAction.GetType().GetManagedReferenceTypeName()}";
                var actionDescription = modAction is IKnotModDescriptor descriptor ? descriptor.GetDescription() : "No description";
                var actionProgressId = Progress.Start(actionTitle, actionDescription, Progress.Options.Indefinite | Progress.Options.Sticky);
                Progress.SetTimeDisplayMode(actionProgressId, Progress.TimeDisplayMode.NoTimeShown);
                
                IKnotModActionResult actionResult = null;

                yield return modAction.Perform((sender, r) =>
                {
                    state.NextModId++;
                    SaveState(state);
                    actionResult = r; 
                    
                    if (!r.IsCompleted && !string.IsNullOrEmpty(r.ResultMessage))
                        Progress.SetDescription(actionProgressId, r.ResultMessage);
                    Progress.Finish(actionProgressId, r.IsCompleted ? Progress.Status.Succeeded : Progress.Status.Failed);
                });

                yield return null;
                yield return null;

                if (actionResult == null)
                {
                    Progress.SetDescription(actionProgressId, "No action result provided");
                    Progress.Finish(actionProgressId, Progress.Status.Failed);
                }
            }
            
            EditorPrefs.DeleteKey(nameof(ModActionState));
            Progress.ShowDetails();
        }

        internal static void Start(ModActionState actionState)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(StartEnumerator(actionState));
        }

        internal static bool TryContinue()
        {
            if (!EditorPrefs.HasKey(nameof(ModActionState)))
                return false;

            var state = JsonUtility.FromJson<ModActionState>(EditorPrefs.GetString(nameof(ModActionState)));
            if (state == null)
                return false;

            Start(state);
            return true;
        }


        public static bool Start(KnotProjectModPreset preset)
        {
            if (preset == null || !EditorUtility.IsPersistent(preset))
            {
                Log($"{preset?.name} could not be processed", LogType.Warning, preset);
                return false;
            }

            if (!preset.Any())
                return false;

            var state = new ModActionState(preset);

            Start(state);
            return true;
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

            public ModActionState(KnotProjectModPreset preset)
            {
                PresetAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(preset));
            }

            public KnotProjectModPreset GetPreset()
            {
                return AssetDatabase.LoadAssetAtPath<KnotProjectModPreset>(
                    AssetDatabase.GUIDToAssetPath(PresetAssetGuid));
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
