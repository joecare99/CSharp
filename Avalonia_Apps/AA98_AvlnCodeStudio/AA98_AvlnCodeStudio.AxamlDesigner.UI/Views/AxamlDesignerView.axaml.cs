using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;

public partial class AxamlDesignerView : UserControl
{
    public AxamlDesignerView() => InitializeComponent();

    public Task<byte[]> CapturePreviewPngAsync(CancellationToken cancellationToken = default) =>
        SurfaceView.CapturePreviewPngAsync(cancellationToken);
}
