using System;
using System.Collections;
using Knot.Core;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Set Product Info", MenuCustomName = BuiltinModActionPath + "Set Product Info")]
    public class KnotSetProductInfoModAction : KnotModActionBase
    {
        [field: SerializeField] public string CompanyName { get; set; } = "My Company Name";
        [field: SerializeField] public string ProductName { get; set; } = "My Product Name";


        public override string GetDescription() => $"Set Company Name \"{CompanyName}\" & Product Name \"{ProductName}\"";


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());

            yield break;
        }
    }
}
