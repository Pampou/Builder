using UnityEngine;

/// <summary>
/// Component attach� � une tuile
/// </summary>
public class Tile : MonoBehaviour
{
    /// <summary>
    /// R�f�rence du transform qui contient l'aspect visuel de la tuile
    /// </summary>
    [SerializeField]
    private Transform _render;

    // Start is called before the first frame update
    void Start()
    {
        ApplyRandomRotation();
    }

    /// <summary>
    /// Applique une rotation al�atoire sur l'axe Y
    /// </summary>
    private void ApplyRandomRotation()
    {
        float yAxisRandom = 90 * Random.Range(0, 4);

        _render.localRotation = Quaternion.Euler(0, yAxisRandom, 0);
    }

}
