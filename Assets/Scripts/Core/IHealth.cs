// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Define operaciones basicas para componentes de salud.
public interface IHealth
{
    int CurrentLives { get; }
    bool IsDead { get; }
    void TakeDamage(int damageAmount = 1);
}
