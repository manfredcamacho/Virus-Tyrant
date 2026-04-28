// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Define el contrato para leer input de movimiento y disparo.
using UnityEngine;

public interface IMovementInputReader
{
    Vector2 ReadMoveInput();
    bool IsFirePressed();
}
