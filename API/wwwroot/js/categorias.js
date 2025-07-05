import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) return;

    // --- Seleção de Elementos ---
    const btnNovaCategoria = document.getElementById('btnNovaCategoria');
    const corpoTabelaCategorias = document.getElementById('corpoTabelaCategorias');
    const inputBusca = document.getElementById('inputBusca');

    // Elementos do Modal de Categoria (Adicionar/Editar)
    const fundoModalCategoria = document.getElementById('fundoModalCategoria');
    const modalCategoria = document.getElementById('modalCategoria');
    const tituloModalCategoria = modalCategoria.querySelector('#tituloModal'); // Reutiliza o ID 'tituloModal'
    const fecharModalCategoriaBtn = document.getElementById('fecharModalCategoria');
    const btnCancelarCategoria = document.getElementById('btnCancelarCategoria');
    const formularioCategoria = document.getElementById('formularioCategoria');
    const inputCategoriaId = document.getElementById('categoriaId'); // Campo hidden para o ID da categoria
    const inputNomeCategoria = document.getElementById('nomeCategoria');
    const inputDescricaoCategoria = document.getElementById('descricaoCategoria');

    // Elementos do Modal de Deleção
    const fundoModalDelecao = document.getElementById('fundoModalDelecao');
    const fecharModalDelecaoBtn = document.getElementById('fecharModalDelecao');
    const btnCancelarDelecao = document.getElementById('btnCancelarDelecao');
    const btnConfirmarDelecao = document.getElementById('btnConfirmarDelecao');

    let categoriaSendoDeletadaId = null; // Variável para armazenar o ID da categoria a ser deletada

    // --- Funções Auxiliares ---

    /**
     * Valida o formulário de categoria.
     * @param {HTMLFormElement} form - O formulário a ser validado.
     * @returns {boolean} True se o formulário for válido, false caso contrário.
     */
    function validarFormulario(form) {
        limparErrosFormulario(form);
        let isValid = true;
        const camposObrigatorios = form.querySelectorAll('[required]');

        camposObrigatorios.forEach(campo => {
            if (!campo.value.trim()) {
                exibirErroCampo(campo, 'Este campo é obrigatório.');
                isValid = false;
            }
        });
        return isValid;
    }

    /**
     * Exibe uma mensagem de erro abaixo de um campo do formulário.
     * @param {HTMLElement} campo - O campo onde o erro ocorreu.
     * @param {string} mensagem - A mensagem de erro.
     */
    function exibirErroCampo(campo, mensagem) {
        campo.classList.add('campo-erro');
        let erroElement = campo.nextElementSibling;
        if (!erroElement || !erroElement.classList.contains('mensagem-erro')) {
            erroElement = document.createElement('span');
            erroElement.classList.add('mensagem-erro');
            campo.parentNode.insertBefore(erroElement, campo.nextSibling);
        }
        erroElement.textContent = mensagem;
    }

    /**
     * Limpa as mensagens de erro de um formulário.
     * @param {HTMLFormElement} form - O formulário a ser limpo.
     */
    function limparErrosFormulario(form) {
        form.querySelectorAll('.campo-erro').forEach(campo => {
            campo.classList.remove('campo-erro');
        });
        form.querySelectorAll('.mensagem-erro').forEach(erro => {
            erro.remove();
        });
    }

    /**
     * Exibe uma notificação global (sucesso ou erro).
     * @param {string} mensagem - A mensagem a ser exibida.
     * @param {string} tipo - 'sucesso' ou 'erro'.
     */
    function exibirNotificacao(mensagem, tipo) {
        const notificacaoDiv = document.createElement('div');
        notificacaoDiv.classList.add('notificacao', tipo);
        notificacaoDiv.textContent = mensagem;
        document.body.appendChild(notificacaoDiv);

        setTimeout(() => {
            notificacaoDiv.remove();
        }, 3000);
    }

    /**
     * Abre o modal de categoria (para adicionar ou editar).
     * @param {object | null} categoria - Objeto categoria se for edição, null se for nova.
     */
    function abrirModalCategoria(categoria = null) {
        fundoModalCategoria.classList.add('ativo');
        modalCategoria.classList.add('ativo');
        document.body.style.overflow = 'hidden'; // Evita rolagem do corpo

        formularioCategoria.reset();
        limparErrosFormulario(formularioCategoria);
        inputCategoriaId.value = ''; // Limpa o ID para nova categoria

        if (categoria) {
            tituloModalCategoria.textContent = 'Editar Categoria';
            inputCategoriaId.value = categoria.idCategoria; // Supondo que a API retorne idCategoria
            inputNomeCategoria.value = categoria.nome;
            inputDescricaoCategoria.value = categoria.descricao || '';
        } else {
            tituloModalCategoria.textContent = 'Nova Categoria';
        }
    }

    /**
     * Fecha o modal de categoria.
     */
    function fecharModalCategoria() {
        fundoModalCategoria.classList.remove('ativo');
        modalCategoria.classList.remove('ativo');
        document.body.style.overflow = ''; // Restaura a rolagem
        formularioCategoria.reset();
        limparErrosFormulario(formularioCategoria);
    }

    /**
     * Abre o modal de confirmação de deleção.
     * @param {number} id - O ID da categoria a ser deletada.
     */
    function abrirModalDelecao(id) {
        categoriaSendoDeletadaId = id;
        fundoModalDelecao.classList.add('ativo');
        fundoModalDelecao.querySelector('.janela-modal').classList.add('ativo');
        document.body.style.overflow = 'hidden';
    }

    /**
     * Fecha o modal de confirmação de deleção.
     */
    function fecharModalDelecao() {
        categoriaSendoDeletadaId = null;
        fundoModalDelecao.classList.remove('ativo');
        fundoModalDelecao.querySelector('.janela-modal').classList.remove('ativo');
        document.body.style.overflow = '';
    }

    // --- Funções de Carregamento de Dados e Tabela ---

    async function carregarCategorias() {
        corpoTabelaCategorias.innerHTML = `<tr><td colspan="3" class="estado-carregando">Carregando categorias...</td></tr>`;
        try {
            const todasAsCategorias = await api.buscarCategorias({ idUsuario });
            const termoBusca = inputBusca.value.toLowerCase();

            const categoriasFiltradas = todasAsCategorias.filter(categoria =>
                categoria.nome.toLowerCase().includes(termoBusca) ||
                (categoria.descricao && categoria.descricao.toLowerCase().includes(termoBusca))
            );

            corpoTabelaCategorias.innerHTML = ''; // Limpa antes de preencher

            if (categoriasFiltradas.length === 0) {
                corpoTabelaCategorias.innerHTML = `<tr><td colspan="3" class="estado-vazio">Nenhuma categoria encontrada.</td></tr>`;
                return;
            }

            categoriasFiltradas.forEach(categoria => {
                const row = corpoTabelaCategorias.insertRow();
                row.innerHTML = `
                    <td>${categoria.nome}</td>
                    <td>${categoria.descricao || 'N/A'}</td>
                    <td class="acoes">
                        <button class="botao-icone" data-action="editar" data-id="${categoria.idCategoria}"><i class="fas fa-edit"></i></button>
                        <button class="botao-icone" data-action="excluir" data-id="${categoria.idCategoria}"><i class="fas fa-trash-alt"></i></button>
                    </td>
                `;
            });

        } catch (error) {
            console.error('Erro ao carregar categorias:', error.message);
            corpoTabelaCategorias.innerHTML = `<tr><td colspan="3" class="estado-erro">Erro ao carregar categorias.</td></tr>`;
            exibirNotificacao('Erro ao carregar categorias.', 'erro');
        }
    }

    // --- Manipuladores de Eventos ---

    btnNovaCategoria.addEventListener('click', () => abrirModalCategoria());
    fecharModalCategoriaBtn.addEventListener('click', fecharModalCategoria);
    btnCancelarCategoria.addEventListener('click', fecharModalCategoria);

    // Fechar modal de categoria ao clicar fora
    fundoModalCategoria.addEventListener('click', (event) => {
        if (event.target === fundoModalCategoria) {
            fecharModalCategoria();
        }
    });

    // Submissão do formulário de categoria
    formularioCategoria.addEventListener('submit', async (event) => {
        event.preventDefault();
        
        if (!validarFormulario(formularioCategoria)) {
            exibirNotificacao('Por favor, preencha todos os campos obrigatórios corretamente.', 'erro');
            return;
        }

        const dadosCategoria = {
            nome: inputNomeCategoria.value,
            descricao: inputDescricaoCategoria.value
        };

        const params = { idUsuario: idUsuario };

        try {
            const categoriaId = inputCategoriaId.value;
            if (categoriaId) {
                // Atualizar Categoria
                params.idCategoria = parseInt(categoriaId);
                await api.atualizarCategoria(dadosCategoria, params);
                exibirNotificacao('Categoria atualizada com sucesso!', 'sucesso');
            } else {
                // Criar Nova Categoria
                await api.criarCategoria(dadosCategoria, params);
                exibirNotificacao('Categoria criada com sucesso!', 'sucesso');
            }
            fecharModalCategoria();
            carregarCategorias(); // Recarrega a lista após a operação
        } catch (error) {
            console.error('Erro ao salvar categoria:', error.message);
            exibirNotificacao('Erro ao salvar categoria. Verifique os dados.', 'erro');
        }
    });

    // Ações da tabela (editar/excluir)
    corpoTabelaCategorias.addEventListener('click', async (event) => {
        const target = event.target.closest('button');
        if (!target) return;

        const action = target.dataset.action;
        const idCategoria = parseInt(target.dataset.id); 

        if (action === 'editar') {
            try {
                // Para editar, precisamos dos dados completos da categoria.
                // A API de buscarCategorias retorna CategoriaResponseViewModel.
                // Precisamos filtrar a categoria específica pelo ID.
                const todasAsCategorias = await api.buscarCategorias({ idUsuario });
                const categoriaParaEditar = todasAsCategorias.find(c => c.idCategoria === idCategoria);

                if (categoriaParaEditar) {
                    abrirModalCategoria(categoriaParaEditar);
                } else {
                    exibirNotificacao('Categoria não encontrada para edição.', 'erro');
                }
            } catch (error) {
                console.error('Erro ao buscar categoria para edição:', error.message);
                exibirNotificacao('Erro ao carregar dados da categoria para edição.', 'erro');
            }
        } else if (action === 'excluir') {
            abrirModalDelecao(idCategoria);
        }
    });

    // Eventos do modal de deleção
    fecharModalDelecaoBtn.addEventListener('click', fecharModalDelecao);
    btnCancelarDelecao.addEventListener('click', fecharModalDelecao);
    btnConfirmarDelecao.addEventListener('click', async () => {
        if (categoriaSendoDeletadaId) {
            try {
                await api.deletarCategoria({ idUsuario, idCategoria: categoriaSendoDeletadaId });
                exibirNotificacao('Categoria excluída com sucesso!', 'sucesso');
                fecharModalDelecao();
                carregarCategorias(); // Recarrega a lista após a exclusão
            } catch (error) {
                console.error('Erro ao excluir categoria:', error.message);
                exibirNotificacao('Erro ao excluir categoria.', 'erro');
            }
        }
    });

    // Fechar modal de deleção ao clicar fora
    fundoModalDelecao.addEventListener('click', (event) => {
        if (event.target === fundoModalDelecao) {
            fecharModalDelecao();
        }
    });

    // Busca com debounce para evitar muitas requisições
    inputBusca.addEventListener('keyup', CONFIG.UTIL.debounce(carregarCategorias, 300));

    // --- Inicialização ---
    carregarCategorias(); // Carrega as categorias iniciais
});


