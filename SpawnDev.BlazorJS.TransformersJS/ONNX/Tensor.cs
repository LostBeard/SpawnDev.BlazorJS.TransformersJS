using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS.ONNX
{
    /// <summary>
    /// Represent multi-dimensional arrays to feed to or fetch from model inferencing.<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/Tensor-1.html
    /// </summary>
    /// <typeparam name="TData">string[] | Int8Array | Uint8Array | Int16Array | Uint16Array | Int32Array | Uint32Array | Float32Array | Float64Array | BigInt64Array | BigUint64Array</typeparam>
    public class Tensor<TData> : Tensor
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Tensor(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// The tensor data as type TData
        /// </summary>
        public TData Data => JSRef!.Get<TData>("data");
        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <param name="dims"></param>
        /// <returns></returns>
        public override Tensor<TData> Reshape(IEnumerable<int> dims) => JSRef!.Call<Tensor<TData>>("reshape", dims)!;

        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <returns></returns>
        public override Tensor<TData> Clone() => JSRef!.Call<Tensor<TData>>("clone")!;
    }
    /// <summary>
    /// Represent multi-dimensional arrays to feed to or fetch from model inferencing.<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/TensorConstructor.html<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/Tensor-1.html
    /// </summary>
    public class Tensor : RuntimeTensor
    {
        /// <inheritdoc/>
        public Tensor(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Dimensions of the tensor.
        /// </summary>
        public int[] Dims => JSRef!.Get<int[]>("dims");
        /// <summary>
        /// Type of the tensor.<br/>
        /// Example:
        /// - "float32"<br/>
        /// </summary>
        public string Type => JSRef!.Get<string>("type");
        /// <summary>
        /// The location of the tensor data.
        /// </summary>
        public string Location => JSRef!.Get<string>("location");
        /// <summary>
        /// The number of elements in the tensor.
        /// </summary>
        public int Size => JSRef!.Get<int>("size");
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
        /// Performs Tensor dtype conversion.<br/>
        /// If the self Tensor already has the correct dtype, then self is returned.
        /// </summary>
        /// <param name="type">The desired data type.</param>
        /// <returns>The converted tensor.</returns>
        public Tensor To(string type) => JSRef!.Call<Tensor>("to", type);
        /// <summary>
        /// Performs Tensor dtype conversion.<br/>
        /// If the self Tensor already has the correct dtype, then self is returned.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public Tensor<TData> To<TData>(string type) => JSRef!.Call<Tensor<TData>>("to", type);
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
        public virtual Tensor Reshape(IEnumerable<int> dims) => (Tensor)JSRef!.Call(GetType(), "reshape", dims)!;
        /// <summary>
        /// Create a new tensor with the same data buffer and specified dims.
        /// </summary>
        /// <param name="dims">New dimensions. Size should match the old one.</param>
        /// <returns></returns>
        public virtual Tensor<TData> Reshape<TData>(IEnumerable<int> dims) => JSRef!.Call<Tensor<TData>>("reshape", dims)!;


        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <returns>A new Tensor with the same type, data, and dimensions as the original.</returns>
        public virtual Tensor Clone() => (Tensor)JSRef!.Call(GetType(), "clone")!;
        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public virtual Tensor<TData> Clone<TData>() => JSRef!.Call<Tensor<TData>>("clone")!;
    }
}
