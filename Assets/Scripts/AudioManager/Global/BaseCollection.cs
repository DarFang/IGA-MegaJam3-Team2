using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseCollection<T> where T : class {
    public List<T> Items { get; protected set; }
    public int CurrentIndex;
    public bool IsShuffled = false;
    public string Name;

    protected List<T> originalItems; // Для скидання до початкового порядку

    public BaseCollection(List<T> items, string name = "Default") {
        Items = new List<T>(items);
        originalItems = new List<T>(items);
        CurrentIndex = 0;
        Name = name;
    }

    public virtual void Shuffle() {
        if (Items.Count <= 1) return;

        for (int i = Items.Count - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            (Items[i], Items[j]) = (Items[j], Items[i]);
        }
        IsShuffled = true;
        CurrentIndex = 0;
    }

    public virtual void ResetToOriginalOrder() {
        Items = new List<T>(originalItems);
        IsShuffled = false;
        CurrentIndex = 0;
    }

    public virtual T GetNext(bool loop) {
        if (Items == null || Items.Count == 0) return null;

        CurrentIndex++;
        if (CurrentIndex >= Items.Count) {
            CurrentIndex = loop ? 0 : Items.Count - 1;
        }
        return Items[CurrentIndex];
    }

    public virtual T GetPrevious(bool loop) {
        if (Items.Count == 0) return null;

        CurrentIndex--;
        if (CurrentIndex < 0) {
            CurrentIndex = loop ? Items.Count - 1 : 0;
        }
        return Items[CurrentIndex];
    }

    public virtual T GetCurrent() {
        if (Items == null || Items.Count == 0) return null;
        CurrentIndex = Mathf.Clamp(CurrentIndex, 0, Items.Count - 1);
        return Items[CurrentIndex];
    }
}
