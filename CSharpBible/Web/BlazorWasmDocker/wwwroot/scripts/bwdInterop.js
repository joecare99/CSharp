var bwdInterop = {};

bwdInterop.setLocalStorage = function (key, data) {
    localStorage.setItem(key, data);
}

bwdInterop.getLocalStorage = function (key) {
    return localStorage.getItem(key);
}

bwdInterop.removeLocalStorage = function (key) {
    localStorage.removeItem(key);
}
