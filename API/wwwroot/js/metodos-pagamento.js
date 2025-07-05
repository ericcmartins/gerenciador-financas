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
        metodos: [],
        contas: [],
        metodoParaDeletarId: null,
    };

    // --- SELEÇÃO DE ELEMENTOS ---
    const corpoTabelaMetodos = document.getElementById('corpoTabelaMetodos');
    const btnNovoMetodo = document.getElementById('btnNovoMetodo');
    const inputBusca = document.getElementById('inputBusca');
    const nomeUsuarioEl = document.querySelector('.nome-usuario');

    // Modal de Adicionar/Editar
    const fundoModalMetodo = document.getElementById('fundoModalMetodo');
    const tituloModal = document.getElementById('tituloModal');
    const formularioMetodo = document.getElementById('formularioMetodo');
    const fecharModalMetodoBtn = document.getElementById('fecharModalMetodo');
    const btnCancelarMetodo = document.getElementById('btnCancelarMetodo');
    const inputMetodoId = document.getElementById('metodoId');
    const selectContaVinculada = document.getElementById('contaVinculada');
    
    // Modal de Deleção
    const fundoModalDelecao = document.getElementById('fundoModalDelecao');
    const btnConfirmarDelecao = document.getElementById('btnConfirmarDelecao');
    const btnCancelarDelecao = document.getElementById('btnCancelarDelecao');
    const fecharModalDelecaoBtn = document.getElementById('fecharModalDelecao');

    
    // --- FUNÇÕES DE RENDERIZAÇÃO E DADOS ---

    /**
     * Carrega o nome do usuário do localStorage e exibe no cabeçalho.
     */
    function carregarInfoUsuario() {
        const nomeUsuario = localStorage.getItem('finon_user_name');
        if (nomeUsuario && nomeUsuarioEl) {
            nomeUsuarioEl.textContent = nomeUsuario;
        }
    }

    /**
     * Carrega todos os métodos de pagamento da API e os exibe na tela.
     */
    async function carregarMetodosPagamento() {
        corpoTabelaMetodos.innerHTML = `<tr><td colspan="6" class="carregando">Carregando...</td></tr>`;

        try {
            const todosOsMetodos = await api.buscarMetodosPagamento({ idUsuario });
            estado.metodos = Array.isArray(todosOsMetodos) ? todosOsMetodos : [];
            renderizarTabela();
        } catch (erro) {
            console.error('Erro ao carregar métodos de pagamento:', erro);
            corpoTabelaMetodos.innerHTML = `<tr><td colspan="6" class="estado-vazio-pagina">Erro ao carregar dados.</td></tr>`;
        }
    }

    /**
     * Renderiza a tabela com base nos dados filtrados do estado.
     */
    function renderizarTabela() {
        const termoBusca = inputBusca.value.toLowerCase();
        const metodosFiltrados = estado.metodos.filter(m => 
            m.nome.toLowerCase().includes(termoBusca) ||
            m.tipo.toLowerCase().includes(termoBusca)
        );

        corpoTabelaMetodos.innerHTML = '';
        if (metodosFiltrados.length === 0) {
            corpoTabelaMetodos.innerHTML = `<tr><td colspan="6" class="estado-vazio-pagina">Nenhum método encontrado.</td></tr>`;
            return;
        }

        metodosFiltrados.forEach(metodo => {
            const conta = estado.contas.find(c => c.idConta === metodo.idConta);
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${metodo.nome}</td>
                <td>${metodo.tipo}</td>
                <td>${conta?.numeroConta || 'N/A'}</td>
                <td>${metodo.limite ? CONFIG.UTIL.formatarMoeda(metodo.limite) : 'N/A'}</td>
                <td>${metodo.descricao || '-'}</td>
                <td>
                    <div class="botoes-acao">
                        <button class="botao-acao editar" data-id="${metodo.idMetodo}"><i class="fas fa-edit"></i></button>
                        <button class="botao-acao deletar" data-id="${metodo.idMetodo}"><i class="fas fa-trash"></i></button>
                    </div>
                </td>
            `;
            corpoTabelaMetodos.appendChild(tr);
        });
    }

    // --- LÓGICA DO MODAL ---

    async function abrirModal(metodo = null) {
        formularioMetodo.reset();
        inputMetodoId.value = '';
        tituloModal.textContent = 'Novo Método de Pagamento';

        // Popula o select de contas
        popularSelect(selectContaVinculada, estado.contas, 'idConta', 'numeroConta', 'Selecione uma conta');

        if (metodo) { // Modo Edição
            tituloModal.textContent = 'Editar Método de Pagamento';
            inputMetodoId.value = metodo.idMetodo;
            document.getElementById('nomeMetodo').value = metodo.nome;
            document.getElementById('tipoMetodo').value = metodo.tipo;
            document.getElementById('limiteMetodo').value = metodo.limite || '';
            document.getElementById('descricaoMetodo').value = metodo.descricao || '';
            
            if (metodo.idConta) {
                selectContaVinculada.value = metodo.idConta;
            }
        }

        fundoModalMetodo.classList.add('ativo');
    }

    function fecharModal() {
        fundoModalMetodo.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const id = inputMetodoId.value;
        
        const dadosCorpo = {
            nome: document.getElementById('nomeMetodo').value.trim(),
            tipo: document.getElementById('tipoMetodo').value,
            limite: document.getElementById('limiteMetodo').value ? parseFloat(document.getElementById('limiteMetodo').value) : null,
            descricao: document.getElementById('descricaoMetodo').value.trim(),
        };

        const dadosParams = {
            idUsuario,
            idConta: selectContaVinculada.value
        };

        if (!dadosCorpo.nome || !dadosCorpo.tipo || !dadosParams.idConta) {
            alert('Por favor, preencha todos os campos obrigatórios (*).');
            return;
        }

        try {
            if (id) { // ATUALIZAR
                dadosParams.idMetodoPagamento = id;
                await api.atualizarMetodoPagamento(dadosCorpo, dadosParams);
                alert('Método atualizado com sucesso!');
            } else { // CRIAR
                await api.criarMetodoPagamento(dadosCorpo, dadosParams);
                alert('Método criado com sucesso!');
            }
            fecharModal();
            carregarMetodosPagamento();
        } catch (erro) {
            console.error("Erro ao salvar método:", erro);
            alert(`Erro ao salvar: ${erro.message}`);
        }
    }

    function abrirModalDelecao(id) {
        estado.metodoParaDeletarId = id;
        fundoModalDelecao.classList.add('ativo');
    }

    function fecharModalDelecao() {
        estado.metodoParaDeletarId = null;
        fundoModalDelecao.classList.remove('ativo');
    }

    async function confirmarDelecao() {
        if (!estado.metodoParaDeletarId) return;

        try {
            await api.deletarMetodoPagamento({ idUsuario, idMetodoPagamento: estado.metodoParaDeletarId });
            alert('Método de pagamento excluído com sucesso.');
            fecharModalDelecao();
            carregarMetodosPagamento();
        } catch(erro) {
            alert(`Erro ao excluir: ${erro.message}`);
        }
    }


    // --- FUNÇÕES AUXILIARES ---
    
    function popularSelect(elemento, dados, valorKey, textoKey, placeholder) {
        elemento.innerHTML = `<option value="">${placeholder}</option>`;
        if (Array.isArray(dados)) {
            dados.forEach(item => {
                elemento.innerHTML += `<option value="${item[valorKey]}">${item[textoKey]}</option>`;
            });
        }
    }

    // --- CONFIGURAÇÃO DE EVENTOS ---

    function configurarEventListeners() {
        btnNovoMetodo.addEventListener('click', () => abrirModal());
        fecharModalMetodoBtn.addEventListener('click', fecharModal);
        btnCancelarMetodo.addEventListener('click', fecharModal);
        formularioMetodo.addEventListener('submit', submeterFormulario);
        inputBusca.addEventListener('keyup', CONFIG.UTIL.debounce(renderizarTabela, 300));

        // Delegação de eventos para a tabela
        corpoTabelaMetodos.addEventListener('click', (evento) => {
            const botao = evento.target.closest('.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            const acao = botao.classList.contains('editar') ? 'editar' : 'deletar';

            if (acao === 'editar') {
                const metodo = estado.metodos.find(m => m.idMetodo == id);
                if (metodo) abrirModal(metodo);
            } else if (acao === 'deletar') {
                abrirModalDelecao(id);
            }
        });

        // Listeners do modal de deleção
        fecharModalDelecaoBtn.addEventListener('click', fecharModalDelecao);
        btnCancelarDelecao.addEventListener('click', fecharModalDelecao);
        btnConfirmarDelecao.addEventListener('click', confirmarDelecao);
    }
    
    // --- INICIALIZAÇÃO DA PÁGINA ---
    
    async function init() {
        carregarInfoUsuario();
        
        // É crucial carregar as contas antes, para que os nomes possam ser exibidos
        try {
            const contasDaApi = await api.buscarContas({ idUsuario });
            estado.contas = Array.isArray(contasDaApi) ? contasDaApi : [];
        } catch (erro) {
            console.error("Erro fatal ao carregar contas:", erro);
            alert("Não foi possível carregar suas contas. A página pode não funcionar corretamente.");
        }
        
        await carregarMetodosPagamento();
        configurarEventListeners();
    }

    init();
});
