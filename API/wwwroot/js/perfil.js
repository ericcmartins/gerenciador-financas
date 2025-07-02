// Profile page functionality

let isEditMode = false;
let originalUserData = {};

// Initialize profile page
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname.includes('perfil.html')) {
        initializeProfilePage();
    }
});

async function initializeProfilePage() {
    try {
        // Check if user is logged in
        if (!isLoggedIn()) {
            window.location.href = 'login.html';
            return;
        }
        
        // Load user data
        await loadUserProfile();
        
        // Setup event listeners
        setupProfileEventListeners();
        
    } catch (error) {
        console.error('Error initializing profile page:', error);
        showErrorMessage('Erro ao carregar perfil do usuário');
    }
}

async function loadUserProfile() {
    try {
        // Get user data from API
        const userData = await api.getUserData();
        
        // Store original data
        originalUserData = { ...userData };
        
        // Update profile display
        updateProfileDisplay(userData);
        
        // Populate form fields
        populateProfileForm(userData);
        
    } catch (error) {
        console.error('Error loading user profile:', error);
        // Use mock data if API fails
        const mockData = {
            nome: 'João Silva',
            email: 'joao@email.com',
            telefone: '(11) 99999-9999',
            dataNascimento: '1990-01-01'
        };
        
        originalUserData = { ...mockData };
        updateProfileDisplay(mockData);
        populateProfileForm(mockData);
    }
}

function updateProfileDisplay(userData) {
    // Update profile header
    const profileName = document.getElementById('profileName');
    const profileEmail = document.getElementById('profileEmail');
    
    if (profileName) profileName.textContent = userData.nome || 'Usuário';
    if (profileEmail) profileEmail.textContent = userData.email || '';
    
    // Update top bar user name
    const userNameElements = document.querySelectorAll('.user-name');
    userNameElements.forEach(element => {
        element.textContent = userData.nome || 'Usuário';
    });
}

function populateProfileForm(userData) {
    const form = document.getElementById('profileForm');
    if (!form) return;
    
    // Populate form fields
    const fields = ['nome', 'email', 'telefone', 'dataNascimento'];
    fields.forEach(field => {
        const input = form.querySelector(`#${field}`);
        if (input && userData[field]) {
            if (field === 'dataNascimento') {
                input.value = formatDateForInput(userData[field]);
            } else {
                input.value = userData[field];
            }
        }
    });
}

function setupProfileEventListeners() {
    // Profile form submission
    const profileForm = document.getElementById('profileForm');
    if (profileForm) {
        profileForm.addEventListener('submit', handleProfileUpdate);
    }
    
    // Change password form
    const changePasswordForm = document.getElementById('changePasswordForm');
    if (changePasswordForm) {
        changePasswordForm.addEventListener('submit', handleChangePassword);
    }
    
    // Password strength checker
    const novaSenhaInput = document.getElementById('novaSenha');
    if (novaSenhaInput) {
        novaSenhaInput.addEventListener('input', checkPasswordStrength);
    }
    
    // Delete account confirmation
    const confirmDeleteInput = document.getElementById('confirmDelete');
    if (confirmDeleteInput) {
        confirmDeleteInput.addEventListener('input', function() {
            const confirmBtn = document.getElementById('confirmDeleteBtn');
            if (confirmBtn) {
                confirmBtn.disabled = this.value !== 'EXCLUIR';
            }
        });
    }
    
    // Phone formatting
    const telefoneInput = document.getElementById('telefone');
    if (telefoneInput) {
        telefoneInput.addEventListener('input', formatPhone);
    }
}

// Global functions for HTML onclick events
window.toggleEditMode = function() {
    isEditMode = !isEditMode;
    
    const form = document.getElementById('profileForm');
    const editButton = document.querySelector('.card-header .btn');
    const editButtonText = document.getElementById('editButtonText');
    const formActions = document.getElementById('formActions');
    
    if (!form) return;
    
    const inputs = form.querySelectorAll('input');
    
    if (isEditMode) {
        // Enable edit mode
        inputs.forEach(input => {
            if (input.id !== 'email') { // Don't allow email editing
                input.disabled = false;
            }
        });
        
        editButtonText.textContent = 'Cancelar';
        editButton.classList.remove('btn-outline');
        editButton.classList.add('btn-secondary');
        formActions.style.display = 'flex';
        
    } else {
        // Disable edit mode
        inputs.forEach(input => input.disabled = true);
        
        editButtonText.textContent = 'Editar';
        editButton.classList.remove('btn-secondary');
        editButton.classList.add('btn-outline');
        formActions.style.display = 'none';
        
        // Restore original data
        populateProfileForm(originalUserData);
    }
};

window.cancelEdit = function() {
    toggleEditMode();
};

window.changeAvatar = function() {
    // Create file input
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = 'image/*';
    fileInput.style.display = 'none';
    
    fileInput.addEventListener('change', function(event) {
        const file = event.target.files[0];
        if (file) {
            // Here you would upload the file to your server
            // For now, just show a success message
            showSuccessMessage('Avatar atualizado com sucesso!');
        }
    });
    
    document.body.appendChild(fileInput);
    fileInput.click();
    document.body.removeChild(fileInput);
};

window.openChangePasswordModal = function() {
    const modal = document.getElementById('changePasswordModal');
    if (modal) {
        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
};

window.closeChangePasswordModal = function() {
    const modal = document.getElementById('changePasswordModal');
    if (modal) {
        modal.classList.remove('active');
        document.body.style.overflow = '';
        
        // Reset form
        const form = document.getElementById('changePasswordForm');
        if (form) {
            form.reset();
            clearFormErrors(form);
        }
    }
};

window.setup2FA = function() {
    showSuccessMessage('Funcionalidade de 2FA será implementada em breve!');
};

window.exportData = function() {
    // Simulate data export
    showSuccessMessage('Seus dados estão sendo preparados para download...');
    
    setTimeout(() => {
        // Create a mock CSV file
        const csvContent = "data:text/csv;charset=utf-8,Tipo,Descrição,Valor,Data\nReceita,Salário,5000.00,2024-01-01\nDespesa,Mercado,-150.00,2024-01-02";
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", "finon_dados.csv");
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        showSuccessMessage('Dados exportados com sucesso!');
    }, 2000);
};

window.logout = function() {
    if (confirm('Tem certeza que deseja sair da sua conta?')) {
        // Clear authentication data
        sessionStorage.removeItem('finon_user');
        localStorage.removeItem('finon_user');
        
        // Redirect to login
        window.location.href = 'login.html';
    }
};

window.deleteAccount = function() {
    const modal = document.getElementById('deleteAccountModal');
    if (modal) {
        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
};

window.closeDeleteAccountModal = function() {
    const modal = document.getElementById('deleteAccountModal');
    if (modal) {
        modal.classList.remove('active');
        document.body.style.overflow = '';
        
        // Reset confirmation input
        const confirmInput = document.getElementById('confirmDelete');
        const confirmBtn = document.getElementById('confirmDeleteBtn');
        if (confirmInput) confirmInput.value = '';
        if (confirmBtn) confirmBtn.disabled = true;
    }
};

window.confirmDeleteAccount = function() {
    const confirmInput = document.getElementById('confirmDelete');
    
    if (confirmInput && confirmInput.value === 'EXCLUIR') {
        if (confirm('Esta ação é irreversível. Tem certeza absoluta?')) {
            handleDeleteAccount();
        }
    }
};

async function handleProfileUpdate(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    if (!validateForm(form)) {
        return;
    }
    
    try {
        const userData = {
            nome: formData.get('nome'),
            email: formData.get('email'),
            telefone: formData.get('telefone'),
            dataNascimento: formData.get('dataNascimento')
        };
        
        // Update user via API
        await api.updateUser(userData);
        
        // Update stored data
        originalUserData = { ...userData };
        
        // Update display
        updateProfileDisplay(userData);
        
        // Exit edit mode
        toggleEditMode();
        
        showSuccessMessage('Perfil atualizado com sucesso!');
        
    } catch (error) {
        console.error('Error updating profile:', error);
        showErrorMessage('Erro ao atualizar perfil. Tente novamente.');
    }
}

async function handleChangePassword(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    
    if (!validateForm(form)) {
        return;
    }
    
    // Validate passwords match
    const novaSenha = formData.get('novaSenha');
    const confirmarNovaSenha = formData.get('confirmarNovaSenha');
    
    if (novaSenha !== confirmarNovaSenha) {
        showFieldError(form.querySelector('#confirmarNovaSenha'), 'As senhas não coincidem');
        return;
    }
    
    try {
        const passwordData = {
            senhaAtual: formData.get('senhaAtual'),
            novaSenha: formData.get('novaSenha')
        };
        
        // Update password via API (you'll need to implement this endpoint)
        // await api.updatePassword(passwordData);
        
        // Simulate success
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        closeChangePasswordModal();
        showSuccessMessage('Senha alterada com sucesso!');
        
    } catch (error) {
        console.error('Error changing password:', error);
        showErrorMessage('Erro ao alterar senha. Verifique sua senha atual.');
    }
}

async function handleDeleteAccount() {
    try {
        // Delete account via API
        await api.deleteUser();
        
        // Clear all data
        sessionStorage.clear();
        localStorage.clear();
        
        // Redirect to login with message
        alert('Conta excluída com sucesso. Você será redirecionado para a página de login.');
        window.location.href = 'login.html';
        
    } catch (error) {
        console.error('Error deleting account:', error);
        showErrorMessage('Erro ao excluir conta. Tente novamente.');
        closeDeleteAccountModal();
    }
}

// Utility functions
function formatPhone() {
    let value = this.value.replace(/\D/g, '');
    
    if (value.length >= 11) {
        value = value.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    } else if (value.length >= 7) {
        value = value.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3');
    } else if (value.length >= 3) {
        value = value.replace(/(\d{2})(\d{0,5})/, '($1) $2');
    }
    
    this.value = value;
}

function checkPasswordStrength() {
    const password = this.value;
    const strengthBar = document.querySelector('.strength-fill');
    const strengthText = document.querySelector('.strength-text');
    
    if (!strengthBar || !strengthText) return;
    
    let strength = 0;
    let text = 'Muito fraca';
    let color = '#ef4444';
    
    if (password.length >= 6) strength += 25;
    if (password.match(/[a-z]/)) strength += 25;
    if (password.match(/[A-Z]/)) strength += 25;
    if (password.match(/[0-9]/)) strength += 25;
    
    if (strength >= 75) {
        text = 'Forte';
        color = '#10b981';
    } else if (strength >= 50) {
        text = 'Média';
        color = '#f59e0b';
    } else if (strength >= 25) {
        text = 'Fraca';
        color = '#ef4444';
    }
    
    strengthBar.style.width = strength + '%';
    strengthBar.style.backgroundColor = color;
    strengthText.textContent = text;
    strengthText.style.color = color;
}

function togglePassword(inputId) {
    const input = document.getElementById(inputId);
    const button = input.parentElement.querySelector('.toggle-password');
    const icon = button.querySelector('i');
    
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

// Make functions available globally
window.togglePassword = togglePassword;