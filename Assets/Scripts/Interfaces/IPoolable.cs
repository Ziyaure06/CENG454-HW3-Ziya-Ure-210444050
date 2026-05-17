public interface IPoolable
{
    // Obje havuzdan sahneye çaðrýldýðýnda çalýþýr (Örn: Caný sýfýrlamak, konumu ayarlamak için)
    void OnSpawn();

    // Obje ölüp/kullanýlýp havuza geri döndüðünde çalýþýr.
    // KESÝN KURAL: Tüm Event Unsubscribe (olay abonelik iptalleri) burada yapýlmalýdýr!
    void OnDespawn();
}