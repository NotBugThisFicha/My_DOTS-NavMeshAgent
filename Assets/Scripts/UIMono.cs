
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UITextCountMono : MonoBehaviour
{
    public Text text;
    private Entity numb;
    private EntityManager manager;
    private UnitSpawnerPropertys prop;
    public int num;
    private bool isit;

    public static UITextCountMono Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        //manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //numb = manager.CreateEntity();
        //manager.AddComponent<UITextCountComponent>(numb);

    }

    private void OnDestroy()
    {
        Instance = null;
    }
    public void SetCount(int value)
    {
        text.text = $"{value}";
    }
}
