// Authentication functionality

// Initialize auth page
document.addEventListener('DOMContentLoaded', function() {
    initializeAuthPage();
});

function initializeAuthPage() {
    const currentPage = window.location.pathname.split('/').pop();
    
    switch(currentPage) {
        case 'login.html':
            initializeLoginPage();
            break;
        case 'cadastro.html':
            initializeCadastroPage();
            break;
        case 'esqueci-senha.html':
            initializeEsqueciSenhaPage();
            break;
        case 'recuperar-senha.html':
            initializeRecuperarSenhaPage();
            break;
    }
}

// Login Page
function initializeLoginPage() {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }
    
    // Check if user is already logged in
    if (isLoggedIn()) {
        window.location.href = 'index.html';
    }
}
async function handleLogin(event) {
    event.preventDefault();

    const form = event.target;
    const formData = new FormData(form);
    const submitBtn = form.querySelector('button[type="submit"]');

    // Ativar estado de carregamento
    setButtonLoading(submitBtn, true);

    try {
        const loginData = {
            email: formData.get('email'),
            senha: formData.get('senha')
        };

        // URL corrigida para o endpoint correto
        const response = await fetch('https://localhost:5001/usuario/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        });

        if (!response.ok) {
            throw new Error('Erro ao fazer login');
        }

        const result = await response.json();

        const lembrarMe = formData.get('lembrarMe') === 'on';

        // Ajuste das propriedades para maiúsculas conforme seu backend
        saveLoginState({
            email: loginData.email,
            token: result.Token,        
            expiracao: result.Expiracao 
        }, lembrarMe);

        window.location.href = 'index.html';

    } catch (error) {
        console.error('Erro no login:', error);
        showAuthError('Email ou senha incorretos.');
    } finally {
        setButtonLoading(submitBtn, false);
    }
}


// Cadastro Page
function initializeCadastroPage() {
    const cadastroForm = document.getElementById('cadastroForm');
    if (cadastroForm) {
        cadastroForm.addEventListener('submit', handleCadastro);
    }
    
    // Password strength checker
    const senhaInput = document.getElementById('senha');
    if (senhaInput) {
        senhaInput.addEventListener('input', checkPasswordStrength);
    }
    
    // Phone mask
    const telefoneInput = document.getElementById('telefone');
    if (telefoneInput) {
        telefoneInput.addEventListener('input', formatPhone);
    }
}

async function handleCadastro(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    const submitBtn = form.querySelector('button[type="submit"]');
    
    // Validate passwords match
    const senha = formData.get('senha');
    const confirmarSenha = formData.get('confirmarSenha');
    
    if (senha !== confirmarSenha) {
        showAuthError('As senhas não coincidem.');
        return;
    }
    
    // Show loading state
    setButtonLoading(submitBtn, true);
    
    try {
        const userData = {
            nome: formData.get('nome'),
            email: formData.get('email'),
            senha: formData.get('senha'),
            telefone: formData.get('telefone'),
            dataNascimento: formData.get('dataNascimento')
        };
        
        // Create user via API
        await api.createUser(userData);
        
        // Show success and redirect
        showAuthSuccess('Conta criada com sucesso! Redirecionando...');
        
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 2000);
        
    } catch (error) {
        console.error('Cadastro error:', error);
        showAuthError('Erro ao criar conta. Tente novamente.');
    } finally {
        setButtonLoading(submitBtn, false);
    }
}

// Esqueci Senha Page
function initializeEsqueciSenhaPage() {
    const esqueceuSenhaForm = document.getElementById('esqueceuSenhaForm');
    if (esqueceuSenhaForm) {
        esqueceuSenhaForm.addEventListener('submit', handleEsqueciSenha);
    }
}

async function handleEsqueciSenha(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    const submitBtn = form.querySelector('button[type="submit"]');
    
    // Show loading state
    setButtonLoading(submitBtn, true);
    
    try {
        const email = formData.get('email');
        
        // Simulate sending recovery email
        await simulatePasswordRecovery(email);
        
        // Show success modal
        const successModal = document.getElementById('successModal');
        if (successModal) {
            successModal.classList.add('active');
        }
        
    } catch (error) {
        console.error('Password recovery error:', error);
        showAuthError('Erro ao enviar email de recuperação. Tente novamente.');
    } finally {
        setButtonLoading(submitBtn, false);
    }
}

// Recuperar Senha Page
function initializeRecuperarSenhaPage() {
    const recuperarSenhaForm = document.getElementById('recuperarSenhaForm');
    if (recuperarSenhaForm) {
        recuperarSenhaForm.addEventListener('submit', handleRecuperarSenha);
    }
    
    // Password strength checker
    const novaSenhaInput = document.getElementById('novaSenha');
    if (novaSenhaInput) {
        novaSenhaInput.addEventListener('input', function() {
            checkPasswordStrength.call(this);
        });
    }
}

async function handleRecuperarSenha(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    const submitBtn = form.querySelector('button[type="submit"]');
    
    // Validate passwords match
    const novaSenha = formData.get('novaSenha');
    const confirmarNovaSenha = formData.get('confirmarNovaSenha');
    
    if (novaSenha !== confirmarNovaSenha) {
        showAuthError('As senhas não coincidem.');
        return;
    }
    
    // Show loading state
    setButtonLoading(submitBtn, true);
    
    try {
        // Get token from URL
        const urlParams = new URLSearchParams(window.location.search);
        const token = urlParams.get('token');
        
        if (!token) {
            throw new Error('Token de recuperação inválido');
        }
        
        // Reset password
        await simulatePasswordReset(token, novaSenha);
        
        // Show success and redirect
        showAuthSuccess('Senha redefinida com sucesso! Redirecionando...');
        
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 2000);
        
    } catch (error) {
        console.error('Password reset error:', error);
        showAuthError('Erro ao redefinir senha. Tente novamente.');
    } finally {
        setButtonLoading(submitBtn, false);
    }
}

// Utility Functions
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

function checkPasswordStrength() {
    const password = this.value;
    const strengthBar = this.parentElement.parentElement.querySelector('.strength-fill');
    const strengthText = this.parentElement.parentElement.querySelector('.strength-text');
    
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

function setButtonLoading(button, loading) {
    const btnText = button.querySelector('.btn-text');
    const btnLoading = button.querySelector('.btn-loading');
    
    if (loading) {
        button.classList.add('loading');
        button.disabled = true;
        if (btnText) btnText.style.display = 'none';
        if (btnLoading) btnLoading.style.display = 'flex';
    } else {
        button.classList.remove('loading');
        button.disabled = false;
        if (btnText) btnText.style.display = 'inline';
        if (btnLoading) btnLoading.style.display = 'none';
    }
}

function showAuthError(message) {
    // Remove existing error messages
    const existingErrors = document.querySelectorAll('.auth-error');
    existingErrors.forEach(error => error.remove());
    
    // Create error element
    const errorDiv = document.createElement('div');
    errorDiv.className = 'auth-error alert alert-error';
    errorDiv.innerHTML = `
        <i class="fas fa-exclamation-circle"></i>
        ${message}
    `;
    
    // Insert before form
    const form = document.querySelector('.auth-form');
    if (form) {
        form.parentNode.insertBefore(errorDiv, form);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            errorDiv.remove();
        }, 5000);
    }
}

function showAuthSuccess(message) {
    // Remove existing messages
    const existingMessages = document.querySelectorAll('.auth-error, .auth-success');
    existingMessages.forEach(msg => msg.remove());
    
    // Create success element
    const successDiv = document.createElement('div');
    successDiv.className = 'auth-success alert alert-success';
    successDiv.innerHTML = `
        <i class="fas fa-check-circle"></i>
        ${message}
    `;
    
    // Insert before form
    const form = document.querySelector('.auth-form');
    if (form) {
        form.parentNode.insertBefore(successDiv, form);
    }
}

// Authentication state management
function saveLoginState(userData, remember) {
    const storage = remember ? localStorage : sessionStorage;
    storage.setItem('finon_user', JSON.stringify(userData));
}


function isLoggedIn() {
    const user = getCurrentUser();
    return !!(user && user.token);
}


function getCurrentUser() {
    const sessionUser = sessionStorage.getItem('finon_user');
    const localUser = localStorage.getItem('finon_user');
    
    const userStr = sessionUser || localUser;
    return userStr ? JSON.parse(userStr) : null;
}

function logout() {
    sessionStorage.removeItem('finon_user');
    localStorage.removeItem('finon_user');
    window.location.href = 'login.html';
}

// Simulate API calls (replace with actual API integration)
async function simulateLogin(loginData) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            // Simulate successful login
            if (loginData.email && loginData.senha) {
                resolve({ success: true });
            } else {
                reject(new Error('Invalid credentials'));
            }
        }, 1500);
    });
}

async function simulatePasswordRecovery(email) {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve({ success: true });
        }, 1500);
    });
}

async function simulatePasswordReset(token, newPassword) {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve({ success: true });
        }, 1500);
    });
}

// Export functions for use in other files
window.authFunctions = {
    togglePassword,
    checkPasswordStrength,
    formatPhone,
    isLoggedIn,
    getCurrentUser,
    logout
};