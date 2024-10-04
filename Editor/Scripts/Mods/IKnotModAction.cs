using System;
using System.Collections;

namespace Knot.ProjectMod.Editor
{
    public interface IKnotModAction : IKnotMod
    {
        IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed);
    }
}
