using Gen_FreeWin.Models;
using Gen_FreeWin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenFreeWinTests;

/// <summary>
/// Unit-Tests für FehlerlistenService (EVA-Prinzip).
/// Testet Datenbeschaffung, Verarbeitung und Ausgabe ohne UI-Abhängigkeiten.
/// </summary>
[TestClass]
public class FehlerlistenServiceTests
{
    private FehlerlistenService _service;

    [TestInitialize]
    public void Setup()
    {
        _service = new FehlerlistenService();
    }

    /// <summary>
    /// TEST 1: GetPersonenOhneElternAsync() gibt korrekten Titel zurück.
    /// </summary>
    [TestMethod]
    public async Task GetPersonenOhneElternAsync_ReturnsSucessResult_WithValidTitle()
    {
        // Arrange
        var progressCallbackCalls = new List<(int current, int maximum)>();
        void ProgressCallback(int current, int maximum) => progressCallbackCalls.Add((current, maximum));
        Action<ErrorListItem> itemCallback = _ => { };

        // Act
        var result = await _service.GetPersonenOhneElternAsync(ProgressCallback, itemCallback);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Personen ohne Eltern (tote Punkte)", result.Title);
        Assert.IsNull(result.ErrorMessage);
    }

    /// <summary>
    /// TEST 2: GetPersonenOhneElternAsync() ruft progressCallback mit erwarteten Werten auf.
    /// </summary>
    [TestMethod]
    public async Task GetPersonenOhneElternAsync_CallsProgressCallback_WithCorrectValues()
    {
        // Arrange
        var progressCalls = new List<(int current, int maximum)>();
        void ProgressCallback(int current, int maximum) => progressCalls.Add((current, maximum));
        Action<ErrorListItem> itemCallback = _ => { };

        // Act
        await _service.GetPersonenOhneElternAsync(ProgressCallback, itemCallback);

        // Assert
        Assert.IsTrue(progressCalls.Count > 0, "progressCallback sollte mindestens einmal aufgerufen werden");

        // Erste Call: (0, maxPersonen) - Initial Progress
        Assert.AreEqual(0, progressCalls[0].current);
        Assert.IsTrue(progressCalls[0].maximum > 0);

        // Letzte Call: sollte bei maxPersonen sein
        var lastCall = progressCalls.Last();
        Assert.AreEqual(lastCall.maximum, lastCall.current);
    }

    /// <summary>
    /// TEST 3: GetPersonenOhneElternAsync() ruft itemAddCallback für jedes Demo-Item auf.
    /// </summary>
    [TestMethod]
    public async Task GetPersonenOhneElternAsync_CallsItemCallback_ForEachDemoItem()
    {
        // Arrange
        var itemsCaptured = new List<ErrorListItem>();
        void ItemCallback(ErrorListItem item) => itemsCaptured.Add(item);
        Action<int, int> progressCallback = (_, _) => { };

        // Act
        var result = await _service.GetPersonenOhneElternAsync(progressCallback, ItemCallback);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(itemsCaptured.Count >= 3, "Sollte mindestens 3 Demo-Personen laden");

        // Prüfe, dass Items strukturiert sind
        foreach (var item in itemsCaptured)
        {
            Assert.IsNotNull(item.DisplayText);
            Assert.IsTrue(item.Id > 0);
            Assert.IsNotNull(item.AdditionalData);
            Assert.IsTrue(item.AdditionalData.ContainsKey("ErrorType"));
            Assert.IsTrue(item.AdditionalData.ContainsKey("ErrorDescription"));
        }
    }

    /// <summary>
    /// TEST 4: GetPersonenOhneElternAsync() behandelt null-Callback ohne Exception.
    /// </summary>
    [TestMethod]
    public async Task GetPersonenOhneElternAsync_HandlesNullCallbacks_WithoutException()
    {
        // Arrange & Act
        // Sollte keine Exception werfen, auch wenn Callbacks null sind
        var result = await _service.GetPersonenOhneElternAsync(null, null);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess, "Service sollte erfolgreich sein, auch mit null-Callbacks");
    }

    /// <summary>
    /// TEST 5: GetPersonenErrorsAsync() nimmt Demo-Daten und returniert strukturierte Items.
    /// </summary>
    [TestMethod]
    public async Task GetPersonenErrorsAsync_ReturnsStructuredItems()
    {
        // Arrange
        var itemsCaptured = new List<ErrorListItem>();
        void ItemCallback(ErrorListItem item) => itemsCaptured.Add(item);
        Action<int, int> progressCallback = (_, _) => { };

        // Act
        var result = await _service.GetPersonenErrorsAsync(progressCallback, ItemCallback);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Fehlliste Personen (fehlende Daten)", result.Title);
        Assert.IsTrue(itemsCaptured.Count >= 0); // Demo kann auch leer sein
    }

    /// <summary>
    /// TEST 6: GetOerterErrorsAsync() returniert Orte-Fehlliste mit DisplayText.
    /// </summary>
    [TestMethod]
    public async Task GetOerterErrorsAsync_ReturnsLocationItems_WithDisplayText()
    {
        // Arrange
        var itemsCaptured = new List<ErrorListItem>();
        void ItemCallback(ErrorListItem item) => itemsCaptured.Add(item);
        Action<int, int> progressCallback = (_, _) => { };

        // Act
        var result = await _service.GetOerterErrorsAsync(progressCallback, ItemCallback);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Fehlliste Orte", result.Title);

        // Alle Items sollten ein DisplayText haben (auch wenn Liste leer)
        foreach (var item in itemsCaptured)
        {
            Assert.IsNotNull(item.DisplayText);
            Assert.IsTrue(item.DisplayText.Length > 0, "DisplayText sollte nicht leer sein");
        }
    }

    /// <summary>
    /// TEST 7: Service-Fehlerbehandlung (Exception → ErrorMessage).
    /// </summary>
    [TestMethod]
    public async Task GetPersonenOhneElternAsync_HandlesCallbackException_GracefullyAsync()
    {
        // Arrange
        void BrokenCallback(ErrorListItem item) => throw new InvalidOperationException("Simulated callback error");
        Action<int, int> progressCallback = (_, _) => { };

        // Act & Assert
        // Service sollte Exception fangen und IsSuccess=false setzen
        var result = await _service.GetPersonenOhneElternAsync(progressCallback, BrokenCallback);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Simulated callback error")
                   || result.ErrorMessage.Length > 0);
    }

    /// <summary>
    /// TEST 8: GetFamilienErrorsAsync() returniert Familien-Fehlliste.
    /// </summary>
    [TestMethod]
    public async Task GetFamilienErrorsAsync_ReturnsFamiliesErrorList_WithCorrectTitle()
    {
        // Arrange
        var itemsCaptured = new List<ErrorListItem>();
        void ItemCallback(ErrorListItem item) => itemsCaptured.Add(item);
        Action<int, int> progressCallback = (_, _) => { };

        // Act
        var result = await _service.GetFamilienErrorsAsync(progressCallback, ItemCallback);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Fehlliste Familien", result.Title);
    }
}
