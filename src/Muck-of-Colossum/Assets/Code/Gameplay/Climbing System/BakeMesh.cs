using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer), typeof(MeshCollider))]
public class BakeMesh : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;
    private Mesh bakedMesh;

    [Header("Настройки")]
    [Tooltip("Обновлять ли коллайдер каждый кадр. Если выключено – он обновляется только вручную.")]
    [SerializeField] public bool updateColliderEachFrame = false;

    [Tooltip("Пропускать обновление, если объект не виден камерой.")]
    [SerializeField] private bool skipIfNotVisible = true;

    public Mesh BakedMesh => bakedMesh;

    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        bakedMesh = new Mesh();
        
        //ForceUpdateCollider();
    }

    void LateUpdate()
    {
        if (skipIfNotVisible && !skinnedMeshRenderer.isVisible)
            return;
        
        

        skinnedMeshRenderer.BakeMesh(bakedMesh);
        
        if (updateColliderEachFrame)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = bakedMesh;
        }
    }

    /// <summary>
    /// Принудительно обновить MeshCollider (например, в момент зацепа).
    /// </summary>
    public void ForceUpdateCollider()
    {
        skinnedMeshRenderer.BakeMesh(bakedMesh);
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = bakedMesh;
    }

    public void ClearBakedMesh()
    {
        meshCollider.sharedMesh = null;
    } 

    public void ForceUpdateMesh()
    {
        skinnedMeshRenderer.BakeMesh(bakedMesh);

    }
}