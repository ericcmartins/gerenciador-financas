// Métodos de Pagamento page functionality

let allMetodos = [];
let editingMetodo = null;
let deletingMetodoId = null;

// Initialize métodos de pagamento page
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname.includes('metodos-pagamento.html')) {
        initializeMetodosPage();
    }
});

async function initializeMetodosPage() {
    try {
        // Load initial data
        await loadMetodos();
        
        // Setup event listeners
        setupMetodosEventListeners();
        
        // Display métodos
        displayMetodos();
        displayMetodosGrid();
        
    } catch (error) {
        console.error('Error initializing métodos page:', error);
        showErrorMessage('Erro ao carregar página de métodos de pagamento');
    }
}

async function loadMetodos() {
    try {
        allMetodos = await api.getPaymentMethods();
    } catch (error) {
        console.error('Error loading métodos:', error);
        allMetodos = [];
    }
}

function displayMetodos() {
    const tableBody = document.getElementById('metodosTableBody');
    if (!tableBody) return;
    
    if (allMetodos.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="6" class="text-center">
                    <div class="page-empty-state">
                        <i class="fas fa-credit-card"></i>
                        <h3>Nenhum método de pagamento encontrado</h3>
                        <p>Adicione seus cartões e formas de pagamento</p>
                        <button class="btn btn-primary" onclick="openMetodoModal()">
                            <i class="fas fa-plus"></i>
                            Novo Método
                        </button>
                    </div>
                </td>
            </tr>
        `;
        return;
    }
    
    tableBody.innerHTML = allMetodos.map(metodo => `
        <tr>
            <td>${metodo.nome}</td>
            <td>
                <span class="status-badge active">${metodo.tipo}</span>
            </td>
            <td>${getAccountName(metodo.contaId) || '-'}</td>
            <td>${metodo.limite ? formatCurrency(metodo.limite) : 'Sem limite'}</td>
            <td>${metodo.descricao || '-'}</td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editMetodo(${metodo.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="confirmDeleteMetodo(${metodo.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

function displayMetodosGrid() {
    const grid = document.getElementById('paymentMethodsGrid');
    if (!grid) return;
    
    if (allMetodos.length === 0) {
        grid.innerHTML = '';
        return;
    }
    
    grid.innerHTML = allMetodos.map(metodo => `
        <div class="payment-method-card">
            <div class="payment-method-header">
                <div class="payment-method-info">
                    <h3>${metodo.nome}</h3>
                    <div class="payment-method-type">${metodo.tipo}</div>
                    <div class="payment-method-account">${getAccountName(metodo.contaId) || 'Conta não encontrada'}</div>
                </div>
                <div class="payment-method-icon">
                    <i class="fas fa-${getPaymentMethodIcon(metodo.tipo)}"></i>
                </div>
            </div>
            <div class="payment-method-details">
                ${metodo.limite ? `<div class="payment-method-limit">Limite: ${formatCurrency(metodo.limite)}</div>` : ''}
                ${metodo.descricao ? `<div class="payment-method-description">${metodo.descricao}</div>` : ''}
            </div>
            <div class="payment-method-actions" style="margin-top: 1rem; display: flex; gap: 0.5rem; justify-content: flex-end;">
                <button class="action-btn edit-btn" onclick="editMetodo(${metodo.id})" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="action-btn delete-btn" onclick="confirmDeleteMetodo(${metodo.id})" title="Excluir">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        </div>
    `).join('');
}

function getAccountName(accountId) {
    const account = accounts.find(acc => acc.id === accountId);
    return account ? account.numeroConta : null;
}

function getPaymentMethodIcon(tipo) {
    const icons = {
        'Cartão de Crédito': 'credit-card',
        'Cartão de Débito': 'credit-card',
        'PIX': 'mobile-alt',
        'Dinheiro': 'money-bill-wave',
        'Transferência': 'exchange-alt',
        'Boleto': 'barcode',
        'Outro': 'wallet'
    };
    return icons[tipo] || 'wallet';
}

function setupMetodosEventListeners() {
    // Form submission
    const metodoForm = document.getElementById('metodoForm');
    if (metodoForm) {
        metodoForm.addEventListener('submit', handleMetodoSubmit);
    }
    
    // Search functionality
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', UTILS.debounce(handleSearch, 300));
    }
}

function handleSearch(event) {
    const searchTerm = event.target.value.toLowerCase();
    const filteredMetodos = allMetodos.filter(metodo => 
        metodo.nome.toLowerCase().includes(searchTerm) ||
        metodo.tipo.toLowerCase().includes(searchTerm) ||
        (metodo.descricao && metodo.descricao.toLowerCase().includes(searchTerm))
    );
    
    displayFilteredMetodos(filteredMetodos);
}

function displayFilteredMetodos(metodos) {
    const tableBody = document.getElementById('metodosTableBody');
    if (!tableBody) return;
    
    if (metodos.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="6" class="text-center">
                    <div class="empty-state">
                        <i class="fas fa-search"></i>
                        <h3>Nenhum método encontrado</h3>
                        <p>Tente ajustar os termos de busca</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }
    
    tableBody.innerHTML = metodos.map(metodo => `
        <tr>
            <td>${metodo.nome}</td>
            <td>
                <span class="status-badge active">${metodo.tipo}</span>
            </td>
            <td>${getAccountName(metodo.contaId) || '-'}</td>
            <td>${metodo.limite ? formatCurrency(metodo.limite) : 'Sem limite'}</td>
            <td>${metodo.descricao || '-'}</td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editMetodo(${metodo.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="confirmDeleteMetodo(${metodo.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

// Global functions for HTML onclick events
window.openMetodoModal = function(metodoId = null) {
    const modal = document.getElementById('modalOverlay');
    const modalTitle = document.getElementById('modalTitle');
    const form = document.getElementById('metodoForm');
    
    if (!modal || !modalTitle || !form) return;
    
    // Reset form
    form.reset();
    clearFormErrors(form);
    editingMetodo = null;
    
    if (metodoId) {
        // Edit mode
        const metodo = allMetodos.find(m => m.id === metodoId);
        if (metodo) {
            modalTitle.textContent = 'Editar Método de Pagamento';
            editingMetodo = metodo;
            
            // Populate form
            document.getElementById('nome').value = metodo.nome;
            document.getElementById('tipo').value = metodo.tipo;
            document.getElementById('limite').value = metodo.limite || '';
            document.getElementById('descricao').value = metodo.descricao || '';
        }
    } else {
        // Create mode
        modalTitle.textContent = 'Novo Método de Pagamento';
    }
    
    // Populate accounts select
    const contaSelect = document.getElementById('conta');
    if (contaSelect) {
        populateAccounts(contaSelect);
        if (editingMetodo) {
            contaSelect.value = editingMetodo.contaId;
        }
    }
    
    // Show modal
    openModal();
};

window.editMetodo = function(metodoId) {
    openMetodoModal(metodoId);
};

window.confirmDeleteMetodo = function(metodoId) {
    deletingMetodoId = metodoId;
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
    deletingMetodoId = null;
};

window.confirmDelete = function() {
    if (deletingMetodoId) {
        handleDeleteMetodo(deletingMetodoId);
    }
};

async function handleMetodoSubmit(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    if (!validateForm(form)) {
        return;
    }
    
    try {
        const metodoData = {
            nome: formData.get('nome'),
            tipo: formData.get('tipo'),
            limite: formData.get('limite') ? parseFloat(formData.get('limite')) : null,
            descricao: formData.get('descricao') || null
        };
        
        const contaId = parseInt(formData.get('conta'));
        
        if (editingMetodo) {
            // Update existing método
            await api.updatePaymentMethod(editingMetodo.id, metodoData, contaId);
            showSuccessMessage('Método de pagamento atualizado com sucesso!');
        } else {
            // Create new método
            await api.createPaymentMethod(metodoData, contaId);
            showSuccessMessage('Método de pagamento criado com sucesso!');
        }
        
        // Close modal and refresh data
        closeModal();
        await loadMetodos();
        
        // Update global payment methods array
        paymentMethods = allMetodos;
        
        displayMetodos();
        displayMetodosGrid();
        
    } catch (error) {
        console.error('Error saving método:', error);
        showErrorMessage('Erro ao salvar método de pagamento. Tente novamente.');
    }
}

async function handleDeleteMetodo(metodoId) {
    try {
        await api.deletePaymentMethod(metodoId);
        showSuccessMessage('Método de pagamento excluído com sucesso!');
        
        // Close delete modal
        closeDeleteModal();
        
        // Refresh data
        await loadMetodos();
        
        // Update global payment methods array
        paymentMethods = allMetodos;
        
        displayMetodos();
        displayMetodosGrid();
        
    } catch (error) {
        console.error('Error deleting método:', error);
        showErrorMessage('Erro ao excluir método de pagamento. Tente novamente.');
        closeDeleteModal();
    }
}