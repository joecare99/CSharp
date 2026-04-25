using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Core.Services;

/// <summary>
/// Persists processing configurations as deterministic JSON documents.
/// </summary>
public sealed class JsonProcessingConfigurationStorage : IProcessingConfigurationStorage
{
    /// <inheritdoc/>
    public ProcessingConfigurationModel Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("A file path is required.", nameof(filePath));

        using var stream = File.OpenRead(filePath);
        var serializer = CreateSerializer();
        var dto = serializer.ReadObject(stream) as ProcessingConfigurationDto;
        if (dto == null)
            throw new SerializationException("The processing configuration document could not be read.");

        return dto.ToModel();
    }

    /// <inheritdoc/>
    public void Save(string filePath, ProcessingConfigurationModel configuration)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("A file path is required.", nameof(filePath));
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        using var stream = File.Create(filePath);
        var serializer = CreateSerializer();
        serializer.WriteObject(stream, ProcessingConfigurationDto.FromModel(configuration));
        stream.Flush();
    }

    private static DataContractJsonSerializer CreateSerializer()
    {
        return new DataContractJsonSerializer(typeof(ProcessingConfigurationDto));
    }

    [DataContract]
    private sealed class ProcessingConfigurationDto
    {
        [DataMember(Name = "configurationName", Order = 1)]
        public string ConfigurationName { get; set; } = string.Empty;

        [DataMember(Name = "steps", Order = 2)]
        public List<ProcessingStepDto> Steps { get; set; } = new();

        public ProcessingConfigurationModel ToModel()
        {
            var steps = new List<ProcessingStepState>(Steps.Count);
            foreach (var step in Steps)
                steps.Add(step.ToModel());

            return new ProcessingConfigurationModel(ConfigurationName, steps);
        }

        public static ProcessingConfigurationDto FromModel(ProcessingConfigurationModel model)
        {
            var dto = new ProcessingConfigurationDto
            {
                ConfigurationName = model.ConfigurationName
            };

            foreach (var step in model.Steps)
                dto.Steps.Add(ProcessingStepDto.FromModel(step));

            return dto;
        }
    }

    [DataContract]
    private sealed class ProcessingStepDto
    {
        [DataMember(Name = "stepId", Order = 1)]
        public string StepId { get; set; } = string.Empty;

        [DataMember(Name = "operationName", Order = 2)]
        public string OperationName { get; set; } = string.Empty;

        [DataMember(Name = "isEnabled", Order = 3)]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "inputs", Order = 4)]
        public List<ProcessingInputDto> Inputs { get; set; } = new();

        [DataMember(Name = "parameters", Order = 5)]
        public List<ProcessingParameterDto> Parameters { get; set; } = new();

        [DataMember(Name = "outputs", Order = 6)]
        public List<ProcessingOutputDto> Outputs { get; set; } = new();

        public ProcessingStepState ToModel()
        {
            var inputs = new List<ProcessingInputState>(Inputs.Count);
            foreach (var input in Inputs)
                inputs.Add(new ProcessingInputState(input.SourceKind, input.ChannelName, input.SourceStepId));

            var parameters = new List<ProcessingParameterState>(Parameters.Count);
            foreach (var parameter in Parameters)
                parameters.Add(new ProcessingParameterState(parameter.Name, parameter.ValueText));

            var outputs = new List<ProcessingOutputState>(Outputs.Count);
            foreach (var output in Outputs)
                outputs.Add(new ProcessingOutputState(output.OutputRole, output.ChannelName, output.UnitText));

            return new ProcessingStepState(StepId, OperationName, IsEnabled, inputs, parameters, outputs);
        }

        public static ProcessingStepDto FromModel(ProcessingStepState model)
        {
            var dto = new ProcessingStepDto
            {
                StepId = model.StepId,
                OperationName = model.OperationName,
                IsEnabled = model.IsEnabled
            };

            foreach (var input in model.Inputs)
                dto.Inputs.Add(new ProcessingInputDto
                {
                    SourceKind = input.SourceKind,
                    ChannelName = input.ChannelName,
                    SourceStepId = input.SourceStepId
                });

            foreach (var parameter in model.Parameters)
                dto.Parameters.Add(new ProcessingParameterDto
                {
                    Name = parameter.Name,
                    ValueText = parameter.ValueText
                });

            foreach (var output in model.Outputs)
                dto.Outputs.Add(new ProcessingOutputDto
                {
                    OutputRole = output.OutputRole,
                    ChannelName = output.ChannelName,
                    UnitText = output.UnitText
                });

            return dto;
        }
    }

    [DataContract]
    private sealed class ProcessingInputDto
    {
        [DataMember(Name = "sourceKind", Order = 1)]
        public string SourceKind { get; set; } = string.Empty;

        [DataMember(Name = "channelName", Order = 2)]
        public string ChannelName { get; set; } = string.Empty;

        [DataMember(Name = "sourceStepId", Order = 3, EmitDefaultValue = false)]
        public string? SourceStepId { get; set; }
    }

    [DataContract]
    private sealed class ProcessingParameterDto
    {
        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "valueText", Order = 2)]
        public string ValueText { get; set; } = string.Empty;
    }

    [DataContract]
    private sealed class ProcessingOutputDto
    {
        [DataMember(Name = "outputRole", Order = 1, EmitDefaultValue = false)]
        public string? OutputRole { get; set; }

        [DataMember(Name = "channelName", Order = 2)]
        public string ChannelName { get; set; } = string.Empty;

        [DataMember(Name = "unitText", Order = 3, EmitDefaultValue = false)]
        public string? UnitText { get; set; }
    }
}
