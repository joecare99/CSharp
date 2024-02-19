using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_39_MultiModelTest.Models
{
    public partial class ScopedModel : NotificationObjectCT, IScopedModel
    {
        private ISystemModel? _parent;
        public string Name { get; private set; } = "";

        public string Description { get; private set; }= "";

        public ISystemModel? parent { get => _parent; set => SetParent(value); }

        public Guid Id { get; }= Guid.NewGuid();

        public IServiceScope? Scope { get; set; }

        [ObservableProperty]
        private int _ICommonValue;

        private void SetParent(ISystemModel? value)
        {
            if (_parent == value || _parent != null)
                return;
            _parent = value;
            Name = $"ScopedModel_{_parent!.ScModels.Count}";
            Description = $"ScopedModel_{_parent.ScModels.Count} Description";
        }

    }
}
