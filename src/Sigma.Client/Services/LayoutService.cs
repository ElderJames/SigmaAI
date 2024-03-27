using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Client.Services
{
    public class LayoutService
    {
        public event EventHandler<bool> OnSiderCollapsedChanged;

        public void ChangeSiderCollapsed(bool isCollapsed)
        {
            OnSiderCollapsedChanged?.Invoke(this, isCollapsed);
        }
    }
}
