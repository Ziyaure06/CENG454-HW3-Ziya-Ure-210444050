public interface IInteractable
{
    // Oyuncu etkileþim tuþuna ('E') basýlý tuttukça çaðrýlýr.
    // deltaTime: Etkileþimin sürece baðlý (zamanla duvarýn yükselmesi vb.) hesaplanabilmesi için gereklidir.
    void Interact(float deltaTime);
}
