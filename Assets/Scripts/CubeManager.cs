using UnityEngine;
using System.Collections.Generic;


// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// Controller for adding additonal cubes that are joining the session
// Listens to the blackboard events when players are added/removed
public class CubeManager : MonoBehaviour
{
    /// <summary>
    /// Prefab reference for the CubePanel to spawn
    /// </summary>
    [SerializeField] private GameObject cubePrefab; // pass in the CubePanel prefab
    [SerializeField] private float spacing = 2.5f;
    [SerializeField] private Vector3 startPosition = Vector3.zero;
    private readonly Dictionary<int, GameObject> activeCubes = new();

    /// <summary>
    /// Initialize event subscriptions
    /// </summary>
    private void Start()
    {
        Blackboard.Instance.OnPlayerAdded += HandlePlayerAdded;
        Blackboard.Instance.OnPlayerRemoved += HandlePlayerRemoved;
    }

    /// <summary>
    /// Cleanup event subscriptions
    /// </summary>
    private void OnDestroy()
    {
        Blackboard.Instance.OnPlayerAdded -= HandlePlayerAdded;
        Blackboard.Instance.OnPlayerRemoved -= HandlePlayerRemoved;
    }

    /// <summary>
    /// Handles a player added event 
    /// </summary>
    /// <param name="slot"></param>
    private void HandlePlayerAdded(int slot)
    {
        SpawnCube(slot);
    }

    /// <summary>
    /// Remove the cube at the given slot index
    /// </summary>
    /// <param name="slot"></param>
    private void HandlePlayerRemoved(int slot)
    {
        if (activeCubes.TryGetValue(slot, out GameObject cube))
        {
            Destroy(cube);
            activeCubes.Remove(slot);
            Debug.Log($"[CubeManager] Removed cube for slot {slot}");
        }
    }

    /// <summary>
    ///  Spawn a cube using the CubePanel Prefab at the given slot index
    /// </summary>
    /// <param name="slot"></param>
    private void SpawnCube(int slot)
    {
        Vector3 pos = startPosition + new Vector3(slot * spacing, 0f, 0f);

        GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity);
        activeCubes[slot] = cube;

        CubePanel panel = cube.GetComponent<CubePanel>();
        panel.SlotIndex = slot;

        Debug.Log($"[CubeManager] Spawned cube for slot {slot}");
    }
}
