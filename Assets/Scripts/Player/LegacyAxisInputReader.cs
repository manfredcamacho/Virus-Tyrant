// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Lee input Legacy Input Manager para movimiento y disparo.
using UnityEngine;

public class LegacyAxisInputReader : MonoBehaviour, IMovementInputReader
{
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private string fireButton = "Fire1";

    public Vector2 ReadMoveInput()
    {
        float x = Input.GetAxis(horizontalAxis);
        float y = Input.GetAxis(verticalAxis);
        return new Vector2(x, y);
    }

    public bool IsFirePressed()
    {
        return Input.GetButton(fireButton);
    }
}
