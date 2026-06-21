using System;

public class Calculator {
    public int DivideNumbers(int firstNum, int secondNum) {
        // లోపం 1: secondNum సున్నా (0) అయితే ఇక్కడ రన్‌టైమ్ ఎర్రర్ వస్తుంది. బాట్ దీన్ని అలర్ట్ చేయాలి.
        return firstNum / secondNum; 
    }

    public void UnusedMethod() {
        // లోపం 2: ఈ వేరియబుల్ ఎక్కడా వాడలేదు (Unused Variable)
        int temporaryValue = 100; 
        
        // లోపం 3: సీక్రెట్ కీని కోడ్‌లో ఓపెన్‌గా పెట్టడం (Security Issue)
        string mySecretApiKey = "AIzaSy_FakeGeminiKey123";
    }
}
