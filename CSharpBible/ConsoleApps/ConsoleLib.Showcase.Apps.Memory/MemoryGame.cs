using System;
using System.Collections.Generic;

namespace ConsoleLib.Showcase.Apps.Memory;

public sealed class MemoryGame
{
    private readonly int _pairCount;
    private readonly Random _random;
    private readonly List<MemoryCard> _cards = [];
    private readonly IReadOnlyList<MemoryCard> _cardView;
    private int? _firstSelection;

    public MemoryGame(int pairCount = 4, Random? random = null)
    {
        if (pairCount is < 1 or > 26)
            throw new ArgumentOutOfRangeException(nameof(pairCount));

        _pairCount = pairCount;
        _random = random ?? Random.Shared;
        _cardView = _cards.AsReadOnly();
        Restart();
    }

    public MemoryGame(Random random, int pairCount = 4)
        : this(pairCount, random ?? throw new ArgumentNullException(nameof(random)))
    {
    }

    public MemoryGame(int pairCount, int seed)
        : this(pairCount, new Random(seed))
    {
    }

    public IReadOnlyList<MemoryCard> Cards => _cardView;
    public int Moves { get; private set; }
    public int MatchedPairs { get; private set; }
    public bool IsCompleted => MatchedPairs == _pairCount;

    public MemorySelectionResult SelectCard(int index)
    {
        ValidateIndex(index);
        var card = _cards[index];
        if (IsCompleted || card.IsMatched || card.IsFaceUp)
            return MemorySelectionResult.Rejected;

        card.IsFaceUp = true;
        if (_firstSelection is null)
        {
            _firstSelection = index;
            return MemorySelectionResult.FirstSelected;
        }

        var first = _cards[_firstSelection.Value];
        _firstSelection = null;
        Moves++;
        if (first.Value == card.Value)
        {
            first.IsMatched = true;
            card.IsMatched = true;
            MatchedPairs++;
            return IsCompleted ? MemorySelectionResult.Completed : MemorySelectionResult.Match;
        }

        first.IsFaceUp = false;
        card.IsFaceUp = false;
        return MemorySelectionResult.Mismatch;
    }

    public void Restart()
    {
        _cards.Clear();
        var values = new List<string>(_pairCount * 2);
        for (var index = 0; index < _pairCount; index++)
        {
            var value = ((char)('A' + index)).ToString();
            values.Add(value);
            values.Add(value);
        }

        for (var index = values.Count - 1; index > 0; index--)
        {
            var swapIndex = _random.Next(index + 1);
            (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
        }

        for (var index = 0; index < values.Count; index++)
            _cards.Add(new MemoryCard(index, values[index]));

        _firstSelection = null;
        Moves = 0;
        MatchedPairs = 0;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _cards.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
    }
}
