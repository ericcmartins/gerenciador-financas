// Contas page functionality

let allContas = [];
let editingConta = null;
let deletingContaId = null;

// Initialize contas page
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname.includes('contas.html')) {
        initializeContasPage();
    }
});

async function initializeContasPage() {
    try {
        // Load initial data
        await loadContas();
        
        // Setup event listeners
        setupContasEventListeners();
        
        // Display contas
        displayContas();
        displayContasGrid();
        
    } catch (error) {
        console.error('Error initializing contas page:', error);
        showErrorMessage('Erro ao carregar página de contas');
    }
}

async function loadContas() {
    try {
        allContas = await api.getAccounts();
        
        // Load balance for each account
        const balances = await api.getBalanceByAccount().catch(() => []);
        
        // Merge balance data with accounts
        allContas = allContas.map(conta => {
            const balance = balances.find(b => b.numeroConta === conta.numeroConta);
            return {
                ...conta,
                saldo: balance ? balance.saldoConta : 0
            };
        });
        
    } catch (error) {
        console.error('Error loading contas:', error);
        allContas = [];
    }
}

function displayContas() {
    const tableBody = document.getElementById('contasTableBody');
    if (!tableBody) return;
    
    if (allContas.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center">
                    <div class="page-empty-state">
                        <i class="fas fa-university"></i>
                        <h3>Nenhuma conta encontrada</h3>
                        <p>Adicione suas contas bancárias e carteiras</p>
                        <button class="btn btn-primary" onclick="openContaModal()">
                            <i class="fas fa-plus"></i>
                            Nova Conta
                        </button>
                    </div>
                </td>
            </tr>
        `;
        return;
    }
    
    tableBody.innerHTML = allContas.map(conta => `
        <tr>
            <td>${conta.numeroConta}</td>
            <td>
                <span class="status-badge active">${conta.tipo}</span>
            </td>
            <td>${conta.instituicao}</td>
            <td class="amount-cell ${conta.saldo >= 0 ? 'income' : 'expense'}">${formatCurrency(conta.saldo)}</td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editConta(${conta.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="confirmDeleteConta(${conta.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

function displayContasGrid() {
    const grid = document.getElementById('accountsGrid');
    if (!grid) return;
    
    if (allContas.length === 0) {
        grid.innerHTML = '';
        return;
    }
    
    grid.innerHTML = allContas.map(conta => `
        <div class="account-card">
            <div class="account-header">
                <div class="account-info">
                    <h3>${conta.numeroConta}</h3>
                    <div class="account-type">${conta.tipo}</div>
                    <div class="account-institution">${conta.instituicao}</div>
                </div>
                <div class="account-icon">
                    <i class="fas fa-${getAccountIcon(conta.tipo)}"></i>
                </div>
            </div>
            <div class="account-balance">
                <div class="balance-label">Saldo</div>
                <div class="balance-amount ${conta.saldo >= 0 ? 'positive' : 'negative'}">${formatCurrency(conta.saldo)}</div>
            </div>
            <div class="account-actions" style="margin-top: 1rem; display: flex; gap: 0.5rem; justify-content: flex-end;">
                <button class="action-btn edit-btn" onclick="editConta(${conta.id})" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="action-btn delete-btn" onclick="confirmDeleteConta(${conta.id})" title="Excluir">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        </div>
    `).join('');
}

function getAccountIcon(tipo) {
    const icons = {
        'Conta Corrente': 'university',
        'Conta Poupança': 'piggy-bank',
        'Conta Salário': 'money-check-alt',
        'Carteira Digital': 'mobile-alt',
        'Dinheiro': 'money-bill-wave',
        'Outro': 'wallet'
    };
    return icons[tipo] || 'wallet';
}

function setupContasEventListeners() {
    // Form submission
    const contaForm = document.getElementById('contaForm');
    if (contaForm) {
        contaForm.addEventListener('submit', handleContaSubmit);
    }
    
    // Search functionality
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', UTILS.debounce(handleSearch, 300));
    }
}

function handleSearch(event) {
    const searchTerm = event.target.value.toLowerCase();
    const filteredContas = allContas.filter(conta => 
        conta.numeroConta.toLowerCase().includes(searchTerm) ||
        conta.tipo.toLowerCase().includes(searchTerm) ||
        conta.instituicao.toLowerCase().includes(searchTerm)
    );
    
    displayFilteredContas(filteredContas);
}

function displayFilteredContas(contas) {
    const tableBody = document.getElementById('contasTableBody');
    if (!tableBody) return;
    
    if (contas.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center">
                    <div class="empty-state">
                        <i class="fas fa-search"></i>
                        <h3>Nenhuma conta encontrada</h3>
                        <p>Tente ajustar os termos de busca</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }
    
    tableBody.innerHTML = contas.map(conta => `
        <tr>
            <td>${conta.numeroConta}</td>
            <td>
                <span class="status-badge active">${conta.tipo}</span>
            </td>
            <td>${conta.instituicao}</td>
            <td class="amount-cell ${conta.saldo >= 0 ? 'income' : 'expense'}">${formatCurrency(conta.saldo)}</td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editConta(${conta.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="confirmDeleteConta(${conta.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

// Global functions for HTML onclick events
window.openContaModal = function(contaId = null) {
    const modal = document.getElementById('modalOverlay');
    const modalTitle = document.getElementById('modalTitle');
    const form = document.getElementById('contaForm');
    
    if (!modal || !modalTitle || !form) return;
    
    // Reset form
    form.reset();
    clearFormErrors(form);
    editingConta = null;
    
    if (contaId) {
        // Edit mode
        const conta = allContas.find(c => c.id === contaId);
        if (conta) {
            modalTitle.textContent = 'Editar Conta';
            editingConta = conta;
            
            // Populate form
            document.getElementById('numeroConta').value = conta.numeroConta;
            document.getElementById('tipo').value = conta.tipo;
            document.getElementById('instituicao').value = conta.instituicao;
        }
    } else {
        // Create mode
        modalTitle.textContent = 'Nova Conta';
    }
    
    // Show modal
    openModal();
};

window.editConta = function(contaId) {
    openContaModal(contaId);
};

window.confirmDeleteConta = function(contaId) {
    deletingContaId = contaId;
    const deleteModal = document.getElementById('deleteModalOverlay');
    if (deleteModal) {
        deleteModal.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
};

window.closeDeleteModal = function() {
    const deleteModal = document.getElementById('deleteModalOverlay');
    if (deleteModal) {
        deleteModal.classList.remove('active');
        document.body.style.overflow = '';
    }
    deletingContaId = null;
};

window.confirmDelete = function() {
    if (deletingContaId) {
        handleDeleteConta(deletingContaId);
    }
};

async function handleContaSubmit(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    if (!validateForm(form)) {
        return;
    }
    
    try {
        const contaData = {
            numeroConta: formData.get('numeroConta'),
            tipo: formData.get('tipo'),
            instituicao: formData.get('instituicao')
        };
        
        if (editingConta) {
            // Update existing conta
            await api.updateAccount(editingConta.id, contaData);
            showSuccessMessage('Conta atualizada com sucesso!');
        } else {
            // Create new conta
            await api.createAccount(contaData);
            showSuccessMessage('Conta criada com sucesso!');
        }
        
        // Close modal and refresh data
        closeModal();
        await loadContas();
        
        // Update global accounts array
        accounts = allContas;
        
        displayContas();
        displayContasGrid();
        
    } catch (error) {
        console.error('Error saving conta:', error);
        showErrorMessage('Erro ao salvar conta. Tente novamente.');
    }
}

async function handleDeleteConta(contaId) {
    try {
        await api.deleteAccount(contaId);
        showSuccessMessage('Conta excluída com sucesso!');
        
        // Close delete modal
        closeDeleteModal();
        
        // Refresh data
        await loadContas();
        
        // Update global accounts array
        accounts = allContas;
        
        displayContas();
        displayContasGrid();
        
    } catch (error) {
        console.error('Error deleting conta:', error);
        showErrorMessage('Erro ao excluir conta. Tente novamente.');
        closeDeleteModal();
    }
}