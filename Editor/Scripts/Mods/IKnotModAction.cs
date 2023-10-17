using System;
using System.Collections;

namespace Knot.ProjectMod.Editor
{
    public interface IKnotModAction : IKnotMod
    {
        string BuildDescription();

        IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed);
    }
}
