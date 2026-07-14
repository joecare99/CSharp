/*
 * FEHLERLISTENSERVICE UNIT-TESTS – DOKUMENTATION
 * 
 * Diese Datei dokumentiert die Unit-Tests, die für FehlerlistenService implementiert werden sollten.
 * Die Tests folgen dem MSTest 4.2.3 Pattern und validieren die EVA-Implementierung.
 * 
 * HINWEIS: Diese Tests können implementiert werden, sobald das Test-Projekt MSTest-Abhängigkeiten hat.
 * 
 * ============================================
 * TEST-PLAN
 * ============================================
 * 
 * 1. GetPersonenOhneElternAsync_ReturnsSuccessResult
 *    - Input: Progress + Item Callbacks
 *    - Process: Service iteriert Demo-Daten
 *    - Output: ErrorListResult mit IsSuccess=true, Title="Personen ohne Eltern (tote Punkte)"
 *    - Assertion: result.IsSuccess == true && itemsAdded.Count > 0
 * 
 * 2. GetPersonenOhneElternAsync_CallsProgressCallbacks
 *    - Input: Track progressCallback invocations
 *    - Process: Service ruft Progress-Callback für jedes Item auf
 *    - Output: Callback-Liste mit (current, max) Paaren
 *    - Assertion: progressCalls.Count >= itemsAdded.Count
 * 
 * 3. GetFamilienErrorsAsync_ReturnsStructuredItems
 *    - Input: Callback an FamilienErrorsAsync
 *    - Process: Service formatiert Items mit ID, DisplayText, AdditionalData
 *    - Output: ErrorListItem mit allen erforderlichen Feldern
 *    - Assertion: item.DisplayText != "", item.Id > 0, item.AdditionalData.ContainsKey("ErrorType")
 * 
 * 4. GetOerterErrorsAsync_ReturnsSuccessResult
 *    - Input: OerterErrorsAsync
 *    - Process: Gibt Orte-Demo-Daten zurück
 *    - Output: ErrorListResult mit Title="Fehlliste Orte"
 *    - Assertion: result.Title == "Fehlliste Orte" && itemsAdded.Count > 0
 * 
 * 5. HandlesNullCallbacksGracefully
 *    - Input: null progressCallback, null itemAddCallback
 *    - Process: Service sollte nicht null-ausnahmen werfen
 *    - Output: ErrorListResult mit IsSuccess=true
 *    - Assertion: Assert.Throws wird NICHT erwartet (graceful handling)
 * 
 * 6. DeriveErrorTypeCorrectly
 *    - Input: GetPersonenOhneElternAsync (enthält "Eltern" in ErrorDescription)
 *    - Process: ErrorType-Ableitung basierend auf Keywords
 *    - Output: item.AdditionalData["ErrorType"] == "MissingParent"
 *    - Assertion: errorType == "MissingParent"
 * 
 * 7. GetFamilienErrorsAsync_DerivesDatesErrorType
 *    - Input: GetFamilienErrorsAsync (enthält "Datum" im Error-Text)
 *    - Process: Fehlertyp-Matching
 *    - Output: item.AdditionalData["ErrorType"] enthält "Date"
 *    - Assertion: errorType.Contains("Date") || errorType == "MissingDate"
 * 
 * ============================================
 * Benötigte Test-Infrastruktur
 * ============================================
 * 
 * - MSTest 4.2.3 (oder neuere Version)
 * - GenFreeWin.csproj als Abhängigkeit
 * - Test-Projekt mit [TestClass], [TestMethod], [TestInitialize] Attributen
 * 
 * ============================================
 * Code-Vorlage (später zu verwenden)
 * ============================================
 * 
 * [TestClass]
 * public class FehlerlistenServiceTests
 * {
 *     private IFehlerlistenService _service;
 * 
 *     [TestInitialize]
 *     public void Setup()
 *     {
 *         _service = new FehlerlistenService();
 *     }
 * 
 *     [TestMethod]
 *     public async Task GetPersonenOhneElternAsync_ReturnsSuccessResult()
 *     {
 *         var progressCalls = new List<(int, int)>();
 *         var itemsAdded = new List<ErrorListItem>();
 *         var result = await _service.GetPersonenOhneElternAsync(
 *             (c, m) => progressCalls.Add((c, m)),
 *             item => itemsAdded.Add(item));
 *         Assert.IsTrue(result.IsSuccess);
 *     }
 *     
 *     // ... weitere Tests folgen
 * }
 * 
 * ============================================
 */
