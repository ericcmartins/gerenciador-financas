// Dashboard specific functionality

let dashboardCharts = {};
let dashboardData = {
    receitas: 0,
    despesas: 0,
    saldo: 0,
    metas: 0
};

// Initialize dashboard
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname.includes('index.html') || window.location.pathname === '/') {
        initializeDashboard();
    }
});

async function initializeDashboard() {
    try {
        // Load dashboard data
        await loadDashboardData();
        
        // Update summary cards
        updateSummaryCards();
        
        // Initialize charts
        initializeCharts();
        
        // Load recent transactions
        loadRecentTransactions();
        
        // Setup dashboard event listeners
        setupDashboardEventListeners();
        
    } catch (error) {
        console.error('Error initializing dashboard:', error);
        showErrorMessage('Erro ao carregar dados do dashboard');
    }
}

async function loadDashboardData() {
    try {
        const period = document.getElementById('periodoSelect')?.value || 1;
        
        // Load summary data
        const [receitas, despesas, metas] = await Promise.all([
            api.getTotalIncomes(period).catch(() => []),
            api.getTotalExpenses(period).catch(() => []),
            api.getGoals().catch(() => [])
        ]);
        
        // Calculate totals
        dashboardData.receitas = Array.isArray(receitas) ? 
            receitas.reduce((sum, item) => sum + (item.totalReceita || 0), 0) : 0;
        
        dashboardData.despesas = Array.isArray(despesas) ? 
            despesas.reduce((sum, item) => sum + (item.totalDespesa || 0), 0) : 0;
        
        dashboardData.saldo = dashboardData.receitas - dashboardData.despesas;
        dashboardData.metas = Array.isArray(metas) ? metas.filter(meta => !meta.conclu ida).length : 0;
        
    } catch (error) {
        console.error('Error loading dashboard data:', error);
        // Set default values if API fails
        dashboardData = { receitas: 0, despesas: 0, saldo: 0, metas: 0 };
    }
}

function updateSummaryCards() {
    // Update total receitas
    const totalReceitasElement = document.getElementById('totalReceitas');
    if (totalReceitasElement) {
        totalReceitasElement.textContent = formatCurrency(dashboardData.receitas);
    }
    
    // Update total despesas
    const totalDespesasElement = document.getElementById('totalDespesas');
    if (totalDespesasElement) {
        totalDespesasElement.textContent = formatCurrency(dashboardData.despesas);
    }
    
    // Update saldo atual
    const saldoAtualElement = document.getElementById('saldoAtual');
    if (saldoAtualElement) {
        saldoAtualElement.textContent = formatCurrency(dashboardData.saldo);
        
        // Update color based on balance
        const card = saldoAtualElement.closest('.summary-card');
        if (dashboardData.saldo >= 0) {
            card.classList.remove('negative-balance');
            card.classList.add('positive-balance');
        } else {
            card.classList.remove('positive-balance');
            card.classList.add('negative-balance');
        }
    }
    
    // Update metas ativas
    const metasAtivasElement = document.getElementById('metasAtivas');
    if (metasAtivasElement) {
        metasAtivasElement.textContent = dashboardData.metas;
    }
}

async function initializeCharts() {
    try {
        await initializeExpensesChart();
        await initializeEvolutionChart();
    } catch (error) {
        console.error('Error initializing charts:', error);
    }
}

async function initializeExpensesChart() {
    const canvas = document.getElementById('despesasChart');
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    
    try {
        const period = document.getElementById('periodoSelect')?.value || 1;
        const expensesByCategory = await api.getExpensesByCategory(period);
        
        const labels = expensesByCategory.map(item => item.categoria || 'Sem categoria');
        const data = expensesByCategory.map(item => item.totalDespesa || 0);
        const colors = CONFIG.COLORS.CHART_COLORS.slice(0, labels.length);
        
        // Destroy existing chart if it exists
        if (dashboardCharts.expenses) {
            dashboardCharts.expenses.destroy();
        }
        
        dashboardCharts.expenses = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: colors,
                    borderWidth: 2,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const value = formatCurrency(context.raw);
                                return `${context.label}: ${value}`;
                            }
                        }
                    }
                }
            }
        });
        
    } catch (error) {
        console.error('Error loading expenses chart:', error);
        showEmptyChart(ctx, 'Nenhum dado de despesas encontrado');
    }
}

async function initializeEvolutionChart() {
    const canvas = document.getElementById('evolucaoChart');
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    
    try {
        // Generate mock data for the last 6 months
        const months = [];
        const receitasData = [];
        const despesasData = [];
        
        for (let i = 5; i >= 0; i--) {
            const date = new Date();
            date.setMonth(date.getMonth() - i);
            months.push(date.toLocaleDateString('pt-BR', { month: 'short' }));
            
            // Mock data - in a real app, you'd fetch this from the API
            receitasData.push(Math.random() * 5000 + 3000);
            despesasData.push(Math.random() * 4000 + 2000);
        }
        
        // Destroy existing chart if it exists
        if (dashboardCharts.evolution) {
            dashboardCharts.evolution.destroy();
        }
        
        dashboardCharts.evolution = new Chart(ctx, {
            type: 'line',
            data: {
                labels: months,
                datasets: [
                    {
                        label: 'Receitas',
                        data: receitasData,
                        borderColor: CONFIG.COLORS.SUCCESS,
                        backgroundColor: CONFIG.COLORS.SUCCESS + '20',
                        borderWidth: 3,
                        fill: false,
                        tension: 0.4
                    },
                    {
                        label: 'Despesas',
                        data: despesasData,
                        borderColor: CONFIG.COLORS.DANGER,
                        backgroundColor: CONFIG.COLORS.DANGER + '20',
                        borderWidth: 3,
                        fill: false,
                        tension: 0.4
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            usePointStyle: true,
                            padding: 20
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const value = formatCurrency(context.raw);
                                return `${context.dataset.label}: ${value}`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return formatCurrency(value);
                            }
                        }
                    }
                }
            }
        });
        
    } catch (error) {
        console.error('Error loading evolution chart:', error);
        showEmptyChart(ctx, 'Nenhum dado de evolução encontrado');
    }
}

function showEmptyChart(ctx, message) {
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
    ctx.fillStyle = '#6b7280';
    ctx.font = '16px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(message, ctx.canvas.width / 2, ctx.canvas.height / 2);
}

async function loadRecentTransactions() {
    const transactionsList = document.getElementById('transacoesList');
    if (!transactionsList) return;
    
    try {
        showLoading(transactionsList, 'Carregando transações...');
        
        // Load recent expenses and incomes
        const [expenses, incomes] = await Promise.all([
            api.getExpenses(1).catch(() => []),
            api.getIncomes(1).catch(() => [])
        ]);
        
        // Combine and sort by date
        const allTransactions = [
            ...expenses.map(expense => ({ ...expense, type: 'expense' })),
            ...incomes.map(income => ({ ...income, type: 'income' }))
        ].sort((a, b) => new Date(b.dataDespesa || b.dataReceita) - new Date(a.dataDespesa || a.dataReceita))
         .slice(0, 5); // Show only last 5 transactions
        
        if (allTransactions.length === 0) {
            transactionsList.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-receipt"></i>
                    <h3>Nenhuma transação encontrada</h3>
                    <p>Adicione suas primeiras receitas e despesas</p>
                </div>
            `;
            return;
        }
        
        transactionsList.innerHTML = allTransactions.map(transaction => `
            <div class="transaction-item">
                <div class="transaction-info">
                    <div class="transaction-icon ${transaction.type}">
                        <i class="fas fa-arrow-${transaction.type === 'income' ? 'up' : 'down'}"></i>
                    </div>
                    <div class="transaction-details">
                        <h4>${transaction.descricao}</h4>
                        <p>${transaction.categoria} • ${transaction.conta}</p>
                    </div>
                </div>
                <div class="transaction-amount">
                    <div class="amount ${transaction.type}">${formatCurrency(transaction.valor)}</div>
                    <div class="date">${formatDate(transaction.dataDespesa || transaction.dataReceita)}</div>
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading recent transactions:', error);
        transactionsList.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-exclamation-triangle"></i>
                <h3>Erro ao carregar transações</h3>
                <p>Tente novamente mais tarde</p>
            </div>
        `;
    }
}

function setupDashboardEventListeners() {
    // Period selector change
    const periodoSelect = document.getElementById('periodoSelect');
    if (periodoSelect) {
        periodoSelect.addEventListener('change', async function() {
            await loadDashboardData();
            updateSummaryCards();
            await initializeExpensesChart();
        });
    }
    
    // Quick action buttons
    setupQuickActions();
    
    // Modal form submissions
    setupModalForms();
}

function setupQuickActions() {
    // These functions will be called by onclick attributes in HTML
    window.openModal = function(type) {
        const modal = document.getElementById('modalOverlay');
        const modalTitle = document.getElementById('modalTitle');
        const form = document.getElementById('transactionForm');
        const metodoPagamentoGroup = document.getElementById('metodoPagamentoGroup');
        
        if (!modal || !modalTitle || !form) return;
        
        // Reset form
        form.reset();
        clearFormErrors(form);
        
        // Set current date
        const dataField = document.getElementById('data');
        if (dataField) {
            dataField.value = UTILS.getCurrentDate();
        }
        
        // Configure modal based on type
        if (type === 'receita') {
            modalTitle.textContent = 'Nova Receita';
            metodoPagamentoGroup.style.display = 'none';
            form.dataset.type = 'receita';
        } else if (type === 'despesa') {
            modalTitle.textContent = 'Nova Despesa';
            metodoPagamentoGroup.style.display = 'block';
            form.dataset.type = 'despesa';
        } else if (type === 'meta') {
            // Redirect to goals page or implement goal modal
            window.location.href = 'metas.html';
            return;
        }
        
        // Populate selects
        populateModalSelects();
        
        // Show modal
        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    };
}

function populateModalSelects() {
    const categoriaSelect = document.getElementById('categoria');
    const contaSelect = document.getElementById('conta');
    const metodoPagamentoSelect = document.getElementById('metodoPagamento');
    
    if (categoriaSelect) {
        populateCategories(categoriaSelect);
    }
    
    if (contaSelect) {
        populateAccounts(contaSelect);
    }
    
    if (metodoPagamentoSelect) {
        populatePaymentMethods(metodoPagamentoSelect);
    }
}

function setupModalForms() {
    const transactionForm = document.getElementById('transactionForm');
    if (transactionForm) {
        transactionForm.addEventListener('submit', handleTransactionSubmit);
    }
    
    // Recurrent checkbox toggle
    const recorrenteCheckbox = document.getElementById('recorrente');
    const frequenciaGroup = document.getElementById('frequenciaGroup');
    
    if (recorrenteCheckbox && frequenciaGroup) {
        recorrenteCheckbox.addEventListener('change', function() {
            frequenciaGroup.style.display = this.checked ? 'block' : 'none';
        });
    }
}

async function handleTransactionSubmit(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    const type = form.dataset.type;
    
    if (!validateForm(form)) {
        return;
    }
    
    try {
        const transactionData = {
            valor: parseFloat(formData.get('valor')),
            descricao: formData.get('descricao'),
            recorrente: formData.get('recorrente') === 'on',
            frequencia: formData.get('frequencia') ? parseInt(formData.get('frequencia')) : null
        };
        
        if (type === 'receita') {
            transactionData.dataReceita = formData.get('data');
            await api.createIncome(
                transactionData,
                parseInt(formData.get('categoria')),
                parseInt(formData.get('conta'))
            );
            showSuccessMessage('Receita adicionada com sucesso!');
        } else if (type === 'despesa') {
            transactionData.dataDespesa = formData.get('data');
            await api.createExpense(
                transactionData,
                parseInt(formData.get('categoria')),
                parseInt(formData.get('conta')),
                parseInt(formData.get('metodoPagamento'))
            );
            showSuccessMessage('Despesa adicionada com sucesso!');
        }
        
        // Close modal and refresh dashboard
        closeModal();
        await loadDashboardData();
        updateSummaryCards();
        await initializeCharts();
        loadRecentTransactions();
        
    } catch (error) {
        console.error('Error saving transaction:', error);
        showErrorMessage('Erro ao salvar transação. Tente novamente.');
    }
}

// Export dashboard functions
window.dashboardFunctions = {
    loadDashboardData,
    updateSummaryCards,
    initializeCharts,
    loadRecentTransactions
};