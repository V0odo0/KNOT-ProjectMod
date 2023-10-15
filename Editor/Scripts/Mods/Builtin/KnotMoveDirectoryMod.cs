using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    public class KnotMoveDirectoryMod : IKnotProjectMod
    {
        public string OldName;
        public string NewName;

        void Foo()
        {

        }
        
        /*public override Task<bool> Process()
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
            }*/
    }
}
