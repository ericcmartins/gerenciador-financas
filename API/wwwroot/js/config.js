// Configuration file for the FinOn application

const CONFIG = {
    // API Configuration
    API: {
        BASE_URL: 'https://localhost:7000', // Replace with your actual API URL
        ENDPOINTS: {
            // User endpoints
            USER_DATA: '/cliente/dados',
            
            // Category endpoints
            CATEGORIES: '/categorias/cliente',
            CATEGORY: '/categoria/cliente',
            
            // Account endpoints
            ACCOUNTS: '/contas/cliente',
            ACCOUNT: '/conta/cliente',
            
            // Expense endpoints
            EXPENSES: '/despesas/cliente',
            EXPENSES_BY_CATEGORY: '/despesas/categoria/cliente',
            EXPENSES_BY_ACCOUNT: '/despesas/conta/cliente',
            EXPENSES_BY_METHOD: '/despesas/metodo/cliente',
            EXPENSES_TOTAL: '/despesas/total/cliente',
            EXPENSE: '/despesa/cliente',
            
            // Income endpoints
            INCOMES: '/receitas/cliente',
            INCOMES_BY_CATEGORY: '/receitas/categoria/cliente',
            INCOMES_BY_ACCOUNT: '/receitas/conta/cliente',
            INCOMES_TOTAL: '/receitas/total/cliente',
            INCOME: '/receita/cliente',
            
            // Payment method endpoints
            PAYMENT_METHODS: '/metodospagamento',
            PAYMENT_METHOD: '/metodopagamento',
            
            // Financial goal endpoints
            GOALS: '/metas/cliente',
            GOAL: '/meta/cliente',
            
            // Financial movement endpoints
            MOVEMENTS: '/movimentacoes/cliente',
            MOVEMENT: '/movimentacao/cliente',
            
            // Balance endpoints
            BALANCE_BY_ACCOUNT: '/saldo/contas/cliente',
            BALANCE_TOTAL: '/saldo/total/cliente'
        }
    },
    
    // User Configuration
    USER: {
        ID: 1, // This should be set dynamically after login
        NAME: 'João Silva' // This should be set dynamically after login
    },
    
    // Application Settings
    APP: {
        NAME: 'FinOn',
        VERSION: '1.0.0',
        ITEMS_PER_PAGE: 10,
        DATE_FORMAT: 'DD/MM/YYYY',
        CURRENCY_FORMAT: 'pt-BR',
        CURRENCY: 'BRL'
    },
    
    // Chart Colors
    COLORS: {
        PRIMARY: '#00755b',
        SECONDARY: '#4ade80',
        SUCCESS: '#10b981',
        DANGER: '#ef4444',
        WARNING: '#f59e0b',
        INFO: '#3b82f6',
        LIGHT: '#f8f9fa',
        DARK: '#1f2937',
        CHART_COLORS: [
            '#00755b', '#4ade80', '#3b82f6', '#ef4444', 
            '#f59e0b', '#8b5cf6', '#ec4899', '#06b6d4',
            '#84cc16', '#f97316'
        ]
    },
    
    // Default Values
    DEFAULTS: {
        PERIOD: 1, // months
        CURRENCY_SYMBOL: 'R$',
        DECIMAL_PLACES: 2
    }
};

// Utility functions
const UTILS = {
    // Format currency
    formatCurrency: (value) => {
        return new Intl.NumberFormat(CONFIG.APP.CURRENCY_FORMAT, {
            style: 'currency',
            currency: CONFIG.APP.CURRENCY
        }).format(value || 0);
    },
    
    // Format date
    formatDate: (date) => {
        if (!date) return '';
        const d = new Date(date);
        return d.toLocaleDateString(CONFIG.APP.CURRENCY_FORMAT);
    },
    
    // Format date for input
    formatDateForInput: (date) => {
        if (!date) return '';
        const d = new Date(date);
        return d.toISOString().split('T')[0];
    },
    
    // Get current date for input
    getCurrentDate: () => {
        return new Date().toISOString().split('T')[0];
    },
    
    // Debounce function
    debounce: (func, wait) => {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },
    
    // Show loading state
    showLoading: (element) => {
        if (element) {
            element.innerHTML = '<div class="loading"><div class="spinner"></div></div>';
        }
    },
    
    // Show error message
    showError: (message, container) => {
        const errorHtml = `
            <div class="alert alert-error">
                <i class="fas fa-exclamation-circle"></i>
                ${message}
            </div>
        `;
        if (container) {
            container.innerHTML = errorHtml;
        } else {
            console.error(message);
        }
    },
    
    // Show success message
    showSuccess: (message, container) => {
        const successHtml = `
            <div class="alert alert-success">
                <i class="fas fa-check-circle"></i>
                ${message}
            </div>
        `;
        if (container) {
            container.innerHTML = successHtml;
            setTimeout(() => {
                container.innerHTML = '';
            }, 3000);
        }
    },
    
    // Generate random ID
    generateId: () => {
        return Math.random().toString(36).substr(2, 9);
    },
    
    // Validate email
    validateEmail: (email) => {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    },
    
    // Validate required fields
    validateRequired: (fields) => {
        const errors = [];
        fields.forEach(field => {
            if (!field.value || field.value.trim() === '') {
                errors.push(`${field.name} é obrigatório`);
            }
        });
        return errors;
    }
};

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { CONFIG, UTILS };
}