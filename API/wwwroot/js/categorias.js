// Categorias page functionality

let allCategorias = [];
let editingCategoria = null;
let deletingCategoriaId = null;

// Initialize categorias page
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname.includes('categorias.html')) {
        initializeCategoriasPage();
    }
});

async function initializeCategoriasPage() {
    try {
        // Load initial data
        await loadCategorias();
        
        // Setup event listeners
        setupCategoriasEventListeners();
        
        // Display categorias
        displayCategorias();
        displayCategoriasGrid();
        
    } catch (error) {
        console.error('Error initializing categorias page:', error);
        showErrorMessage('Erro ao carregar página de categorias');
    }
}

async function loadCategorias() {
    try {
        allCategorias = await api.getCategories();
    } catch (error) {
        console.error('Error loading categorias:', error);
        allCategorias = [];
    }
}

function displayCategorias() {
    const tableBody = document.getElementById('categoriasTableBody');
    if (!tableBody) return;
    
    if (allCategorias.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center">
                    <div class="page-empty-state">
                        <i class="fas fa-tags"></i>
                        <h3>Nenhuma categoria encontrada</h3>
                        <p>Crie sua primeira categoria para organizar suas transações</p>
                        <button class="btn btn-primary" onclick="openCategoriaModal()">
                            <i class="fas fa-plus"></i>
                            Nova Categoria
                        </button>
                    </div>
                </td>
            </tr>
        `;
        return;
    }
    
    tableBody.innerHTML = allCategorias.map(categoria => `
        <tr>
            <td>${categoria.nome}</td>
            <td>${categoria.descricao || '-'}</td>
            <td class="text-center">
                <span class="status-badge active">${getRandomUsage()}</span>
            </td>
            <td class="text-center">
                <span class="status-badge active">${getRandomUsage()}</span>
            </td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editCategoria(${categoria.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="confirmDeleteCategoria(${categoria.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

function displayCategoriasGrid() {
    const grid = document.getElementById('categoriesGrid');
    if (!grid) return;
    
    if (allCategorias.length === 0) {
        grid.innerHTML = '';
        return;
    }
    
    grid.innerHTML = allCategorias.map(categoria => `
        <div class="category-card">
            <div class="category-header">
                <div class="category-info">
                    <h3>${categoria.nome}</h3>
                    <p>${categoria.descricao || 'Sem descrição'}</p>
                </div>
                <div class="category-actions">
                    <button class="action-btn edit-btn" onclick="editCategoria(${categoria.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="confirmDeleteCategoria(${categoria.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
            <div class="category-stats">
                <div class="stat-item">
                    <span class="number">${getRandomUsage()}</span>
                    <span class="label">Receitas</span>
                </div>
                <div class="stat-item">
                    <span class="number">${getRandomUsage()}</span>
                    <span class="label">Despesas</span>
                </div>
            </div>
        </div>
    `).join('');
}

function getRandomUsage() {
    return Math.floor(Math.random() * 20);
}

function setupCategoriasEventListeners() {
    // Form submission
    const categoriaForm = document.getElementById('categoriaForm');
    if (categoriaForm) {
        categoriaForm.addEventListener('submit', handleCategoriaSubmit);
    }
    
    // Search functionality
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', UTILS.debounce(handleSearch, 300));
    }
}

function handleSearch(event) {
    const searchTerm = event.target.value.toLowerCase();
    const filteredCategorias = allCategorias.filter(categoria => 
        categoria.nome.toLowerCase().includes(searchTerm) ||
        (categoria.descricao && categoria.descricao.toLowerCase().includes(searchTerm))
    );
    
    displayFilteredCategorias(filteredCategorias);
}

function displayFilteredCategorias(categorias) {
    const tableBody = document.getElementById('categoriasTableBody');
    if (!tableBody) return;
    
    if (categorias.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center">
                    <div class="empty-state">
                        <i class="fas fa-search"></i>
                        <h3>Nenhuma categoria encontrada</h3>
                        <p>Tente ajustar os termos de busca</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }
    
    tableBody.innerHTML = categorias.map(categoria => `
        <tr>
            <td>${categoria.nome}</td>
            <td>${categoria.descricao || '-'}</td>
            <td class="text-center">
                <span class="status-badge active">${getRandomUsage()}</span>
            </td>
            <td class="text-center">
                <span class="status-badge active">${getRandomUsage()}</span>
            </td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn edit-btn" onclick="editCategoria(${categoria.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete-btn" onclick="confirmDeleteCategoria(${categoria.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

// Global functions for HTML onclick events
window.openCategoriaModal = function(categoriaId = null) {
    const modal = document.getElementById('modalOverlay');
    const modalTitle = document.getElementById('modalTitle');
    const form = document.getElementById('categoriaForm');
    
    if (!modal || !modalTitle || !form) return;
    
    // Reset form
    form.reset();
    clearFormErrors(form);
    editingCategoria = null;
    
    if (categoriaId) {
        // Edit mode
        const categoria = allCategorias.find(c => c.id === categoriaId);
        if (categoria) {
            modalTitle.textContent = 'Editar Categoria';
            editingCategoria = categoria;
            
            // Populate form
            document.getElementById('nome').value = categoria.nome;
            document.getElementById('descricao').value = categoria.descricao || '';
        }
    } else {
        // Create mode
        modalTitle.textContent = 'Nova Categoria';
    }
    
    // Show modal
    openModal();
};

window.editCategoria = function(categoriaId) {
    openCategoriaModal(categoriaId);
};

window.confirmDeleteCategoria = function(categoriaId) {
    deletingCategoriaId = categoriaId;
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
    deletingCategoriaId = null;
};

window.confirmDelete = function() {
    if (deletingCategoriaId) {
        handleDeleteCategoria(deletingCategoriaId);
    }
};

async function handleCategoriaSubmit(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    if (!validateForm(form)) {
        return;
    }
    
    try {
        const categoriaData = {
            nome: formData.get('nome'),
            descricao: formData.get('descricao') || null
        };
        
        if (editingCategoria) {
            // Update existing categoria
            await api.updateCategory(editingCategoria.id, categoriaData);
            showSuccessMessage('Categoria atualizada com sucesso!');
        } else {
            // Create new categoria
            await api.createCategory(categoriaData);
            showSuccessMessage('Categoria criada com sucesso!');
        }
        
        // Close modal and refresh data
        closeModal();
        await loadCategorias();
        
        // Update global categories array
        categories = allCategorias;
        
        displayCategorias();
        displayCategoriasGrid();
        
    } catch (error) {
        console.error('Error saving categoria:', error);
        showErrorMessage('Erro ao salvar categoria. Tente novamente.');
    }
}

async function handleDeleteCategoria(categoriaId) {
    try {
        await api.deleteCategory(categoriaId);
        showSuccessMessage('Categoria excluída com sucesso!');
        
        // Close delete modal
        closeDeleteModal();
        
        // Refresh data
        await loadCategorias();
        
        // Update global categories array
        categories = allCategorias;
        
        displayCategorias();
        displayCategoriasGrid();
        
    } catch (error) {
        console.error('Error deleting categoria:', error);
        showErrorMessage('Erro ao excluir categoria. Tente novamente.');
        closeDeleteModal();
    }
}