
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

    public void AddEntry(InventoryEntryData entry)
    {
        TryAddEntry(entry);
    }

    public bool TryAddEntry(InventoryEntryData entry)
    {
        if (entry == null)
            return false;

        InventoryStack existing =
            entries.Find(x => x.data == entry);

        int currentAmount = existing != null
            ? existing.amount
            : 0;

        int maximum = Mathf.Max(1, entry.maxStack);

        if (currentAmount >= maximum)
        {
            Debug.Log(
                "No se puede agregar " + entry.entryName +
                ": alcanzó su maxStack."
            );

            return false;
        }

        AddEntryInternal(entry);

        InventoryStack stack =
            entries.Find(x => x.data == entry);

        Debug.Log(
            "Elemento agregado: " + entry.entryName +
            " (" + stack.amount + "/" + maximum + ")"
        );

        OnEntryAdded?.Invoke(entry, stack.amount);
        OnInventoryChanged?.Invoke();

        return true;
    }

    private void AddEntryInternal(InventoryEntryData entry)
    {
        if (entry == null)
            return;

        InventoryStack existing =
            entries.Find(x => x.data == entry);

        int maximum = Mathf.Max(1, entry.maxStack);

        if (existing != null)
        {
            if (existing.amount < maximum)
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

    public int GetAmount(InventoryEntryData entry)
    {
        if (entry == null)
            return 0;

        InventoryStack stack =
            entries.Find(x => x.data == entry);

        return stack != null ? stack.amount : 0;
    }

    public bool CanAddEntry(InventoryEntryData entry)
    {
        if (entry == null)
            return false;

        return GetAmount(entry) < Mathf.Max(1, entry.maxStack);
    }

    public void RemoveEntry(InventoryEntryData entry)
    {
        InventoryStack existing =
            entries.Find(x => x.data == entry);

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
