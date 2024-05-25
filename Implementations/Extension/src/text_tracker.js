// Add an event listener for text selection events in the document.
console.log("text_tracker.js loaded");

let options = null;

chrome.runtime.sendMessage({ type: 'getOptions' }, function (response) {
    console.log('getting options');
    options = response;
    console.log('received options', response);

});

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    // Check if the message is about a text selection.
    if (message) {
        switch (message.type) {
            case 'options':
                options = JSON.parse(message.text);
                console.log('text tracker received new options');
                console.log(options);
                break;
        }
    }
});

setInterval(captureSelectedText, 500);

let previousText = "";

let isMousePressed = false;

window.addEventListener('mousedown', function () {
    isMousePressed = true;
});

window.addEventListener('mouseup', function () {
    isMousePressed = false;
});

//There could be other shortcuts like the below. Could even be the main way to control Extension if we want to separate the bulky UI component
let isCtrlPressed = false;
let isRPressed = false;

window.addEventListener('keydown', function (event) {
    if (event.key === 'Control') {
        isCtrlPressed = true;
    } else if (event.key === 'Shift') {
        isRPressed = true;
    } else if (event.key === 'e' || event.key === 'E') {
        if (isCtrlPressed && isRPressed) {
            removeKavitaSelectionMenu();
        }
    }
});

window.addEventListener('keyup', function (event) {
    if (event.key === 'Control') {
        isCtrlPressed = false;
    } else if (event.key === 'Shift') {
        isRPressed = false;
    }
});

//This exists because Kavita has a very annoying selection menu that pops up when you select text
//invoked by a mouse up event. This is 
function removeKavitaSelectionMenu() {
    console.log("Trying to remove Kavita Selection Menu");
    var bookContainerElement = document.querySelector('.book-container');
    if (bookContainerElement) {
        //Could theortically lead to a bunch of problems, because Kavita is built with Angular,
        //but i think this keeps the reference to the node intact? Because the manipulation menu (to set color, direction, etc)
        //still works. But the event listener is lost. Some javascript oddity I don't understand. Or maybe I didn't unsderstand the Kavita
        //code. Who knows.
        bookContainerElement.replaceWith(bookContainerElement.cloneNode(true));
    }
}


function captureSelectedText() {
    if (options.translateSelection) {

        var selectedText = window.getSelection().toString();

        if (selectedText.trim() === "") {
            //This is done because in some readers (i.e Kavita) the selection is in the document, not in the window
            selectedText = document.getSelection().toString();
        }
        // Capture selected text if it is different from the previous sent, is not empty
        // and the user is not currently selecting text
        if (selectedText.trim() !== "" && selectedText !== previousText && !isMousePressed) {
            console.log("Selected Text : " + selectedText);
            previousText = selectedText;
            chrome.runtime.sendMessage({ type: 'text-selected', text: selectedText });
        }
    }
}
