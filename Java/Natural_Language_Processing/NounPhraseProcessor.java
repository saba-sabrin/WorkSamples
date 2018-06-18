/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package nlp.semantic.relations;

import edu.stanford.nlp.ling.CoreAnnotations;
import edu.stanford.nlp.ling.CoreLabel;
import edu.stanford.nlp.ling.tokensregex.TokenSequenceMatcher;
import edu.stanford.nlp.ling.tokensregex.TokenSequencePattern;
import edu.stanford.nlp.pipeline.Annotation;
import edu.stanford.nlp.simple.Sentence;
import edu.stanford.nlp.util.CoreMap;
import nlp.helper.Helper;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

/**
 *
 * @author Saba Sabrin
 */
public class NounPhraseProcessor {

    //NP: {<DT|PP\$>?<JJ>*<NN>+} {<NNP>+}	{<NNS>+}
    private TokenSequencePattern pattern = TokenSequencePattern.compile("([tag:DT])? ([tag:JJ])* ([tag:NN]|[tag:NNS]|[tag:NNPS]|[tag:NNP])+");
    private List<String> stopwords;
    public static int totalDocumentWordCount = 0;

    public NounPhraseProcessor(String stopwordsFile) {
        try {
            stopwords = loadStopwords(stopwordsFile);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    /**
     * Created By: Saba Sabrin
     * @param document
     * @param removeduplicates
     * @return List of String type elements
     */
    public List<String> getNounPhrases(Annotation document, boolean removeduplicates) {

        List<String> result = new ArrayList<String>();
        List<CoreLabel> tokens = new ArrayList<CoreLabel>();

        List<CoreMap> sentences = document.get(CoreAnnotations.SentencesAnnotation.class);

        for (CoreMap sentence : sentences) {

            for (CoreLabel token : sentence.get(CoreAnnotations.TokensAnnotation.class)) {
                tokens.add(token);
            }

            TokenSequenceMatcher matcher = pattern.getMatcher(tokens);

            while (matcher.find()) {
                result.add(matcher.group());
            }

            totalDocumentWordCount = totalDocumentWordCount + tokens.size();
            tokens = new ArrayList<>();
        }

        if(!removeduplicates)
            return RemoveStopwords(result);
        
        Set<String> filteredSet = new HashSet<String>(RemoveStopwords(result));
        return new ArrayList(filteredSet);
    }

    /**
     * Created By: Saba Sabrin
     * @param document
     * @param NPLength
     * @param removeduplicates
     * @return List<String>
     */
    public List<String> getNounPhrases(Annotation document, int NPLength, boolean removeduplicates) {

        List<String> result = new ArrayList<String>();
        List<CoreLabel> tokens = new ArrayList<CoreLabel>();
        List<CoreMap> sentences = document.get(CoreAnnotations.SentencesAnnotation.class);

        for (CoreMap sentence : sentences) {

            for (CoreLabel token : sentence.get(CoreAnnotations.TokensAnnotation.class)) {
                tokens.add(token);
            }

            TokenSequenceMatcher matcher = pattern.getMatcher(tokens);

            while (matcher.find()) {

                //System.out.println(matcher.group());

                String[] terms = matcher.group().toString().split(" ");

                if(terms.length >= NPLength) {
                    result.add(matcher.group().toString().trim());
                }
            }
        }

        if(!removeduplicates) {
            // removal of stop words
            return removeStopwords(result);
        }

        // Duplicate word removal
        Set<String> filteredSet = new HashSet<String>(removeStopwords(result));
        return new ArrayList(filteredSet);
    }

    /**
     * 
     * @param document
     * @param removeStopWords
     * @return 
     */
    public String concatenateNounPhrases(Annotation document,Boolean removeStopWords) {

        List<String> result = new ArrayList<String>();
        List<CoreLabel> tokens = new ArrayList<CoreLabel>();
        String concatenatedText = "";

        List<CoreMap> sentences = document.get(CoreAnnotations.SentencesAnnotation.class);

        for (CoreMap sentence : sentences) {

            String currentSentence = sentence.toString();

            for (CoreLabel token : sentence.get(CoreAnnotations.TokensAnnotation.class)) {
                tokens.add(token);
            }

            TokenSequenceMatcher matcher = pattern.getMatcher(tokens);

            while (matcher.find()) {
                String match = matcher.group();
                String concept = ConcatenateWithoutPatternAdj(matcher.group());
                if(removeStopWords)
                    concept=removeStopwords(concept,"_");
                try {
                    currentSentence = currentSentence.replaceAll("\\b" + matcher.group() + "\\b", concept);
                } catch (Exception e) {
                    System.out.println(e.toString());
                }             
            }
            tokens = new ArrayList<CoreLabel>();
            concatenatedText += currentSentence + "\n";           
        }
        return concatenatedText;
    }

    /**
     * 
     * @param stopwordsFile
     * @return
     * @throws IOException 
     */
    private List<String> loadStopwords(String stopwordsFile) throws IOException {
        List<String> words = new ArrayList<String>();

        BufferedReader br = new BufferedReader(new FileReader(stopwordsFile));
        String line;
        while ((line = br.readLine()) != null) {
            words.add(line);
        }
        br.close();
        return words;
    }

    /**
     * 
     * @param source
     * @return 
     */
    public List<String> removeStopwords(List<String> source) {
        List<String> cleanedWords = new ArrayList<String>();

        for (int i = 0; i < source.size(); i++) {

            List<String> multiWordList = new ArrayList<String>(Arrays.asList(source.get(i).split(" ")));
            String np = "";
            for (int j = 0; j < multiWordList.size() - 1; j++) {
                if (stopwords.contains(multiWordList.get(j))) {
                    continue;
                }
                np += multiWordList.get(j) + " ";
            }

            np += multiWordList.get(multiWordList.size() - 1);
            cleanedWords.add(np);
        }
        return cleanedWords;
    }

    /**
     * 
     * @param source
     * @return 
     */
    public String removeStopwords(String source) {

        List<String> multiWordList = new ArrayList<String>(Arrays.asList(source.split(" ")));
        String np = "";
        for (int j = 0; j < multiWordList.size() - 1; j++) {
            if (stopwords.contains(multiWordList.get(j))) {
                continue;
            }
            np += multiWordList.get(j) + " ";
        }
        np += multiWordList.get(multiWordList.size() - 1);

        return np;
    }

    /**
     * 
     * @param source
     * @param splitter
     * @return 
     */
    public String removeStopwords(String source, String splitter) {

        List<String> multiWordList = new ArrayList<String>(Arrays.asList(source.split(splitter)));
        String np = "";
        for (int j = 0; j < multiWordList.size() - 1; j++) {
            if (stopwords.contains(multiWordList.get(j))) {
                continue;
            }
            np += multiWordList.get(j) + splitter;
        }
        np += multiWordList.get(multiWordList.size() - 1);

        return np;
    }

    /**
     * 
     * @param source
     * @return 
     */
    public String ConcatenateWithoutPatternAdj(String source) {
        List<String> multiWordList = new ArrayList<String>(Arrays.asList(source.split(" ")));
        String np = "np_";
        if ("other".equals(multiWordList.get(0)) || "such".equals(multiWordList.get(0))) {
            np = multiWordList.get(0) + " nP";
        } else {
            np = "np_" + multiWordList.get(0);
        }

        for (int j = 1; j < multiWordList.size(); j++) {
            if (stopwords.contains(multiWordList.get(j))) {
                continue;
            }
            np += "_" + multiWordList.get(j);
        }
        return np;
    }
}
