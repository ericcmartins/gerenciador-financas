// API service for handling HTTP requests

class ApiService {
    constructor() {
        this.baseURL = CONFIG.API.BASE_URL;
        this.userId = CONFIG.USER.ID;
    }

    // Generic request method
    async request(endpoint, options = {}) {
        const url = `${this.baseURL}${endpoint}`;
        
        const defaultOptions = {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        };

        const config = { ...defaultOptions, ...options };

        try {
            const response = await fetch(url, config);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Handle different response types
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return await response.json();
            } else {
                return await response.text();
            }
        } catch (error) {
            console.error('API Request failed:', error);
            throw error;
        }
    }

    // GET request
    async get(endpoint, params = {}) {
        const queryString = new URLSearchParams(params).toString();
        const url = queryString ? `${endpoint}?${queryString}` : endpoint;
        
        return this.request(url, {
            method: 'GET'
        });
    }

    // POST request
    async post(endpoint, data = {}, params = {}) {
        const queryString = new URLSearchParams(params).toString();
        const url = queryString ? `${endpoint}?${queryString}` : endpoint;
        
        return this.request(url, {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    // PUT request
    async put(endpoint, data = {}, params = {}) {
        const queryString = new URLSearchParams(params).toString();
        const url = queryString ? `${endpoint}?${queryString}` : endpoint;
        
        return this.request(url, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    // DELETE request
    async delete(endpoint, params = {}) {
        const queryString = new URLSearchParams(params).toString();
        const url = queryString ? `${endpoint}?${queryString}` : endpoint;
        
        return this.request(url, {
            method: 'DELETE'
        });
    }

    // User API methods
    async getUserData() {
        return this.get(CONFIG.API.ENDPOINTS.USER_DATA, { idUsuario: this.userId });
    }

    async createUser(userData) {
        return this.post(CONFIG.API.ENDPOINTS.USER_DATA, userData);
    }

    async updateUser(userData) {
        return this.put(CONFIG.API.ENDPOINTS.USER_DATA, userData, { idUsuario: this.userId });
    }

    async deleteUser() {
        return this.delete(CONFIG.API.ENDPOINTS.USER_DATA, { idUsuario: this.userId });
    }

    // Category API methods
    async getCategories() {
        return this.get(CONFIG.API.ENDPOINTS.CATEGORIES, { idUsuario: this.userId });
    }

    async createCategory(categoryData) {
        return this.post(CONFIG.API.ENDPOINTS.CATEGORY, categoryData, { idUsuario: this.userId });
    }

    async updateCategory(categoryId, categoryData) {
        return this.put(CONFIG.API.ENDPOINTS.CATEGORY, categoryData, { 
            idCategoria: categoryId, 
            idUsuario: this.userId 
        });
    }

    async deleteCategory(categoryId) {
        return this.delete(CONFIG.API.ENDPOINTS.CATEGORY, { 
            idCategoria: categoryId, 
            idUsuario: this.userId 
        });
    }

    // Account API methods
    async getAccounts() {
        return this.get(CONFIG.API.ENDPOINTS.ACCOUNTS, { idUsuario: this.userId });
    }

    async createAccount(accountData) {
        return this.post(CONFIG.API.ENDPOINTS.ACCOUNT, accountData, { idUsuario: this.userId });
    }

    async updateAccount(accountId, accountData) {
        return this.put(CONFIG.API.ENDPOINTS.ACCOUNT, accountData, { 
            idUsuario: this.userId, 
            idConta: accountId 
        });
    }

    async deleteAccount(accountId) {
        return this.delete(CONFIG.API.ENDPOINTS.ACCOUNT, { 
            idUsuario: this.userId, 
            idConta: accountId 
        });
    }

    // Balance API methods
    async getBalanceByAccount() {
        return this.get(CONFIG.API.ENDPOINTS.BALANCE_BY_ACCOUNT, { idUsuario: this.userId });
    }

    async getTotalBalance() {
        return this.get(CONFIG.API.ENDPOINTS.BALANCE_TOTAL, { idUsuario: this.userId });
    }

    // Expense API methods
    async getExpenses(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.EXPENSES, params);
    }

    async getExpensesByCategory(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.EXPENSES_BY_CATEGORY, params);
    }

    async getExpensesByAccount(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.EXPENSES_BY_ACCOUNT, params);
    }

    async getExpensesByMethod(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.EXPENSES_BY_METHOD, params);
    }

    async getTotalExpenses(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.EXPENSES_TOTAL, params);
    }

    async createExpense(expenseData, categoryId, accountId, paymentMethodId) {
        return this.post(CONFIG.API.ENDPOINTS.EXPENSE, expenseData, {
            idUsuario: this.userId,
            idCategoria: categoryId,
            idConta: accountId,
            idMetodoPagamento: paymentMethodId
        });
    }

    async updateExpense(expenseId, expenseData, categoryId, accountId, paymentMethodId) {
        return this.put(CONFIG.API.ENDPOINTS.EXPENSE, expenseData, {
            idUsuario: this.userId,
            idDespesa: expenseId,
            idCategoria: categoryId,
            idConta: accountId,
            idMetodoPagamento: paymentMethodId
        });
    }

    async deleteExpense(expenseId) {
        return this.delete(CONFIG.API.ENDPOINTS.EXPENSE, {
            idUsuario: this.userId,
            idDespesa: expenseId
        });
    }

    // Income API methods
    async getIncomes(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.INCOMES, params);
    }

    async getIncomesByCategory(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.INCOMES_BY_CATEGORY, params);
    }

    async getIncomesByAccount(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.INCOMES_BY_ACCOUNT, params);
    }

    async getTotalIncomes(period = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        return this.get(CONFIG.API.ENDPOINTS.INCOMES_TOTAL, params);
    }

    async createIncome(incomeData, categoryId, accountId) {
        return this.post(CONFIG.API.ENDPOINTS.INCOME, incomeData, {
            idUsuario: this.userId,
            idCategoria: categoryId,
            idConta: accountId
        });
    }

    async updateIncome(incomeId, incomeData, categoryId, accountId) {
        return this.put(CONFIG.API.ENDPOINTS.INCOME, incomeData, {
            idUsuario: this.userId,
            idReceita: incomeId,
            idCategoria: categoryId,
            idConta: accountId
        });
    }

    async deleteIncome(incomeId) {
        return this.delete(CONFIG.API.ENDPOINTS.INCOME, {
            idUsuario: this.userId,
            idReceita: incomeId
        });
    }

    // Payment Method API methods
    async getPaymentMethods() {
        return this.get(CONFIG.API.ENDPOINTS.PAYMENT_METHODS, { idUsuario: this.userId });
    }

    async createPaymentMethod(paymentMethodData, accountId) {
        return this.post(CONFIG.API.ENDPOINTS.PAYMENT_METHOD, paymentMethodData, {
            idUsuario: this.userId,
            idConta: accountId
        });
    }

    async updatePaymentMethod(paymentMethodId, paymentMethodData, accountId) {
        return this.put(CONFIG.API.ENDPOINTS.PAYMENT_METHOD, paymentMethodData, {
            idUsuario: this.userId,
            idConta: accountId,
            idMetodoPagamento: paymentMethodId
        });
    }

    async deletePaymentMethod(paymentMethodId) {
        return this.delete(CONFIG.API.ENDPOINTS.PAYMENT_METHOD, {
            idUsuario: this.userId,
            idMetodoPagamento: paymentMethodId
        });
    }

    // Financial Goal API methods
    async getGoals() {
        return this.get(CONFIG.API.ENDPOINTS.GOALS, { idUsuario: this.userId });
    }

    async createGoal(goalData) {
        return this.post(CONFIG.API.ENDPOINTS.GOAL, goalData, { idUsuario: this.userId });
    }

    async updateGoal(goalId, goalData) {
        return this.put(CONFIG.API.ENDPOINTS.GOAL, goalData, {
            idUsuario: this.userId,
            idMetaFinanceira: goalId
        });
    }

    async deleteGoal(goalId) {
        return this.delete(CONFIG.API.ENDPOINTS.GOAL, {
            idMetaFinanceira: goalId,
            idUsuario: this.userId
        });
    }

    // Financial Movement API methods
    async getMovements(period = null, type = null) {
        const params = { idUsuario: this.userId };
        if (period) params.periodo = period;
        if (type) params.tipoMovimentacao = type;
        return this.get(CONFIG.API.ENDPOINTS.MOVEMENTS, params);
    }

    async createMovement(movementData, originAccountId, destinationAccountId) {
        return this.post(CONFIG.API.ENDPOINTS.MOVEMENT, movementData, {
            idUsuario: this.userId,
            idContaOrigem: originAccountId,
            idContaDestino: destinationAccountId
        });
    }

    async updateMovement(movementId, movementData, originAccountId = null, destinationAccountId = null) {
        const params = {
            idUsuario: this.userId,
            idMovimentacaoFinanceira: movementId
        };
        if (originAccountId) params.idContaOrigem = originAccountId;
        if (destinationAccountId) params.idContaDestino = destinationAccountId;
        
        return this.put(CONFIG.API.ENDPOINTS.MOVEMENT, movementData, params);
    }

    async deleteMovement(movementId) {
        return this.delete(CONFIG.API.ENDPOINTS.MOVEMENT, {
            idUsuario: this.userId,
            idMovimentacaoFinanceira: movementId
        });
    }
}

// Create global API instance
const api = new ApiService();

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ApiService;
}