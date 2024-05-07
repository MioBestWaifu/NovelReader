// Add an event listener for text selection events in the document.
console.log("text_tracker.js loaded");

setInterval(printSelectedText, 500);

function printSelectedText() {
    // Check for text selection
    var selectedText = window.getSelection().toString();

    // Print selected text if any
    if (selectedText.trim() !== "") {
        console.log("Selected Text : " + selectedText);
        chrome.runtime.sendMessage({ type: 'text-selected', text: selectedText });
    }
}
  