import os
import requests
# Correct import for the new SDK
from google import genai 

# Initialize the client as shown in the new documentation
# Ensure GEMINI_API_KEY is correctly set in your environment
gemini_key = os.getenv("GEMINI_API_KEY")
client = genai.Client(api_key=gemini_key)
