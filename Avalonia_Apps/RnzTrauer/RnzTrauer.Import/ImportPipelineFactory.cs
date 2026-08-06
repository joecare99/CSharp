using RnzTrauer.Import.Services;

namespace RnzTrauer.Import;

/// <summary>Builds the default import component without requiring a UI host.</summary>
public static class ImportPipelineFactory
{
    /// <summary>Creates a fresh pipeline with isolated stateful parser services.</summary>
    public static IHtmlImportPipeline CreateDefault()
    {
        return new HtmlImportPipeline(
            new HtmlSchemaImporter(
                new HtmlTextNormalizer(),
                new HtmlCallbackTokenizer(),
                new SchemaFilter(),
                new SchemaImportAccumulator(),
                new HtmlEncodingDecoder()));
    }
}
