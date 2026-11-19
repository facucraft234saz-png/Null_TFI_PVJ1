
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryStack
{
    public InventoryEntryData data;
    public int amount;
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Elementos iniciales")]
    [SerializeField]
    private List<InventoryEntryData> startingEntries =
        new List<InventoryEntryData>();

    private List<InventoryStack> entries =
        new List<InventoryStack>();

    public event Action OnInventoryChanged;

    // Avisa qué ítem se agregó y cuál es su cantidad actual.
    public event Action<InventoryEntryData, int> OnEntryAdded;

    public IReadOnlyList<InventoryStack> Entries => entries;

    private void Awake()
    {
        entries.Clear();

        foreach (InventoryEntryData entry in startingEntries)
        {
            AddEntryInternal(entry);
        }
    }

    // Este método también será utilizado por la futura tienda.
    public void AddEntry(InventoryEntryData entry)
    {
        if (entry == null)
            return;

        AddEntryInternal(entry);

        InventoryStack stack = entries.Find(x => x.data == entry);

        Debug.Log("Elemento agregado: " + entry.entryName);

        OnEntryAdded?.Invoke(entry, stack.amount);
        OnInventoryChanged?.Invoke();
    }

    private void AddEntryInternal(InventoryEntryData entry)
    {
        if (entry == null)
            return;

        InventoryStack existing = entries.Find(x => x.data == entry);

        if (existing != null)
        {
            existing.amount++;
        }
        else
        {
            entries.Add(new InventoryStack
            {
                data = entry,
                amount = 1
            });
        }
    }

    public void RemoveEntry(InventoryEntryData entry)
    {
        InventoryStack existing = entries.Find(x => x.data == entry);

        if (existing == null)
            return;

        existing.amount--;

        if (existing.amount <= 0)
            entries.Remove(existing);

        OnInventoryChanged?.Invoke();
    }

    public void ClearInventory()
    {
        entries.Clear();
        OnInventoryChanged?.Invoke();
    }

    public int GetCategoryCount(InventoryCategory category)
    {
        int count = 0;

        foreach (InventoryStack entry in entries)
        {
            if (entry.data != null &&
                entry.data.category == category)
            {
                count += entry.amount;
            }
        }

        return count;
    }
}