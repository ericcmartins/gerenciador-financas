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
        categorias: [],
        categoriaParaDeletarId: null,
    };

    // --- SELEÇÃO DE ELEMENTOS ---
    const corpoTabelaCategorias = document.getElementById('corpoTabelaCategorias');
    const btnNovaCategoria = document.getElementById('btnNovaCategoria');
    const inputBusca = document.getElementById('inputBusca');

    // Modal de Categoria
    const fundoModalCategoria = document.getElementById('fundoModalCategoria');
    const modalCategoria = document.getElementById('modalCategoria');
    const tituloModal = document.getElementById('tituloModal');
    const formularioCategoria = document.getElementById('formularioCategoria');
    const fecharModalCategoriaBtn = document.getElementById('fecharModalCategoria');
    const btnCancelarCategoria = document.getElementById('btnCancelarCategoria');
    const inputCategoriaId = document.getElementById('categoriaId');

    // Modal de Deleção
    const fundoModalDelecao = document.getElementById('fundoModalDelecao');
    const btnConfirmarDelecao = document.getElementById('btnConfirmarDelecao');
    const btnCancelarDelecao = document.getElementById('btnCancelarDelecao');
    const fecharModalDelecaoBtn = document.getElementById('fecharModalDelecao');


    // --- FUNÇÕES DE RENDERIZAÇÃO E DADOS ---

    async function carregarCategorias() {
        corpoTabelaCategorias.innerHTML = `<tr><td colspan="3" class="carregando">Carregando...</td></tr>`;
        try {
            const categoriasDaApi = await api.buscarCategorias({ idUsuario });
            estado.categorias = categoriasDaApi;
            renderizarTabela();
        } catch (erro) {
            console.error('Erro ao carregar categorias:', erro.message);
            corpoTabelaCategorias.innerHTML = `<tr><td colspan="3">Erro ao carregar categorias.</td></tr>`;
        }
    }
    
    function renderizarTabela() {
        corpoTabelaCategorias.innerHTML = '';
        const termoBusca = inputBusca.value.toLowerCase();
        
        const categoriasFiltradas = estado.categorias.filter(cat => 
            cat.nome.toLowerCase().includes(termoBusca) || 
            (cat.descricao && cat.descricao.toLowerCase().includes(termoBusca))
        );

        if (categoriasFiltradas.length === 0) {
            corpoTabelaCategorias.innerHTML = `<tr><td colspan="3" class="estado-vazio-pagina">Nenhuma categoria encontrada.</td></tr>`;
            return;
        }

        categoriasFiltradas.forEach(categoria => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${categoria.nome}</td>
                <td>${categoria.descricao || '-'}</td>
                <td>
                    <div class="botoes-acao">
                        <button class="botao-acao editar" data-id="${categoria.idCategoria}"><i class="fas fa-edit"></i></button>
                        <button class="botao-acao deletar" data-id="${categoria.idCategoria}"><i class="fas fa-trash"></i></button>
                    </div>
                </td>
            `;
            corpoTabelaCategorias.appendChild(tr);
        });
    }

    // --- LÓGICA DO MODAL ---

    function abrirModal(categoria = null) {
        formularioCategoria.reset();
        inputCategoriaId.value = '';
        tituloModal.textContent = 'Nova Categoria';

        if (categoria) { // Modo Edição
            tituloModal.textContent = 'Editar Categoria';
            inputCategoriaId.value = categoria.idCategoria;
            document.getElementById('nomeCategoria').value = categoria.nome;
            document.getElementById('descricaoCategoria').value = categoria.descricao || '';
        }

        fundoModalCategoria.classList.add('ativo');
    }

    function fecharModal() {
        fundoModalCategoria.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const id = inputCategoriaId.value;
        const dadosCorpo = {
            nome: document.getElementById('nomeCategoria').value,
            descricao: document.getElementById('descricaoCategoria').value
        };

        if (!dadosCorpo.nome) {
            alert('O nome da categoria é obrigatório.');
            return;
        }

        try {
            if (id) { // ATUALIZAR
                await api.atualizarCategoria(dadosCorpo, { idUsuario, idCategoria: id });
                alert('Categoria atualizada com sucesso!');
            } else { // CRIAR
                await api.criarCategoria(dadosCorpo, { idUsuario });
                alert('Categoria criada com sucesso!');
            }
            fecharModal();
            carregarCategorias(); // Recarrega a tabela
        } catch (erro) {
            alert(`Erro ao salvar categoria: ${erro.message}`);
        }
    }
    
    function abrirModalDelecao(id) {
        estado.categoriaParaDeletarId = id;
        fundoModalDelecao.classList.add('ativo');
    }

    function fecharModalDelecao() {
        estado.categoriaParaDeletarId = null;
        fundoModalDelecao.classList.remove('ativo');
    }

    async function confirmarDelecao() {
        if (!estado.categoriaParaDeletarId) return;

        try {
            await api.deletarCategoria({ idUsuario, idCategoria: estado.categoriaParaDeletarId });
            alert('Categoria excluída com sucesso.');
            fecharModalDelecao();
            carregarCategorias();
        } catch (erro) {
            alert(`Erro ao excluir: ${erro.message}`);
        }
    }


    // --- CONFIGURAÇÃO DE EVENTOS ---
    
    function configurarEventListeners() {
        btnNovaCategoria.addEventListener('click', () => abrirModal());
        fecharModalCategoriaBtn.addEventListener('click', fecharModal);
        btnCancelarCategoria.addEventListener('click', fecharModal);
        formularioCategoria.addEventListener('submit', submeterFormulario);
        inputBusca.addEventListener('keyup', CONFIG.UTIL.debounce(renderizarTabela, 300));
        
        // Delegação de eventos para botões de ação na tabela
        corpoTabelaCategorias.addEventListener('click', (evento) => {
            const botao = evento.target.closest('.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            const acao = botao.classList.contains('editar') ? 'editar' : 'deletar';

            if (acao === 'editar') {
                const categoria = estado.categorias.find(c => c.idCategoria == id);
                if (categoria) abrirModal(categoria);
            } else if (acao === 'deletar') {
                abrirModalDelecao(id);
            }
        });

        // Eventos do modal de deleção
        fecharModalDelecaoBtn.addEventListener('click', fecharModalDelecao);
        btnCancelarDelecao.addEventListener('click', fecharModalDelecao);
        btnConfirmarDelecao.addEventListener('click', confirmarDelecao);
    }
    
    // --- INICIALIZAÇÃO DA PÁGINA ---
    
    carregarCategorias();
    configurarEventListeners();
});