using UnityEngine;

public class StageObjectScript : GenericScript
{
    [SerializeField] protected GameObject player;

    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask noLayer;
}
