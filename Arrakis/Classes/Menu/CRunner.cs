using UnityEngine;

namespace Arrakis.Classes
{
    public class CRunner : MonoBehaviour
    {
        public static CRunner instance;
        public void Awake() =>
            instance = this;
    }
}