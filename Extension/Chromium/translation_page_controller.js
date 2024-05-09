let vue = new Vue({
    el: '#app',
    data: {
        translations: [] // Your dynamic data
    }
});

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    // Check if the message is about a text selection.
    if (message && message.type === 'translation') {
        console.log("Received translation: " + message.text);
        let translationObject = JSON.parse(message.text);
        vue.translations.push(translationObject);
    }
});