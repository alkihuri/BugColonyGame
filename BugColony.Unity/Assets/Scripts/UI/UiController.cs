using System;
using System.Collections.Generic;
using MonoContainer;
using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [Header("Pool Setup")] [SerializeField]
    private TextMeshProUGUI fieldPrefab;

    [SerializeField] private Transform fieldsRoot;

    [Header("Optional preplaced fields in scene")] [SerializeField]
    private List<TextMeshProUGUI> preplacedFields = new();

    private readonly Stack<TextMeshProUGUI> _freeFields = new();
    private readonly HashSet<TextMeshProUGUI> _usedFields = new();

    // Main mapping: one data container -> one ui field
    private readonly Dictionary<string, TextMeshProUGUI> _fieldByContainer = new();

    // Reverse mapping to clean dictionary on release
    
    private readonly Dictionary<TextMeshProUGUI, string> _containerByField = new();

    private void Awake()
    {
        if (fieldsRoot == null)
        {
            fieldsRoot = transform;
        }

        for (int i = 0; i < preplacedFields.Count; i++)
        {
            var field = preplacedFields[i];
            if (field == null)
            {
                continue;
            }

            field.transform.SetParent(fieldsRoot, false);
            field.gameObject.SetActive(false);
            _freeFields.Push(field);
        }
    }

    public TextMeshProUGUI GetFreeOrNewField(string data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "MonoContainer is null.");
        }

        // If already mapped, reuse existing field
        if (_fieldByContainer.TryGetValue(data, out var existingField) && existingField != null)
        {
            if (!_usedFields.Contains(existingField))
            {
                _usedFields.Add(existingField);
            }

            if (!existingField.gameObject.activeSelf)
            {
                existingField.gameObject.SetActive(true);
            }

            return existingField;
        }

        var field = GetFieldFromPoolOrCreate();

        _usedFields.Add(field);
        _fieldByContainer[data] = field;
        _containerByField[field] = data;

        field.gameObject.SetActive(true);
        field.text = string.Empty;
        return field;
    }

    public void ReleaseField(TextMeshProUGUI field)
    {
        if (field == null)
        {
            return;
        }

        if (_usedFields.Remove(field))
        {
            if (_containerByField.TryGetValue(field, out var data))
            {
                _containerByField.Remove(field);
                if (data != null)
                {
                    _fieldByContainer.Remove(data);
                }
            }

            field.text = string.Empty;
            field.gameObject.SetActive(false);
            field.transform.SetParent(fieldsRoot, false);
            _freeFields.Push(field);
        }
    }

    public void ReleaseField(string data)
    {
        if (data == null)
        {
            return;
        }

        if (_fieldByContainer.TryGetValue(data, out var field))
        {
            ReleaseField(field);
        }
    }

    public void ReleaseAll()
    {
        var snapshot = new List<TextMeshProUGUI>(_usedFields);
        for (int i = 0; i < snapshot.Count; i++)
        {
            ReleaseField(snapshot[i]);
        }

        _fieldByContainer.Clear();
        _containerByField.Clear();
    }

    private TextMeshProUGUI GetFieldFromPoolOrCreate()
    {
        TextMeshProUGUI field = null;

        while (_freeFields.Count > 0 && field == null)
        {
            field = _freeFields.Pop();
        }

        if (field != null)
        {
            return field;
        }

        if (fieldPrefab == null)
        {
            throw new InvalidOperationException($"{nameof(UiController)}: fieldPrefab is not assigned.");
        }

        return Instantiate(fieldPrefab, fieldsRoot);
    }
}