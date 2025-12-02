using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// 
    /// </summary>
    public class Tensor : JSObject
    {
        /// <inheritdoc/>
        public Tensor(IJSInProcessObjectReference _ref) : base(_ref) { }

        public Tensor(Float32Array data, int[] shape) : base(JS.New("Tensor", data, shape)) { }

        public Tensor(float[] data, int[] shape) : base(JS.New("Tensor", Float32Array.From(data), shape)) { }

        /// <summary>
        /// A property that returns an array of numbers representing the dimensions
        /// </summary>
        public int[] Shape => JSRef!.Get<int[]>("shape");
        /// <summary>
        /// A method that returns the actual data in a typed array format
        /// </summary>
        /// <returns></returns>
        public Float32Array Data() => JSRef!.Call<Float32Array>("data");
        /// <summary>
        /// Method: Transfers the tensor to WebGPU memory, returning a Promise&lt;Tensor>
        /// </summary>
        public Task<Tensor> To(string accelerator) => JSRef!.CallAsync<Tensor>("to", accelerator);
    }
}
