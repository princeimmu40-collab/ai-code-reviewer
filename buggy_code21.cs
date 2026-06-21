using System;

public class OrderProcessor {
    public void ProcessOrder(string? customerName) {
        // లోపం 1: customerName null అయితే ఇక్కడ NullReferenceException వస్తుంది (బాట్ దీన్ని పట్టుకోవాలి)
        int nameLength = customerName.Length; 
        Console.WriteLine($"Processing order for customer length: {nameLength}");

        // లోపం 2: హార్డ్ కోడెడ్ పాస్‌వర్డ్ లేదా సీక్రెట్ (Security Issue)
        string databaseToken = "SECRET_12345_TOKEN";
    }

    // లోపం 3: ఈ మెథడ్ ఎక్కడా వాడలేదు (Unused Method)
    private void UnusedHelperMethod() {
        Console.WriteLine("This is never called.");
    }
}
