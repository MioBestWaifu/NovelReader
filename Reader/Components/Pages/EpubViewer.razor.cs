using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Parsing;
using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using TouchEventArgs = Microsoft.AspNetCore.Components.Web.TouchEventArgs;
using Mio.Translation;
#if ANDROID
using Xamarin.Google.Crypto.Tink.Proto;
#endif
using Mio.Reader.Utilitarians;

namespace Mio.Reader.Components.Pages
{
    public partial class EpubViewer
    {
        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private ConfigurationsService Configs { get; set; }
        [Inject]
        private LibraryService Library { get; set; }
        [Inject]
        private Translator Translator { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int BookIndex { get; set; } = 0;


        private readonly DevicePlatform plataform = DeviceInfo.Current.Platform;

        private Epub? Book { get; set; } = null;
        private EpubInteraction interaction;
        //I sure have a penchant for nested lists. But what can I do? A book IS a list of pages, a page IS a list of lines, and a line IS a list of words. I dont make the rules. Is there some smarter data structure? Probably. But who cares, this is fine.
        private List<List<List<Node>>> Pages { get; set; } = new List<List<List<Node>>>();

        private int CurrentChapter { get; set; } = 0;
        private int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                try
                {
                    currentPage = value;
                    navigatorClass = "rdr-navigator";
                    InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                    Task.Run(() => StartOrResetNavigatorAnimation());
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.InnerException);
                    Debug.WriteLine(e.StackTrace);
                }
            }
        }


        private int currentPage;

        private int linesPerPage;

        private bool Initialized { get; set; } = false;

        private string previousElementId = "";
        private int touchId = 0;

        private bool showPopup = false;
        private bool showTableOfContents = false;


        private string navigatorClass;
        private int animationCounter = 0;

        private string selectedWordId = "";
        private TextNode? selectedNode;

        private int fuckedLines = 0;

        protected override Task OnInitializedAsync()
        {
            if (Configs.ReadingManner == ReadingManner.Japanese)
            {
                JS.InvokeVoidAsync("changeDirectionToRTL");
            }
            else
            {
                JS.InvokeVoidAsync("changeDirectionToLTR");
            }
            CurrentPage = 0;
            interaction = Library.Books[BookIndex];
            CurrentChapter = interaction.LastChapter;
            if (EpubParser.analyzer is null)
                EpubParser.analyzer = new Analyzer(Configs.PathToUnidic);
            Task.Run(() => SetBook());
            return base.OnInitializedAsync();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JS.InvokeVoidAsync("setEpubViewerReference", DotNetObjectReference.Create(this));
                DeviceDisplay.Current.MainDisplayInfoChanged += HandleMainDisplayInfoChanged;
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        private async Task SetBook()
        {

            try
            {
                Book = await EpubLoader.LoadEpub(interaction.Metadata);
                Task.Run(() => LoadChapter(CurrentChapter, true));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
                Debug.WriteLine(e.StackTrace);
            }
        }


        private async void StartOrResetNavigatorAnimation()
        {
            animationCounter++;
            await Task.Delay(5000);
            animationCounter--;
            if (animationCounter <= 0)
            {
                animationCounter = 0;
                navigatorClass = "rdr-navigator rdr-start-animation";
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void NextPage()
        {
            if (showPopup)
            {
                ClosePopup();
            }
            if (CurrentPage < Pages.Count - 1)
            {
                CurrentPage++;
                interaction.LastPage = CurrentPage;
            }
            else
            {
                NextChapter();
            }

            if (Configs.ReadingManner == ReadingManner.Japanese)
                JS.InvokeVoidAsync("scrollToHorizontalStart");
            else
                JS.InvokeVoidAsync("scrollToVerticalStart");
        }

        private void PreviousPage()
        {
            if (showPopup)
            {
                ClosePopup();
            }
            if (CurrentPage > 0)
            {
                CurrentPage--;
                interaction.LastPage = CurrentPage;
            }
            else
            {
                PreviousChapter();
            }

            if (Configs.ReadingManner == ReadingManner.Japanese)
                JS.InvokeVoidAsync("scrollToHorizontalStart");
            else
                JS.InvokeVoidAsync("scrollToVerticalStart");
        }


        private void NavigateToPage(ChangeEventArgs e)
        {
            if (showPopup)
            {
                ClosePopup();
            }

            int page = int.Parse(e.Value.ToString());
            if (page < 1 || page > Pages.Count)
            {
                return;
            }

            page--;

            if (CurrentPage != page)
            {
                CurrentPage = page;
                interaction.LastPage = CurrentPage;
                if (Configs.ReadingManner == ReadingManner.Japanese)
                    JS.InvokeVoidAsync("scrollToHorizontalStart");
                else
                    JS.InvokeVoidAsync("scrollToVerticalStart");
            }
        }

        private void NavigateToChapter(int index)
        {
            if (showPopup)
            {
                ClosePopup();
            }
            if (CurrentChapter != index)
            {
                CurrentChapter = index;
                interaction.LastChapter = CurrentChapter;
                interaction.LastPage = 0;
                Task.Run(() => LoadChapter(CurrentChapter));
            }
        }

        private void NextChapter()
        {
            if (CurrentChapter < Book?.TableOfContents.Count - 1)
            {
                CurrentChapter++;
                interaction.LastChapter = CurrentChapter;
                interaction.LastPage = 0;
                Task.Run(() => LoadChapter(CurrentChapter));
            }
        }

        private void PreviousChapter()
        {
            if (CurrentChapter > 0)
            {
                CurrentChapter--;
                interaction.LastChapter = CurrentChapter;
                interaction.LastPage = 0;
                Task.Run(() => LoadChapter(CurrentChapter, setPageToLast: true));
            }
        }

        private async Task LoadChapter(int index, bool firstLoad = false, bool setPageToLast = false)
        {
            Chapter chapter = Book.TableOfContents[index].Item2;
            //To keep track of if the user went to another chapter while the current one was loading
            int thisTaskChapterIndex = CurrentChapter;
            List<Task> parsingTasks = new List<Task>();

            //There needs to be error handling here. Like, a chapter cannot be in loading state forever if something breaks in the loadings. Also, the error needs to be shown and logged.
            if (chapter.LoadStatus == LoadingStatus.Unloaded)
            {
                List<XElement> lines = await EpubParser.BreakChapterToLines(chapter);
                Pages = await PreparePages(lines);
                if (firstLoad && interaction.LastTimeNumberOfPages != 0)
                {
                    CurrentPage = interaction.LastTimeNumberOfPages == Pages.Count ? interaction.LastPage : 0;
                }
                else if (setPageToLast)
                {
                    CurrentPage = Pages.Count - 1;
                }
                else
                {
                    CurrentPage = 0;
                }

                interaction.LastTimeNumberOfPages = Pages.Count;

                chapter.PrepareLines(lines.Count);

                //For the first loading
                Initialized = true;

                chapter.LoadStatus = LoadingStatus.Loading;
                for (int i = 0; i < lines.Count; i++)
                {
                    int currentIndex = i; // Capture the current index
                    parsingTasks.Add(EpubParser.ParseLine(chapter, lines[currentIndex]).ContinueWith(async parseLineTask =>
                    {
                        Debug.WriteLine("Parsing line " + currentIndex);
                        List<Node> line = parseLineTask.Result;
                        chapter.PushLineToIndex(currentIndex, line);
                        if (thisTaskChapterIndex == CurrentChapter)
                        {
                            PushLineToPages(currentIndex, line);
                            if (currentIndex/linesPerPage == currentPage) {
                                InvokeAsync(() =>
                                {
                                    StateHasChanged();
                                });
                            }
                        }
                    }));
                }

                await Task.WhenAll(parsingTasks);
                int j = 0;
                foreach (List<Node> line in chapter.Lines)
                {
                    int currentLineIndex = j++;
                    foreach (Node node in line)
                    {
                        if (node is TextNode textNode)
                        {
                            Task.Run(() => TranslateFragment(textNode).ContinueWith(async _ =>
                            {
                                chapter.FinishedTextNodes++;
                                if (currentLineIndex/linesPerPage == currentPage) {
                                    InvokeAsync(() =>
                                    {
                                        StateHasChanged();
                                    });
                                }
                            }));
                        }
                    }
                }
            }


            else
            {
                Pages = await BreakChapterToPages(chapter);
                if (setPageToLast)
                {
                    CurrentPage = Pages.Count - 1;
                }
                else
                {
                    CurrentPage = 0;
                }
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private async Task TranslateFragment(TextNode node, bool translateGeneral = true, bool translateNames = true, bool translateChars = true)
        {
            if(node.lexeme is null)
            {
                return;
            }

            Task generalTask = Task.Run(async () =>
            {
                if (translateGeneral && node.lexeme is not null && Analyzer.signicantCategories.Contains(node.lexeme.Category))
                {
                    node.JmdictEntries = await Translator.TranslateWord(node.lexeme.BaseForm);
                    node.HasFinishedGeneral = true;
                }
            });

            Task namesTask = Task.Run(async () => { 
                if (translateNames)
                {
                    node.NameEntry = await Translator.TranslateName(node.lexeme.Surface);
                    node.HasFinishedNames = true;
                }
            });

            await Task.WhenAll(generalTask, namesTask);

            Task charsTask = Task.Run(async () =>
            {
                if (translateChars)
                {
                    List<Task> charTranslationTasks = [];
                    for (int i = 0; i <node.Characters.Count; i++)
                    {
                        JapaneseCharacter iterationCharacter = node.Characters[i];
                        if (iterationCharacter is Romaji)
                            //Presumes romaji only occurs in romaji-only words. I am not aware of any circumstance where this is not true.
                            break;
                        else if (iterationCharacter is Kana kana)
                        {
                            if (kana.IsYoon)
                            {
                                try
                                {
                                    Kana? previousNonYoonKana = null;
                                    for (int j = i - 1; j >= 0; j--)
                                    {
                                        if (node.Characters[j] is Kana possibleKana && !possibleKana.IsYoon)
                                        {
                                            previousNonYoonKana = possibleKana;
                                            break;
                                        }
                                    }
                                    if(previousNonYoonKana is null)
                                    {
                                        continue;
                                    }
                                    if (previousNonYoonKana.Composition is null)
                                    {
                                        previousNonYoonKana.Composition = previousNonYoonKana.Literal.ToString();
                                    }
                                    previousNonYoonKana.Composition += kana.Literal;
                                    previousNonYoonKana.Reading = Translator.TranslateKana(previousNonYoonKana.Composition);
                                } catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }
                            }
                            else 
                            {
                                kana.Reading = Translator.TranslateKana(iterationCharacter.Literal.ToString());
                            }
                        }
                        else if (iterationCharacter is Kanji kanji)
                        {
                            Task task = Task.Run(async () =>
                            {
                                kanji.Entry = await Translator.TranslateKanji(kanji.Literal);
                            }
                            );
                            charTranslationTasks.Add(task);
                        }
                    }

                    await Task.WhenAll(charTranslationTasks).ContinueWith(_ =>
                    {
                        node.HasFinishedChars = true;
                    });
                }
            });

            await charsTask;

        }

        private void PushLineToPages(int index, List<Node> line)
        {
            int linesPerPage = Pages[CurrentPage].Count;
            int pageIndex = Math.DivRem(index, linesPerPage, out int lineIndex);
            Pages[pageIndex][lineIndex] = line;
        }

        private async Task<int> UpdateLinesPerPage()
        {
            int linesPerPage;
            if (Configs.ReadingManner == ReadingManner.Japanese)
            {
                //Value out of my ass, replace with something actual based on the used css + margins
                int lineWidth = 40;
                int windowWidth = await JS.InvokeAsync<int>("getWindowWidth");
                linesPerPage = windowWidth / lineWidth;
            }
            else
            {
                //Value out of my ass, replace with something actual based on the used css + margins
                int lineHeight = 30;
                int windowHeight = await JS.InvokeAsync<int>("getWindowHeight");
                linesPerPage = windowHeight / lineHeight;
            }

            this.linesPerPage = linesPerPage;

            return linesPerPage;
        }

        private async Task<List<List<List<Node>>>> PreparePages(List<XElement> lines)
        {
            int linesPerPage = await UpdateLinesPerPage();

            List<List<List<Node>>> pages = new List<List<List<Node>>>();

            int totalLines = lines.Count;
            int totalPages = (totalLines + linesPerPage - 1) / linesPerPage; // Calculate total pages

            for (int i = 0; i < totalPages; i++)
            {
                List<List<Node>> page = new List<List<Node>>(linesPerPage); // Create a new page with linesPerPage lines
                for (int j = 0; j < linesPerPage; j++)
                {
                    page.Add([new Node() { }]); // Add an empty line to the page
                }
                pages.Add(page); // Add the page to the pages
            }

            return pages;
        }


        private async Task<List<List<List<Node>>>> BreakChapterToPages(Chapter chapter)
        {

            int linesPerPage = await UpdateLinesPerPage();
            List<List<List<Node>>> pages = [];
            List<List<Node>> currentPage = [];
            foreach (List<Node> line in chapter.Lines)
            {
                currentPage.Add(line);

                if (currentPage.Count == linesPerPage)
                {
                    pages.Add(currentPage);
                    currentPage = new List<List<Node>>();
                }
            }

            // Add the last page if it has any lines
            if (currentPage.Count > 0)
            {
                pages.Add(currentPage);
            }

            return pages;
        }

        private async void ShowPopup(string id, TextNode? node)
        {
            if (node is null || node.Characters is null)
            {
                return;
            }
            selectedWordId = id;
            selectedNode = node;
            if (showPopup)
            {
                await RemovePreviousElementBackgroundColor();
            }
            showPopup = true;
            await JS.InvokeVoidAsync("setElementBackgroundColor", id, "var(--neutral-stroke-layer-rest");
            previousElementId = id;
            StateHasChanged();
        }

        private async void ClosePopup()
        {
            showPopup = false;
            selectedNode = null;
            selectedWordId = "";
            await RemovePreviousElementBackgroundColor();
        }

        private void HandleClickOnElement(MouseEventArgs e, string id, TextNode node)
        {
            if (plataform != DevicePlatform.WinUI)
                return;
            if (showPopup)
            {
                ClosePopup();
            }
            ShowPopup(id, node);
        }

        private async void HandleTouchStart(TouchEventArgs e, string elementId, TextNode node)
        {
            if (plataform != DevicePlatform.Android)
                return;
            this.touchId++;
            int currentTouchId = this.touchId;

            await Task.Delay(750);
            if (currentTouchId == this.touchId)
            {
                if (showPopup)
                {
                    await InvokeAsync(() => ClosePopup());
                }
                await InvokeAsync(() => ShowPopup(elementId, node));
                return;
            }
        }

        private void HandleKeyPress(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Escape:
                    if (showPopup)
                        ClosePopup();
                    break;
                //These should be different on image-only pages, which is one more reason to have separate components for separate chapter types
                case Keys.ArrowLeft:
                    if (Configs.ReadingManner == ReadingManner.Western)
                        PreviousPage();
                    break;
                case Keys.ArrowRight:
                    if (Configs.ReadingManner == ReadingManner.Western)
                        NextPage();
                    break;
                case Keys.ArrowUp:
                    if (Configs.ReadingManner == ReadingManner.Japanese)
                        PreviousPage();
                    break;
                case Keys.ArrowDown:
                    if (Configs.ReadingManner == ReadingManner.Japanese)
                        NextPage();
                    break;
            }
            //Does not work without this call. Shouldn't Blazor be able to detect changes and re-render automatically?
            StateHasChanged();
        }

        private async Task RemovePreviousElementBackgroundColor()
        {
            await JS.InvokeVoidAsync("removeElementBackgroundColor", previousElementId);
            previousElementId = "";
        }

        public async void HandleDoubleClick(MouseEventArgs e)
        {
            //I think this is undesirable on Windows, but it might actually not be
            if (plataform != DevicePlatform.Android)
            {
                return;
            }

            double x = e.ScreenX;

            double windowWidth = await JS.InvokeAsync<int>("getWindowWidth");
            double maxXToLeft = 0.33 * windowWidth;
            double minXToRight = 0.66 * windowWidth;

            if (x < maxXToLeft)
            {
                if (Configs.ReadingManner == ReadingManner.Japanese)
                    NextPage();
                else
                    PreviousPage();
            }
            else if (x > minXToRight)
            {
                if (Configs.ReadingManner == ReadingManner.Japanese)
                    PreviousPage();
                else
                    NextPage();
            }
        }

        [JSInvokable]
        public async void HandleWindowResize()
        {
            try
            {
                //Deactivated this because upon testing i felt like the lines per page can be kept as is, it feels ok.
                //However, if this is to be used, two things must be done:
                //1 - The lines calculation has to be reworked to account for height, which obviously affects the width of the line.
                //2 - The page the user will end at has to be recalculated to be the equivalent of where he currently is.
                //Pages = await BreakChapterToPages(Book!.TableOfContents[CurrentChapter].Item2);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
                Debug.WriteLine(e.StackTrace);
            }
        }

        public void HandleMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs? e)
        {
            //Same as above, deactivated because it's not needed for now
            Debug.WriteLine("Display info changed");
        }
    }
}
