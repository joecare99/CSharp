import { dotnet } from './_framework/dotnet.js'

const titleElement = document.getElementById('startup-title');
const statusElement = document.getElementById('startup-status');
const detailsElement = document.getElementById('startup-details');

function setStatus(message) {
    if (statusElement) {
        statusElement.textContent = message;
    }
}

function showError(error) {
    if (titleElement) {
        titleElement.textContent = 'Avln_Bubbles Browser startup failed';
    }

    const message = error instanceof Error
        ? `${error.message}\n\n${error.stack ?? ''}`
        : String(error);

    setStatus('The JavaScript/.NET bootstrap reported an error.');

    if (detailsElement) {
        detailsElement.hidden = false;
        detailsElement.textContent = message;
    }

    console.error(error);
}

const isBrowser = typeof window !== 'undefined';
if (!isBrowser) throw new Error('Expected to be running in a browser');

setStatus('Browser environment detected. Loading .NET runtime...');

try {
    const dotnetRuntime = await dotnet
        .withDiagnosticTracing(true)
        .withApplicationArgumentsFromQuery()
        .create();

    setStatus('Runtime loaded. Resolving application entry point...');

    const config = dotnetRuntime.getConfig();
    setStatus(`Starting ${config.mainAssemblyName}...`);

    await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
}
catch (error) {
    showError(error);
}
