package FeatureSelection;

/**
 * Created by Saba Sabrin on 6/12/2017.
 */

import com.sun.org.apache.bcel.internal.generic.NEW;
import javafx.util.Pair;
import weka.attributeSelection.*;
import weka.attributeSelection.ASEvaluation.*;
import weka.classifiers.Evaluation;
import weka.classifiers.meta.AttributeSelectedClassifier;
import weka.classifiers.trees.J48;
import weka.core.*;
import weka.core.converters.ConverterUtils.DataSource;
import weka.filters.Filter;

import java.io.*;
import java.time.Duration;
import java.time.Instant;
import java.util.*;

public class FeatureProcessor {

    public static List<String> allFeatures = new ArrayList<>();
    public static Map<FeatureSelectionType, Map<String, Double>> _allFeatureSet = new HashMap();

    //static String featuresPath = "E:\\Saba\\Project\\NEW\\WekaFeatureSelection\\resources\\";
    static String featuresPath = "E:\\Saba\\Results\\Subject_Matter_Codes\\Label_Features_By_DF\\";
    static List<String> stopwords = new ArrayList<>();

    public static void main(String[] args) throws Exception {
        Instant t1 = Instant.now();

        // Load the stopwords
        stopwords = loadStopwords("resources/stopwords.txt");

        PrepareLabelFeatures();

        Instant t2 = Instant.now();
        long ns = Duration.between(t1, t2).getSeconds();
        long mins = ns / 60;
        System.out.println("Execution Duration: " + mins + " minutes");
    }

    private static void PrepareLabelFeatures() throws Exception{
        // load data
        System.out.println("\n0. Loading data");

        // get files
        File directory = new File(featuresPath);
        File[] allFiles = directory.listFiles();

        for (File featureFile : allFiles) {
            if (featureFile.isFile()) {
                System.out.println("Processing file :" + featureFile.getCanonicalPath());
                String fileName = featureFile.getName().substring(0, featureFile.getName().length() - 5);

                DataSource source = new DataSource(featureFile.getCanonicalPath());
                Instances data = source.getDataSet();

                if (data.classIndex() == -1)
                    data.setClassIndex(data.numAttributes()-1);

                //String feature = GetFeatureName(data.attribute(5).toString());

                // 1. Feature Selection using Correlation (Pearson's technique)
                System.out.println("Feature selection process started for Correlation analysis. . .");
                FSbyCorrelation(data);

                // 2. Feature Selection using Information Gain
                //double[][] FS_Result_IG = FSbyInfoGain(data);

                // 3. Feature Selection using Gain Ratio
                System.out.println("Feature selection process started for Gain Ratio. . .");
                FSbyGainRatio(data);

                // Get the features combined
                List<String> _FeaturesAll = CombineAllFeatures();

                // Write the feature data
                WriteData(fileName, _FeaturesAll);

                System.out.println("Finished processing!");
            }
        }
    }

    private static String GetFeatureName(String FeatureName){
        String result = "";
        String[] attr = FeatureName.split(" ");

        for(int i = 1; i < attr.length-1; i++){
            String finalVal = attr[i].replace("'","");

            if(attr.length > 3){
                result = result + finalVal + " ";
            }
            else
            {
                result = result + finalVal;
            }
        }

        return result.trim();
    }

    private static void FSbyCorrelation(Instances data) throws Exception {

        Map<String, Double> Features = new HashMap<>();
        double minCorThreshold = 0.2;

        AttributeSelection attsel = new AttributeSelection();
        CorrelationAttributeEval evalCor = new CorrelationAttributeEval();
        Ranker search = new Ranker();

        attsel.setEvaluator(evalCor);
        attsel.setSearch(search);
        attsel.SelectAttributes(data);
        attsel.setRanking(true);
        //int[] indices = attsel.selectedAttributes();

        double[][] rankedAttr = attsel.rankedAttributes();

        // Result data have two values
        // attribute index, attrEvaluator value
        // Iterate through the resulting attribute set
        // filter all correlation coefficient with threshold value equal or larger than 0.2
        for(int j = 0; j < rankedAttr.length; j++){
            double[] attrValues = rankedAttr[j];
            Number objNumber = attrValues[0];
            Integer attrIndex = objNumber.intValue();

            if(attrValues[1] >= minCorThreshold){
                String feature = GetFeatureName(data.attribute(attrIndex).toString());

                // clean the extracted feature
                String cleanFeature = removeStopwords(feature);

                if(!cleanFeature.isEmpty()) {
                    Features.put(cleanFeature, attrValues[1]);
                }
            }
        }

        _allFeatureSet.put(FeatureSelectionType.Correlation, Features);
    }

    private static double[][] FSbyInfoGain(Instances data) throws Exception{

        AttributeSelection attsel = new AttributeSelection();
        InfoGainAttributeEval evalIG = new InfoGainAttributeEval();
        Ranker search = new Ranker();

        attsel.setEvaluator(evalIG);
        attsel.setSearch(search);
        attsel.SelectAttributes(data);
        attsel.setRanking(true);
        //int[] indices = attsel.selectedAttributes();

        double[][] rankedAttr = attsel.rankedAttributes();

        return rankedAttr;
    }

    private static void FSbyGainRatio(Instances data) throws Exception{

        Map<String, Double> Features = new HashMap<>();

        AttributeSelection attsel = new AttributeSelection();
        GainRatioAttributeEval evalGR = new GainRatioAttributeEval();
        Ranker search = new Ranker();

        attsel.setEvaluator(evalGR);
        attsel.setSearch(search);
        attsel.SelectAttributes(data);
        attsel.setRanking(true);
        //int[] indices = attsel.selectedAttributes();

        double[][] rankedAttr = attsel.rankedAttributes();

        int topSetSize = 300;
        int counter = 0;
        double threshold = 0.15;

        for(int j = 0; j < rankedAttr.length; j++){
            double[] attrValues = rankedAttr[j];
            Number objNumber = attrValues[0];
            Integer attrIndex = objNumber.intValue();

            if(counter < topSetSize) {
                if (attrValues[1] > threshold) {
                    String feature = GetFeatureName(data.attribute(attrIndex).toString());

                    // clean the extracted feature
                    String cleanFeature = removeStopwords(feature);

                    if(!cleanFeature.isEmpty()) {
                        Features.put(cleanFeature, attrValues[1]);
                    }

                    counter++;
                }
            }
        }

        _allFeatureSet.put(FeatureSelectionType.Gain_Ratio, Features);
    }

    private static List<String> CombineAllFeatures(){

        // extract the feature names the common feature set
        TreeSet<String> _features = new TreeSet<>();
        for(Map<String, Double> objFeatures : _allFeatureSet.values()) {
            Set<String> _allKeys = objFeatures.keySet();
            for(String objFeature : _allKeys){
                _features.add(objFeature);
            }
        }

        List<String> _FeaturesAll = new ArrayList<>();
        // iterate through the features
        for(String feature : _features){

            String text = feature;

            // iterate through different feature selection technique & get the values
            for (FeatureSelectionType featureType : FeatureSelectionType.values()) {
                Map<String, Double> tempFeatures = _allFeatureSet.get(featureType);

                // check if this feature set contains this feature
                if (tempFeatures != null) {
                    if (tempFeatures.containsKey(feature)) {
                        text = text + "," + featureType.toString() + "=" + tempFeatures.get(feature);
                    }
                }
            }

            _FeaturesAll.add(text);
        }

        return _FeaturesAll;
    }

    private static void WriteData(String fileName, List<String> Features) throws IOException {
        try (Writer writer = new BufferedWriter(new OutputStreamWriter(
                new FileOutputStream(featuresPath + fileName + ".txt"), "UTF-8"))) {
            for(String data : Features){
                writer.write(data + "\n");
            }

            writer.close();
        }
    }

    public static String removeStopwords(String source) {

        List<String> multiWordList = new ArrayList<String>(Arrays.asList(source.split(" ")));
        String np = "";
        for (int j = 0; j < multiWordList.size(); j++) {
            if (stopwords.contains(multiWordList.get(j))) {
                continue;
            }
            np += multiWordList.get(j) + " ";
        }

        return np.trim();
    }

    /**
     *
     * @param stopwordsFile
     * @return
     * @throws IOException
     */
    private static List<String> loadStopwords(String stopwordsFile) throws IOException {
        List<String> words = new ArrayList<String>();

        BufferedReader br = new BufferedReader(new FileReader(stopwordsFile));
        String line;
        while ((line = br.readLine()) != null) {
            words.add(line);
        }
        br.close();
        return words;
    }
}
