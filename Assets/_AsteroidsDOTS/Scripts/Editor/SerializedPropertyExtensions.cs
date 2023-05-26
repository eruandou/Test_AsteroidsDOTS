using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SerializedPropertyExtensions
{
    public static bool Contains(this SerializedProperty property, Object obj)
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            if (property.GetArrayElementAtIndex(i)?.objectReferenceValue == obj)
            {
                return true;
            }
        }

        return false;
    }
    
    public static SerializedProperty Insert(this SerializedProperty property, int index)
    {
        property.InsertArrayElementAtIndex(index);
        return property.GetArrayElementAtIndex(index);
    }

    public static void Add(this SerializedProperty property, Object obj)
    {
        property.Insert(property.arraySize).objectReferenceValue = obj;
    }
    
    public static bool Remove(this SerializedProperty property, Object obj)
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            if (property.GetArrayElementAtIndex(i)?.objectReferenceValue == obj)
            {
                property.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public static SerializedProperty ElementAt(this SerializedProperty property, int index)
    {
        return property.GetArrayElementAtIndex(index);
    }
    
    public static void RemoveAt(this SerializedProperty property, int index)
    {
        property.DeleteArrayElementAtIndex(index);
    }

    public static void Clear(this SerializedProperty property)
    {
        property.ClearArray();
        property.arraySize = 0;
    }
}
