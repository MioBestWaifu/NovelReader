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


/*window.resizeHandler = {
    initialize: function (dotnetHelper) {
        window.addEventListener('resize', function () {
            dotnetHelper.invokeMethodAsync('OnResize');
        });
    }
};*/