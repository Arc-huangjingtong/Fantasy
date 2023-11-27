using UnityEngine;
using System.Collections.Generic;
using Fantasy;

public partial class GameManager : MonoBehaviour
{
    public Location location;

    public static ISend _sender = null;
    public static ISend sender => _sender;

    public static Response _response = null;
    public static Response response => _response;

    private static GameManager _instance = null;
    public static GameManager Ins => _instance;
    public bool DontDestroy = true;

    // 可选角色类集
    [HideInInspector] public List<Player> playerClasses = new List<Player>(); 
    [HideInInspector] public List<UnitBase> unitClasses = new List<UnitBase>(); 
    
    private void Awake()
    {
        _sender = GameObject.Find("Network").GetComponent<ISend>();
        _response = GameObject.Find("Network").GetComponent<Response>();

        // 注意&与&&区别
        if (_instance != null & _instance == this)
            return;

        _instance = this;
        if(_instance.DontDestroy) DontDestroyOnLoad(gameObject);

        FindUnitPrefabs();
    }

    private void Start()
    {
        // 职业与角色预览器初始
        // roleViewer = gameObject.AddComponent<RoleViewer>();
        // classViewer = gameObject.AddComponent<ClassViewer>();
    }

    // 获取unit的角色预制体对象
    public void FindUnitPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Unit");
        playerClasses = FindUnitClasses<Player>(prefabs);
        unitClasses = FindUnitClasses<UnitBase>(prefabs);
    }

    public List<T> FindUnitClasses<T>(GameObject[] prefabs)
    {
        List<T> classes = new List<T>();
        foreach (GameObject prefab in prefabs)
        {
            T entity = prefab.GetComponent<T>();
            if (entity != null)
                classes.Add(entity);
        }
        return classes;
    }

    // 获取Unit
    public GameObject GetUnit(string prefabName,string name = null)
    {
        return Pool.Instance.Get(PoolType.Unit,prefabName,name);
    }
    

    // 将Unit放回对象池
    public void PushUnit(string name, GameObject item)
    {
        Pool.Instance.Push(PoolType.Unit,name, item);
    }
}