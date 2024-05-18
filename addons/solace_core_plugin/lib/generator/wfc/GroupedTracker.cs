using System;
using System.Collections.Generic;
using System.Linq;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

public class GroupedTracker
{
    private readonly Dictionary<float, HashSet<int>> _itemsByWeight = new();
    private readonly Dictionary<int, float> _weightByItem = new();
    public bool IsEmpty => _weightByItem.Count == 0;


    public int GetRandomMin()
    {
        var minItems = _itemsByWeight[_itemsByWeight.Keys.Min()].ToArray();
        var randomItem = minItems[Random.Shared.Next() % minItems.Length];
        return randomItem;
    }

    public void Clear()
    {
        _itemsByWeight.Clear();
        _weightByItem.Clear();
    }

    public void TrackItem(int itemId, float currentWeight)
    {
        var itemWasTracked = _weightByItem.TryGetValue(itemId, out var lastWeight);

        if (!itemWasTracked)
        {
            NewItem(itemId, currentWeight);
            return;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (currentWeight == lastWeight)
        {
            return;
        }

        UpdateItem(itemId, lastWeight, currentWeight);
    }

    public void UntrackItem(int itemId)
    {
        var waveWasTracked = _weightByItem.TryGetValue(itemId, out var lastEntropy);
        if (!waveWasTracked) return;
        RemoveItem(itemId, lastEntropy);
    }

    private void NewItem(int itemId, float weight)
    {
        _weightByItem[itemId] = weight;
        if (!_itemsByWeight.TryGetValue(weight, out var itemIdSet))
        {
            _itemsByWeight[weight] = new HashSet<int> { itemId };
        }
        else
        {
            itemIdSet.Add(itemId);
        }
    }

    private void UpdateItem(int itemId, float lastWeight, float newWeight)
    {
        _itemsByWeight[lastWeight].Remove(itemId);
        if (_itemsByWeight[lastWeight].Count == 0)
        {
            _itemsByWeight.Remove(lastWeight);
        }

        NewItem(itemId, newWeight);
    }

    private void RemoveItem(int itemId, float lastEntropy)
    {
        _itemsByWeight[lastEntropy].Remove(itemId);
        if (_itemsByWeight[lastEntropy].Count == 0)
        {
            _itemsByWeight.Remove(lastEntropy);
        }

        _weightByItem.Remove(itemId);
    }
}