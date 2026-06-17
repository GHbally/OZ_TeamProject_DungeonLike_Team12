using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float tileSize = 20f;

    private void LateUpdate()
    {
        if (player == null) return;

        float halfTile = tileSize / 2f;

        float diffX = player.position.x - transform.position.x;
        float diffY = player.position.y - transform.position.y;

        float newX = transform.position.x;
        float newY = transform.position.y;
        bool hasMoved = false;

        if (Mathf.Abs(diffX) > halfTile)
        {
            float offset = (diffX > 0) ? tileSize : -tileSize;
            newX += offset;
            hasMoved = true;
        }

        if (Mathf.Abs(diffY) > halfTile)
        {
            float offset = (diffY > 0) ? tileSize : -tileSize;
            newY += offset;
            hasMoved = true;
        }

        if (hasMoved)
        {
            transform.position = new Vector3(newX, newY, transform.position.z);
        }
    }
}
