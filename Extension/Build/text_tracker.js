// Add an event listener for text selection events in the document.
console.log("text_tracker.js loaded");

setInterval(printSelectedText, 500);

let previousText = "";

let isMousePressed = false;

window.addEventListener('mousedown', function() {
    isMousePressed = true;
});

window.addEventListener('mouseup', function() {
    isMousePressed = false;
});

function printSelectedText() {
    // Check for text selection
    var selectedText = window.getSelection().toString();

    // Print selected text if it is different from the previous sent, is not empty
    // and the user is not currently selecting text
    if (selectedText.trim() !== "" && selectedText !== previousText && !isMousePressed) {
        console.log("Selected Text : " + selectedText);
        previousText = selectedText;
        chrome.runtime.sendMessage({ type: 'text-selected', text: selectedText });
    }
}
  