// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Define operaciones basicas para componentes de salud.
public interface IHealth
{
    int CurrentLives { get; }
    bool IsDead { get; }
    void TakeDamage(int damageAmount = 1);
}
