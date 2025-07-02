// Despesas page functionality

let currentPage = 1;
let totalPages = 1;
let currentFilters = {};
let allDespesas = [];
let editingDespesa = null;

// Initialize despesas page
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname.includes('despesas.html')) {
        initializeDespesasPage();
    }
});

async function initializeDespesasPage() {
    try {
        // Load initial data
        await loadDespesas();
        
        // Setup event listeners
        setupDespesasEventListeners();
        
        // Populate filter selects
        populateFilterSelects();
        
        // Update summary
        updateDespesasSummary();
        
    } catch (error) {
        console.error('Error initializing despesas page:', error);
        showErrorMessage('Erro ao carregar página de despesas');
    }
}

async function loadDespesas() {
    const tableBody = document.getElementById('despesasTableBody');
    if (!tableBody) return;
    
    try {
        showLoading(tableBody, 'Carregando despesas...');
        
        const period = document.getElementById('filtroData')?.value || 1;
        allDespesas = await api.getExpenses(period);
        
        // Apply filters
        const filteredDespesas = applyFilters(allDespesas);
        
        // Update pagination
        updatePagination(filteredDespesas.length);
        
        // Display despesas
        displayDespesas(filteredDespesas);
        
    } catch (error) {
        console.error('Error loading despesas:', error);
        tableBody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center">
                    <div class="empty-state">
                        <i class="fas fa-exclamation-triangle"></i>
                        <p>Erro ao carregar despesas</p>
                    </div>
                </td>
            </tr>
        `;
    }
}

function applyFilters(despesas) {
    let filtered = [...despesas];
    
    // Filter by category
    if (currentFilters.categoria) {
        filtered = filtered.filter(despesa => despesa.categoria === currentFilters.categoria);
    }
    
    // Filter by account
    if (currentFilters.conta) {
        filtered = filtered.filter(despesa => despesa.conta === currentFilters.conta);
    }
    
    // Filter by payment method
    if (currentFilters.metodo) {
        filtered = filtered.filter(despesa => despesa.metodoPagamento === currentFilters.metodo);
    }
    
    return filtered;
}

function displayDespesas(despesas) {
    const tableBody = document.getElementById('despesasTableBody');
    if (!tableBody) return;
    
    if (despesas.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center">
                    <div class="page-empty-state">
                        <i class="fas fa-arrow-down"></i>
                        <h3>Nenhuma despesa encontrada</h3>
                        <p>Adicione sua primeira despesa para começar</p>
                        <button class="btn btn-primary" onclick="openDespesaModal()">
                            <i class="fas fa-plus"></i>
                            Nova Despesa
                        </button>
                    </div>
                </td>
            </tr>
        `;
        return;
    }
    
    // Paginate results
    const startIndex = (currentPage - 1) * CONFIG.APP.ITEMS_PER_PAGE;
    const endIndex = startIndex + CONFIG.APP.ITEMS_PER_PAGE;
    const paginatedDespesas = despesas.slice(startIndex, endIndex);
    
    tableBody.innerHTML = paginatedDespesas.map(despesa => `
        <tr>
            <td>${formatDate(despesa.dataDespesa)}</td>
            <td>${despesa.descricao}</td>
            <td>${despesa.categoria}</td>
            <td>${despesa.conta}</td>
            <td>${despesa.metodoPagamento}</td>
            <td class="amount-cell expense">${formatCurrency(despesa.valor)}</td>
            <td>
                ${despesa.recorrente ? 
                    `<span class="status-badge recurrent">Sim (${despesa.frequencia} dias)</span>` : 
                    '<span class="status-badge">Não</span>'
                }
            </td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editDespesa(${despesa.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="deleteDespesa(${despesa.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

function updatePagination(totalItems) {
    totalPages = Math.ceil(totalItems / CONFIG.APP.ITEMS_PER_PAGE);
    
    const pageInfo = document.getElementById('pageInfo');
    const prevButton = document.getElementById('prevPage');
    const nextButton = document.getElementById('nextPage');
    
    if (pageInfo) {
        pageInfo.textContent = `Página ${currentPage} de ${totalPages}`;
    }
    
    if (prevButton) {
        prevButton.disabled = currentPage <= 1;
    }
    
    if (nextButton) {
        nextButton.disabled = currentPage >= totalPages;
    }
}

function updateDespesasSummary() {
    const totalDespesasElement = document.getElementById('totalDespesas');
    const despesasRecorrentesElement = document.getElementById('despesasRecorrentes');
    const mediaMensalElement = document.getElementById('mediaMensal');
    
    if (!allDespesas.length) {
        if (totalDespesasElement) totalDespesasElement.textContent = formatCurrency(0);
        if (despesasRecorrentesElement) despesasRecorrentesElement.textContent = '0';
        if (mediaMensalElement) mediaMensalElement.textContent = formatCurrency(0);
        return;
    }
    
    const total = allDespesas.reduce((sum, despesa) => sum + despesa.valor, 0);
    const recorrentes = allDespesas.filter(despesa => despesa.recorrente).length;
    const media = total / Math.max(1, allDespesas.length);
    
    if (totalDespesasElement) {
        totalDespesasElement.textContent = formatCurrency(total);
    }
    
    if (despesasRecorrentesElement) {
        despesasRecorrentesElement.textContent = recorrentes.toString();
    }
    
    if (mediaMensalElement) {
        mediaMensalElement.textContent = formatCurrency(media);
    }
}

function setupDespesasEventListeners() {
    // Filter change events
    const filtroData = document.getElementById('filtroData');
    
    if (filtroData) {
        filtroData.addEventListener('change', loadDespesas);
    }
    
    // Form submission
    const despesaForm = document.getElementById('despesaForm');
    if (despesaForm) {
        despesaForm.addEventListener('submit', handleDespesaSubmit);
    }
    
    // Recurrent checkbox
    const recorrenteCheckbox = document.getElementById('recorrente');
    const frequenciaGroup = document.getElementById('frequenciaGroup');
    
    if (recorrenteCheckbox && frequenciaGroup) {
        recorrenteCheckbox.addEventListener('change', function() {
            frequenciaGroup.style.display = this.checked ? 'block' : 'none';
        });
    }
}

function populateFilterSelects() {
    const filtroCategoria = document.getElementById('filtroCategoria');
    const filtroConta = document.getElementById('filtroConta');
    const filtroMetodo = document.getElementById('filtroMetodo');
    
    if (filtroCategoria) {
        populateCategories(filtroCategoria, 'Todas as categorias');
    }
    
    if (filtroConta) {
        populateAccounts(filtroConta, 'Todas as contas');
    }
    
    if (filtroMetodo) {
        populatePaymentMethods(filtroMetodo, 'Todos os métodos');
    }
}

// Global functions for HTML onclick events
window.openDespesaModal = function(despesaId = null) {
    const modal = document.getElementById('modalOverlay');
    const modalTitle = document.getElementById('modalTitle');
    const form = document.getElementById('despesaForm');
    
    if (!modal || !modalTitle || !form) return;
    
    // Reset form
    form.reset();
    clearFormErrors(form);
    editingDespesa = null;
    
    if (despesaId) {
        // Edit mode
        const despesa = allDespesas.find(d => d.id === despesaId);
        if (despesa) {
            modalTitle.textContent = 'Editar Despesa';
            editingDespesa = despesa;
            
            // Populate form
            document.getElementById('valor').value = despesa.valor;
            document.getElementById('descricao').value = despesa.descricao;
            document.getElementById('dataDespesa').value = formatDateForInput(despesa.dataDespesa);
            document.getElementById('recorrente').checked = despesa.recorrente;
            document.getElementById('frequencia').value = despesa.frequencia || '';
            
            // Show frequency group if recurrent
            const frequenciaGroup = document.getElementById('frequenciaGroup');
            if (frequenciaGroup) {
                frequenciaGroup.style.display = despesa.recorrente ? 'block' : 'none';
            }
        }
    } else {
        // Create mode
        modalTitle.textContent = 'Nova Despesa';
        document.getElementById('dataDespesa').value = UTILS.getCurrentDate();
    }
    
    // Populate selects
    const categoriaSelect = document.getElementById('categoria');
    const contaSelect = document.getElementById('conta');
    const metodoPagamentoSelect = document.getElementById('metodoPagamento');
    
    if (categoriaSelect) {
        populateCategories(categoriaSelect);
        if (editingDespesa) {
            const categoryOption = Array.from(categoriaSelect.options).find(option => 
                option.textContent === editingDespesa.categoria
            );
            if (categoryOption) {
                categoriaSelect.value = categoryOption.value;
            }
        }
    }
    
    if (contaSelect) {
        populateAccounts(contaSelect);
        if (editingDespesa) {
            const accountOption = Array.from(contaSelect.options).find(option => 
                option.textContent === editingDespesa.conta
            );
            if (accountOption) {
                contaSelect.value = accountOption.value;
            }
        }
    }
    
    if (metodoPagamentoSelect) {
        populatePaymentMethods(metodoPagamentoSelect);
        if (editingDespesa) {
            const methodOption = Array.from(metodoPagamentoSelect.options).find(option => 
                option.textContent === editingDespesa.metodoPagamento
            );
            if (methodOption) {
                metodoPagamentoSelect.value = methodOption.value;
            }
        }
    }
    
    // Show modal
    openModal();
};

window.editDespesa = function(despesaId) {
    openDespesaModal(despesaId);
};

window.deleteDespesa = function(despesaId) {
    const despesa = allDespesas.find(d => d.id === despesaId);
    if (!despesa) return;
    
    if (confirm(`Tem certeza que deseja excluir a despesa "${despesa.descricao}"?`)) {
        handleDeleteDespesa(despesaId);
    }
};

window.aplicarFiltros = function() {
    const filtroCategoria = document.getElementById('filtroCategoria');
    const filtroConta = document.getElementById('filtroConta');
    const filtroMetodo = document.getElementById('filtroMetodo');
    
    currentFilters = {
        categoria: filtroCategoria?.value || '',
        conta: filtroConta?.value || '',
        metodo: filtroMetodo?.value || ''
    };
    
    currentPage = 1;
    const filteredDespesas = applyFilters(allDespesas);
    updatePagination(filteredDespesas.length);
    displayDespesas(filteredDespesas);
};

window.exportarDespesas = function() {
    if (allDespesas.length === 0) {
        showErrorMessage('Nenhuma despesa para exportar');
        return;
    }
    
    // Create CSV content
    const headers = ['Data', 'Descrição', 'Categoria', 'Conta', 'Método', 'Valor', 'Recorrente', 'Frequência'];
    const csvContent = [
        headers.join(','),
        ...allDespesas.map(despesa => [
            formatDate(despesa.dataDespesa),
            `"${despesa.descricao}"`,
            `"${despesa.categoria}"`,
            `"${despesa.conta}"`,
            `"${despesa.metodoPagamento}"`,
            despesa.valor,
            despesa.recorrente ? 'Sim' : 'Não',
            despesa.frequencia || ''
        ].join(','))
    ].join('\n');
    
    // Download file
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `despesas_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
    
    showSuccessMessage('Despesas exportadas com sucesso!');
};

window.previousPage = function() {
    if (currentPage > 1) {
        currentPage--;
        const filteredDespesas = applyFilters(allDespesas);
        updatePagination(filteredDespesas.length);
        displayDespesas(filteredDespesas);
    }
};

window.nextPage = function() {
    if (currentPage < totalPages) {
        currentPage++;
        const filteredDespesas = applyFilters(allDespesas);
        updatePagination(filteredDespesas.length);
        displayDespesas(filteredDespesas);
    }
};

async function handleDespesaSubmit(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    if (!validateForm(form)) {
        return;
    }
    
    try {
        const despesaData = {
            valor: parseFloat(formData.get('valor')),
            descricao: formData.get('descricao'),
            dataDespesa: formData.get('dataDespesa'),
            recorrente: formData.get('recorrente') === 'on',
            frequencia: formData.get('frequencia') ? parseInt(formData.get('frequencia')) : null
        };
        
        const categoriaId = parseInt(formData.get('categoria'));
        const contaId = parseInt(formData.get('conta'));
        const metodoPagamentoId = parseInt(formData.get('metodoPagamento'));
        
        if (editingDespesa) {
            // Update existing despesa
            await api.updateExpense(editingDespesa.id, despesaData, categoriaId, contaId, metodoPagamentoId);
            showSuccessMessage('Despesa atualizada com sucesso!');
        } else {
            // Create new despesa
            await api.createExpense(despesaData, categoriaId, contaId, metodoPagamentoId);
            showSuccessMessage('Despesa criada com sucesso!');
        }
        
        // Close modal and refresh data
        closeModal();
        await loadDespesas();
        updateDespesasSummary();
        
    } catch (error) {
        console.error('Error saving despesa:', error);
        showErrorMessage('Erro ao salvar despesa. Tente novamente.');
    }
}

async function handleDeleteDespesa(despesaId) {
    try {
        await api.deleteExpense(despesaId);
        showSuccessMessage('Despesa excluída com sucesso!');
        
        // Refresh data
        await loadDespesas();
        updateDespesasSummary();
        
    } catch (error) {
        console.error('Error deleting despesa:', error);
        showErrorMessage('Erro ao excluir despesa. Tente novamente.');
    }
}