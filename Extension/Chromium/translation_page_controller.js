chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message && message.type === 'translation') {
        console.log("Received translation");
        let translationObject = JSON.parse(message.text);
        console.log(JSON.stringify(translationObject));

        let mainElement = document.getElementById('main');
        let newDiv = document.createElement('div');
        newDiv.className = 'translationBlock';

        let p1 = document.createElement('p');
        p1.textContent ="Original: " + translationObject[0].kanjiElements[0].kanji;
        newDiv.appendChild(p1);

        let p2 = document.createElement('p');
        p2.textContent ="Translation: " + translationObject[0].senseElements[0].glosses[0];
        newDiv.appendChild(p2);

        mainElement.appendChild(newDiv);
    }
});