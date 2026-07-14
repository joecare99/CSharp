using GenFreeWin.Services;
using GenFreeWin.Services.Interfaces;
using GenFree.Interfaces;

namespace GenFreeWin.ViewModels;

public partial class NamenSuchViewModel
{
    private IDocumentOutputService _documentOutputService;
    private INamenSuchDocumentComposer _documentComposer;

    private IDocumentOutputService DocumentOutputService => _documentOutputService ??= new DocumentOutputService();
    private INamenSuchDocumentComposer DocumentComposer => _documentComposer ??= new NamenSuchDocumentComposer();

    public void Retweg3(IDocument Display)
    {
        DocumentOutputService.Retweg3(Display);
    }

    public bool TrimEnd(IDocument display)
    {
        return DocumentOutputService.TrimEnd(display);
    }
}
