using SpawnDev.BlazorJS.JSObjects;
using System.Reflection;

namespace SpawnDev.BlazorJS.TransformersJS.Demo
{
    class WebGLUtilities
    {
        public static void SetRectangle(WebGLRenderingContext gl, float x, float y, float width, float height)
        {
            var x1 = x;
            var x2 = x + width;
            var y1 = y;
            var y2 = y + height;
            using var bufferData = new Float32Array([x1, y1, x2, y1, x1, y2, x1, y2, x2, y1, x2, y2]);
            gl.BufferData(GL.ARRAY_BUFFER, bufferData, GL.STATIC_DRAW);
        }
        public static WebGLProgram? CreateProgramFromScripts(WebGLRenderingContext gl, string[] shaderScriptIds, string[]? optAttribs = null, int?[]? optLocations = null)
        {
            var shaders = new List<WebGLShader>();
            for (var i = 0; i < shaderScriptIds.Length; i++)
            {
                var shaderName = shaderScriptIds[i];
                var shaderType = shaderName.Contains("vertex") ? GL.VERTEX_SHADER : GL.FRAGMENT_SHADER;
                var shader = CreateShaderFromScript(gl, shaderName, shaderType);
                if (shader == null)
                {
                    throw new Exception($"Failed to compile shader: {shaderName}");
                }
                shaders.Add(shader);
            }
            return CreateProgram(gl, shaders, optAttribs, optLocations);
        }
        static WebGLProgram? CreateProgram(WebGLRenderingContext gl, List<WebGLShader> shaders, string[]? optAttribs, int?[]? optLocations)
        {
            var program = gl.CreateProgram();
            foreach (var shader in shaders)
            {
                gl.AttachShader(program, shader);
            }
            if (optAttribs != null)
            {
                for (var i = 0; i < optAttribs.Length; i++)
                {
                    gl.BindAttribLocation(program, (uint)(optLocations?[i] ?? i), optAttribs[i]);
                }
            }
            gl.LinkProgram(program);
            var linked = gl.GetProgramParameter<bool>(program, GL.LINK_STATUS);
            if (!linked)
            {
                var lastError = gl.GetProgramInfoLog(program);
                Console.WriteLine("Error in program linking:" + lastError);
                gl.DeleteProgram(program);
                return null;
            }
            return program;
        }
        public static string? ReadEmbeddedResource(string resourceName, Assembly? assembly = null)
        {
            try
            {
                assembly ??= Assembly.GetCallingAssembly();
                using var stream = assembly.GetManifestResourceStream(resourceName)!;
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
        static WebGLShader? CreateShaderFromScript(WebGLRenderingContext gl, string scriptId, uint shaderType)
        {
            var shaderSource = ReadEmbeddedResource($"SpawnDev.BlazorJS.TransformersJS.Demo.Shaders.{scriptId}.glsl");
            if (shaderSource == null)
            {
                return null;
            }
            var shader = gl.CreateShader(shaderType);
            gl.ShaderSource(shader, shaderSource);
            gl.CompileShader(shader);
            var compiled = gl.GetShaderParameter<bool>(shader, GL.COMPILE_STATUS);
            if (!compiled)
            {
                var lastError = gl.GetShaderInfoLog(shader);
                Console.WriteLine("Error compiling shader '" + shader + "':" + lastError);
                gl.DeleteShader(shader);
                return null;
            }
            return shader;
        }
        public static void ResizeCanvasToDisplaySize(HTMLCanvasElement canvas)
        {
            var width = canvas.ClientWidth;
            var height = canvas.ClientHeight;
            if (canvas.Width != width || canvas.Height != height)
            {
                canvas.Width = (int)width;
                canvas.Height = (int)height;
            }
        }
    }
}
