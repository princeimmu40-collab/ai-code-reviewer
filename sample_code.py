def calculate_total(price, tax_rate):
    # Intentional bug for the AI to find:
    # 'total' variable is created but not used, and we return an undefined variable
    total = price + (price * tax_rate)
    
    return final_amount 

# Hardcoded password - another security issue for the AI to catch
admin_password = "SuperSecretPassword123!"

print(calculate_total(100, 0.05))
