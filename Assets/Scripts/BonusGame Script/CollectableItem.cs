using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public enum CollectableType
    {
        TypeA,
        TypeB
    }

    public CollectableType type;
    public BonusGameStageTwo stageTwoController;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!stageTwoController)
            return;

        if (!other.CompareTag("Player"))
            return;

        stageTwoController.RegisterCollectable(type);
        Destroy(gameObject);
    }

    public void SetType(CollectableType newType)
    {
        type = newType;
    }
}
