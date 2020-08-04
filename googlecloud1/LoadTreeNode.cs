using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace googlecloud1
{
    class LoadTreeNode : LoadFolder
    {
        public TreeNode node { get; set; }
        public LoadTreeNode(string id, DriveInfo drive, List<DriveInfo> driveinfo ,TreeNode node)
            : base(id, drive, driveinfo)
        {
            this.node = node;
        }
    }
}
