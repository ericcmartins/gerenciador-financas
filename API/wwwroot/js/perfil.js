import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) {
        window.location.href = 'login.html';
        return;
    }

    // --- ESTADO DA PÁGINA ---
    const estado = {
        dadosOriginais: null, // Para guardar os dados antes de editar
    };

    // --- SELEÇÃO DE ELEMENTOS ---
    const nomePerfilEl = document.getElementById('nomePerfil');
    const emailPerfilEl = document.getElementById('emailPerfil');
    const formularioPerfil = document.getElementById('formularioPerfil');

    // Inputs do formulário
    const inputNome = document.getElementById('nome');
    const inputEmail = document.getElementById('email');
    const inputTelefone = document.getElementById('telefone');
    const inputDataNascimento = document.getElementById('dataNascimento');
    const todosOsInputs = formularioPerfil.querySelectorAll('input');

    // Botões de ação
    const btnEditarPerfil = document.getElementById('btnEditarPerfil');
    const textoBotaoEditar = document.getElementById('textoBotaoEditar');
    const acoesFormulario = document.getElementById('acoesFormularioPerfil');
    const btnCancelarEdicao = document.getElementById('btnCancelarEdicao');

    // Botões de segurança
    const btnAbrirModalSenha = document.getElementById('btnAbrirModalSenha');
    const btnAbrirModalDelecaoConta = document.getElementById('btnAbrirModalDelecaoConta');


    // --- FUNÇÕES ---

    /**
     * Busca os dados do usuário na API e preenche os campos da página.
     */
    async function carregarDadosPerfil() {
        try {
            const dados = await api.buscarDadosPessoais({ idUsuario });
            estado.dadosOriginais = dados; // Salva os dados originais

            // Preenche o card de cabeçalho
            nomePerfilEl.textContent = dados.nome;
            emailPerfilEl.textContent = dados.email;

            // Preenche os campos do formulário
            inputNome.value = dados.nome;
            inputEmail.value = dados.email;
            inputTelefone.value = dados.telefone || '';
            inputDataNascimento.value = CONFIG.UTIL.formatarDataParaInput(dados.dataNascimento);

        } catch (erro) {
            console.error("Erro ao carregar dados do perfil:", erro);
            alert("Não foi possível carregar suas informações. Tente recarregar a página.");
        }
    }

    /**
     * Alterna entre o modo de visualização e o modo de edição do formulário.
     * @param {boolean} ativar - True para ativar a edição, false para desativar.
     */
    function alternarModoEdicao(ativar = false) {
        if (ativar) {
            // Habilita os campos para edição
            todosOsInputs.forEach(input => input.disabled = false);
            // O email não deve ser editável
            inputEmail.disabled = true;

            acoesFormulario.style.display = 'flex';
            textoBotaoEditar.textContent = 'Visualizando';
            btnEditarPerfil.disabled = true; // Desativa o botão de editar enquanto estiver em modo de edição
        } else {
            // Desabilita os campos e volta aos dados originais
            todosOsInputs.forEach(input => input.disabled = true);

            // Repopula com os dados originais caso o usuário cancele
            if (estado.dadosOriginais) {
                inputNome.value = estado.dadosOriginais.nome;
                inputTelefone.value = estado.dadosOriginais.telefone || '';
                inputDataNascimento.value = CONFIG.UTIL.formatarDataParaInput(estado.dadosOriginais.dataNascimento);
            }

            acoesFormulario.style.display = 'none';
            textoBotaoEditar.textContent = 'Editar';
            btnEditarPerfil.disabled = false;
        }
    }

    /**
     * Lida com a submissão do formulário de perfil para atualizar os dados.
     * @param {Event} evento - O evento de submissão do formulário.
     */
    async function submeterFormularioPerfil(evento) {
        evento.preventDefault();

        const dadosParaAtualizar = {
            nome: inputNome.value.trim(),
            email: inputEmail.value.trim(), // Mesmo que desabilitado, enviamos para a API
            telefone: inputTelefone.value.trim(),
            dataNascimento: inputDataNascimento.value,
            // A API pode requerer a senha para confirmação ou pode não permitir a alteração dela aqui.
            // Por simplicidade, não vamos enviar a senha ao atualizar os dados.
        };

        if (!dadosParaAtualizar.nome) {
            alert('O campo nome é obrigatório.');
            return;
        }

        try {
            await api.atualizarDadosPessoais(dadosParaAtualizar, { idUsuario });
            alert('Perfil atualizado com sucesso!');

            // Sai do modo de edição e recarrega os dados para garantir consistência
            alternarModoEdicao(false);
            await carregarDadosPerfil();

        } catch (erro) {
            console.error("Erro ao atualizar perfil:", erro);
            alert(`Erro ao salvar: ${erro.message}`);
        }
    }

    /**
     * Lida com a exclusão da conta do usuário.
     */
    async function excluirContaUsuario() {
        const confirmacao = window.confirm(
            "Você tem certeza que deseja excluir sua conta?\n\n" +
            "ATENÇÃO: Esta ação é permanente e todos os seus dados (receitas, despesas, contas, etc.) serão perdidos para sempre. Não será possível recuperar sua conta."
        );

        if (!confirmacao) {
            return; // Usuário cancelou a ação
        }

        try {
            // Exibe um feedback visual de que a operação está em andamento
            document.body.style.cursor = 'wait';
            btnAbrirModalDelecaoConta.disabled = true;
            btnAbrirModalDelecaoConta.textContent = 'Excluindo...';

            await api.deletarUsuario({ idUsuario });

            // Executa o logout para limpar o storage local
            api.logout();

            alert('Sua conta foi excluída com sucesso. Sentiremos sua falta!');

            // Redireciona para a página de login
            window.location.href = 'login.html';

        } catch (erro) {
            console.error("Erro ao excluir conta:", erro);
            alert(`Não foi possível excluir sua conta: ${erro.message}`);

            // Restaura o botão ao estado original em caso de erro
            document.body.style.cursor = 'default';
            btnAbrirModalDelecaoConta.disabled = false;
            btnAbrirModalDelecaoConta.innerHTML = '<i class="fas fa-trash"></i> Excluir conta';
        }
    }


    // --- CONFIGURAÇÃO DOS EVENT LISTENERS ---

    function configurarEventListeners() {
        btnEditarPerfil.addEventListener('click', () => alternarModoEdicao(true));
        btnCancelarEdicao.addEventListener('click', () => alternarModoEdicao(false));
        formularioPerfil.addEventListener('submit', submeterFormularioPerfil);

        // Listener para o botão de alterar senha (ainda sem funcionalidade de modal)
        btnAbrirModalSenha.addEventListener('click', () => {
            alert('Funcionalidade de alterar senha ainda não implementada.');
        });

        // Adiciona o listener para o botão de exclusão de conta
        btnAbrirModalDelecaoConta.addEventListener('click', excluirContaUsuario);
    }

    // --- INICIALIZAÇÃO DA PÁGINA ---

    function init() {
        carregarDadosPerfil();
        configurarEventListeners();
    }

    init();
});