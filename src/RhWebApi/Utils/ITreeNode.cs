using System.Collections.Generic;

namespace RhWebApi.Utils
{
    public interface ITreeNode
    {
        ITreeNode GetParent();

        IEnumerable<ITreeNode> GetChildrens();
    }
}