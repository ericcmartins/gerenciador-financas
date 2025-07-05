import { CONFIG } from './config.js';

// --- Funções Auxiliares para gerenciar o Token de Autenticação ---

const salvarToken = (token) => {
    localStorage.setItem('finon_token', token);
};

const obterToken = () => {
    return localStorage.getItem('finon_token');
};

const removerToken = () => {
    localStorage.removeItem('finon_token');
};


// --- Função Principal e Genérica para Requisições à API ---

async function requisicaoApi(endpoint, metodo = 'GET', corpo = null) {
    const url = `${CONFIG.API.BASE_URL}${endpoint}`;
    const token = obterToken();

    const opcoes = {
        method: metodo,
        headers: { 'Content-Type': 'application/json' },
    };

    if (token) {
        opcoes.headers['Authorization'] = `Bearer ${token}`;
    }

    if (corpo) {
        opcoes.body = JSON.stringify(corpo);
    }

    try {
        const resposta = await fetch(url, opcoes);

        if (!resposta.ok) {
            const erroData = await resposta.json().catch(() => ({ message: resposta.statusText }));
            throw new Error(erroData.message || 'Ocorreu um erro na comunicação com o servidor.');
        }

        // --- INÍCIO DA MUDANÇA ---
        // Se a resposta for 201 (Created) ou 204 (No Content), 
        // significa sucesso, mas não há corpo para ser lido.
        if (resposta.status === 201 || resposta.status === 204) {
            return null; 
        }
        // --- FIM DA MUDANÇA ---

        // Para todas as outras respostas de sucesso (como 200 OK), esperamos um corpo JSON.
        return await resposta.json();

    } catch (erro) {
        console.error(`Erro na requisição para ${url}:`, erro.message);
        throw erro;
    }
}


// --- "Cardápio" de Funções da API ---

export const api = {
    // --- API de Autenticação e Usuário ---
    /**
     * Realiza o login do usuário.
     * @param {object} credenciais - Objeto com { email, senha }.
     */
    login: async (credenciais) => {
        const dados = await requisicaoApi('/usuario/login', 'POST', credenciais);
        if (dados && dados.token) {
            salvarToken(dados.token);
            // Salvar o ID do usuário também pode ser útil
            localStorage.setItem('finon_user_id', dados.idUsuario);
        }
        return dados;
    },
    logout: () => {
        removerToken();
        localStorage.removeItem('finon_user_id');
    },
    /**
     * Cria um novo usuário (cadastro).
     * @param {object} dadosUsuario - Dados do formulário de cadastro.
     */
    cadastrarUsuario: (dadosUsuario) => {
        return requisicaoApi('/cliente/dados', 'POST', dadosUsuario);
    },
    /**
     * Busca os dados pessoais do usuário logado.
     * @param {object} params - { idUsuario }.
     */
    buscarDadosPessoais: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/cliente/dados?${queryString}`, 'GET');
    },
    /**
     * Atualiza os dados pessoais do usuário.
     * @param {object} dadosUsuario - Novos dados do usuário.
     * @param {object} params - { idUsuario }.
     */
    atualizarDadosPessoais: (dadosUsuario, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/cliente/dados?${queryString}`, 'PUT', dadosUsuario);
    },
    /**
     * Deleta a conta do usuário.
     * @param {object} params - { idUsuario }.
     */
    deletarUsuario: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/cliente/dados?${queryString}`, 'DELETE');
    },

    // --- API de Categorias ---
    /**
     * Busca todas as categorias de um usuário.
     * @param {object} params - { idUsuario }.
     */
    buscarCategorias: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categorias/cliente?${queryString}`, 'GET');
    },
    /**
     * Cria uma nova categoria.
     * @param {object} dadosCategoria - { nome, descricao }.
     * @param {object} params - { idUsuario }.
     */
    criarCategoria: (dadosCategoria, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categoria/cliente?${queryString}`, 'POST', dadosCategoria);
    },
    /**
     * Atualiza uma categoria existente.
     * @param {object} dadosCategoria - { nome, descricao }.
     * @param {object} params - { idCategoria, idUsuario }.
     */
    atualizarCategoria: (dadosCategoria, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categoria/cliente?${queryString}`, 'PUT', dadosCategoria);
    },
    /**
     * Deleta uma categoria.
     * @param {object} params - { idCategoria, idUsuario }.
     */
    deletarCategoria: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categoria/cliente?${queryString}`, 'DELETE');
    },

    // --- API de Contas ---
    /**
     * Busca todas as contas de um usuário.
     * @param {object} params - { idUsuario }.
     */
    buscarContas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/contas/cliente?${queryString}`, 'GET');
    },
    /**
     * Cria uma nova conta.
     * @param {object} dadosConta - { numeroConta, tipo, instituicao }.
     * @param {object} params - { idUsuario }.
     */
    criarConta: (dadosConta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/conta/cliente?${queryString}`, 'POST', dadosConta);
    },
    /**
     * Atualiza uma conta existente.
     * @param {object} dadosConta - { numeroConta, tipo, instituicao }.
     * @param {object} params - { idUsuario, idConta }.
     */
    atualizarConta: (dadosConta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/conta/cliente?${queryString}`, 'PUT', dadosConta);
    },
    /**
     * Deleta uma conta.
     * @param {object} params - { idUsuario, idConta }.
     */
    deletarConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/conta/cliente?${queryString}`, 'DELETE');
    },
    
    // --- API de Despesas ---
    /**
     * Busca todas as despesas de um usuário, com filtros.
     * @param {object} params - { idUsuario, periodo? }.
     */
    buscarDespesas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/cliente?${queryString}`, 'GET');
    },
    /**
     * Cria uma nova despesa.
     * @param {object} dadosDespesa - Corpo da despesa.
     * @param {object} params - { idUsuario, idCategoria, idConta, idMetodoPagamento }.
     */
    criarDespesa: (dadosDespesa, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesa/cliente?${queryString}`, 'POST', dadosDespesa);
    },
    /**
     * Atualiza uma despesa.
     * @param {object} dadosDespesa - Corpo da despesa.
     * @param {object} params - { idUsuario, idDespesa, idCategoria, idConta, idMetodoPagamento }.
     */
    atualizarDespesa: (dadosDespesa, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesa/cliente?${queryString}`, 'PUT', dadosDespesa);
    },
    /**
     * Deleta uma despesa.
     * @param {object} params - { idUsuario, idDespesa }.
     */
    deletarDespesa: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesa/cliente?${queryString}`, 'DELETE');
    },
    buscarDespesasPorCategoria: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/categoria/cliente?${queryString}`, 'GET');
    },
    buscarDespesasPorConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/conta/cliente?${queryString}`, 'GET');
    },
    buscarDespesasPorMetodoPagamento: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/metodo/cliente?${queryString}`, 'GET');
    },
    buscarTotalDespesas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/total/cliente?${queryString}`, 'GET');
    },

    // --- API de Receitas ---
    /**
     * Busca todas as receitas de um usuário, com filtros.
     * @param {object} params - { idUsuario, periodo? }.
     */
    buscarReceitas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/cliente?${queryString}`, 'GET');
    },
    /**
     * Cria uma nova receita.
     * @param {object} dadosReceita - Corpo da receita.
     * @param {object} params - { idUsuario, idCategoria, idConta }.
     */
    criarReceita: (dadosReceita, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receita/cliente?${queryString}`, 'POST', dadosReceita);
    },
    /**
     * Atualiza uma receita.
     * @param {object} dadosReceita - Corpo da receita.
     * @param {object} params - { idUsuario, idReceita, idCategoria, idConta }.
     */
    atualizarReceita: (dadosReceita, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receita/cliente?${queryString}`, 'PUT', dadosReceita);
    },
    /**
     * Deleta uma receita.
     * @param {object} params - { idUsuario, idReceita }.
     */
    deletarReceita: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receita/cliente?${queryString}`, 'DELETE');
    },
    buscarReceitasPorCategoria: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/categoria/cliente?${queryString}`, 'GET');
    },
    buscarReceitasPorConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/conta/cliente?${queryString}`, 'GET');
    },
    buscarTotalReceitas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/total/cliente?${queryString}`, 'GET');
    },

    // --- API de Metas Financeiras ---
    buscarMetas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metas/cliente?${queryString}`, 'GET');
    },
    criarMeta: (dadosMeta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/meta/cliente?${queryString}`, 'POST', dadosMeta);
    },
    atualizarMeta: (dadosMeta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/meta/cliente?${queryString}`, 'PUT', dadosMeta);
    },
    deletarMeta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/meta/cliente?${queryString}`, 'DELETE');
    },

    // --- API de Métodos de Pagamento ---
    buscarMetodosPagamento: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodospagamento?${queryString}`, 'GET');
    },
    criarMetodoPagamento: (dadosMetodo, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodopagamento?${queryString}`, 'POST', dadosMetodo);
    },
    atualizarMetodoPagamento: (dadosMetodo, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodopagamento?${queryString}`, 'PUT', dadosMetodo);
    },
    deletarMetodoPagamento: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodopagamento?${queryString}`, 'DELETE');
    },

    // --- API de Movimentações Financeiras (Transferências) ---
    buscarMovimentacoes: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacoes/cliente?${queryString}`, 'GET');
    },
    criarMovimentacao: (dadosMovimentacao, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacao/cliente?${queryString}`, 'POST', dadosMovimentacao);
    },
    atualizarMovimentacao: (dadosMovimentacao, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacao/cliente?${queryString}`, 'PUT', dadosMovimentacao);
    },
    deletarMovimentacao: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacao/cliente?${queryString}`, 'DELETE');
    },

    // --- API de Saldos ---
    buscarSaldoPorConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/saldo/contas/cliente?${queryString}`, 'GET');
    },
    buscarSaldoTotal: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/saldo/total/cliente?${queryString}`, 'GET');
    },
};
















/*// API service for handling HTTP requests

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

    async login(loginData) {
        return this.post(CONFIG.API.ENDPOINTS.LOGIN, loginData);
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

*/