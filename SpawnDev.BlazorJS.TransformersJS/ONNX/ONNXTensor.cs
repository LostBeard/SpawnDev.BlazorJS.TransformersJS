using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS.ONNX
{
    /// <summary>
    /// Represent multi-dimensional arrays to feed to or fetch from model inferencing.<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/Tensor-1.html
    /// </summary>
    /// <typeparam name="TData">Array&lt;string> | Int8Array | Uint8Array | Int16Array | Uint16Array | Int32Array | Uint32Array | Float16Array | Float32Array | Float64Array | BigInt64Array | BigUint64Array</typeparam>
    public class ONNXTensor<TData> : ONNXTensor
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public ONNXTensor(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// The tensor data as type TData
        /// </summary>
        public TData Data => JSRef!.Get<TData>("data");
        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <param name="dims"></param>
        /// <returns></returns>
        public override ONNXTensor<TData> Reshape(IEnumerable<int> dims) => JSRef!.Call<ONNXTensor<TData>>("reshape", dims)!;
    }
    /// <summary>
    /// Represent multi-dimensional arrays to feed to or fetch from model inferencing.<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/TensorConstructor.html<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/Tensor-1.html
    /// </summary>
    public class ONNXTensor : RuntimeTensor
    {
        /// <inheritdoc/>
        public ONNXTensor(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Dimensions of the tensor.
        /// </summary>
        public override long[] Dims => JSRef!.Get<long[]>("dims");
        /// <summary>
        /// Type of the tensor.<br/>
        /// Example:
        /// - "float32"<br/>
        /// </summary>
        public override string Type => JSRef!.Get<string>("type");
        /// <summary>
        /// DataLocation: "none" | "cpu" | "cpu-pinned" | "texture" | "gpu-buffer" | "ml-tensor"
        /// </summary>
        public override string Location => JSRef!.Get<string>("location");
        /// <summary>
        /// The number of elements in the tensor.
        /// </summary>
        public override long Size => JSRef!.Get<long>("size");
        /// <summary>
        /// Get the WebGPU buffer that holds the tensor data.<br/>
        /// If the data is not on GPU as WebGPU buffer, throw error.
        /// </summary>
        public GPUBuffer GPUBuffer => JSRef!.Get<GPUBuffer>("gpuBuffer");
        /// <summary>
        /// Get the WebGL texture that holds the tensor data.<br/>
        /// If the data is not on GPU as WebGL texture, throw error.
        /// </summary>
        public WebGLTexture Texture => JSRef!.Get<WebGLTexture>("texture");
        /// <summary>
        /// Get the buffer data of the tensor.<br/>
        /// If the data is not on CPU (eg. it's in the form of WebGL texture or WebGPU buffer), throw error.
        /// </summary>
        public TData Get_Data<TData>() => JSRef!.Get<TData>("data");
        /// <summary>
        /// Get the buffer data of the tensor.<br/>
        /// If the data is on CPU, returns the data immediately. If the data is on GPU, downloads the data and returns the promise.
        /// </summary>
        /// <typeparam name="TData">string[] | Int8Array | Uint8Array | Int16Array | Uint16Array | Int32Array | Uint32Array | Float32Array | Float64Array | BigInt64Array | BigUint64Array</typeparam>
        /// <returns></returns>
        public Task<TData> GetData<TData>() => JSRef!.CallAsync<TData>("getData");
        /// <summary>
        /// Get the buffer data of the tensor.<br/>
        /// If the data is on CPU, returns the data immediately. If the data is on GPU, downloads the data and returns the promise.
        /// </summary>
        /// <typeparam name="TData">string[] | Int8Array | Uint8Array | Int16Array | Uint16Array | Int32Array | Uint32Array | Float32Array | Float64Array | BigInt64Array | BigUint64Array</typeparam>
        /// <param name="releaseData">whether release the data on GPU. Ignore if data is already on CPU.</param>
        /// <returns></returns>
        public Task<TData> GetData<TData>(bool releaseData) => JSRef!.CallAsync<TData>("getData", releaseData);
        /// <summary>
        /// creates a DataURL instance from tensor
        /// </summary>
        /// <param name="options">An optional object representing options for creating a DataURL instance from the tensor.</param>
        /// <returns></returns>
        public string ToDataURL(TensorToDataUrlOptions? options = null) => options == null ? JSRef!.Call<string>("toDataURL") : JSRef!.Call<string>("toDataURL", options);
        /// <summary>
        /// creates an ImageData instance from tensor
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public string ToImageData(TensorToImageDataOptions? options = null) => options == null ? JSRef!.Call<string>("toImageData") : JSRef!.Call<string>("toImageData", options);
        /// <summary>
        /// Calls dispose on the Javascript object and optionally (default) disposes the JSRef also<br/>
        /// </summary>
        /// <param name="disposeJSRef"></param>
        public void DisposeJS(bool disposeJSRef = true)
        {
            JSRef!.CallVoid("dispose");
            if (disposeJSRef) Dispose();
        }
        /// <summary>
        /// Create a new tensor with the same data buffer and specified dims.
        /// </summary>
        /// <param name="dims">New dimensions. Size should match the old one.</param>
        /// <returns></returns>
        public virtual ONNXTensor Reshape(IEnumerable<int> dims) => (ONNXTensor)JSRef!.Call(GetType(), "reshape", dims)!;
        /// <summary>
        /// Create a new tensor with the same data buffer and specified dims.
        /// </summary>
        /// <param name="dims">New dimensions. Size should match the old one.</param>
        /// <returns></returns>
        public virtual ONNXTensor<TData> Reshape<TData>(IEnumerable<int> dims) => JSRef!.Call<ONNXTensor<TData>>("reshape", dims)!;

        /// <summary>
        /// Create a tensor from a WebGPU buffer
        /// https://github.com/microsoft/onnxruntime/blob/cee825d34d533ca325bfd8f8269c86133ae512e6/js/common/lib/tensor-factory.ts#L357
        /// </summary>
        public static ONNXTensor FromGPUBuffer(GPUBuffer buffer, TensorFromGpuBufferOptions options) => JS.Call<ONNXTensor>("ONNXTensor.fromGPUBuffer", buffer, options);
    }
    /// <summary>
    /// 
    /// </summary>
    public class TensorFromGpuBufferOptions
    {
        /// <summary>
        /// the data type of the tensor. If omitted, assume 'float32'.
        /// </summary>
        public string DataType { get; set; } = "float32";

        /// <summary>
        /// the dimension of the tensor. Required.
        /// </summary>
        public long[] Dims { get; set; } = new long[] { 1 };

        // an optional function to download the tensor data from GPU to CPU. If omitted, the GPU data will not be able to download.Usually, this is provided by a GPU backend for the inference outputs.Users don't need to provide this function.
        // public ActionCallback Download { get; set; } = null!;

        // an optional function to dispose the tensor data on GPU. If omitted, the GPU data will not be disposed. Usually, this is provided by a GPU backend for the inference outputs.Users don't need to provide this function.
        // public ActionCallback Dispose { get; set; } = null!;
    }
}
