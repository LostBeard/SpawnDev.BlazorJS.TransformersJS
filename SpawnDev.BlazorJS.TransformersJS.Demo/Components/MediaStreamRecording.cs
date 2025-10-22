using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Components
{
    public class MediaStreamRecording : IDisposable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Blob? Data { get; set; } = null;
        public string URL { get; set; } = "";
        /// <summary>
        /// Message duration in milliseconds
        /// </summary>
        public double Duration { get; set; }
        public MediaStreamRecording(Blob[] blobs, string mimeType)
        {
            Data = new Blob(blobs, new BlobOptions { Type = mimeType });
            URL = SpawnDev.BlazorJS.JSObjects.URL.CreateObjectURL(Data);
        }
        public MediaStreamRecording(Blob blob)
        {
            Data = blob;
            URL = SpawnDev.BlazorJS.JSObjects.URL.CreateObjectURL(Data);
        }
        public void Dispose()
        {
            if (!string.IsNullOrEmpty(URL))
            {
                SpawnDev.BlazorJS.JSObjects.URL.RevokeObjectURL(URL);
                URL = "";
            }
            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
        }
    }
}
