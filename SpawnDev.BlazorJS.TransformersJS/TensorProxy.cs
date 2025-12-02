using Microsoft.JSInterop;
using SpawnDev.BlazorJS.TransformersJS.ONNX;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Transformers.js Tensor proxy wraps the underlying runtime Tensor, such as an ONNX Runtime Tensor (ORT Tensor)
    /// </summary>
    public class TensorProxy : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TensorProxy(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Returns the ORT Tensor if this tensor proxy wraps one
        /// </summary>
        public Tensor? OrtTensor => JSRef!.Get<Tensor?>("ort_tensor");
        /// <summary>
        /// Returns the ORT Tensor if this tensor proxy wraps one
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public Tensor<TData>? Get_OrtTensor<TData>() => JSRef!.Get<Tensor<TData>?>("ort_tensor");
    }
    /// <summary>
    /// Transformers.js Tensor proxy wraps the underlying runtime Tensor, such as an ONNX Runtime Tensor (ORT Tensor)
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class TensorProxy<TData> : TensorProxy
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TensorProxy(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Returns the ORT Tensor if this tensor proxy wraps one
        /// </summary>
        public new Tensor<TData>? OrtTensor => JSRef!.Get<Tensor<TData>?>("ort_tensor");
    }
}
