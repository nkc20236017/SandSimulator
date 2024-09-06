using UnityEngine;

public class ComputeShaderSand : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private float brushSize = 5f;
    private RenderTexture renderTexture;
    private int mainKernelIndex;
    private int addSandKernelIndex;
    private int eraseSandKernelIndex;

    private void Start()
    {
        renderTexture = new RenderTexture(width, height, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        mainKernelIndex = computeShader.FindKernel("CSMain");
        addSandKernelIndex = computeShader.FindKernel("AddSand");
        eraseSandKernelIndex = computeShader.FindKernel("EraseSand");
    }

    private void Update()
    {
        computeShader.SetTexture(mainKernelIndex, "Result", renderTexture);
        computeShader.SetTexture(addSandKernelIndex, "Result", renderTexture);
        computeShader.SetTexture(eraseSandKernelIndex, "Result", renderTexture);
        computeShader.SetInt("Width", width);
        computeShader.SetInt("Height", height);
        computeShader.SetFloat("Time", Time.time);

        Vector2 mousePos = Input.mousePosition;
        mousePos.x = mousePos.x * width / Screen.width;
        mousePos.y = mousePos.y * height / Screen.height;

        computeShader.SetVector("MousePos", mousePos);
        computeShader.SetFloat("BrushSize", brushSize);

        if (Input.GetMouseButton(0))
        {
            computeShader.Dispatch(addSandKernelIndex, width / 8, height / 8, 1);
        }
        else if (Input.GetMouseButton(1))
        {
            computeShader.Dispatch(eraseSandKernelIndex, width / 8, height / 8, 1);
        }

        computeShader.Dispatch(mainKernelIndex, width / 8, height / 8, 1);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(renderTexture, dest);
    }
}
