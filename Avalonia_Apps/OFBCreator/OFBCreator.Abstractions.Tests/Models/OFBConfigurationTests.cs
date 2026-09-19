using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OFBCreator.Abstractions.Tests.Models;

using OFBCreator.Abstractions.Models;
using OFBCreator.Abstractions.Interfaces;
using NSubstitute;

/// <summary>
/// Tests for OFBConfiguration model — validation, property setting, null handling.
/// </summary>
[TestClass]
public class OFBConfigurationTests
{
    [TestMethod]
    public void Validate_ValidParams_ShouldReturnSuccess()
    {
        // Arrange & Act
        var result = OFBConfiguration.Validate(
            inputPath: "C:\\data\\family.ged",
            outputPath: "C:\\output\\ofb.docx",
            title: "Familienbuch Mustermund" );

        // Assert
        Assert.IsTrue( result.IsValid );
        Assert.IsNotNull( result.Configuration );
        Assert.AreEqual( 0, result.Errors.Count );
    }

    [TestMethod]
    public void Validate_ValidParamsWithProvider_ShouldReturnSuccess()
    {
        // Arrange
        var provider = Substitute.For<IUserDocumentFormatProvider>();
        provider.IsFormatSupported( "docx" ).ReturnsForAnyArgs( true );

        // Act
        var result = OFBConfiguration.Validate(
            inputPath: "C:\\data\\family.ged",
            outputPath: "C:\\output\\ofb.docx",
            title: "Familienbuch Mustermund",
            formatProvider: provider );

        // Assert
        Assert.IsTrue( result.IsValid );
        Assert.IsNotNull( result.Configuration );
    }

    [TestMethod]
    public void Validate_NullInputPath_ShouldHaveErrors()
    {
        // Act
        var result = OFBConfiguration.Validate(
            inputPath: null,
            outputPath: "C:\\output\\ofb.docx",
            title: "Familienbuch Mustermund" );

        // Assert
        Assert.IsFalse( result.IsValid );
        Assert.IsNotNull( result.Errors );
        Assert.IsTrue( result.Errors.Count > 0 );
    }

    [TestMethod]
    public void Validate_EmptyOutputPath_ShouldHaveErrors()
    {
        // Act
        var result = OFBConfiguration.Validate(
            inputPath: "C:\\data\\family.ged",
            outputPath: "",
            title: "Familienbuch Mustermund" );

        // Assert
        Assert.IsFalse( result.IsValid );
        Assert.IsTrue( result.Errors.Count > 0 );
    }

    [TestMethod]
    public void Validate_WhitespaceTitle_ShouldHaveErrors()
    {
        // Act
        var result = OFBConfiguration.Validate(
            inputPath: "C:\\data\\family.ged",
            outputPath: "C:\\output\\ofb.docx",
            title: "   " );

        // Assert
        Assert.IsFalse( result.IsValid );
        Assert.IsTrue( result.Errors.Count > 0 );
    }

    [TestMethod]
    public void Validate_AllNullParams_ShouldHaveMultipleErrors()
    {
        // Act
        var result = OFBConfiguration.Validate(
            inputPath: null,
            outputPath: null,
            title: null );

        // Assert
        Assert.IsFalse( result.IsValid );
        Assert.AreEqual( 3, result.Errors.Count );
    }

    [TestMethod]
    public void Validate_ShouldFillDefaultValuesForNullOptions()
    {
        // Arrange & Act
        var result = OFBConfiguration.Validate(
            inputPath: "C:\\data\\family.ged",
            outputPath: "C:\\output\\ofb.docx",
            title: "Familienbuch" );

        // Assert
        Assert.IsTrue( result.IsValid );
        Assert.IsNull( result.Configuration!.PlaceId );
        Assert.IsFalse( result.Configuration.IncludeDescendants );
    }

    [TestMethod]
    public void Configuration_ShouldPreserveAllValues()
    {
        // Arrange — use object initializer for init properties (can't set after construction)
        var result = OFBConfiguration.Validate(
            inputPath: "C:\\data\\family.ged",
            outputPath: "C:\\output\\ofb.docx",
            title: "Familienbuch Mustermund" );

        var config = new OFBConfiguration
        {
            InputPath = result.Configuration!.InputPath,
            OutputPath = result.Configuration.OutputPath,
            Title = result.Configuration.Title,
            PlaceId = "P123",
            IncludeDescendants = true,
            Preface = "Dies ist ein Vorwort.",
            Legend = "Zeichenerklärung: ...",
        };

        // Assert
        Assert.AreEqual( "P123", config.PlaceId );
        Assert.IsTrue( config.IncludeDescendants );
        Assert.AreEqual( "Dies ist ein Vorwort.", config.Preface );
        Assert.AreEqual( "Zeichenerklärung: ...", config.Legend );
    }

    [TestMethod]
    public void IsValid_ShouldReflectErrorsCount()
    {
        // Arrange — valid case
        var valid = OFBConfiguration.Validate(
            inputPath: "a.ged",
            outputPath: "b.docx",
            title: "T" );

        Assert.IsTrue( valid.IsValid );
        Assert.AreEqual( 0, valid.Errors.Count );

        // Arrange — invalid case
        var invalid = OFBConfiguration.Validate(
            inputPath: null,
            outputPath: null,
            title: null );

        Assert.IsFalse( invalid.IsValid );
        Assert.IsTrue( invalid.Errors.Count > 0 );
    }
}
