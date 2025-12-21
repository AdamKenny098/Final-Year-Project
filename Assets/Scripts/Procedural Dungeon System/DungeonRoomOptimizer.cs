using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonRoomOptimizer : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartOptimization()
    {
        DeleteNodes();

        ReduceColliders("dungeonFloor");
        ReduceColliders("dungeonCeiling");
        ReduceColliders("dungeonWall");

        CombineBlockMeshes();
        DungeonRoomBuilder.Instance.AddNavMeshSurface();
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

    public void ReduceColliders(string targetTag)
    {

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(targetTag))
        {
            Collider[] childColliders = obj.GetComponentsInChildren<Collider>();

            //New bounds is not empty. Check what happens when nothing is passed in (Colliders with massive areas)
            if (childColliders.Length == 0) continue;
            
            Bounds singleBounds = new Bounds(childColliders[0].bounds.center, Vector3.zero);
            foreach (Collider col in childColliders)
            {
                //Engulf all child colliders into one
                singleBounds.Encapsulate(col.bounds);
            }

            BoxCollider boxC = obj.GetComponent<BoxCollider>();
            if (boxC == null)
            {
                boxC = obj.AddComponent<BoxCollider>();
            }
            boxC.center = obj.transform.InverseTransformPoint(singleBounds.center);
            boxC.size = singleBounds.size;

            foreach (Collider col in childColliders)
            {

                Destroy(col);
            }

        }
    }

    public void CombineBlockMeshes()
    {
        foreach (Room room in DungeonRoomBuilder.Instance.allRooms)
        {
            foreach (Transform child in room.transform)
            {
                BoxCollider boxC = child.gameObject.GetComponent<BoxCollider>();
                if (child.name == "Walls" || child.name == "Floor" || child.name == "Ceiling")
                {

                    List<MeshFilter> meshesToCombine = new List<MeshFilter>();

                    foreach (Transform grandChild in child)
                    {
                        BoxCollider boxD = grandChild.gameObject.GetComponent<BoxCollider>();
                        if (boxC.bounds.Intersects(boxD.bounds))
                        {
                            MeshFilter filter = grandChild.GetComponent<MeshFilter>();
                            if (filter != null)
                            {
                                meshesToCombine.Add(filter);
                            }

                        }
                    }

                    if (meshesToCombine.Count == 0) continue;

                    // Combine meshes: https://docs.unity3d.com/6000.2/Documentation/ScriptReference/CombineInstance.html
                    // CombineInstance holds which mesh and where it is.
                    CombineInstance[] combine = new CombineInstance[meshesToCombine.Count];
                    for (int i = 0; i < meshesToCombine.Count; i++)
                    {
                        combine[i].mesh = meshesToCombine[i].sharedMesh;
                        combine[i].transform = meshesToCombine[i].transform.localToWorldMatrix;
                    }

                    Mesh combinedMesh = new Mesh();
                    combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Avoid the limitation of 65535 vertices
                    //Combine into one mesh and use the transforms of world space
                    //https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
                    combinedMesh.CombineMeshes(combine, true, true);

                    MeshFilter childMF = child.GetComponent<MeshFilter>();
                    if (childMF == null)
                    {
                        childMF = child.gameObject.AddComponent<MeshFilter>();
                    }

                    MeshRenderer childMR = child.GetComponent<MeshRenderer>();

                    if (childMR == null)
                    {
                        childMR = child.gameObject.AddComponent<MeshRenderer>();
                    }

                    childMF.sharedMesh = combinedMesh;
                    childMR.sharedMaterial = meshesToCombine[0].GetComponent<MeshRenderer>().sharedMaterial;

                    foreach (Transform grandChild in child)
                    {
                        Destroy(grandChild.gameObject);
                    }

                }
            }
        }
    }

    public IEnumerator DelayedNavMeshAddition()
    {
        yield return new WaitForEndOfFrame();
        
        foreach (GameObject floor in DungeonRoomBuilder.Instance.floors)
        {
            NavMeshSurface navMeshSurface = floor.GetComponent<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
        }
    }

    public void CollectBounds()
    {
        foreach (Room room in DungeonRoomDecorator.Instance.allWorkableRooms)
        {
            room.occupiedAreas.Clear();

            Transform decorRoot = room.transform.Find("Room Decorations");

            Collider[] colliders = decorRoot.GetComponentsInChildren<Collider>(false);

            foreach (Collider col in colliders)
            {
                room.occupiedAreas.Add(col.bounds);
            }
        }
    }

}
