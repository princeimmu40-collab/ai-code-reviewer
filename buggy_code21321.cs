using System;
using System.IO;

public class DataExporter {
    public void ExportUserData(string filePath, string rawData) {
        // లోపం 1: StreamWriter ని సరిగ్గా క్లోజ్ చేయలేదు / Dispose చేయలేదు (Resource Leak)
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine(rawData);

        // లోపం 2: ఎక్సెప్షన్ పట్టుకున్నారు కానీ లోపల ఏం చేయలేదు (Empty Catch Block - Bad Practice)
        try {
            int parsedData = int.Parse(rawData);
            Console.WriteLine($"Parsed Number: {parsedData}");
        }
        catch (Exception ex) {
            // బాట్ దీన్ని పట్టుకోవాలి: ఎర్రర్‌ని లాగ్ చేయకుండా ఖాళీగా వదిలేశారు
        }
    }

    // లోపం 3: ఈ పబ్లిక్ మెథడ్ ఎక్కడా వాడలేదు మరియు ఇందులో హార్డ్ కోడెడ్ యుటిలిటీ ఉంది
    public void ObsoleteCalculation() {
        int a = 10;
        int b = 0;
        int result = a / b; // సున్నాValue తో డివైడ్ చేయడం వల్ల DivideByZeroException వస్తుంది!
        Console.WriteLine(result);
    }
}
