// Todd Tanner
// 2024
// This script detects simd support and loads an apropriate build of Blazor WASM
"use strict";
(async () => {
    // Blazor WASM will fail to load if BigInt64Array or BigUint64Array is not found, but it does not use them on startup
    // This fix was added t oenable support for Safari 14 and 15 which do not support BigInt64Array
    if (!globalThis.BigInt64Array) globalThis.BigInt64Array = function () { };
    if (!globalThis.BigUint64Array) globalThis.BigUint64Array = function () { };
    //
    var url = new URL(location.href);
    let verboseStart = url.searchParams.get('verboseStart') === '1';
    var forceCompatMode = url.searchParams.get('forceCompatMode') === '1';
    var supportsSimd = await wasmFeatureDetect.simd();
    if (verboseStart) console.log('supportsSimd', supportsSimd);
    // compat mode build could be built without wasm exception support if needed and detected here
    var supportsExceptions = await wasmFeatureDetect.exceptions();
    if (verboseStart) console.log('supportsExceptions', supportsExceptions);
    var compatModeAvailable = true; // published !== false;    // requires service-worker.js which has 2 versions; 1 for development and one for when published. used to set published to true in the published version and false in the development version.
    var useCompatMode = !supportsSimd && compatModeAvailable;
    if (forceCompatMode && compatModeAvailable) {
        if (verboseStart) console.log('forceCompatMode', forceCompatMode);
        useCompatMode = true;
    }
    if (verboseStart) console.log('useCompatMode', useCompatMode);
    // Blazor United (.Net 8 Blazor Web App) Blazor.start settings are slightly different than Blazor WebAssembly (Blazor WebAssembly Standalone App)
    var getRuntimeType = function () {
        for (var script of document.scripts) {
            if (script.src.indexOf('_framework/blazor.web.js') !== -1) return 'united';
            if (script.src.indexOf('_framework/blazor.webassembly.js') !== -1) return 'wasm';
        }
        return '';
    }
    var runtimeType = getRuntimeType();
    // customize the resource loader for the runtime that is loaded
    // https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/startup?view=aspnetcore-8.0#load-boot-resources
    var webAssemblyConfig = {
        loadBootResource: function (type, name, defaultUri, integrity) {
            if (verboseStart) console.log(`Loading: '${type}', '${name}', '${defaultUri}', '${integrity}'`);
            if (useCompatMode) {
                let newUrl = defaultUri.replace('_framework/', '_frameworkCompat/');
                return newUrl;
            }
        },
    };
    if (runtimeType === 'wasm') {
        // Blazor WebAssembly Standalone App
        Blazor.start(webAssemblyConfig);
    } else if (runtimeType === 'united') {
        // Blazor Web App (formally Blazor United)
        Blazor.start({ webAssembly: webAssemblyConfig });
    } else {
        // Fallback supports both known Blazor WASM runtimes
        // Modified loader that will work with both United and WASM runtimes (doesn't require detection)
        webAssemblyConfig.webAssembly = webAssemblyConfig;
        Blazor.start(webAssemblyConfig);
    }
})();
    