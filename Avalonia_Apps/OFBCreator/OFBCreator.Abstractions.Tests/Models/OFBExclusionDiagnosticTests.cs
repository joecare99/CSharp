using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OFBCreator.Abstractions.Tests.Models;

using OFBCreator.Abstractions.Models;
using NSubstitute;

/// <summary>
/// Tests for OFBExclusionDiagnostic — factory methods and property setting.
/// </summary>
[TestClass]
public class OFBExclusionDiagnosticTests
{
    [TestMethod]
    public void Create_ShouldSetAllProperties()
    {
        // Arrange & Act
        var diagnostic = OFBExclusionDiagnostic.Create(
            entityId: "F123",
            entityType: "Family",
            reasonCode: "PlaceNotInScope",
            explanation: "Family does not belong to a selected place." );

        // Assert
        Assert.AreEqual( "F123", diagnostic.EntityId );
        Assert.AreEqual( "Family", diagnostic.EntityType );
        Assert.AreEqual( "PlaceNotInScope", diagnostic.ReasonCode );
        Assert.AreEqual( "Family does not belong to a selected place.", diagnostic.Explanation );
    }

    [TestMethod]
    [DataRow( null, "Family", "MissingId" )]
    [DataRow( "F1", null, "MissingType" )]
    [DataRow( "F1", "Family", null )]
    public void Create_ShouldThrowOnNullRequiredParams( string? entityId, string? entityType, string? reasonCode )
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<System.ArgumentNullException>( () =>
            OFBExclusionDiagnostic.Create(
                entityId!,
                entityType!,
                reasonCode!,
                "Some explanation" ) );

        var expectedParam = entityId == null ? nameof( entityId ) : (entityType == null ? nameof( entityType ) : nameof( reasonCode ));
        Assert.IsTrue( ex.ParamName!.Contains( expectedParam ), $"Expected param '{expectedParam}' but got '{ex.ParamName}'." );
    }

    [TestMethod]
    public void Create_ShouldAllowEmptyOptionalFields()
    {
        // Arrange & Act
        var diagnostic = OFBExclusionDiagnostic.Create(
            entityId: "P456",
            entityType: "Person",
            reasonCode: "MissingBirthDate",
            explanation: string.Empty );

        // Assert
        Assert.AreEqual( "P456", diagnostic.EntityId );
        Assert.IsTrue( string.IsNullOrEmpty( diagnostic.Explanation ) );
    }

    [TestMethod]
    public void FromEntity_ShouldExtractIdAndType()
    {
        // Arrange
        var person = NSubstitute.Substitute.For<GenInterfaces.Interfaces.Genealogic.IGenPerson>();
        
        // Act
        var diagnostic = OFBExclusionDiagnostic.FromEntity(
            person,
            "MissingBirthDate",
            "No birth date found for this person." );

        // Assert
        Assert.IsNotNull( diagnostic );
        Assert.AreEqual( "Person", diagnostic.EntityType );
        Assert.AreEqual( "MissingBirthDate", diagnostic.ReasonCode );
        Assert.AreEqual( "No birth date found for this person.", diagnostic.Explanation );
    }
}
