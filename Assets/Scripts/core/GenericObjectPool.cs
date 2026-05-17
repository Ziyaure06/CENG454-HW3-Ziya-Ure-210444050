using UnityEngine;
using UnityEngine.Pool;

// T tipi kesinlikle bir MonoBehaviour olmalý ve IPoolable arayüzünü uygulamalýdýr.
public class GenericObjectPool<T> where T : MonoBehaviour, IPoolable
{
    private readonly IObjectPool<T> _pool;
    private readonly T _prefab;
    private readonly Transform _parentTransform;

    // Kurucu metot (Constructor)
    public GenericObjectPool(T prefab, int defaultCapacity = 10, int maxSize = 100, Transform parentTransform = null)
    {
        _prefab = prefab;
        _parentTransform = parentTransform;

        // Unity'nin dahili ObjectPool'unu konfigüre ediyoruz
        _pool = new ObjectPool<T>(
            createFunc: CreateItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    // 1. Havuzda obje kalmadýysa yeni obje üretir
    private T CreateItem()
    {
        T item = Object.Instantiate(_prefab, _parentTransform);
        return item;
    }

    // 2. Obje havuzdan sahneye istendiðinde çalýþýr
    private void OnTakeFromPool(T item)
    {
        item.gameObject.SetActive(true);
        item.OnSpawn(); // IPoolable sözleþmesi gereði OnSpawn tetiklenir
    }

    // 3. Obje iþini bitirip havuza geri dönerken çalýþýr
    private void OnReturnedToPool(T item)
    {
        item.OnDespawn(); // IPoolable sözleþmesi gereði Event'ler burada temizlenir!
        item.gameObject.SetActive(false);
    }

    // 4. Havuz kapasitesi dolduysa ve obje iade ediliyorsa, objeyi tamamen yok eder
    private void OnDestroyPoolObject(T item)
    {
        Object.Destroy(item.gameObject);
    }

    // Dýþarýdan havuzdan obje çekmek için kullanýlacak metot
    public T Get()
    {
        return _pool.Get();
    }

    // Dýþarýdan objeyi havuza geri yollamak için kullanýlacak metot
    public void Release(T item)
    {
        _pool.Release(item);
    }
}