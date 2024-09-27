using System;
using System.Collections;
using System.Linq;
using Knot.Core;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Add Build Scene", MenuCustomName = BuiltinModActionPath + "Add Build Scene", Order = -301)]
    public class KnotAddBuildSceneModAction : KnotModActionBase
    {
        public SceneAsset Scene
        {
            get => _scene;
            set => _scene = value;
        }
        [SerializeField] private SceneAsset _scene;

        public int InsertAtIndex
        {
            get => _insertAtIndex;
            set => _insertAtIndex = value;
        }
        [SerializeField] private int _insertAtIndex;


        public override string BuildDescription()
        {
            if (Scene == null)
                return "Missing Scene reference";

            return $"Add {Scene.name} to Build at index {InsertAtIndex}";
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
            if (curScenes.Any(s => s.path == targetScenePath))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Completed($"Scene {Scene.name} is already in Build"));
                yield break;
            }

            var insertAtIndex = Mathf.Clamp(InsertAtIndex, 0, curScenes.Count);
            curScenes.Insert(insertAtIndex, new EditorBuildSettingsScene(targetScenePath, true));
            EditorBuildSettings.scenes = curScenes.ToArray();
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
        }
    }
}
