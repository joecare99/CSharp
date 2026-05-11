using System;
using System.Collections.Generic;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Validates <see cref="ContentAnalysisRequest"/> instances before analysis execution.
/// </summary>
public static class ContentAnalysisRequestValidator
{
    /// <summary>
    /// Validates the specified request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>The structured validation result.</returns>
    public static ContentAnalysisRequestValidationResult Validate(ContentAnalysisRequest? request)
    {
        List<ContentAnalysisValidationIssue> issues = [];

        if (request is null)
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest),
                Code = "request.null",
                Message = "The content analysis request must not be null.",
            });

            return new ContentAnalysisRequestValidationResult
            {
                IsValid = false,
                Issues = issues,
            };
        }

        if (string.IsNullOrWhiteSpace(request.MediaType))
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.MediaType),
                Code = "mediaType.required",
                Message = "The media type must not be empty.",
            });
        }

        if (request.ContentKind == OllamaContentKind.SourceCode && string.IsNullOrWhiteSpace(request.Language))
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.Language),
                Code = "language.required",
                Message = "A source code request must declare a language.",
            });
        }

        if (request.ContentKind == OllamaContentKind.Image && !request.MediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.MediaType),
                Code = "mediaType.image.invalid",
                Message = "An image request must use an image media type.",
            });
        }

        switch (request.SourceKind)
        {
            case OllamaContentSourceKind.Inline:
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    issues.Add(new ContentAnalysisValidationIssue
                    {
                        Field = nameof(ContentAnalysisRequest.Content),
                        Code = "content.required",
                        Message = "Inline content must not be empty.",
                    });
                }

                break;

            case OllamaContentSourceKind.FilePath:
                if (string.IsNullOrWhiteSpace(request.FilePath))
                {
                    issues.Add(new ContentAnalysisValidationIssue
                    {
                        Field = nameof(ContentAnalysisRequest.FilePath),
                        Code = "filePath.required",
                        Message = "A file-backed request must specify a file path.",
                    });
                }

                break;
        }

        for (int i = 0; i < request.Criteria.Count; i++)
        {
            ContentAnalysisCriterion criterion = request.Criteria[i];

            if (string.IsNullOrWhiteSpace(criterion.Name))
            {
                issues.Add(new ContentAnalysisValidationIssue
                {
                    Field = $"{nameof(ContentAnalysisRequest.Criteria)}[{i}].{nameof(ContentAnalysisCriterion.Name)}",
                    Code = "criteria.name.required",
                    Message = "Each analysis criterion must define a name.",
                });
            }

            if (criterion.Weight is < 0 or > 1)
            {
                issues.Add(new ContentAnalysisValidationIssue
                {
                    Field = $"{nameof(ContentAnalysisRequest.Criteria)}[{i}].{nameof(ContentAnalysisCriterion.Weight)}",
                    Code = "criteria.weight.range",
                    Message = "Criterion weights must be within the range from 0.0 to 1.0.",
                });
            }
        }

        return new ContentAnalysisRequestValidationResult
        {
            IsValid = issues.Count == 0,
            Issues = issues,
        };
    }
}
