using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Classe component qui va gérer tout l'aspect build
/// </summary>
public class Builder : MonoBehaviour
{
    [SerializeField]
    private Image _nextTilePreview;

    /// <summary>
    /// Masque de calque pour filtrer la grille de construction
    /// </summary>
    [SerializeField]
    private LayerMask _gridMask;

    /// <summary>
    /// Masque de calque pour filtrer les tuiles
    /// </summary>
    [SerializeField]
    private LayerMask _tileMask;

    /// <summary>
    /// Endroit où "stocker" les tuiles
    /// </summary>
    [SerializeField]
    private Transform _world;

    /// <summary>
    /// Pas de grille
    /// </summary>
    [SerializeField]
    private float _stepGrid;

    /// <summary>
    /// Référence de la caméra. Pour éviter d'appeler Camera.main
    /// </summary>
    [SerializeField]
    private Camera _camera;

    /// <summary>
    /// Transform du curseur
    /// </summary>
    [SerializeField]
    private Transform _builderCursor;

    /// <summary>
    /// Référence de la tuile fantôme guide
    /// </summary>
    [SerializeField]
    private GhostTile _ghostTile;

    /// <summary>
    /// Position du curseur
    /// </summary>
    private Vector3 _cursorPosition;

    /// <summary>
    /// Peut-on placer la prochaine tuile ?
    /// </summary>
    private bool _canPlaceNextTile;

    /// <summary>
    /// Inputs
    /// </summary>
    private GameInputs _gameInputs;

    /// <summary>
    /// Sommes-nous dans le cas d'un remplacement de tuile ?
    /// </summary>
    private bool _replace;

    /// <summary>
    /// Est-ce que la première tuile a été placée ?
    /// </summary>
    private bool _firstTilePlaced;

    /// <summary>
    /// Toutes les tuiles placées
    /// </summary>
    private List<GameObject> _tiles;

    /// <summary>
    /// Nom de la prochaine tuile à placer
    /// </summary>
    private string _nextTileName;

    // Start is called before the first frame update
    void Start()
    {
        _gameInputs = new GameInputs();
        _gameInputs.Enable();

        _tiles = new List<GameObject>();
        GetNextTile();
    }

    /// <summary>
    /// Lorsque l'on clique
    /// </summary>
    private void PlaceTile()
    {
        if (!_canPlaceNextTile)
            return;

        DestroyTileIfAlreadyExists();

        GameObject newTile = Instantiate(Resources.Load($"Tiles/_PFB_{_nextTileName}"), _cursorPosition, Quaternion.identity) as GameObject;
        newTile.transform.SetParent(_world);

        _tiles.Add(newTile);
        _ghostTile.Hide();

        GetNextTile();

        if (!_firstTilePlaced)
            _firstTilePlaced = true;
    }

    /// <summary>
    /// Récupère la prochaine tuile à placer
    /// </summary>
    public void GetNextTile()
    {
        string[] availablesTiles = new string[] { "GrassTile", "ForestTile", "RocksTile" };

        _nextTileName = availablesTiles[Random.Range(0, availablesTiles.Length)];
        _nextTilePreview.sprite = Resources.Load<Sprite>($"TilesPreview/_SPR_{_nextTileName}Preview");
    }

    /// <summary>
    /// Détruit la tuile à la position actuelle
    /// </summary>
    private void DestroyTileIfAlreadyExists()
    {
        GameObject tile = _tiles.Where(k => k.transform.position.x == _cursorPosition.x && k.transform.position.z == _cursorPosition.z).FirstOrDefault();

        if (tile == null)
            return;

        Destroy(tile);
        _tiles.Remove(tile);
    }

    // Update is called once per frame
    void Update()
    {
        MoveCursor();

        if (_gameInputs.UI.Click.WasPressedThisFrame())
            PlaceTile();
    }

    /// <summary>
    /// Déplace le curseur
    /// </summary>
    private void MoveCursor()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _gridMask))
        {
            Vector3 position = raycastHit.point;

            position.y = 0;

            position.x = Mathf.Round(position.x / _stepGrid) * _stepGrid;
            position.z = Mathf.Round(position.z / _stepGrid) * _stepGrid;

            if (_cursorPosition != position)
            {
                _cursorPosition = position;
                _builderCursor.position = _cursorPosition;
                UpdateGhostTilePlaceable();
            }
        }
    }

    /// <summary>
    /// Met à jour visuellement le fantôme pour informer si on peut placer une tuile ou non
    /// </summary>
    private void UpdateGhostTilePlaceable()
    {
        if (_ghostTile.IsHidden())
            _ghostTile.Show();

        _canPlaceNextTile = CheckIfCanPlaceTileHere();

        _ghostTile.SetCanBePlacedFeedback(_canPlaceNextTile);
    }

    /// <summary>
    /// Vérification pour savoir si on peut placer une tuile à cette position
    /// </summary>
    private bool CheckIfCanPlaceTileHere()
    {
        //Si on est dans le cas de tentative de placement de la première tuile. On retourne immédiatement vrai.
        if(!_firstTilePlaced)
            return true;

        //On vérifie dans la foulée si il s'agit d'une tentative de remplacement
        _replace = Physics.CheckSphere(_cursorPosition, 1f, _tileMask);

        return _replace
            || Physics.CheckSphere(_cursorPosition + Vector3.right, 1f, _tileMask)
            || Physics.CheckSphere(_cursorPosition - Vector3.right, 1f, _tileMask)
            || Physics.CheckSphere(_cursorPosition + Vector3.forward, 1f, _tileMask)
            || Physics.CheckSphere(_cursorPosition - Vector3.forward, 1f, _tileMask);
    }
}
