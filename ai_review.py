import os
import requests
import google.generativeai as genai

# Environment Variables లోడ్ చేయడం
gemini_key = os.getenv("GEMINI_API_KEY")
github_token = os.getenv("GITHUB_TOKEN")
pr_number = os.getenv("PR_NUMBER")
repo_name = os.getenv("REPO_NAME")

if not gemini_key or not github_token or not pr_number:
    print("Missing required environment variables.")
    exit(1)

# Gemini AI కాన్ఫిగరేషన్
genai.configure(api_key=gemini_key)
model = genai.GenerativeModel("gemini-1.5-flash")

# 1. GitHub PR నుండి మారిన కోడ్ (Diff) ని తీసుకురావడం
github_url = f"[https://api.github.com/repos/](https://api.github.com/repos/){repo_name}/pulls/{pr_number}"
headers = {
    "Authorization": f"token {github_token}",
    "Accept": "application/vnd.github.v3.diff"
}

response = requests.get(github_url, headers=headers)
if response.status_code != 200:
    print(f"Failed to fetch PR diff: {response.status_code}")
    exit(1)

pr_diff = response.text

# 2. Gemini AI ని కోడ్ రివ్యూ అడగడం
prompt = f"You are an expert code reviewer. Review the following GitHub Pull Request diff and provide feedback on bugs, security flaws, and performance improvements. Keep your response clean and professional:\n\n{pr_diff}"
ai_response = model.generate_content(prompt)
review_comment = ai_response.text

# 3. రివ్యూ కామెంట్‌ను GitHub PR పై పోస్ట్ చేయడం
comment_url = f"[https://api.github.com/repos/](https://api.github.com/repos/){repo_name}/issues/{pr_number}/comments"
comment_headers = {
    "Authorization": f"token {github_token}",
    "Accept": "application/vnd.github.v3+json"
}
comment_data = {"body": f"### 🤖 Gemini AI Code Review\n\n{review_comment}"}

post_response = requests.post(comment_url, headers=comment_headers, json=comment_data)
if post_response.status_code == 201:
    print("Review comment posted successfully!")
else:
    print(f"Failed to post comment: {post_response.status_code}, {post_response.text}")
