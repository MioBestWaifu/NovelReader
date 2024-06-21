# Major
- Rework the current translation system to include multiple keys for one entry, as well as to include the names dictionary and KanjiDict
- ~~Implement FluentUI across the project~~
- ~~Implement library (picking books folder, saving progress)~~
- Implement translation of entire sentence
- Implement extensive customization. Colors, fonts, sizes, directions and translation level should all be changed by settings.

# Minor
- Mobile landscape port is shitty. Should:
	- ~~Take zooming into account when swiping~~
	- Display large vertical images vertically, the full-width on-demand-height scheme only works well for large screens.
	- ~~Possibly, replace swipe pagination with tap pagination, and hold the text to translate~~
- Display the furiganas (either from rb tags or from the translation pipeline) over kanjis in a easy way. Either by rendering them on top (rb tag) or by hovering with a tooltip. Both should be optional.