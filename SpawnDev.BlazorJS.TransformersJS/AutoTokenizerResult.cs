using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoTokenizerResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoTokenizerResult(IJSInProcessObjectReference _ref) : base(_ref) { }
        public Tensor<BigInt64Array> InputIds => JSRef!.Get<Tensor<BigInt64Array>>("input_ids");
        public Tensor<BigInt64Array> AttentionMask => JSRef!.Get<Tensor<BigInt64Array>>("attention_mask");
    }
}
