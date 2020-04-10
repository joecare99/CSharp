using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSFreeVision.Base
{
    public partial class Group : View , IContainer
    {
        private Dictionary<string,View> _Components = new Dictionary<string, View>();

        public Group()
        {
            InitializeComponent();
        }

        public Group(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public ComponentCollection Components => (ComponentCollection)_Components;

        public void Add(IComponent component) => _Components.Add(null,(View)component);

        public void Add(IComponent component, string name)
        {
            _Components.Add(name,(View)component);
        }

        public void Remove(IComponent component) => _Components.Remove(null);
    }
}
