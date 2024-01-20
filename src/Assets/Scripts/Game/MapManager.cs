using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject fan;
    public GameObject plateform;
    public GameObject plateformWaypoints;
    public GameObject[] groundsToMove;
    public GameObject[] groundsToDelete;

    public static MapManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
    
        instance = this;
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex == 0)
        {
            fan.SetActive(false);
            plateform.SetActive(false);
            plateformWaypoints.SetActive(false);
        }
        else if (levelIndex == 1)
        {
            fan.SetActive(true);
            plateform.SetActive(true);
            plateformWaypoints.SetActive(true);

            for (int i = 0; i < groundsToMove.Length; i++)
            {
                Vector3 pos = groundsToMove[i].transform.localPosition;
                groundsToMove[i].transform.localPosition = new Vector3(pos.x, pos.y + 0.5f, pos.z);
            }

            for (int i = 0; i < groundsToDelete.Length; i++)
            {
                Destroy(groundsToDelete[i].gameObject);
            }
        }
    }
}
