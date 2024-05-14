# Version History adn Roadmap
This is a description of the major changes implemented in past versions, being implemented in the version under development, and planned for the future.

## Current version: 0.5
- Create the Analysis module, by categorizing processes and webpages, and reporting on them
- Optmize translation
- Optimize tracking records
- Take environment variables, basis for future user personalization
- General code revision in Services

## Past versions

### 0.4
- Creation of Extension options page to select what should be tracked and watched.
- Creation of Extension translations page to record recent translations.
- Creation of Extension command page to send commands to Services.


### 0.3
- Creation of Tester project
- Transition the Extension to Angular 
- Parsing the JMDict to a folder of JSONs and a conversion table. The conversion table contains lexemes as keys and a reference to file-offset address as values. As such, only the refence key is kept in memory and the full data is searched by loading from the JSONs as needed.
- Using Unidic for morphological analysis

### 0.2
- Browser selection watching
- Translation japanese-to-english of entries in the JMDict

### 0.1
- Initialization
- Browser and process tracking, saving the data to CSV 

## Roadmap

### To be determined
- Japanese-to-english translation
    - Use JMDict fully. Many properties are ignored, but most should not be because they are important.
        - One of them determines if a entry is usually in kana. If it is, kana should be included in the search keys irrespective of kanji
        - Alternatively, kana keys should be included anyways
    - Use Kanjidic
    - Use JMnedict
- Integrate with Kavita
    - Download and install it on Command 
    - Launch it and read specific book on Command
    - Track it properly
- Have a downloading module to get the optional data 
- Schedule 
    - Create tasks
    - Send reminders
- Categorize processes and web pages for the purposes of analysis.