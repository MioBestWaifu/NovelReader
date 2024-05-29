window.getWindowWidth = function () {
    return window.innerWidth;
};

window.getWindowHeight = function () {
    return window.innerHeight;
};

window.setFocusToElement = function(id) {
    document.getElementById(id).focus();
}

window.setElementBackgroundColor = function (x, y, color) {
    document.elementFromPoint(x, y).style.backgroundColor = color;
}

window.removeElementBackgroundColor = function (x, y) {
    document.elementFromPoint(x, y).style.backgroundColor = '';
}