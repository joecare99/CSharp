import { dotnet } from './_framework/dotnet.js'

const hostElement = document.getElementById('out');
const splashElement = document.getElementById('startup-splash');

function closeSplash() {
    if (!splashElement || splashElement.classList.contains('splash-close')) {
        return;
    }

    splashElement.classList.add('splash-close');
    window.setTimeout(() => splashElement.remove(), 250);
}

function hasApplicationContent() {
    if (!hostElement) {
        return false;
    }

    return Array.from(hostElement.children).some(child => child !== splashElement);
}

function watchForApplicationContent() {
    if (!hostElement || !splashElement) {
        return;
    }

    if (hasApplicationContent()) {
        closeSplash();
        return;
    }

    const observer = new MutationObserver(() => {
        if (!hasApplicationContent()) {
            return;
        }

        observer.disconnect();
        closeSplash();
    });

    observer.observe(hostElement, { childList: true, subtree: false });
}

const isBrowser = typeof window !== 'undefined';
if (!isBrowser) throw new Error('Expected to be running in a browser');

watchForApplicationContent();

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
