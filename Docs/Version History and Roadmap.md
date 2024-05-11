# Version History adn Roadmap
This is a description of the major changes implemented in past versions, being implemented in the version under development, and planned for the future.

## Current version: 0.3
The current version is to be a stepping stone for future devolepment. New major features are not expected, its goal is to optimize the existing features. To that end, the following will be done, in no particular order: 

- Create and implement a Tester project to run alongside Services in development environment, so that real-time testing features dont have to be integrated into the working Services branch. (DONE)
- Transition the Extension to Angular (DONE)
- Try, and if sucessfull integrate, SQLite into Maria to maintain translation and tracking data. If this is implemented, the curent dictionary-reading logic will be separated from JapaneseTranslator. It will remain in Services.  (FAIL)
- Update the japanese translation to use data in storage and a cache. Create a implementation to conver the EDRDG database into JSON files.

## Past versions

### 0.2
- Browser selection watching
- Translation japanese-to-english of entries in the JMDict

### 0.1
- Initialization
- Browser and process tracking, saving the data to CSV 

## Roadmap

### 0.4
- Extension options, select what should be tracked and watched
- Extension full translations page, formated properly

### To be determined
- Japanese-to-english translation
    - Use JMDict fully. Many properties are ignored, but most should not be because they are important.
        - One of them determines if a entry is usually in kana. If it is, kana should be included in the search keys irrespective of kanji
    - Use Kanjidic
    - Use JMnedict
    - Allow the use of Unidict por morphological analysis
- Integrate with Kavita
    - Download and install it on Command 
    - Launch it and read specific book on Command
    - Track it properly
- Have a downloading module to get the optional data 
- Schedule 
    - Create tasks
    - Send reminders