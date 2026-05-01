// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Define el contrato para leer input de movimiento y disparo.
using UnityEngine;

public interface IMovementInputReader
{
    Vector2 ReadMoveInput();
    bool IsFirePressed();
}
