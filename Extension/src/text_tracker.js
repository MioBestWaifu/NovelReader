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


function captureSelectedText() {
    if (options.translateSelection) {

        var selectedText = window.getSelection().toString();

        if(selectedText.trim() !== ""){
            //This is done because in some readers (i.e Kavita) the selection is the document, not on the window
            selectedText = document.getSelection().toString();
        }
        // Print selected text if it is different from the previous sent, is not empty
        // and the user is not currently selecting text
        if (selectedText.trim() !== "" && selectedText !== previousText && !isMousePressed) {
            console.log("Selected Text : " + selectedText);
            previousText = selectedText;
            chrome.runtime.sendMessage({ type: 'text-selected', text: selectedText });
        }
    }
}
