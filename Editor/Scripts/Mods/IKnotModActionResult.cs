using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    public interface IKnotModActionResult
    {
        bool IsCompleted { get; }
        string ResultMessage { get; }
    }
}
