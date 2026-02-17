using Microsoft.JSInterop;
using SpawnDev.BlazorJS.TransformersJS.ONNX;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Represents a model that provides an interface to a JavaScript object and enables asynchronous execution of
    /// JavaScript methods.
    /// </summary>
    /// <remarks>Inherits from JSObject and is typically used to invoke JavaScript functions from .NET code.
    /// The Run method executes a JavaScript function named 'run' on the referenced object, passing the specified
    /// argument. This class is intended for scenarios where interaction with JavaScript models is required in Blazor or
    /// similar environments.</remarks>
    public class Model : JSObject
    {
        /// <inheritdoc/>
        public Model(IJSInProcessObjectReference _ref) : base(_ref) { }

        /// <summary>
        /// Run the model with feeds (input tensors).
        /// </summary>
        public Task<JSObject> Run(object feeds) => JSRef!.CallAsync<JSObject>("run", feeds);

        /// <summary>
        /// Run the model with feeds and run options.
        /// Use options = new { preferredOutputLocation = "gpu-buffer" } to keep output on GPU.
        /// </summary>
        public Task<JSObject> Run(object feeds, object options) => JSRef!.CallAsync<JSObject>("run", feeds, options);

        /// <summary>
        /// Run the model with strongly-typed feeds and optional options.
        /// </summary>
        public Task<Dictionary<string, RuntimeTensor>> Run(Dictionary<string, RuntimeTensor> feeds, object? options = null)
            => options == null
                ? JSRef!.CallAsync<Dictionary<string, RuntimeTensor>>("run", feeds)
                : JSRef!.CallAsync<Dictionary<string, RuntimeTensor>>("run", feeds, options);

        /// <summary>
        /// Gets the underlying ONNX InferenceSession.
        /// In Transformers.js, this is model.session (the ORT session object).
        /// </summary>
        public JSObject Session => JSRef!.Get<JSObject>("session");

        /// <summary>
        /// Gets the model's input names (from the ONNX session).
        /// </summary>
        public string[] InputNames => JSRef!.Get<string[]>("session.inputNames");

        /// <summary>
        /// Gets the model's output names (from the ONNX session).
        /// </summary>
        public string[] OutputNames => JSRef!.Get<string[]>("session.outputNames");
    }

    /// <summary>
    /// The Pipeline class is the class from which all pipelines inherit. Refer to this class for methods shared across different pipelines.<br/>
    /// https://huggingface.co/docs/transformers.js/api/pipelines#module_pipelines.Pipeline<br/>
    /// https://huggingface.co/docs/transformers.js/api/pipelines
    /// </summary>
    public class Pipeline : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Pipeline(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Calls JS object's dispose() method
        /// </summary>
        public void DisposeJS() => JSRef!.CallVoid("dispose");
        /// <summary>
        /// Gets the pipeline model session
        /// </summary>
        public Model Model => JS!.Get< Model>("model");
        /// <summary>
        /// Runs the pipeline _call method asynchronously
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<T> _Call<T>(params object[] args) => JSRef!.CallAsync<T>("_call.apply", JSRef, args);
        /// <summary>
        /// Runs the pipeline _call method synchronously
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public T _CallSync<T>(params object[] args) => JSRef!.Call<T>("_call.apply", JSRef, args);
        /// <summary>
        /// Checks if the returned JSObject is already an array. If it is, it is returned, otherwise an array is created with the single item.<br/>
        /// If the value is null, an empty array is returned.<br/>
        /// This method is useful for handling pipeline calls that may return either a single object or an array of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOrArray"></param>
        /// <returns></returns>
        protected T[] MakeArray<T>(JSObject valueOrArray)
        {
            if (valueOrArray == null) return new T[0];
            return JSObjects.Array.IsArray(valueOrArray) ? valueOrArray.JSRefAs<T[]>() : new T[] { valueOrArray.JSRefAs<T>() };
        }
    }
}
