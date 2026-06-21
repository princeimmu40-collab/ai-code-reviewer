import os

def calculate_average(numbers):
    # Bug 1: This will crash with a ZeroDivisionError if the list is empty
    total_sum = sum(numbers)
    count = len(numbers)
    
    # Bug 2: 'average_value' is assigned but the function returns 'result' (undefined variable)
    average_value = total_sum / count
    return result

def get_user_data():
    # Bug 3: Security flaw - hardcoded API token exposed in source code
    api_token = "ghp_FakeTokenXYZ1234567890SecretKeyHere"
    
    # Bug 4: Resource leak - file is opened but never closed
    log_file = open("access_log.txt", "w")
    log_file.write("Fetching user data...")
    
    return {"status": "success"}

# Bug 5: Testing with an empty list to trigger the division crash
print(calculate_average([]))
