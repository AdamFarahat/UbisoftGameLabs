using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "BillboardSettingsSO", menuName = "Scriptable Objects/BillboardSettingsSO")]
public class BillboardSettingsSO : ScriptableObject
{
    private static BillboardSettingsSO _instance;
    public static BillboardSettingsSO Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<BillboardSettingsSO>("BillboardSettingsSO");
                Assert.IsNotNull(_instance, "BillboardSettingsSO not found in Resources!");
            }

            return _instance;
        }
    }

    [SerializeField] private float globalZOffset = -100f;
    public float GlobalZOffset => globalZOffset;
}
