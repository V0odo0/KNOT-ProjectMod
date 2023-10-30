using System;
using System.Collections;
using System.Collections.Generic;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Set Product Info")]
    public class KnotSetProductInfoModAction : KnotModActionBase
    {
        [field: SerializeField] public string CompanyName { get; set; } = "My Company Name";
        [field: SerializeField] public string ProductName { get; set; } = "My Product Name";


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;

            yield break;
        }
    }
}
