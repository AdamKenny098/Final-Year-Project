using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonRoomOptimizer : MonoBehaviour
{
    private DungeonRoomBuilder builder;
    private DungeonRoomDecorator decorator;
    private Transform floorRoot;

    public Material wallMaterial;
    public Material floorCeilingMaterial;
    public GameObject floorExitPrefab;

    public void StartOptimization()
    {
        DeleteNodes();

        ReduceCollidersOnCurrentFloor("dungeonFloor");
        ReduceCollidersOnCurrentFloor("dungeonCeiling");
        ReduceCollidersOnCurrentFloor("dungeonWall");

        CombineBlockMeshes();   // Pass 1: blocks -> room section mesh
        CombineFloorShells();   // Pass 2: room sections -> floor shell mesh

        StartCoroutine(DelayedNavMeshAddition());
    }

    public void DeleteNodes()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach (GameObject node in nodes)
        {
            Destroy(node);
        }
    }

    void ReduceCollidersOnCurrentFloor(string targetTag)
    {
        if (floorRoot == null) return;

        foreach (Transform t in floorRoot.GetComponentsInChildren<Transform>(true))
        {
            if (!t.CompareTag(targetTag)) continue;

            Collider[] childColliders = t.GetComponentsInChildren<Collider>(true);
            if (childColliders.Length == 0) continue;

            Bounds bounds = new Bounds(childColliders[0].bounds.center, Vector3.zero);

            foreach (Collider col in childColliders)
            {
                bounds.Encapsulate(col.bounds);
            }

            BoxCollider boxC = t.GetComponent<BoxCollider>();
            if (boxC == null)
                boxC = t.gameObject.AddComponent<BoxCollider>();

            boxC.center = t.InverseTransformPoint(bounds.center);
            boxC.size = bounds.size;

            foreach (Collider col in childColliders)
            {
                if (col != boxC)
                    Destroy(col);
            }
        }
    }

    public void CombineBlockMeshes()
    {
        foreach (Room room in builder.allRooms)
        {
            if (room == null) continue;

            foreach (Transform child in room.transform)
            {
                if (child.name != "Walls" && child.name != "Floor" && child.name != "Ceiling")
                    continue;

                List<MeshFilter> meshesToCombine = new List<MeshFilter>();

                foreach (Transform grandChild in child)
                {
                    MeshFilter filter = grandChild.GetComponent<MeshFilter>();
                    if (filter != null && filter.sharedMesh != null)
                    {
                        meshesToCombine.Add(filter);
                    }
                }

                if (meshesToCombine.Count == 0)
                    continue;

                CombineInstance[] combine = new CombineInstance[meshesToCombine.Count];

                for (int i = 0; i < meshesToCombine.Count; i++)
                {
                    combine[i].mesh = meshesToCombine[i].sharedMesh;
                    combine[i].transform = child.worldToLocalMatrix * meshesToCombine[i].transform.localToWorldMatrix;
                }

                Mesh combinedMesh = new Mesh();
                combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                combinedMesh.CombineMeshes(combine, true, true);
                combinedMesh.RecalculateBounds();
                combinedMesh.RecalculateNormals();

                MeshFilter childMF = child.GetComponent<MeshFilter>();
                if (childMF == null)
                    childMF = child.gameObject.AddComponent<MeshFilter>();

                MeshRenderer childMR = child.GetComponent<MeshRenderer>();
                if (childMR == null)
                    childMR = child.gameObject.AddComponent<MeshRenderer>();

                childMF.sharedMesh = combinedMesh;

                if (child.name == "Walls")
                    childMR.sharedMaterial = wallMaterial;
                else
                    childMR.sharedMaterial = floorCeilingMaterial;

                foreach (Collider c in child.GetComponents<Collider>())
                {
                    Destroy(c);
                }

                if (child.name == "Walls")
                {
                    MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = combinedMesh;
                    mc.convex = false;
                }
                else
                {
                    BoxCollider bc = child.gameObject.AddComponent<BoxCollider>();
                    bc.center = combinedMesh.bounds.center;
                    bc.size = combinedMesh.bounds.size;
                }

                List<GameObject> toDestroy = new List<GameObject>();
                foreach (Transform grandChild in child)
                {
                    toDestroy.Add(grandChild.gameObject);
                }

                for (int i = 0; i < toDestroy.Count; i++)
                {
                    Destroy(toDestroy[i]);
                }
            }
        }
    }

    public void CombineFloorShells()
    {
        if (floorRoot == null)
        {
            Debug.LogWarning("No floorRoot assigned to optimizer.");
            return;
        }

        CombineCategory("dungeonWall", "CombinedWalls", wallMaterial, true);
        CombineCategory("dungeonFloor", "CombinedFloors", floorCeilingMaterial, false);
        CombineCategory("dungeonCeiling", "CombinedCeilings", floorCeilingMaterial, false);
    }

    public void CombineCategory(string parentTag, string combinedName, Material material, bool addMeshCollider)
    {
        Transform existing = floorRoot.Find(combinedName);
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        List<MeshFilter> sourceMeshFilters = new List<MeshFilter>();

        foreach (Transform t in floorRoot.GetComponentsInChildren<Transform>(true))
        {
            if (!t.CompareTag(parentTag)) continue;

            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                sourceMeshFilters.Add(mf);
            }
        }

        if (sourceMeshFilters.Count == 0)
        {
            Debug.Log($"No meshes found for {combinedName} under {floorRoot.name}");
            return;
        }

        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (MeshFilter mf in sourceMeshFilters)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = floorRoot.worldToLocalMatrix * mf.transform.localToWorldMatrix;
            combine.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = $"{floorRoot.name}_{combinedName}";
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);
        combinedMesh.RecalculateBounds();
        combinedMesh.RecalculateNormals();

        GameObject combinedObj = new GameObject(combinedName);
        combinedObj.transform.SetParent(floorRoot, false);
        combinedObj.transform.localPosition = Vector3.zero;
        combinedObj.transform.localRotation = Quaternion.identity;
        combinedObj.transform.localScale = Vector3.one;
        combinedObj.isStatic = true;

        MeshFilter newMF = combinedObj.AddComponent<MeshFilter>();
        MeshRenderer newMR = combinedObj.AddComponent<MeshRenderer>();

        newMF.sharedMesh = combinedMesh;
        newMR.sharedMaterial = material;

        if (addMeshCollider)
        {
            MeshCollider mc = combinedObj.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
            mc.convex = false;
        }
        else if (combinedName == "CombinedFloors")
        {
            MeshCollider mc = combinedObj.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
            mc.convex = false;
        }

        DisableSourceSectionObjects(sourceMeshFilters);
    }

    public void DisableSourceSectionObjects(List<MeshFilter> meshFilters)
    {
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf == null) continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.enabled = false;

            Collider[] colliders = mf.GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }
    }

    public IEnumerator DelayedNavMeshAddition()
    {
        yield return new WaitForEndOfFrame();

        Transform combinedFloors = floorRoot.Find("CombinedFloors");
        if (combinedFloors == null)
        {
            Debug.LogWarning("CombinedFloors not found, cannot build NavMesh.");
            yield break;
        }

        NavMeshSurface navMeshSurface = combinedFloors.GetComponent<NavMeshSurface>();
        if (navMeshSurface == null)
            navMeshSurface = combinedFloors.gameObject.AddComponent<NavMeshSurface>();

        navMeshSurface.BuildNavMesh();

        DungeonEntitySpawner spawner = FindObjectOfType<DungeonEntitySpawner>();
        if (spawner != null)
            spawner.SpawnAll();

        SimpleAI[] enemies = FindObjectsOfType<SimpleAI>();
        foreach (SimpleAI ai in enemies)
        {
            ai.RebindToNavMesh();
            ai.EnableAI();
        }
    }

    public void CollectBounds()
    {
        foreach (Room room in decorator.allWorkableRooms)
        {
            room.occupiedAreas.Clear();

            Transform decorRoot = room.transform.Find("Room Decorations");
            if (decorRoot == null) continue;

            Collider[] colliders = decorRoot.GetComponentsInChildren<Collider>(false);

            foreach (Collider col in colliders)
            {
                room.occupiedAreas.Add(col.bounds);
            }
        }
    }

    public void FillReferences(DungeonRoomBuilder builder, DungeonRoomDecorator decorator, Transform floorRoot)
    {
        this.builder = builder;
        this.decorator = decorator;
        this.floorRoot = floorRoot;
    }

    public void DestroyOldRoomShellObjects()
    {
        foreach (Room room in builder.allRooms)
        {
            if (room == null) continue;

            List<GameObject> toDestroy = new List<GameObject>();

            foreach (Transform child in room.transform)
            {
                if (child.name == "Walls" || child.name == "Floor" || child.name == "Ceiling")
                {
                    toDestroy.Add(child.gameObject);
                }
            }

            for (int i = 0; i < toDestroy.Count; i++)
            {
                Destroy(toDestroy[i]);
            }
        }
    }
}