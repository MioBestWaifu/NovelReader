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
      // Log or process the selected text.
      //Check if text is different from the previous when actually implementing this
      console.log('Selected text:', message.text);
    }
});
  
/* });
 */
// Function to send tab information to the other application
function sendTabInfoToHost(tab) {
    // Extract title and URL from the tab
    const title = tab.title;
    const url = decodeURIComponent(tab.url);
    url = removeQueryAndProtocol(url);

    if (url == "" || url == "chrome://newtab/" || url == "edge://newtab/" ||
        url.startsWith("https://www.google.com/") || url.startsWith("https://www.bing.com/") ||
        url.startsWith("https://search.brave")){
        return;
    }

    if(!url.startsWith("http://") && !url.startsWith("https://")){
        return;
    }


    // Construct message to send to the other application
    const message = {
        title: title,
        url: url
    };

    console.log(JSON.stringify(message));

    // Send message to the other application via HTTP POST request
    sendHttpPostRequest(message);
}

// Function to send HTTP POST request to the other application
function sendHttpPostRequest(data) {
    // URL of the endpoint in the other application
    const endpointUrl = 'http://localhost:47100/';

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

    // Send HTTP POST request
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


