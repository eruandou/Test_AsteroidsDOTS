using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _AsteroidsDOTS.Scripts.Editor
{
    [Serializable]
    public class RecentFilesBrowserData : ScriptableObject
    {
        public List<Object> favoriteSelections = new List<Object>();
        public List<Object> previousSelections = new List<Object>();
        public int maxItems = 20;

        [HideInInspector] public Vector2 scrollPos;
    }
}