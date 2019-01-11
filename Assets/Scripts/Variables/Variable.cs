using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Variable<T> : ScriptableObject
{
    public T Value;
}