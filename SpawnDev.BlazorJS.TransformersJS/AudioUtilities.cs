using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Audio utilities<br/>
    /// Adapted from https://www.npmjs.com/package/audiobuffer-to-wav
    /// </summary>
    public static class AudioUtilities
    {
        public static ArrayBuffer EncodeWAV(Float32Array samples)
        {
            var offset = 44;
            var buffer = new ArrayBuffer(offset + samples.Length * 4);
            using var view = new DataView(buffer);
            uint sampleRate = 16000;
            /* RIFF identifier */
            WriteString(view, 0, "RIFF");
            /* RIFF chunk length */
            view.SetUint32(4, (uint)(36 + samples.Length * 4), true);
            /* RIFF type */
            WriteString(view, 8, "WAVE");
            /* format chunk identifier */
            WriteString(view, 12, "fmt ");
            /* format chunk length */
            view.SetUint32(16, 16, true);
            /* sample format (raw) */
            view.SetUint16(20, 3, true);
            /* channel count */
            view.SetUint16(22, 1, true);
            /* sample rate */
            view.SetUint32(24, sampleRate, true);
            /* byte rate (sample rate * block align) */
            view.SetUint32(28, sampleRate * 4, true);
            /* block align (channel count * bytes per sample) */
            view.SetUint16(32, 4, true);
            /* bits per sample */
            view.SetUint16(34, 32, true);
            /* data chunk identifier */
            WriteString(view, 36, "data");
            /* data chunk length */
            view.SetUint32(40, (uint)samples.Length * 4, true);
            for (var i = 0; i < samples.Length; ++i, offset += 4)
            {
                view.SetFloat32(offset, samples[i], true);
            }
            return buffer;
        }
        public static byte[] EncodeWAV(float[] samples)
        {
            int offset = 44;
            int bufferLength = offset + samples.Length * 4;
            byte[] buffer = new byte[bufferLength];
            int sampleRate = 16000;
            // Create a memory stream to write the WAV data
            using (var memoryStream = new System.IO.MemoryStream(buffer))
            using (var writer = new System.IO.BinaryWriter(memoryStream))
            {
                /* RIFF identifier */
                WriteString(writer, "RIFF");
                /* RIFF chunk length */
                writer.Write(36 + samples.Length * 4);
                /* RIFF type */
                WriteString(writer, "WAVE");
                /* format chunk identifier */
                WriteString(writer, "fmt ");
                /* format chunk length */
                writer.Write(16);
                /* sample format (raw) */
                writer.Write((ushort)3);
                /* channel count */
                writer.Write((ushort)1);
                /* sample rate */
                writer.Write(sampleRate);
                /* byte rate (sample rate * block align) */
                writer.Write(sampleRate * 4);
                /* block align (channel count * bytes per sample) */
                writer.Write((ushort)4);
                /* bits per sample */
                writer.Write((ushort)32);
                /* data chunk identifier */
                WriteString(writer, "data");
                /* data chunk length */
                writer.Write(samples.Length * 4);
                foreach (var sample in samples)
                {
                    writer.Write(sample);
                }
            }
            return buffer;
        }

        private static void WriteString(System.IO.BinaryWriter writer, string value)
        {
            foreach (char c in value)
            {
                writer.Write((byte)c);
            }
        }
        private static void WriteString(DataView writer, int offset, string value)
        {
            for (var i = 0; i < value.Length; i++)
            {
                {
                    writer.SetUint8(offset + i, (byte)value[i]);
                }
            }
        }
    }
}
