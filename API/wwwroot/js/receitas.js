// Receitas page functionality

let currentPage = 1;
let totalPages = 1;
let currentFilters = {};
let allReceitas = [];
let editingReceita = null;

// Initialize receitas page
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname.includes('receitas.html')) {
        initializeReceitasPage();
    }
});

async function initializeReceitasPage() {
    try {
        // Load initial data
        await loadReceitas();
        
        // Setup event listeners
        setupReceitasEventListeners();
        
        // Populate filter selects
        populateFilterSelects();
        
        // Update summary
        updateReceitasSummary();
        
    } catch (error) {
        console.error('Error initializing receitas page:', error);
        showErrorMessage('Erro ao carregar página de receitas');
    }
}

async function loadReceitas() {
    const tableBody = document.getElementById('receitasTableBody');
    if (!tableBody) return;
    
    try {
        showLoading(tableBody, 'Carregando receitas...');
        
        const period = document.getElementById('filtroData')?.value || 1;
        allReceitas = await api.getIncomes(period);
        
        // Apply filters
        const filteredReceitas = applyFilters(allReceitas);
        
        // Update pagination
        updatePagination(filteredReceitas.length);
        
        // Display receitas
        displayReceitas(filteredReceitas);
        
    } catch (error) {
        console.error('Error loading receitas:', error);
        tableBody.innerHTML = `
            <tr>
                <td colspan="7" class="text-center">
                    <div class="empty-state">
                        <i class="fas fa-exclamation-triangle"></i>
                        <p>Erro ao carregar receitas</p>
                    </div>
                </td>
            </tr>
        `;
    }
}

function applyFilters(receitas) {
    let filtered = [...receitas];
    
    // Filter by category
    if (currentFilters.categoria) {
        filtered = filtered.filter(receita => receita.categoria === currentFilters.categoria);
    }
    
    // Filter by account
    if (currentFilters.conta) {
        filtered = filtered.filter(receita => receita.conta === currentFilters.conta);
    }
    
    return filtered;
}

function displayReceitas(receitas) {
    const tableBody = document.getElementById('receitasTableBody');
    if (!tableBody) return;
    
    if (receitas.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="7" class="text-center">
                    <div class="page-empty-state">
                        <i class="fas fa-arrow-up"></i>
                        <h3>Nenhuma receita encontrada</h3>
                        <p>Adicione sua primeira receita para começar</p>
                        <button class="btn btn-primary" onclick="openReceitaModal()">
                            <i class="fas fa-plus"></i>
                            Nova Receita
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
    const paginatedReceitas = receitas.slice(startIndex, endIndex);
    
    tableBody.innerHTML = paginatedReceitas.map(receita => `
        <tr>
            <td>${formatDate(receita.dataReceita)}</td>
            <td>${receita.descricao}</td>
            <td>${receita.categoria}</td>
            <td>${receita.conta}</td>
            <td class="amount-cell income">${formatCurrency(receita.valor)}</td>
            <td>
                ${receita.recorrente ? 
                    `<span class="status-badge recurrent">Sim (${receita.frequencia} dias)</span>` : 
                    '<span class="status-badge">Não</span>'
                }
            </td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editReceita(${receita.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="deleteReceita(${receita.id})" title="Excluir">
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

function updateReceitasSummary() {
    const totalReceitasElement = document.getElementById('totalReceitas');
    const receitasRecorrentesElement = document.getElementById('receitasRecorrentes');
    const mediaMensalElement = document.getElementById('mediaMensal');
    
    if (!allReceitas.length) {
        if (totalReceitasElement) totalReceitasElement.textContent = formatCurrency(0);
        if (receitasRecorrentesElement) receitasRecorrentesElement.textContent = '0';
        if (mediaMensalElement) mediaMensalElement.textContent = formatCurrency(0);
        return;
    }
    
    const total = allReceitas.reduce((sum, receita) => sum + receita.valor, 0);
    const recorrentes = allReceitas.filter(receita => receita.recorrente).length;
    const media = total / Math.max(1, allReceitas.length);
    
    if (totalReceitasElement) {
        totalReceitasElement.textContent = formatCurrency(total);
    }
    
    if (receitasRecorrentesElement) {
        receitasRecorrentesElement.textContent = recorrentes.toString();
    }
    
    if (mediaMensalElement) {
        mediaMensalElement.textContent = formatCurrency(media);
    }
}

function setupReceitasEventListeners() {
    // Filter change events
    const filtroData = document.getElementById('filtroData');
    
    if (filtroData) {
        filtroData.addEventListener('change', loadReceitas);
    }
    
    // Form submission
    const receitaForm = document.getElementById('receitaForm');
    if (receitaForm) {
        receitaForm.addEventListener('submit', handleReceitaSubmit);
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
    
    if (filtroCategoria) {
        populateCategories(filtroCategoria, 'Todas as categorias');
    }
    
    if (filtroConta) {
        populateAccounts(filtroConta, 'Todas as contas');
    }
}

// Global functions for HTML onclick events
window.openReceitaModal = function(receitaId = null) {
    const modal = document.getElementById('modalOverlay');
    const modalTitle = document.getElementById('modalTitle');
    const form = document.getElementById('receitaForm');
    
    if (!modal || !modalTitle || !form) return;
    
    // Reset form
    form.reset();
    clearFormErrors(form);
    editingReceita = null;
    
    if (receitaId) {
        // Edit mode
        const receita = allReceitas.find(r => r.id === receitaId);
        if (receita) {
            modalTitle.textContent = 'Editar Receita';
            editingReceita = receita;
            
            // Populate form
            document.getElementById('valor').value = receita.valor;
            document.getElementById('descricao').value = receita.descricao;
            document.getElementById('dataReceita').value = formatDateForInput(receita.dataReceita);
            document.getElementById('recorrente').checked = receita.recorrente;
            document.getElementById('frequencia').value = receita.frequencia || '';
            
            // Show frequency group if recurrent
            const frequenciaGroup = document.getElementById('frequenciaGroup');
            if (frequenciaGroup) {
                frequenciaGroup.style.display = receita.recorrente ? 'block' : 'none';
            }
        }
    } else {
        // Create mode
        modalTitle.textContent = 'Nova Receita';
        document.getElementById('dataReceita').value = UTILS.getCurrentDate();
    }
    
    // Populate selects
    const categoriaSelect = document.getElementById('categoria');
    const contaSelect = document.getElementById('conta');
    
    if (categoriaSelect) {
        populateCategories(categoriaSelect);
        if (editingReceita) {
            // Find and select the category
            const categoryOption = Array.from(categoriaSelect.options).find(option => 
                option.textContent === editingReceita.categoria
            );
            if (categoryOption) {
                categoriaSelect.value = categoryOption.value;
            }
        }
    }
    
    if (contaSelect) {
        populateAccounts(contaSelect);
        if (editingReceita) {
            // Find and select the account
            const accountOption = Array.from(contaSelect.options).find(option => 
                option.textContent === editingReceita.conta
            );
            if (accountOption) {
                contaSelect.value = accountOption.value;
            }
        }
    }
    
    // Show modal
    openModal();
};

window.editReceita = function(receitaId) {
    openReceitaModal(receitaId);
};

window.deleteReceita = function(receitaId) {
    const receita = allReceitas.find(r => r.id === receitaId);
    if (!receita) return;
    
    if (confirm(`Tem certeza que deseja excluir a receita "${receita.descricao}"?`)) {
        handleDeleteReceita(receitaId);
    }
};

window.aplicarFiltros = function() {
    const filtroCategoria = document.getElementById('filtroCategoria');
    const filtroConta = document.getElementById('filtroConta');
    
    currentFilters = {
        categoria: filtroCategoria?.value || '',
        conta: filtroConta?.value || ''
    };
    
    currentPage = 1;
    const filteredReceitas = applyFilters(allReceitas);
    updatePagination(filteredReceitas.length);
    displayReceitas(filteredReceitas);
};

window.exportarReceitas = function() {
    if (allReceitas.length === 0) {
        showErrorMessage('Nenhuma receita para exportar');
        return;
    }
    
    // Create CSV content
    const headers = ['Data', 'Descrição', 'Categoria', 'Conta', 'Valor', 'Recorrente', 'Frequência'];
    const csvContent = [
        headers.join(','),
        ...allReceitas.map(receita => [
            formatDate(receita.dataReceita),
            `"${receita.descricao}"`,
            `"${receita.categoria}"`,
            `"${receita.conta}"`,
            receita.valor,
            receita.recorrente ? 'Sim' : 'Não',
            receita.frequencia || ''
        ].join(','))
    ].join('\n');
    
    // Download file
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `receitas_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
    
    showSuccessMessage('Receitas exportadas com sucesso!');
};

window.previousPage = function() {
    if (currentPage > 1) {
        currentPage--;
        const filteredReceitas = applyFilters(allReceitas);
        updatePagination(filteredReceitas.length);
        displayReceitas(filteredReceitas);
    }
};

window.nextPage = function() {
    if (currentPage < totalPages) {
        currentPage++;
        const filteredReceitas = applyFilters(allReceitas);
        updatePagination(filteredReceitas.length);
        displayReceitas(filteredReceitas);
    }
};

async function handleReceitaSubmit(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    if (!validateForm(form)) {
        return;
    }
    
    try {
        const receitaData = {
            valor: parseFloat(formData.get('valor')),
            descricao: formData.get('descricao'),
            dataReceita: formData.get('dataReceita'),
            recorrente: formData.get('recorrente') === 'on',
            frequencia: formData.get('frequencia') ? parseInt(formData.get('frequencia')) : null
        };
        
        const categoriaId = parseInt(formData.get('categoria'));
        const contaId = parseInt(formData.get('conta'));
        
        if (editingReceita) {
            // Update existing receita
            await api.updateIncome(editingReceita.id, receitaData, categoriaId, contaId);
            showSuccessMessage('Receita atualizada com sucesso!');
        } else {
            // Create new receita
            await api.createIncome(receitaData, categoriaId, contaId);
            showSuccessMessage('Receita criada com sucesso!');
        }
        
        // Close modal and refresh data
        closeModal();
        await loadReceitas();
        updateReceitasSummary();
        
    } catch (error) {
        console.error('Error saving receita:', error);
        showErrorMessage('Erro ao salvar receita. Tente novamente.');
    }
}

async function handleDeleteReceita(receitaId) {
    try {
        await api.deleteIncome(receitaId);
        showSuccessMessage('Receita excluída com sucesso!');
        
        // Refresh data
        await loadReceitas();
        updateReceitasSummary();
        
    } catch (error) {
        console.error('Error deleting receita:', error);
        showErrorMessage('Erro ao excluir receita. Tente novamente.');
    }
}