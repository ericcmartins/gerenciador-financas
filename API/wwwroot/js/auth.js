import { api } from './api.js';
import { CONFIG } from './config.js';

// Executa o nosso código apenas quando o HTML da página estiver completamente carregado.
document.addEventListener('DOMContentLoaded', () => {

    // --- Roteador de Página ---
    // Verifica qual formulário está na página atual e inicia a lógica correspondente.
    const formularioLogin = document.getElementById('formularioLogin');
    if (formularioLogin) {
        iniciarPaginaLogin();
    }

    const formularioCadastro = document.getElementById('formularioCadastro');
    if (formularioCadastro) {
        iniciarPaginaCadastro();
    }
});


// --- LÓGICA DA PÁGINA DE LOGIN ---

function iniciarPaginaLogin() {
    const formulario = document.getElementById('formularioLogin');
    const inputEmail = document.getElementById('email');
    const inputSenha = document.getElementById('senha');
    const botaoSubmit = formulario.querySelector('button[type="submit"]');

    formulario.addEventListener('submit', async (evento) => {
        evento.preventDefault();
        removerMensagemErro(formulario);

        const email = inputEmail.value.trim();
        const senha = inputSenha.value.trim();

        if (!email || !senha) {
            mostrarMensagemErro('Por favor, preencha o email e a senha.', formulario);
            return;
        }

        try {
            CONFIG.UTIL.mostrarCarregamento(botaoSubmit);
            await api.login({ email, senha });
            window.location.href = '/index.html'; // Altere para sua página principal

        } catch (erro) {
            mostrarMensagemErro(erro.message, formulario);

        } finally {
            CONFIG.UTIL.esconderCarregamento(botaoSubmit);
        }
    });
    configurarBotoesDeSenha();
}


// --- LÓGICA DA PÁGINA DE CADASTRO ---

function iniciarPaginaCadastro() {
    const formulario = document.getElementById('formularioCadastro');
    const botaoSubmit = formulario.querySelector('button[type="submit"]');
    
    // Seleciona todos os campos do formulário de cadastro
    const inputNome = document.getElementById('nome');
    const inputEmail = document.getElementById('email');
    const inputTelefone = document.getElementById('telefone');
    const inputDataNascimento = document.getElementById('dataNascimento');
    const inputSenha = document.getElementById('senha');
    const inputConfirmarSenha = document.getElementById('confirmarSenha');
    const inputAceitarTermos = document.getElementById('aceitarTermos');

    formulario.addEventListener('submit', async (evento) => {
        evento.preventDefault();
        removerMensagemErro(formulario);

        // Coleta os valores de todos os campos
        const dadosCadastro = {
            nome: inputNome.value.trim(),
            email: inputEmail.value.trim(),
            telefone: inputTelefone.value.trim(),
            dataNascimento: inputDataNascimento.value,
            senha: inputSenha.value,
        };

        const confirmarSenha = inputConfirmarSenha.value;

        // --- Validações no Frontend ---
        if (!dadosCadastro.nome || !dadosCadastro.email || !dadosCadastro.senha || !confirmarSenha) {
            mostrarMensagemErro('Por favor, preencha todos os campos obrigatórios.', formulario);
            return;
        }
        if (dadosCadastro.senha !== confirmarSenha) {
            mostrarMensagemErro('As senhas não coincidem.', formulario);
            return;
        }
        if (!inputAceitarTermos.checked) {
            mostrarMensagemErro('Você precisa aceitar os Termos de Uso.', formulario);
            return;
        }

        // Se tudo estiver válido, chama a API
        try {
            CONFIG.UTIL.mostrarCarregamento(botaoSubmit);

            // Usa a função da API que criamos a partir do Swagger
            await api.cadastrarUsuario(dadosCadastro);
            
            // Se o cadastro deu certo
            alert('Conta criada com sucesso! Você será redirecionado para a página de login.');
            window.location.href = 'login.html';

        } catch (erro) {
            // Se a API retornar um erro (ex: email já existe)
            mostrarMensagemErro(erro.message, formulario);
        } finally {
            CONFIG.UTIL.esconderCarregamento(botaoSubmit);
        }
    });
    configurarBotoesDeSenha();
}


// --- FUNÇÕES AUXILIARES (usadas por ambas as páginas) ---

function mostrarMensagemErro(mensagem, formulario) {
    const botaoSubmit = formulario.querySelector('button[type="submit"]');
    const divErro = document.createElement('div');
    divErro.className = 'mensagem-erro';
    divErro.textContent = mensagem;
    formulario.insertBefore(divErro, botaoSubmit);
}

function removerMensagemErro(formulario) {
    const mensagemExistente = formulario.querySelector('.mensagem-erro');
    if (mensagemExistente) {
        mensagemExistente.remove();
    }
}

function configurarBotoesDeSenha() {
    const botoes = document.querySelectorAll('.alternar-visibilidade-senha');
    botoes.forEach(botao => {
        // Previne que o mesmo evento seja adicionado múltiplas vezes
        if (botao.dataset.listenerAdicionado) return;

        botao.addEventListener('click', () => {
            const grupoInput = botao.parentElement;
            const inputSenha = grupoInput.querySelector('input');
            const icone = botao.querySelector('i');

            if (inputSenha.type === 'password') {
                inputSenha.type = 'text';
                icone.classList.remove('fa-eye');
                icone.classList.add('fa-eye-slash');
            } else {
                inputSenha.type = 'password';
                icone.classList.remove('fa-eye-slash');
                icone.classList.add('fa-eye');
            }
        });
        botao.dataset.listenerAdicionado = 'true';
    });
}



