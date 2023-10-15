using System;
using System.Collections;
using System.Collections.Generic;
using Knot.ProjectMod.Editor.Attributes;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [CreateAssetMenu(menuName = "KNOT/ProjectMod/Preset", fileName = "ProjectModPreset")]
    public class KnotProjectModPreset : ScriptableObject
    {
        public List<IKnotProjectMod> Mods => _mods ?? (_mods = new List<IKnotProjectMod>());
        [SerializeReference, KnotTypePicker(typeof(IKnotProjectMod))] 
        private List<IKnotProjectMod> _mods;

        /*
        [ContextMenu(nameof(Apply))]
        public void Apply()
        {
            try
            {
                EditorCoroutineUtility.StartCoroutine(ApplyYield(), this);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        IEnumerator ApplyYield()
        {
            var index = 1;
            List<OsConfigProcessor> allProcessors = new List<OsConfigProcessor>();
            Dictionary<OsConfigProcessor, bool> reports = new Dictionary<OsConfigProcessor, bool>();
            foreach (var p in Mods)
            {
                if (p is TemplatesProcessor tp)
                {
                    foreach (var t in tp.Templates)
                        allProcessors.Add(t.Processor);
                }
                else allProcessors.Add(p);
            }
            foreach (var p in allProcessors)
            {
                if (!p.Enabled)
                    continue;

                reports.TryAdd(p, false);

                var t = p.Process();
                while (!t.IsCompleted)
                    yield return null;

                if (t.Result)
                    reports[p] = true;

                index++;
            }

            StringBuilder reportString = new StringBuilder();
            reportString.AppendLine($"{nameof(PrebuildOsConfig)}:");

            index = 0;
            foreach (var r in reports)
            {
                reportString.AppendLine($"{index} - {r.Key.GetType().Name} {(r.Value ? "OK" : "Failed")}");
                index++;
            }

            EditorUtility.ClearProgressBar();
            Debug.Log(reportString.ToString());
        }


        [Serializable]
        public abstract class OsConfigProcessor
        {
            public bool Enabled => _enabled;
            [SerializeField] private bool _enabled = true;

            public abstract Task<bool> Process();
        }

        [Serializable]
        public class TemplatesProcessor : OsConfigProcessor
        {
            public List<PrebuildOsConfigProcessorTemplate> Templates;


            public override Task<bool> Process()
            {
                if (Templates == null)
                    return Task.FromResult(false);

                bool success = true;
                foreach (var t in Templates)
                {
                    var ts = t.Processor.Process();
                    if (ts.Result == false)
                        success = false;
                }

                return Task.FromResult(success);
            }
        }

        [Serializable]
        public class RenameFolderProcessor : OsConfigProcessor
        {
            public string OldName;
            public string NewName;

            public override Task<bool> Process()
            {
                try
                {
                    if (string.IsNullOrEmpty(OldName) || string.IsNullOrEmpty(NewName) || !Directory.Exists(OldName))
                        return Task.FromResult(false);

                    if (Directory.Exists(NewName) && Directory.GetFiles(NewName).Length == 0)
                        Directory.Delete(NewName);
                    else if (Directory.Exists(NewName))
                        return Task.FromResult(false);

                    Directory.Move(OldName, NewName);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
        }

        [Serializable]
        public class SetScriptingDefineSymbols : OsConfigProcessor
        {
            public BuildTargetGroup Target;
            public List<string> Defines;

            public override Task<bool> Process()
            {
                try
                {
                    bool hasDiff = false;
                    PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(Target),
                        out var current);
                    if (current != null)
                    {
                        if (current.Length == Defines.Count)
                        {
                            foreach (var c in current)
                            {
                                if (!Defines.Contains(c))
                                {
                                    hasDiff = true;
                                    break;
                                }
                            }
                        }
                        else hasDiff = true;
                    }

                    if (hasDiff)
                        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(Target), Defines.ToArray());

                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
        }

        [Serializable]
        public class SetScriptingBackend : OsConfigProcessor
        {
            public BuildTargetGroup Target;
            public ScriptingImplementation Implementation;


            public override Task<bool> Process()
            {
                try
                {
                    PlayerSettings.SetScriptingBackend(NamedBuildTarget.FromBuildTargetGroup(Target), Implementation);
                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
        }

        [Serializable]
        public class RemovePackages : OsConfigProcessor
        {
            public List<string> PackageNames;

            public override async Task<bool> Process()
            {
                try
                {
                    bool success = false;

                    foreach (var packageName in PackageNames)
                    {
                        if (string.IsNullOrEmpty(packageName))
                            continue;

                        var request = Client.Remove(packageName);
                        while (!request.IsCompleted)
                            await Task.Delay(1);

                        if (request.Status == StatusCode.Success)
                            success = true;
                    }

                    Client.Resolve();
                    return success;
                }
                catch
                {
                    return false;
                }
            }
        }

        [Serializable]
        public class AddPackages : OsConfigProcessor
        {
            public List<string> PackageNames;

            public override async Task<bool> Process()
            {
                try
                {
                    bool success = false;

                    foreach (var packageName in PackageNames)
                    {
                        if (string.IsNullOrEmpty(packageName))
                            continue;

                        var request = Client.Add(packageName);
                        while (!request.IsCompleted)
                            await Task.Delay(1);

                        if (request.Status == StatusCode.Success)
                            success = true;
                    }

                    Client.Resolve();
                    return success;
                }
                catch
                {
                    return false;
                }
            }
        }

        [Serializable]
        public class SetDefaultAppFeaturesAsset : OsConfigProcessor
        {
            public AppFeaturesAsset DefaultAppFeaturesAsset;

            public override Task<bool> Process()
            {
                try
                {
                    App.Resources.DefaultAppFeatures = DefaultAppFeaturesAsset;

                    EditorUtility.SetDirty(App.Resources);
                    AssetDatabase.SaveAssetIfDirty(App.Resources);

                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
        }

        [Serializable]
        public class RenameFileProcessor : OsConfigProcessor
        {
            public string OldName;
            public string NewName;

            public override Task<bool> Process()
            {
                try
                {
                    if (string.IsNullOrEmpty(OldName) || string.IsNullOrEmpty(NewName) || !File.Exists(OldName))
                        return Task.FromResult(false);

                    File.Move(OldName, NewName);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
        }
        */
    }
}
