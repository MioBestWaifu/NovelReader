chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message && message.type === 'translation') {
        console.log("Received translation");
        let translationObject = JSON.parse(message.text);
        console.log(JSON.stringify(translationObject));

        let mainElement = document.getElementById('main');
        let newDiv = document.createElement('div');
        newDiv.textContent = translationObject.senseElements[0].glosses[0];
        mainElement.appendChild(newDiv);
    }
});