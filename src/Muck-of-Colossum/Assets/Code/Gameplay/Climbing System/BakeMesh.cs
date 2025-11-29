using UnityEngine;

/// <summary>
/// Обновляет анимированный меш.
/// Два режима:
///   FastUpdate – только вертексы для навигации (без коллайдера).
///   FullUpdate – ещё и MeshCollider (дорого).
/// </summary>
[RequireComponent(typeof(SkinnedMeshRenderer), typeof(MeshCollider))]
public class BakeMesh : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;
    private Mesh bakedMesh;

    [Header("Настройки")]
    [Tooltip("Обновлять ли коллайдер каждый кадр. Если выключено – он обновляется только вручную.")]
    [SerializeField] private bool updateColliderEachFrame = false;

    [Tooltip("Пропускать обновление, если объект не виден камерой.")]
    [SerializeField] private bool skipIfNotVisible = true;

    public Mesh BakedMesh => bakedMesh;

    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        bakedMesh = new Mesh();
        
        ForceUpdateCollider();
    }

    void LateUpdate()
    {
        if (skipIfNotVisible && !skinnedMeshRenderer.isVisible)
            return;

        // Быстрое обновление вершин
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        if (updateColliderEachFrame)
        {
            // Дорого, но точно
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

    public void ForceUpdateMesh()
    {
        skinnedMeshRenderer.BakeMesh(bakedMesh);

    }
}