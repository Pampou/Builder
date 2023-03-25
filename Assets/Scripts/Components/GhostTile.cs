using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTile : MonoBehaviour
{
    [SerializeField]
    private Color _canBuildColor;

    [SerializeField]
    private Color _cannotBuildColor;

    [SerializeField]
    private Material _referenceMaterial;

    [SerializeField]
    private Renderer[] _renderers;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsHidden()
    {
        return !gameObject.activeSelf;
    }

    public void SetCanBePlacedFeedback(bool canPlaceNextTile)
    {
        foreach(Renderer renderer in _renderers)
        {
            renderer.material.color = canPlaceNextTile ? _canBuildColor : _cannotBuildColor;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
