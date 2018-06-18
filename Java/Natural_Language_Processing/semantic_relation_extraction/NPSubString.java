package nlp.semantic.relations.taxonomic;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by Saba on 29.03.2017.
 */
public class NPSubString {
    private String concept;

    private String subconcept;

    private int occurances;

    public NPSubString(String concept, String subconcept){
        this.concept = concept;
        this.subconcept = subconcept;
        this.occurances = 1;
    }

    public void setConcept(String concept) {
        this.concept = concept;
    }

    public int getOccurances() {
        return occurances;
    }

    public void setSubconcept(String specific) {
        this.subconcept = subconcept;
    }

    public String getConcept() {
        return concept;
    }

    public String getSubconcept() {
        return subconcept;
    }

    public void IncOccurance(int noccurancces) {
        this.occurances += noccurancces;
    }

    @Override
    public String toString() {
        return subconcept + " is a type of " + concept + ", occurances: " + occurances;
    }
}
