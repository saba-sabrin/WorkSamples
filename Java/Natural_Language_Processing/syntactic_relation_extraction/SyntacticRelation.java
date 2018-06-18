/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package nlp.semantic.relations.nontaxonomic;

import edu.stanford.nlp.ling.CoreAnnotations.SentencesAnnotation;
import edu.stanford.nlp.pipeline.Annotation;
import edu.stanford.nlp.pipeline.StanfordCoreNLP;
import edu.stanford.nlp.semgraph.SemanticGraph;
import edu.stanford.nlp.semgraph.SemanticGraphCoreAnnotations.CollapsedCCProcessedDependenciesAnnotation;
import edu.stanford.nlp.util.CoreMap;

import java.time.Duration;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import nlp.semantic.relations.NounPhraseProcessor;
/* For representing a sentence that is annotated with pos tags and np chunks.*/
import edu.washington.cs.knowitall.nlp.ChunkedSentence;

/* String -> ChunkedSentence */
import edu.washington.cs.knowitall.nlp.OpenNlpSentenceChunker;

/* The class that is responsible for extraction. */
import edu.washington.cs.knowitall.extractor.ReVerbExtractor;

/* The class that is responsible for assigning a confidence score to an
 * extraction.
 */
import edu.washington.cs.knowitall.extractor.conf.ConfidenceFunction;
import edu.washington.cs.knowitall.extractor.conf.ReVerbOpenNlpConfFunction;

/* A class for holding a (arg1, rel, arg2) triple. */
import edu.washington.cs.knowitall.nlp.extraction.ChunkedBinaryExtraction;
import java.util.Properties;

/**
 * extracting subject verb object
 * http://nlp.stanford.edu/software/dependencies_manual.pdf using
 * https://github.com/knowitall/reverb
 *
 * @author Saba Sabrin
 */
public class SyntacticRelation {

    private NounPhraseProcessor nounPhraseProcessor;

    public SyntacticRelation(String stopwords) {
        nounPhraseProcessor = new NounPhraseProcessor(stopwords);
    }

    /**
     * 
     * @param document
     * @return
     * @throws Exception 
     */
    public List<NonTaxonomicTriple> extractSVO(String document) throws Exception {
        List<NonTaxonomicTriple> relationTriples = new ArrayList<>();

        String sentStr = document;

        // Looks on the classpath for the default model files.
        OpenNlpSentenceChunker chunker = new OpenNlpSentenceChunker();
        ChunkedSentence sent = chunker.chunkSentence(sentStr);

        // Prints out extractions from the sentence.
        ReVerbExtractor reverb = new ReVerbExtractor();
        ConfidenceFunction confFunc = new ReVerbOpenNlpConfFunction();
        for (ChunkedBinaryExtraction extr : reverb.extract(sent)) {
            double conf = confFunc.getConf(extr);
            NonTaxonomicTriple newRelation = new NonTaxonomicTriple(extr.getArgument1().getText(), extr.getArgument2().getText(), extr.getRelation().getText(), conf);
            relationTriples.add(newRelation);
        }

        return relationTriples;
    }

    /**
     * 
     * @param text
     * @return
     * @throws Exception 
     */
    public List<NonTaxonomicTriple> extractDependencyGraphBasedSVO(String text) throws Exception {

        List<NonTaxonomicTriple> relationTriples = new ArrayList<>();

        // creates a StanfordCoreNLP object, with POS tagging, lemmatization, NER, parsing, and coreference resolution
        Properties props = new Properties();
        props.put("annotators", "tokenize, ssplit, pos, lemma, ner, parse");
        StanfordCoreNLP pipeline = new StanfordCoreNLP(props);

        // create an empty Annotation just with the given text
        Annotation document = new Annotation(text);

        //System.out.println("annotation started!!");
        Instant t1 = Instant.now();

        // run all Annotators on this text
        pipeline.annotate(document);

        //System.out.println("annotation ended!!");

        // these are all the sentences in this document
        // a CoreMap is essentially a Map that uses class objects as keys and has values with custom types
        List<CoreMap> sentences = document.get(SentencesAnnotation.class);

        //System.out.println(sentences);

        sentences.stream().map((sentence) -> sentence.get(CollapsedCCProcessedDependenciesAnnotation.class)).forEach((SemanticGraph dependencies) -> {

            // this is the Stanford dependency graph of the current sentence
            //System.out.println(dependencies.typedDependencies());

            dependencies.typedDependencies().stream().forEach((dependency) -> {

                String dep = dependency.dep().word();
                String gov = dependency.gov().word();

                if (!(dep == null || gov == null)) {

                    if (dep.startsWith("np_") && gov.startsWith("np_")) {                       
                        gov = nounPhraseProcessor.removeStopwords(gov.substring(3), "_");
                        dep = nounPhraseProcessor.removeStopwords(dep.substring(3), "_");

                        // Relation Types
                        // getSpecific() = collapsed dependencies based on "with", "and, such_as, like" based relations
                        // getLongName() = collapsed dependencies with description
                        // getShortName() = basic ones..

                        // Filter relevant typed dependency relations
                        if(dependency.reln().getSpecific() != null) {

                            NonTaxonomicTriple newRelation = new NonTaxonomicTriple(dep, gov, dependency.reln().getSpecific(), 0.5);

                            if (!relationTriples.contains(newRelation)) {
                                relationTriples.add(newRelation);
                            }
                        }
                    }
                }
            });
        });

        Instant t2 = Instant.now();
        long ns = Duration.between(t1, t2).getSeconds();
        System.out.println("Non-Taxonomic terms extraction Duration: " + ns + " seconds");

        return relationTriples;
    }
}
