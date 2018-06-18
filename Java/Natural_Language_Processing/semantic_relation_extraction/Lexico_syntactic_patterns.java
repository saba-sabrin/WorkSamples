/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package nlp.semantic.relations.taxonomic;

import edu.stanford.nlp.ling.CoreAnnotations;
import edu.stanford.nlp.ling.CoreLabel;
import edu.stanford.nlp.ling.tokensregex.TokenSequenceMatcher;
import edu.stanford.nlp.ling.tokensregex.TokenSequencePattern;
import edu.stanford.nlp.pipeline.Annotation;
import edu.stanford.nlp.util.CoreMap;
import java.util.ArrayList;
import java.util.List;
import java.util.Arrays;
import nlp.semantic.relations.NounPhraseProcessor;

/**
 * Created by Saba Sabrin
 */
public class Lexico_syntactic_patterns {

    private List<Pattern> patterns = new ArrayList<Pattern>();
    private String np = "/np_(_|[a-zA-Z|\\d])+/";
    private NounPhraseProcessor nounPhraseProcessor;

    public Lexico_syntactic_patterns(String stopwords) {
        // example of tags based pattern 
        // patterns.add(new Pattern("([tag:DT])? ([tag:NN]|[tag:NNS])* /such/ /as/ ([tag:NN]|[tag:NNS])+ ( /,/ ([tag:NN]|[tag:NNS]))? ((and |or ) ([tag:NN]|[tag:NNS]))?", "first"));    
        // load Hearst patterns
        this.patterns.addAll(Hearst_patterns());
        nounPhraseProcessor = new NounPhraseProcessor(stopwords);
    }

    public Lexico_syntactic_patterns(List<Pattern> patterns, String stopwords) {
        this.patterns = patterns;
        nounPhraseProcessor = new NounPhraseProcessor(stopwords);
    }

    /**
     * 
     * @return 
     */
    public List<Pattern> Hearst_patterns() {
        List<Pattern> Hearst_patterns = new ArrayList<Pattern>();
        //pattern 1: food such as Supervised Machine learning techniques, spagitti, Biscuit or cucumber.
        Hearst_patterns.add(new Pattern(np + "/such/ /as/" + np + "(/,/ " + np + ")* (/,/)? ((and |or ) " + np + ")?", "first", 1));
        //pattern 2: such next food1 as cake, spagitti and cucumber..
        Hearst_patterns.add(new Pattern("/such/" + np + "/as/" + np + "(/,/ " + np + ")* (/,/)? ((and |or ) " + np + ")", "first", 1));
        //pattern 3: cucumber, cake, spagitti and other food4.
        Hearst_patterns.add(new Pattern(np + "(/,/ " + np + ")* (/,/)? ((and |or ) other" + np + ")", "last", 1));
        //pattern 4: food8 including cake, the spagitti or cucumber.
        Hearst_patterns.add(new Pattern(np + " (/,/)? /including/ " + np + "(/,/ " + np + ")* (/,/)? ((and |or (other)?)" + np + ")", "first", 1));
        //pattern 45: food9 especially cake, spagitti or cucumber..
        Hearst_patterns.add(new Pattern(np + " (/,/)? /especially/ (/,/)?" + np + "(/,/ " + np + ")* (/,/)? ((and |or (other)?)" + np + ")", "first", 1));
        return Hearst_patterns;
        //applications such as surveying, np_robotics and np_stereo_vision 
    }

    /**
     * 
     * @param sentence
     * @param pattern
     * @return 
     */
    List<Hyponyms> getSentenceHyponyms(String sentence, Pattern pattern) {
        List<Hyponyms> hyponyms = new ArrayList<>();
        sentence = sentence.replace(",", "").replace(".", "");
        List<String> terms = new ArrayList<>(Arrays.asList(sentence.split(" ")));

        List<String> filteredTerms = new ArrayList<>();
        for (int j = 0; j < terms.size(); j++) {
            if (terms.get(j).startsWith("np_")) {
                filteredTerms.add(nounPhraseProcessor.removeStopwords(terms.get(j).substring(3), "_"));
            }
        }

        if (pattern.getOrder().equals("first")) {
            for (int i = 1; i < filteredTerms.size(); i++) {
                hyponyms.add(new Hyponyms(filteredTerms.get(0), filteredTerms.get(i), Arrays.asList(pattern.getId()), pattern.getConfidence()));
            }
        } else {
            for (int i = 0; i < filteredTerms.size() - 1; i++) {
                hyponyms.add(new Hyponyms(filteredTerms.get(filteredTerms.size() - 1), filteredTerms.get(i), Arrays.asList(pattern.getId()), pattern.getConfidence()));
            }
        }
        return hyponyms;
    }
}
