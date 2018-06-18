package nlp.semantic.relations.taxonomic;

import edu.stanford.nlp.ling.CoreAnnotations;
import edu.stanford.nlp.ling.CoreLabel;
import edu.stanford.nlp.pipeline.Annotation;
import edu.stanford.nlp.util.CoreMap;
import nlp.semantic.relations.NounPhraseProcessor;
import java.lang.*;

import java.util.*;

/**
 * Created by Saba on 29.03.2017.
 */
public class ConceptHierarchy {
    private NounPhraseProcessor nounPhraseProcessor;

    public ConceptHierarchy(String stopwords){
        nounPhraseProcessor = new NounPhraseProcessor(stopwords);
    }

    public List<NPSubString> GetConcepts(Annotation document)
    {
        List<NPSubString> objConcepts = new ArrayList<>();
        List<CoreLabel> tokens = new ArrayList<CoreLabel>();
        List<CoreMap> sentences = document.get(CoreAnnotations.SentencesAnnotation.class);

        List<String> terms_withoutNP = new ArrayList<>();

        // Traverse each sentence with np's
        for (CoreMap sentence : sentences) {

            for (CoreLabel token : sentence.get(CoreAnnotations.TokensAnnotation.class)) {
                tokens.add(token);
            }

            for (CoreLabel objToken : tokens)
            {
                //System.out.print(objToken);

                List<String> wordsList = new ArrayList<>(Arrays.asList(objToken.toString().split("-")));

                //System.out.println(wordsList);

                for(int i = 0; i < wordsList.size(); i++) {

                    //System.out.println(wordsList.get(i));

                    if(wordsList.get(i).startsWith("np_"))
                    {
                        //System.out.println(wordsList.get(i));

                        terms_withoutNP.add(nounPhraseProcessor.removeStopwords(wordsList.get(i).substring(3), "_"));
                    }
                }
            }

            tokens = new ArrayList<CoreLabel>();
        }

        //System.out.println("Found Terms: " + terms_withoutNP + " and No. of terms: " + terms_withoutNP.size());

        objConcepts = GetSentenceConcepts(terms_withoutNP);

        return objConcepts;
    }

    private List<NPSubString> GetSentenceConcepts(List<String> terms)
    {
        List<NPSubString> concepts = new ArrayList<>();

        for(int i = 0; i < terms.size(); i++) {

            List<String> subConcepts = new ArrayList<>(Arrays.asList(terms.get(i).split("_")));

            // Getting sub-concept from the NP
            if(subConcepts.size() > 1)
            {
                // Find the concept relations in an ordered way
                List<String> conceptHierarchy = FindConceptHierarchy(subConcepts);
                NPSubString objCategory = null;

                for(int ind = 1; ind <= conceptHierarchy.size(); ind++)
                {
                    if(ind == 1)
                    {
                        objCategory = new NPSubString(conceptHierarchy.get(conceptHierarchy.size() - ind), terms.get(i));
                    }
                    else
                    {
                        objCategory = new NPSubString(conceptHierarchy.get(conceptHierarchy.size() - ind), conceptHierarchy.get(conceptHierarchy.size() - (ind - 1)));
                    }

                    if(!concepts.contains(objCategory))
                    {
                        concepts.add(objCategory);
                    }
                }
            }
        }

        return concepts;
    }

    private List<String> FindConceptHierarchy(List<String> subConcepts)
    {
        boolean IsIterative = false;
        List<String> Concepts = new ArrayList<>();

        for(int j = 1; j < subConcepts.size(); j++ )
        {
            if(IsIterative) {
                Concepts.add(subConcepts.get(subConcepts.size() - j) + "_" + Concepts.get(j - 2));
            }
            else {
                Concepts.add(subConcepts.get(subConcepts.size() - j));
            }

            IsIterative = true;
        }

        return Concepts;
    }
}
