# Version History and Roadmap
This is a description of the major changes implemented in past versions, being implemented in the version under development, and planned for the future.

## Current version: 0.7
- Build an Epub reader capable of handling japanese light novels in Readers.
- Integrate said reader with Translation in NovelReader.

## Past versions
### 0.6
  - Reorganized the architecture into a three-tiered structure: Common, Services, and Implementations.
  - Created the Readers and NovelReader Implementations.

### 0.5
- Translation storage and access optimized by using MessagePack
- Tracking storage optimized by using MessagePack and consolidating daily files
- Taking enviroment variable for the various paths and options

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
    - Automatically tro to transalte a whole page or section.
    - Maintain state of translation on session, integrated with cache, restoring the recent translation to the Extension if the Extension is reinitialized.
- Integrate with Kavita
    - Download and install it on Command 
    - Launch it and read specific book on Command
    - Track it properly
- Have a downloading module to get the optional data 
- Schedule 
    - Create tasks
    - Send reminders
- Categorize processes and web pages for the purposes of analysis.