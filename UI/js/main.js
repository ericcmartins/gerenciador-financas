// Main JavaScript file for common functionality

// Global variables
let currentUser = null;
let categories = [];
let accounts = [];
let paymentMethods = [];

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});

// Initialize application
async function initializeApp() {
    try {
        // Check authentication for protected pages
        if (!isAuthPage() && !isLoggedIn()) {
            window.location.href = 'login.html';
            return;
        }
        
        // Setup event listeners
        setupEventListeners();
        
        // Load initial data for authenticated pages
        if (!isAuthPage()) {
            await loadInitialData();
        }
        
        // Setup sidebar toggle for mobile
        setupMobileMenu();
        
        console.log('Application initialized successfully');
    } catch (error) {
        console.error('Error initializing application:', error);
        showErrorMessage('Erro ao inicializar a aplicação. Verifique sua conexão.');
    }
}

// Check if current page is an auth page
function isAuthPage() {
    const authPages = ['login.html', 'cadastro.html', 'esqueci-senha.html', 'recuperar-senha.html'];
    const currentPage = window.location.pathname.split('/').pop();
    return authPages.includes(currentPage);
}

// Check if user is logged in
function isLoggedIn() {
    const sessionUser = sessionStorage.getItem('finon_user');
    const localUser = localStorage.getItem('finon_user');
    return !!(sessionUser || localUser);
}

// Setup common event listeners
function setupEventListeners() {
    // Menu toggle for mobile
    const menuToggle = document.getElementById('menuToggle');
    if (menuToggle) {
        menuToggle.addEventListener('click', toggleSidebar);
    }

    // Close modals when clicking outside
    document.addEventListener('click', function(event) {
        if (event.target.classList.contains('modal-overlay')) {
            closeModal();
        }
    });

    // Close modals with Escape key
    document.addEventListener('keydown', function(event) {
        if (event.key === 'Escape') {
            closeModal();
            closeDeleteModal();
        }
    });

    // Form validation on submit
    document.addEventListener('submit', function(event) {
        const form = event.target;
        if (form.tagName === 'FORM') {
            if (!validateForm(form)) {
                event.preventDefault();
            }
        }
    });

    // User menu toggle
    document.addEventListener('click', function(event) {
        const userMenu = document.getElementById('userMenu');
        if (userMenu && !event.target.closest('.user-info')) {
            userMenu.classList.remove('active');
        }
    });
}

// Load initial data needed across the application
async function loadInitialData() {
    try {
        // Load user data
        currentUser = await api.getUserData().catch(() => null);
        
        // Load categories
        categories = await api.getCategories().catch(() => []);
        
        // Load accounts
        accounts = await api.getAccounts().catch(() => []);
        
        // Load payment methods
        paymentMethods = await api.getPaymentMethods().catch(() => []);
        
        // Update UI with user info
        updateUserInfo();
        
    } catch (error) {
        console.error('Error loading initial data:', error);
        // Continue with empty data if API is not available
        categories = [];
        accounts = [];
        paymentMethods = [];
    }
}

// Update user information in the UI
function updateUserInfo() {
    const userNameElements = document.querySelectorAll('.user-name');
    userNameElements.forEach(element => {
        element.textContent = currentUser?.nome || CONFIG.USER.NAME;
    });
}

// Setup mobile menu functionality
function setupMobileMenu() {
    const sidebar = document.querySelector('.sidebar');
    const mainContent = document.querySelector('.main-content');
    
    // Close sidebar when clicking on main content on mobile
    if (mainContent) {
        mainContent.addEventListener('click', function() {
            if (window.innerWidth <= 768 && sidebar && sidebar.classList.contains('active')) {
                sidebar.classList.remove('active');
            }
        });
    }
}

// Toggle sidebar for mobile
function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    if (sidebar) {
        sidebar.classList.toggle('active');
    }
}

// Toggle user menu
function toggleUserMenu() {
    const userMenu = document.getElementById('userMenu');
    if (userMenu) {
        userMenu.classList.toggle('active');
    }
}

// Logout function
function logout() {
    if (confirm('Tem certeza que deseja sair?')) {
        sessionStorage.removeItem('finon_user');
        localStorage.removeItem('finon_user');
        window.location.href = 'login.html';
    }
}

// Modal functions
function openModal(modalId = 'modalOverlay') {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(modalId = 'modalOverlay') {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('active');
        document.body.style.overflow = '';
        
        // Reset form if exists
        const form = modal.querySelector('form');
        if (form) {
            form.reset();
            clearFormErrors(form);
        }
    }
}

function closeDeleteModal() {
    closeModal('deleteModalOverlay');
}

// Form validation
function validateForm(form) {
    clearFormErrors(form);
    
    const requiredFields = form.querySelectorAll('[required]');
    let isValid = true;
    
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            showFieldError(field, 'Este campo é obrigatório');
            isValid = false;
        }
    });
    
    // Validate email fields
    const emailFields = form.querySelectorAll('input[type="email"]');
    emailFields.forEach(field => {
        if (field.value && !UTILS.validateEmail(field.value)) {
            showFieldError(field, 'Email inválido');
            isValid = false;
        }
    });
    
    // Validate number fields
    const numberFields = form.querySelectorAll('input[type="number"]');
    numberFields.forEach(field => {
        if (field.hasAttribute('min') && field.value && parseFloat(field.value) < parseFloat(field.getAttribute('min'))) {
            showFieldError(field, `Valor deve ser maior que ${field.getAttribute('min')}`);
            isValid = false;
        }
    });
    
    return isValid;
}

function showFieldError(field, message) {
    field.classList.add('error');
    
    // Remove existing error message
    const existingError = field.parentNode.querySelector('.field-error');
    if (existingError) {
        existingError.remove();
    }
    
    // Add new error message
    const errorElement = document.createElement('div');
    errorElement.className = 'field-error';
    errorElement.textContent = message;
    errorElement.style.color = '#ef4444';
    errorElement.style.fontSize = '0.8rem';
    errorElement.style.marginTop = '0.25rem';
    
    field.parentNode.appendChild(errorElement);
}

function clearFormErrors(form) {
    // Remove error classes
    const errorFields = form.querySelectorAll('.error');
    errorFields.forEach(field => {
        field.classList.remove('error');
    });
    
    // Remove error messages
    const errorMessages = form.querySelectorAll('.field-error');
    errorMessages.forEach(message => {
        message.remove();
    });
}

// Utility functions for UI
function showErrorMessage(message, duration = 5000) {
    showNotification(message, 'error', duration);
}

function showSuccessMessage(message, duration = 3000) {
    showNotification(message, 'success', duration);
}

function showNotification(message, type = 'info', duration = 3000) {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.notification');
    existingNotifications.forEach(notification => notification.remove());
    
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `notification alert alert-${type}`;
    notification.innerHTML = `
        <i class="fas fa-${getNotificationIcon(type)}"></i>
        <span>${message}</span>
        <button class="close-notification" onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;
    
    // Style the notification
    notification.style.position = 'fixed';
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '9999';
    notification.style.minWidth = '300px';
    notification.style.maxWidth = '500px';
    notification.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.15)';
    notification.style.borderRadius = '8px';
    notification.style.animation = 'slideInRight 0.3s ease';
    
    // Add to document
    document.body.appendChild(notification);
    
    // Auto remove after duration
    if (duration > 0) {
        setTimeout(() => {
            if (notification.parentElement) {
                notification.style.animation = 'slideOutRight 0.3s ease';
                setTimeout(() => notification.remove(), 300);
            }
        }, duration);
    }
}

function getNotificationIcon(type) {
    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };
    return icons[type] || 'info-circle';
}

// Loading states
function showLoading(element, message = 'Carregando...') {
    if (element) {
        element.innerHTML = `
            <div class="loading">
                <div class="spinner"></div>
                <p>${message}</p>
            </div>
        `;
    }
}

function hideLoading(element) {
    if (element) {
        const loading = element.querySelector('.loading');
        if (loading) {
            loading.remove();
        }
    }
}

// Data formatting helpers
function formatCurrency(value) {
    return UTILS.formatCurrency(value);
}

function formatDate(date) {
    return UTILS.formatDate(date);
}

function formatDateForInput(date) {
    return UTILS.formatDateForInput(date);
}

// Populate select options
function populateSelect(selectElement, options, valueKey = 'id', textKey = 'nome', placeholder = 'Selecione...') {
    if (!selectElement) return;
    
    // Clear existing options except the first one (placeholder)
    selectElement.innerHTML = `<option value="">${placeholder}</option>`;
    
    // Add new options
    options.forEach(option => {
        const optionElement = document.createElement('option');
        optionElement.value = option[valueKey];
        optionElement.textContent = option[textKey];
        selectElement.appendChild(optionElement);
    });
}

// Populate categories in select elements
function populateCategories(selectElement, placeholder = 'Selecione uma categoria') {
    populateSelect(selectElement, categories, 'id', 'nome', placeholder);
}

// Populate accounts in select elements
function populateAccounts(selectElement, placeholder = 'Selecione uma conta') {
    populateSelect(selectElement, accounts, 'id', 'numeroConta', placeholder);
}

// Populate payment methods in select elements
function populatePaymentMethods(selectElement, placeholder = 'Selecione um método') {
    populateSelect(selectElement, paymentMethods, 'id', 'nome', placeholder);
}

// Export functions for use in other files
window.FinOnApp = {
    openModal,
    closeModal,
    closeDeleteModal,
    showErrorMessage,
    showSuccessMessage,
    showNotification,
    showLoading,
    hideLoading,
    formatCurrency,
    formatDate,
    formatDateForInput,
    populateCategories,
    populateAccounts,
    populatePaymentMethods,
    validateForm,
    toggleUserMenu,
    logout
};

// Make functions globally available
window.toggleUserMenu = toggleUserMenu;
window.logout = logout;

// Add CSS for notifications and user menu
const additionalStyles = `
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
    
    .notification {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 1rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }
    
    .close-notification {
        background: none;
        border: none;
        color: inherit;
        cursor: pointer;
        padding: 0.25rem;
        border-radius: 4px;
        margin-left: auto;
        opacity: 0.7;
        transition: opacity 0.3s ease;
    }
    
    .close-notification:hover {
        opacity: 1;
    }
    
    .field-error {
        color: #ef4444;
        font-size: 0.8rem;
        margin-top: 0.25rem;
    }
    
    .form-group input.error,
    .form-group select.error,
    .form-group textarea.error {
        border-color: #ef4444;
        box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
    }
    
    .user-menu {
        position: absolute;
        top: 100%;
        right: 0;
        background: white;
        border: 1px solid #e5e7eb;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        min-width: 180px;
        z-index: 1000;
        opacity: 0;
        visibility: hidden;
        transform: translateY(-10px);
        transition: all 0.3s ease;
    }
    
    .user-menu.active {
        opacity: 1;
        visibility: visible;
        transform: translateY(0);
    }
    
    .user-menu a {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0.75rem 1rem;
        color: #374151;
        text-decoration: none;
        transition: background-color 0.3s ease;
    }
    
    .user-menu a:hover {
        background-color: #f3f4f6;
    }
    
    .user-menu a:first-child {
        border-radius: 8px 8px 0 0;
    }
    
    .user-menu a:last-child {
        border-radius: 0 0 8px 8px;
    }
    
    .user-info {
        position: relative;
    }
    
    .user-avatar {
        cursor: pointer;
    }
`;

// Add styles to document
const styleSheet = document.createElement('style');
styleSheet.textContent = additionalStyles;
document.head.appendChild(styleSheet);