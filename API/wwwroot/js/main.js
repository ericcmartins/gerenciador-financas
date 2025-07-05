import { api } from './api.js';

/**
 * Guarda de Autenticação (Auth Guard)
 * Esta função é executada imediatamente para proteger a página.
 * Ela verifica se existe um token de login. Se não houver, redireciona
 * o usuário para a página de login antes que qualquer conteúdo seja mostrado.
 */
(() => {
    // Tenta pegar o token do armazenamento local.
    const token = localStorage.getItem('finon_token');

    // Se não houver token, o usuário não está logado.
    if (!token) {
        // Redireciona para a página de login.
        window.location.href = 'login.html';
    }
})();


/**
 * Adiciona os "ouvintes" de eventos e a lógica principal da página
 * apenas depois que todo o conteúdo HTML foi completamente carregado.
 */
document.addEventListener('DOMContentLoaded', () => {
    // Pega o ID do usuário que foi salvo no login.
    const idUsuario = localStorage.getItem('finon_user_id');

    // Se por algum motivo o ID do usuário não estiver salvo, não podemos continuar.
    if (!idUsuario) {
        console.error('ID do usuário não encontrado. Deslogando...');
        api.logout(); // Limpa qualquer resquício de token
        window.location.href = 'login.html';
        return;
    }

    // Seleciona os elementos comuns do layout
    const elementoNomeUsuario = document.querySelector('.nome-usuario');
    const avatarUsuario = document.getElementById('avatarUsuario');
    const menuUsuario = document.getElementById('menuUsuario');
    const botaoSair = document.getElementById('botaoSair');
    const alternarMenu = document.getElementById('alternarMenu');
    const barraLateral = document.querySelector('.barra-lateral');

    // --- Lógica para carregar os dados do usuário ---
    async function carregarDadosUsuario() {
        try {
            // Chama a API para buscar os dados pessoais
            const dados = await api.buscarDadosPessoais({ idUsuario });
            if (dados && dados.nome) {
                // Atualiza o nome do usuário na barra superior
                elementoNomeUsuario.textContent = dados.nome;
            }
        } catch (erro) {
            console.error('Falha ao carregar dados do usuário:', erro.message);
            // Se falhar (ex: token inválido), desloga o usuário
            api.logout();
            window.location.href = 'login.html';
        }
    }

    // --- Lógica do Menu do Usuário (Dropdown) ---
    avatarUsuario.addEventListener('click', () => {
        menuUsuario.classList.toggle('ativo');
    });

    // Fecha o menu se o usuário clicar fora dele
    document.addEventListener('click', (evento) => {
        if (!avatarUsuario.contains(evento.target) && !menuUsuario.contains(evento.target)) {
            menuUsuario.classList.remove('ativo');
        }
    });

    // --- Lógica do Botão Sair (Logout) ---
    botaoSair.addEventListener('click', (evento) => {
        evento.preventDefault();
        console.log('Saindo...');
        api.logout(); // Limpa o token do localStorage
        window.location.href = 'login.html'; // Redireciona para o login
    });
    
    // --- Lógica para Abrir/Fechar o Menu Lateral (Mobile) ---
    alternarMenu.addEventListener('click', () => {
        barraLateral.classList.toggle('ativo');
    });


    // --- Inicialização ---
    // Assim que a página carrega, busca os dados do usuário para exibir o nome.
    carregarDadosUsuario();
});
