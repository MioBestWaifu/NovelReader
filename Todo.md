# Major
- ~~Rework the current translation system to include multiple keys for one entry, as well as to include the names dictionary and KanjiDict~~
- ~~Implement FluentUI across the project~~
- ~~Implement library (picking books folder, saving progress)~~
- Implement translation of entire sentence
- Implement extensive customization. Colors, fonts, sizes, directions and translation level should all be changed by settings.
- The Analyzer has certain undesirable behaviors, like separating flexionings and auxiliars from the verb, and breaking words into smaller fraction when they could be larger, that are not necessarily Mecab's fault, but rather the way the Unidic is made to work. To adjust those, it would be necessary to customize the Unidic. I have no idea how to do so and it would be a lot of work.
- Now that multiple results can return for the same translation, priority is important. ~~As it stands, the order of results is effectively random. It is possible to put some order based of the Edrdg alone, as it includes some usage level information, and some other attributes that may be used to order keys, such as if a work is usually rendered in kana.~~ But it would be better to add a weight to the priority based on how many times a key was used. This is probably not that difficult, there must be a dozen models out there that can be adapted and the difficulty would be to gather enough data to do it and resolve ambiguities. Another way is to weight priority at translation time based on the context. Again, that would be difficult but not impossible, there must be a bunch of ready models. But that requires even more data and might even be technically unfeasible for the target devices.
- Look into if the translation pipeline continues even after a book page has been closed or navigated out. It possibly does because blazor keeps pages around since you can navigate back to them after navigating out, and besides the pipeline exists on background threads. If this does happen, it should be stopped.

# Minor
- ~~Mobile landscape port is shitty. Should:~~
	- ~~Take zooming into account when swiping~~
	- ~~Display large vertical images vertically, the full-width on-demand-height scheme only works well for large screens~~.
	- ~~Possibly, replace swipe pagination with tap pagination, and hold the text to translate~~
- Display the furiganas (either from rb tags or from the translation pipeline) over kanjis in a easy way. Either by rendering them on top (rb tag) or by hovering with a tooltip. Both should be optional.
- Expand the grammatical categories and rethink the way they are used in translations. Right now there is a workadround in place to deal with na-adjectives and things acting like suffixes in compound words mecab breaks, such as 車 in 自動車, get treated as unknown.


/*
                                 * One of the nodes that gets errored here is some hiragana MeCab turns into 踏ん反る. The JMDict
                                 * does not contain an entry for that, and other EDRDG-based translators cannot find it either, so not my fault.
                                 * Sometimes the same happens with other verbs written in hiragana that are found in ohter EDRDG-Based dictionaries, so that is my fault,
                                 * most likely because the dictionary-building process only uses one key and is overall very faulty and simple.
                                 * Also, there is a possibility that the Analyzer is fucking some things up by converting hiragana to kanji. 
                                 * I do not understand fucks of Mecab inner workings, so it might actually do a good job of
                                 * determining the correct kanjification when many kanji words have the same reading. But if it
                                 * doesn't, then it may screw the translation over. Dont know what to do about it if true, 
                                 * because even though it is the idea that at some point all possible keys to a word will be in the conversion table,
                                 * it may be flexed even in hiragana and Mecab will be needed to deal with that. Maybe this is configurable but I dont know.
                                 * 
                                 * Another kind of common error is for weird ass fantasy names (testing this with Tensai Oujo to Tensei Reijou no Mahoukakumei) 
                                 * and other things (mostly) written in katakana that the Translator cannot make sense of. It is not suposed to either.
                                 * Those should: A) be inserted into the dictionary from a custom database or 
                                 * B) be parsed to hiragana or C) be ignored.
                                 * 
                                 * Anyways, not fixing any of that now, this version is intended for the display parts only.
                                 * 
                                 */
