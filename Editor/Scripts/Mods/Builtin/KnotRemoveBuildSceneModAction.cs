using System;
using System.Collections;
using System.Linq;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Remove Build Scene", MenuCustomName = BuiltinModActionPath + "Remove Build Scene", Order = -300)]
    public class KnotRemoveBuildSceneModAction : KnotModActionBase
    {
        public SceneAsset Scene
        {
            get => _scene;
            set => _scene = value;
        }
        [SerializeField] private SceneAsset _scene;


        public override string BuildDescription()
        {
            if (Scene == null)
                return "Missing Scene reference";

            return $"Remove {Scene.name}";
        }

        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (Scene == null)
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed("Missing Scene reference"));
                yield break;
            }

            var targetScenePath = AssetDatabase.GetAssetPath(Scene);
            var curScenes = EditorBuildSettings.scenes.Where(s => s.enabled).ToList();
            var sceneToRemove = curScenes.FirstOrDefault(s => s.path == targetScenePath);
            if (sceneToRemove == null)
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Completed($"Scene {Scene.name} was already removed"));
                yield break;
            }

            curScenes.Remove(sceneToRemove);
            EditorBuildSettings.scenes = curScenes.ToArray();
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
        }
    }
}
