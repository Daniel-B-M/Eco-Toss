using UnityEngine;

namespace EcoToss.Core
{
    /// <summary>
    /// ScriptableObject defining item data for a specific waste type,
    /// shared by all corresponding item instances in the scene.
    /// </summary>
    [CreateAssetMenu(fileName = "WasteData_", menuName = "EcoToss/Waste Data")]
    public class WasteData : ScriptableObject
    {
        [Header("Identidad")]
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string hint;

        [Header("Clasificación")]
        [SerializeField] private BinType correctBin;

        [Header("Visual")]
        [SerializeField] private GameObject prefab;

        // Public read-only accessors
        public string DisplayName => displayName;
        public string Hint => hint;
        public BinType CorrectBin => correctBin;
        public GameObject Prefab => prefab;
    }
}
