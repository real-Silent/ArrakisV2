using System.Collections.Generic;
using UnityEngine;

namespace Arrakis.Extensions
{
    public static class MiscExt
    {
        public static IEnumerable<GameObject> Children(this Transform t)
        {
            var list = new List<GameObject>();
            for (int i = 0; i < t.childCount; i++)
                list.Add(t.GetChild(i).gameObject);
            return list;
        }
    }
}