using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IPoolable
{
    [Header("Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 1f; // D��man can� 1 oldu�u i�in 1 yeterli
    [SerializeField] private float lifetime = 3f; // Hi�bir �eye �arpmazsa ka� saniye sonra silinsin?

    // Havuz sisteminin bu mermiyi geri alabilmesi i�in Action
    public Action<Projectile> ReturnToPoolAction;

    private Rigidbody _rb;
    private float _lifeTimer;
    private bool _isDeactivated = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false; // Mermi havada s�z�lmeli
        _rb.isKinematic = false; // �arp��ma alg�lamak i�in Kinematic OLMAMALI (ama yer�ekimsiz)

        // �arp��malar�n d�zg�n alg�lanmas� i�in BoxCollider'da IsTrigger se�ili olmal� (Unity'den ayarla)
    }

    // IPoolable S�zle�mesi: Havuzdan ��karken �al���r
    public void OnSpawn()
    {
        _isDeactivated = false;
        _lifeTimer = lifetime;
    }

    // D��ar�dan silah�n mermiyi f�rlatmak i�in �a��rd��� metot
    public void Launch(Vector3 direction)
    {
        _rb.linearVelocity = direction.normalized * speed;
        // Merminin rotasyonunu gidi� y�n�ne �evir
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void Update()
    {
        if (_isDeactivated) return;

        // Zaman a��m� kontrol�
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDeactivated) return;

        // �arpt���m�z obje hasar alabiliyor mu? (�rn: D��man)
        IDamageable hitObject = other.GetComponent<IDamageable>();

        // �NEML�: Merminin oyuncunun kendisine �arpmas�n� engellemeliyiz
        if (hitObject != null && other.gameObject.tag != "Player")
        {
            hitObject.TakeDamage(damage);
            Deactivate(); // Hasar verince mermi yok olur
        }
        // E�er hasar almayan static bir objeye (�rn: yer) �arparsa da yok olsun
        else if (other.gameObject.isStatic)
        {
            Deactivate();
        }
    }

    // IPoolable S�zle�mesi: Havuza geri d�nerken �al���r
    public void OnDespawn()
    {
        // BUG �NLEME KURALI: Action referans�n� temizle
        ReturnToPoolAction = null;
        _rb.linearVelocity = Vector3.zero; // H�z� s�f�rla
    }

    private void Deactivate()
    {
        if (_isDeactivated) return;
        _isDeactivated = true;
        ReturnToPoolAction?.Invoke(this); // Havuza geri d�nme iste�i
    }
}