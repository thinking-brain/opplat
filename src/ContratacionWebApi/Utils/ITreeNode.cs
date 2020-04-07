using System.Collections.Generic;

namespace ContratacionWebApi.Utils
{
    public interface ITreeNode
    {
        ITreeNode GetParent();

        IEnumerable<ITreeNode> GetChildrens();
    }
}