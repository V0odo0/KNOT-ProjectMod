using System.Collections.Generic;

namespace Knot.ProjectMod.Editor
{
    public interface IKnotCombinedModAction : IKnotModAction
    {
        IEnumerable<IKnotModAction> Combine(IEnumerable<IKnotCombinedModAction> orderedModActions);
    }
}
