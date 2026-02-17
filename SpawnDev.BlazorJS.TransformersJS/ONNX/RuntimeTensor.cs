using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS.ONNX
{
    /// <summary>
    /// Base class for runtime tensors in ONNX Runtime Web.<br/>
    /// Represents multi-dimensional arrays used in model inferencing.<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/Tensor-1.html
    /// </summary>
    public class RuntimeTensor : JSObject
    {
        /// <inheritdoc/>
        public RuntimeTensor(IJSInProcessObjectReference _ref) : base(_ref) { }

        /// <summary>
        /// Dimensions of the tensor.
        /// </summary>
        public virtual long[] Dims => JSRef!.Get<long[]>("dims");

        /// <summary>
        /// Type of the tensor.<br/>
        /// Examples: "float32", "int32", "int64", "string", etc.
        /// </summary>
        public virtual string Type => JSRef!.Get<string>("type");

        /// <summary>
        /// DataLocation: "none" | "cpu" | "cpu-pinned" | "texture" | "gpu-buffer" | "ml-tensor"
        /// </summary>
        public virtual string Location => JSRef!.Get<string>("location");

        /// <summary>
        /// The number of elements in the tensor.
        /// </summary>
        public virtual long Size => JSRef!.Get<long>("size");
    }
}
