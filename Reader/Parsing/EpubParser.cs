﻿using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;
using Mio.Translation.Japanese;
using Mio.Translation.Japanese.Edrdg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
#if WINDOWS
using Windows.Graphics.Printing.Workflow;
#endif

namespace Mio.Reader.Parsing
{

    internal static class EpubParser
    {

        //([,!?、。．。「」『』…．！？：；（）()'\"“”])";
        //include any other separators that might be missing
        //Why two fields? Class regex as above is fast for regexing, list is faster for comparing.
        private static readonly List<string> separatorsAsList = new List<string> { ",", "!", "?", "、", "。", "．", "「", "」", "『", "』", "…", "．", "！", "？", "：", "；", "（", "）", "(", ")", "'", "\"", "“", "”" };

        private static readonly string separatorsRegex = "([" + string.Join("", separatorsAsList.Select(Regex.Escape)) + "])";

        //Obviouslt breaks if configs is not assigned before analyzer, but that should never happen because this field is assinged in the very ConfigurationsService constructor.
        public static ConfigurationsService Configs { private get { return configs; } set { configs = value; analyzer = new JapaneseAnalyzer(value.PathToUnidic); } }
        private static ConfigurationsService configs;
        public static JapaneseAnalyzer analyzer;

        private static JapaneseTranslator translator = new JapaneseTranslator();



        public static Task<List<Node>> ParseLine(Chapter chapter, XElement line)
        {
            try
            {
                if (line.Name == Namespaces.xhtmlNs + "p")
                {
                    return Task.FromResult(ParseTextElement(line));
                }

                else if (line.Name == Namespaces.xhtmlNs + "img" || line.Name == Namespaces.svgNs + "svg")
                {
                    if (line.Name == Namespaces.svgNs + "svg")
                    {
                        XElement imageElement = line.Element(Namespaces.svgNs + "image");
                        if (imageElement != null)
                        {
                            return Task.FromResult(ParseImageElement(chapter, imageElement, Namespaces.xlinkNs +"href"));
                        }
                    }
                    else
                    {
                        return Task.FromResult(ParseImageElement(chapter, line, "src"));
                    }
                }

                return Task.FromResult(new List<Node>());
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error parsing line: {e.Message}");
                Debug.WriteLine($"Line: {line}");
                Debug.WriteLine(e.StackTrace);
                return Task.FromResult(new List<Node>());
            }
        
        }


        private static List<Node> ParseTextElement(XElement originalElement)
        {

            string line = GetParagraphText(originalElement);
            if (line == string.Empty)
            {
                return [new TextNode() { Text = "" }];
            }
            //Breaking lines into sentences for smoother morphological analysis
            string[] sentences = Regex.Split(line, separatorsRegex);
            List<Node> nodes = new List<Node>();
            for (int i = 0, n = sentences.Length; i < n; i++)
            {
                //Should never be zero, i think. If it happens, will cause a bug. Purposefully not checking to see if breaks.
                //Also, This means that the separators are not interactable as part of a word. This is not a problem, because separators ARE NOT words.
                if (sentences[i].Length == 1 && separatorsAsList.Contains(sentences[i]))
                {
                    if(nodes.Count == 0)
                    {
                        nodes.Add(new TextNode() { Text = sentences[i] });
                    } else
                    {
                        nodes[^1].Text += sentences[i];
                    }
                    continue;
                }

                List<JapaneseLexeme> lexemes = analyzer.Analyze(sentences[i]);
                foreach (var lexeme in lexemes)
                {
                    TextNode node = new TextNode();
                    node.Text = lexeme.Surface;
                    nodes.Add(node);
                    if (JapaneseAnalyzer.signicantCategories.Contains(lexeme.Category))
                    {
                        try
                        {
                            //Only one entry because i dont want to deal with multiple entries in the UI now.
                            node.EdrdgEntry = translator.Translate(lexeme.BaseForm)![0];
                        }
                        catch (Exception e)
                        {
                            /*
                             * One of the nodes that gets errored here is some hiragana MeCab turns into 踏ん反る. The JMDict
                             * does not contain an entry for that, and other EDRDG-based translators cannot find it either, so not my fault.
                             * Sometimes the same happens with other verbs written in hiragana that are found in ohter EDRDG-Based dictionaries, so that is my propably fault,
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
                            Debug.WriteLine($"Error translating node: {lexeme.Surface} {lexeme.Category}");
                        }
                    }
                }
            }

            return nodes;
        }

        private static List<Node> ParseImageElement(Chapter chapter, XElement originalElement, string srcAttribute)
        {
            string path = originalElement.Attribute(srcAttribute)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return ParseImageElement(imageEntry);
        }

        private static List<Node> ParseImageElement(Chapter chapter, XElement originalElement, XName srcName)
        {
            string path = originalElement.Attribute(srcName)!.Value;
            ZipArchiveEntry imageEntry = Utils.GetRelativeEntry(chapter.FileReference, path);
            return ParseImageElement(imageEntry);
        }

        private static List<Node> ParseImageElement (ZipArchiveEntry imageEntry)
        {
            byte[] imageBytes = new byte[imageEntry.Length];
            using (var stream = imageEntry.Open())
            {
                stream.ReadAsync(imageBytes, 0, imageBytes.Length).Wait();
            }
            string base64 = Convert.ToBase64String(imageBytes);

            return [new ImageNode() { Text = base64 }];
        }

        /// <summary>
        /// Breaks a Xhtml chapter into lines. Paragraphs are considered lines, irrespective of content. Images are considered lines as well.
        /// </summary>
        /// <param name="originalXhtml"></param>
        /// <returns>The </returns>
        private static Task<List<XElement>> BreakXhtmlToLines(string originalXhtml)
        {
            // Parse the XHTML content
            XDocument doc = XDocument.Parse(originalXhtml);

            // List to store the lines
            List<XElement> lines = doc.Descendants().Where(n => n.Name == Namespaces.xhtmlNs + "p" || n.Name == Namespaces.xhtmlNs + "img" || n.Name == Namespaces.svgNs +"svg").ToList();

            return Task.FromResult(lines);
        }

        public static async Task<List<XElement>> BreakChapterToLines(Chapter chapter)
        {
            string orginalXhtml = await new StreamReader(chapter.FileReference.Open()).ReadToEndAsync();

            return await BreakXhtmlToLines(orginalXhtml);
        }

        private static string GetParagraphText(XElement paragraph)
        {
            if (paragraph.IsEmpty || paragraph.HasElements && paragraph.Elements(Namespaces.xhtmlNs + "br").Count() >= 1 && paragraph.Value.Trim() == "")
            {
                return string.Empty;
            }
            // Use a StringBuilder for efficiency
            StringBuilder sb = new StringBuilder();

            foreach (var node in paragraph.Nodes())
            {
                if (node is XText textNode)
                {
                    sb.Append(textNode.Value);
                }
                else if (node is XElement elementNode)
                {
                    if (elementNode.Name == Namespaces.xhtmlNs + "ruby")
                    {
                        // Add the ruby content (kanji) and ignore rt tags
                        var rubyText = elementNode.Nodes().Where(n => !(n is XElement e && e.Name == Namespaces.xhtmlNs + "rt")).Select(n => n.ToString());
                        sb.Append(string.Join("", rubyText));
                    }
                    else if (elementNode.Name != Namespaces.xhtmlNs + "rt")
                    {
                        sb.Append(elementNode.Value);
                    }
                }
            }

            return sb.ToString();
        }
    }
}