using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.TransformersJS.ONNX;
using System.Collections;
using Array = SpawnDev.BlazorJS.JSObjects.Array;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Transformers.js Tensor<br/>
    /// Wraps the ONNXRuntime Tensor (ONNXTensor)<br/>
    /// https://github.com/huggingface/transformers.js/blob/main/packages/transformers/src/utils/tensor.js
    /// </summary>
    /// <typeparam name="TData">Array&lt;string> | Int8Array | Uint8Array | Int16Array | Uint16Array | Int32Array | Uint32Array | Float16Array | Float32Array | Float64Array | BigInt64Array | BigUint64Array</typeparam>
    public class Tensor<TData> : Tensor
    {
        /// <inheritdoc/>
        public Tensor(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Creates a new Tensor
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        /// <param name="shape"></param>
        public Tensor(string dataType, TData data, long[] shape) : base(JS.New("Transformers.Tensor", dataType, data, shape)) { }
        /// <summary>
        /// New instance from an ONNXTensor
        /// </summary>
        /// <param name="tensor"></param>
        public Tensor(ONNXTensor<TData> tensor) : base(JS.New("Transformers.Tensor", tensor)) { }
        /// <summary>
        /// Returns the ORT Tensor if this tensor proxy wraps one
        /// </summary>
        public new ONNXTensor<TData>? OrtTensor => JSRef!.Get<ONNXTensor<TData>?>("ort_tensor");
        /// <summary>
        /// A method that returns the actual data in a typed array format
        /// </summary>
        /// <returns></returns>
        public new TData Data => JSRef!.Get<TData>("data");
        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <returns></returns>
        public new Tensor<TData> Clone() => JSRef!.Call<Tensor<TData>>("clone")!;
        /// <summary>
        /// Returns the value at the specified index.
        /// </summary>
        /// <param name="index">The index to access</param>
        /// <returns></returns>
        public T At<T>(params long[] index) => JSRef!.Call<T>("at", index)!;
        /// <summary>
        /// Transfers the tensor to WebGPU memory or CPU, returning a Promise&lt;Tensor&lt;TData>>
        /// </summary>
        public new Task<Tensor<TData>> To(string accelerator) => JSRef!.CallAsync<Tensor<TData>>("to", accelerator);
    }
    /// <summary>
    /// Transformers.js Tensor<br/>
    /// Wraps the ONNXRuntime Tensor (ONNXTensor)<br/>
    /// https://github.com/huggingface/transformers.js/blob/main/src/utils/tensor.js
    /// </summary>
    public class Tensor : JSObject
    {
        static Lazy<Dictionary<string, Type>> _DataTypeMap = new Lazy<Dictionary<string, Type>>(() =>
        {
            if (!JS.IsBrowser) return new Dictionary<string, Type>();
            return new Dictionary<string, Type>
            {
                { "float32", typeof(Float32Array) },
                // @ts-ignore ts(2552) Limited availability of Float16Array across browsers:
                // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Float16Array
                { "float16", !JS.IsUndefined(nameof(Float16Array)) ? typeof(Float16Array): typeof(Uint16Array)},
                { "float64", typeof(Float64Array)},
                { "string", typeof(Array<string>)}, // string[]
                { "int8", typeof(Int8Array) },
                { "uint8", typeof(Uint8Array) },
                { "int16", typeof(Int16Array) },
                { "uint16", typeof(Uint16Array) },
                { "int32", typeof(Int32Array) },
                { "uint32", typeof(Uint32Array) },
                { "int64", typeof(BigInt64Array) },
                { "uint64", typeof(BigUint64Array) },
                { "bool", typeof(Uint8Array) },
                { "uint4", typeof(Uint8Array) },
                { "int4", typeof(Int8Array) },
            };
        });
        /// <summary>
        /// DataTypeMap<br/>
        /// https://github.com/huggingface/transformers.js/blob/main/src/utils/tensor.js
        /// </summary>
        public static Dictionary<string, Type> DataTypeMap => _DataTypeMap.Value;
        /// <inheritdoc/>
        public Tensor(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Creates a new Tensor from data type, data, and shape
        /// </summary>
        /// <param name="dataType">The data type (e.g., "float32", "int32", etc.)</param>
        /// <param name="data">The tensor data</param>
        /// <param name="shape">The tensor shape/dimensions</param>
        public Tensor(string dataType, TypedArray data, long[] shape) : base(JS.New("Transformers.Tensor", dataType, data, shape)) { }
        /// <summary>
        /// Creates a new Tensor from an ONNXTensor
        /// </summary>
        /// <param name="tensor">The ONNX tensor to wrap</param>
        public Tensor(ONNXTensor tensor) : base(JS.New("Transformers.Tensor", tensor)) { }
        /// <summary>
        /// Returns the ORT Tensor if this tensor proxy wraps one
        /// </summary>
        public ONNXTensor? OrtTensor => JSRef!.Get<ONNXTensor?>("ort_tensor");
        /// <summary>
        /// Returns the ORT Tensor if this tensor proxy wraps one
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public ONNXTensor<TData>? Get_OrtTensor<TData>() => JSRef!.Get<ONNXTensor<TData>?>("ort_tensor");
        /// <summary>
        /// Data as JSObject
        /// </summary>
        /// <returns></returns>
        public JSObject? Data => JSRef!.Get<JSObject?>("data");
        /// <summary>
        /// Data as TData
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData? Get_Data<TData>() => JSRef!.Get<TData?>("data");
        /// <summary>
        /// A property that returns an array of numbers representing the dimensions
        /// </summary>
        public long[] Dims => JSRef!.Get<long[]>("dims");
        /// <summary>
        /// The data element type<vr/>
        /// See DataTypeMap
        /// </summary>
        public string Type => JSRef!.Get<string>("type");
        /// <summary>
        /// The data location
        /// </summary>
        public string Location => JSRef!.Get<string>("location");
        /// <summary>
        /// The number of elements in the tensor.
        /// </summary>
        public long Size => JSRef!.Get<long>("size");
        /// <summary>
        /// Transfers the tensor to WebGPU memory or CPU, returning a Promise&lt;Tensor>
        /// </summary>
        public Task<Tensor> To(string accelerator) => JSRef!.CallAsync<Tensor>("to", accelerator);
        /// <summary>
        /// Transfers the tensor to WebGPU memory or CPU, returning a Promise&lt;Tensor>
        /// </summary>
        public Task<Tensor<TData>> To<TData>(string accelerator) => JSRef!.CallAsync<Tensor<TData>>("to", accelerator);
        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <returns></returns>
        public Tensor<TData> Clone<TData>() => JSRef!.Call<Tensor<TData>>("clone")!;
        /// <summary>
        /// Creates a deep copy of the current Tensor.
        /// </summary>
        /// <returns></returns>
        public Tensor Clone() => JSRef!.Call<Tensor>("clone")!;
        /// <summary>
        /// Returns a new Tensor with the same data as the original but with a new shape.
        /// </summary>
        /// <param name="dims">The new shape dimensions</param>
        /// <returns></returns>
        public Tensor Reshape(params long[] dims) => JSRef!.Call<Tensor>("reshape", dims)!;
        /// <summary>
        /// Returns a new Tensor with the same data as the original but with a new shape.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="dims">The new shape dimensions</param>
        /// <returns></returns>
        public Tensor<TData> Reshape<TData>(params long[] dims) => JSRef!.Call<Tensor<TData>>("reshape", dims)!;
        /// <summary>
        /// Returns a new tensor with dimensions of size 1 removed.
        /// </summary>
        /// <param name="dim">If given, the input will be squeezed only in this dimension</param>
        /// <returns></returns>
        public Tensor Squeeze(long? dim = null) => dim.HasValue ? JSRef!.Call<Tensor>("squeeze", dim.Value)! : JSRef!.Call<Tensor>("squeeze")!;
        /// <summary>
        /// Returns a new tensor with dimensions of size 1 removed.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="dim">If given, the input will be squeezed only in this dimension</param>
        /// <returns></returns>
        public Tensor<TData> Squeeze<TData>(long? dim = null) => dim.HasValue ? JSRef!.Call<Tensor<TData>>("squeeze", dim.Value)! : JSRef!.Call<Tensor<TData>>("squeeze")!;
        /// <summary>
        /// Returns a new tensor with a dimension of size 1 inserted at the specified position.
        /// </summary>
        /// <param name="dim">The index at which to insert the singleton dimension</param>
        /// <returns></returns>
        public Tensor Unsqueeze(long dim) => JSRef!.Call<Tensor>("unsqueeze", dim)!;
        /// <summary>
        /// Returns a new tensor with a dimension of size 1 inserted at the specified position.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="dim">The index at which to insert the singleton dimension</param>
        /// <returns></returns>
        public Tensor<TData> Unsqueeze<TData>(long dim) => JSRef!.Call<Tensor<TData>>("unsqueeze", dim)!;
        /// <summary>
        /// Returns a portion of the tensor given the start and end indices.
        /// </summary>
        /// <param name="slices">Array of slice ranges for each dimension</param>
        /// <returns></returns>
        public Tensor Slice(params long?[][] slices) => JSRef!.Call<Tensor>("slice", slices)!;
        /// <summary>
        /// Returns a portion of the tensor given the start and end indices.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="slices">Array of slice ranges for each dimension</param>
        /// <returns></returns>
        public Tensor<TData> Slice<TData>(params long?[][] slices) => JSRef!.Call<Tensor<TData>>("slice", slices)!;
        /// <summary>
        /// Permutes the dimensions of the tensor.
        /// </summary>
        /// <param name="dims">The desired ordering of dimensions</param>
        /// <returns></returns>
        public Tensor Permute(params long[] dims) => JSRef!.Call<Tensor>("permute", dims)!;
        /// <summary>
        /// Permutes the dimensions of the tensor.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="dims">The desired ordering of dimensions</param>
        /// <returns></returns>
        public Tensor<TData> Permute<TData>(params long[] dims) => JSRef!.Call<Tensor<TData>>("permute", dims)!;
        /// <summary>
        /// Returns a new tensor with the mean value over the specified dimension(s).
        /// </summary>
        /// <param name="dim">The dimension to reduce. If null, reduces all dimensions</param>
        /// <param name="keepdim">Whether the output tensor has dim retained or not</param>
        /// <returns></returns>
        public Tensor Mean(long? dim = null, bool keepdim = false) => dim.HasValue ? JSRef!.Call<Tensor>("mean", dim.Value, keepdim)! : JSRef!.Call<Tensor>("mean", keepdim)!;
        /// <summary>
        /// Returns the value at the specified index.
        /// </summary>
        /// <param name="index">The index to access</param>
        /// <returns></returns>
        public T At<T>(params long[] index) => JSRef!.Call<T>("at", index)!;
        /// <summary>
        /// Calls dispose on the Javascript object and optionally (default) disposes the JSRef also<br/>
        /// </summary>
        /// <param name="disposeJSRef"></param>
        public void DisposeJS(bool disposeJSRef = true)
        {
            JSRef!.CallVoid("dispose");
            if (disposeJSRef) Dispose();
        }
    }
}
