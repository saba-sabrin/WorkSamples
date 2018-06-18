/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package nlp.semantic.relations.nontaxonomic;

import java.util.Objects;

/**
 *
 * @author Saba Sabrin
 */
public class NonTaxonomicTriple {
    
    private String PrimaryElement;
    
    private String SubElement;
    
    private String Relation;
    
    private double Confidence;
    
    public NonTaxonomicTriple(String PrimaryElement, String SubElement, String Relation) {
        this.PrimaryElement = PrimaryElement;
        this.SubElement = SubElement;
        this.Relation = Relation;
        this.Confidence = 1.0;
    }

    public NonTaxonomicTriple(String PrimaryElement, String SubElement, String Relation, double Confidence) {
        this.PrimaryElement = PrimaryElement;
        this.SubElement = SubElement;
        this.Relation = Relation;
        this.Confidence = Confidence;
    }

    public double getConfidence() {
        return Confidence;
    }

    public void setConfidence(double Confidence) {
        this.Confidence = Confidence;
    }   

    public String getPrimaryElement() {
        return PrimaryElement;
    }

    public String getSubElement() {
        return SubElement;
    }

    public String getRelation() {
        return Relation;
    }

    public void setPrimaryElement(String PrimaryElement) {
        this.PrimaryElement = PrimaryElement;
    }

    public void setSubElement(String SubElement) {
        this.SubElement = SubElement;
    }

    public void setRelation(String Relation) {
        this.Relation = Relation;
    }

    @Override
    public String toString() {
        return "RelationTriple{" + "Primary Element = " + PrimaryElement + ", Related Element = " + SubElement + ", Relation = " + Relation + '}';
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 23 * hash + Objects.hashCode(this.PrimaryElement);
        hash = 23 * hash + Objects.hashCode(this.SubElement);
        return hash;
    }

    @Override
    public boolean equals(Object obj) {
        if (this == obj) {
            return true;
        }
        if (obj == null) {
            return false;
        }
        if (getClass() != obj.getClass()) {
            return false;
        }
        final NonTaxonomicTriple other = (NonTaxonomicTriple) obj;
        if (!Objects.equals(this.PrimaryElement, other.PrimaryElement)) {
            return false;
        }
        return Objects.equals(this.SubElement, other.SubElement);
    }
}
