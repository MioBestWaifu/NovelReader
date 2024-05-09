// Listen for the extension being installed, updated, or enabled
/* chrome.runtime.onInstalled.addListener(function () { */
// Listen for tab updates
chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
    // Check if the tab has finished loading and is active
    if (changeInfo.status === 'complete' && tab.active) {
        // Send message to the other application whenever a new tab is loaded
        sendTabInfoToHost(tab);
    }
});

// Listen for tab switching
chrome.tabs.onActivated.addListener(activeInfo => {
    // Get information about the newly activated tab
    chrome.tabs.get(activeInfo.tabId, tab => {
        // Send message to the other application whenever a tab is switched
        sendTabInfoToHost(tab);
    });
});

// Listen for tab URL changes (user navigation)
chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
    // Check if the URL has changed and the tab is active
    if (changeInfo.url && tab.active) {
        // Send message to the other application whenever the URL changes
        sendTabInfoToHost(tab);
    }
});

// Add a listener for messages from content scripts.
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    // Check if the message is about a text selection.
    if (message && message.type === 'text-selected') {
        chrome.runtime.sendMessage({ type: 'pinto', text: message.text });
        // Log or process the selected text.
        //Check if text is different from the previous when actually implementing this
        sendTranslationRequest(message.text);
    }
});

chrome.action.onClicked.addListener((tab) => {
    chrome.tabs.create({ url: chrome.runtime.getURL('translation_history.html') });
  });

/* });
 */
// Function to send tab information to the other application
function sendTabInfoToHost(tab) {
    // Extract title and URL from the tab
    let title = tab.title;
    let url = decodeURIComponent(tab.url);

    if (!url.startsWith("http://") && !url.startsWith("https://")) {
        return;
    }
    url = removeQueryAndProtocol(url);

    console.log(url);

    if (url == "" || url == "chrome://newtab/" || url == "edge://newtab/" ||
        url.startsWith("www.google.com/") || url.startsWith("www.bing.com/") ||
        url.startsWith("search.brave")) {
        return;
    }



    // Construct message to send to the other application
    const message = {
        action: "add",
        module: "tracking",
        submodule: "browser",
        options: {
            url: url,
            title: title,
            time: new Date().toLocaleTimeString('en-US', { hour12: false })
        }
    };

    console.log(JSON.stringify(message));

    // Send message to the other application via HTTP POST request
    sendHttpPostRequest(message);
}

function sendTranslationRequest(text) {
    const command = {
        action: "translate",
        module: "translation",
        submodule: "jp",
        options: {
            term: text
        }
    };

    sendHttpPostRequest(command, true);

}

// Function to send HTTP POST request to the other application
function sendHttpPostRequest(data, awaitResponse = false) {
    // URL of the endpoint in the other application
    const endpointUrl = 'http://localhost:47100/';
    console.log('Sending HTTP POST request to:', endpointUrl);
    // Convert data object to JSON string
    const jsonData = JSON.stringify(data);

    // Define HTTP request options
    const requestOptions = {
        method: 'POST',
        mode: 'no-cors',
        headers: {
            'Content-Type': 'application/json'
        },
        body: jsonData
    };

    if (awaitResponse) {
        // Send HTTP POST request and return the response
        return fetch(endpointUrl, requestOptions)
            .then(response => response.json())
            .then(data => {
                console.log('Response:', data);
                return data;
            })
            .catch(error => {
                console.error('Error sending HTTP POST request:', error);
            });
    }

    // Send HTTP POST request and ignore the response
    fetch(endpointUrl, requestOptions)
        .catch(error => {
            console.error('Error sending HTTP POST request:', error);
        });
}

function removeQueryAndProtocol(url) {
    // Parse the URL string using URL constructor
    const parsedUrl = new URL(url);

    // Remove query parameters
    const urlWithoutParams = parsedUrl.origin + parsedUrl.pathname;

    // Remove protocol
    const urlWithoutProtocol = urlWithoutParams.replace(/^https?:\/\//, '');

    return urlWithoutProtocol;
}


