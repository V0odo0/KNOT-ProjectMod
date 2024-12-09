using System;
using System.Collections;
using Knot.Core;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Set Product Info", MenuCustomName = BuiltinModActionPath + "Set Product Info")]
    public class KnotSetProductInfoModAction : KnotModActionBase
    {
        [field: SerializeField] public string CompanyName { get; set; } = "My Company Name";
        [field: SerializeField] public string ProductName { get; set; } = "My Product Name";
        [field: SerializeField] public Sprite DefaultIcon { get; set; }



        public override string GetDescription() => 
            $"Set Company Name \"{CompanyName}\" & Product Name \"{ProductName}\" & {nameof(DefaultIcon)} {(DefaultIcon == null ? "null" : DefaultIcon.name)}";


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new []{ DefaultIcon == null ? null : DefaultIcon.texture }, IconKind.Any);
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());

            yield break;
        }
    }
}
