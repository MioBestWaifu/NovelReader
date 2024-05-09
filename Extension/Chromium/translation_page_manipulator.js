chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    // Check if the message is about a text selection.
    if (message && message.type === 'pinto') {
        console.log("Rola: " + message.text);
    }
});