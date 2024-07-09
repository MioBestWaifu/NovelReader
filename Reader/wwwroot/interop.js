//Could use this same solution to prevent the selection browser behavior without removing selection entirely?
document.addEventListener('keydown', function (e) {
    if (e.ctrlKey && e.shiftKey && e.key.toLowerCase() === 's') {
        e.preventDefault();
    }
});

window.getWindowWidth = function () {
    return window.innerWidth;
};

window.getWindowHeight = function () {
    return window.innerHeight;
};

window.setFocusToElement = function(id) {
    document.getElementById(id).focus();
}

window.setElementBackgroundColor = function (id, color) {
    var element = document.getElementById(id);
    if (element) {
        element.style.backgroundColor = color;
    }
}

window.removeElementBackgroundColor = function (id) {
    var element = document.getElementById(id);
    if (element) {
        element.style.backgroundColor = '';
    }
}

window.changeDirectionToRTL = function () {
    document.documentElement.setAttribute('dir', 'rtl');
}

window.changeDirectionToLTR = function () {
    document.documentElement.setAttribute('dir', 'ltr');
}

window.scrollToHorizontalStart = function () {
    document.body.scrollTo(0, document.body.scrollTop);
}

window.scrollToVerticalStart = function () {
    document.body.scrollTo(document.body.scrollLeft, 0);
}

window.focusOnElement = function (id) {
    document.getElementById(id).focus();
}

let epubViewerReference = null;

window.setEpubViewerReference = function (instance) {
    epubViewerReference = instance;
}

window.onresize = function () {
    epubViewerReference.invokeMethodAsync("HandleWindowResize");
};
/*window.resizeHandler = {
    initialize: function (dotnetHelper) {
        window.addEventListener('resize', function () {
            dotnetHelper.invokeMethodAsync('OnResize');
        });
    }
};*/